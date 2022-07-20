namespace Substrate.TileEntities;

using Substrate.Core;
using Substrate.Nbt;

public class TileEntityChest : TileEntity, IItemContainer {
    public static readonly SchemaNodeCompound ChestSchema = TileEntity.Schema.MergeInto(new SchemaNodeCompound("") {
        new SchemaNodeString("id", TypeId),
        new SchemaNodeList("Items", TagType.TAG_COMPOUND, ItemCollection.Schema),
    });

    public static string TypeId {
        get { return "Chest"; }
    }

    private const int CAPACITY = 27;

    private ItemCollection items;

    protected TileEntityChest(string id) : base(id) {
        this.items = new ItemCollection(CAPACITY);
    }

    public TileEntityChest() : this(TypeId) { }

    public TileEntityChest(TileEntity te) : base(te) {
        TileEntityChest tec = te as TileEntityChest;
        if(tec != null) {
            this.items = tec.items.Copy();
        } else {
            this.items = new ItemCollection(CAPACITY);
        }
    }

    #region ICopyable<TileEntity> Members
    public override TileEntity Copy() {
        return new TileEntityChest(this);
    }
    #endregion

    #region IItemContainer Members
    public ItemCollection Items {
        get { return this.items; }
    }
    #endregion

    #region INBTObject<TileEntity> Members
    public override TileEntity LoadTree(TagNode tree) {
        TagNodeCompound ctree = tree as TagNodeCompound;
        if(ctree == null || base.LoadTree(tree) == null) {
            return null;
        }

        TagNodeList items = ctree["Items"].ToTagList();
        this.items = new ItemCollection(CAPACITY).LoadTree(items);

        return this;
    }

    public override TagNode BuildTree() {
        TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
        tree["Items"] = this.items.BuildTree();

        return tree;
    }

    public override bool ValidateTree(TagNode tree) {
        return new NbtVerifier(tree, ChestSchema).Verify();
    }
    #endregion
}
