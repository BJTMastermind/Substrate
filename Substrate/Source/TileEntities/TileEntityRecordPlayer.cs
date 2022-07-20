namespace Substrate.TileEntities;

using Substrate.Nbt;

public class TileEntityRecordPlayer : TileEntity {
    public static readonly SchemaNodeCompound RecordPlayerSchema = TileEntity.Schema.MergeInto(new SchemaNodeCompound("") {
        new SchemaNodeString("id", TypeId),
        new SchemaNodeScaler("Record", TagType.TAG_INT, SchemaOptions.OPTIONAL),
    });

    public static string TypeId {
        get { return "RecordPlayer"; }
    }

    private int? record = null;

    public int? Record {
        get { return this.record; }
        set { this.record = value; }
    }

    protected TileEntityRecordPlayer(string id) : base(id) { }

    public TileEntityRecordPlayer() : this(TypeId) { }

    public TileEntityRecordPlayer(TileEntity te) : base(te) {
        TileEntityRecordPlayer tes = te as TileEntityRecordPlayer;
        if(tes != null) {
            this.record = tes.record;
        }
    }

    #region ICopyable<TileEntity> Members
    public override TileEntity Copy() {
        return new TileEntityRecordPlayer(this);
    }
    #endregion

    #region INBTObject<TileEntity> Members
    public override TileEntity LoadTree(TagNode tree) {
        TagNodeCompound ctree = tree as TagNodeCompound;
        if(ctree == null || base.LoadTree(tree) == null) {
            return null;
        }

        if(ctree.ContainsKey("Record")) {
            this.record = ctree["Record"].ToTagInt();
        }
        return this;
    }

    public override TagNode BuildTree() {
        TagNodeCompound tree = base.BuildTree() as TagNodeCompound;

        if(this.record != null) {
            tree["Record"] = new TagNodeInt((int) this.record);
        }
        return tree;
    }

    public override bool ValidateTree(TagNode tree) {
        return new NbtVerifier(tree, RecordPlayerSchema).Verify();
    }
    #endregion
}
