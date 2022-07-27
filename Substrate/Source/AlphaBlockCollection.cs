namespace Substrate;

using Substrate.Core;
using Substrate.Nbt;

/// <summary>
/// Functions for reading and modifying a bounded-size collection of Alpha-compatible block data.
/// </summary>
/// <remarks>An <see cref="AlphaBlockCollection"/> is a wrapper around existing pieces of data.  Although it
/// holds references to data, it does not "own" the data in the same way that a <see cref="IChunk"/> does.  An
/// <see cref="AlphaBlockCollection"/> simply overlays a higher-level interface on top of existing data.</remarks>
public class AlphaBlockCollection : IBoundedAlphaBlockCollection, IBoundedActiveBlockCollection {
    private readonly int xdim;
    private readonly int ydim;
    private readonly int zdim;

    private IDataArray3 blocks;
    private IDataArray3 data;
    private IDataArray3 blockLight;
    private IDataArray3 skyLight;

    private IDataArray2 heightMap;

    private TagNodeList tileEntities;
    private TagNodeList tileTicks;

    private BlockLight lightManager;
    private BlockFluid fluidManager;
    private BlockTileEntities tileEntityManager;
    private BlockTileTicks tileTickManager;

    private bool dirty = false;
    private bool autoLight = true;
    private bool autoFluid = false;
    private bool autoTick = false;

    public delegate AlphaBlockCollection NeighborLookupHandler(int relx, int rely, int relz);

    /// <summary>
    /// Creates a new <see cref="AlphaBlockCollection"/> of a given dimension.
    /// </summary>
    /// <param name="xdim">The length of the X-dimension of the collection.</param>
    /// <param name="ydim">The length of the Y-dimension of the collection.</param>
    /// <param name="zdim">The length of the Z-dimension of the collection.</param>
    [Obsolete]
    public AlphaBlockCollection(int xdim, int ydim, int zdim) {
        this.blocks = new XZYByteArray(xdim, ydim, zdim);
        this.data = new XZYNibbleArray(xdim, ydim, zdim);
        this.blockLight = new XZYNibbleArray(xdim, ydim, zdim);
        this.skyLight = new XZYNibbleArray(xdim, ydim, zdim);
        this.heightMap = new ZXByteArray(xdim, zdim);
        this.tileEntities = new TagNodeList(TagType.TAG_COMPOUND);
        this.tileTicks = new TagNodeList(TagType.TAG_COMPOUND);

        this.xdim = xdim;
        this.ydim = ydim;
        this.zdim = zdim;

        Refresh();
    }

    /// <summary>
    /// Creates a new <see cref="AlphaBlockCollection"/> overlay on top of Alpha-specific units of data.
    /// </summary>
    /// <param name="blocks">An array of Block IDs.</param>
    /// <param name="data">An array of data nibbles.</param>
    /// <param name="blockLight">An array of block light nibbles.</param>
    /// <param name="skyLight">An array of sky light nibbles.</param>
    /// <param name="heightMap">An array of height map values.</param>
    /// <param name="tileEntities">A list of tile entities corresponding to blocks in this collection.</param>
    public AlphaBlockCollection(IDataArray3 blocks, IDataArray3 data, IDataArray3 blockLight, IDataArray3 skyLight,
      IDataArray2 heightMap, TagNodeList tileEntities) : this(blocks, data, blockLight, skyLight, heightMap, tileEntities, null) { }

    /// <summary>
    /// Creates a new <see cref="AlphaBlockCollection"/> overlay on top of Alpha-specific units of data.
    /// </summary>
    /// <param name="blocks">An array of Block IDs.</param>
    /// <param name="data">An array of data nibbles.</param>
    /// <param name="blockLight">An array of block light nibbles.</param>
    /// <param name="skyLight">An array of sky light nibbles.</param>
    /// <param name="heightMap">An array of height map values.</param>
    /// <param name="tileEntities">A list of tile entities corresponding to blocks in this collection.</param>
    /// <param name="tileTicks">A list of tile ticks corresponding to blocks in this collection.</param>
    public AlphaBlockCollection(IDataArray3 blocks, IDataArray3 data, IDataArray3 blockLight,
      IDataArray3 skyLight, IDataArray2 heightMap, TagNodeList tileEntities, TagNodeList tileTicks) {
        this.blocks = blocks;
        this.data = data;
        this.blockLight = blockLight;
        this.skyLight = skyLight;
        this.heightMap = heightMap;
        this.tileEntities = tileEntities;
        this.tileTicks = tileTicks;

        if(this.tileTicks == null) {
            this.tileTicks = new TagNodeList(TagType.TAG_COMPOUND);
        }

        this.xdim = this.blocks.XDim;
        this.ydim = this.blocks.YDim;
        this.zdim = this.blocks.ZDim;

        Refresh();
    }

    /// <summary>
    /// Updates internal managers if underlying data, such as TileEntities, have been modified outside of the container.
    /// </summary>
    public void Refresh() {
        this.lightManager = new BlockLight(this);
        this.fluidManager = new BlockFluid(this);
        this.tileEntityManager = new BlockTileEntities(this.blocks, this.tileEntities);
        this.tileTickManager = new BlockTileTicks(this.blocks, this.tileTicks);
    }

    internal TagNodeList TileTicks {
        get { return this.tileTicks; }
    }

    #region Events
    public event NeighborLookupHandler ResolveNeighbor {
        add {
            this.lightManager.ResolveNeighbor += delegate (int relx, int rely, int relz) {
                return value(relx, rely, relz);
            };
            this.fluidManager.ResolveNeighbor += delegate (int relx, int rely, int relz) {
                return value(relx, rely, relz);
            };
        }

        remove {
            this.lightManager = new BlockLight(this);
            this.fluidManager = new BlockFluid(this);
        }
    }

    public event BlockCoordinateHandler TranslateCoordinates {
        add { this.tileEntityManager.TranslateCoordinates += value; }
        remove { this.tileEntityManager.TranslateCoordinates -= value; }
    }
    #endregion

    /// <summary>
    /// Gets or sets a value indicating whether changes to blocks will trigger automatic lighting updates.
    /// </summary>
    /// <remarks>Automatic updates to lighting may spill into neighboring <see cref="AlphaBlockCollection"/> objects, if they can
    /// be resolved.</remarks>
    public bool AutoLight {
        get { return this.autoLight; }
        set { this.autoLight = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether changes to blocks will trigger automatic fluid updates.
    /// </summary>
    /// <remarks>Automatic updates to fluid may cascade through neighboring <see cref="AlphaBlockCollection"/> objects and beyond,
    /// if they can be resolved.</remarks>
    public bool AutoFluid {
        get { return this.autoFluid; }
        set { this.autoFluid = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether changes to blocks will create tile tick entries.
    /// </summary>
    public bool AutoTileTick {
        get { return this.autoTick; }
        set { this.autoTick = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="AlphaBlockCollection"/> needs to be saved.
    /// </summary>
    /// <remarks>If this <see cref="AlphaBlockCollection"/> is backed by a reference conainer type, set this property to false
    /// to prevent any modifications from being saved.  The next update will set this property true again, however.</remarks>
    public bool IsDirty {
        get { return this.dirty; }
        set { this.dirty = value; }
    }

    /// <summary>
    /// Returns a new <see cref="AlphaBlock"/> object from local coordinates relative to this collection.
    /// </summary>
    /// <param name="x">Local X-coordinate of block.</param>
    /// <param name="y">Local Y-coordinate of block.</param>
    /// <param name="z">Local Z-coordiante of block.</param>
    /// <returns>A new <see cref="AlphaBlock"/> object representing context-independent data of a single block.</returns>
    /// <remarks>Context-independent data excludes data such as lighting.  <see cref="AlphaBlock"/> object actually contain a copy
    /// of the data they represent, so changes to the <see cref="AlphaBlock"/> will not affect this container, and vice-versa.</remarks>
    public AlphaBlock GetBlock(int x, int y, int z) {
        return new AlphaBlock(this, x, y, z);
    }

    /// <summary>
    /// Returns a new <see cref="AlphaBlockRef"/> object from local coordaintes relative to this collection.
    /// </summary>
    /// <param name="x">Local X-coordinate of block.</param>
    /// <param name="y">Local Y-coordinate of block.</param>
    /// <param name="z">Local Z-coordinate of block.</param>
    /// <returns>A new <see cref="AlphaBlockRef"/> object representing context-dependent data of a single block.</returns>
    /// <remarks>Context-depdendent data includes all data associated with this block.  Since a <see cref="AlphaBlockRef"/> represents
    /// a view of a block within this container, any updates to data in the container will be reflected in the <see cref="AlphaBlockRef"/>,
    /// and vice-versa for updates to the <see cref="AlphaBlockRef"/>.</remarks>
    public AlphaBlockRef GetBlockRef(int x, int y, int z) {
        return new AlphaBlockRef(this, this.blocks.GetIndex(x, y, z));
    }

    /// <summary>
    /// Updates a block in this collection with values from a <see cref="AlphaBlock"/> object.
    /// </summary>
    /// <param name="x">Local X-coordinate of a block.</param>
    /// <param name="y">Local Y-coordinate of a block.</param>
    /// <param name="z">Local Z-coordinate of a block.</param>
    /// <param name="block">A <see cref="AlphaBlock"/> object to copy block data from.</param>
    public void SetBlock(int x, int y, int z, AlphaBlock block) {
        SetID(x, y, z, block.ID);
        SetData(x, y, z, block.Data);

        TileEntity te = block.GetTileEntity();
        if(te != null) {
            SetTileEntity(x, y, z, te.Copy());
        }

        TileTick tt = block.GetTileTick();
        if(tt != null) {
            SetTileTick(x, y, z, tt.Copy());
        }
    }

    #region IBoundedBlockCollection Members
    /// <inheritdoc/>
    public int XDim {
        get { return this.xdim; }
    }

    /// <inheritdoc/>
    public int YDim {
        get { return this.ydim; }
    }

    /// <inheritdoc/>
    public int ZDim {
        get { return this.zdim; }
    }

    IBlock IBoundedBlockCollection.GetBlock(int x, int y, int z) {
        return GetBlock(x, y, z);
    }

    IBlock IBoundedBlockCollection.GetBlockRef(int x, int y, int z) {
        return GetBlockRef(x, y, z);
    }

    /// <inheritdoc/>
    public void SetBlock(int x, int y, int z, IBlock block) {
        SetID(x, y, z, block.ID);
    }

    /// <inheritdoc/>
    public BlockInfo GetInfo(int x, int y, int z) {
        return BlockInfo.BlockTable[this.blocks[x, y, z]];
    }

    internal BlockInfo GetInfo(int index) {
        return BlockInfo.BlockTable[this.blocks[index]];
    }

    /// <inheritdoc/>
    public int GetID(int x, int y, int z) {
        return this.blocks[x, y, z];
    }

    internal int GetID(int index) {
        return this.blocks[index];
    }

    /// <inheritdoc/>
    /// <remarks>Depending on the options set for this <see cref="AlphaBlockCollection"/>, this method can be very
    /// heavy-handed in the amount of work it does to maintain consistency of tile entities, lighting, fluid, etc.
    /// for the affected block and possibly many other indirectly-affected blocks in the collection or neighboring
    /// collections.  If many SetID calls are expected to be made, some of this auto-reconciliation behavior should
    /// be disabled, and the data should be rebuilt at the <see cref="AlphaBlockCollection"/>-level at the end.</remarks>
    public void SetID(int x, int y, int z, int id) {
        int oldid = this.blocks[x, y, z];
        if(oldid == id) {
            return;
        }

        // Update value
        this.blocks[x, y, z] = id;

        // Update tile entities
        BlockInfo info1 = BlockInfo.BlockTable[oldid];
        BlockInfo info2 = BlockInfo.BlockTable[id];

        BlockInfoEx einfo1 = info1 as BlockInfoEx;
        BlockInfoEx einfo2 = info2 as BlockInfoEx;

        if(einfo1 != einfo2) {
            if(einfo1 != null || !info1.Registered) {
                ClearTileEntity(x, y, z);
            }

            if(einfo2 != null) {
                CreateTileEntity(x, y, z);
            }
        }

        // Light consistency
        if(this.autoLight) {
            if(info1.ObscuresLight != info2.ObscuresLight) {
                this.lightManager.UpdateHeightMap(x, y, z);
            }

            if(info1.Luminance != info2.Luminance || info1.Opacity != info2.Opacity || info1.TransmitsLight != info2.TransmitsLight) {
                UpdateBlockLight(x, y, z);
            }

            if(info1.Opacity != info2.Opacity || info1.TransmitsLight != info2.TransmitsLight) {
                UpdateSkyLight(x, y, z);
            }
        }

        // Fluid consistency
        if(this.autoFluid) {
            if(info1.State == BlockState.FLUID || info2.State == BlockState.FLUID) {
                UpdateFluid(x, y, z);
            }
        }

        // TileTick consistency
        if(this.autoTick) {
            if(info1.ID != info2.ID) {
                ClearTileTick(x, y, z);
                if(info2.Tick > 0) {
                    SetTileTickValue(x, y, z, info2.Tick);
                }
            }
        }
        this.dirty = true;
    }

    internal void SetID(int index, int id) {
        int x, y, z;
        this.blocks.GetMultiIndex(index, out x, out y, out z);

        SetID(x, y, z, id);
    }

    /// <inheritdoc/>
    public int CountByID(int id) {
        int c = 0;
        for(int i = 0; i < this.blocks.Length; i++) {
            if(this.blocks[i] == id) {
                c++;
            }
        }

        return c;
    }
    #endregion


    #region IBoundedDataBlockContainer Members
    IDataBlock IBoundedDataBlockCollection.GetBlock(int x, int y, int z) {
        return GetBlock(x, y, z);
    }

    IDataBlock IBoundedDataBlockCollection.GetBlockRef(int x, int y, int z) {
        return GetBlockRef(x, y, z);
    }

    /// <inheritdoc/>
    public void SetBlock(int x, int y, int z, IDataBlock block) {
        SetID(x, y, z, block.ID);
        SetData(x, y, z, block.Data);
    }

    /// <inheritdoc/>
    public int GetData(int x, int y, int z) {
        return this.data[x, y, z];
    }

    internal int GetData(int index) {
        return this.data[index];
    }

    /// <inheritdoc/>
    public void SetData(int x, int y, int z, int data) {
        if(this.data[x, y, z] != data) {
            this.data[x, y, z] = (byte) data;
            this.dirty = true;
        }

        /*if(BlockManager.EnforceDataLimits && BlockInfo.BlockTable[blocks[index]] != null) {
            if(!BlockInfo.BlockTable[blocks[index]].TestData(data)) {
                return false;
            }
        }*/
    }

    internal void SetData(int index, int data) {
        if(this.data[index] != data) {
            this.data[index] = (byte) data;
            this.dirty = true;
        }
    }

    /// <inheritdoc/>
    public int CountByData(int id, int data) {
        int c = 0;
        for(int i = 0; i < this.blocks.Length; i++) {
            if(this.blocks[i] == id && this.data[i] == data) {
                c++;
            }
        }
        return c;
    }
    #endregion

    #region IBoundedLitBlockCollection Members
    ILitBlock IBoundedLitBlockCollection.GetBlock(int x, int y, int z) {
        throw new NotImplementedException();
    }

    ILitBlock IBoundedLitBlockCollection.GetBlockRef(int x, int y, int z) {
        return GetBlockRef(x, y, z);
    }

    /// <inheritdoc/>
    public void SetBlock(int x, int y, int z, ILitBlock block) {
        SetID(x, y, z, block.ID);
        SetBlockLight(x, y, z, block.BlockLight);
        SetSkyLight(x, y, z, block.SkyLight);
    }

    /// <inheritdoc/>
    public int GetBlockLight(int x, int y, int z) {
        return this.blockLight[x, y, z];
    }

    internal int GetBlockLight(int index) {
        return this.blockLight[index];
    }

    /// <inheritdoc/>
    public int GetSkyLight(int x, int y, int z) {
        return this.skyLight[x, y, z];
    }

    internal int GetSkyLight(int index) {
        return this.skyLight[index];
    }

    /// <inheritdoc/>
    public void SetBlockLight(int x, int y, int z, int light) {
        if(this.blockLight[x, y, z] != light) {
            this.blockLight[x, y, z] = (byte) light;
            this.dirty = true;
        }
    }

    internal void SetBlockLight(int index, int light) {
        if(this.blockLight[index] != light) {
            this.blockLight[index] = (byte) light;
            this.dirty = true;
        }
    }

    /// <inheritdoc/>
    public void SetSkyLight(int x, int y, int z, int light) {
        if(this.skyLight[x, y, z] != light) {
            this.skyLight[x, y, z] = (byte) light;
            this.dirty = true;
        }
    }

    internal void SetSkyLight(int index, int light) {
        if(this.skyLight[index] != light) {
            this.skyLight[index] = (byte) light;
            this.dirty = true;
        }
    }

    /// <inheritdoc/>
    public int GetHeight(int x, int z) {
        return this.heightMap[x, z];
    }

    /// <inheritdoc/>
    public void SetHeight(int x, int z, int height) {
        this.heightMap[x, z] = (byte) height;
    }

    /// <inheritdoc/>
    public void UpdateBlockLight(int x, int y, int z) {
        this.lightManager.UpdateBlockLight(x, y, z);
        this.dirty = true;
    }

    /// <inheritdoc/>
    public void UpdateSkyLight(int x, int y, int z) {
        this.lightManager.UpdateBlockSkyLight(x, y, z);
        this.dirty = true;
    }

    /// <inheritdoc/>
    public void ResetBlockLight() {
        this.blockLight.Clear();
        this.dirty = true;
    }

    /// <inheritdoc/>
    public void ResetSkyLight() {
        this.skyLight.Clear();
        this.dirty = true;
    }

    /// <inheritdoc/>
    public void RebuildBlockLight() {
        this.lightManager.RebuildBlockLight();
        this.dirty = true;
    }

    /// <inheritdoc/>
    public void RebuildSkyLight() {
        this.lightManager.RebuildBlockSkyLight();
        this.dirty = true;
    }

    /// <inheritdoc/>
    public void RebuildHeightMap() {
        this.lightManager.RebuildHeightMap();
        this.dirty = true;
    }

    /// <inheritdoc/>
    public void StitchBlockLight() {
        this.lightManager.StitchBlockLight();
        this.dirty = true;
    }

    /// <inheritdoc/>
    public void StitchSkyLight() {
        this.lightManager.StitchBlockSkyLight();
        this.dirty = true;
    }

    /// <inheritdoc/>
    public void StitchBlockLight(IBoundedLitBlockCollection blockset, BlockCollectionEdge edge) {
        this.lightManager.StitchBlockLight(blockset, edge);
        this.dirty = true;
    }

    /// <inheritdoc/>
    public void StitchSkyLight(IBoundedLitBlockCollection blockset, BlockCollectionEdge edge) {
        this.lightManager.StitchBlockSkyLight(blockset, edge);
        this.dirty = true;
    }
    #endregion

    #region IBoundedPropertyBlockCollection Members
    IPropertyBlock IBoundedPropertyBlockCollection.GetBlock(int x, int y, int z) {
        return GetBlock(x, y, z);
    }

    IPropertyBlock IBoundedPropertyBlockCollection.GetBlockRef(int x, int y, int z) {
        return GetBlockRef(x, y, z);
    }

    /// <inheritdoc/>
    public void SetBlock(int x, int y, int z, IPropertyBlock block) {
        SetID(x, y, z, block.ID);
        SetTileEntity(x, y, z, block.GetTileEntity().Copy());
    }

    /// <inheritdoc/>
    public TileEntity GetTileEntity(int x, int y, int z) {
        return this.tileEntityManager.GetTileEntity(x, y, z);
    }

    internal TileEntity GetTileEntity(int index) {
        int x, y, z;
        this.blocks.GetMultiIndex(index, out x, out y, out z);

        return this.tileEntityManager.GetTileEntity(x, y, z);
    }

    /// <inheritdoc/>
    public void SetTileEntity(int x, int y, int z, TileEntity te) {
        this.tileEntityManager.SetTileEntity(x, y, z, te);
        this.dirty = true;
    }

    internal void SetTileEntity(int index, TileEntity te) {
        int x, y, z;
        this.blocks.GetMultiIndex(index, out x, out y, out z);

        this.tileEntityManager.SetTileEntity(x, y, z, te);
        this.dirty = true;
    }

    /// <inheritdoc/>
    public void CreateTileEntity(int x, int y, int z) {
        this.tileEntityManager.CreateTileEntity(x, y, z);
        this.dirty = true;
    }

    internal void CreateTileEntity(int index) {
        int x, y, z;
        this.blocks.GetMultiIndex(index, out x, out y, out z);

        this.tileEntityManager.CreateTileEntity(x, y, z);
        this.dirty = true;
    }

    /// <inheritdoc/>
    public void ClearTileEntity(int x, int y, int z) {
        this.tileEntityManager.ClearTileEntity(x, y, z);
        this.dirty = true;
    }

    internal void ClearTileEntity(int index) {
        int x, y, z;
        this.blocks.GetMultiIndex(index, out x, out y, out z);

        this.tileEntityManager.ClearTileEntity(x, y, z);
        this.dirty = true;
    }
    #endregion

    #region IBoundedActiveBlockCollection Members
    IActiveBlock IBoundedActiveBlockCollection.GetBlock(int x, int y, int z) {
        return GetBlock(x, y, z);
    }

    IActiveBlock IBoundedActiveBlockCollection.GetBlockRef(int x, int y, int z) {
        return GetBlockRef(x, y, z);
    }

    /// <inheritdoc/>
    public void SetBlock(int x, int y, int z, IActiveBlock block) {
        SetID(x, y, z, block.ID);
        SetTileTick(x, y, z, block.GetTileTick().Copy());
    }

    /// <inheritdoc/>
    public int GetTileTickValue(int x, int y, int z) {
        return this.tileTickManager.GetTileTickValue(x, y, z);
    }

    internal int GetTileTickValue(int index) {
        int x, y, z;
        this.blocks.GetMultiIndex(index, out x, out y, out z);

        return this.tileTickManager.GetTileTickValue(x, y, z);
    }

    /// <inheritdoc/>
    public void SetTileTickValue(int x, int y, int z, int tickValue) {
        this.tileTickManager.SetTileTickValue(x, y, z, tickValue);
        this.dirty = true;
    }

    internal void SetTileTickValue(int index, int tickValue) {
        int x, y, z;
        this.blocks.GetMultiIndex(index, out x, out y, out z);

        this.tileTickManager.SetTileTickValue(x, y, z, tickValue);
        this.dirty = true;
    }

    /// <inheritdoc/>
    public TileTick GetTileTick(int x, int y, int z) {
        return this.tileTickManager.GetTileTick(x, y, z);
    }

    internal TileTick GetTileTick(int index) {
        int x, y, z;
        this.blocks.GetMultiIndex(index, out x, out y, out z);

        return this.tileTickManager.GetTileTick(x, y, z);
    }

    /// <inheritdoc/>
    public void SetTileTick(int x, int y, int z, TileTick tt) {
        this.tileTickManager.SetTileTick(x, y, z, tt);
        this.dirty = true;
    }

    internal void SetTileTick(int index, TileTick tt) {
        int x, y, z;
        this.blocks.GetMultiIndex(index, out x, out y, out z);

        this.tileTickManager.SetTileTick(x, y, z, tt);
        this.dirty = true;
    }

    /// <inheritdoc/>
    public void CreateTileTick(int x, int y, int z) {
        this.tileTickManager.CreateTileTick(x, y, z);
        this.dirty = true;
    }

    internal void CreateTileTick(int index) {
        int x, y, z;
        this.blocks.GetMultiIndex(index, out x, out y, out z);

        this.tileTickManager.CreateTileTick(x, y, z);
        this.dirty = true;
    }

    /// <inheritdoc/>
    public void ClearTileTick(int x, int y, int z) {
        this.tileTickManager.ClearTileTick(x, y, z);
        this.dirty = true;
    }

    internal void ClearTileTick(int index) {
        int x, y, z;
        this.blocks.GetMultiIndex(index, out x, out y, out z);

        this.tileTickManager.ClearTileTick(x, y, z);
        this.dirty = true;
    }
    #endregion

    /// <summary>
    /// Resets all fluid blocks in the collection to their inactive type.
    /// </summary>
    public void ResetFluid() {
        this.fluidManager.ResetWater(this.blocks, this.data);
        this.fluidManager.ResetLava(this.blocks, this.data);
        this.dirty = true;
    }

    /// <summary>
    /// Performs fluid simulation on all fluid blocks on this container.
    /// </summary>
    /// <remarks>Simulation will cause inactive fluid blocks to convert into and spread active fluid blocks according
    /// to the fluid calculation rules in Minecraft.  Fluid calculation may spill into neighboring block collections
    /// (and beyond).</remarks>
    public void RebuildFluid() {
        this.fluidManager.RebuildWater();
        this.fluidManager.RebuildLava();
        this.dirty = true;
    }

    /// <summary>
    /// Recalculates fluid starting from a given fluid block in this collection.
    /// </summary>
    /// <param name="x">Local X-coordinate of block.</param>
    /// <param name="y">Local Y-coordinate of block.</param>
    /// <param name="z">Local Z-coordiante of block.</param>
    public void UpdateFluid(int x, int y, int z) {
        bool autofluid = this.autoFluid;
        this.autoFluid = false;

        int blocktype = this.blocks[x, y, z];

        if(blocktype == BlockInfo.Water.ID || blocktype == BlockInfo.StationaryWater.ID) {
            this.fluidManager.UpdateWater(x, y, z);
            this.dirty = true;
        } else if(blocktype == BlockInfo.Lava.ID || blocktype == BlockInfo.StationaryLava.ID) {
            this.fluidManager.UpdateLava(x, y, z);
            this.dirty = true;
        }

        this.autoFluid = autofluid;
    }

    /*#region IEnumerable<AlphaBlockRef> Members
    public IEnumerator<AlphaBlockRef> GetEnumerator() {
        return new AlphaBlockEnumerator(this);
    }
    #endregion

    #region IEnumerable Members
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
        return new AlphaBlockEnumerator(this);
    }
    #endregion

    public class AlphaBlockEnumerator : IEnumerator<AlphaBlockRef> {
        private AlphaBlockCollection collection;
        private int index;
        private int size;

        public AlphaBlockEnumerator(AlphaBlockCollection collection) {
            collection = collection;
            index = -1;
            size = collection.XDim * collection.YDim * collection.ZDim;
        }

        #region IEnumerator<Entity> Members
        public AlphaBlockRef Current {
            get {
                if(index == -1 || index == size) {
                    throw new InvalidOperationException();
                }
                return new AlphaBlockRef(collection, index);
            }
        }
        #endregion

        #region IDisposable Members
        public void Dispose() {}
        #endregion

        #region IEnumerator Members
        object System.Collections.IEnumerator.Current {
            get { return Current; }
        }

        public bool MoveNext() {
            if(++index == size) {
                return false;
            }

            return true;
        }

        public void Reset() {
            index = -1;
        }
        #endregion
    }*/
}
