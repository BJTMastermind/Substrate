namespace Substrate;

using Substrate.Core;

public class AlphaBlockManager : BlockManager {
    public AlphaBlockManager(IChunkManager cm) : base(cm) {
        IChunk c = AlphaChunk.Create(0, 0);

        this.chunkXDim = c.Blocks.XDim;
        this.chunkYDim = c.Blocks.YDim;
        this.chunkZDim = c.Blocks.ZDim;
        this.chunkXMask = this.chunkXDim - 1;
        this.chunkYMask = this.chunkYDim - 1;
        this.chunkZMask = this.chunkZDim - 1;
        this.chunkXLog = Log2(this.chunkXDim);
        this.chunkYLog = Log2(this.chunkYDim);
        this.chunkZLog = Log2(this.chunkZDim);
    }
}

public class AnvilBlockManager : BlockManager {
    public AnvilBlockManager(IChunkManager cm) : base(cm) {
        IChunk c = AnvilChunk.Create(0, 0);

        this.chunkXDim = c.Blocks.XDim;
        this.chunkYDim = c.Blocks.YDim;
        this.chunkZDim = c.Blocks.ZDim;
        this.chunkXMask = this.chunkXDim - 1;
        this.chunkYMask = this.chunkYDim - 1;
        this.chunkZMask = this.chunkZDim - 1;
        this.chunkXLog = Log2(this.chunkXDim);
        this.chunkYLog = Log2(this.chunkYDim);
        this.chunkZLog = Log2(this.chunkZDim);
    }
}

/// <summary>
/// Represents an Alpha-compatible interface for globally managing blocks.
/// </summary>
public abstract class BlockManager : IVersion10BlockManager, IBlockManager {
    public const int MIN_X = -32000000;
    public const int MAX_X = 32000000;
    public const int MIN_Y = 0;
    public const int MAX_Y = 256;
    public const int MIN_Z = -32000000;
    public const int MAX_Z = 32000000;

    protected int chunkXDim;
    protected int chunkYDim;
    protected int chunkZDim;
    protected int chunkXMask;
    protected int chunkYMask;
    protected int chunkZMask;
    protected int chunkXLog;
    protected int chunkYLog;
    protected int chunkZLog;

    protected IChunkManager chunkMan;

    protected ChunkRef cache;

    private bool autoLight = true;
    private bool autoFluid = false;
    private bool autoTileTick = false;

    /// <summary>
    /// Gets or sets a value indicating whether changes to blocks will trigger automatic lighting updates.
    /// </summary>
    public bool AutoLight {
        get { return this.autoLight; }
        set { this.autoLight = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether changes to blocks will trigger automatic fluid updates.
    /// </summary>
    public bool AutoFluid {
        get { return this.autoFluid; }
        set { this.autoFluid = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether changes to blocks will trigger automatic fluid updates.
    /// </summary>
    public bool AutoTileTick {
        get { return this.autoTileTick; }
        set { this.autoTileTick = value; }
    }

    /// <summary>
    /// Constructs a new <see cref="BlockManager"/> instance on top of the given <see cref="IChunkManager"/>.
    /// </summary>
    /// <param name="cm">An <see cref="IChunkManager"/> instance.</param>
    public BlockManager(IChunkManager cm) {
        this.chunkMan = cm;
    }

    /// <summary>
    /// Returns a new <see cref="AlphaBlock"/> object from global coordinates.
    /// </summary>
    /// <param name="x">Global X-coordinate of block.</param>
    /// <param name="y">Global Y-coordinate of block.</param>
    /// <param name="z">Global Z-coordiante of block.</param>
    /// <returns>A new <see cref="AlphaBlock"/> object representing context-independent data of a single block.</returns>
    /// <remarks>Context-independent data excludes data such as lighting.  <see cref="AlphaBlock"/> object actually contain a copy
    /// of the data they represent, so changes to the <see cref="AlphaBlock"/> will not affect this container, and vice-versa.</remarks>
    public AlphaBlock GetBlock(int x, int y, int z) {
        this.cache = GetChunk(x, y, z);
        if(this.cache == null || !Check(x, y, z)) {
            return null;
        }

        return this.cache.Blocks.GetBlock(x & this.chunkXMask, y & this.chunkYMask, z & this.chunkZMask);
    }

    /// <summary>
    /// Returns a new <see cref="AlphaBlockRef"/> object from global coordaintes.
    /// </summary>
    /// <param name="x">Global X-coordinate of block.</param>
    /// <param name="y">Global Y-coordinate of block.</param>
    /// <param name="z">Global Z-coordinate of block.</param>
    /// <returns>A new <see cref="AlphaBlockRef"/> object representing context-dependent data of a single block.</returns>
    /// <remarks>Context-depdendent data includes all data associated with this block.  Since a <see cref="AlphaBlockRef"/> represents
    /// a view of a block within this container, any updates to data in the container will be reflected in the <see cref="AlphaBlockRef"/>,
    /// and vice-versa for updates to the <see cref="AlphaBlockRef"/>.</remarks>
    public AlphaBlockRef GetBlockRef(int x, int y, int z) {
        this.cache = GetChunk(x, y, z);
        if(this.cache == null || !Check(x, y, z)) {
            return new AlphaBlockRef();
        }

        return this.cache.Blocks.GetBlockRef(x & this.chunkXMask, y & this.chunkYMask, z & this.chunkZMask);
    }

    /// <summary>
    /// Updates a block with values from a <see cref="AlphaBlock"/> object.
    /// </summary>
    /// <param name="x">Global X-coordinate of a block.</param>
    /// <param name="y">Global Y-coordinate of a block.</param>
    /// <param name="z">Global Z-coordinate of a block.</param>
    /// <param name="block">A <see cref="AlphaBlock"/> object to copy block data from.</param>
    public void SetBlock(int x, int y, int z, AlphaBlock block) {
        this.cache = GetChunk(x, y, z);
        if(this.cache == null || !Check(x, y, z)) {
            return;
        }

        this.cache.Blocks.SetBlock(x & this.chunkXMask, y & this.chunkYMask, z & this.chunkZMask, block);
    }

    /// <summary>
    /// Gets a reference object to a single chunk given global coordinates to a block within that chunk.
    /// </summary>
    /// <param name="x">Global X-coordinate of a block.</param>
    /// <param name="y">Global Y-coordinate of a block.</param>
    /// <param name="z">Global Z-coordinate of a block.</param>
    /// <returns>A <see cref="ChunkRef"/> to a single chunk containing the given block.</returns>
    public ChunkRef GetChunk(int x, int y, int z) {
        x >>= this.chunkXLog;
        z >>= this.chunkZLog;
        return this.chunkMan.GetChunkRef(x, z);
    }

    protected int Log2(int x) {
        int c = 0;
        while(x > 1) {
            x >>= 1;
            c++;
        }
        return c;
    }

    /// <summary>
    /// Called by other block-specific 'get' and 'set' functions to filter
    /// out operations on some blocks.  Override this method in derrived
    /// classes to filter the entire BlockManager.
    /// </summary>
    protected virtual bool Check(int x, int y, int z) {
        return (x >= MIN_X) && (x < MAX_X) &&
            (y >= MIN_Y) && (y < MAX_Y) &&
            (z >= MIN_Z) && (z < MAX_Z);
    }

    #region IBlockContainer Members
    IBlock IBlockCollection.GetBlock(int x, int y, int z) {
        return GetBlock(x, y, z);
    }

    IBlock IBlockCollection.GetBlockRef(int x, int y, int z) {
        return GetBlockRef(x, y, z);
    }

    /// <inheritdoc/>
    public void SetBlock(int x, int y, int z, IBlock block) {
        this.cache = GetChunk(x, y, z);
        if(this.cache == null || !Check(x, y, z)) {
            return;
        }

        this.cache.Blocks.SetBlock(x, y, z, block);
    }

    /// <inheritdoc/>
    public BlockInfo GetInfo(int x, int y, int z) {
        this.cache = GetChunk(x, y, z);
        if(this.cache == null || !Check(x, y, z)) {
            return null;
        }

        return this.cache.Blocks.GetInfo(x & this.chunkXMask, y & this.chunkYMask, z & this.chunkZMask);
    }

    /// <inheritdoc/>
    public int GetID(int x, int y, int z) {
        this.cache = GetChunk(x, y, z);
        if(this.cache == null) {
            return 0;
        }

        return this.cache.Blocks.GetID(x & this.chunkXMask, y & this.chunkYMask, z & this.chunkZMask);
    }

    /// <inheritdoc/>
    public void SetID(int x, int y, int z, int id) {
        this.cache = GetChunk(x, y, z);
        if(this.cache == null || !Check(x, y, z)) {
            return;
        }

        bool autolight = this.cache.Blocks.AutoLight;
        bool autofluid = this.cache.Blocks.AutoFluid;
        bool autoTileTick = this.cache.Blocks.AutoTileTick;

        this.cache.Blocks.AutoLight = this.autoLight;
        this.cache.Blocks.AutoFluid = this.autoFluid;
        this.cache.Blocks.AutoTileTick = this.autoTileTick;

        this.cache.Blocks.SetID(x & this.chunkXMask, y & this.chunkYMask, z & this.chunkZMask, id);

        this.cache.Blocks.AutoFluid = autofluid;
        this.cache.Blocks.AutoLight = autolight;
        this.cache.Blocks.AutoTileTick = autoTileTick;
    }
    #endregion

    #region IDataBlockCollection Members
    IDataBlock IDataBlockCollection.GetBlock(int x, int y, int z) {
        return GetBlock(x, y, z);
    }

    IDataBlock IDataBlockCollection.GetBlockRef(int x, int y, int z) {
        return GetBlockRef(x, y, z);
    }

    /// <inheritdoc/>
    public void SetBlock(int x, int y, int z, IDataBlock block) {
        this.cache = GetChunk(x, y, z);
        if(this.cache == null || !Check(x, y, z)) {
            return;
        }

        this.cache.Blocks.SetBlock(x, y, z, block);
    }

    /// <inheritdoc/>
    public int GetData(int x, int y, int z) {
        this.cache = GetChunk(x, y, z);
        if(this.cache == null) {
            return 0;
        }

        return this.cache.Blocks.GetData(x & this.chunkXMask, y & this.chunkYMask, z & this.chunkZMask);
    }

    /// <inheritdoc/>
    public void SetData(int x, int y, int z, int data) {
        this.cache = GetChunk(x, y, z);
        if(this.cache == null || !Check(x, y, z)) {
            return;
        }

        this.cache.Blocks.SetData(x & this.chunkXMask, y & this.chunkYMask, z & this.chunkZMask, data);
    }
    #endregion


    #region ILitBlockContainer Members
    ILitBlock ILitBlockCollection.GetBlock(int x, int y, int z) {
        throw new NotImplementedException();
    }

    ILitBlock ILitBlockCollection.GetBlockRef(int x, int y, int z) {
        return GetBlockRef(x, y, z);
    }

    /// <inheritdoc/>
    public void SetBlock(int x, int y, int z, ILitBlock block) {
        this.cache = GetChunk(x, y, z);
        if(this.cache == null || !Check(x, y, z)) {
            return;
        }

        this.cache.Blocks.SetBlock(x, y, z, block);
    }

    /// <inheritdoc/>
    public int GetBlockLight(int x, int y, int z) {
        this.cache = GetChunk(x, y, z);
        if(this.cache == null) {
            return 0;
        }

        return this.cache.Blocks.GetBlockLight(x & this.chunkXMask, y & this.chunkYMask, z & this.chunkZMask);
    }

    /// <inheritdoc/>
    public int GetSkyLight(int x, int y, int z) {
        this.cache = GetChunk(x, y, z);
        if(this.cache == null) {
            return 0;
        }

        return this.cache.Blocks.GetSkyLight(x & this.chunkXMask, y & this.chunkYMask, z & this.chunkZMask);
    }

    /// <inheritdoc/>
    public void SetBlockLight(int x, int y, int z, int light) {
        this.cache = GetChunk(x, y, z);
        if(this.cache == null || !Check(x, y, z)) {
            return;
        }

        this.cache.Blocks.SetBlockLight(x & this.chunkXMask, y & this.chunkYMask, z & this.chunkZMask, light);
    }

    /// <inheritdoc/>
    public void SetSkyLight(int x, int y, int z, int light) {
        this.cache = GetChunk(x, y, z);
        if(this.cache == null || !Check(x, y, z)) {
            return;
        }

        this.cache.Blocks.SetSkyLight(x & this.chunkXMask, y & this.chunkYMask, z & this.chunkZMask, light);
    }

    /// <inheritdoc/>
    public int GetHeight(int x, int z) {
        this.cache = GetChunk(x, 0, z);
        if(this.cache == null || !Check(x, 0, z)) {
            return 0;
        }

        return this.cache.Blocks.GetHeight(x & this.chunkXMask, z & this.chunkZMask);
    }

    /// <inheritdoc/>
    public void SetHeight(int x, int z, int height) {
        this.cache = GetChunk(x, 0, z);
        if(this.cache == null || !Check(x, 0, z)) {
            return;
        }

        this.cache.Blocks.SetHeight(x & this.chunkXMask, z & this.chunkZMask, height);
    }

    /// <inheritdoc/>
    public void UpdateBlockLight(int x, int y, int z) {
        this.cache = GetChunk(x, y, z);
        if(this.cache == null || !Check(x, y, z)) {
            return;
        }

        this.cache.Blocks.UpdateBlockLight(x & this.chunkXMask, y & this.chunkYMask, z & this.chunkZMask);
    }

    /// <inheritdoc/>
    public void UpdateSkyLight(int x, int y, int z) {
        this.cache = GetChunk(x, y, z);
        if(this.cache == null || !Check(x, y, z)) {
            return;
        }

        this.cache.Blocks.UpdateBlockLight(x & this.chunkXMask, y & this.chunkYMask, z & this.chunkZMask);
    }
    #endregion

    #region IPropertyBlockContainer Members
    IPropertyBlock IPropertyBlockCollection.GetBlock(int x, int y, int z) {
        return GetBlock(x, y, z);
    }

    IPropertyBlock IPropertyBlockCollection.GetBlockRef(int x, int y, int z) {
        return GetBlockRef(x, y, z);
    }

    /// <inheritdoc/>
    public void SetBlock(int x, int y, int z, IPropertyBlock block) {
        this.cache = GetChunk(x, y, z);
        if(this.cache == null || !Check(x, y, z)) {
            return;
        }

        this.cache.Blocks.SetBlock(x, y, z, block);
    }

    /// <inheritdoc/>
    public TileEntity GetTileEntity(int x, int y, int z) {
        this.cache = GetChunk(x, y, z);
        if(this.cache == null || !Check(x, y, z)) {
            return null;
        }

        return this.cache.Blocks.GetTileEntity(x & this.chunkXMask, y & this.chunkYMask, z & this.chunkZMask);
    }

    /// <inheritdoc/>
    public void SetTileEntity(int x, int y, int z, TileEntity te) {
        this.cache = GetChunk(x, y, z);
        if(this.cache == null || !Check(x, y, z)) {
            return;
        }

        this.cache.Blocks.SetTileEntity(x & this.chunkXMask, y & this.chunkYMask, z & this.chunkZMask, te);
    }

    /// <inheritdoc/>
    public void CreateTileEntity(int x, int y, int z) {
        this.cache = GetChunk(x, y, z);
        if(this.cache == null || !Check(x, y, z)) {
            return;
        }

        this.cache.Blocks.CreateTileEntity(x & this.chunkXMask, y & this.chunkYMask, z & this.chunkZMask);
    }

    /// <inheritdoc/>
    public void ClearTileEntity(int x, int y, int z) {
        this.cache = GetChunk(x, y, z);
        if(this.cache == null || !Check(x, y, z)) {
            return;
        }

        this.cache.Blocks.ClearTileEntity(x & this.chunkXMask, y & this.chunkYMask, z & this.chunkZMask);
    }
    #endregion

    #region IActiveBlockContainer Members
    IActiveBlock IActiveBlockCollection.GetBlock(int x, int y, int z) {
        return GetBlock(x, y, z);
    }

    IActiveBlock IActiveBlockCollection.GetBlockRef(int x, int y, int z) {
        return GetBlockRef(x, y, z);
    }

    /// <inheritdoc/>
    public void SetBlock(int x, int y, int z, IActiveBlock block) {
        this.cache = GetChunk(x, y, z);
        if(this.cache == null || !Check(x, y, z)) {
            return;
        }

        this.cache.Blocks.SetBlock(x, y, z, block);
    }

    /// <inheritdoc/>
    public int GetTileTickValue(int x, int y, int z) {
        this.cache = GetChunk(x, y, z);
        if(this.cache == null || !Check(x, y, z)) {
            return 0;
        }

        return this.cache.Blocks.GetTileTickValue(x & this.chunkXMask, y & this.chunkYMask, z & this.chunkZMask);
    }

    /// <inheritdoc/>
    public void SetTileTickValue(int x, int y, int z, int tickValue) {
        this.cache = GetChunk(x, y, z);
        if(this.cache == null || !Check(x, y, z)) {
            return;
        }

        this.cache.Blocks.SetTileTickValue(x & this.chunkXMask, y & this.chunkYMask, z & this.chunkZMask, tickValue);
    }

    /// <inheritdoc/>
    public TileTick GetTileTick(int x, int y, int z) {
        this.cache = GetChunk(x, y, z);
        if(this.cache == null || !Check(x, y, z)) {
            return null;
        }

        return this.cache.Blocks.GetTileTick(x & this.chunkXMask, y & this.chunkYMask, z & this.chunkZMask);
    }

    /// <inheritdoc/>
    public void SetTileTick(int x, int y, int z, TileTick te) {
        this.cache = GetChunk(x, y, z);
        if(this.cache == null || !Check(x, y, z)) {
            return;
        }

        this.cache.Blocks.SetTileTick(x & this.chunkXMask, y & this.chunkYMask, z & this.chunkZMask, te);
    }

    /// <inheritdoc/>
    public void CreateTileTick(int x, int y, int z) {
        this.cache = GetChunk(x, y, z);
        if(this.cache == null || !Check(x, y, z)) {
            return;
        }

        this.cache.Blocks.CreateTileTick(x & this.chunkXMask, y & this.chunkYMask, z & this.chunkZMask);
    }

    /// <inheritdoc/>
    public void ClearTileTick(int x, int y, int z) {
        this.cache = GetChunk(x, y, z);
        if(this.cache == null || !Check(x, y, z)) {
            return;
        }

        this.cache.Blocks.ClearTileTick(x & this.chunkXMask, y & this.chunkYMask, z & this.chunkZMask);
    }
    #endregion
}
