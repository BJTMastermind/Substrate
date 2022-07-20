namespace Substrate.Entities;

using Substrate.Nbt;

public class EntityItem : TypedEntity {
    public static readonly SchemaNodeCompound ItemSchema = TypedEntity.Schema.MergeInto(new SchemaNodeCompound("") {
        new SchemaNodeString("id", TypeId),
        new SchemaNodeScaler("Health", TagType.TAG_SHORT),
        new SchemaNodeScaler("Age", TagType.TAG_SHORT),
        new SchemaNodeCompound("Item", Item.Schema),
    });

    public static string TypeId {
        get { return "Item"; }
    }

    private short _health;
    private short _age;

    private Item _item;

    public int Health {
        get { return this._health; }
        set { this._health = (short) value; }
    }

    public int Age {
        get { return this._age; }
        set { this._age = (short) value; }
    }

    public Item Item {
        get { return this._item; }
        set { this._item = value; }
    }

    protected EntityItem(string id) : base(id) { }

    public EntityItem() : this(TypeId) { }

    public EntityItem(TypedEntity e) : base(e) {
        EntityItem e2 = e as EntityItem;
        if(e2 != null) {
            this._health = e2._health;
            this._age = e2._age;
            this._item = e2._item.Copy();
        }
    }

    #region INBTObject<Entity> Members
    public override TypedEntity LoadTree(TagNode tree) {
        TagNodeCompound ctree = tree as TagNodeCompound;
        if(ctree == null || base.LoadTree(tree) == null) {
            return null;
        }

        this._health = ctree["Health"].ToTagShort();
        this._age = ctree["Age"].ToTagShort();

        this._item = new Item().LoadTree(ctree["Item"]);

        return this;
    }

    public override TagNode BuildTree() {
        TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
        tree["Health"] = new TagNodeShort(this._health);
        tree["Age"] = new TagNodeShort(this._age);
        tree["Item"] = this._item.BuildTree();

        return tree;
    }

    public override bool ValidateTree(TagNode tree) {
        return new NbtVerifier(tree, ItemSchema).Verify();
    }
    #endregion

    #region ICopyable<Entity> Members
    public override TypedEntity Copy() {
        return new EntityItem(this);
    }
    #endregion
}
