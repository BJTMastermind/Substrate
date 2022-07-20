namespace Substrate.Entities;

using Substrate.Nbt;

public class EntityArrow : EntityThrowable {
    public static readonly SchemaNodeCompound ArrowSchema = ThrowableSchema.MergeInto(new SchemaNodeCompound("") {
        new SchemaNodeString("id", TypeId),
        new SchemaNodeScaler("inData", TagType.TAG_BYTE, SchemaOptions.CREATE_ON_MISSING),
        new SchemaNodeScaler("player", TagType.TAG_BYTE, SchemaOptions.CREATE_ON_MISSING),
    });

    public static string TypeId {
        get { return "Arrow"; }
    }

    private byte inData;
    private byte player;

    public int InData {
        get { return this.inData; }
        set { this.inData = (byte) value; }
    }

    public bool IsPlayerArrow {
        get { return this.player != 0; }
        set { this.player = (byte) (value ? 1 : 0); }
    }

    protected EntityArrow(string id) : base(id) { }

    public EntityArrow() : this(TypeId) { }

    public EntityArrow(TypedEntity e) : base(e) {
        EntityArrow e2 = e as EntityArrow;
        if(e2 != null) {
            this.inData = e2.inData;
            this.player = e2.player;
        }
    }

    #region INBTObject<Entity> Members
    public override TypedEntity LoadTree(TagNode tree) {
        TagNodeCompound ctree = tree as TagNodeCompound;
        if(ctree == null || base.LoadTree(tree) == null) {
            return null;
        }

        this.inData = ctree["inData"].ToTagByte();
        this.player = ctree["player"].ToTagByte();

        return this;
    }

    public override TagNode BuildTree() {
        TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
        tree["inData"] = new TagNodeShort(this.inData);
        tree["player"] = new TagNodeShort(this.player);

        return tree;
    }

    public override bool ValidateTree(TagNode tree) {
        return new NbtVerifier(tree, ArrowSchema).Verify();
    }
    #endregion

    #region ICopyable<Entity> Members
    public override TypedEntity Copy() {
        return new EntityArrow(this);
    }
    #endregion
}
