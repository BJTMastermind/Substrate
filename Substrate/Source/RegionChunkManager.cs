namespace Substrate;

using System.Collections;
using Substrate.Core;

/// <summary>
/// Represents a Beta-compatible interface for globally managing chunks.
/// </summary>
public class RegionChunkManager : IChunkManager, IEnumerable<ChunkRef> {
    private const int REGION_XLEN = 32;
    private const int REGION_ZLEN = 32;

    private const int REGION_XLOG = 5;
    private const int REGION_ZLOG = 5;

    private const int REGION_XMASK = 0x1F;
    private const int REGION_ZMASK = 0x1F;

    private IRegionManager regionMan;

    private ChunkCache cache;

    /// <summary>
    /// Creates a new <see cref="RegionChunkManager"/> instance given a backing <see cref="RegionManager"/> and <see cref="ChunkCache"/>.
    /// </summary>
    /// <param name="rm">A <see cref="RegionManager"/> exposing access to regions.</param>
    /// <param name="cache">A shared cache for storing chunks read in.</param>
    public RegionChunkManager(IRegionManager rm, ChunkCache cache) {
        this.regionMan = rm;
        this.cache = cache;
    }

    /// <summary>
    /// Creates a new <see cref="RegionChunkManager"/> instance from another.
    /// </summary>
    /// <param name="cm">A <see cref="RegionChunkManager"/> to get a <see cref="RegionManager"/> and <see cref="ChunkCache"/> from.</param>
    public RegionChunkManager(RegionChunkManager cm) {
        this.regionMan = cm.regionMan;
        this.cache = cm.cache;
    }

    /// <summary>
    /// Gets the <see cref="RegionManager"/> backing this manager.
    /// </summary>
    public IRegionManager RegionManager {
        get { return this.regionMan; }
    }

    #region IChunkContainer
    /// <inheritdoc/>
    public int ChunkGlobalX(int cx) {
        return cx;
    }

    /// <inheritdoc/>
    public int ChunkGlobalZ(int cz) {
        return cz;
    }

    /// <inheritdoc/>
    public int ChunkLocalX(int cx) {
        return cx & REGION_XMASK;
    }

    /// <inheritdoc/>
    public int ChunkLocalZ(int cz) {
        return cz & REGION_ZMASK;
    }

    /// <inheritdoc/>
    public IChunk GetChunk(int cx, int cz) {
        IRegion r = GetRegion(cx, cz);
        if(r == null) {
            return null;
        }
        return r.GetChunk(cx & REGION_XMASK, cz & REGION_ZMASK);
    }

    /// <inheritdoc/>
    public ChunkRef GetChunkRef(int cx, int cz) {
        IRegion r = GetRegion(cx, cz);
        if(r == null) {
            return null;
        }
        return r.GetChunkRef(cx & REGION_XMASK, cz & REGION_ZMASK);
    }

    /// <inheritdoc/>
    public bool ChunkExists(int cx, int cz) {
        IRegion r = GetRegion(cx, cz);
        if(r == null) {
            return false;
        }
        return r.ChunkExists(cx & REGION_XMASK, cz & REGION_ZMASK);
    }

    /// <inheritdoc/>
    public ChunkRef CreateChunk(int cx, int cz) {
        IRegion r = GetRegion(cx, cz);
        if(r == null) {
            int rx = cx >> REGION_XLOG;
            int rz = cz >> REGION_ZLOG;
            r = this.regionMan.CreateRegion(rx, rz);
        }
        return r.CreateChunk(cx & REGION_XMASK, cz & REGION_ZMASK);
    }

    /// <inheritdoc/>
    public ChunkRef SetChunk(int cx, int cz, IChunk chunk) {
        IRegion r = GetRegion(cx, cz);
        if(r == null) {
            int rx = cx >> REGION_XLOG;
            int rz = cz >> REGION_ZLOG;
            r = this.regionMan.CreateRegion(rx, rz);
        }

        chunk.SetLocation(cx, cz);
        r.SaveChunk(chunk);

        return r.GetChunkRef(cx & REGION_XMASK, cz & REGION_ZMASK);
    }

    /// <inheritdoc/>
    public int Save() {
        this.cache.SyncDirty();

        int saved = 0;
        IEnumerator<ChunkRef> en = this.cache.GetDirtyEnumerator();
        while(en.MoveNext()) {
            ChunkRef chunk = en.Current;

            IRegion r = GetRegion(chunk.X, chunk.Z);
            if(r == null) {
                continue;
            }
            using(Stream chunkOutStream = r.GetChunkOutStream(chunk.LocalX, chunk.LocalZ)) {
                chunk.Save(chunkOutStream);
                saved++;
            }
        }
        this.cache.ClearDirty();
        return saved;
    }

    /// <inheritdoc/>
    public bool SaveChunk(IChunk chunk) {
        IRegion r = GetRegion(chunk.X, chunk.Z);
        if(r == null) {
            return false;
        }
        return r.SaveChunk(chunk);
    }

    /// <inheritdoc/>
    public bool DeleteChunk(int cx, int cz) {
        IRegion r = GetRegion(cx, cz);
        if(r == null) {
            return false;
        }

        if(!r.DeleteChunk(cx & REGION_XMASK, cz & REGION_ZMASK)) {
            return false;
        }

        if(r.ChunkCount() == 0) {
            this.regionMan.DeleteRegion(r.X, r.Z);
        }
        return true;
    }

    /// <inheritdoc/>
    public bool CanDelegateCoordinates {
        get { return true; }
    }
    #endregion

    /// <summary>
    /// Copies a chunk from one location to another.
    /// </summary>
    /// <param name="srccx">The global X-coordinate of the source chunk.</param>
    /// <param name="srccz">The global Z-coordinate of the source chunk.</param>
    /// <param name="dstcx">The global X-coordinate of the destination chunk.</param>
    /// <param name="dstcz">The global Z-coordinate of the destination chunk.</param>
    /// <returns>A <see cref="ChunkRef"/> for the destination chunk.</returns>
    public ChunkRef CopyChunk(int srccx, int srccz, int dstcx, int dstcz) {
        IRegion srcr = GetRegion(srccx, srccz);
        if(srcr == null) {
            return null;
        }

        IRegion dstr = GetRegion(dstcx, dstcz);
        if(dstr == null) {
            int rx = dstcx >> REGION_XLOG;
            int rz = dstcz >> REGION_ZLOG;
            dstr = this.regionMan.CreateRegion(rx, rz);
        }

        IChunk c = srcr.GetChunk(srccx & REGION_XMASK, srccz & REGION_ZMASK);
        c.SetLocation(dstcx, dstcz);

        dstr.SaveChunk(c);

        return dstr.GetChunkRef(dstcx & REGION_XMASK, dstcz & REGION_ZMASK);
    }

    /// <summary>
    /// Performs a full chunk relight sequence on all modified chunks.
    /// </summary>
    public void RelightDirtyChunks() {
        //List<ChunkRef> dirty = new List<ChunkRef>();
        Dictionary<ChunkKey, ChunkRef> dirty = new Dictionary<ChunkKey, ChunkRef>();

        this.cache.SyncDirty();

        IEnumerator<ChunkRef> en = this.cache.GetDirtyEnumerator();
        while(en.MoveNext()) {
            ChunkKey key = new ChunkKey(en.Current.X, en.Current.Z);
            dirty[key] = en.Current;
        }

        foreach(ChunkRef chunk in dirty.Values) {
            chunk.Blocks.ResetBlockLight();
            chunk.Blocks.ResetSkyLight();
        }

        foreach(ChunkRef chunk in dirty.Values) {
            chunk.Blocks.RebuildBlockLight();
            chunk.Blocks.RebuildSkyLight();
        }

        foreach(ChunkRef chunk in dirty.Values) {
            if(!dirty.ContainsKey(new ChunkKey(chunk.X, chunk.Z - 1))) {
                ChunkRef east = chunk.GetEastNeighbor();
                chunk.Blocks.StitchBlockLight(east.Blocks, BlockCollectionEdge.EAST);
                chunk.Blocks.StitchSkyLight(east.Blocks, BlockCollectionEdge.EAST);
            }

            if(!dirty.ContainsKey(new ChunkKey(chunk.X, chunk.Z + 1))) {
                ChunkRef west = chunk.GetWestNeighbor();
                chunk.Blocks.StitchBlockLight(west.Blocks, BlockCollectionEdge.WEST);
                chunk.Blocks.StitchSkyLight(west.Blocks, BlockCollectionEdge.WEST);
            }

            if(!dirty.ContainsKey(new ChunkKey(chunk.X - 1, chunk.Z))) {
                ChunkRef north = chunk.GetNorthNeighbor();
                chunk.Blocks.StitchBlockLight(north.Blocks, BlockCollectionEdge.NORTH);
                chunk.Blocks.StitchSkyLight(north.Blocks, BlockCollectionEdge.NORTH);
            }

            if(!dirty.ContainsKey(new ChunkKey(chunk.X + 1, chunk.Z))) {
                ChunkRef south = chunk.GetSouthNeighbor();
                chunk.Blocks.StitchBlockLight(south.Blocks, BlockCollectionEdge.SOUTH);
                chunk.Blocks.StitchSkyLight(south.Blocks, BlockCollectionEdge.SOUTH);
            }
        }
    }

    /// <summary>
    /// Gets the timestamp of the chunk from its underlying region file.
    /// </summary>
    /// <param name="cx">The global X-coordinate of a chunk.</param>
    /// <param name="cz">The global Z-coordinate of a chunk.</param>
    /// <returns>The timestamp of the chunk from its underlying region file.</returns>
    /// <remarks>The value returned may differ from any timestamp stored in the chunk data itself.</remarks>
    public int GetChunkTimestamp(int cx, int cz) {
        IRegion r = GetRegion(cx, cz);
        if(r == null) {
            return 0;
        }

        return r.GetChunkTimestamp(cx & REGION_XMASK, cz & REGION_ZMASK);
    }

    /// <summary>
    /// Sets the timestamp of the chunk in its underlying region file.
    /// </summary>
    /// <param name="cx">The global X-coordinate of a chunk.</param>
    /// <param name="cz">The global Z-coordinate of a chunk.</param>
    /// <param name="timestamp">The new timestamp value.</param>
    /// <remarks>This function will only update the timestamp of the chunk slot in the underlying region file.  It will not update
    /// any timestamp information in the chunk data itself.</remarks>
    public void SetChunkTimestamp(int cx, int cz, int timestamp) {
        IRegion r = GetRegion(cx, cz);
        if(r == null) {
            return;
        }

        r.SetChunkTimestamp(cx & REGION_XMASK, cz & REGION_ZMASK, timestamp);
    }

    private ChunkRef GetChunkRefInRegion(IRegion r, int lcx, int lcz) {
        int cx = r.X * REGION_XLEN + lcx;
        int cz = r.Z * REGION_ZLEN + lcz;
        return GetChunkRef(cx, cz);
    }

    private IRegion GetRegion(int cx, int cz) {
        cx >>= REGION_XLOG;
        cz >>= REGION_ZLOG;
        return this.regionMan.GetRegion(cx, cz);
    }

    #region IEnumerable<ChunkRef> Members
    /// <summary>
    /// Returns an enumerator that iterates through all chunks in all regions of the world.
    /// </summary>
    /// <returns>An enumerator for this manager.</returns>
    public IEnumerator<ChunkRef> GetEnumerator() {
        return new Enumerator(this);
    }
    #endregion

    #region IEnumerable Members
    /// <inheritdoc/>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
        return new Enumerator(this);
    }
    #endregion

    private class Enumerator : IEnumerator<ChunkRef> {
        private RegionChunkManager cm;

        private IEnumerator<IRegion> enum_;
        private IRegion region;
        private ChunkRef chunk;

        private int x = 0;
        private int z = -1;

        public Enumerator(RegionChunkManager cm) {
            this.cm = cm;
            this.enum_ = this.cm.RegionManager.GetEnumerator();
            if(this.enum_.MoveNext()) {
                this.region = this.enum_.Current;
            }
        }

        public virtual bool MoveNext() {
            if(this.region == null) {
                return false;
            }

            if(this.enum_ == null) {
                return MoveNextInRegion();
            } else {
                while(true) {
                    if(this.x >= RegionChunkManager.REGION_XLEN) {
                        if(!this.enum_.MoveNext()) {
                            return false;
                        }
                        this.x = 0;
                        this.z = -1;
                        this.region = this.enum_.Current;
                    }
                    if(MoveNextInRegion()) {
                        this.chunk = this.region.GetChunkRef(this.x, this.z);
                        return true;
                    }
                }
            }
        }

        protected bool MoveNextInRegion() {
            for(; this.x < RegionChunkManager.REGION_XLEN; this.x++) {
                for(this.z++; this.z < RegionChunkManager.REGION_ZLEN; this.z++) {
                    if(this.region.ChunkExists(this.x, this.z)) {
                        goto FoundNext;
                    }
                }
                this.z = -1;
            }
            FoundNext:
            return (this.x < RegionChunkManager.REGION_XLEN);
        }

        public void Reset() {
            if(this.enum_ != null) {
                this.enum_.Reset();
                this.enum_.MoveNext();
                this.region = this.enum_.Current;
            }
            this.x = 0;
            this.z = -1;
        }

        void IDisposable.Dispose() { }

        object IEnumerator.Current {
            get { return Current; }
        }

        ChunkRef IEnumerator<ChunkRef>.Current {
            get { return Current; }
        }

        public ChunkRef Current {
            get {
                if(this.x >= RegionChunkManager.REGION_XLEN) {
                    throw new InvalidOperationException();
                }
                return this.chunk;
            }
        }
    }
}

public class MissingChunkException : Exception { }
