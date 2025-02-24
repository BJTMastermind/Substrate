﻿namespace Substrate;

using System.Collections;

/// <summary>
/// Provides named id values for known enchantment types.
/// </summary>
/// <remarks>See <see cref="BlockType"/> for additional information.</remarks>
public class EnchantmentType {
    public const int PROTECTION = 0;
    public const int FIRE_PROTECTION = 1;
    public const int FEATHER_FALLING = 2;
    public const int BLAST_PROTECTION = 3;
    public const int PROJECTILE_PROTECTION = 4;
    public const int RESPIRATION = 5;
    public const int AQUA_AFFINITY = 6;
    public const int THORNS = 7;
    public const int DEPTH_STRIDER = 8;
    public const int FROST_WALKER = 9;
    public const int CURSE_OF_BINDING = 10;
    public const int SHARPNESS = 16;
    public const int SMITE = 17;
    public const int BANE_OF_ARTHROPODS = 18;
    public const int KNOCKBACK = 19;
    public const int FIRE_ASPECT = 20;
    public const int LOOTING = 21;
    public const int SWEEPING_EDGE = 22;
    public const int EFFICIENCY = 32;
    public const int SILK_TOUCH = 33;
    public const int UNBREAKING = 34;
    public const int FORTUNE = 35;
    public const int POWER = 48;
    public const int PUNCH = 49;
    public const int FLAME = 50;
    public const int INFINITY = 51;
    public const int LUCK_OF_THE_SEA = 61;
    public const int LURE = 62;
    public const int MENDING = 70;
    public const int CURSE_OF_VANISHING = 71;
}

/// <summary>
/// Provides information on a specific type of enchantment.
/// </summary>
/// <remarks>By default, all known MC enchantment types are already defined and registered, assuming Substrate
/// is up to date with the current MC version.
/// New enchantment types may be created and used at runtime, and will automatically populate various static lookup tables
/// in the <see cref="EnchantmentInfo"/> class.</remarks>
public class EnchantmentInfo {
    private static Random rand = new Random();

    private class CacheTableDict<T> : ICacheTable<T> {
        private Dictionary<int, T> cache;

        public T this[int index] {
            get {
                T val;
                if(this.cache.TryGetValue(index, out val)) {
                    return val;
                }
                return default(T);
            }
        }

        public CacheTableDict(Dictionary<int, T> cache) {
            this.cache = cache;
        }

        public IEnumerator<T> GetEnumerator() {
            foreach(T val in this.cache.Values) {
                yield return val;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }

    private static readonly Dictionary<int, EnchantmentInfo> enchTable;

    private int id = 0;
    private string name = "";
    private int maxLevel = 0;

    private static readonly CacheTableDict<EnchantmentInfo> enchTableCache;

    /// <summary>
    /// Gets the lookup table for id-to-info values.
    /// </summary>
    public static ICacheTable<EnchantmentInfo> EnchantmentTable {
        get { return enchTableCache; }
    }

    /// <summary>
    /// Gets the id of the enchantment type.
    /// </summary>
    public int ID {
        get { return this.id; }
    }

    /// <summary>
    /// Gets the name of the enchantment type.
    /// </summary>
    public string Name {
        get { return this.name; }
    }

    /// <summary>
    /// Gets the maximum level allowed for this enchantment type.
    /// </summary>
    public int MaxLevel {
        get { return this.maxLevel; }
    }

    /// <summary>
    /// Constructs a new <see cref="EnchantmentInfo"/> record for the given enchantment id.
    /// </summary>
    /// <param name="id">The id of an item type.</param>
    public EnchantmentInfo(int id) {
        this.id = id;
        enchTable[this.id] = this;
    }

    /// <summary>
    /// Constructs a new <see cref="EnchantmentInfo"/> record for the given enchantment id and name.
    /// </summary>
    /// <param name="id">The id of an item type.</param>
    /// <param name="name">The name of an item type.</param>
    public EnchantmentInfo(int id, string name) {
        this.id = id;
        this.name = name;
        enchTable[this.id] = this;
    }

    /// <summary>
    /// Sets the maximum level for this enchantment type.
    /// </summary>
    /// <param name="level">The maximum allowed level.</param>
    /// <returns>The object instance used to invoke this method.</returns>
    public EnchantmentInfo SetMaxLevel(int level) {
        this.maxLevel = level;
        return this;
    }

    /// <summary>
    /// Chooses a registered enchantment type at random and returns it.
    /// </summary>
    /// <returns></returns>
    public static EnchantmentInfo GetRandomEnchantment() {
        List<EnchantmentInfo> list = new List<EnchantmentInfo>(enchTable.Values);
        return list[rand.Next(list.Count)];
    }

    public static EnchantmentInfo Protection;
    public static EnchantmentInfo FireProtection;
    public static EnchantmentInfo FeatherFalling;
    public static EnchantmentInfo BlastProtection;
    public static EnchantmentInfo ProjectileProtection;
    public static EnchantmentInfo Respiration;
    public static EnchantmentInfo AquaAffinity;
    public static EnchantmentInfo Thorns;
    public static EnchantmentInfo DepthStrider;
    public static EnchantmentInfo FrostWalker;
    public static EnchantmentInfo CurseOfBinding;
    public static EnchantmentInfo Sharpness;
    public static EnchantmentInfo Smite;
    public static EnchantmentInfo BaneOfArthropods;
    public static EnchantmentInfo Knockback;
    public static EnchantmentInfo FireAspect;
    public static EnchantmentInfo Looting;
    public static EnchantmentInfo Sweeping;
    public static EnchantmentInfo Efficiency;
    public static EnchantmentInfo SilkTouch;
    public static EnchantmentInfo Unbreaking;
    public static EnchantmentInfo Fortune;
    public static EnchantmentInfo Power;
    public static EnchantmentInfo Punch;
    public static EnchantmentInfo Flame;
    public static EnchantmentInfo Infinity;
    public static EnchantmentInfo LuckOfTheSea;
    public static EnchantmentInfo Lure;
    public static EnchantmentInfo Mending;
    public static EnchantmentInfo CurseOfVanishing;

    static EnchantmentInfo() {
        enchTable = new Dictionary<int, EnchantmentInfo>();
        enchTableCache = new CacheTableDict<EnchantmentInfo>(enchTable);

        Protection = new EnchantmentInfo(EnchantmentType.PROTECTION, "Protection").SetMaxLevel(4);
        FireProtection = new EnchantmentInfo(EnchantmentType.FIRE_PROTECTION, "Fire Protection").SetMaxLevel(4);
        FeatherFalling = new EnchantmentInfo(EnchantmentType.FEATHER_FALLING, "Feather Falling").SetMaxLevel(4);
        BlastProtection = new EnchantmentInfo(EnchantmentType.BLAST_PROTECTION, "Blast Protection").SetMaxLevel(4);
        ProjectileProtection = new EnchantmentInfo(EnchantmentType.PROJECTILE_PROTECTION, "Projectile Protection").SetMaxLevel(4);
        Respiration = new EnchantmentInfo(EnchantmentType.RESPIRATION, "Respiration").SetMaxLevel(3);
        AquaAffinity = new EnchantmentInfo(EnchantmentType.AQUA_AFFINITY, "Aqua Affinity").SetMaxLevel(1);
        Thorns = new EnchantmentInfo(EnchantmentType.THORNS, "Thorns").SetMaxLevel(3);
        DepthStrider = new EnchantmentInfo(EnchantmentType.DEPTH_STRIDER, "Depth Strider").SetMaxLevel(3);
        FrostWalker = new EnchantmentInfo(EnchantmentType.FROST_WALKER, "Frost Walker").SetMaxLevel(2);
        CurseOfBinding = new EnchantmentInfo(EnchantmentType.CURSE_OF_BINDING, "Curse of Binding").SetMaxLevel(1);
        Sharpness = new EnchantmentInfo(EnchantmentType.SHARPNESS, "Sharpness").SetMaxLevel(5);
        Smite = new EnchantmentInfo(EnchantmentType.SMITE, "Smite").SetMaxLevel(5);
        BaneOfArthropods = new EnchantmentInfo(EnchantmentType.BANE_OF_ARTHROPODS, "Bane of Arthropods").SetMaxLevel(5);
        Knockback = new EnchantmentInfo(EnchantmentType.KNOCKBACK, "Knockback").SetMaxLevel(2);
        FireAspect = new EnchantmentInfo(EnchantmentType.FIRE_ASPECT, "Fire Aspect").SetMaxLevel(2);
        Looting = new EnchantmentInfo(EnchantmentType.LOOTING, "Looting").SetMaxLevel(3);
        Sweeping = new EnchantmentInfo(EnchantmentType.SWEEPING_EDGE, "Sweeping Edge").SetMaxLevel(3);
        Efficiency = new EnchantmentInfo(EnchantmentType.EFFICIENCY, "Efficiency").SetMaxLevel(5);
        SilkTouch = new EnchantmentInfo(EnchantmentType.SILK_TOUCH, "Silk Touch").SetMaxLevel(1);
        Unbreaking = new EnchantmentInfo(EnchantmentType.UNBREAKING, "Unbreaking").SetMaxLevel(3);
        Fortune = new EnchantmentInfo(EnchantmentType.FORTUNE, "Fortune").SetMaxLevel(3);
        Power = new EnchantmentInfo(EnchantmentType.POWER, "Power").SetMaxLevel(5);
        Punch = new EnchantmentInfo(EnchantmentType.PUNCH, "Punch").SetMaxLevel(2);
        Flame = new EnchantmentInfo(EnchantmentType.FLAME, "Flame").SetMaxLevel(1);
        Infinity = new EnchantmentInfo(EnchantmentType.INFINITY, "Infinity").SetMaxLevel(1);
        LuckOfTheSea = new EnchantmentInfo(EnchantmentType.LUCK_OF_THE_SEA, "Luck of the Sea").SetMaxLevel(3);
        Lure = new EnchantmentInfo(EnchantmentType.LURE, "Lure").SetMaxLevel(3);
        Mending = new EnchantmentInfo(EnchantmentType.MENDING, "Mending").SetMaxLevel(1);
        CurseOfVanishing = new EnchantmentInfo(EnchantmentType.CURSE_OF_VANISHING, "Curse of Vanishing").SetMaxLevel(1);
    }
}
