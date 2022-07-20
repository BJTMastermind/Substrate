namespace Substrate;

using Substrate.Core;

/// <summary>
/// A single Alpha-compatible block with context-independent data.
/// </summary>
/// <remarks><para>In general, you should prefer other types for accessing block data including <see cref="AlphaBlockRef"/>,
/// <see cref="BlockManager"/>, and the <see cref="AlphaBlockCollection"/> property of <see cref="IChunk"/> and <see cref="ChunkRef"/>.</para>
/// <para>You should use the <see cref="AlphaBlock"/> type when you need to copy individual blocks into a custom collection or
/// container, and context-depdendent data such as coordinates and lighting have no well-defined meaning.  <see cref="AlphaBlock"/>
/// offers a relatively compact footprint for storing the unique identity of a block's manifestation in the world.</para>
/// <para>A single <see cref="AlphaBlock"/> object may also provide a convenient way to paste a block into many locations in
/// a block collection type.</para></remarks>
public class AlphaBlock : IDataBlock, IPropertyBlock, IActiveBlock, ICopyable<AlphaBlock> {
    private int id;
    private int data;
    private TileEntity tileEntity;
    private TileTick tileTick;

    /// <summary>
    /// Create a new <see cref="AlphaBlock"/> instance of the given type with default data.
    /// </summary>
    /// <param name="id">The id (type) of the block.</param>
    /// <remarks>If the specified block type requires a Tile Entity as part of its definition, a default
    /// <see cref="TileEntity"/> of the appropriate type will automatically be created.</remarks>
    public AlphaBlock(int id) {
        this.id = id;
        UpdateTileEntity(0, id);
    }

    /// <summary>
    /// Create a new <see cref="AlphaBlock"/> instance of the given type and data value.
    /// </summary>
    /// <param name="id">The id (type) of the block.</param>
    /// <param name="data">The block's supplementary data value, currently limited to the range [0-15].</param>
    /// <remarks>If the specified block type requires a Tile Entity as part of its definition, a default
    /// <see cref="TileEntity"/> of the appropriate type will automatically be created.</remarks>
    public AlphaBlock(int id, int data) {
        this.id = id;
        this.data = data;
        UpdateTileEntity(0, id);
    }

    /// <summary>
    /// Crrates a new <see cref="AlphaBlock"/> from a given block in an existing <see cref="AlphaBlockCollection"/>.
    /// </summary>
    /// <param name="chunk">The block collection to reference.</param>
    /// <param name="lx">The local X-coordinate of a block within the collection.</param>
    /// <param name="ly">The local Y-coordinate of a block within the collection.</param>
    /// <param name="lz">The local Z-coordinate of a block within the collection.</param>
    public AlphaBlock(AlphaBlockCollection chunk, int lx, int ly, int lz) {
        this.id = chunk.GetID(lx, ly, lz);
        this.data = chunk.GetData(lx, ly, lz);

        TileEntity te = chunk.GetTileEntity(lx, ly, lz);
        if(te != null) {
            this.tileEntity = te.Copy();
        }
    }

    #region IBlock Members
    /// <summary>
    /// Gets information on the type of the block.
    /// </summary>
    public BlockInfo Info {
        get { return BlockInfo.BlockTable[this.id]; }
    }

    /// <summary>
    /// Gets or sets the id (type) of the block.
    /// </summary>
    /// <remarks>If the new or old type have non-matching Tile Entity requirements, the embedded Tile Entity data
    /// will be updated to keep consistent with the new block type.</remarks>
    public int ID {
        get { return this.id; }
        set {
            UpdateTileEntity(this.id, value);
            this.id = value;
        }
    }
    #endregion

    #region IDataBlock Members
    /// <summary>
    /// Gets or sets the supplementary data value of the block.
    /// </summary>
    public int Data {
        get { return this.data; }
        set {
            /*if(BlockManager.EnforceDataLimits && BlockInfo.BlockTable[id] != null) {
                if(!BlockInfo.BlockTable[id].TestData(value)) {
                    return;
                }
            }*/
            this.data = value;
        }
    }
    #endregion

    #region IPropertyBlock Members
    /// <summary>
    /// Gets the Tile Entity record of the block if it has one.
    /// </summary>
    /// <returns>The <see cref="TileEntity"/> attached to this block, or null if the block type does not require a Tile Entity.</returns>
    public TileEntity GetTileEntity() {
        return this.tileEntity;
    }

    /// <summary>
    /// Sets a new Tile Entity record for the block.
    /// </summary>
    /// <param name="te">A Tile Entity record compatible with the block's type.</param>
    /// <exception cref="ArgumentException">Thrown when an incompatible <see cref="TileEntity"/> is added to a block.</exception>
    /// <exception cref="InvalidOperationException">Thrown when a <see cref="TileEntity"/> is added to a block that does not use tile entities.</exception>
    public void SetTileEntity(TileEntity te) {
        BlockInfoEx info = BlockInfo.BlockTable[this.id] as BlockInfoEx;
        if(info == null) {
            throw new InvalidOperationException("The current block type does not accept a Tile Entity");
        }

        if(te.GetType() != TileEntityFactory.Lookup(info.TileEntityName)) {
            throw new ArgumentException("The current block type is not compatible with the given Tile Entity", "te");
        }

        this.tileEntity = te;
    }

    /// <summary>
    /// Creates a default Tile Entity record appropriate for the block.
    /// </summary>
    public void CreateTileEntity() {
        BlockInfoEx info = BlockInfo.BlockTable[this.id] as BlockInfoEx;
        if(info == null) {
            throw new InvalidOperationException("The given block is of a type that does not support TileEntities.");
        }

        TileEntity te = TileEntityFactory.Create(info.TileEntityName);
        if(te == null) {
            throw new UnknownTileEntityException("The TileEntity type '" + info.TileEntityName + "' has not been registered with the TileEntityFactory.");
        }

        this.tileEntity = te;
    }

    /// <summary>
    /// Removes any Tile Entity currently attached to the block.
    /// </summary>
    public void ClearTileEntity() {
        this.tileEntity = null;
    }
    #endregion

    #region IActiveBlock Members
    public int TileTickValue {
        get {
            if(this.tileTick == null) {
                return 0;
            }

            return this.tileTick.Ticks;
        }

        set {
            if(this.tileTick == null) {
                CreateTileTick();
            }

            this.tileTick.Ticks = value;
        }
    }

    /// <summary>
    /// Gets the <see cref="TileTick"/> record of the block if it has one.
    /// </summary>
    /// <returns>The <see cref="TileTick"/> attached to this block, or null if the block type does not require a Tile Entity.</returns>
    public TileTick GetTileTick() {
        return this.tileTick;
    }

    /// <summary>
    /// Sets a new <see cref="TileTick"/> record for the block.
    /// </summary>
    /// <param name="tt">A <see cref="TileTick"/> record compatible with the block's type.</param>
    public void SetTileTick(TileTick tt) {
        this.tileTick = tt;
    }

    /// <summary>
    /// Creates a default <see cref="TileTick"/> record appropriate for the block.
    /// </summary>
    public void CreateTileTick() {
        this.tileTick = new TileTick() {
            ID = id,
        };
    }

    /// <summary>
    /// Removes any <see cref="TileTick"/> currently attached to the block.
    /// </summary>
    public void ClearTileTick() {
        this.tileTick = null;
    }
    #endregion

    #region ICopyable<Block> Members
    /// <summary>
    /// Creates a deep copy of the <see cref="AlphaBlock"/>.
    /// </summary>
    /// <returns>A new <see cref="AlphaBlock"/> representing the same data.</returns>
    public AlphaBlock Copy() {
        AlphaBlock block = new AlphaBlock(this.id, this.data);
        if(this.tileEntity != null) {
            block.tileEntity = this.tileEntity.Copy();
        }

        return block;
    }
    #endregion

    private void UpdateTileEntity(int old, int value) {
        BlockInfoEx info1 = BlockInfo.BlockTable[old] as BlockInfoEx;
        BlockInfoEx info2 = BlockInfo.BlockTable[value] as BlockInfoEx;

        if(info1 != info2) {
            if(info1 != null) {
                this.tileEntity = null;
            }

            if(info2 != null) {
                this.tileEntity = TileEntityFactory.Create(info2.TileEntityName);
            }
        }
    }
}
