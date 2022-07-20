namespace Substrate;

using Substrate.Core;
using Substrate.Nbt;

/// <summary>
/// The base Entity type for Minecraft Entities, providing access to data common to all Minecraft Entities.
/// </summary>
public class Entity : INbtObject<Entity>, ICopyable<Entity> {
    private static readonly SchemaNodeCompound schema = new SchemaNodeCompound("") {
        new SchemaNodeList("Pos", TagType.TAG_DOUBLE, 3),
        new SchemaNodeList("Motion", TagType.TAG_DOUBLE, 3),
        new SchemaNodeList("Rotation", TagType.TAG_FLOAT, 2),
        new SchemaNodeScaler("FallDistance", TagType.TAG_FLOAT),
        new SchemaNodeScaler("Fire", TagType.TAG_SHORT),
        new SchemaNodeScaler("Air", TagType.TAG_SHORT),
        new SchemaNodeScaler("OnGround", TagType.TAG_BYTE),
    };

    private TagNodeCompound source;

    private Vector3 pos;
    private Vector3 motion;
    private Orientation rotation;

    private float fallDistance;
    private short fire;
    private short air;
    private byte onGround;

    /// <summary>
    /// Gets or sets the global position of the entity in fractional block coordinates.
    /// </summary>
    public Vector3 Position {
        get { return this.pos; }
        set { this.pos = value; }
    }

    /// <summary>
    /// Gets or sets the velocity of the entity.
    /// </summary>
    public Vector3 Motion {
        get { return this.motion; }
        set { this.motion = value; }
    }

    /// <summary>
    /// Gets or sets the orientation of the entity.
    /// </summary>
    public Orientation Rotation {
        get { return this.rotation; }
        set { this.rotation = value; }
    }

    /// <summary>
    /// Gets or sets the distance that the entity has fallen, if it is falling.
    /// </summary>
    public double FallDistance {
        get { return this.fallDistance; }
        set { this.fallDistance = (float) value; }
    }

    /// <summary>
    /// Gets or sets the fire counter of the entity.
    /// </summary>
    public int Fire {
        get { return this.fire; }
        set { this.fire = (short) value; }
    }

    /// <summary>
    /// Gets or sets the remaining air availale to the entity.
    /// </summary>
    public int Air {
        get { return this.air; }
        set { this.air = (short) value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is currently touch the ground.
    /// </summary>
    public bool IsOnGround {
        get { return this.onGround == 1; }
        set { this.onGround = (byte) (value ? 1 : 0); }
    }

    /// <summary>
    /// Gets the source <see cref="TagNodeCompound"/> used to create this <see cref="Entity"/> if it exists.
    /// </summary>
    public TagNodeCompound Source {
        get { return this.source; }
    }

    /// <summary>
    /// Constructs a new generic <see cref="Entity"/> with default values.
    /// </summary>
    public Entity() {
        this.pos = new Vector3();
        this.motion = new Vector3();
        this.rotation = new Orientation();

        this.source = new TagNodeCompound();
    }

    /// <summary>
    /// Constructs a new generic <see cref="Entity"/> by copying fields from another <see cref="Entity"/> object.
    /// </summary>
    /// <param name="e">An <see cref="Entity"/> to copy fields from.</param>
    protected Entity(Entity e) {
        this.pos = new Vector3();
        this.pos.X = e.pos.X;
        this.pos.Y = e.pos.Y;
        this.pos.Z = e.pos.Z;

        this.motion = new Vector3();
        this.motion.X = e.motion.X;
        this.motion.Y = e.motion.Y;
        this.motion.Z = e.motion.Z;

        this.rotation = new Orientation();
        this.rotation.Pitch = e.rotation.Pitch;
        this.rotation.Yaw = e.rotation.Yaw;

        this.fallDistance = e.fallDistance;
        this.fire = e.fire;
        this.air = e.air;
        this.onGround = e.onGround;

        if(e.source != null) {
            this.source = e.source.Copy() as TagNodeCompound;
        }
    }

    /// <summary>
    /// Moves the <see cref="Entity"/> by given block offsets.
    /// </summary>
    /// <param name="diffX">The X-offset to move by, in blocks.</param>
    /// <param name="diffY">The Y-offset to move by, in blocks.</param>
    /// <param name="diffZ">The Z-offset to move by, in blocks.</param>
    public virtual void MoveBy(int diffX, int diffY, int diffZ) {
        this.pos.X += diffX;
        this.pos.Y += diffY;
        this.pos.Z += diffZ;
    }

    #region INBTObject<Entity> Members
    /// <summary>
    /// Gets a <see cref="SchemaNode"/> representing the basic schema of an Entity.
    /// </summary>
    public static SchemaNodeCompound Schema {
        get { return schema; }
    }

    /// <summary>
    /// Attempt to load an Entity subtree into the <see cref="Entity"/> without validation.
    /// </summary>
    /// <param name="tree">The root node of an Entity subtree.</param>
    /// <returns>The <see cref="Entity"/> returns itself on success, or null if the tree was unparsable.</returns>
    public Entity LoadTree(TagNode tree) {
        TagNodeCompound ctree = tree as TagNodeCompound;
        if(ctree == null) {
            return null;
        }

        TagNodeList pos = ctree["Pos"].ToTagList();
        this.pos = new Vector3();
        this.pos.X = pos[0].ToTagDouble();
        this.pos.Y = pos[1].ToTagDouble();
        this.pos.Z = pos[2].ToTagDouble();

        TagNodeList motion = ctree["Motion"].ToTagList();
        this.motion = new Vector3();
        this.motion.X = motion[0].ToTagDouble();
        this.motion.Y = motion[1].ToTagDouble();
        this.motion.Z = motion[2].ToTagDouble();

        TagNodeList rotation = ctree["Rotation"].ToTagList();
        this.rotation = new Orientation();
        this.rotation.Yaw = rotation[0].ToTagFloat();
        this.rotation.Pitch = rotation[1].ToTagFloat();

        this.fire = ctree["Fire"].ToTagShort();
        this.air = ctree["Air"].ToTagShort();
        this.onGround = ctree["OnGround"].ToTagByte();

        this.source = ctree.Copy() as TagNodeCompound;

        return this;
    }

    /// <summary>
    /// Attempt to load an Entity subtree into the <see cref="Entity"/> with validation.
    /// </summary>
    /// <param name="tree">The root node of an Entity subtree.</param>
    /// <returns>The <see cref="Entity"/> returns itself on success, or null if the tree failed validation.</returns>
    public Entity LoadTreeSafe(TagNode tree) {
        if(!ValidateTree(tree)) {
            return null;
        }

        return LoadTree(tree);
    }

    /// <summary>
    /// Builds an Entity subtree from the current data.
    /// </summary>
    /// <returns>The root node of an Entity subtree representing the current data.</returns>
    public TagNode BuildTree() {
        TagNodeCompound tree = new TagNodeCompound();

        TagNodeList pos = new TagNodeList(TagType.TAG_DOUBLE);
        pos.Add(new TagNodeDouble(this.pos.X));
        pos.Add(new TagNodeDouble(this.pos.Y));
        pos.Add(new TagNodeDouble(this.pos.Z));
        tree["Pos"] = pos;

        TagNodeList motion = new TagNodeList(TagType.TAG_DOUBLE);
        motion.Add(new TagNodeDouble(this.motion.X));
        motion.Add(new TagNodeDouble(this.motion.Y));
        motion.Add(new TagNodeDouble(this.motion.Z));
        tree["Motion"] = motion;

        TagNodeList rotation = new TagNodeList(TagType.TAG_FLOAT);
        rotation.Add(new TagNodeFloat((float) this.rotation.Yaw));
        rotation.Add(new TagNodeFloat((float) this.rotation.Pitch));
        tree["Rotation"] = rotation;

        tree["FallDistance"] = new TagNodeFloat(this.fallDistance);
        tree["Fire"] = new TagNodeShort(this.fire);
        tree["Air"] = new TagNodeShort(this.air);
        tree["OnGround"] = new TagNodeByte(this.onGround);

        if(this.source != null) {
            tree.MergeFrom(this.source);
        }

        return tree;
    }

    /// <summary>
    /// Validate an Entity subtree against a basic schema.
    /// </summary>
    /// <param name="tree">The root node of an Entity subtree.</param>
    /// <returns>Status indicating whether the tree was valid against the internal schema.</returns>
    public bool ValidateTree(TagNode tree) {
        return new NbtVerifier(tree, schema).Verify();
    }
    #endregion

    #region ICopyable<Entity> Members
    /// <summary>
    /// Creates a deep-copy of the <see cref="Entity"/>.
    /// </summary>
    /// <returns>A deep-copy of the <see cref="Entity"/>.</returns>
    public Entity Copy() {
        return new Entity(this);
    }
    #endregion
}

/// <summary>
/// A base entity type for all entities except <see cref="Player"/> entities.
/// </summary>
/// <remarks>Generally, this class should be subtyped into new concrete Entity types, as this generic type is unable to
/// capture any of the custom data fields.  It is however still possible to create instances of <see cref="Entity"/> objects,
/// which may allow for graceful handling of unknown Entity types.</remarks>
public class TypedEntity : Entity, INbtObject<TypedEntity>, ICopyable<TypedEntity> {
    private static readonly SchemaNodeCompound schema = Entity.Schema.MergeInto(new SchemaNodeCompound("") {
        new SchemaNodeScaler("id", TagType.TAG_STRING),
    });

    private string id;

    /// <summary>
    /// Gets the id (type) of the entity.
    /// </summary>
    public string ID {
        get { return this.id; }
    }

    /// <summary>
    /// Creates a new generic <see cref="TypedEntity"/> with the given id.
    /// </summary>
    /// <param name="id">The id (name) of the Entity.</param>
    public TypedEntity(string id) : base() {
        this.id = id;
    }

    /// <summary>
    /// Constructs a new <see cref="TypedEntity"/> by copying an existing one.
    /// </summary>
    /// <param name="e">The <see cref="TypedEntity"/> to copy.</param>
    protected TypedEntity(TypedEntity e) : base(e) {
        this.id = e.id;
    }

    #region INBTObject<Entity> Members
    /// <summary>
    /// Gets a <see cref="SchemaNode"/> representing the basic schema of an Entity.
    /// </summary>
    public static new SchemaNodeCompound Schema {
        get { return schema; }
    }

    /// <summary>
    /// Attempt to load an Entity subtree into the <see cref="TypedEntity"/> without validation.
    /// </summary>
    /// <param name="tree">The root node of an Entity subtree.</param>
    /// <returns>The <see cref="TypedEntity"/> returns itself on success, or null if the tree was unparsable.</returns>
    public virtual new TypedEntity LoadTree(TagNode tree) {
        TagNodeCompound ctree = tree as TagNodeCompound;
        if(ctree == null || base.LoadTree(tree) == null) {
            return null;
        }

        this.id = ctree["id"].ToTagString();

        return this;
    }

    /// <summary>
    /// Attempt to load an Entity subtree into the <see cref="TypedEntity"/> with validation.
    /// </summary>
    /// <param name="tree">The root node of an Entity subtree.</param>
    /// <returns>The <see cref="TypedEntity"/> returns itself on success, or null if the tree failed validation.</returns>
    public virtual new TypedEntity LoadTreeSafe(TagNode tree) {
        if(!ValidateTree(tree)) {
            return null;
        }

        return LoadTree(tree);
    }

    /// <summary>
    /// Builds an Entity subtree from the current data.
    /// </summary>
    /// <returns>The root node of an Entity subtree representing the current data.</returns>
    public virtual new TagNode BuildTree() {
        TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
        tree["id"] = new TagNodeString(this.id);

        return tree;
    }

    /// <summary>
    /// Validate an Entity subtree against a basic schema.
    /// </summary>
    /// <param name="tree">The root node of an Entity subtree.</param>
    /// <returns>Status indicating whether the tree was valid against the internal schema.</returns>
    public virtual new bool ValidateTree(TagNode tree) {
        return new NbtVerifier(tree, schema).Verify();
    }
    #endregion

    #region ICopyable<Entity> Members
    /// <summary>
    /// Creates a deep-copy of the <see cref="TypedEntity"/>.
    /// </summary>
    /// <returns>A deep-copy of the <see cref="TypedEntity"/>.</returns>
    public virtual new TypedEntity Copy() {
        return new TypedEntity(this);
    }
    #endregion
}
