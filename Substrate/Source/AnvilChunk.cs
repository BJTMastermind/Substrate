namespace Substrate;

using Substrate.Core;
using Substrate.Nbt;

public class AnvilChunk : IChunk, INbtObject<AnvilChunk>, ICopyable<AnvilChunk> {
    public static SchemaNodeCompound LevelSchema = new SchemaNodeCompound() {
        new SchemaNodeCompound("Level") {
            new SchemaNodeList("Sections", TagType.TAG_COMPOUND, new SchemaNodeCompound() {
                new SchemaNodeArray("Blocks", 4096),
                new SchemaNodeArray("Data", 2048),
                new SchemaNodeArray("SkyLight", 2048),
                new SchemaNodeArray("BlockLight", 2048),
                new SchemaNodeScaler("Y", TagType.TAG_BYTE),
                new SchemaNodeArray("Add", 2048, SchemaOptions.OPTIONAL),
            }),
            new SchemaNodeArray("Biomes", 256, SchemaOptions.OPTIONAL),
            new SchemaNodeIntArray("HeightMap", 256),
            new SchemaNodeList("Entities", TagType.TAG_COMPOUND, SchemaOptions.CREATE_ON_MISSING),
            new SchemaNodeList("TileEntities", TagType.TAG_COMPOUND, TileEntity.Schema, SchemaOptions.CREATE_ON_MISSING),
            new SchemaNodeList("TileTicks", TagType.TAG_COMPOUND, TileTick.Schema, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("LastUpdate", TagType.TAG_LONG, SchemaOptions.CREATE_ON_MISSING),
            new SchemaNodeScaler("xPos", TagType.TAG_INT),
            new SchemaNodeScaler("zPos", TagType.TAG_INT),
            new SchemaNodeScaler("TerrainPopulated", TagType.TAG_BYTE, SchemaOptions.CREATE_ON_MISSING),
        },
    };

    private const int XDIM = 16;
    private const int YDIM = 256;
    private const int ZDIM = 16;

    private NbtTree tree;

    private int cx;
    private int cz;

    private AnvilSection[] sections;

    private IDataArray3 blocks;
    private IDataArray3 data;
    private IDataArray3 blockLight;
    private IDataArray3 skyLight;

    private ZXIntArray heightMap;
    private ZXByteArray biomes;

    private TagNodeList entities;
    private TagNodeList tileEntities;
    private TagNodeList tileTicks;

    private AlphaBlockCollection blockManager;
    private EntityCollection entityManager;
    private AnvilBiomeCollection biomeManager;

    private AnvilChunk() {
        this.sections = new AnvilSection[16];
    }

    public int X {
        get { return this.cx; }
    }

    public int Z {
        get { return this.cz; }
    }

    public AnvilSection[] Sections {
        get { return this.sections; }
    }

    public AlphaBlockCollection Blocks {
        get { return this.blockManager; }
    }

    public AnvilBiomeCollection Biomes {
        get { return this.biomeManager; }
    }

    public EntityCollection Entities {
        get { return this.entityManager; }
    }

    public NbtTree Tree {
        get { return this.tree; }
    }

    public bool IsTerrainPopulated {
        get { return this.tree.Root["Level"].ToTagCompound()["TerrainPopulated"].ToTagByte() == 1; }
        set { this.tree.Root["Level"].ToTagCompound()["TerrainPopulated"].ToTagByte().Data = (byte) (value ? 1 : 0); }
    }

    public static AnvilChunk Create(int x, int z) {
        AnvilChunk c = new AnvilChunk();

        c.cx = x;
        c.cz = z;

        c.BuildNBTTree();
        return c;
    }

    public static AnvilChunk Create(NbtTree tree) {
        AnvilChunk c = new AnvilChunk();
        return c.LoadTree(tree.Root);
    }

    public static AnvilChunk CreateVerified(NbtTree tree) {
        AnvilChunk c = new AnvilChunk();
        return c.LoadTreeSafe(tree.Root);
    }

    /// <summary>
    /// Updates the chunk's global world coordinates.
    /// </summary>
    /// <param name="x">Global X-coordinate.</param>
    /// <param name="z">Global Z-coordinate.</param>
    public virtual void SetLocation(int x, int z) {
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

    public bool Save(Stream outStream) {
        if(outStream == null || !outStream.CanWrite) {
            return false;
        }

        BuildConditional();

        NbtTree tree = new NbtTree();
        tree.Root["Level"] = BuildTree();

        tree.WriteTo(outStream);

        return true;
    }

    #region INbtObject<AnvilChunk> Members
    public AnvilChunk LoadTree(TagNode tree) {
        TagNodeCompound ctree = tree as TagNodeCompound;
        if(ctree == null) {
            return null;
        }

        this.tree = new NbtTree(ctree);

        TagNodeCompound level = this.tree.Root["Level"] as TagNodeCompound;

        TagNodeList sections = level["Sections"] as TagNodeList;
        foreach(TagNodeCompound section in sections) {
            AnvilSection anvilSection = new AnvilSection(section);
            if(anvilSection.Y < 0 || anvilSection.Y >= this.sections.Length) {
                continue;
            }

            this.sections[anvilSection.Y] = anvilSection;
        }

        FusedDataArray3[] blocksBA = new FusedDataArray3[this.sections.Length];
        YZXNibbleArray[] dataBA = new YZXNibbleArray[this.sections.Length];
        YZXNibbleArray[] skyLightBA = new YZXNibbleArray[this.sections.Length];
        YZXNibbleArray[] blockLightBA = new YZXNibbleArray[this.sections.Length];

        for(int i = 0; i < this.sections.Length; i++) {
            if(this.sections[i] == null) {
                this.sections[i] = new AnvilSection(i);
            }

            blocksBA[i] = new FusedDataArray3(this.sections[i].AddBlocks, this.sections[i].Blocks);
            dataBA[i] = this.sections[i].Data;
            skyLightBA[i] = this.sections[i].SkyLight;
            blockLightBA[i] = this.sections[i].BlockLight;
        }

        this.blocks = new CompositeDataArray3(blocksBA);
        this.data = new CompositeDataArray3(dataBA);
        this.skyLight = new CompositeDataArray3(skyLightBA);
        this.blockLight = new CompositeDataArray3(blockLightBA);

        this.heightMap = new ZXIntArray(XDIM, ZDIM, level["HeightMap"] as TagNodeIntArray);

        if(level.ContainsKey("Biomes")) {
            this.biomes = new ZXByteArray(XDIM, ZDIM, level["Biomes"] as TagNodeByteArray);
        } else {
            level["Biomes"] = new TagNodeByteArray(new byte[256]);
            this.biomes = new ZXByteArray(XDIM, ZDIM, level["Biomes"] as TagNodeByteArray);
            for(int x = 0; x < XDIM; x++) {
                for(int z = 0; z < ZDIM; z++) {
                    this.biomes[x, z] = BiomeType.Default;
                }
            }
        }

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
        this.biomeManager = new AnvilBiomeCollection(this.biomes);

        return this;
    }

    public AnvilChunk LoadTreeSafe(TagNode tree) {
        if(!ValidateTree(tree)) {
            return null;
        }

        return LoadTree(tree);
    }

    private bool ShouldIncludeSection(AnvilSection section) {
        int y = (section.Y + 1) * section.Blocks.YDim;
        for(int i = 0; i < this.heightMap.Length; i++) {
            if(this.heightMap[i] > y) {
                return true;
            }
        }

        return !section.CheckEmpty();
    }

    public TagNode BuildTree() {
        TagNodeCompound level = this.tree.Root["Level"] as TagNodeCompound;
        TagNodeCompound levelCopy = new TagNodeCompound();
        foreach(KeyValuePair<string, TagNode> node in level) {
            levelCopy.Add(node.Key, node.Value);
        }

        TagNodeList sections = new TagNodeList(TagType.TAG_COMPOUND);
        for(int i = 0; i < this.sections.Length; i++) {
            if(ShouldIncludeSection(this.sections[i])) {
                sections.Add(this.sections[i].BuildTree());
            }
        }

        levelCopy["Sections"] = sections;

        if(this.tileTicks.Count == 0) {
            levelCopy.Remove("TileTicks");
        }

        return levelCopy;
    }

    public bool ValidateTree(TagNode tree) {
        NbtVerifier v = new NbtVerifier(tree, LevelSchema);
        return v.Verify();
    }
    #endregion

    #region ICopyable<AnvilChunk> Members
    public AnvilChunk Copy() {
        return AnvilChunk.Create(this.tree.Copy());
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

        this.sections = new AnvilSection[16];
        TagNodeList sections = new TagNodeList(TagType.TAG_COMPOUND);

        for(int i = 0; i < this.sections.Length; i++) {
            this.sections[i] = new AnvilSection(i);
            sections.Add(this.sections[i].BuildTree());
        }

        FusedDataArray3[] blocksBA = new FusedDataArray3[this.sections.Length];
        YZXNibbleArray[] dataBA = new YZXNibbleArray[this.sections.Length];
        YZXNibbleArray[] skyLightBA = new YZXNibbleArray[this.sections.Length];
        YZXNibbleArray[] blockLightBA = new YZXNibbleArray[this.sections.Length];

        for(int i = 0; i < this.sections.Length; i++) {
            blocksBA[i] = new FusedDataArray3(this.sections[i].AddBlocks, this.sections[i].Blocks);
            dataBA[i] = this.sections[i].Data;
            skyLightBA[i] = this.sections[i].SkyLight;
            blockLightBA[i] = this.sections[i].BlockLight;
        }

        this.blocks = new CompositeDataArray3(blocksBA);
        this.data = new CompositeDataArray3(dataBA);
        this.skyLight = new CompositeDataArray3(skyLightBA);
        this.blockLight = new CompositeDataArray3(blockLightBA);

        TagNodeIntArray heightMap = new TagNodeIntArray(new int[elements2]);
        this.heightMap = new ZXIntArray(XDIM, ZDIM, heightMap);

        TagNodeByteArray biomes = new TagNodeByteArray(new byte[elements2]);
        this.biomes = new ZXByteArray(XDIM, ZDIM, biomes);
        for(int x = 0; x < XDIM; x++) {
            for(int z = 0; z < ZDIM; z++) {
                this.biomes[x, z] = BiomeType.Default;
            }
        }

        this.entities = new TagNodeList(TagType.TAG_COMPOUND);
        this.tileEntities = new TagNodeList(TagType.TAG_COMPOUND);
        this.tileTicks = new TagNodeList(TagType.TAG_COMPOUND);

        TagNodeCompound level = new TagNodeCompound();
        level.Add("Sections", sections);
        level.Add("HeightMap", heightMap);
        level.Add("Biomes", biomes);
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
