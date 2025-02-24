﻿namespace Substrate;

using Substrate.Core;
using Substrate.Nbt;

/// <summary>
/// Represents a Tile Entity record, which provides additional data to a block.
/// </summary>
/// <remarks>Generally, this class should be subtyped into new concrete Tile Entity types, as this generic type is unable to
/// capture any of the custom data fields that make Tile Entities useful in the first place.  It is however still possible to
/// create instances of <see cref="TileEntity"/> objects, which may allow for graceful handling of unknown Tile Entities.</remarks>
public class TileEntity : INbtObject<TileEntity>, ICopyable<TileEntity> {
    private static readonly SchemaNodeCompound schema = new SchemaNodeCompound("") {
        new SchemaNodeScaler("id", TagType.TAG_STRING),
        new SchemaNodeScaler("x", TagType.TAG_INT),
        new SchemaNodeScaler("y", TagType.TAG_INT),
        new SchemaNodeScaler("z", TagType.TAG_INT),
    };

    private TagNodeCompound source;

    private string id;
    private int x;
    private int y;
    private int z;

    /// <summary>
    /// Gets the id (name) of the Tile Entity.
    /// </summary>
    public string ID {
        get { return this.id; }
    }

    /// <summary>
    /// Gets or sets the global X-coordinate of the block that this Tile Entity is associated with.
    /// </summary>
    public int X {
        get { return this.x; }
        set { this.x = value; }
    }

    /// <summary>
    /// Gets or sets the global Y-coordinate of the block that this Tile Entity is associated with.
    /// </summary>
    public int Y {
        get { return this.y; }
        set { this.y = value; }
    }

    /// <summary>
    /// Gets or sets the global Z-coordinate of the block that this Tile Entity is associated with.
    /// </summary>
    public int Z {
        get { return this.z; }
        set { this.z = value; }
    }

    /// <summary>
    /// Gets the source <see cref="TagNodeCompound"/> used to create this <see cref="TileEntity"/> if it exists.
    /// </summary>
    public TagNodeCompound Source {
        get { return this.source; }
    }

    /// <summary>
    /// Constructs a blank <see cref="TileEntity"/>.
    /// </summary>
    protected TileEntity() {
        this.source = new TagNodeCompound();
    }

    /// <summary>
    /// Constructs a nonspecific <see cref="TileEntity"/> with a given ID.
    /// </summary>
    /// <param name="id">The id (name) of the Tile Entity.</param>
    public TileEntity(string id) {
        this.id = id;
        this.source = new TagNodeCompound();
    }

    /// <summary>
    /// Constructs a <see cref="TileEntity"/> by copying an existing one.
    /// </summary>
    /// <param name="te">The <see cref="TileEntity"/> to copy.</param>
    public TileEntity(TileEntity te) {
        this.id = te.id;
        this.x = te.x;
        this.y = te.y;
        this.z = te.z;

        if(te.source != null) {
            this.source = te.source.Copy() as TagNodeCompound;
        }
    }

    /// <summary>
    /// Checks whether the Tile Entity is located(associated with a block) at the specific global coordinates.
    /// </summary>
    /// <param name="x">The global X-coordinate to test.</param>
    /// <param name="y">The global Y-coordinate to test.</param>
    /// <param name="z">The global Z-coordinate to test.</param>
    /// <returns>Status indicating whether the Tile Entity is located at the specified global coordinates.</returns>
    public bool LocatedAt(int x, int y, int z) {
        return this.x == x && this.y == y && this.z == z;
    }

    /// <summary>
    /// Moves the <see cref="TileEntity"/> by given block offsets.
    /// </summary>
    /// <param name="diffX">The X-offset to move by, in blocks.</param>
    /// <param name="diffY">The Y-offset to move by, in blocks.</param>
    /// <param name="diffZ">The Z-offset to move by, in blocks.</param>
    public virtual void MoveBy(int diffX, int diffY, int diffZ) {
        this.x += diffX;
        this.y += diffY;
        this.z += diffZ;
    }

    #region ICopyable<TileEntity> Members
    /// <summary>
    /// Creates a deep-copy of the <see cref="TileEntity"/> including any data defined in a subtype.
    /// </summary>
    /// <returns>A deep-copy of the <see cref="TileEntity"/>.</returns>
    public virtual TileEntity Copy() {
        return new TileEntity(this);
    }
    #endregion

    /// <summary>
    /// Attempt to construct a new <see cref="TileEntity"/> from a Tile Entity subtree without validation.
    /// </summary>
    /// <param name="tree">The root node of a Tile Entity subtree.</param>
    /// <returns>A new <see cref="TileEntity"/> on success, or null if the tree was unparsable.</returns>
    public static TileEntity FromTree(TagNode tree) {
        return new TileEntity().LoadTree(tree);
    }

    /// <summary>
    /// Attempt to construct a new <see cref="TileEntity"/> from a Tile Entity subtree with validation.
    /// </summary>
    /// <param name="tree">The root node of a Tile Entity subtree.</param>
    /// <returns>A new <see cref="TileEntity"/> on success, or null if the tree failed validation.</returns>
    public static TileEntity FromTreeSafe(TagNode tree) {
        return new TileEntity().LoadTreeSafe(tree);
    }

    #region INBTObject<TileEntity> Members
    /// <summary>
    /// Gets a <see cref="SchemaNode"/> representing the basic schema of a Tile Entity.
    /// </summary>
    public static SchemaNodeCompound Schema {
        get { return schema; }
    }

    /// <summary>
    /// Attempt to load a Tile Entity subtree into the <see cref="TileEntity"/> without validation.
    /// </summary>
    /// <param name="tree">The root node of a Tile Entity subtree.</param>
    /// <returns>The <see cref="TileEntity"/> returns itself on success, or null if the tree was unparsable.</returns>
    public virtual TileEntity LoadTree(TagNode tree) {
        TagNodeCompound ctree = tree as TagNodeCompound;
        if(ctree == null) {
            return null;
        }

        this.id = ctree["id"].ToTagString();
        this.x = ctree["x"].ToTagInt();
        this.y = ctree["y"].ToTagInt();
        this.z = ctree["z"].ToTagInt();

        this.source = ctree.Copy() as TagNodeCompound;

        return this;
    }

    /// <summary>
    /// Attempt to load a Tile Entity subtree into the <see cref="TileEntity"/> with validation.
    /// </summary>
    /// <param name="tree">The root node of a Tile Entity subtree.</param>
    /// <returns>The <see cref="TileEntity"/> returns itself on success, or null if the tree failed validation.</returns>
    public virtual TileEntity LoadTreeSafe(TagNode tree) {
        if(!ValidateTree(tree)) {
            return null;
        }

        return LoadTree(tree);
    }

    /// <summary>
    /// Builds a Tile Entity subtree from the current data.
    /// </summary>
    /// <returns>The root node of a Tile Entity subtree representing the current data.</returns>
    public virtual TagNode BuildTree() {
        TagNodeCompound tree = new TagNodeCompound();
        tree["id"] = new TagNodeString(this.id);
        tree["x"] = new TagNodeInt(this.x);
        tree["y"] = new TagNodeInt(this.y);
        tree["z"] = new TagNodeInt(this.z);

        if(this.source != null) {
            tree.MergeFrom(this.source);
        }

        return tree;
    }

    /// <summary>
    /// Validate a Tile Entity subtree against a basic schema.
    /// </summary>
    /// <param name="tree">The root node of a Tile Entity subtree.</param>
    /// <returns>Status indicating whether the tree was valid against the internal schema.</returns>
    public virtual bool ValidateTree(TagNode tree) {
        return new NbtVerifier(tree, schema).Verify();
    }
    #endregion
}
