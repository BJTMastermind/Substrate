﻿namespace Substrate.TileEntities;

using Substrate.Core;
using Substrate.Nbt;

public class TileEntityFurnace : TileEntity, IItemContainer {
    public static readonly SchemaNodeCompound FurnaceSchema = TileEntity.Schema.MergeInto(new SchemaNodeCompound("") {
        new SchemaNodeString("id", TypeId),
        new SchemaNodeScaler("BurnTime", TagType.TAG_SHORT),
        new SchemaNodeScaler("CookTime", TagType.TAG_SHORT),
        new SchemaNodeList("Items", TagType.TAG_COMPOUND, ItemCollection.Schema),
    });

    public static string TypeId {
        get { return "Furnace"; }
    }

    private const int _CAPACITY = 3;

    private short burnTime;
    private short cookTime;

    private ItemCollection items;

    public int BurnTime {
        get { return this.burnTime; }
        set { this.burnTime = (short) value; }
    }

    public int CookTime {
        get { return this.cookTime; }
        set { this.cookTime = (short) value; }
    }

    protected TileEntityFurnace(string id) : base(id) {
        this.items = new ItemCollection(_CAPACITY);
    }

    public TileEntityFurnace() : this(TypeId) {
    }

    public TileEntityFurnace(TileEntity te) : base(te) {
        TileEntityFurnace tec = te as TileEntityFurnace;
        if(tec != null) {
            this.cookTime = tec.cookTime;
            this.burnTime = tec.burnTime;
            this.items = tec.items.Copy();
        } else {
            this.items = new ItemCollection(_CAPACITY);
        }
    }

    #region ICopyable<TileEntity> Members
    public override TileEntity Copy() {
        return new TileEntityFurnace(this);
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

        this.burnTime = ctree["BurnTime"].ToTagShort();
        this.cookTime = ctree["CookTime"].ToTagShort();

        TagNodeList items = ctree["Items"].ToTagList();
        this.items = new ItemCollection(_CAPACITY).LoadTree(items);

        return this;
    }

    public override TagNode BuildTree() {
        TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
        tree["BurnTime"] = new TagNodeShort(this.burnTime);
        tree["CookTime"] = new TagNodeShort(this.cookTime);
        tree["Items"] = this.items.BuildTree();

        return tree;
    }

    public override bool ValidateTree(TagNode tree) {
        return new NbtVerifier(tree, FurnaceSchema).Verify();
    }
    #endregion
}
