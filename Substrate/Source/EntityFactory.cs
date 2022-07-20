namespace Substrate;

using Substrate.Entities;
using Substrate.Nbt;

/// <summary>
/// Creates new instances of concrete <see cref="TypedEntity"/> types from a dynamic registry.
/// </summary>
/// <remarks>This factory allows specific <see cref="TypedEntity"/> objects to be generated as an NBT tree is parsed.  New types can be
/// registered with the factory at any time, so that custom <see cref="TypedEntity"/> types can be supported.  By default, the standard
/// Entities of Minecraft are registered with the factory at startup and bound to their respective 'id' fields.</remarks>
public class EntityFactory {
    private static Dictionary<string, Type> registry = new Dictionary<string, Type>();

    /// <summary>
    /// Create a new instance of a concrete <see cref="TypedEntity"/> type by name.
    /// </summary>
    /// <param name="type">The name that a concrete <see cref="TypedEntity"/> type was registered with.</param>
    /// <returns>A new instance of a concrete <see cref="TypedEntity"/> type, or null if no type was registered with the given name.</returns>
    public static TypedEntity Create(string type) {
        Type t;
        if(!registry.TryGetValue(type, out t)) {
            return null;
        }

        return Activator.CreateInstance(t) as TypedEntity;
    }

    /// <summary>
    /// Create a new instance of a concrete <see cref="TypedEntity"/> type by NBT node.
    /// </summary>
    /// <param name="tree">A <see cref="TagNodeCompound"/> representing a single Entity, containing an 'id' field of the Entity's registered name.</param>
    /// <returns>A new instance of a concrete <see cref="TypedEntity"/> type, or null if no type was registered with the given name.</returns>
    public static TypedEntity Create(TagNodeCompound tree) {
        TagNode type;
        if(!tree.TryGetValue("id", out type)) {
            return null;
        }

        Type t;
        if(!registry.TryGetValue(type.ToTagString(), out t)) {
            return null;
        }

        TypedEntity te = Activator.CreateInstance(t) as TypedEntity;

        return te.LoadTreeSafe(tree);
    }

    /// <summary>
    /// Creates a new instance of a nonspecific <see cref="TypedEntity"/> object by NBT node.
    /// </summary>
    /// <param name="tree">A <see cref="TagNodeCompound"/> representing a single Entity, containing an 'id' field.</param>
    /// <returns>A new instance of a <see cref="TypedEntity"/> object, or null if the entity is not typed.</returns>
    public static TypedEntity CreateGeneric(TagNodeCompound tree) {
        TagNode type;
        if(!tree.TryGetValue("id", out type)) {
            return null;
        }

        TypedEntity te = new TypedEntity(type.ToTagString().Data);

        return te.LoadTreeSafe(tree);
    }

    /// <summary>
    /// Lookup a concrete <see cref="TypedEntity"/> type by name.
    /// </summary>
    /// <param name="type">The name that a concrete <see cref="TypedEntity"/> type was registered with.</param>
    /// <returns>The <see cref="Type"/> of a concrete <see cref="TypedEntity"/> type, or null if no type was registered with the given name.</returns>
    public static Type Lookup(string type) {
        Type t;
        if(!registry.TryGetValue(type, out t)) {
            return null;
        }

        return t;
    }

    /// <summary>
    /// Registers a new concrete <see cref="TypedEntity"/> type with the <see cref="EntityFactory"/>, binding it to a given name.
    /// </summary>
    /// <param name="id">The name to bind to a concrete <see cref="TypedEntity"/> type.</param>
    /// <param name="subtype">The <see cref="Type"/> of a concrete <see cref="TypedEntity"/> type.</param>
    public static void Register(string id, Type subtype) {
        registry[id] = subtype;
    }

    /// <summary>
    /// Gets an enumerator over all registered Entities.
    /// </summary>
    public static IEnumerable<KeyValuePair<string, Type>> RegisteredEntities {
        get {
            foreach(KeyValuePair<string, Type> kvp in registry) {
                yield return kvp;
            }
        }
    }

    static EntityFactory() {
        registry[EntityArrow.TypeId] = typeof(EntityArrow);
        registry[EntityBlaze.TypeId] = typeof(EntityBlaze);
        registry[EntityBoat.TypeId] = typeof(EntityBoat);
        registry[EntityCaveSpider.TypeId] = typeof(EntityCaveSpider);
        registry[EntityChicken.TypeId] = typeof(EntityChicken);
        registry[EntityCow.TypeId] = typeof(EntityCow);
        registry[EntityCreeper.TypeId] = typeof(EntityCreeper);
        registry[EntityEgg.TypeId] = typeof(EntityEgg);
        registry[EntityEnderDragon.TypeId] = typeof(EntityEnderDragon);
        registry[EntityEnderman.TypeId] = typeof(EntityEnderman);
        registry[EntityEnderEye.TypeId] = typeof(EntityEnderEye);
        registry[EntityFallingSand.TypeId] = typeof(EntityFallingSand);
        registry[EntityFireball.TypeId] = typeof(EntityFireball);
        registry[EntityGhast.TypeId] = typeof(EntityGhast);
        registry[EntityGiant.TypeId] = typeof(EntityGiant);
        registry[EntityItem.TypeId] = typeof(EntityItem);
        registry[EntityMagmaCube.TypeId] = typeof(EntityMagmaCube);
        registry[EntityMinecart.TypeId] = typeof(EntityMinecart);
        registry[EntityMob.TypeId] = typeof(EntityMob);
        registry[EntityMonster.TypeId] = typeof(EntityMonster);
        registry[EntityMooshroom.TypeId] = typeof(EntityMooshroom);
        registry[EntityPainting.TypeId] = typeof(EntityPainting);
        registry[EntityPig.TypeId] = typeof(EntityPig);
        registry[EntityPigZombie.TypeId] = typeof(EntityPigZombie);
        registry[EntityPrimedTnt.TypeId] = typeof(EntityPrimedTnt);
        registry[EntitySheep.TypeId] = typeof(EntitySheep);
        registry[EntitySilverfish.TypeId] = typeof(EntitySilverfish);
        registry[EntitySkeleton.TypeId] = typeof(EntitySkeleton);
        registry[EntitySlime.TypeId] = typeof(EntitySlime);
        registry[EntitySmallFireball.TypeId] = typeof(EntitySmallFireball);
        registry[EntitySnowball.TypeId] = typeof(EntitySnowball);
        registry[EntitySnowman.TypeId] = typeof(EntitySnowman);
        registry[EntitySpider.TypeId] = typeof(EntitySpider);
        registry[EntitySquid.TypeId] = typeof(EntitySquid);
        registry[EntityEnderPearl.TypeId] = typeof(EntityEnderPearl);
        registry[EntityVillager.TypeId] = typeof(EntityVillager);
        registry[EntityWolf.TypeId] = typeof(EntityWolf);
        registry[EntityXPOrb.TypeId] = typeof(EntityXPOrb);
        registry[EntityZombie.TypeId] = typeof(EntityZombie);
    }
}
