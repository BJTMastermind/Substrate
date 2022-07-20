namespace Substrate;

using Substrate.Core;
using Substrate.Nbt;

/// <summary>
/// A Minecraft Alpha- and Beta-compatible chunk data structure.
/// </summary>
/// <remarks>
/// A Chunk internally wraps an NBT_Tree of raw chunk data.  Modifying the chunk will update the tree, and vice-versa.
/// </remarks>
public class AlphaChunk : IChunk, INbtObject<AlphaChunk>, ICopyable<AlphaChunk> {
    private const int XDIM = 16;
    private const int YDIM = 128;
    private const int ZDIM = 16;

    /// <summary>
    /// An NBT Schema definition for valid chunk data.
    /// </summary>
    public static SchemaNodeCompound LevelSchema = new SchemaNodeCompound() {
        new SchemaNodeCompound("Level") {
            new SchemaNodeArray("Blocks", 32768),
            new SchemaNodeArray("Data", 16384),
            new SchemaNodeArray("SkyLight", 16384),
            new SchemaNodeArray("BlockLight", 16384),
            new SchemaNodeArray("HeightMap", 256),
            new SchemaNodeList("Entities", TagType.TAG_COMPOUND, SchemaOptions.CREATE_ON_MISSING),
            new SchemaNodeList("TileEntities", TagType.TAG_COMPOUND, TileEntity.Schema, SchemaOptions.CREATE_ON_MISSING),
            new SchemaNodeList("TileTicks", TagType.TAG_COMPOUND, TileTick.Schema, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("LastUpdate", TagType.TAG_LONG, SchemaOptions.CREATE_ON_MISSING),
            new SchemaNodeScaler("xPos", TagType.TAG_INT),
            new SchemaNodeScaler("zPos", TagType.TAG_INT),
            new SchemaNodeScaler("TerrainPopulated", TagType.TAG_BYTE, SchemaOptions.CREATE_ON_MISSING),
        },
    };

    private NbtTree tree;

    private int cx;
    private int cz;

    private XZYByteArray blocks;
    private XZYNibbleArray data;
    private XZYNibbleArray blockLight;
    private XZYNibbleArray skyLight;
    private ZXByteArray heightMap;

    private TagNodeList entities;
    private TagNodeList tileEntities;
    private TagNodeList tileTicks;

    private AlphaBlockCollection blockManager;
    private EntityCollection entityManager;

    /// <summary>
    /// Gets the global X-coordinate of the chunk.
    /// </summary>
    public int X {
        get { return this.cx; }
    }

    /// <summary>
    /// Gets the global Z-coordinate of the chunk.
    /// </summary>
    public int Z {
        get { return this.cz; }
    }

    /// <summary>
    /// Gets the collection of all blocks and their data stored in the chunk.
    /// </summary>
    public AlphaBlockCollection Blocks {
        get { return this.blockManager; }
    }

    public AnvilBiomeCollection Biomes {
        get { return null; }
    }

    /// <summary>
    /// Gets the collection of all entities stored in the chunk.
    /// </summary>
    public EntityCollection Entities {
        get { return this.entityManager; }
    }

    /// <summary>
    /// Provides raw access to the underlying NBT_Tree.
    /// </summary>
    public NbtTree Tree {
        get { return this.tree; }
    }

    /// <summary>
    /// Gets or sets the chunk's TerrainPopulated status.
    /// </summary>
    public bool IsTerrainPopulated {
        get { return this.tree.Root["Level"].ToTagCompound()["TerrainPopulated"].ToTagByte() == 1; }
        set { this.tree.Root["Level"].ToTagCompound()["TerrainPopulated"].ToTagByte().Data = (byte) (value ? 1 : 0); }
    }

    private AlphaChunk() { }

    /// <summary>
    /// Creates a default (empty) chunk.
    /// </summary>
    /// <param name="x">Global X-coordinate of the chunk.</param>
    /// <param name="z">Global Z-coordinate of the chunk.</param>
    /// <returns>A new Chunk object.</returns>
    public static AlphaChunk Create(int x, int z) {
        AlphaChunk c = new AlphaChunk();
        c.cx = x;
        c.cz = z;

        c.BuildNBTTree();
        return c;
    }

    /// <summary>
    /// Creates a chunk object from an existing NBT_Tree.
    /// </summary>
    /// <param name="tree">An NBT_Tree conforming to the chunk schema definition.</param>
    /// <returns>A new Chunk object wrapping an existing NBT_Tree.</returns>
    public static AlphaChunk Create(NbtTree tree) {
        AlphaChunk c = new AlphaChunk();
        return c.LoadTree(tree.Root);
    }

    /// <summary>
    /// Creates a chunk object from a verified NBT_Tree.
    /// </summary>
    /// <param name="tree">An NBT_Tree conforming to the chunk schema definition.</param>
    /// <returns>A new Chunk object wrapping an existing NBT_Tree, or null on verification failure.</returns>
    public static AlphaChunk CreateVerified(NbtTree tree) {
        AlphaChunk c = new AlphaChunk();
        return c.LoadTreeSafe(tree.Root);
    }

    /// <summary>
    /// Updates the chunk's global world coordinates.
    /// </summary>
    /// <param name="x">Global X-coordinate.</param>
    /// <param name="z">Global Z-coordinate.</param>
    public void SetLocation(int x, int z) {
        int diffx = (x - this.cx) * XDIM;
        int diffz = (z - this.cz) * ZDIM;

        // Update chunk position
        this.cx = x;
        this.cz = z;

        this.tree.Root["Level"].ToTagCompound()["xPos"].ToTagInt().Data = x;
        this.tree.Root["Level"].ToTagCompound()["zPos"].ToTagInt().Data = z;

        // Update tile entity coordinates
        List<TileEntity> tileEntites = new List<TileEntity>();
        foreach(TagNodeCompound tag in this.tileEntities) {
            TileEntity te = TileEntityFactory.Create(tag);
            if(te == null) {
                te = TileEntity.FromTreeSafe(tag);
            }

            if(te != null) {
                te.MoveBy(diffx, 0, diffz);
                tileEntites.Add(te);
            }
        }

        this.tileEntities.Clear();
        foreach(TileEntity te in tileEntites) {
            this.tileEntities.Add(te.BuildTree());
        }

        // Update tile tick coordinates
        if(this.tileTicks != null) {
            List<TileTick> tileTicks = new List<TileTick>();
            foreach(TagNodeCompound tag in this.tileTicks) {
                TileTick tt = TileTick.FromTreeSafe(tag);

                if(tt != null) {
                    tt.MoveBy(diffx, 0, diffz);
                    tileTicks.Add(tt);
                }
            }

            this.tileTicks.Clear();
            foreach(TileTick tt in tileTicks) {
                this.tileTicks.Add(tt.BuildTree());
            }
        }

        // Update entity coordinates
        List<TypedEntity> entities = new List<TypedEntity>();
        foreach(TypedEntity entity in this.entityManager) {
            entity.MoveBy(diffx, 0, diffz);
            entities.Add(entity);
        }

        this.entities.Clear();
        foreach(TypedEntity entity in entities) {
            this.entityManager.Add(entity);
        }
    }

    /// <summary>
    /// Saves a Chunk's underlying NBT_Tree to an output stream.
    /// </summary>
    /// <param name="outStream">An open, writable output stream.</param>
    /// <returns>True if the data is written out to the stream.</returns>
    public bool Save(Stream outStream) {
        if(outStream == null || !outStream.CanWrite) {
            return false;
        }

        BuildConditional();

        this.tree.WriteTo(outStream);

        return true;
    }

    #region INBTObject<Chunk> Members
    /// <summary>
    /// Loads the Chunk from an NBT tree rooted at the given TagValue node.
    /// </summary>
    /// <param name="tree">Root node of an NBT tree.</param>
    /// <returns>A reference to the current Chunk, or null if the tree is unparsable.</returns>
    public AlphaChunk LoadTree(TagNode tree) {
        TagNodeCompound ctree = tree as TagNodeCompound;
        if(ctree == null) {
            return null;
        }

        this.tree = new NbtTree(ctree);

        TagNodeCompound level = this.tree.Root["Level"] as TagNodeCompound;

        this.blocks = new XZYByteArray(XDIM, YDIM, ZDIM, level["Blocks"] as TagNodeByteArray);
        this.data = new XZYNibbleArray(XDIM, YDIM, ZDIM, level["Data"] as TagNodeByteArray);
        this.blockLight = new XZYNibbleArray(XDIM, YDIM, ZDIM, level["BlockLight"] as TagNodeByteArray);
        this.skyLight = new XZYNibbleArray(XDIM, YDIM, ZDIM, level["SkyLight"] as TagNodeByteArray);
        this.heightMap = new ZXByteArray(XDIM, ZDIM, level["HeightMap"] as TagNodeByteArray);

        this.entities = level["Entities"] as TagNodeList;
        this.tileEntities = level["TileEntities"] as TagNodeList;

        if(level.ContainsKey("TileTicks")) {
            this.tileTicks = level["TileTicks"] as TagNodeList;
        } else {
            this.tileTicks = new TagNodeList(TagType.TAG_COMPOUND);
        }

        // List-type patch up
        if(this.entities.Count == 0) {
            level["Entities"] = new TagNodeList(TagType.TAG_COMPOUND);
            this.entities = level["Entities"] as TagNodeList;
        }

        if(this.tileEntities.Count == 0) {
            level["TileEntities"] = new TagNodeList(TagType.TAG_COMPOUND);
            this.tileEntities = level["TileEntities"] as TagNodeList;
        }

        if(this.tileTicks.Count == 0) {
            level["TileTicks"] = new TagNodeList(TagType.TAG_COMPOUND);
            this.tileTicks = level["TileTicks"] as TagNodeList;
        }

        this.cx = level["xPos"].ToTagInt();
        this.cz = level["zPos"].ToTagInt();

        this.blockManager = new AlphaBlockCollection(this.blocks, this.data, this.blockLight, this.skyLight, this.heightMap, this.tileEntities, this.tileTicks);
        this.entityManager = new EntityCollection(this.entities);

        return this;
    }

    /// <summary>
    /// Loads the Chunk from a validated NBT tree rooted at the given TagValue node.
    /// </summary>
    /// <param name="tree">Root node of an NBT tree.</param>
    /// <returns>A reference to the current Chunk, or null if the tree does not conform to the chunk's NBT Schema definition.</returns>
    public AlphaChunk LoadTreeSafe(TagNode tree) {
        if(!ValidateTree(tree)) {
            return null;
        }
        return LoadTree(tree);
    }

    /// <summary>
    /// Gets a valid NBT tree representing the Chunk.
    /// </summary>
    /// <returns>The root node of the Chunk's NBT tree.</returns>
    public TagNode BuildTree() {
        BuildConditional();
        return this.tree.Root;
    }

    /// <summary>
    /// Validates an NBT tree against the chunk's NBT schema definition.
    /// </summary>
    /// <param name="tree">The root node of the NBT tree to verify.</param>
    /// <returns>Status indicating if the tree represents a valid chunk.</returns>
    public bool ValidateTree(TagNode tree) {
        NbtVerifier v = new NbtVerifier(tree, LevelSchema);
        return v.Verify();
    }
    #endregion

    #region ICopyable<Chunk> Members
    /// <summary>
    /// Creates a deep copy of the Chunk and its underlying NBT tree.
    /// </summary>
    /// <returns>A new Chunk with copied data.</returns>
    public AlphaChunk Copy() {
        return AlphaChunk.Create(this.tree.Copy());
    }
    #endregion

    private void BuildConditional() {
        TagNodeCompound level = this.tree.Root["Level"] as TagNodeCompound;
        if(this.tileTicks != this.blockManager.TileTicks && this.blockManager.TileTicks.Count > 0) {
            this.tileTicks = this.blockManager.TileTicks;
            level["TileTicks"] = this.tileTicks;
        }
    }

    private void BuildNBTTree() {
        int elements2 = XDIM * ZDIM;
        int elements3 = elements2 * YDIM;

        TagNodeByteArray blocks = new TagNodeByteArray(new byte[elements3]);
        TagNodeByteArray data = new TagNodeByteArray(new byte[elements3 >> 1]);
        TagNodeByteArray blocklight = new TagNodeByteArray(new byte[elements3 >> 1]);
        TagNodeByteArray skylight = new TagNodeByteArray(new byte[elements3 >> 1]);
        TagNodeByteArray heightMap = new TagNodeByteArray(new byte[elements2]);

        this.blocks = new XZYByteArray(XDIM, YDIM, ZDIM, blocks);
        this.data = new XZYNibbleArray(XDIM, YDIM, ZDIM, data);
        this.blockLight = new XZYNibbleArray(XDIM, YDIM, ZDIM, blocklight);
        this.skyLight = new XZYNibbleArray(XDIM, YDIM, ZDIM, skylight);
        this.heightMap = new ZXByteArray(XDIM, ZDIM, heightMap);

        this.entities = new TagNodeList(TagType.TAG_COMPOUND);
        this.tileEntities = new TagNodeList(TagType.TAG_COMPOUND);
        this.tileTicks = new TagNodeList(TagType.TAG_COMPOUND);

        TagNodeCompound level = new TagNodeCompound();
        level.Add("Blocks", blocks);
        level.Add("Data", data);
        level.Add("SkyLight", blocklight);
        level.Add("BlockLight", skylight);
        level.Add("HeightMap", heightMap);
        level.Add("Entities", this.entities);
        level.Add("TileEntities", this.tileEntities);
        level.Add("TileTicks", this.tileTicks);
        level.Add("LastUpdate", new TagNodeLong(Timestamp()));
        level.Add("xPos", new TagNodeInt(this.cx));
        level.Add("zPos", new TagNodeInt(this.cz));
        level.Add("TerrainPopulated", new TagNodeByte());

        this.tree = new NbtTree();
        this.tree.Root.Add("Level", level);

        this.blockManager = new AlphaBlockCollection(this.blocks, this.data, this.blockLight, this.skyLight, this.heightMap, this.tileEntities);
        this.entityManager = new EntityCollection(this.entities);
    }

    private int Timestamp() {
        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return (int) ((DateTime.UtcNow - epoch).Ticks / (10000L * 1000L));
    }
}
