namespace Substrate.TileEntities;

using Substrate.Nbt;

public class TileEntityMobSpawner : TileEntity {
    public static readonly SchemaNodeCompound MobSpawnerSchema = TileEntity.Schema.MergeInto(new SchemaNodeCompound("") {
        new SchemaNodeString("id", TypeId),
        new SchemaNodeScaler("EntityId", TagType.TAG_STRING),
        new SchemaNodeScaler("Delay", TagType.TAG_SHORT),
        new SchemaNodeScaler("MaxSpawnDelay", TagType.TAG_SHORT, SchemaOptions.OPTIONAL),
        new SchemaNodeScaler("MinSpawnDelay", TagType.TAG_SHORT, SchemaOptions.OPTIONAL),
        new SchemaNodeScaler("SpawnCount", TagType.TAG_SHORT, SchemaOptions.OPTIONAL),
        new SchemaNodeScaler("SpawnRange", TagType.TAG_SHORT, SchemaOptions.OPTIONAL),
        new SchemaNodeScaler("MaxNearbyEnemies", TagType.TAG_SHORT, SchemaOptions.OPTIONAL),
        new SchemaNodeScaler("RequiredPlayerRange", TagType.TAG_SHORT, SchemaOptions.OPTIONAL),
        new SchemaNodeScaler("MaxExperience", TagType.TAG_INT, SchemaOptions.OPTIONAL),
        new SchemaNodeScaler("RemainingExperience", TagType.TAG_INT, SchemaOptions.OPTIONAL),
        new SchemaNodeScaler("ExperienceRegenTick", TagType.TAG_INT, SchemaOptions.OPTIONAL),
        new SchemaNodeScaler("ExperienceRegenRate", TagType.TAG_INT, SchemaOptions.OPTIONAL),
        new SchemaNodeScaler("ExperienceRegenAmount", TagType.TAG_INT, SchemaOptions.OPTIONAL),
        new SchemaNodeCompound("SpawnData", SchemaOptions.OPTIONAL),
    });

    public static string TypeId {
        get { return "MobSpawner"; }
    }

    private short delay;
    private string entityID;
    private short? maxDelay;
    private short? minDelay;
    private short? spawnCount;
    private short? spawnRange;
    private short? maxNearbyEnemies;
    private short? requiredPlayerRange;
    private int? maxExperience;
    private int? remainingExperience;
    private int? experienceRegenTick;
    private int? experienceRegenRate;
    private int? experienceRegenAmount;
    private TagNodeCompound spawnData;

    public int Delay {
        get { return this.delay; }
        set { this.delay = (short) value; }
    }

    public TagNodeCompound SpawnData {
        get { return this.spawnData; }
        set { this.spawnData = value; }
    }

    public string EntityID {
        get { return this.entityID; }
        set { this.entityID = value; }
    }

    public short MaxSpawnDelay {
        get { return this.maxDelay ?? 0; }
        set { this.maxDelay = value; }
    }

    public short MinSpawnDelay {
        get { return this.minDelay ?? 0; }
        set { this.minDelay = value; }
    }

    public short SpawnCount {
        get { return this.spawnCount ?? 0; }
        set { this.spawnCount = value; }
    }

    public short SpawnRange {
        get { return this.spawnRange ?? 0; }
        set { this.spawnRange = value; }
    }

    public short MaxNearbyEnemies {
        get { return this.maxNearbyEnemies ?? 0; }
        set { this.maxNearbyEnemies = value; }
    }

    public short RequiredPlayerRange {
        get { return this.requiredPlayerRange ?? 0; }
        set { this.requiredPlayerRange = value; }
    }

    public int MaxExperience {
        get { return this.maxExperience ?? 0; }
        set { this.maxExperience = value; }
    }

    public int RemainingExperience {
        get { return this.remainingExperience ?? 0; }
        set { this.remainingExperience = value; }
    }

    public int ExperienceRegenTick {
        get { return this.experienceRegenTick ?? 0; }
        set { this.experienceRegenTick = value; }
    }

    public int ExperienceRegenRate {
        get { return this.experienceRegenRate ?? 0; }
        set { this.experienceRegenRate = value; }
    }

    public int ExperienceRegenAmount {
        get { return this.experienceRegenAmount ?? 0; }
        set { this.experienceRegenAmount = value; }
    }

    protected TileEntityMobSpawner(string id) : base(id) { }

    public TileEntityMobSpawner() : this(TypeId) { }

    public TileEntityMobSpawner(TileEntity te) : base(te) {
        TileEntityMobSpawner tes = te as TileEntityMobSpawner;
        if(tes != null) {
            this.delay = tes.delay;
            this.entityID = tes.entityID;
            this.maxDelay = tes.maxDelay;
            this.minDelay = tes.minDelay;
            this.spawnCount = tes.spawnCount;
            this.spawnRange = tes.spawnRange;
            this.maxNearbyEnemies = tes.maxNearbyEnemies;
            this.requiredPlayerRange = tes.requiredPlayerRange;
            this.maxExperience = tes.maxExperience;
            this.remainingExperience = tes.remainingExperience;
            this.experienceRegenTick = tes.experienceRegenTick;
            this.experienceRegenRate = tes.experienceRegenRate;
            this.experienceRegenAmount = tes.experienceRegenAmount;

            if(tes.spawnData != null) {
                this.spawnData = tes.spawnData.Copy() as TagNodeCompound;
            }
        }
    }

    #region ICopyable<TileEntity> Members
    public override TileEntity Copy() {
        return new TileEntityMobSpawner(this);
    }
    #endregion

    #region INBTObject<TileEntity> Members
    public override TileEntity LoadTree(TagNode tree) {
        TagNodeCompound ctree = tree as TagNodeCompound;
        if(ctree == null || base.LoadTree(tree) == null) {
            return null;
        }

        this.delay = ctree["Delay"].ToTagShort();
        this.entityID = ctree["EntityId"].ToTagString();

        if(ctree.ContainsKey("MaxSpawnDelay")) {
            this.maxDelay = ctree["MaxSpawnDelay"].ToTagShort();
        }

        if(ctree.ContainsKey("MinSpawnDelay")) {
            this.minDelay = ctree["MinSpawnDelay"].ToTagShort();
        }

        if(ctree.ContainsKey("SpawnCount")) {
            this.spawnCount = ctree["SpawnCount"].ToTagShort();
        }

        if(ctree.ContainsKey("SpawnRange")) {
            this.spawnRange = ctree["SpawnRange"].ToTagShort();
        }

        if(ctree.ContainsKey("MaxNearbyEnemies")) {
            this.maxNearbyEnemies = ctree["MaxNearbyEnemies"].ToTagShort();
        }

        if(ctree.ContainsKey("RequiredPlayerRange")) {
            this.requiredPlayerRange = ctree["RequiredPlayerRange"].ToTagShort();
        }

        if(ctree.ContainsKey("MaxExperience")) {
            this.maxExperience = ctree["MaxExperience"].ToTagInt();
        }

        if(ctree.ContainsKey("RemainingExperience")) {
            this.remainingExperience = ctree["RemainingExperience"].ToTagInt();
        }

        if(ctree.ContainsKey("ExperienceRegenTick")) {
            this.experienceRegenTick = ctree["ExperienceRegenTick"].ToTagInt();
        }

        if(ctree.ContainsKey("ExperienceRegenRate")) {
            this.experienceRegenRate = ctree["ExperienceRegenRate"].ToTagInt();
        }

        if(ctree.ContainsKey("ExperienceRegenAmount")) {
            this.experienceRegenRate = ctree["ExperienceRegenAmount"].ToTagInt();
        }

        if(ctree.ContainsKey("SpawnData")) {
            this.spawnData = ctree["SpawnData"].ToTagCompound();
        }
        return this;
    }

    public override TagNode BuildTree() {
        TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
        tree["EntityId"] = new TagNodeString(this.entityID);
        tree["Delay"] = new TagNodeShort(this.delay);

        if(this.maxDelay != null) {
            tree["MaxSpawnDelay"] = new TagNodeShort(this.maxDelay ?? 0);
        }

        if(this.minDelay != null) {
            tree["MinSpawnDelay"] = new TagNodeShort(this.minDelay ?? 0);
        }

        if(this.spawnCount != null) {
            tree["SpawnCount"] = new TagNodeShort(this.spawnCount ?? 0);
        }

        if(this.spawnRange != null) {
            tree["SpawnRange"] = new TagNodeShort(this.spawnRange ?? 0);
        }

        if(this.maxNearbyEnemies != null) {
            tree["MaxNearbyEnemies"] = new TagNodeShort(this.maxNearbyEnemies ?? 0);
        }

        if(this.requiredPlayerRange != null) {
            tree["RequiredPlayerRange"] = new TagNodeShort(this.requiredPlayerRange ?? 0);
        }

        if(this.maxExperience != null) {
            tree["MaxExperience"] = new TagNodeInt(this.maxExperience ?? 0);
        }

        if(this.remainingExperience != null) {
            tree["RemainingExperience"] = new TagNodeInt(this.remainingExperience ?? 0);
        }

        if(this.experienceRegenTick != null) {
            tree["ExperienceRegenTick"] = new TagNodeInt(this.experienceRegenTick ?? 0);
        }

        if(this.experienceRegenRate != null) {
            tree["ExperienceRegenRate"] = new TagNodeInt(this.experienceRegenRate ?? 0);
        }

        if(this.experienceRegenAmount != null) {
            tree["ExperienceRegenAmount"] = new TagNodeInt(this.experienceRegenAmount ?? 0);
        }

        if(this.spawnData != null && this.spawnData.Count > 0) {
            tree["SpawnData"] = this.spawnData;
        }
        return tree;
    }

    public override bool ValidateTree(TagNode tree) {
        return new NbtVerifier(tree, MobSpawnerSchema).Verify();
    }
    #endregion
}
