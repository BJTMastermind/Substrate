namespace Substrate.Entities;

using Substrate.Core;
using Substrate.Nbt;

/// <summary>
/// Encompasses data in the "ActiveEffects" compound attribute of mob entity types, used to specify potion effects
/// </summary>
public class ActiveEffects : ICopyable<ActiveEffects> {
    private byte _id;
    private byte _amplifier;
    private int _duration;

    /// <summary>
    /// Gets or sets the ID of the potion effect type.
    /// </summary>
    public int Id {
        get { return this._id; }
        set { this._id = (byte) value; }
    }

    /// <summary>
    /// Gets or sets the amplification of the potion effect.
    /// </summary>
    public int Amplifier {
        get { return this._amplifier; }
        set { this._amplifier = (byte) value; }
    }

    /// <summary>
    /// Gets or sets the remaining duration of the potion effect.
    /// </summary>
    public int Duration {
        get { return this._duration; }
        set { this._duration = value; }
    }

    /// <summary>
    /// Determine if the combination of properties in this ActiveEffects is valid.
    /// </summary>
    public bool IsValid {
        get { return !(this._id == 0 || this._amplifier == 0 || this._duration == 0); }
    }

    #region ICopyable<ActiveEffects> Members
    public ActiveEffects Copy() {
        ActiveEffects ae = new ActiveEffects();
        ae._amplifier = this._amplifier;
        ae._duration = this._duration;
        ae._id = this._id;

        return ae;
    }
    #endregion
}

public class EntityMob : TypedEntity {
    public static readonly SchemaNodeCompound MobSchema = TypedEntity.Schema.MergeInto(new SchemaNodeCompound("") {
        new SchemaNodeString("id", TypeId),
        new SchemaNodeScaler("AttackTime", TagType.TAG_SHORT),
        new SchemaNodeScaler("DeathTime", TagType.TAG_SHORT),
        new SchemaNodeScaler("Health", TagType.TAG_SHORT),
        new SchemaNodeScaler("HurtTime", TagType.TAG_SHORT),
        new SchemaNodeCompound("ActiveEffects", SchemaOptions.OPTIONAL) {
            new SchemaNodeScaler("Id", TagType.TAG_BYTE),
            new SchemaNodeScaler("Amplifier", TagType.TAG_BYTE),
            new SchemaNodeScaler("Duration", TagType.TAG_INT),
        },
    });

    public static string TypeId {
        get { return "Mob"; }
    }

    private short _attackTime;
    private short _deathTime;
    private short _health;
    private short _hurtTime;

    private ActiveEffects _activeEffects;

    public int AttackTime {
        get { return this._attackTime; }
        set { this._attackTime = (short) value; }
    }

    public int DeathTime {
        get { return this._deathTime; }
        set { this._deathTime = (short) value; }
    }

    public int Health {
        get { return this._health; }
        set { this._health = (short) value; }
    }

    public int HurtTime {
        get { return this._hurtTime; }
        set { this._hurtTime = (short) value; }
    }

    public ActiveEffects ActiveEffects {
        get { return this._activeEffects; }
        set { this._activeEffects = value; }
    }

    protected EntityMob(string id) : base(id) {
        this._activeEffects = new ActiveEffects();
    }

    public EntityMob() : this(TypeId) { }

    public EntityMob(TypedEntity e) : base(e) {
        EntityMob e2 = e as EntityMob;
        if(e2 != null) {
            this._attackTime = e2._attackTime;
            this._deathTime = e2._deathTime;
            this._health = e2._health;
            this._hurtTime = e2._hurtTime;
            this._activeEffects = e2._activeEffects.Copy();
        } else {
            this._activeEffects = new ActiveEffects();
        }
    }

    #region INBTObject<Entity> Members
    public override TypedEntity LoadTree(TagNode tree) {
        TagNodeCompound ctree = tree as TagNodeCompound;
        if(ctree == null || base.LoadTree(tree) == null) {
            return null;
        }

        this._attackTime = ctree["AttackTime"].ToTagShort();
        this._deathTime = ctree["DeathTime"].ToTagShort();
        this._health = ctree["Health"].ToTagShort();
        this._hurtTime = ctree["HurtTime"].ToTagShort();

        if(ctree.ContainsKey("ActiveEffects")) {
            TagNodeCompound ae = ctree["ActiveEffects"].ToTagCompound();

            this._activeEffects = new ActiveEffects();
            this._activeEffects.Id = ae["Id"].ToTagByte();
            this._activeEffects.Amplifier = ae["Amplifier"].ToTagByte();
            this._activeEffects.Duration = ae["Duration"].ToTagInt();
        }
        return this;
    }

    public override TagNode BuildTree() {
        TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
        tree["AttackTime"] = new TagNodeShort(this._attackTime);
        tree["DeathTime"] = new TagNodeShort(this._deathTime);
        tree["Health"] = new TagNodeShort(this._health);
        tree["HurtTime"] = new TagNodeShort(this._hurtTime);

        if(this._activeEffects != null && this._activeEffects.IsValid) {
            TagNodeCompound ae = new TagNodeCompound();
            ae["Id"] = new TagNodeByte((byte) this._activeEffects.Id);
            ae["Amplifier"] = new TagNodeByte((byte) this._activeEffects.Amplifier);
            ae["Duration"] = new TagNodeInt(this._activeEffects.Duration);

            tree["ActiveEffects"] = ae;
        }
        return tree;
    }

    public override bool ValidateTree(TagNode tree) {
        return new NbtVerifier(tree, MobSchema).Verify();
    }
    #endregion

    #region ICopyable<Entity> Members
    public override TypedEntity Copy() {
        return new EntityMob(this);
    }
    #endregion
}
