namespace Substrate;

using Substrate.Core;
using Substrate.Nbt;

/// <summary>
/// Represents an item (or item stack) within an item slot.
/// </summary>
public class Item : INbtObject<Item>, ICopyable<Item> {
    private static readonly SchemaNodeCompound schema = new SchemaNodeCompound("") {
        new SchemaNodeScaler("id", TagType.TAG_SHORT),
        new SchemaNodeScaler("Damage", TagType.TAG_SHORT),
        new SchemaNodeScaler("Count", TagType.TAG_BYTE),
        new SchemaNodeCompound("tag", new SchemaNodeCompound("") {
            new SchemaNodeList("ench", TagType.TAG_COMPOUND, Enchantment.Schema, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("title", TagType.TAG_STRING, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("author", TagType.TAG_STRING, SchemaOptions.OPTIONAL),
            new SchemaNodeList("pages", TagType.TAG_STRING, SchemaOptions.OPTIONAL),
        }, SchemaOptions.OPTIONAL),
    };

    private TagNodeCompound source;

    private short id;
    private byte count;
    private short damage;

    private List<Enchantment> enchantments;

    /// <summary>
    /// Constructs an empty <see cref="Item"/> instance.
    /// </summary>
    public Item() {
        this.enchantments = new List<Enchantment>();
        this.source = new TagNodeCompound();
    }

    /// <summary>
    /// Constructs an <see cref="Item"/> instance representing the given item id.
    /// </summary>
    /// <param name="id">An item id.</param>
    public Item(int id) : this() {
        this.id = (short) id;
    }

    #region Properties
    /// <summary>
    /// Gets an <see cref="ItemInfo"/> entry for this item's type.
    /// </summary>
    public ItemInfo Info {
        get { return ItemInfo.ItemTable[this.id]; }
    }

    /// <summary>
    /// Gets or sets the current type (id) of the item.
    /// </summary>
    public int ID {
        get { return this.id; }
        set { this.id = (short) value; }
    }

    /// <summary>
    /// Gets or sets the damage value of the item.
    /// </summary>
    /// <remarks>The damage value may represent a generic data value for some items.</remarks>
    public int Damage {
        get { return this.damage; }
        set { this.damage = (short) value; }
    }

    /// <summary>
    /// Gets or sets the number of this item stacked together in an item slot.
    /// </summary>
    public int Count {
        get { return this.count; }
        set { this.count = (byte) value; }
    }

    /// <summary>
    /// Gets the list of <see cref="Enchantment"/>s applied to this item.
    /// </summary>
    public IList<Enchantment> Enchantments {
        get { return this.enchantments; }
    }

    /// <summary>
    /// Gets the source <see cref="TagNodeCompound"/> used to create this <see cref="Item"/> if it exists.
    /// </summary>
    public TagNodeCompound Source {
        get { return this.source; }
    }

    /// <summary>
    /// Gets a <see cref="SchemaNode"/> representing the schema of an item.
    /// </summary>
    public static SchemaNodeCompound Schema {
        get { return schema; }
    }
    #endregion

    #region ICopyable<Item> Members
    /// <inheritdoc/>
    public Item Copy() {
        Item item = new Item();
        item.id = this.id;
        item.count = this.count;
        item.damage = this.damage;

        foreach(Enchantment e in this.enchantments) {
            item.enchantments.Add(e.Copy());
        }

        if(this.source != null) {
            item.source = this.source.Copy() as TagNodeCompound;
        }

        return item;
    }
    #endregion

    #region INBTObject<Item> Members
    /// <inheritdoc/>
    public Item LoadTree(TagNode tree) {
        TagNodeCompound ctree = tree as TagNodeCompound;
        if(ctree == null) {
            return null;
        }

        this.enchantments.Clear();

        this.id = ctree["id"].ToTagShort();
        this.count = ctree["Count"].ToTagByte();
        this.damage = ctree["Damage"].ToTagShort();

        if(ctree.ContainsKey("tag")) {
            TagNodeCompound tagtree = ctree["tag"].ToTagCompound();
            if(tagtree.ContainsKey("ench")) {
                TagNodeList enchList = tagtree["ench"].ToTagList();

                foreach(TagNode tag in enchList) {
                    this.enchantments.Add(new Enchantment().LoadTree(tag));
                }
            }
        }
        this.source = ctree.Copy() as TagNodeCompound;

        return this;
    }

    /// <inheritdoc/>
    public Item LoadTreeSafe(TagNode tree) {
        if(!ValidateTree(tree)) {
            return null;
        }

        return LoadTree(tree);
    }

    /// <inheritdoc/>
    public TagNode BuildTree() {
        TagNodeCompound tree = new TagNodeCompound();
        tree["id"] = new TagNodeShort(this.id);
        tree["Count"] = new TagNodeByte(this.count);
        tree["Damage"] = new TagNodeShort(this.damage);

        if(this.enchantments.Count > 0) {
            TagNodeList enchList = new TagNodeList(TagType.TAG_COMPOUND);
            foreach(Enchantment e in this.enchantments) {
                enchList.Add(e.BuildTree());
            }

            TagNodeCompound tagtree = new TagNodeCompound();
            tagtree["ench"] = enchList;

            if(this.source != null && this.source.ContainsKey("tag")) {
                tagtree.MergeFrom(this.source["tag"].ToTagCompound());
            }

            tree["tag"] = tagtree;
        }

        if(this.source != null) {
            tree.MergeFrom(this.source);
        }
        return tree;
    }

    /// <inheritdoc/>
    public bool ValidateTree(TagNode tree) {
        return new NbtVerifier(tree, schema).Verify();
    }
    #endregion
}
