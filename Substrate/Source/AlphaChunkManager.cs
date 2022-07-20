namespace Substrate;

using System.Collections;
using System.Text.RegularExpressions;
using Substrate.Core;
using Substrate.Nbt;

/// <summary>
/// Represents an Alpha-compatible interface for globally managing chunks.
/// </summary>
public class AlphaChunkManager : IChunkManager, IEnumerable<ChunkRef> {
    private string mapPath;

    //protected Dictionary<ChunkKey, WeakReference> cache;
    private LRUCache<ChunkKey, ChunkRef> cache;
    private Dictionary<ChunkKey, ChunkRef> dirty;

    /// <summary>
    /// Gets the path to the base directory containing the chunk directory structure.
    /// </summary>
    public string ChunkPath {
        get { return this.mapPath; }
    }

    /// <summary>
    /// Creates a new <see cref="AlphaChunkManager"/> instance for the give chunk base directory.
    /// </summary>
    /// <param name="mapDir">The path to the chunk base directory.</param>
    public AlphaChunkManager(string mapDir) {
        this.mapPath = mapDir;
        this.cache = new LRUCache<ChunkKey, ChunkRef>(256);
        this.dirty = new Dictionary<ChunkKey, ChunkRef>();
    }

    private ChunkFile GetChunkFile(int cx, int cz) {
        return new ChunkFile(this.mapPath, cx, cz);
    }

    private NbtTree GetChunkTree(int cx, int cz) {
        ChunkFile cf = GetChunkFile(cx, cz);
        using(Stream nbtstr = cf.GetDataInputStream()) {
            if(nbtstr == null) {
                return null;
            }

            return new NbtTree(nbtstr);
        }
    }

    private bool SaveChunkTree(int cx, int cz, NbtTree tree) {
        ChunkFile cf = GetChunkFile(cx, cz);
        using(Stream zipstr = cf.GetDataOutputStream()) {
            if(zipstr == null) {
                return false;
            }

            tree.WriteTo(zipstr);
        }

        return true;
    }

    private Stream GetChunkOutStream(int cx, int cz) {
        return new ChunkFile(this.mapPath, cx, cz).GetDataOutputStream();
    }

    #region IChunkContainer Members
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
        return cx;
    }

    /// <inheritdoc/>
    public int ChunkLocalZ(int cz) {
        return cz;
    }

    /// <inheritdoc/>
    public IChunk GetChunk(int cx, int cz) {
        if(!ChunkExists(cx, cz)) {
            return null;
        }

        return AlphaChunk.CreateVerified(GetChunkTree(cx, cz));
    }

    /// <inheritdoc/>
    public ChunkRef GetChunkRef(int cx, int cz) {
        ChunkKey k = new ChunkKey(cx, cz);
        ChunkRef c = null;

        //WeakReference chunkref = null;
        if(this.cache.TryGetValue(k, out c)) {
            return c;
        }

        c = ChunkRef.Create(this, cx, cz);
        if(c != null) {
            this.cache[k] = c;
        }

        return c;
    }

    /// <inheritdoc/>
    public ChunkRef CreateChunk(int cx, int cz) {
        DeleteChunk(cx, cz);
        AlphaChunk chunk = AlphaChunk.Create(cx, cz);

        using(Stream chunkOutStream = GetChunkOutStream(cx, cz)) {
            chunk.Save(chunkOutStream);
        }

        ChunkRef cr = ChunkRef.Create(this, cx, cz);
        ChunkKey k = new ChunkKey(cx, cz);
        this.cache[k] = cr;

        return cr;
    }

    /// <inheritdoc/>
    public bool ChunkExists(int cx, int cz) {
        return new ChunkFile(this.mapPath, cx, cz).Exists();
    }

    /// <inheritdoc/>
    public bool DeleteChunk(int cx, int cz) {
        new ChunkFile(this.mapPath, cx, cz).Delete();

        ChunkKey k = new ChunkKey(cx, cz);
        this.cache.Remove(k);
        this.dirty.Remove(k);

        return true;
    }

    /// <inheritdoc/>
    public ChunkRef SetChunk(int cx, int cz, IChunk chunk) {
        DeleteChunk(cx, cz);
        chunk.SetLocation(cx, cz);
        using(Stream chunkOutStream = GetChunkOutStream(cx, cz)) {
            chunk.Save(chunkOutStream);
        }

        ChunkRef cr = ChunkRef.Create(this, cx, cz);
        ChunkKey k = new ChunkKey(cx, cz);
        this.cache[k] = cr;

        return cr;
    }

    /// <inheritdoc/>
    public int Save() {
        foreach(KeyValuePair<ChunkKey, ChunkRef> e in this.cache) {
            if(e.Value.IsDirty) {
                this.dirty[e.Key] = e.Value;
            }
        }

        int saved = 0;
        foreach(ChunkRef chunkRef in this.dirty.Values) {
            int cx = ChunkGlobalX(chunkRef.X);
            int cz = ChunkGlobalZ(chunkRef.Z);

            using(Stream chunkOutStream = GetChunkOutStream(cx, cz)) {
                if(chunkRef.Save(chunkOutStream)) {
                    saved++;
                }
            }
        }

        this.dirty.Clear();
        return saved;
    }

    /// <inheritdoc/>
    public bool SaveChunk(IChunk chunk) {
        using(Stream chunkOutStream = GetChunkOutStream(ChunkGlobalX(chunk.X), ChunkGlobalZ(chunk.Z))) {
            if(chunk.Save(chunkOutStream)) {
                this.dirty.Remove(new ChunkKey(chunk.X, chunk.Z));
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc/>
    public bool CanDelegateCoordinates {
        get { return true; }
    }
    #endregion

    /// <summary>
    /// Gets the (last modified) timestamp of the underlying chunk file.
    /// </summary>
    /// <param name="cx">The global X-coordinate of a chunk.</param>
    /// <param name="cz">The global Z-coordinate of a chunk.</param>
    /// <returns>The last modified timestamp of the underlying chunk file.</returns>
    public int GetChunkTimestamp(int cx, int cz) {
        ChunkFile cf = GetChunkFile(cx, cz);
        if(cf == null) {
            return 0;
        }

        return cf.GetModifiedTime();
    }

    #region IEnumerable<ChunkRef> Members
    /// <summary>
    /// Gets an enumerator that iterates through all the chunks in the world.
    /// </summary>
    /// <returns>An enumerator for this manager.</returns>
    public IEnumerator<ChunkRef> GetEnumerator() {
        return new Enumerator(this);
    }
    #endregion

    #region IEnumerable Members
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
        return new Enumerator(this);
    }
    #endregion

    private class Enumerator : IEnumerator<ChunkRef> {
        protected AlphaChunkManager cm;
        protected Queue<string> tld;
        protected Queue<string> sld;
        protected Queue<ChunkRef> chunks;

        private string curtld;
        private string cursld;
        private ChunkRef curchunk;

        public Enumerator(AlphaChunkManager cfm) {
            this.cm = cfm;

            if(!Directory.Exists(this.cm.ChunkPath)) {
                throw new DirectoryNotFoundException();
            }

            Reset();
        }

        private bool MoveNextTLD() {
            if(this.tld.Count == 0) {
                return false;
            }

            this.curtld = this.tld.Dequeue();

            //string path = Path.Combine(cm.ChunkPath, curtld);

            string[] files = Directory.GetDirectories(this.curtld);
            foreach(string file in files) {
                this.sld.Enqueue(file);
            }

            return true;
        }

        public bool MoveNextSLD() {
            while(this.sld.Count == 0) {
                if(MoveNextTLD() == false) {
                    return false;
                }
            }

            this.cursld = this.sld.Dequeue();

            //string path = Path.Combine(cm.ChunkPath, curtld);
            //path = Path.Combine(path, cursld);

            string[] files = Directory.GetFiles(this.cursld);
            foreach(string file in files) {
                int x;
                int z;

                string basename = Path.GetFileName(file);

                if(!ParseFileName(basename, out x, out z)) {
                    continue;
                }

                ChunkRef cref = this.cm.GetChunkRef(x, z);
                if(cref != null) {
                    this.chunks.Enqueue(cref);
                }
            }

            return true;
        }

        public bool MoveNext() {
            while(this.chunks.Count == 0) {
                if(MoveNextSLD() == false) {
                    return false;
                }
            }

            this.curchunk = this.chunks.Dequeue();
            return true;
        }

        public void Reset() {
            this.curchunk = null;

            this.tld = new Queue<string>();
            this.sld = new Queue<string>();
            this.chunks = new Queue<ChunkRef>();

            string[] files = Directory.GetDirectories(this.cm.ChunkPath);
            foreach(string file in files) {
                this.tld.Enqueue(file);
            }
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
                if(this.curchunk == null) {
                    throw new InvalidOperationException();
                }
                return this.curchunk;
            }
        }

        private bool ParseFileName(string filename, out int x, out int z) {
            x = 0;
            z = 0;

            Match match = namePattern.Match(filename);
            if(!match.Success) {
                return false;
            }

            x = (int) Base36.Decode(match.Groups[1].Value);
            z = (int) Base36.Decode(match.Groups[2].Value);
            return true;
        }

        protected static Regex namePattern = new Regex("c\\.(-?[0-9a-zA-Z]+)\\.(-?[0-9a-zA-Z]+)\\.dat$");
    }
}
