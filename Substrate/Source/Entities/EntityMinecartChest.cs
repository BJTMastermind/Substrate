﻿namespace Substrate.Entities;

using Substrate.Core;
using Substrate.Nbt;

public class EntityMinecartChest : EntityMinecart, IItemContainer {
    public static readonly SchemaNodeCompound MinecartChestSchema = MinecartSchema.MergeInto(new SchemaNodeCompound("") {
        new SchemaNodeList("Items", TagType.TAG_COMPOUND, ItemCollection.Schema),
    });

    public static new string TypeId {
        get { return EntityMinecart.TypeId; }
    }

    private static int _CAPACITY = 27;

    private ItemCollection _items;

    protected EntityMinecartChest(string id) : base(id) {
        this._items = new ItemCollection(_CAPACITY);
    }

    public EntityMinecartChest() : base() {
        this._items = new ItemCollection(_CAPACITY);
    }

    public EntityMinecartChest(TypedEntity e) : base(e) {
        EntityMinecartChest e2 = e as EntityMinecartChest;
        if(e2 != null) {
            this._items = e2._items.Copy();
        }
    }

    #region IItemContainer Members
    public ItemCollection Items {
        get { return this._items; }
    }
    #endregion

    #region INBTObject<Entity> Members
    public override TypedEntity LoadTree(TagNode tree) {
        TagNodeCompound ctree = tree as TagNodeCompound;
        if(ctree == null || base.LoadTree(tree) == null) {
            return null;
        }

        TagNodeList items = ctree["Items"].ToTagList();
        this._items = this._items.LoadTree(items);

        return this;
    }

    public override TagNode BuildTree() {
        TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
        tree["Items"] = this._items.BuildTree();

        return tree;
    }

    public override bool ValidateTree(TagNode tree) {
        return new NbtVerifier(tree, MinecartChestSchema).Verify();
    }
    #endregion

    #region ICopyable<Entity> Members
    public override TypedEntity Copy() {
        return new EntityMinecartChest(this);
    }
    #endregion
}
