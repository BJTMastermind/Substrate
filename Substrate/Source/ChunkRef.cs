namespace Substrate;

using Substrate.Core;

/// <summary>
/// Provides a wrapper around a physical Chunk stored in a chunk container.
/// </summary>
/// <remarks>
/// Modifying data in a ChunkRef will signal to the chunk container that the physical chunk needs to be saved.
/// </remarks>
public class ChunkRef : IChunk {
    private IChunkContainer container;
    private IChunk chunk;

    private AlphaBlockCollection blocks;
    private AnvilBiomeCollection biomes;
    private EntityCollection entities;

    private int cx;
    private int cz;

    private bool dirty;

    /// <summary>
    /// Gets the global X-coordinate of the chunk.
    /// </summary>
    public int X {
        get { return this.container.ChunkGlobalX(this.cx); }
    }

    /// <summary>
    /// Gets the global Z-coordinate of the chunk.
    /// </summary>
    public int Z {
        get { return this.container.ChunkGlobalZ(this.cz); }
    }

    /// <summary>
    /// Gets the local X-coordinate of the chunk within container.
    /// </summary>
    public int LocalX {
        get { return this.container.ChunkLocalX(this.cx); }
    }

    /// <summary>
    /// Gets the local Z-coordinate of the chunk within container.
    /// </summary>
    public int LocalZ {
        get { return this.container.ChunkLocalZ(this.cz); }
    }

    /// <summary>
    /// Gets the collection of all blocks and their data stored in the chunk.
    /// </summary>
    public AlphaBlockCollection Blocks {
        get {
            if(this.blocks == null) {
                GetChunk();
            }
            return this.blocks;
        }
    }

    /// <summary>
    /// Gets the collection of all blocks and their data stored in the chunk.
    /// </summary>
    public AnvilBiomeCollection Biomes {
        get {
            if(this.biomes == null) {
                GetChunk();
            }
            return this.biomes;
        }
    }

    /// <summary>
    /// Gets the collection of all entities stored in the chunk.
    /// </summary>
    public EntityCollection Entities {
        get {
            if(this.entities == null) {
                GetChunk();
            }
            return this.entities;
        }
    }

    /// <summary>
    /// Gets or sets the value indicating that the chunk has been modified, but not saved.
    /// </summary>
    public bool IsDirty {
        get {
            return this.dirty
                || (this.blocks != null && this.blocks.IsDirty)
                || (this.entities != null && this.entities.IsDirty);
        }

        set {
            this.dirty = value;
            if(this.blocks != null) {
                this.blocks.IsDirty = false;
            }

            if(this.entities != null) {
                this.entities.IsDirty = false;
            }
        }
    }

    /// <summary>
    /// Forbid direct instantiation of ChunkRef objects
    /// </summary>
    private ChunkRef() { }

    /// <summary>
    /// Create a reference to a chunk stored in a chunk container.
    /// </summary>
    /// <param name="container">Chunk container</param>
    /// <param name="cx">Local X-coordinate of chunk within container.</param>
    /// <param name="cz">Local Z-coordinate of chunk within container.</param>
    /// <returns>ChunkRef representing a reference to a physical chunk at the specified location within the container.</returns>
    public static ChunkRef Create(IChunkContainer container, int cx, int cz) {
        if(!container.ChunkExists(cx, cz)) {
            return null;
        }

        ChunkRef c = new ChunkRef();

        c.container = container;
        c.cx = cx;
        c.cz = cz;

        return c;
    }

    /// <summary>
    /// Gets or sets the chunk's TerrainPopulated status.
    /// </summary>
    public bool IsTerrainPopulated {
        get { return GetChunk().IsTerrainPopulated; }
        set {
            if(GetChunk().IsTerrainPopulated != value) {
                GetChunk().IsTerrainPopulated = value;
                this.dirty = true;
            }
        }
    }

    /// <summary>
    /// Saves the underlying physical chunk to the specified output stream.
    /// </summary>
    /// <param name="outStream">An open output stream.</param>
    /// <returns>A value indicating whether the chunk is no longer considered dirty.</returns>
    public bool Save(Stream outStream) {
        if(IsDirty) {
            if(GetChunk().Save(outStream)) {
                IsDirty = false;
                return true;
            }
            return false;
        }
        return true;
    }

    public void SetLocation(int x, int z) {
        int relX = LocalX + (x - X);
        int relZ = LocalZ + (z - Z);

        ChunkRef c = this.container.SetChunk(relX, relZ, GetChunk());

        this.container = c.container;
        this.cx = c.cx;
        this.cz = c.cz;
    }

    /// <summary>
    /// Gets a ChunkRef to the chunk positioned immediately north (X - 1).
    /// </summary>
    /// <returns>ChunkRef to the northern neighboring chunk.</returns>
    public ChunkRef GetNorthNeighbor() {
        return this.container.GetChunkRef(this.cx - 1, this.cz);
    }

    /// <summary>
    /// Gets a ChunkRef to the chunk positioned immediately south (X + 1).
    /// </summary>
    /// <returns>ChunkRef to the southern neighboring chunk.</returns>
    public ChunkRef GetSouthNeighbor() {
        return this.container.GetChunkRef(this.cx + 1, this.cz);
    }

    /// <summary>
    /// Gets a ChunkRef to the chunk positioned immediatly east (Z - 1).
    /// </summary>
    /// <returns>ChunkRef to the eastern neighboring chunk.</returns>
    public ChunkRef GetEastNeighbor() {
        return this.container.GetChunkRef(this.cx, this.cz - 1);
    }

    /// <summary>
    /// Gets a ChunkRef to the chunk positioned immedately west (Z + 1).
    /// </summary>
    /// <returns>ChunkRef to the western neighboring chunk.</returns>
    public ChunkRef GetWestNeighbor() {
        return this.container.GetChunkRef(this.cx, this.cz + 1);
    }

    /// <summary>
    /// Returns a deep copy of the physical chunk underlying the ChunkRef.
    /// </summary>
    /// <returns>A copy of the physical Chunk object.</returns>
    /*public Chunk GetChunkCopy() {
        return GetChunk().Copy();
    }*/

    /// <summary>
    /// Returns the reference of the physical chunk underlying the ChunkRef, and releases the reference from itself.
    /// </summary>
    /// <remarks>
    /// This function returns the reference to the chunk stored in the chunk container.  Because the ChunkRef simultaneously gives up
    /// its "ownership" of the Chunk, the container will not consider the Chunk dirty even if it is modified.  Attempting to use the ChunkRef after
    /// releasing its internal reference will query the container for a new reference.  If the chunk is still cached, it will get the same reference
    /// back, otherwise it will get an independent copy.  Chunks should only be taken from ChunkRefs to transfer them to another ChunkRef, or
    /// to modify them without intending to permanently store the changes.
    /// </remarks>
    /// <returns>The physical Chunk object underlying the ChunkRef</returns>
    public IChunk GetChunkRef() {
        IChunk chunk = GetChunk();
        this.chunk = null;
        this.dirty = false;

        return chunk;
    }

    /// <summary>
    /// Replaces the underlying physical chunk with a different one, updating its physical location to reflect the ChunkRef.
    /// </summary>
    /// <remarks>
    /// Use this function to save chunks that have been created or manipulated independently of a container, or to
    /// move a physical chunk between locations within a container (by taking the reference from another ChunkRef).
    /// </remarks>
    /// <param name="chunk">Physical Chunk to store into the location represented by this ChunkRef.</param>
    public void SetChunkRef(IChunk chunk) {
        this.chunk = chunk;
        this.chunk.SetLocation(X, Z);
        this.dirty = true;
    }

    /// <summary>
    /// Gets an internal Chunk reference from cache or queries the container for it.
    /// </summary>
    /// <returns>The ChunkRef's underlying Chunk.</returns>
    private IChunk GetChunk() {
        if(this.chunk == null) {
            this.chunk = this.container.GetChunk(this.cx, this.cz);

            if(this.chunk != null) {
                this.blocks = this.chunk.Blocks;
                this.biomes = this.chunk.Biomes;
                this.entities = this.chunk.Entities;

                // Set callback functions in the underlying block collection
                this.blocks.ResolveNeighbor += ResolveNeighborHandler;
                this.blocks.TranslateCoordinates += TranslateCoordinatesHandler;
            }
        }
        return this.chunk;
    }

    /// <summary>
    /// Callback function to return the block collection of a ChunkRef at a relative offset to this one.
    /// </summary>
    /// <param name="relx">Relative offset from the X-coordinate.</param>
    /// <param name="rely">Relative offset from the Y-coordinate.</param>
    /// <param name="relz">Relative offset from the Z-coordinate.</param>
    /// <returns>Another ChunkRef's underlying block collection, or null if the ChunkRef cannot be found.</returns>
    private AlphaBlockCollection ResolveNeighborHandler(int relx, int rely, int relz) {
        ChunkRef cr = this.container.GetChunkRef(this.cx + relx, this.cz + relz);
        if(cr != null) {
            return cr.Blocks;
        }

        return null;
    }

    /// <summary>
    /// Translates chunk-local block coordinates to corresponding global coordinates.
    /// </summary>
    /// <param name="lx">Chunk-local X-coordinate.</param>
    /// <param name="ly">Chunk-local Y-coordinate.</param>
    /// <param name="lz">Chunk-local Z-coordinate.</param>
    /// <returns>BlockKey containing the global block coordinates.</returns>
    private BlockKey TranslateCoordinatesHandler(int lx, int ly, int lz) {
        int x = X * this.blocks.XDim + lx;
        int z = Z * this.blocks.ZDim + lz;

        return new BlockKey(x, ly, z);
    }
}
