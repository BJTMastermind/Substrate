﻿namespace Substrate;

using Substrate.Nbt;
using Substrate.TileEntities;

/// <summary>
/// Creates new instances of concrete <see cref="TileEntity"/> types from a dynamic registry.
/// </summary>
/// <remarks>This factory allows specific <see cref="TileEntity"/> objects to be generated as an NBT tree is parsed.  New types can be
/// registered with the factory at any time, so that custom <see cref="TileEntity"/> types can be supported.  By default, the standard
/// Tile Entities of Minecraft are registered with the factory at startup and bound to their respective 'id' fields.</remarks>
public class TileEntityFactory {
    private static Dictionary<string, Type> registry = new Dictionary<string, Type>();

    /// <summary>
    /// Create a new instance of a concrete <see cref="TileEntity"/> type by name.
    /// </summary>
    /// <param name="type">The name that a concrete <see cref="TileEntity"/> type was registered with.</param>
    /// <returns>A new instance of a concrete <see cref="TileEntity"/> type, or null if no type was registered with the given name.</returns>
    public static TileEntity Create(string type) {
        Type t;
        if(!registry.TryGetValue(type, out t)) {
            return null;
        }
        return Activator.CreateInstance(t) as TileEntity;
    }

    /// <summary>
    /// Create a new instance of a concrete <see cref="TileEntity"/> type by NBT node.
    /// </summary>
    /// <param name="tree">A <see cref="TagNodeCompound"/> representing a single Tile Entity, containing an 'id' field of the Tile Entity's registered name.</param>
    /// <returns>A new instance of a concrete <see cref="TileEntity"/> type, or null if no type was registered with the given name.</returns>
    public static TileEntity Create(TagNodeCompound tree) {
        string type = tree["id"].ToTagString();

        Type t;
        if(!registry.TryGetValue(type, out t)) {
            return null;
        }

        TileEntity te = Activator.CreateInstance(t) as TileEntity;

        return te.LoadTreeSafe(tree);
    }

    /// <summary>
    /// Create a new instance of a concrete <see cref="TileEntity"/> type by NBT node.
    /// </summary>
    /// <param name="tree">A <see cref="TagNodeCompound"/> representing a single Tile Entity, containing an 'id' field of the Tile Entity's registered name.</param>
    /// <returns>A new instance of a concrete <see cref="TileEntity"/> type, or null if no type was registered with the given name.</returns>
    public static TileEntity CreateGeneric(TagNodeCompound tree) {
        string type = tree["id"].ToTagString();
        Type t;

        if(!registry.TryGetValue(type, out t)) {
            t = typeof(TileEntity);
        }

        TileEntity te = Activator.CreateInstance(t, true) as TileEntity;

        return te.LoadTreeSafe(tree);
    }

    /// <summary>
    /// Lookup a concrete <see cref="TileEntity"/> type by name.
    /// </summary>
    /// <param name="type">The name that a concrete <see cref="TileEntity"/> type was registered with.</param>
    /// <returns>The <see cref="Type"/> of a concrete <see cref="TileEntity"/> type, or null if no type was registered with the given name.</returns>
    public static Type Lookup(string type) {
        Type t;
        if(!registry.TryGetValue(type, out t)) {
            return null;
        }
        return t;
    }

    /// <summary>
    /// Registers a new concrete <see cref="TileEntity"/> type with the <see cref="TileEntityFactory"/>, binding it to a given name.
    /// </summary>
    /// <param name="id">The name to bind to a concrete <see cref="TileEntity"/> type.</param>
    /// <param name="subtype">The <see cref="Type"/> of a concrete <see cref="TileEntity"/> type.</param>
    public static void Register(string id, Type subtype) {
        registry[id] = subtype;
    }

    /// <summary>
    /// Gets an enumerator over all registered TileEntities.
    /// </summary>
    public static IEnumerable<KeyValuePair<string, Type>> RegisteredTileEntities {
        get {
            foreach(KeyValuePair<string, Type> kvp in registry) {
                yield return kvp;
            }
        }
    }

    static TileEntityFactory() {
        registry[TileEntityEndPortal.TypeId] = typeof(TileEntityEndPortal);
        registry[TileEntityBeacon.TypeId] = typeof(TileEntityBeacon);
        registry[TileEntityBrewingStand.TypeId] = typeof(TileEntityBrewingStand);
        registry[TileEntityChest.TypeId] = typeof(TileEntityChest);
        registry[TileEntityControl.TypeId] = typeof(TileEntityControl);
        registry[TileEntityEnchantmentTable.TypeId] = typeof(TileEntityEnchantmentTable);
        registry[TileEntityFurnace.TypeId] = typeof(TileEntityFurnace);
        registry[TileEntityMobSpawner.TypeId] = typeof(TileEntityMobSpawner);
        registry[TileEntityMusic.TypeId] = typeof(TileEntityMusic);
        registry[TileEntityPiston.TypeId] = typeof(TileEntityPiston);
        registry[TileEntityRecordPlayer.TypeId] = typeof(TileEntityRecordPlayer);
        registry[TileEntitySign.TypeId] = typeof(TileEntitySign);
        registry[TileEntityTrap.TypeId] = typeof(TileEntityTrap);
    }
}

/// <summary>
/// An exception that is thrown when unknown TileEntity types are queried.
/// </summary>
public class UnknownTileEntityException : Exception {
    public UnknownTileEntityException(string message) : base(message) { }
}
