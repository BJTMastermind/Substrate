namespace Substrate;

using Substrate.Core;
using Substrate.Nbt;

/// <summary>
/// Represents an enchantment that can be applied to some <see cref="Item"/>s.
/// </summary>
public class Enchantment : INbtObject<Enchantment>, ICopyable<Enchantment> {
    private static readonly SchemaNodeCompound schema = new SchemaNodeCompound("") {
        new SchemaNodeScaler("id", TagType.TAG_SHORT),
        new SchemaNodeScaler("lvl", TagType.TAG_SHORT),
    };

    private TagNodeCompound source;

    private short id;
    private short level;

    /// <summary>
    /// Constructs a blank <see cref="Enchantment"/>.
    /// </summary>
    public Enchantment() { }

    /// <summary>
    /// Constructs an <see cref="Enchantment"/> from a given id and level.
    /// </summary>
    /// <param name="id">The id (type) of the enchantment.</param>
    /// <param name="level">The level of the enchantment.</param>
    public Enchantment(int id, int level) {
        this.id = (short) id;
        this.level = (short) level;
    }

    #region Properties
    /// <summary>
    /// Gets an <see cref="EnchantmentInfo"/> entry for this enchantment's type.
    /// </summary>
    public EnchantmentInfo Info {
        get { return EnchantmentInfo.EnchantmentTable[this.id]; }
    }

    /// <summary>
    /// Gets or sets the current type(id) of the enchantment.
    /// </summary>
    public int Id {
        get { return this.id; }
        set { this.id = (short) value; }
    }

    /// <summary>
    /// Gets or sets the level of the enchantment.
    /// </summary>
    public int Level {
        get { return this.level; }
        set { this.level = (short) value; }
    }

    /// <summary>
    /// Gets a <see cref="SchemaNode"/> representing the schema of an enchantment.
    /// </summary>
    public static SchemaNodeCompound Schema {
        get { return schema; }
    }
    #endregion

    #region INbtObject<Enchantment> Members
    /// <inheritdoc />
    public Enchantment LoadTree(TagNode tree) {
        TagNodeCompound ctree = tree as TagNodeCompound;
        if(ctree == null) {
            return null;
        }

        this.id = ctree["id"].ToTagShort();
        this.level = ctree["lvl"].ToTagShort();

        this.source = ctree.Copy() as TagNodeCompound;

        return this;
    }

    /// <inheritdoc />
    public Enchantment LoadTreeSafe(TagNode tree) {
        if(!ValidateTree(tree)) {
            return null;
        }
        return LoadTree(tree);
    }

    /// <inheritdoc />
    public TagNode BuildTree() {
        TagNodeCompound tree = new TagNodeCompound();
        tree["id"] = new TagNodeShort(this.id);
        tree["lvl"] = new TagNodeShort(this.level);

        if(this.source != null) {
            tree.MergeFrom(this.source);
        }
        return tree;
    }

    /// <inheritdoc />
    public bool ValidateTree(TagNode tree) {
        return new NbtVerifier(tree, schema).Verify();
    }
    #endregion

    #region ICopyable<Enchantment> Members
    /// <inheritdoc />
    public Enchantment Copy() {
        Enchantment ench = new Enchantment(this.id, this.level);

        if(this.source != null) {
            ench.source = this.source.Copy() as TagNodeCompound;
        }
        return ench;
    }
    #endregion
}
