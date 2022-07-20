namespace Substrate.ImportExport;

using Substrate.Core;
using Substrate.Nbt;

/// <summary>
/// Provides import and export support for the 3rd party schematic file format.
/// </summary>
public class Schematic {
    private static SchemaNodeCompound schema = new SchemaNodeCompound() {
        new SchemaNodeScaler("Width", TagType.TAG_SHORT),
        new SchemaNodeScaler("Length", TagType.TAG_SHORT),
        new SchemaNodeScaler("Height", TagType.TAG_SHORT),
        new SchemaNodeString("Materials", "Alpha"),
        new SchemaNodeArray("Blocks"),
        new SchemaNodeArray("Data"),
        new SchemaNodeList("Entities", TagType.TAG_COMPOUND, Entity.Schema),
        new SchemaNodeList("TileEntities", TagType.TAG_COMPOUND, TileEntity.Schema),
    };

    private XZYByteArray blocks;
    private XZYNibbleArray data;
    private XZYNibbleArray blockLight;
    private XZYNibbleArray skyLight;
    private ZXByteArray heightMap;

    private TagNodeList entities;
    private TagNodeList tileEntities;

    private AlphaBlockCollection blockset;
    private EntityCollection entityset;

    private Schematic() { }

    /// <summary>
    /// Create an exportable schematic wrapper around existing blocks and entities.
    /// </summary>
    /// <param name="blocks">An existing <see cref="AlphaBlockCollection"/>.</param>
    /// <param name="entities">An existing <see cref="EntityCollection"/>.</param>
    public Schematic(AlphaBlockCollection blocks, EntityCollection entities) {
        this.blockset = blocks;
        this.entityset = entities;
    }

    /// <summary>
    /// Create an empty, exportable schematic of given dimensions.
    /// </summary>
    /// <param name="xdim">The length of the X-dimension in blocks.</param>
    /// <param name="ydim">The length of the Y-dimension in blocks.</param>
    /// <param name="zdim">The length of the Z-dimension in blocks.</param>
    public Schematic(int xdim, int ydim, int zdim) {
        this.blocks = new XZYByteArray(xdim, ydim, zdim);
        this.data = new XZYNibbleArray(xdim, ydim, zdim);
        this.blockLight = new XZYNibbleArray(xdim, ydim, zdim);
        this.skyLight = new XZYNibbleArray(xdim, ydim, zdim);
        this.heightMap = new ZXByteArray(xdim, zdim);

        this.entities = new TagNodeList(TagType.TAG_COMPOUND);
        this.tileEntities = new TagNodeList(TagType.TAG_COMPOUND);

        this.blockset = new AlphaBlockCollection(this.blocks, this.data, this.blockLight, this.skyLight, this.heightMap, this.tileEntities);
        this.entityset = new EntityCollection(this.entities);
    }

    #region Properties
    /// <summary>
    /// Gets or sets the underlying block collection.
    /// </summary>
    public AlphaBlockCollection Blocks {
        get { return this.blockset; }
        set { this.blockset = value; }
    }

    /// <summary>
    /// Gets or sets the underlying entity collection.
    /// </summary>
    public EntityCollection Entities {
        get { return this.entityset; }
        set { this.entityset = value; }
    }
    #endregion

    /// <summary>
    /// Imports a schematic file at the given path and returns in as a <see cref="Schematic"/> object.
    /// </summary>
    /// <param name="path">The path to the schematic file.</param>
    /// <returns>A <see cref="Schematic"/> object containing the decoded schematic file data.</returns>
    public static Schematic Import(string path) {
        NBTFile schematicFile = new NBTFile(path);
        if(!schematicFile.Exists()) {
            return null;
        }
        NbtTree tree;

        using(Stream nbtStream = schematicFile.GetDataInputStream()) {
            if(nbtStream == null) {
                return null;
            }

            tree = new NbtTree(nbtStream);
        }

        NbtVerifier v = new NbtVerifier(tree.Root, schema);
        if(!v.Verify()) {
            return null;
        }

        //TagNodeCompound schematic = tree.Root["Schematic"] as TagNodeCompound;
        TagNodeCompound schematic = tree.Root;
        int xdim = schematic["Width"].ToTagShort();
        int zdim = schematic["Length"].ToTagShort();
        int ydim = schematic["Height"].ToTagShort();

        Schematic self = new Schematic(xdim, ydim, zdim);

        // Damnit, schematic is YZX ordering.
        YZXByteArray schemaBlocks = new YZXByteArray(xdim, ydim, zdim, schematic["Blocks"].ToTagByteArray());
        YZXByteArray schemaData = new YZXByteArray(xdim, ydim, zdim, schematic["Data"].ToTagByteArray());

        for(int x = 0; x < xdim; x++) {
            for(int y = 0; y < ydim; y++) {
                for(int z = 0; z < zdim; z++) {
                    self.blocks[x, y, z] = schemaBlocks[x, y, z];
                    self.data[x, y, z] = schemaData[x, y, z];
                }
            }
        }

        TagNodeList entities = schematic["Entities"] as TagNodeList;
        foreach(TagNode e in entities) {
            self.entities.Add(e);
        }

        TagNodeList tileEntities = schematic["TileEntities"] as TagNodeList;
        foreach(TagNode te in tileEntities) {
            self.tileEntities.Add(te);
        }

        self.blockset.Refresh();

        return self;
    }

    /// <summary>
    /// Exports the <see cref="Schematic"/> object to a schematic file.
    /// </summary>
    /// <param name="path">The path to write out the schematic file to.</param>
    public void Export(string path) {
        int xdim = this.blockset.XDim;
        int ydim = this.blockset.YDim;
        int zdim = this.blockset.ZDim;

        byte[] blockData = new byte[xdim * ydim * zdim];
        byte[] dataData = new byte[xdim * ydim * zdim];

        YZXByteArray schemaBlocks = new YZXByteArray(this.blockset.XDim, this.blockset.YDim, this.blockset.ZDim, blockData);
        YZXByteArray schemaData = new YZXByteArray(this.blockset.XDim, this.blockset.YDim, this.blockset.ZDim, dataData);

        TagNodeList entities = new TagNodeList(TagType.TAG_COMPOUND);
        TagNodeList tileEntities = new TagNodeList(TagType.TAG_COMPOUND);

        for(int x = 0; x < xdim; x++) {
            for(int z = 0; z < zdim; z++) {
                for(int y = 0; y < ydim; y++) {
                    AlphaBlock block = this.blockset.GetBlock(x, y, z);
                    schemaBlocks[x, y, z] = (byte) block.ID;
                    schemaData[x, y, z] = (byte) block.Data;

                    TileEntity te = block.GetTileEntity();
                    if(te != null) {
                        te.X = x;
                        te.Y = y;
                        te.Z = z;

                        tileEntities.Add(te.BuildTree());
                    }
                }
            }
        }

        foreach(TypedEntity e in this.entityset) {
            entities.Add(e.BuildTree());
        }

        TagNodeCompound schematic = new TagNodeCompound();
        schematic["Width"] = new TagNodeShort((short) xdim);
        schematic["Length"] = new TagNodeShort((short) zdim);
        schematic["Height"] = new TagNodeShort((short) ydim);

        schematic["Entities"] = entities;
        schematic["TileEntities"] = tileEntities;

        schematic["Materials"] = new TagNodeString("Alpha");

        schematic["Blocks"] = new TagNodeByteArray(blockData);
        schematic["Data"] = new TagNodeByteArray(dataData);

        NBTFile schematicFile = new NBTFile(path);

        using(Stream nbtStream = schematicFile.GetDataOutputStream()) {
            if(nbtStream == null) {
                return;
            }

            NbtTree tree = new NbtTree(schematic, "Schematic");
            tree.WriteTo(nbtStream);
        }
    }
}
