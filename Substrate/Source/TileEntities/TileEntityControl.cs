﻿namespace Substrate.TileEntities;

using Substrate.Nbt;

public class TileEntityControl : TileEntity {
    public static readonly SchemaNodeCompound ControlSchema = TileEntity.Schema.MergeInto(new SchemaNodeCompound("") {
        new SchemaNodeString("id", TypeId),
        new SchemaNodeScaler("Command", TagType.TAG_STRING),
    });

    public static string TypeId {
        get { return "Control"; }
    }

    private string command;

    public string Command {
        get { return this.command; }
        set { this.command = value; }
    }

    protected TileEntityControl(string id) : base(id) { }

    public TileEntityControl() : this(TypeId) { }

    public TileEntityControl(TileEntity te) : base(te) {
        TileEntityControl tes = te as TileEntityControl;
        if(tes != null) {
            this.command = tes.command;
        }
    }

    #region ICopyable<TileEntity> Members
    public override TileEntity Copy() {
        return new TileEntityControl(this);
    }
    #endregion

    #region INBTObject<TileEntity> Members
    public override TileEntity LoadTree(TagNode tree) {
        TagNodeCompound ctree = tree as TagNodeCompound;
        if(ctree == null || base.LoadTree(tree) == null) {
            return null;
        }
        this.command = ctree["Command"].ToTagString();

        return this;
    }

    public override TagNode BuildTree() {
        TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
        tree["Command"] = new TagNodeString(this.command);

        return tree;
    }

    public override bool ValidateTree(TagNode tree) {
        return new NbtVerifier(tree, ControlSchema).Verify();
    }
    #endregion
}
