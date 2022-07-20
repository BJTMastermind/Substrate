namespace Substrate.TileEntities;

using Substrate.Nbt;

public class TileEntityBeacon : TileEntity {
    public static readonly SchemaNodeCompound BeaconSchema = TileEntity.Schema.MergeInto(new SchemaNodeCompound("") {
        new SchemaNodeString("id", TypeId),
        new SchemaNodeScaler("Levels", TagType.TAG_INT),
        new SchemaNodeScaler("Primary", TagType.TAG_INT),
        new SchemaNodeScaler("Secondary", TagType.TAG_INT),
    });

    public static string TypeId {
        get { return "Beacon"; }
    }

    private int levels;
    private int primary;
    private int secondary;

    public int Levels {
        get { return this.levels; }
        set { this.levels = value; }
    }

    public int Primary {
        get { return this.primary; }
        set { this.primary = value; }
    }

    public int Secondary {
        get { return this.secondary; }
        set { this.secondary = value; }
    }

    protected TileEntityBeacon(string id) : base(id) { }

    public TileEntityBeacon() : this(TypeId) { }

    public TileEntityBeacon(TileEntity te) : base(te) {
        TileEntityBeacon tes = te as TileEntityBeacon;
        if(tes != null) {
            this.levels = tes.levels;
            this.primary = tes.primary;
            this.secondary = tes.secondary;
        }
    }

    #region ICopyable<TileEntity> Members
    public override TileEntity Copy() {
        return new TileEntityBeacon(this);
    }
    #endregion

    #region INBTObject<TileEntity> Members
    public override TileEntity LoadTree(TagNode tree) {
        TagNodeCompound ctree = tree as TagNodeCompound;
        if(ctree == null || base.LoadTree(tree) == null) {
            return null;
        }

        this.levels = ctree["Levels"].ToTagInt();
        this.primary = ctree["Primary"].ToTagInt();
        this.secondary = ctree["Secondary"].ToTagInt();

        return this;
    }

    public override TagNode BuildTree() {
        TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
        tree["Levels"] = new TagNodeInt(this.levels);
        tree["Primary"] = new TagNodeInt(this.primary);
        tree["Secondary"] = new TagNodeInt(this.secondary);

        return tree;
    }

    public override bool ValidateTree(TagNode tree) {
        return new NbtVerifier(tree, BeaconSchema).Verify();
    }
    #endregion
}
