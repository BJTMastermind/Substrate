namespace Substrate;

using Substrate.Core;
using Substrate.Nbt;

/// <summary>
/// Encompases data to specify player abilities, especially mode-dependent abilities.
/// </summary>
/// <remarks>Whether or not any of these values are respected by the game client is dependent upon the active game mode.</remarks>
public class PlayerAbilities : ICopyable<PlayerAbilities> {
    private bool flying = false;
    private bool instabuild = false;
    private bool mayfly = false;
    private bool invulnerable = false;
    private bool maybuild = true;

    private float walkSpeed = 0.1f;
    private float flySpeed = 0.05f;

    /// <summary>
    /// Gets or sets whether the player is currently flying.
    /// </summary>
    public bool Flying {
        get { return this.flying; }
        set { this.flying = value; }
    }

    /// <summary>
    /// Gets or sets whether the player can instantly build or mine.
    /// </summary>
    public bool InstantBuild {
        get { return this.instabuild; }
        set { this.instabuild = value; }
    }

    /// <summary>
    /// Gets or sets whether the player is allowed to fly.
    /// </summary>
    public bool MayFly {
        get { return this.mayfly; }
        set { this.mayfly = value; }
    }

    /// <summary>
    /// Gets or sets whether the player can take damage.
    /// </summary>
    public bool Invulnerable {
        get { return this.invulnerable; }
        set { this.invulnerable = value; }
    }

    /// <summary>
    /// Gets or sets whether the player can create or destroy blocks.
    /// </summary>
    public bool MayBuild {
        get { return this.maybuild; }
        set { this.maybuild = value; }
    }

    /// <summary>
    /// Gets or sets the player's walking speed.  Always 0.1.
    /// </summary>
    public float FlySpeed {
        get { return this.flySpeed; }
        set { this.flySpeed = value; }
    }

    /// <summary>
    /// Gets or sets the player's flying speed.  Always 0.05.
    /// </summary>
    public float WalkSpeed {
        get { return this.walkSpeed; }
        set { this.walkSpeed = value; }
    }

    #region ICopyable<PlayerAbilities> Members
    /// <inheritdoc />
    public PlayerAbilities Copy() {
        PlayerAbilities pa = new PlayerAbilities();
        pa.flying = this.flying;
        pa.instabuild = this.instabuild;
        pa.mayfly = this.mayfly;
        pa.invulnerable = this.invulnerable;
        pa.maybuild = this.maybuild;
        pa.walkSpeed = this.walkSpeed;
        pa.flySpeed = this.flySpeed;

        return pa;
    }
    #endregion
}

public enum PlayerGameType {
    Survival = 0,
    Creative = 1,
    Adventure = 2,
    Spectator = 3,
}

/// <summary>
/// Represents a Player from either single- or multi-player Minecraft.
/// </summary>
/// <remarks>Unlike <see cref="TypedEntity"/> objects, <see cref="Player"/> objects do not need to be added to chunks.  They
/// are stored individually or within level data.</remarks>
public class Player : Entity, INbtObject<Player>, ICopyable<Player>, IItemContainer {
    private static readonly SchemaNodeCompound schema = Entity.Schema.MergeInto(new SchemaNodeCompound("") {
        new SchemaNodeScaler("AttackTime", TagType.TAG_SHORT, SchemaOptions.CREATE_ON_MISSING),
        new SchemaNodeScaler("DeathTime", TagType.TAG_SHORT),
        new SchemaNodeScaler("Health", TagType.TAG_FLOAT),
        new SchemaNodeScaler("HurtTime", TagType.TAG_SHORT),
        new SchemaNodeScaler("Dimension", TagType.TAG_INT),
        new SchemaNodeList("Inventory", TagType.TAG_COMPOUND, ItemCollection.Schema),
        //new SchemaNodeList("EnderItems", TagType.TAG_COMPOUND, ItemCollection.Schema, SchemaOptions.OPTIONAL),
        new SchemaNodeScaler("World", TagType.TAG_STRING, SchemaOptions.OPTIONAL),
        new SchemaNodeScaler("Sleeping", TagType.TAG_BYTE, SchemaOptions.CREATE_ON_MISSING),
        new SchemaNodeScaler("SleepTimer", TagType.TAG_SHORT, SchemaOptions.CREATE_ON_MISSING),
        new SchemaNodeScaler("SpawnX", TagType.TAG_INT, SchemaOptions.OPTIONAL),
        new SchemaNodeScaler("SpawnY", TagType.TAG_INT, SchemaOptions.OPTIONAL),
        new SchemaNodeScaler("SpawnZ", TagType.TAG_INT, SchemaOptions.OPTIONAL),
        new SchemaNodeScaler("foodLevel", TagType.TAG_INT, SchemaOptions.OPTIONAL),
        new SchemaNodeScaler("foodTickTimer", TagType.TAG_INT, SchemaOptions.OPTIONAL),
        new SchemaNodeScaler("foodExhaustionLevel", TagType.TAG_FLOAT, SchemaOptions.OPTIONAL),
        new SchemaNodeScaler("foodSaturationLevel", TagType.TAG_FLOAT, SchemaOptions.OPTIONAL),
        new SchemaNodeScaler("XpP", TagType.TAG_FLOAT, SchemaOptions.OPTIONAL),
        new SchemaNodeScaler("XpLevel", TagType.TAG_INT, SchemaOptions.OPTIONAL),
        new SchemaNodeScaler("XpTotal", TagType.TAG_INT, SchemaOptions.OPTIONAL),
        new SchemaNodeScaler("Score", TagType.TAG_INT, SchemaOptions.OPTIONAL),
        new SchemaNodeScaler("playerGameType", TagType.TAG_INT, SchemaOptions.OPTIONAL),
        new SchemaNodeCompound("abilities", new SchemaNodeCompound("") {
            new SchemaNodeScaler("flying", TagType.TAG_BYTE),
            new SchemaNodeScaler("instabuild", TagType.TAG_BYTE),
            new SchemaNodeScaler("mayfly", TagType.TAG_BYTE),
            new SchemaNodeScaler("invulnerable", TagType.TAG_BYTE),
            new SchemaNodeScaler("mayBuild", TagType.TAG_BYTE, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("walkSpeed", TagType.TAG_FLOAT, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("flySpeed", TagType.TAG_FLOAT, SchemaOptions.OPTIONAL),
        }, SchemaOptions.OPTIONAL),
    });

    private const int _CAPACITY = 105;
    private const int _ENDER_CAPACITY = 27;

    private short attackTime;
    private short deathTime;
    private float health;
    private short hurtTime;

    private int dimension;
    private byte sleeping;
    private short sleepTimer;
    private int? spawnX;
    private int? spawnY;
    private int? spawnZ;

    private int? foodLevel;
    private int? foodTickTimer;
    private float? foodExhaustion;
    private float? foodSaturation;
    private float? xpP;
    private int? xpLevel;
    private int? xpTotal;
    private int? score;

    private string world;
    private string name;

    private PlayerAbilities abilities;
    private PlayerGameType? gameType;

    private ItemCollection inventory;
    private ItemCollection enderItems;

    /// <summary>
    /// Gets or sets the number of ticks left in the player's "invincibility shield" after last struck.
    /// </summary>
    public int AttackTime {
        get { return this.attackTime; }
        set { this.attackTime = (short) value; }
    }

    /// <summary>
    /// Gets or sets the number of ticks that the player has been dead for.
    /// </summary>
    public int DeathTime {
        get { return this.deathTime; }
        set { this.deathTime = (short) value; }
    }

    /// <summary>
    /// Gets or sets the amount of the player's health.
    /// </summary>
    public float Health {
        get { return this.health; }
        set { this.health = value; }
    }

    /// <summary>
    /// Gets or sets the player's Hurt Time value.
    /// </summary>
    public int HurtTime {
        get { return this.hurtTime; }
        set { this.hurtTime = (short) value; }
    }

    /// <summary>
    /// Gets or sets the dimension that the player is currently in.
    /// </summary>
    public int Dimension {
        get { return this.dimension; }
        set { this.dimension = value; }
    }

    public PlayerGameType GameType {
        get { return this.gameType ?? PlayerGameType.Survival; }
        set { this.gameType = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the player is sleeping in a bed.
    /// </summary>
    public bool IsSleeping {
        get { return this.sleeping == 1; }
        set { this.sleeping = (byte) (value ? 1 : 0); }
    }

    /// <summary>
    /// Gets or sets the player's Sleep Timer value.
    /// </summary>
    public int SleepTimer {
        get { return this.sleepTimer; }
        set { this.sleepTimer = (short) value; }
    }

    /// <summary>
    /// Gets or sets the player's personal spawn point, set by sleeping in beds.
    /// </summary>
    public SpawnPoint Spawn {
        get { return new SpawnPoint(this.spawnX ?? 0, this.spawnY ?? 0, this.spawnZ ?? 0); }
        set {
            this.spawnX = value.X;
            this.spawnY = value.Y;
            this.spawnZ = value.Z;
        }
    }

    /// <summary>
    /// Tests if the player currently has a personal spawn point.
    /// </summary>
    public bool HasSpawn {
        get { return this.spawnX != null && this.spawnY != null && this.spawnZ != null; }
    }

    /// <summary>
    /// Gets or sets the name of the world that the player is currently within.
    /// </summary>
    public string World {
        get { return this.world; }
        set { this.world = value; }
    }

    /// <summary>
    /// Gets or sets the name that is used when the player is read or written from a <see cref="PlayerManager"/>.
    /// </summary>
    public string Name {
        get { return this.name; }
        set { this.name = value; }
    }

    /// <summary>
    /// Gets or sets the player's score.
    /// </summary>
    public int Score {
        get { return this.score ?? 0; }
        set { this.score = value; }
    }

    /// <summary>
    /// Gets or sets the player's XP Level.
    /// </summary>
    public int XPLevel {
        get { return this.xpLevel ?? 0; }
        set { this.xpLevel = value; }
    }

    /// <summary>
    /// Gets or sets the amount of the player's XP points.
    /// </summary>
    public int XPTotal {
        get { return this.xpTotal ?? 0; }
        set { this.xpTotal = value; }
    }

    /// <summary>
    /// Gets or sets the hunger level of the player.  Valid values range 0 - 20.
    /// </summary>
    public int HungerLevel {
        get { return this.foodLevel ?? 0; }
        set { this.foodLevel = value; }
    }

    /// <summary>
    /// Gets or sets the player's hunger saturation level, which is reserve food capacity above <see cref="HungerLevel"/>.
    /// </summary>
    public float HungerSaturationLevel {
        get { return this.foodSaturation ?? 0; }
        set { this.foodSaturation = value; }
    }

    /// <summary>
    /// Gets or sets the counter towards the next hunger point decrement.  Valid values range 0.0 - 4.0.
    /// </summary>
    public float HungerExhaustionLevel {
        get { return this.foodExhaustion ?? 0; }
        set { this.foodExhaustion = value; }
    }

    /// <summary>
    /// Gets or sets the timer used to periodically heal or damage the player based on <see cref="HungerLevel"/>.  Valid values range 0 - 80.
    /// </summary>
    public int HungerTimer {
        get { return this.foodTickTimer ?? 0; }
        set { this.foodTickTimer = value; }
    }

    /// <summary>
    /// Gets the state of the player's abilities.
    /// </summary>
    public PlayerAbilities Abilities {
        get { return this.abilities; }
    }

    /// <summary>
    /// Creates a new <see cref="Player"/> object with reasonable default values.
    /// </summary>
    public Player() : base() {
        this.inventory = new ItemCollection(_CAPACITY);
        this.enderItems = new ItemCollection(_ENDER_CAPACITY);
        this.abilities = new PlayerAbilities();

        // Sane defaults
        this.dimension = 0;
        this.sleeping = 0;
        this.sleepTimer = 0;

        Air = 300;
        Health = 20.0f;
        Fire = -20;
    }

    /// <summary>
    /// Creates a copy of a <see cref="Player"/> object.
    /// </summary>
    /// <param name="p">The <see cref="Player"/> to copy fields from.</param>
    protected Player(Player p) : base(p) {
        this.attackTime = p.attackTime;
        this.deathTime = p.deathTime;
        this.health = p.health;
        this.hurtTime = p.hurtTime;

        this.dimension = p.dimension;
        this.gameType = p.gameType;
        this.sleeping = p.sleeping;
        this.sleepTimer = p.sleepTimer;
        this.spawnX = p.spawnX;
        this.spawnY = p.spawnY;
        this.spawnZ = p.spawnZ;
        this.world = p.world;
        this.inventory = p.inventory.Copy();
        this.enderItems = p.inventory.Copy();

        this.foodLevel = p.foodLevel;
        this.foodTickTimer = p.foodTickTimer;
        this.foodSaturation = p.foodSaturation;
        this.foodExhaustion = p.foodExhaustion;
        this.xpP = p.xpP;
        this.xpLevel = p.xpLevel;
        this.xpTotal = p.xpTotal;
        this.abilities = p.abilities.Copy();
    }

    /// <summary>
    /// Clears the player's personal spawn point.
    /// </summary>
    public void ClearSpawn() {
        this.spawnX = null;
        this.spawnY = null;
        this.spawnZ = null;
    }

    private bool AbilitiesSet() {
        return this.abilities.Flying
            || this.abilities.InstantBuild
            || this.abilities.MayFly
            || this.abilities.Invulnerable;
    }

    #region INBTObject<Player> Members
    /// <summary>
    /// Gets a <see cref="SchemaNode"/> representing the schema of a Player.
    /// </summary>
    public static new SchemaNodeCompound Schema {
        get { return schema; }
    }

    /// <summary>
    /// Attempt to load a Player subtree into the <see cref="Player"/> without validation.
    /// </summary>
    /// <param name="tree">The root node of a Player subtree.</param>
    /// <returns>The <see cref="Player"/> returns itself on success, or null if the tree was unparsable.</returns>
    public virtual new Player LoadTree(TagNode tree) {
        TagNodeCompound ctree = tree as TagNodeCompound;
        if(ctree == null || base.LoadTree(tree) == null) {
            return null;
        }

        this.attackTime = ctree["AttackTime"].ToTagShort();
        this.deathTime = ctree["DeathTime"].ToTagShort();
        this.health = ctree["Health"].ToTagFloat();
        this.hurtTime = ctree["HurtTime"].ToTagShort();

        this.dimension = ctree["Dimension"].ToTagInt();
        this.sleeping = ctree["Sleeping"].ToTagByte();
        this.sleepTimer = ctree["SleepTimer"].ToTagShort();

        if(ctree.ContainsKey("SpawnX")) {
            this.spawnX = ctree["SpawnX"].ToTagInt();
        }
        if(ctree.ContainsKey("SpawnY")) {
            this.spawnY = ctree["SpawnY"].ToTagInt();
        }
        if(ctree.ContainsKey("SpawnZ")) {
            this.spawnZ = ctree["SpawnZ"].ToTagInt();
        }

        if(ctree.ContainsKey("World")) {
            this.world = ctree["World"].ToTagString();
        }

        if(ctree.ContainsKey("foodLevel")) {
            this.foodLevel = ctree["foodLevel"].ToTagInt();
        }

        if(ctree.ContainsKey("foodTickTimer")) {
            this.foodTickTimer = ctree["foodTickTimer"].ToTagInt();
        }

        if(ctree.ContainsKey("foodExhaustionLevel")) {
            this.foodExhaustion = ctree["foodExhaustionLevel"].ToTagFloat();
        }

        if(ctree.ContainsKey("foodSaturationLevel")) {
            this.foodSaturation = ctree["foodSaturationLevel"].ToTagFloat();
        }

        if(ctree.ContainsKey("XpP")) {
            this.xpP = ctree["XpP"].ToTagFloat();
        }

        if(ctree.ContainsKey("XpLevel")) {
            this.xpLevel = ctree["XpLevel"].ToTagInt();
        }

        if(ctree.ContainsKey("XpTotal")) {
            this.xpTotal = ctree["XpTotal"].ToTagInt();
        }

        if(ctree.ContainsKey("Score")) {
            this.score = ctree["Score"].ToTagInt();
        }

        if(ctree.ContainsKey("abilities")) {
            TagNodeCompound pb = ctree["abilities"].ToTagCompound();

            this.abilities = new PlayerAbilities();
            this.abilities.Flying = pb["flying"].ToTagByte().Data == 1;
            this.abilities.InstantBuild = pb["instabuild"].ToTagByte().Data == 1;
            this.abilities.MayFly = pb["mayfly"].ToTagByte().Data == 1;
            this.abilities.Invulnerable = pb["invulnerable"].ToTagByte().Data == 1;

            if(pb.ContainsKey("mayBuild")) {
                this.abilities.MayBuild = pb["mayBuild"].ToTagByte().Data == 1;
            }

            if(pb.ContainsKey("walkSpeed")) {
                this.abilities.WalkSpeed = pb["walkSpeed"].ToTagFloat();
            }

            if(pb.ContainsKey("flySpeed")) {
                this.abilities.FlySpeed = pb["flySpeed"].ToTagFloat();
            }
        }

        if(ctree.ContainsKey("PlayerGameType")) {
            this.gameType = (PlayerGameType) ctree["PlayerGameType"].ToTagInt().Data;
        }

        this.inventory.LoadTree(ctree["Inventory"].ToTagList());

        if(ctree.ContainsKey("EnderItems")) {
            if(ctree["EnderItems"].ToTagList().Count > 0) {
                this.enderItems.LoadTree(ctree["EnderItems"].ToTagList());
            }
        }
        return this;
    }

    /// <summary>
    /// Attempt to load a Player subtree into the <see cref="Player"/> with validation.
    /// </summary>
    /// <param name="tree">The root node of a Player subtree.</param>
    /// <returns>The <see cref="Player"/> returns itself on success, or null if the tree failed validation.</returns>
    public virtual new Player LoadTreeSafe(TagNode tree) {
        if(!ValidateTree(tree)) {
            return null;
        }
        return LoadTree(tree);
    }

    /// <summary>
    /// Builds a Player subtree from the current data.
    /// </summary>
    /// <returns>The root node of a Player subtree representing the current data.</returns>
    public virtual new TagNode BuildTree() {
        TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
        tree["AttackTime"] = new TagNodeShort(this.attackTime);
        tree["DeathTime"] = new TagNodeShort(this.deathTime);
        tree["Health"] = new TagNodeFloat(this.health);
        tree["HurtTime"] = new TagNodeShort(this.hurtTime);

        tree["Dimension"] = new TagNodeInt(this.dimension);
        tree["Sleeping"] = new TagNodeByte(this.sleeping);
        tree["SleepTimer"] = new TagNodeShort(this.sleepTimer);

        if(this.spawnX != null && this.spawnY != null && this.spawnZ != null) {
            tree["SpawnX"] = new TagNodeInt(this.spawnX ?? 0);
            tree["SpawnY"] = new TagNodeInt(this.spawnY ?? 0);
            tree["SpawnZ"] = new TagNodeInt(this.spawnZ ?? 0);
        } else {
            tree.Remove("SpawnX");
            tree.Remove("SpawnY");
            tree.Remove("SpawnZ");
        }

        if(this.world != null) {
            tree["World"] = new TagNodeString(this.world);
        }

        if(this.foodLevel != null) {
            tree["foodLevel"] = new TagNodeInt(this.foodLevel ?? 0);
        }

        if(this.foodTickTimer != null) {
            tree["foodTickTimer"] = new TagNodeInt(this.foodTickTimer ?? 0);
        }

        if(this.foodExhaustion != null) {
            tree["foodExhaustionLevel"] = new TagNodeFloat(this.foodExhaustion ?? 0);
        }

        if(this.foodSaturation != null) {
            tree["foodSaturation"] = new TagNodeFloat(this.foodSaturation ?? 0);
        }

        if(this.xpP != null) {
            tree["XpP"] = new TagNodeFloat(this.xpP ?? 0);
        }

        if(this.xpLevel != null) {
            tree["XpLevel"] = new TagNodeInt(this.xpLevel ?? 0);
        }

        if(this.xpTotal != null) {
            tree["XpTotal"] = new TagNodeInt(this.xpTotal ?? 0);
        }

        if(this.score != null) {
            tree["Score"] = new TagNodeInt(this.score ?? 0);
        }

        if(this.gameType != null) {
            tree["playerGameType"] = new TagNodeInt((int) (this.gameType ?? PlayerGameType.Survival));
        }

        if(AbilitiesSet()) {
            TagNodeCompound pb = new TagNodeCompound();
            pb["flying"] = new TagNodeByte(this.abilities.Flying ? (byte) 1 : (byte) 0);
            pb["instabuild"] = new TagNodeByte(this.abilities.InstantBuild ? (byte) 1 : (byte) 0);
            pb["mayfly"] = new TagNodeByte(this.abilities.MayFly ? (byte) 1 : (byte) 0);
            pb["invulnerable"] = new TagNodeByte(this.abilities.Invulnerable ? (byte) 1 : (byte) 0);
            pb["mayBuild"] = new TagNodeByte(this.abilities.MayBuild ? (byte) 1 : (byte) 0);
            pb["walkSpeed"] = new TagNodeFloat(this.abilities.WalkSpeed);
            pb["flySpeed"] = new TagNodeFloat(this.abilities.FlySpeed);

            tree["abilities"] = pb;
        }

        tree["Inventory"] = this.inventory.BuildTree();
        tree["EnderItems"] = this.enderItems.BuildTree();

        return tree;
    }

    /// <summary>
    /// Validate a Player subtree against a schema defintion.
    /// </summary>
    /// <param name="tree">The root node of a Player subtree.</param>
    /// <returns>Status indicating whether the tree was valid against the internal schema.</returns>
    public virtual new bool ValidateTree(TagNode tree) {
        return new NbtVerifier(tree, schema).Verify();
    }
    #endregion

    #region ICopyable<Entity> Members
    /// <summary>
    /// Creates a deep-copy of the <see cref="Player"/>.
    /// </summary>
    /// <returns>A deep-copy of the <see cref="Player"/>.</returns>
    public virtual new Player Copy() {
        return new Player(this);
    }
    #endregion

    #region IItemContainer Members
    /// <summary>
    /// Gets access to an <see cref="ItemCollection"/> representing the player's equipment and inventory.
    /// </summary>
    public ItemCollection Items {
        get { return this.inventory; }
    }
    #endregion

    public ItemCollection EnderItems {
        get { return this.enderItems; }
    }
}
