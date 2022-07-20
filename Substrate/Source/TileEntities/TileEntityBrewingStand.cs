namespace Substrate.TileEntities;

using Substrate.Core;
using Substrate.Nbt;

public class TileEntityBrewingStand : TileEntity, IItemContainer {
    public static readonly SchemaNodeCompound BrewingStandSchema = TileEntity.Schema.MergeInto(new SchemaNodeCompound("") {
        new SchemaNodeString("id", TypeId),
        new SchemaNodeList("Items", TagType.TAG_COMPOUND, ItemCollection.Schema),
        new SchemaNodeScaler("BrewTime", TagType.TAG_SHORT),
    });

    public static string TypeId {
        get { return "Cauldron"; }
    }

    private const int CAPACITY = 4;

    private ItemCollection items;
    private short brewTime;

    protected TileEntityBrewingStand(string id) : base(id) {
        this.items = new ItemCollection(CAPACITY);
    }

    public TileEntityBrewingStand() : this(TypeId) { }

    public TileEntityBrewingStand(TileEntity te) : base(te) {
        TileEntityBrewingStand tec = te as TileEntityBrewingStand;
        if(tec != null) {
            this.items = tec.items.Copy();
            this.brewTime = tec.brewTime;
        } else {
            this.items = new ItemCollection(CAPACITY);
        }
    }

    public int BrewTime {
        get { return this.brewTime; }
        set { this.brewTime = (short) value; }
    }

    #region ICopyable<TileEntity> Members
    public override TileEntity Copy() {
        return new TileEntityBrewingStand(this);
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

        this.brewTime = ctree["BrewTime"].ToTagShort();

        return this;
    }

    public override TagNode BuildTree() {
        TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
        tree["Items"] = this.items.BuildTree();
        tree["BrewTime"] = new TagNodeShort(this.brewTime);

        return tree;
    }

    public override bool ValidateTree(TagNode tree) {
        return new NbtVerifier(tree, BrewingStandSchema).Verify();
    }
    #endregion
}
