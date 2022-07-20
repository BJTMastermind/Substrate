namespace Substrate;

using Substrate.Core;
using Substrate.Nbt;

/// <summary>
/// Encompases data to specify game rules.
/// </summary>
public class GameRules : ICopyable<GameRules> {
    private bool commandBlockOutput = true;
    private bool doFireTick = true;
    private bool doMobLoot = true;
    private bool doMobSpawning = true;
    private bool doTileDrops = true;
    private bool keepInventory = false;
    private bool mobGriefing = true;

    /// <summary>
    /// Gets or sets whether or not actions performed by command blocks are displayed in the chat.
    /// </summary>
    public bool CommandBlockOutput {
        get { return this.commandBlockOutput; }
        set { this.commandBlockOutput = value; }
    }

    /// <summary>
    /// Gets or sets whether to spread or remove fire.
    /// </summary>
    public bool DoFireTick {
        get { return this.doFireTick; }
        set { this.doFireTick = value; }
    }

    /// <summary>
    /// Gets or sets whether mobs should drop loot when killed.
    /// </summary>
    public bool DoMobLoot {
        get { return this.doMobLoot; }
        set { this.doMobLoot = value; }
    }

    /// <summary>
    /// Gets or sets whether mobs should spawn naturally.
    /// </summary>
    public bool DoMobSpawning {
        get { return this.doMobSpawning; }
        set { this.doMobSpawning = value; }
    }

    /// <summary>
    /// Gets or sets whether breaking blocks should drop the block's item drop.
    /// </summary>
    public bool DoTileDrops {
        get { return this.doTileDrops; }
        set { this.doTileDrops = value; }
    }

    /// <summary>
    /// Gets or sets whether players keep their inventory after they die.
    /// </summary>
    public bool KeepInventory {
        get { return this.keepInventory; }
        set { this.keepInventory = value; }
    }

    /// <summary>
    /// Gets or sets whether mobs can destroy blocks (creeper explosions, zombies breaking doors, etc.).
    /// </summary>
    public bool MobGriefing {
        get { return this.mobGriefing; }
        set { this.mobGriefing = value; }
    }

    #region ICopyable<GameRules> Members
    /// <inheritdoc />
    public GameRules Copy() {
        GameRules gr = new GameRules();
        gr.commandBlockOutput = this.commandBlockOutput;
        gr.doFireTick = this.doFireTick;
        gr.doMobLoot = this.doMobLoot;
        gr.doMobSpawning = this.doMobSpawning;
        gr.doTileDrops = this.doTileDrops;
        gr.keepInventory = this.keepInventory;
        gr.mobGriefing = this.mobGriefing;

        return gr;
    }
    #endregion
}

/// <summary>
/// Specifies the type of gameplay associated with a world.
/// </summary>
public enum GameType {
    /// <summary>
    /// The world will be played in Survival mode.
    /// </summary>
    SURVIVAL = 0,

    /// <summary>
    /// The world will be played in Creative mode.
    /// </summary>
    CREATIVE = 1,
}

public enum TimeOfDay {
    Daytime = 0,
    Noon = 6000,
    Sunset = 12000,
    Nighttime = 13800,
    Midnight = 18000,
    Sunrise = 22200,
}

/// <summary>
/// Represents general data and metadata of a single world.
/// </summary>
public class Level : INbtObject<Level>, ICopyable<Level> {
    private static SchemaNodeCompound schema = new SchemaNodeCompound() {
        new SchemaNodeCompound("Data") {
            new SchemaNodeScaler("Time", TagType.TAG_LONG),
            new SchemaNodeScaler("LastPlayed", TagType.TAG_LONG, SchemaOptions.CREATE_ON_MISSING),
            new SchemaNodeCompound("Player", Player.Schema, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("SpawnX", TagType.TAG_INT),
            new SchemaNodeScaler("SpawnY", TagType.TAG_INT),
            new SchemaNodeScaler("SpawnZ", TagType.TAG_INT),
            new SchemaNodeScaler("SizeOnDisk", TagType.TAG_LONG, SchemaOptions.CREATE_ON_MISSING),
            new SchemaNodeScaler("RandomSeed", TagType.TAG_LONG),
            new SchemaNodeScaler("version", TagType.TAG_INT, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("LevelName", TagType.TAG_STRING, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("generatorName", TagType.TAG_STRING, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("raining", TagType.TAG_BYTE, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("thundering", TagType.TAG_BYTE, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("rainTime", TagType.TAG_INT, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("thunderTime", TagType.TAG_INT, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("GameType", TagType.TAG_INT, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("MapFeatures", TagType.TAG_BYTE, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("hardcore", TagType.TAG_BYTE, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("generatorVersion", TagType.TAG_INT, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("generatorOptions", TagType.TAG_STRING, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("initialized", TagType.TAG_BYTE, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("allowCommands", TagType.TAG_BYTE, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("DayTime", TagType.TAG_LONG, SchemaOptions.OPTIONAL),
            new SchemaNodeCompound("GameRules", SchemaOptions.OPTIONAL) {
                new SchemaNodeScaler("commandBlockOutput", TagType.TAG_STRING),
                new SchemaNodeScaler("doFireTick", TagType.TAG_STRING),
                new SchemaNodeScaler("doMobLoot", TagType.TAG_STRING),
                new SchemaNodeScaler("doMobSpawning", TagType.TAG_STRING),
                new SchemaNodeScaler("doTileDrops", TagType.TAG_STRING),
                new SchemaNodeScaler("keepInventory", TagType.TAG_STRING),
                new SchemaNodeScaler("mobGriefing", TagType.TAG_STRING),
            },
        },
    };

    private TagNodeCompound source;

    private NbtWorld world;

    private long time;
    private long lastPlayed;

    private Player player;

    private int spawnX;
    private int spawnY;
    private int spawnZ;

    private long sizeOnDisk;
    private long randomSeed;
    private int? version;
    private string name;
    private string generator;

    private byte? raining;
    private byte? thundering;
    private int? rainTime;
    private int? thunderTime;

    private int? gameType;
    private byte? mapFeatures;
    private byte? hardcore;

    private int? generatorVersion;
    private string generatorOptions;
    private byte? initialized;
    private byte? allowCommands;
    private long? _DayTime;

    private GameRules gameRules;

    /// <summary>
    /// Gets or sets the creation time of the world as a long timestamp.
    /// </summary>
    public long Time {
        get { return this.time; }
        set { this.time = value; }
    }

    /// <summary>
    /// Gets or sets the time that the world was last played as a long timestamp.
    /// </summary>
    public long LastPlayed {
        get { return this.lastPlayed; }
        set { this.lastPlayed = value; }
    }

    /// <summary>
    /// Gets or sets the player for single-player worlds.
    /// </summary>
    public Player Player {
        get { return this.player; }
        set {
            this.player = value;
            this.player.World = this.name;
        }
    }

    /// <summary>
    /// Gets or sets the world's spawn point.
    /// </summary>
    public SpawnPoint Spawn {
        get { return new SpawnPoint(this.spawnX, this.spawnY, this.spawnZ); }
        set {
            this.spawnX = value.X;
            this.spawnY = value.Y;
            this.spawnZ = value.Z;
        }
    }

    /// <summary>
    /// Gets the estimated size of the world in bytes.
    /// </summary>
    public long SizeOnDisk {
        get { return this.sizeOnDisk; }
    }

    /// <summary>
    /// Gets or sets the world's random seed.
    /// </summary>
    public long RandomSeed {
        get { return this.randomSeed; }
        set { this.randomSeed = value; }
    }

    /// <summary>
    /// Gets or sets the world's version number.
    /// </summary>
    public int Version {
        get { return this.version ?? 0; }
        set { this.version = value; }
    }

    /// <summary>
    /// Gets or sets the name of the world.
    /// </summary>
    /// <remarks>If there is a <see cref="Player"/> object attached to this world, the player's world field
    /// will also be updated.</remarks>
    public string LevelName {
        get { return this.name; }
        set {
            this.name = value;
            if(this.player != null) {
                this.player.World = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets the name of the world generator.
    /// </summary>
    /// <remarks>This should be 'default', 'flat', or 'largeBiomes'.</remarks>
    public string GeneratorName {
        get { return this.generator; }
        set { this.generator = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating that it is raining in the world.
    /// </summary>
    public bool IsRaining {
        get { return (this.raining ?? 0) == 1; }
        set { this.raining = value ? (byte) 1 : (byte) 0; }
    }

    /// <summary>
    /// Gets or sets a value indicating that it is thunderstorming in the world.
    /// </summary>
    public bool IsThundering {
        get { return (this.thundering ?? 0) == 1; }
        set { this.thundering = value ? (byte) 1 : (byte) 0; }
    }

    /// <summary>
    /// Gets or sets the timer value for controlling rain.
    /// </summary>
    public int RainTime {
        get { return this.rainTime ?? 0; }
        set { this.rainTime = value; }
    }

    /// <summary>
    /// Gets or sets the timer value for controlling thunderstorms.
    /// </summary>
    public int ThunderTime {
        get { return this.thunderTime ?? 0; }
        set { this.thunderTime = value; }
    }

    /// <summary>
    /// Gets or sets the game type associated with this world.
    /// </summary>
    public GameType GameType {
        get { return (GameType) (this.gameType ?? 0); }
        set { this.gameType = (int) value; }
    }

    /// <summary>
    /// Gets or sets a value indicating that structures (dungeons, villages, ...) will be generated.
    /// </summary>
    public bool UseMapFeatures {
        get { return (this.mapFeatures ?? 0) == 1; }
        set { this.mapFeatures = value ? (byte) 1 : (byte) 0; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the map is hardcore mode
    /// </summary>
    public bool Hardcore {
        get { return (this.hardcore ?? 0) == 1; }
        set { this.hardcore = value ? (byte) 1 : (byte) 0; }
    }

    /// <summary>
    /// Gets or sets a value indicating the version of the level generator
    /// </summary>
    public int GeneratorVersion {
        get { return this.generatorVersion ?? 0; }
        set { this.generatorVersion = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating controls options for the generator,
    /// currently only the Superflat generator. The format is a comma separated
    /// list of block IDs from the bottom of the map up, and each block ID may
    /// optionally be preceded by the number of layers and an x.
    /// Damage values are not supported.
    /// </summary>
    public string GeneratorOptions {
        get { return this.generatorOptions ?? ""; }
        set { this.generatorOptions = value; }
    }

    /// <summary>
    /// Gets or sets a value, normally true, indicating whether a world has been
    /// initialized properly after creation. If the initial simulation was canceled
    /// somehow, this can be false and the world will be re-initialized on next load.
    /// </summary>
    public bool Initialized {
        get { return (this.initialized ?? 0) == 1; }
        set { this.initialized = value ? (byte) 1 : (byte) 0; }
    }

    /// <summary>
    /// Gets or sets a value indicating if cheats are enabled.
    /// </summary>
    public bool AllowCommands {
        get { return (this.allowCommands ?? 0) == 1; }
        set { this.allowCommands = value ? (byte) 1 : (byte) 0; }
    }

    /// <summary>
    /// Gets or sets a value indicating the time of day.
    /// 0 is sunrise, 6000 is midday, 12000 is sunset,
    /// 18000 is midnight, 24000 is the next day's 0.
    /// This value keeps counting past 24000 and does not reset to 0
    /// </summary>
    public long DayTime {
        get { return this._DayTime ?? 0; }
        set { this._DayTime = value; }
    }

    /// <summary>
    /// Gets the level's game rules.
    /// </summary>
    public GameRules GameRules {
        get { return this.gameRules; }
    }

    /// <summary>
    /// Gets the source <see cref="TagNodeCompound"/> used to create this <see cref="Level"/> if it exists.
    /// </summary>
    public TagNodeCompound Source {
        get { return this.source; }
    }

    /// <summary>
    /// Gets a <see cref="SchemaNode"/> representing the schema of a level.
    /// </summary>
    public static SchemaNodeCompound Schema {
        get { return schema; }
    }

    /// <summary>
    /// Creates a new <see cref="Level"/> object with reasonable defaults tied to the given world.
    /// </summary>
    /// <param name="world">The world that the <see cref="Level"/> should be tied to.</param>
    public Level(NbtWorld world) {
        this.world = world;

        // Sane defaults
        this.time = 0;
        this.lastPlayed = 0;
        this.spawnX = 0;
        this.spawnY = 64;
        this.spawnZ = 0;
        this.sizeOnDisk = 0;
        this.randomSeed = new Random().Next();
        //version = 19132;
        this.version = 19133;
        this.name = "Untitled";
        this.generator = "default";
        this.hardcore = 0;

        this.generatorOptions = "";
        this.generatorVersion = 1;
        this.initialized = 0;
        this.allowCommands = 0;
        this._DayTime = 0;
        this.gameRules = new GameRules();

        GameType = GameType.SURVIVAL;
        UseMapFeatures = true;

        this.source = new TagNodeCompound();
    }

    /// <summary>
    /// Creates a copy of an existing <see cref="Level"/> object.
    /// </summary>
    /// <param name="p">The <see cref="Level"/> object to copy.</param>
    protected Level(Level p) {
        this.world = p.world;

        this.time = p.time;
        this.lastPlayed = p.lastPlayed;
        this.spawnX = p.spawnX;
        this.spawnY = p.spawnY;
        this.spawnZ = p.spawnZ;
        this.sizeOnDisk = p.sizeOnDisk;
        this.randomSeed = p.randomSeed;
        this.version = p.version;
        this.name = p.name;
        this.generator = p.generator;

        this.raining = p.raining;
        this.thundering = p.thundering;
        this.rainTime = p.rainTime;
        this.thunderTime = p.thunderTime;

        this.gameType = p.gameType;
        this.mapFeatures = p.mapFeatures;
        this.hardcore = p.hardcore;

        this.generatorVersion = p.generatorVersion;
        this.generatorOptions = p.generatorOptions;
        this.initialized = p.initialized;
        this.allowCommands = p.allowCommands;
        this._DayTime = p._DayTime;
        this.gameRules = p.gameRules.Copy();

        if(p.player != null) {
            this.player = p.player.Copy();
        }

        if(p.source != null) {
            this.source = p.source.Copy() as TagNodeCompound;
        }
    }

    /// <summary>
    /// Creates a default player entry for this world.
    /// </summary>
    public void SetDefaultPlayer() {
        this.player = new Player();
        this.player.World = this.name;

        this.player.Position.X = this.spawnX;
        this.player.Position.Y = this.spawnY + 1.7;
        this.player.Position.Z = this.spawnZ;
    }

    /// <summary>
    /// Saves a <see cref="Level"/> object to disk as a standard compressed NBT stream.
    /// </summary>
    /// <returns>True if the level was saved; false otherwise.</returns>
    /// <exception cref="LevelIOException">Thrown when an error is encountered writing out the level.</exception>
    public bool Save() {
        if(this.world == null) {
            return false;
        }

        try {
            NBTFile nf = new NBTFile(Path.Combine(this.world.Path, "level.dat"));
            using(Stream zipstr = nf.GetDataOutputStream()) {
                if(zipstr == null) {
                    NbtIOException nex = new NbtIOException("Failed to initialize compressed NBT stream for output");
                    nex.Data["Level"] = this;
                    throw nex;
                }

                new NbtTree(BuildTree() as TagNodeCompound).WriteTo(zipstr);
            }

            return true;
        } catch(Exception ex) {
            LevelIOException lex = new LevelIOException("Could not save level file.", ex);
            lex.Data["Level"] = this;
            throw lex;
        }
    }

    #region INBTObject<Player> Members
    /// <summary>
    /// Attempt to load a Level subtree into the <see cref="Level"/> without validation.
    /// </summary>
    /// <param name="tree">The root node of a Level subtree.</param>
    /// <returns>The <see cref="Level"/> returns itself on success, or null if the tree was unparsable.</returns>
    public virtual Level LoadTree(TagNode tree) {
        TagNodeCompound dtree = tree as TagNodeCompound;
        if(dtree == null) {
            return null;
        }

        this.version = null;
        this.raining = null;
        this.rainTime = null;
        this.thundering = null;
        this.thunderTime = null;
        this.gameType = null;
        this.mapFeatures = null;
        this.generatorOptions = null;
        this.generatorVersion = null;
        this.allowCommands = null;
        this.initialized = null;
        this._DayTime = null;

        TagNodeCompound ctree = dtree["Data"].ToTagCompound();

        this.time = ctree["Time"].ToTagLong();
        this.lastPlayed = ctree["LastPlayed"].ToTagLong();

        if(ctree.ContainsKey("Player")) {
            this.player = new Player().LoadTree(ctree["Player"]);
        }

        this.spawnX = ctree["SpawnX"].ToTagInt();
        this.spawnY = ctree["SpawnY"].ToTagInt();
        this.spawnZ = ctree["SpawnZ"].ToTagInt();

        this.sizeOnDisk = ctree["SizeOnDisk"].ToTagLong();
        this.randomSeed = ctree["RandomSeed"].ToTagLong();

        if(ctree.ContainsKey("version")) {
            this.version = ctree["version"].ToTagInt();
        }

        if(ctree.ContainsKey("LevelName")) {
            this.name = ctree["LevelName"].ToTagString();
        }

        if(ctree.ContainsKey("generatorName")) {
            this.generator = ctree["generatorName"].ToTagString();
        }

        if(ctree.ContainsKey("raining")) {
            this.raining = ctree["raining"].ToTagByte();
        }

        if(ctree.ContainsKey("thundering")) {
            this.thundering = ctree["thundering"].ToTagByte();
        }

        if(ctree.ContainsKey("rainTime")) {
            this.rainTime = ctree["rainTime"].ToTagInt();
        }

        if(ctree.ContainsKey("thunderTime")) {
            this.thunderTime = ctree["thunderTime"].ToTagInt();
        }

        if(ctree.ContainsKey("GameType")) {
            this.gameType = ctree["GameType"].ToTagInt();
        }

        if(ctree.ContainsKey("MapFeatures")) {
            this.mapFeatures = ctree["MapFeatures"].ToTagByte();
        }

        if(ctree.ContainsKey("hardcore")) {
            this.hardcore = ctree["hardcore"].ToTagByte();
        }

        if(ctree.ContainsKey("generatorVersion")) {
            this.generatorVersion = ctree["generatorVersion"].ToTagInt();
        }

        if(ctree.ContainsKey("generatorOptions")) {
            this.generatorOptions = ctree["generatorOptions"].ToTagString();
        }

        if(ctree.ContainsKey("allowCommands")) {
            this.allowCommands = ctree["allowCommands"].ToTagByte();
        }

        if(ctree.ContainsKey("initialized")) {
            this.initialized = ctree["initialized"].ToTagByte();
        }

        if(ctree.ContainsKey("DayTime")) {
            this._DayTime = ctree["DayTime"].ToTagLong();
        }

        if(ctree.ContainsKey("GameRules")) {
            TagNodeCompound gr = ctree["GameRules"].ToTagCompound();

            this.gameRules = new GameRules();
            this.gameRules.CommandBlockOutput = gr["commandBlockOutput"].ToTagString().Data == "true";
            this.gameRules.DoFireTick = gr["doFireTick"].ToTagString().Data == "true";
            this.gameRules.DoMobLoot = gr["doMobLoot"].ToTagString().Data == "true";
            this.gameRules.DoMobSpawning = gr["doMobSpawning"].ToTagString().Data == "true";
            this.gameRules.DoTileDrops = gr["doTileDrops"].ToTagString().Data == "true";
            this.gameRules.KeepInventory = gr["keepInventory"].ToTagString().Data == "true";
            this.gameRules.MobGriefing = gr["mobGriefing"].ToTagString().Data == "true";
        }

        this.source = ctree.Copy() as TagNodeCompound;
        return this;
    }

    /// <summary>
    /// Attempt to load a Level subtree into the <see cref="Level"/> with validation.
    /// </summary>
    /// <param name="tree">The root node of a Level subtree.</param>
    /// <returns>The <see cref="Level"/> returns itself on success, or null if the tree failed validation.</returns>
    public virtual Level LoadTreeSafe(TagNode tree) {
        if(!ValidateTree(tree)) {
            return null;
        }
        return LoadTree(tree);
    }

    /// <summary>
    /// Builds a Level subtree from the current data.
    /// </summary>
    /// <returns>The root node of a Level subtree representing the current data.</returns>
    public virtual TagNode BuildTree() {
        TagNodeCompound data = new TagNodeCompound();
        data["Time"] = new TagNodeLong(this.time);
        data["LastPlayed"] = new TagNodeLong(this.lastPlayed);

        if(this.player != null) {
            data["Player"] = this.player.BuildTree();
        }

        data["SpawnX"] = new TagNodeInt(this.spawnX);
        data["SpawnY"] = new TagNodeInt(this.spawnY);
        data["SpawnZ"] = new TagNodeInt(this.spawnZ);
        data["SizeOnDisk"] = new TagNodeLong(this.sizeOnDisk);
        data["RandomSeed"] = new TagNodeLong(this.randomSeed);

        if(this.version != null && this.version != 0) {
            data["version"] = new TagNodeInt(this.version ?? 0);
        }

        if(this.name != null) {
            data["LevelName"] = new TagNodeString(this.name);
        }

        if(this.generator != null) {
            data["generatorName"] = new TagNodeString(this.generator);
        }

        if(this.raining != null) {
            data["raining"] = new TagNodeByte(this.raining ?? 0);
        }

        if(this.thundering != null) {
            data["thundering"] = new TagNodeByte(this.thundering ?? 0);
        }

        if(this.rainTime != null) {
            data["rainTime"] = new TagNodeInt(this.rainTime ?? 0);
        }

        if(this.thunderTime != null) {
            data["thunderTime"] = new TagNodeInt(this.thunderTime ?? 0);
        }

        if(this.gameType != null) {
            data["GameType"] = new TagNodeInt(this.gameType ?? 0);
        }

        if(this.mapFeatures != null) {
            data["MapFeatures"] = new TagNodeByte(this.mapFeatures ?? 0);
        }

        if(this.hardcore != null) {
            data["hardcore"] = new TagNodeByte(this.hardcore ?? 0);
        }

        if(this.generatorOptions != null) {
            data["generatorOptions"] = new TagNodeString(this.generatorOptions);
        }

        if(this.generatorVersion != null) {
            data["generatorVersion"] = new TagNodeInt(this.generatorVersion ?? 0);
        }

        if(this.allowCommands != null) {
            data["allowCommands"] = new TagNodeByte(this.allowCommands ?? 0);
        }

        if(this.initialized != null) {
            data["initialized"] = new TagNodeByte(this.initialized ?? 0);
        }

        if(this._DayTime != null) {
            data["DayTime"] = new TagNodeLong(this._DayTime ?? 0);
        }

        TagNodeCompound gr = new TagNodeCompound();
        gr["commandBlockOutput"] = new TagNodeString(this.gameRules.CommandBlockOutput ? "true" : "false");
        gr["doFireTick"] = new TagNodeString(this.gameRules.DoFireTick ? "true" : "false");
        gr["doMobLoot"] = new TagNodeString(this.gameRules.DoMobLoot ? "true" : "false");
        gr["doMobSpawning"] = new TagNodeString(this.gameRules.DoMobSpawning ? "true" : "false");
        gr["doTileDrops"] = new TagNodeString(this.gameRules.DoTileDrops ? "true" : "false");
        gr["keepInventory"] = new TagNodeString(this.gameRules.KeepInventory ? "true" : "false");
        gr["mobGriefing"] = new TagNodeString(this.gameRules.MobGriefing ? "true" : "false");
        data["GameRules"] = gr;

        if(this.source != null) {
            data.MergeFrom(this.source);
        }

        TagNodeCompound tree = new TagNodeCompound();
        tree.Add("Data", data);

        return tree;
    }

    /// <summary>
    /// Validate a Level subtree against a schema defintion.
    /// </summary>
    /// <param name="tree">The root node of a Level subtree.</param>
    /// <returns>Status indicating whether the tree was valid against the internal schema.</returns>
    public virtual bool ValidateTree(TagNode tree) {
        return new NbtVerifier(tree, schema).Verify();
    }
    #endregion

    #region ICopyable<Entity> Members
    /// <summary>
    /// Creates a deep-copy of the <see cref="Level"/>.
    /// </summary>
    /// <returns>A deep-copy of the <see cref="Level"/>, including a copy of the <see cref="Player"/>, if one is attached.</returns>
    public virtual Level Copy() {
        return new Level(this);
    }
    #endregion
}
