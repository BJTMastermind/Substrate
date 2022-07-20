namespace Substrate.Entities;

using Substrate.Nbt;

public class EntityThrowable : TypedEntity {
    public static readonly SchemaNodeCompound ThrowableSchema = TypedEntity.Schema.MergeInto(new SchemaNodeCompound("") {
        new SchemaNodeScaler("xTile", TagType.TAG_SHORT),
        new SchemaNodeScaler("yTile", TagType.TAG_SHORT),
        new SchemaNodeScaler("zTile", TagType.TAG_SHORT),
        new SchemaNodeScaler("inTile", TagType.TAG_BYTE),
        new SchemaNodeScaler("shake", TagType.TAG_BYTE),
        new SchemaNodeScaler("inGround", TagType.TAG_BYTE),
    });

    private short _xTile;
    private short _yTile;
    private short _zTile;
    private byte _inTile;
    private byte _shake;
    private byte _inGround;

    public int XTile {
        get { return this._xTile; }
        set { this._xTile = (short) value; }
    }

    public int YTile {
        get { return this._yTile; }
        set { this._yTile = (short) value; }
    }

    public int ZTile {
        get { return this._zTile; }
        set { this._zTile = (short) value; }
    }

    public int InTile {
        get { return this._inTile; }
        set { this._inTile = (byte) value; }
    }

    public int Shake {
        get { return this._shake; }
        set { this._shake = (byte) value; }
    }

    public bool IsInGround {
        get { return this._inGround == 1; }
        set { this._inGround = (byte) (value ? 1 : 0); }
    }

    protected EntityThrowable(string id) : base(id) { }

    public EntityThrowable(TypedEntity e) : base(e) {
        EntityThrowable e2 = e as EntityThrowable;
        if(e2 != null) {
            this._xTile = e2._xTile;
            this._yTile = e2._yTile;
            this._zTile = e2._zTile;
            this._inTile = e2._inTile;
            this._inGround = e2._inGround;
            this._shake = e2._shake;
        }
    }

    #region INBTObject<Entity> Members
    public override TypedEntity LoadTree(TagNode tree) {
        TagNodeCompound ctree = tree as TagNodeCompound;
        if(ctree == null || base.LoadTree(tree) == null) {
            return null;
        }

        this._xTile = ctree["xTile"].ToTagShort();
        this._yTile = ctree["yTile"].ToTagShort();
        this._zTile = ctree["zTile"].ToTagShort();
        this._inTile = ctree["inTile"].ToTagByte();
        this._shake = ctree["shake"].ToTagByte();
        this._inGround = ctree["inGround"].ToTagByte();

        return this;
    }

    public override TagNode BuildTree() {
        TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
        tree["xTile"] = new TagNodeShort(this._xTile);
        tree["yTile"] = new TagNodeShort(this._yTile);
        tree["zTile"] = new TagNodeShort(this._zTile);
        tree["inTile"] = new TagNodeByte(this._inTile);
        tree["shake"] = new TagNodeByte(this._shake);
        tree["inGround"] = new TagNodeByte(this._inGround);

        return tree;
    }

    public override bool ValidateTree(TagNode tree) {
        return new NbtVerifier(tree, ThrowableSchema).Verify();
    }
    #endregion

    #region ICopyable<Entity> Members
    public override TypedEntity Copy() {
        return new EntityThrowable(this);
    }
    #endregion
}
