namespace Substrate.TileEntities;

using Substrate.Nbt;

public class TileEntitySign : TileEntity {
    public static readonly SchemaNodeCompound SignSchema = TileEntity.Schema.MergeInto(new SchemaNodeCompound("") {
        new SchemaNodeString("id", TypeId),
        new SchemaNodeScaler("Text1", TagType.TAG_STRING),
        new SchemaNodeScaler("Text2", TagType.TAG_STRING),
        new SchemaNodeScaler("Text3", TagType.TAG_STRING),
        new SchemaNodeScaler("Text4", TagType.TAG_STRING),
    });

    public static string TypeId {
        get { return "Sign"; }
    }

    private string text1 = "";
    private string text2 = "";
    private string text3 = "";
    private string text4 = "";

    public string Text1 {
        get { return this.text1; }
        set { this.text1 = value.Length > 14 ? value.Substring(0, 14) : value; }
    }

    public string Text2 {
        get { return this.text2; }
        set { this.text2 = value.Length > 14 ? value.Substring(0, 14) : value; }
    }

    public string Text3 {
        get { return this.text3; }
        set { this.text3 = value.Length > 14 ? value.Substring(0, 14) : value; }
    }

    public string Text4 {
        get { return this.text4; }
        set { this.text4 = value.Length > 14 ? value.Substring(0, 14) : value; }
    }

    protected TileEntitySign(string id) : base(id) { }

    public TileEntitySign() : this(TypeId) { }

    public TileEntitySign(TileEntity te) : base(te) {
        TileEntitySign tes = te as TileEntitySign;
        if(tes != null) {
            this.text1 = tes.text1;
            this.text2 = tes.text2;
            this.text3 = tes.text3;
            this.text4 = tes.text4;
        }
    }

    #region ICopyable<TileEntity> Members
    public override TileEntity Copy() {
        return new TileEntitySign(this);
    }
    #endregion

    #region INBTObject<TileEntity> Members
    public override TileEntity LoadTree(TagNode tree) {
        TagNodeCompound ctree = tree as TagNodeCompound;
        if(ctree == null || base.LoadTree(tree) == null) {
            return null;
        }

        this.text1 = ctree["Text1"].ToTagString();
        this.text2 = ctree["Text2"].ToTagString();
        this.text3 = ctree["Text3"].ToTagString();
        this.text4 = ctree["Text4"].ToTagString();

        return this;
    }

    public override TagNode BuildTree() {
        TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
        tree["Text1"] = new TagNodeString(this.text1);
        tree["Text2"] = new TagNodeString(this.text2);
        tree["Text3"] = new TagNodeString(this.text3);
        tree["Text4"] = new TagNodeString(this.text4);

        return tree;
    }

    public override bool ValidateTree(TagNode tree) {
        return new NbtVerifier(tree, SignSchema).Verify();
    }
    #endregion
}
