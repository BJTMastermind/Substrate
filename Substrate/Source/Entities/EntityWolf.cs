﻿namespace Substrate.Entities;

using Substrate.Nbt;

public class EntityWolf : EntityAnimal {
    public static readonly SchemaNodeCompound WolfSchema = AnimalSchema.MergeInto(new SchemaNodeCompound("") {
        new SchemaNodeString("id", TypeId),
        new SchemaNodeScaler("Owner", TagType.TAG_STRING),
        new SchemaNodeScaler("Sitting", TagType.TAG_BYTE),
        new SchemaNodeScaler("Angry", TagType.TAG_BYTE),
    });

    public static new string TypeId {
        get { return "Wolf"; }
    }

    private string _owner;
    private bool _sitting;
    private bool _angry;

    public string Owner {
        get { return this._owner; }
        set { this._owner = value; }
    }

    public bool IsSitting {
        get { return this._sitting; }
        set { this._sitting = value; }
    }

    public bool IsAngry {
        get { return this._angry; }
        set { this._angry = value; }
    }

    protected EntityWolf(string id) : base(id) { }

    public EntityWolf() : this(TypeId) { }

    public EntityWolf(TypedEntity e) : base(e) {
        EntityWolf e2 = e as EntityWolf;
        if(e2 != null) {
            this._owner = e2._owner;
            this._sitting = e2._sitting;
            this._angry = e2._angry;
        }
    }

    #region INBTObject<Entity> Members
    public override TypedEntity LoadTree(TagNode tree) {
        TagNodeCompound ctree = tree as TagNodeCompound;
        if(ctree == null || base.LoadTree(tree) == null) {
            return null;
        }

        this._owner = ctree["Owner"].ToTagString();
        this._sitting = ctree["Sitting"].ToTagByte() == 1;
        this._angry = ctree["Angry"].ToTagByte() == 1;

        return this;
    }

    public override TagNode BuildTree() {
        TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
        tree["Owner"] = new TagNodeString(this._owner);
        tree["Sitting"] = new TagNodeByte((byte) (this._sitting ? 1 : 0));
        tree["Angry"] = new TagNodeByte((byte) (this._angry ? 1 : 0));

        return tree;
    }

    public override bool ValidateTree(TagNode tree) {
        return new NbtVerifier(tree, WolfSchema).Verify();
    }
    #endregion

    #region ICopyable<Entity> Members
    public override TypedEntity Copy() {
        return new EntityWolf(this);
    }
    #endregion
}
