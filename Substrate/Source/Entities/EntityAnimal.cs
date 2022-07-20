namespace Substrate.Entities;

using Substrate.Nbt;

public class EntityAnimal : EntityMob {
    public static readonly SchemaNodeCompound AnimalSchema = MobSchema.MergeInto(new SchemaNodeCompound("") {
        new SchemaNodeScaler("Age", TagType.TAG_INT, SchemaOptions.CREATE_ON_MISSING),
        new SchemaNodeScaler("InLove", TagType.TAG_INT, SchemaOptions.CREATE_ON_MISSING),
    });

    private int age;
    private int inLove;

    public int Age {
        get { return this.age; }
        set { this.age = value; }
    }

    public int InLove {
        get { return this.inLove; }
        set { this.inLove = value; }
    }

    protected EntityAnimal(string id) : base(id) {
    }

    public EntityAnimal() : this(TypeId) {
    }

    public EntityAnimal(TypedEntity e) : base(e) {
        EntityAnimal e2 = e as EntityAnimal;
        if(e2 != null) {
            this.age = e2.age;
            this.inLove = e2.inLove;
        }
    }

    #region INBTObject<Entity> Members
    public override TypedEntity LoadTree(TagNode tree) {
        TagNodeCompound ctree = tree as TagNodeCompound;
        if(ctree == null || base.LoadTree(tree) == null) {
            return null;
        }

        this.age = ctree["Age"].ToTagInt();
        this.inLove = ctree["InLove"].ToTagInt();

        return this;
    }

    public override TagNode BuildTree() {
        TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
        tree["Age"] = new TagNodeInt(this.age);
        tree["InLove"] = new TagNodeInt(this.inLove);

        return tree;
    }

    public override bool ValidateTree(TagNode tree) {
        return new NbtVerifier(tree, AnimalSchema).Verify();
    }
    #endregion

    #region ICopyable<Entity> Members
    public override TypedEntity Copy() {
        return new EntityAnimal(this);
    }
    #endregion
}
