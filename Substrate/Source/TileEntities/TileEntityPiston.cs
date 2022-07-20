namespace Substrate.TileEntities;

using Substrate.Nbt;

public class TileEntityPiston : TileEntity {
    public static readonly SchemaNodeCompound PistonSchema = TileEntity.Schema.MergeInto(new SchemaNodeCompound("") {
        new SchemaNodeString("id", TypeId),
        new SchemaNodeScaler("blockId", TagType.TAG_INT),
        new SchemaNodeScaler("blockData", TagType.TAG_INT),
        new SchemaNodeScaler("facing", TagType.TAG_INT),
        new SchemaNodeScaler("progress", TagType.TAG_FLOAT),
        new SchemaNodeScaler("extending", TagType.TAG_BYTE),
    });

    public static string TypeId {
        get { return "Piston"; }
    }

    private int? record = null;

    private byte extending;
    private int blockId;
    private int blockData;
    private int facing;
    private float progress;

    public bool Extending {
        get { return this.extending != 0; }
        set { this.extending = (byte) (value ? 1 : 0); }
    }

    public int BlockId {
        get { return this.blockId; }
        set { this.blockId = value; }
    }

    public int BlockData {
        get { return this.blockData; }
        set { this.blockData = value; }
    }

    public int Facing {
        get { return this.facing; }
        set { this.facing = value; }
    }

    public float Progress {
        get { return this.progress; }
        set { this.progress = value; }
    }

    protected TileEntityPiston(string id) : base(id) { }

    public TileEntityPiston() : this(TypeId) { }

    public TileEntityPiston(TileEntity te) : base(te) {
        TileEntityPiston tes = te as TileEntityPiston;
        if(tes != null) {
            this.blockId = tes.blockId;
            this.blockData = tes.blockData;
            this.facing = tes.facing;
            this.progress = tes.progress;
            this.extending = tes.extending;
        }
    }

    #region ICopyable<TileEntity> Members
    public override TileEntity Copy() {
        return new TileEntityPiston(this);
    }
    #endregion

    #region INBTObject<TileEntity> Members
    public override TileEntity LoadTree(TagNode tree) {
        TagNodeCompound ctree = tree as TagNodeCompound;
        if(ctree == null || base.LoadTree(tree) == null) {
            return null;
        }

        this.blockId = ctree["blockId"].ToTagInt();
        this.blockData = ctree["blockData"].ToTagInt();
        this.facing = ctree["facing"].ToTagInt();
        this.progress = ctree["progress"].ToTagFloat();
        this.extending = ctree["extending"].ToTagByte();

        return this;
    }

    public override TagNode BuildTree() {
        TagNodeCompound tree = base.BuildTree() as TagNodeCompound;

        if(this.record != null) {
            tree["blockId"] = new TagNodeInt(this.blockId);
            tree["blockData"] = new TagNodeInt(this.blockData);
            tree["facing"] = new TagNodeInt(this.facing);
            tree["progress"] = new TagNodeFloat(this.progress);
            tree["extending"] = new TagNodeByte(this.extending);
        }
        return tree;
    }

    public override bool ValidateTree(TagNode tree) {
        return new NbtVerifier(tree, PistonSchema).Verify();
    }
    #endregion
}
