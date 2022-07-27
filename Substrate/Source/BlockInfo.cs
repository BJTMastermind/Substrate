namespace Substrate;

using System.Collections;

/// <summary>
/// Provides named id values for known block types.
/// </summary>
/// <remarks><para>The preferred method to lookup
/// Minecraft block IDs is to access the ID field of the corresponding static BlockInfo
/// object in the BlockInfo class.</para>
/// <para>The static BlockInfo objects can be re-bound to new BlockInfo objects, allowing
/// the named object to be bound to a new block ID.  This gives the developer more flexibility
/// in supporting nonstandard worlds, and the ability to future-proof their application against
/// changes to Block IDs, by implementing functionality to import block/ID mappings from an
/// external source and rebinding the objects in BlockInfo.</para></remarks>
public static class BlockType {
    public const int AIR = 0;
    public const int STONE = 1;
    public const int GRASS = 2;
    public const int DIRT = 3;
    public const int COBBLESTONE = 4;
    public const int WOOD_PLANK = 5;
    public const int SAPLING = 6;
    public const int BEDROCK = 7;
    public const int WATER = 8;
    public const int STATIONARY_WATER = 9;
    public const int LAVA = 10;
    public const int STATIONARY_LAVA = 11;
    public const int SAND = 12;
    public const int GRAVEL = 13;
    public const int GOLD_ORE = 14;
    public const int IRON_ORE = 15;
    public const int COAL_ORE = 16;
    public const int WOOD = 17;
    public const int LEAVES = 18;
    public const int SPONGE = 19;
    public const int GLASS = 20;
    public const int LAPIS_ORE = 21;
    public const int LAPIS_BLOCK = 22;
    public const int DISPENSER = 23;
    public const int SANDSTONE = 24;
    public const int NOTE_BLOCK = 25;
    public const int BED = 26;
    public const int POWERED_RAIL = 27;
    public const int DETECTOR_RAIL = 28;
    public const int STICKY_PISTON = 29;
    public const int COBWEB = 30;
    public const int TALL_GRASS = 31;
    public const int DEAD_SHRUB = 32;
    public const int PISTON = 33;
    public const int PISTON_HEAD = 34;
    public const int WOOL = 35;
    public const int PISTON_MOVING = 36;
    public const int YELLOW_FLOWER = 37;
    public const int RED_ROSE = 38;
    public const int BROWN_MUSHROOM = 39;
    public const int RED_MUSHROOM = 40;
    public const int GOLD_BLOCK = 41;
    public const int IRON_BLOCK = 42;
    public const int DOUBLE_STONE_SLAB = 43;
    public const int STONE_SLAB = 44;
    public const int BRICK_BLOCK = 45;
    public const int TNT = 46;
    public const int BOOKSHELF = 47;
    public const int MOSS_STONE = 48;
    public const int OBSIDIAN = 49;
    public const int TORCH = 50;
    public const int FIRE = 51;
    public const int MONSTER_SPAWNER = 52;
    public const int WOOD_STAIRS = 53;
    public const int CHEST = 54;
    public const int REDSTONE_WIRE = 55;
    public const int DIAMOND_ORE = 56;
    public const int DIAMOND_BLOCK = 57;
    public const int CRAFTING_TABLE = 58;
    public const int CROPS = 59;
    public const int FARMLAND = 60;
    public const int FURNACE = 61;
    public const int BURNING_FURNACE = 62;
    public const int SIGN_POST = 63;
    public const int WOOD_DOOR = 64;
    public const int LADDER = 65;
    public const int RAILS = 66;
    public const int COBBLESTONE_STAIRS = 67;
    public const int WALL_SIGN = 68;
    public const int LEVER = 69;
    public const int STONE_PLATE = 70;
    public const int IRON_DOOR = 71;
    public const int WOOD_PLATE = 72;
    public const int REDSTONE_ORE = 73;
    public const int GLOWING_REDSTONE_ORE = 74;
    public const int REDSTONE_TORCH_OFF = 75;
    public const int REDSTONE_TORCH_ON = 76;
    public const int STONE_BUTTON = 77;
    public const int SNOW = 78;
    public const int ICE = 79;
    public const int SNOW_BLOCK = 80;
    public const int CACTUS = 81;
    public const int CLAY_BLOCK = 82;
    public const int SUGAR_CANE = 83;
    public const int JUKEBOX = 84;
    public const int FENCE = 85;
    public const int PUMPKIN = 86;
    public const int NETHERRACK = 87;
    public const int SOUL_SAND = 88;
    public const int GLOWSTONE_BLOCK = 89;
    public const int PORTAL = 90;
    public const int JACK_O_LANTERN = 91;
    public const int CAKE_BLOCK = 92;
    public const int REDSTONE_REPEATER_OFF = 93;
    public const int REDSTONE_REPEATER_ON = 94;
    // public const int LOCKED_CHEST = 95;
    public const int STAINED_GLASS = 95;
    public const int TRAPDOOR = 96;
    public const int SILVERFISH_STONE = 97;
    public const int STONE_BRICK = 98;
    public const int HUGE_RED_MUSHROOM = 99;
    public const int HUGE_BROWN_MUSHROOM = 100;
    public const int IRON_BARS = 101;
    public const int GLASS_PANE = 102;
    public const int MELON = 103;
    public const int PUMPKIN_STEM = 104;
    public const int MELON_STEM = 105;
    public const int VINES = 106;
    public const int FENCE_GATE = 107;
    public const int BRICK_STAIRS = 108;
    public const int STONE_BRICK_STAIRS = 109;
    public const int MYCELIUM = 110;
    public const int LILLY_PAD = 111;
    public const int NETHER_BRICK = 112;
    public const int NETHER_BRICK_FENCE = 113;
    public const int NETHER_BRICK_STAIRS = 114;
    public const int NETHER_WART = 115;
    public const int ENCHANTMENT_TABLE = 116;
    public const int BREWING_STAND = 117;
    public const int CAULDRON = 118;
    public const int END_PORTAL = 119;
    public const int END_PORTAL_FRAME = 120;
    public const int END_STONE = 121;
    public const int DRAGON_EGG = 122;
    public const int REDSTONE_LAMP_OFF = 123;
    public const int REDSTONE_LAMP_ON = 124;
    public const int DOUBLE_WOOD_SLAB = 125;
    public const int WOOD_SLAB = 126;
    public const int COCOA_PLANT = 127;
    public const int SANDSTONE_STAIRS = 128;
    public const int EMERALD_ORE = 129;
    public const int ENDER_CHEST = 130;
    public const int TRIPWIRE_HOOK = 131;
    public const int TRIPWIRE = 132;
    public const int EMERALD_BLOCK = 133;
    public const int SPRUCE_WOOD_STAIRS = 134;
    public const int BIRCH_WOOD_STAIRS = 135;
    public const int JUNGLE_WOOD_STAIRS = 136;
    public const int COMMAND_BLOCK = 137;
    public const int BEACON_BLOCK = 138;
    public const int COBBLESTONE_WALL = 139;
    public const int FLOWER_POT = 140;
    public const int CARROTS = 141;
    public const int POTATOES = 142;
    public const int WOOD_BUTTON = 143;
    public const int HEADS = 144;
    public const int ANVIL = 145;
    public const int TRAPPED_CHEST = 146;
    public const int WEIGHTED_PRESSURE_PLATE_LIGHT = 147;
    public const int WEIGHTED_PRESSURE_PLATE_HEAVY = 148;
    public const int REDSTONE_COMPARATOR_INACTIVE = 149;
    public const int REDSTONE_COMPARATOR_ACTIVE = 150;
    public const int DAYLIGHT_SENSOR = 151;
    public const int REDSTONE_BLOCK = 152;
    public const int NETHER_QUARTZ_ORE = 153;
    public const int HOPPER = 154;
    public const int QUARTZ_BLOCK = 155;
    public const int QUARTZ_STAIRS = 156;
    public const int ACTIVATOR_RAIL = 157;
    public const int DROPPER = 158;
    public const int STAINED_CLAY = 159;
    public const int STAINED_GLASS_PANE = 160;
    public const int LEAVES2 = 161;
    public const int WOOD2 = 162;
    public const int ACACIA_WOOD_STAIRS = 163;
    public const int DARK_OAK_WOOD_STAIRS = 164;
    public const int SLIME_BLOCK = 165;
    public const int BARRIER = 166;
    public const int IRON_TRAPDOOR = 167;
    public const int PRISMARINE = 168;
    public const int SEA_LANTERN = 169;
    public const int HAY_BLOCK = 170;
    public const int CARPET = 171;
    public const int HARDENED_CLAY = 172;
    public const int COAL_BLOCK = 173;
    public const int PACKED_ICE = 174;
    public const int LARGE_FLOWERS = 175;
    public const int STANDING_BANNER = 176;
    public const int WALL_BANNER = 177;
    public const int INVERTED_DAYLIGHT_SENSOR = 178;
    public const int RED_SANDSTONE = 179;
    public const int RED_SANDSTONE_STAIRS = 180;
    public const int DOUBLE_RED_SANDSTONE_SLAB = 181;
    public const int RED_SANDSTONE_SLAB = 182;
    public const int SPRUCE_FENCE_GATE = 183;
    public const int BIRCH_FENCE_GATE = 184;
    public const int JUNGLE_FENCE_GATE = 185;
    public const int DARK_OAK_FENCE_GATE = 186;
    public const int ACACIA_FENCE_GATE = 187;
    public const int SPRUCE_FENCE = 188;
    public const int BIRCH_FENCE = 189;
    public const int JUNGLE_FENCE = 190;
    public const int DARK_OAK_FENCE = 191;
    public const int ACACIA_FENCE = 192;
    public const int SPRUCE_DOOR = 193;
    public const int BIRCH_DOOR = 194;
    public const int JUNGLE_DOOR = 195;
    public const int ACACIA_DOOR = 196;
    public const int DARK_OAK_DOOR = 197;
    public const int END_ROD = 198;
    public const int CHORUS_PLANT = 199;
    public const int CHORUS_FLOWER = 200;
    public const int PURPUR_BLOCK = 201;
    public const int PURPUR_PILLAR = 202;
    public const int PURPUR_STAIRS = 203;
    public const int PURPUR_DOUBLE_SLAB = 204;
    public const int PURPUR_SLAB = 205;
    public const int END_STONE_BRICKS = 206;
    public const int BEETROOT_SEEDS = 207;
    public const int GRASS_PATH = 208;
    public const int END_GATEWAY = 209;
    public const int REPEATING_COMMAND_BLOCK = 210;
    public const int CHAIN_COMMAND_BLOCK = 211;
    public const int FROSTED_ICE = 212;
    public const int MAGMA_BLOCK = 213;
    public const int NETHER_WART_BLOCK = 214;
    public const int RED_NETHER_BRICK = 215;
    public const int BONE_BLOCK = 216;
    public const int STRUCTURE_VOID = 217;
    public const int OBSERVER = 218;
    public const int WHITE_SHULKER_BOX = 219;
    public const int ORANGE_SHULKER_BOX = 220;
    public const int MAGENTA_SHULKER_BOX = 221;
    public const int LIGHT_BLUE_SHULKER_BOX = 222;
    public const int YELLOW_SHULKER_BOX = 223;
    public const int LIME_SHULKER_BOX = 224;
    public const int PINK_SHULKER_BOX = 225;
    public const int GRAY_SHULKER_BOX = 226;
    public const int LIGHT_GRAY_SHULKER_BOX = 227;
    public const int CYAN_SHULKER_BOX = 228;
    public const int PURPLE_SHULKER_BOX = 229;
    public const int BLUE_SHULKER_BOX = 230;
    public const int BROWN_SHULKER_BOX = 231;
    public const int GREEN_SHULKER_BOX = 232;
    public const int RED_SHULKER_BOX = 233;
    public const int BLACK_SHULKER_BOX = 234;
    public const int WHITE_GLAZED_TERRACOTTA = 235;
    public const int ORANGE_GLAZED_TERRACOTTA = 236;
    public const int MAGENTA_GLAZED_TERRACOTTA = 237;
    public const int LIGHT_BLUE_GLAZED_TERRACOTTA = 238;
    public const int YELLOW_GLAZED_TERRACOTTA = 239;
    public const int LIME_GLAZED_TERRACOTTA = 240;
    public const int PINK_GLAZED_TERRACOTTA = 241;
    public const int GRAY_GLAZED_TERRACOTTA = 242;
    public const int LIGHT_GRAY_GLAZED_TERRACOTTA = 243;
    public const int CYAN_GLAZED_TERRACOTTA = 244;
    public const int PURPLE_GLAZED_TERRACOTTA = 245;
    public const int BLUE_GLAZED_TERRACOTTA = 246;
    public const int BROWN_GLAZED_TERRACOTTA = 247;
    public const int GREEN_GLAZED_TERRACOTTA = 248;
    public const int RED_GLAZED_TERRACOTTA = 249;
    public const int BLACK_GLAZED_TERRACOTTA = 250;
    public const int CONCRETE = 251;
    public const int CONCRETE_POWDER = 252;
    public const int STRUCTURE_BLOCK = 255;
}

/// <summary>
/// Represents the physical state of a block, such as solid or fluid.
/// </summary>
public enum BlockState {
    /// <summary>
    /// A solid state that stops movement.
    /// </summary>
    SOLID,

    /// <summary>
    /// A nonsolid state that can be passed through.
    /// </summary>
    NONSOLID,

    /// <summary>
    /// A fluid state that flows and impedes movement.
    /// </summary>
    FLUID
}

/// <summary>
/// Provides information on a specific type of block.
/// </summary>
/// <remarks>By default, all known MC block types are already defined and registered, assuming Substrate
/// is up to date with the current MC version.  All unknown blocks are given a default type and unregistered status.
/// New block types may be created and used at runtime, and will automatically populate various static lookup tables
/// in the <see cref="BlockInfo"/> class.</remarks>
public class BlockInfo {
    /// <summary>
    /// The maximum number of sequential blocks starting at 0 that can be registered.
    /// </summary>
    public const int MAX_BLOCKS = 4096;

    /// <summary>
    /// The maximum opacity value that can be assigned to a block (fully opaque).
    /// </summary>
    public const int MAX_OPACITY = 15;

    /// <summary>
    /// The minimum opacity value that can be assigned to a block (fully transparent).
    /// </summary>
    public const int MIN_OPACITY = 0;

    /// <summary>
    /// The maximum luminance value that can be assigned to a block.
    /// </summary>
    public const int MAX_LUMINANCE = 15;

    /// <summary>
    /// The minimum luminance value that can be assigned to a block.
    /// </summary>
    public const int MIN_LUMINANCE = 0;

    private static readonly BlockInfo[] blockTable;
    private static readonly int[] opacityTable;
    private static readonly int[] luminanceTable;

    private class CacheTableArray<T> : ICacheTable<T> {
        private T[] cache;

        public T this[int index] {
            get { return this.cache[index]; }
        }

        public CacheTableArray(T[] cache) {
            this.cache = cache;
        }

        public IEnumerator<T> GetEnumerator() {
            for(int i = 0; i < this.cache.Length; i++) {
                if(this.cache[i] != null) {
                    yield return this.cache[i];
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }

    private class DataLimits {
        private int low;
        private int high;
        private int bitmask;

        public int Low {
            get { return this.low; }
        }

        public int High {
            get { return this.high; }
        }

        public int Bitmask {
            get { return this.bitmask; }
        }

        public DataLimits(int low, int high, int bitmask) {
            this.low = low;
            this.high = high;
            this.bitmask = bitmask;
        }

        public bool Test(int data) {
            int rdata = data & ~this.bitmask;
            return rdata >= this.low && rdata <= this.high;
        }
    }

    private int id = 0;
    private string name = "";
    private int tick = 0;
    private int opacity = MAX_OPACITY;
    private int luminance = MIN_LUMINANCE;
    private bool transmitLight = false;
    private bool blocksFluid = true;
    private bool registered = false;

    private BlockState state = BlockState.SOLID;

    private DataLimits dataLimits;

    private static readonly CacheTableArray<BlockInfo> blockTableCache;
    private static readonly CacheTableArray<int> opacityTableCache;
    private static readonly CacheTableArray<int> luminanceTableCache;

    /// <summary>
    /// Gets the lookup table for id-to-info values.
    /// </summary>
    public static ICacheTable<BlockInfo> BlockTable {
        get { return blockTableCache; }
    }

    /// <summary>
    /// Gets the lookup table for id-to-opacity values.
    /// </summary>
    public static ICacheTable<int> OpacityTable {
        get { return opacityTableCache; }
    }

    /// <summary>
    /// Gets the lookup table for id-to-luminance values.
    /// </summary>
    public static ICacheTable<int> LuminanceTable {
        get { return luminanceTableCache; }
    }

    /// <summary>
    /// Get's the block's Id.
    /// </summary>
    public int ID {
        get { return this.id; }
    }

    /// <summary>
    /// Get's the name of the block type.
    /// </summary>
    public string Name {
        get { return this.name; }
    }

    /// <summary>
    /// Gets the block's opacity value.  An opacity of 0 is fully transparent to light.
    /// </summary>
    public int Opacity {
        get { return this.opacity; }
    }

    /// <summary>
    /// Gets the block's luminance value.
    /// </summary>
    /// <remarks>Blocks with luminance act as light sources and transmit light to other blocks.</remarks>
    public int Luminance {
        get { return this.luminance; }
    }

    /// <summary>
    /// Checks whether the block transmits light to neighboring blocks.
    /// </summary>
    /// <remarks>A block may stop the transmission of light, but still be illuminated.</remarks>
    public bool TransmitsLight {
        get { return this.transmitLight; }
    }

    /// <summary>
    /// Checks whether the block partially or fully blocks the transmission of light.
    /// </summary>
    public bool ObscuresLight {
        get { return this.opacity > MIN_OPACITY || !this.transmitLight; }
    }

    /// <summary>
    /// Checks whether the block stops fluid from passing through it.
    /// </summary>
    /// <remarks>A block that does not block fluids will be destroyed by fluid.</remarks>
    public bool BlocksFluid {
        get { return this.blocksFluid; }
    }

    /// <summary>
    /// Gets the block's physical state type.
    /// </summary>
    public BlockState State {
        get { return this.state; }
    }

    /// <summary>
    /// Checks whether this block type has been registered as a known type.
    /// </summary>
    public bool Registered {
        get { return this.registered; }
    }

    public int Tick {
        get { return this.tick; }
    }

    internal BlockInfo(int id) {
        this.id = id;
        this.name = "Unknown Block";
        blockTable[this.id] = this;
    }

    /// <summary>
    /// Constructs a new <see cref="BlockInfo"/> record for a given block id and name.
    /// </summary>
    /// <param name="id">The id of the block.</param>
    /// <param name="name">The name of the block.</param>
    /// <remarks>All user-constructed <see cref="BlockInfo"/> objects are registered automatically.</remarks>
    public BlockInfo(int id, string name) {
        this.id = id;
        this.name = name;
        blockTable[this.id] = this;
        this.registered = true;
    }

    /// <summary>
    /// Sets a new opacity value for this block type.
    /// </summary>
    /// <param name="opacity">A new opacity value.</param>
    /// <returns>The object instance used to invoke this method.</returns>
    /// <seealso cref="AlphaBlockCollection.AutoLight"/>
    public BlockInfo SetOpacity(int opacity) {
        this.opacity = MIN_OPACITY + opacity;
        opacityTable[this.id] = this.opacity;

        if(opacity == MAX_OPACITY) {
            this.transmitLight = false;
        } else {
            this.transmitLight = true;
        }

        return this;
    }

    /// <summary>
    /// Sets a new luminance value for this block type.
    /// </summary>
    /// <param name="luminance">A new luminance value.</param>
    /// <returns>The object instance used to invoke this method.</returns>
    /// <seealso cref="AlphaBlockCollection.AutoLight"/>
    public BlockInfo SetLuminance(int luminance) {
        this.luminance = luminance;
        luminanceTable[this.id] = this.luminance;
        return this;
    }

    /// <summary>
    /// Sets whether or not this block type will transmit light to neigboring blocks.
    /// </summary>
    /// <param name="transmit">True if this block type can transmit light to neighbors, false otherwise.</param>
    /// <returns>The object instance used to invoke this method.</returns>
    /// <seealso cref="AlphaBlockCollection.AutoLight"/>
    public BlockInfo SetLightTransmission(bool transmit) {
        this.transmitLight = transmit;
        return this;
    }

    /// <summary>
    /// Sets limitations on what data values are considered valid for this block type.
    /// </summary>
    /// <param name="low">The lowest valid integer value.</param>
    /// <param name="high">The highest valid integer value.</param>
    /// <param name="bitmask">A mask representing which bits are interpreted as a bitmask in the data value.</param>
    /// <returns>The object instance used to invoke this method.</returns>
    public BlockInfo SetDataLimits(int low, int high, int bitmask) {
        this.dataLimits = new DataLimits(low, high, bitmask);
        return this;
    }

    /// <summary>
    /// Sets the physical state of the block type.
    /// </summary>
    /// <param name="state">A physical state.</param>
    /// <returns>The object instance used to invoke this method.</returns>
    public BlockInfo SetState(BlockState state) {
        this.state = state;

        if(this.state == BlockState.SOLID) {
            this.blocksFluid = true;
        } else {
            this.blocksFluid = false;
        }

        return this;
    }

    /// <summary>
    /// Sets whether or not this block type blocks fluids.
    /// </summary>
    /// <param name="blocks">True if this block type blocks fluids, false otherwise.</param>
    /// <returns>The object instance used to invoke this method.</returns>
    /// <seealso cref="AlphaBlockCollection.AutoFluid"/>
    public BlockInfo SetBlocksFluid(bool blocks) {
        this.blocksFluid = blocks;
        return this;
    }

    /// <summary>
    /// Sets the default tick rate/delay used for updating this block.
    /// </summary>
    /// <remarks>Set <paramref name="tick"/> to <c>0</c> to indicate that this block is not processed by tick updates.</remarks>
    /// <param name="tick">The tick rate in frames between scheduled updates on this block.</param>
    /// <returns>The object instance used to invoke this method.</returns>
    /// <seealso cref="AlphaBlockCollection.AutoTileTick"/>
    public BlockInfo SetTick(int tick) {
        this.tick = tick;
        return this;
    }

    /// <summary>
    /// Tests if the given data value is valid for this block type.
    /// </summary>
    /// <param name="data">A data value to test.</param>
    /// <returns>True if the data value is valid, false otherwise.</returns>
    /// <remarks>This method uses internal information set by <see cref="SetDataLimits"/>.</remarks>
    public bool TestData(int data) {
        if(this.dataLimits == null) {
            return true;
        }
        return this.dataLimits.Test(data);
    }

    public static BlockInfo Air;
    public static BlockInfo Stone;
    public static BlockInfo Grass;
    public static BlockInfo Dirt;
    public static BlockInfo Cobblestone;
    public static BlockInfo WoodPlank;
    public static BlockInfo Sapling;
    public static BlockInfo Bedrock;
    public static BlockInfo Water;
    public static BlockInfo StationaryWater;
    public static BlockInfo Lava;
    public static BlockInfo StationaryLava;
    public static BlockInfo Sand;
    public static BlockInfo Gravel;
    public static BlockInfo GoldOre;
    public static BlockInfo IronOre;
    public static BlockInfo CoalOre;
    public static BlockInfo Wood;
    public static BlockInfo Leaves;
    public static BlockInfo Sponge;
    public static BlockInfo Glass;
    public static BlockInfo LapisOre;
    public static BlockInfo LapisBlock;
    public static BlockInfoEx Dispenser;
    public static BlockInfo Sandstone;
    public static BlockInfoEx NoteBlock;
    public static BlockInfoEx Bed;
    public static BlockInfo PoweredRail;
    public static BlockInfo DetectorRail;
    public static BlockInfo StickyPiston;
    public static BlockInfo Cobweb;
    public static BlockInfo TallGrass;
    public static BlockInfo DeadShrub;
    public static BlockInfo Piston;
    public static BlockInfo PistonHead;
    public static BlockInfo Wool;
    public static BlockInfoEx PistonMoving;
    public static BlockInfo YellowFlower;
    public static BlockInfo RedRose;
    public static BlockInfo BrownMushroom;
    public static BlockInfo RedMushroom;
    public static BlockInfo GoldBlock;
    public static BlockInfo IronBlock;
    public static BlockInfo DoubleStoneSlab;
    public static BlockInfo StoneSlab;
    public static BlockInfo BrickBlock;
    public static BlockInfo TNT;
    public static BlockInfo Bookshelf;
    public static BlockInfo MossStone;
    public static BlockInfo Obsidian;
    public static BlockInfo Torch;
    public static BlockInfo Fire;
    public static BlockInfoEx MonsterSpawner;
    public static BlockInfo WoodStairs;
    public static BlockInfoEx Chest;
    public static BlockInfo RedstoneWire;
    public static BlockInfo DiamondOre;
    public static BlockInfo DiamondBlock;
    public static BlockInfo CraftTable;
    public static BlockInfo Crops;
    public static BlockInfo Farmland;
    public static BlockInfoEx Furnace;
    public static BlockInfoEx BurningFurnace;
    public static BlockInfoEx SignPost;
    public static BlockInfo WoodDoor;
    public static BlockInfo Ladder;
    public static BlockInfo Rails;
    public static BlockInfo CobbleStairs;
    public static BlockInfoEx WallSign;
    public static BlockInfo Lever;
    public static BlockInfo StonePlate;
    public static BlockInfo IronDoor;
    public static BlockInfo WoodPlate;
    public static BlockInfo RedstoneOre;
    public static BlockInfo GlowRedstoneOre;
    public static BlockInfo RedstoneTorch;
    public static BlockInfo RedstoneTorchOn;
    public static BlockInfo StoneButton;
    public static BlockInfo Snow;
    public static BlockInfo Ice;
    public static BlockInfo SnowBlock;
    public static BlockInfo Cactus;
    public static BlockInfo ClayBlock;
    public static BlockInfo SugarCane;
    public static BlockInfoEx Jukebox;
    public static BlockInfo Fence;
    public static BlockInfo Pumpkin;
    public static BlockInfo Netherrack;
    public static BlockInfo SoulSand;
    public static BlockInfo Glowstone;
    public static BlockInfo Portal;
    public static BlockInfo JackOLantern;
    public static BlockInfo CakeBlock;
    public static BlockInfo RedstoneRepeater;
    public static BlockInfo RedstoneRepeaterOn;
    // public static BlockInfoEx LockedChest;
    public static BlockInfo StainedGlass;
    public static BlockInfo Trapdoor;
    public static BlockInfo SilverfishStone;
    public static BlockInfo StoneBrick;
    public static BlockInfo HugeRedMushroom;
    public static BlockInfo HugeBrownMushroom;
    public static BlockInfo IronBars;
    public static BlockInfo GlassPane;
    public static BlockInfo Melon;
    public static BlockInfo PumpkinStem;
    public static BlockInfo MelonStem;
    public static BlockInfo Vines;
    public static BlockInfo FenceGate;
    public static BlockInfo BrickStairs;
    public static BlockInfo StoneBrickStairs;
    public static BlockInfo Mycelium;
    public static BlockInfo LillyPad;
    public static BlockInfo NetherBrick;
    public static BlockInfo NetherBrickFence;
    public static BlockInfo NetherBrickStairs;
    public static BlockInfo NetherWart;
    public static BlockInfoEx EnchantmentTable;
    public static BlockInfoEx BrewingStand;
    public static BlockInfo Cauldron;
    public static BlockInfoEx EndPortal;
    public static BlockInfo EndPortalFrame;
    public static BlockInfo EndStone;
    public static BlockInfo DragonEgg;
    public static BlockInfo RedstoneLampOff;
    public static BlockInfo RedstoneLampOn;
    public static BlockInfo DoubleWoodSlab;
    public static BlockInfo WoodSlab;
    public static BlockInfo CocoaPlant;
    public static BlockInfo SandstoneStairs;
    public static BlockInfo EmeraldOre;
    public static BlockInfoEx EnderChest;
    public static BlockInfo TripwireHook;
    public static BlockInfo Tripwire;
    public static BlockInfo EmeraldBlock;
    public static BlockInfo SpruceWoodStairs;
    public static BlockInfo BirchWoodStairs;
    public static BlockInfo JungleWoodStairs;
    public static BlockInfoEx CommandBlock;
    public static BlockInfoEx BeaconBlock;
    public static BlockInfo CobblestoneWall;
    public static BlockInfoEx FlowerPot;
    public static BlockInfo Carrots;
    public static BlockInfo Potatoes;
    public static BlockInfo WoodButton;
    public static BlockInfoEx Heads;
    public static BlockInfo Anvil;
    public static BlockInfoEx TrappedChest;
    public static BlockInfo WeightedPressurePlateLight;
    public static BlockInfo WeightedPressurePlateHeavy;
    public static BlockInfo RedstoneComparatorInactive;
    public static BlockInfo RedstoneComparatorActive;
    public static BlockInfoEx DaylightSensor;
    public static BlockInfo RedstoneBlock;
    public static BlockInfo NetherQuartzOre;
    public static BlockInfoEx Hopper;
    public static BlockInfo QuartzBlock;
    public static BlockInfo QuartzStairs;
    public static BlockInfo ActivatorRail;
    public static BlockInfoEx Dropper;
    public static BlockInfo StainedClay;
    public static BlockInfo StainedGlassPane;
    public static BlockInfo Leaves2;
    public static BlockInfo Wood2;
    public static BlockInfo AcaciaWoodStairs;
    public static BlockInfo DarkOakWoodStairs;
    public static BlockInfo SlimeBlock;
    public static BlockInfo Barrier;
    public static BlockInfo IronTrapdoor;
    public static BlockInfo Prismarine;
    public static BlockInfo SeaLantern;
    public static BlockInfo HayBlock;
    public static BlockInfo Carpet;
    public static BlockInfo HardenedClay;
    public static BlockInfo CoalBlock;
    public static BlockInfo PackedIce;
    public static BlockInfo LargeFlowers;
    public static BlockInfoEx StandingBanner;
    public static BlockInfoEx WallBanner;
    public static BlockInfoEx InvertedDaylightSensor;
    public static BlockInfo RedSandstone;
    public static BlockInfo RedSandstoneStairs;
    public static BlockInfo DoubleRedSandstoneSlab;
    public static BlockInfo RedSandstoneSlab;
    public static BlockInfo SpruceFenceGate;
    public static BlockInfo BirchFenceGate;
    public static BlockInfo JungleFenceGate;
    public static BlockInfo DarkOakFenceGate;
    public static BlockInfo AcaciaFenceGate;
    public static BlockInfo SpruceFence;
    public static BlockInfo BirchFence;
    public static BlockInfo JungleFence;
    public static BlockInfo DarkOakFence;
    public static BlockInfo AcaciaFence;
    public static BlockInfo SpruceDoor;
    public static BlockInfo BirchDoor;
    public static BlockInfo JungleDoor;
    public static BlockInfo AcaciaDoor;
    public static BlockInfo DarkOakDoor;
    public static BlockInfo EndRod;
    public static BlockInfo ChorusPlant;
    public static BlockInfo ChorusFlower;
    public static BlockInfo PurpurBlock;
    public static BlockInfo PurpurPillar;
    public static BlockInfo PurpurStairs;
    public static BlockInfo PurpurDoubleSlab;
    public static BlockInfo PurpurSlab;
    public static BlockInfo EndStoneBricks;
    public static BlockInfo BeetrootSeeds;
    public static BlockInfo GrassPath;
    public static BlockInfoEx EndGateway;
    public static BlockInfoEx RepeatingCommandBlock;
    public static BlockInfoEx ChainCommandBlock;
    public static BlockInfo FrostedIce;
    public static BlockInfo MagmaBlock;
    public static BlockInfo NetherWartBlock;
    public static BlockInfo RedNetherBrick;
    public static BlockInfo BoneBlock;
    public static BlockInfo StructureVoid;
    public static BlockInfo Observer;
    public static BlockInfoEx WhiteShulkerBox;
    public static BlockInfoEx OrangeShulkerBox;
    public static BlockInfoEx MagentaShulkerBox;
    public static BlockInfoEx LightBlueShulkerBox;
    public static BlockInfoEx YellowShulkerBox;
    public static BlockInfoEx LimeShulkerBox;
    public static BlockInfoEx PinkShulkerBox;
    public static BlockInfoEx GrayShulkerBox;
    public static BlockInfoEx LightGrayShulkerBox;
    public static BlockInfoEx CyanShulkerBox;
    public static BlockInfoEx PurpleShulkerBox;
    public static BlockInfoEx BlueShulkerBox;
    public static BlockInfoEx BrownShulkerBox;
    public static BlockInfoEx GreenShulkerBox;
    public static BlockInfoEx RedShulkerBox;
    public static BlockInfoEx BlackShulkerBox;
    public static BlockInfo WhiteGlazedTerracotta;
    public static BlockInfo OrangeGlazedTerracotta;
    public static BlockInfo MagentaGlazedTerracotta;
    public static BlockInfo LightBlueGlazedTerracotta;
    public static BlockInfo YellowGlazedTerracotta;
    public static BlockInfo LimeGlazedTerracotta;
    public static BlockInfo PinkGlazedTerracotta;
    public static BlockInfo GrayGlazedTerracotta;
    public static BlockInfo LightGrayGlazedTerracotta;
    public static BlockInfo CyanGlazedTerracotta;
    public static BlockInfo PurpleGlazedTerracotta;
    public static BlockInfo BlueGlazedTerracotta;
    public static BlockInfo BrownGlazedTerracotta;
    public static BlockInfo GreenGlazedTerracotta;
    public static BlockInfo RedGlazedTerracotta;
    public static BlockInfo BlackGlazedTerracotta;
    public static BlockInfo Concrete;
    public static BlockInfo ConcretePowder;
    public static BlockInfoEx StructureBlock;

    static BlockInfo() {
        blockTable = new BlockInfo[MAX_BLOCKS];
        opacityTable = new int[MAX_BLOCKS];
        luminanceTable = new int[MAX_BLOCKS];

        blockTableCache = new CacheTableArray<BlockInfo>(blockTable);
        opacityTableCache = new CacheTableArray<int>(opacityTable);
        luminanceTableCache = new CacheTableArray<int>(luminanceTable);

        Air = new BlockInfo(0, "Air").SetOpacity(0).SetState(BlockState.NONSOLID);
        Stone = new BlockInfo(1, "Stone").SetDataLimits(0, 6, 0);
        Grass = new BlockInfo(2, "Grass").SetTick(10).SetDataLimits(0, 2, 0);
        Dirt = new BlockInfo(3, "Dirt").SetDataLimits(0, 2, 0);
        Cobblestone = new BlockInfo(4, "Cobblestone");
        WoodPlank = new BlockInfo(5, "Wooden Plank").SetDataLimits(0, 5, 0);
        Sapling = new BlockInfo(6, "Sapling").SetOpacity(0).SetState(BlockState.NONSOLID).SetTick(10).SetDataLimits(0, 5, 0x8);
        Bedrock = new BlockInfo(7, "Bedrock");
        Water = new BlockInfo(8, "Water").SetOpacity(3).SetState(BlockState.FLUID).SetTick(5).SetDataLimits(0, 7, 0x8);
        StationaryWater = new BlockInfo(9, "Stationary Water").SetOpacity(3).SetState(BlockState.FLUID).SetDataLimits(0, 15, 0);
        Lava = new BlockInfo(10, "Lava").SetOpacity(0).SetLuminance(MAX_LUMINANCE).SetState(BlockState.FLUID).SetTick(30).SetLightTransmission(false).SetDataLimits(0, 7, 0x8);
        StationaryLava = new BlockInfo(11, "Stationary Lava").SetOpacity(0).SetLuminance(MAX_LUMINANCE).SetState(BlockState.FLUID).SetDataLimits(0, 15, 0).SetTick(10).SetLightTransmission(false);
        Sand = new BlockInfo(12, "Sand").SetTick(3).SetDataLimits(0, 1, 0);
        Gravel = new BlockInfo(13, "Gravel").SetTick(3);
        GoldOre = new BlockInfo(14, "Gold Ore");
        IronOre = new BlockInfo(15, "Iron Ore");
        CoalOre = new BlockInfo(16, "Coal Ore");
        Wood = new BlockInfo(17, "Wood").SetDataLimits(0, 15, 0);
        Leaves = new BlockInfo(18, "Leaves").SetOpacity(1).SetTick(10).SetDataLimits(0, 3, 0xC);
        Sponge = new BlockInfo(19, "Sponge").SetDataLimits(0, 1, 0);
        Glass = new BlockInfo(20, "Glass").SetOpacity(0);
        LapisOre = new BlockInfo(21, "Lapis Lazuli Ore");
        LapisBlock = new BlockInfo(22, "Lapis Lazuli Block");
        Dispenser = (BlockInfoEx) new BlockInfoEx(23, "Dispenser").SetTick(4).SetDataLimits(0, 13, 0x8);
        Sandstone = new BlockInfo(24, "Sandstone").SetDataLimits(0, 2, 0);
        NoteBlock = (BlockInfoEx) new BlockInfoEx(25, "Note Block");
        Bed = (BlockInfoEx) new BlockInfoEx(26, "Bed").SetOpacity(0).SetDataLimits(0, 3, 0xC);
        PoweredRail = new BlockInfo(27, "Powered Rail").SetOpacity(0).SetState(BlockState.NONSOLID).SetDataLimits(0, 5, 0x8);
        DetectorRail = new BlockInfo(28, "Detector Rail").SetOpacity(0).SetState(BlockState.NONSOLID).SetTick(20).SetDataLimits(0, 5, 0x8);
        StickyPiston = new BlockInfo(29, "Sticky Piston").SetOpacity(0).SetDataLimits(0, 5, 0x8);
        Cobweb = new BlockInfo(30, "Cobweb").SetOpacity(0).SetState(BlockState.NONSOLID);
        TallGrass = new BlockInfo(31, "Tall Grass").SetOpacity(0).SetState(BlockState.NONSOLID).SetDataLimits(0, 2, 0);
        DeadShrub = new BlockInfo(32, "Dead Shrub").SetOpacity(0).SetState(BlockState.NONSOLID);
        Piston = new BlockInfo(33, "Piston").SetOpacity(0).SetDataLimits(0, 5, 0x8);
        PistonHead = new BlockInfo(34, "Piston Head").SetOpacity(0).SetDataLimits(0, 5, 0x8);
        Wool = new BlockInfo(35, "Wool").SetDataLimits(0, 15, 0);
        PistonMoving = (BlockInfoEx) new BlockInfoEx(36, "Piston Extension").SetOpacity(0).SetDataLimits(0, 19, 0);
        YellowFlower = new BlockInfo(37, "Yellow Flower").SetOpacity(0).SetState(BlockState.NONSOLID).SetTick(10).SetDataLimits(0, 0, 0);
        RedRose = new BlockInfo(38, "Red Rose").SetOpacity(0).SetState(BlockState.NONSOLID).SetTick(10).SetDataLimits(0, 8, 0);
        BrownMushroom = new BlockInfo(39, "Brown Mushroom").SetOpacity(0).SetLuminance(1).SetState(BlockState.NONSOLID).SetTick(10).SetDataLimits(0, 15, 0);
        RedMushroom = new BlockInfo(40, "Red Mushroom").SetOpacity(0).SetState(BlockState.NONSOLID).SetTick(10).SetDataLimits(0, 15, 0);
        GoldBlock = new BlockInfo(41, "Gold Block");
        IronBlock = new BlockInfo(42, "Iron Block");
        DoubleStoneSlab = new BlockInfo(43, "Double Slab").SetDataLimits(0, 9, 0x8);
        StoneSlab = new BlockInfo(44, "Slab").SetOpacity(0).SetLightTransmission(false).SetDataLimits(0, 9, 0x8);
        BrickBlock = new BlockInfo(45, "Brick Block");
        TNT = new BlockInfo(46, "TNT").SetDataLimits(0, 1, 0);
        Bookshelf = new BlockInfo(47, "Bookshelf");
        MossStone = new BlockInfo(48, "Moss Stone");
        Obsidian = new BlockInfo(49, "Obsidian");
        Torch = new BlockInfo(50, "Torch").SetOpacity(0).SetLuminance(MAX_LUMINANCE - 1).SetState(BlockState.NONSOLID).SetTick(10).SetDataLimits(1, 5, 0);
        Fire = new BlockInfo(51, "Fire").SetOpacity(0).SetLuminance(MAX_LUMINANCE).SetState(BlockState.NONSOLID).SetTick(40).SetDataLimits(0, 15, 0);
        MonsterSpawner = (BlockInfoEx) new BlockInfoEx(52, "Monster Spawner").SetOpacity(0);
        WoodStairs = new BlockInfo(53, "Wooden Stairs").SetOpacity(0).SetLightTransmission(false).SetDataLimits(0, 3, 0x4);
        Chest = (BlockInfoEx) new BlockInfoEx(54, "Chest").SetOpacity(0).SetDataLimits(2, 5, 0);
        RedstoneWire = new BlockInfo(55, "Redstone Wire").SetOpacity(0).SetState(BlockState.NONSOLID).SetDataLimits(0, 15, 0);
        DiamondOre = new BlockInfo(56, "Diamond Ore");
        DiamondBlock = new BlockInfo(57, "Diamond Block");
        CraftTable = new BlockInfo(58, "Crafting Table");
        Crops = new BlockInfo(59, "Crops").SetOpacity(0).SetState(BlockState.NONSOLID).SetTick(10).SetDataLimits(0, 7, 0);
        Farmland = new BlockInfo(60, "Farmland").SetOpacity(0).SetTick(10).SetLightTransmission(false).SetDataLimits(0, 7, 0);
        Furnace = (BlockInfoEx) new BlockInfoEx(61, "Furnace").SetDataLimits(2, 5, 0);
        BurningFurnace = (BlockInfoEx) new BlockInfoEx(62, "Burning Furnace").SetLuminance(MAX_LUMINANCE - 1).SetDataLimits(2, 5, 0);
        SignPost = (BlockInfoEx) new BlockInfoEx(63, "Sign Post").SetOpacity(0).SetState(BlockState.NONSOLID).SetBlocksFluid(true).SetDataLimits(0, 15, 0);
        WoodDoor = new BlockInfo(64, "Wooden Door").SetOpacity(0).SetDataLimits(0, 0, 0xF);
        Ladder = new BlockInfo(65, "Ladder").SetOpacity(0).SetDataLimits(2, 5, 0);
        Rails = new BlockInfo(66, "Rails").SetOpacity(0).SetState(BlockState.NONSOLID).SetDataLimits(0, 9, 0);
        CobbleStairs = new BlockInfo(67, "Cobblestone Stairs").SetOpacity(0).SetLightTransmission(false).SetDataLimits(0, 3, 0x4);
        WallSign = (BlockInfoEx) new BlockInfoEx(68, "Wall Sign").SetOpacity(0).SetState(BlockState.NONSOLID).SetBlocksFluid(true).SetDataLimits(2, 5, 0);
        Lever = new BlockInfo(69, "Lever").SetOpacity(0).SetState(BlockState.NONSOLID).SetDataLimits(0, 7, 0x8);
        StonePlate = new BlockInfo(70, "Stone Pressure Plate").SetOpacity(0).SetState(BlockState.NONSOLID).SetTick(20).SetDataLimits(0, 0, 0x1);
        IronDoor = new BlockInfo(71, "Iron Door").SetOpacity(0).SetDataLimits(0, 0, 0xF);
        WoodPlate = new BlockInfo(72, "Wooden Pressure Plate").SetOpacity(0).SetState(BlockState.NONSOLID).SetTick(20).SetDataLimits(0, 0, 0x1);
        RedstoneOre = new BlockInfo(73, "Redstone Ore").SetTick(30);
        GlowRedstoneOre = new BlockInfo(74, "Glowing Redstone Ore").SetLuminance(9).SetTick(30);
        RedstoneTorch = new BlockInfo(75, "Redstone Torch (Off)").SetOpacity(0).SetState(BlockState.NONSOLID).SetTick(2).SetDataLimits(1, 5, 0);
        RedstoneTorchOn = new BlockInfo(76, "Redstone Torch (On)").SetOpacity(0).SetLuminance(7).SetState(BlockState.NONSOLID).SetTick(2).SetDataLimits(1, 5, 0);
        StoneButton = new BlockInfo(77, "Stone Button").SetOpacity(0).SetState(BlockState.NONSOLID).SetDataLimits(0, 5, 0x8);
        Snow = new BlockInfo(78, "Snow").SetOpacity(0).SetState(BlockState.NONSOLID).SetTick(10).SetDataLimits(0, 7, 0);
        Ice = new BlockInfo(79, "Ice").SetOpacity(3).SetTick(10);
        SnowBlock = new BlockInfo(80, "Snow Block").SetTick(10);
        Cactus = new BlockInfo(81, "Cactus").SetOpacity(0).SetTick(10).SetBlocksFluid(true).SetDataLimits(0, 15, 0);
        ClayBlock = new BlockInfo(82, "Clay Block");
        SugarCane = new BlockInfo(83, "Sugar Cane").SetOpacity(0).SetState(BlockState.NONSOLID).SetTick(10).SetDataLimits(0, 15, 0);
        Jukebox = (BlockInfoEx) new BlockInfoEx(84, "Jukebox").SetDataLimits(0, 1, 0);
        Fence = new BlockInfo(85, "Fence").SetOpacity(0);
        Pumpkin = new BlockInfo(86, "Pumpkin").SetDataLimits(0, 3, 0x4);
        Netherrack = new BlockInfo(87, "Netherrack");
        SoulSand = new BlockInfo(88, "Soul Sand");
        Glowstone = new BlockInfo(89, "Glowstone Block").SetLuminance(MAX_LUMINANCE);
        Portal = new BlockInfo(90, "Portal").SetOpacity(0).SetLuminance(11).SetState(BlockState.NONSOLID).SetDataLimits(0, 2, 0);
        JackOLantern = new BlockInfo(91, "Jack-O-Lantern").SetLuminance(MAX_LUMINANCE).SetDataLimits(0, 3, 0x4);
        CakeBlock = new BlockInfo(92, "Cake Block").SetOpacity(0).SetDataLimits(0, 6, 0);
        RedstoneRepeater = new BlockInfo(93, "Redstone Repeater (Off)").SetOpacity(0).SetTick(10).SetDataLimits(0, 0, 0xF);
        RedstoneRepeaterOn = new BlockInfo(94, "Redstone Repeater (On)").SetOpacity(0).SetLuminance(7).SetTick(10).SetDataLimits(0, 0, 0xF);
        //LockedChest = (BlockInfoEx) new BlockInfoEx(95, "Locked Chest").SetLuminance(MAX_LUMINANCE).SetTick(10);
        StainedGlass = new BlockInfo(95, "Stained Glass").SetOpacity(0).SetDataLimits(0, 15, 0);
        Trapdoor = new BlockInfo(96, "Trapdoor").SetOpacity(0).SetDataLimits(0, 3, 0xC);
        SilverfishStone = new BlockInfo(97, "Stone with Silverfish").SetDataLimits(0, 5, 0);
        StoneBrick = new BlockInfo(98, "Stone Brick").SetDataLimits(0, 3, 0);
        HugeRedMushroom = new BlockInfo(99, "Huge Red Mushroom").SetDataLimits(0, 15, 0); // VERIFYME data values
        HugeBrownMushroom = new BlockInfo(100, "Huge Brown Mushroom").SetDataLimits(0, 15, 0); // VERIFYME data values
        IronBars = new BlockInfo(101, "Iron Bars").SetOpacity(0);
        GlassPane = new BlockInfo(102, "Glass Pane").SetOpacity(0);
        Melon = new BlockInfo(103, "Melon");
        PumpkinStem = new BlockInfo(104, "Pumpkin Stem").SetOpacity(0).SetState(BlockState.NONSOLID).SetTick(10).SetDataLimits(0, 7, 0);
        MelonStem = new BlockInfo(105, "Melon Stem").SetOpacity(0).SetState(BlockState.NONSOLID).SetTick(10).SetDataLimits(0, 7, 0);
        Vines = new BlockInfo(106, "Vines").SetOpacity(0).SetState(BlockState.NONSOLID).SetTick(10).SetDataLimits(0, 0, 0xF);
        FenceGate = new BlockInfo(107, "Fence Gate").SetOpacity(0).SetDataLimits(0, 3, 0xC); // VERIFYME data values
        BrickStairs = new BlockInfo(108, "Brick Stairs").SetOpacity(0).SetLightTransmission(false).SetDataLimits(0, 3, 0x4);
        StoneBrickStairs = new BlockInfo(109, "Stone Brick Stairs").SetOpacity(0).SetLightTransmission(false).SetDataLimits(0, 3, 0x4);
        Mycelium = new BlockInfo(110, "Mycelium").SetTick(10);
        LillyPad = new BlockInfo(111, "Lilly Pad").SetOpacity(0).SetState(BlockState.NONSOLID);
        NetherBrick = new BlockInfo(112, "Nether Brick");
        NetherBrickFence = new BlockInfo(113, "Nether Brick Fence").SetOpacity(0);
        NetherBrickStairs = new BlockInfo(114, "Nether Brick Stairs").SetOpacity(0).SetLightTransmission(false).SetDataLimits(0, 3, 0x4);
        NetherWart = new BlockInfo(115, "Nether Wart").SetOpacity(0).SetState(BlockState.NONSOLID).SetTick(10).SetDataLimits(0, 3, 0);
        EnchantmentTable = (BlockInfoEx) new BlockInfoEx(116, "Enchantment Table").SetOpacity(0);
        BrewingStand = (BlockInfoEx) new BlockInfoEx(117, "Brewing Stand").SetOpacity(0).SetDataLimits(0, 0, 0x7);
        Cauldron = new BlockInfo(118, "Cauldron").SetOpacity(0).SetDataLimits(0, 3, 0);
        EndPortal = (BlockInfoEx) new BlockInfoEx(119, "End Portal").SetOpacity(0).SetLuminance(MAX_LUMINANCE).SetState(BlockState.NONSOLID).SetDataLimits(0, 3, 0x4);
        EndPortalFrame = new BlockInfo(120, "End Portal Frame").SetLuminance(MAX_LUMINANCE).SetDataLimits(0, 0, 0x7);
        EndStone = new BlockInfo(121, "End Stone");
        DragonEgg = new BlockInfo(122, "Dragon Egg").SetOpacity(0).SetLuminance(1).SetTick(3);
        RedstoneLampOff = new BlockInfo(123, "Redstone Lamp (Off)").SetTick(2);
        RedstoneLampOn = new BlockInfo(124, "Redstone Lamp (On)").SetLuminance(15).SetTick(2);
        DoubleWoodSlab = new BlockInfo(125, "Double Wood Slab").SetDataLimits(0, 5, 0);
        WoodSlab = new BlockInfo(126, "Wood Slab").SetLightTransmission(false).SetDataLimits(0, 5, 0x8);
        CocoaPlant = new BlockInfo(127, "Cocoa Plant").SetLuminance(2).SetOpacity(0).SetDataLimits(0, 3, 0xC);
        SandstoneStairs = new BlockInfo(128, "Sandstone Stairs").SetOpacity(0).SetLightTransmission(false).SetDataLimits(0, 3, 0x4);
        EmeraldOre = new BlockInfo(129, "Emerald Ore");
        EnderChest = (BlockInfoEx) new BlockInfoEx(130, "Ender Chest").SetLuminance(7).SetOpacity(0).SetDataLimits(2, 5, 0);
        TripwireHook = new BlockInfo(131, "Tripwire Hook").SetOpacity(0).SetState(BlockState.NONSOLID).SetDataLimits(0, 3, 0xC);
        Tripwire = new BlockInfo(132, "Tripwire").SetOpacity(0).SetState(BlockState.NONSOLID).SetDataLimits(0, 0, 0xF);
        EmeraldBlock = new BlockInfo(133, "Emerald Block");
        SpruceWoodStairs = new BlockInfo(134, "Sprice Wood Stairs").SetOpacity(0).SetLightTransmission(false).SetDataLimits(0, 3, 0x4);
        BirchWoodStairs = new BlockInfo(135, "Birch Wood Stairs").SetOpacity(0).SetLightTransmission(false).SetDataLimits(0, 3, 0x4);
        JungleWoodStairs = new BlockInfo(136, "Jungle Wood Stairs").SetOpacity(0).SetLightTransmission(false).SetDataLimits(0, 3, 0x4);
        CommandBlock = (BlockInfoEx) new BlockInfoEx(137, "Command Block").SetDataLimits(0, 13, 0);
        BeaconBlock = (BlockInfoEx) new BlockInfoEx(138, "Beacon Block").SetOpacity(0).SetLuminance(MAX_LUMINANCE);
        CobblestoneWall = new BlockInfo(139, "Cobblestone Wall").SetOpacity(0).SetDataLimits(0, 1, 0);
        FlowerPot = (BlockInfoEx) new BlockInfoEx(140, "Flower Pot").SetOpacity(0).SetDataLimits(0, 15, 0); // VERIFYME data values
        Carrots = new BlockInfo(141, "Carrots").SetOpacity(0).SetState(BlockState.NONSOLID).SetTick(10).SetDataLimits(0, 7, 0);
        Potatoes = new BlockInfo(142, "Potatoes").SetOpacity(0).SetState(BlockState.NONSOLID).SetTick(10).SetDataLimits(0, 7, 0);
        WoodButton = new BlockInfo(143, "Wooden Button").SetOpacity(0).SetState(BlockState.NONSOLID).SetDataLimits(0, 5, 0x8);
        Heads = (BlockInfoEx) new BlockInfoEx(144, "Heads").SetOpacity(0).SetDataLimits(0, 5, 0x8);  // VERIFYME data values
        Anvil = new BlockInfo(145, "Anvil").SetOpacity(0).SetDataLimits(0, 3, 0xC);
        TrappedChest = (BlockInfoEx) new BlockInfoEx(146, "Trapped Chest").SetOpacity(0).SetTick(10).SetDataLimits(2, 5, 0);
        WeightedPressurePlateLight = new BlockInfo(147, "Weighted Pressure Plate (Light)").SetOpacity(0).SetState(BlockState.NONSOLID).SetTick(20).SetDataLimits(0, 15, 0);
        WeightedPressurePlateHeavy = new BlockInfo(148, "Weighted Pressure Plate (Heavy)").SetOpacity(0).SetState(BlockState.NONSOLID).SetTick(20).SetDataLimits(0, 15, 0);
        RedstoneComparatorInactive = new BlockInfo(149, "Redstone Comparator (Inactive)").SetOpacity(0).SetTick(10).SetDataLimits(0, 3, 0xC);
        RedstoneComparatorActive = new BlockInfo(150, "Redstone Comparator (Active)").SetOpacity(0).SetLuminance(9).SetTick(10).SetDataLimits(0, 3, 0xC);
        DaylightSensor = (BlockInfoEx) new BlockInfoEx(151, "Daylight Sensor").SetOpacity(0).SetTick(10).SetDataLimits(0, 15, 0);
        RedstoneBlock = new BlockInfo(152, "Block of Redstone").SetTick(10);
        NetherQuartzOre = new BlockInfo(153, "Neither Quartz Ore");
        Hopper = (BlockInfoEx) new BlockInfoEx(154, "Hopper").SetOpacity(0).SetTick(10).SetDataLimits(0, 5, 0x8);
        QuartzBlock = new BlockInfo(155, "Block of Quartz").SetDataLimits(0, 4, 0);
        QuartzStairs = new BlockInfo(156, "Quartz Stairs").SetOpacity(0).SetLightTransmission(false).SetDataLimits(0, 3, 0x4);
        ActivatorRail = new BlockInfo(157, "Activator Rail").SetOpacity(0).SetState(BlockState.NONSOLID).SetTick(10).SetDataLimits(0, 5, 0x8);
        Dropper = (BlockInfoEx) new BlockInfoEx(158, "Dropper").SetTick(10).SetDataLimits(0, 5, 0x8);
        StainedClay = new BlockInfo(159, "Stained Clay").SetDataLimits(0, 15, 0);
        StainedGlassPane = new BlockInfo(160, "Stained Glass Pane").SetOpacity(0).SetDataLimits(0, 15, 0);

        // TODO fill in details
        Leaves2 = new BlockInfo(161, "Leaves (Acacia/Dark Oak)").SetDataLimits(0, 1, 0xC);
        Wood2 = new BlockInfo(162, "Wood (Acacia/Dark Oak)").SetDataLimits(0, 13, 0);
        AcaciaWoodStairs = new BlockInfo(163, "Acacia Wood Stairs").SetDataLimits(0, 3, 0x4);
        DarkOakWoodStairs = new BlockInfo(164, "Dark Oak Wood Stairs").SetDataLimits(0, 3, 0x4);
        SlimeBlock = new BlockInfo(165, "Slime Block");
        Barrier = new BlockInfo(166, "Barrier");
        IronTrapdoor = new BlockInfo(167, "Iron Trapdoor").SetDataLimits(0, 15, 0);
        Prismarine = new BlockInfo(168, "Prismarine").SetDataLimits(0, 2, 0);
        SeaLantern = new BlockInfo(169, "Sea Lantern");

        HayBlock = new BlockInfo(170, "Hay Block").SetDataLimits(0, 8, 0);
        Carpet = new BlockInfo(171, "Carpet").SetOpacity(0).SetLightTransmission(false).SetDataLimits(0, 15, 0);
        HardenedClay = new BlockInfo(172, "Hardened Clay");
        CoalBlock = new BlockInfo(173, "Block of Coal").SetDataLimits(0, 1, 0);

        // TODO fill in details
        PackedIce = new BlockInfo(174, "Packed Ice");
        LargeFlowers = new BlockInfo(175, "Large Flowers").SetDataLimits(0, 5, 0x8);
        StandingBanner = (BlockInfoEx) new BlockInfoEx(176, "Standing Banner").SetDataLimits(0, 15, 0);
        WallBanner = (BlockInfoEx) new BlockInfoEx(177, "Wall Banner").SetDataLimits(2, 5, 0);
        InvertedDaylightSensor = (BlockInfoEx) new BlockInfoEx(178, "Inverted Daylight Sensor").SetDataLimits(0, 15, 0);
        RedSandstone = new BlockInfo(179, "Red Sandstone").SetDataLimits(0, 2, 0);
        RedSandstoneStairs = new BlockInfo(180, "Red Sandstone Stairs").SetDataLimits(0, 3, 0x4);
        DoubleRedSandstoneSlab = new BlockInfo(181, "Double Red Sandstone Slab").SetDataLimits(0, 0, 0x8);
        RedSandstoneSlab = new BlockInfo(182, "Red Sandstone Slab").SetDataLimits(0, 0, 0x8);
        SpruceFenceGate = new BlockInfo(183, "Spruce Fence Gate").SetDataLimits(0, 3, 0xC); // VERIFYME data values
        BirchFenceGate = new BlockInfo(184, "Birch Fence Gate").SetDataLimits(0, 3, 0xC); // VERIFYME data values
        JungleFenceGate = new BlockInfo(185, "Jungle Fence Gate").SetDataLimits(0, 3, 0xC); // VERIFYME data values
        DarkOakFenceGate = new BlockInfo(186, "Dark Oak Fence Gate").SetDataLimits(0, 3, 0xC); // VERIFYME data values
        AcaciaFenceGate = new BlockInfo(187, "Acacia Fence Gate").SetDataLimits(0, 3, 0xC); // VERIFYME data values
        SpruceFence = new BlockInfo(188, "Spruce Fence");
        BirchFence = new BlockInfo(189, "Birch Fence");
        JungleFence = new BlockInfo(190, "Jungle Fence");
        DarkOakFence = new BlockInfo(191, "Dark Oak Fence");
        AcaciaFence = new BlockInfo(192, "Acacia Fence");
        SpruceDoor = new BlockInfo(193, "Spruce Door").SetDataLimits(0, 0, 0xF);
        BirchDoor = new BlockInfo(194, "Birch Door").SetDataLimits(0, 0, 0xF);
        JungleDoor = new BlockInfo(195, "Jungle Door").SetDataLimits(0, 0, 0xF);
        AcaciaDoor = new BlockInfo(196, "Acacia Door").SetDataLimits(0, 0, 0xF);
        DarkOakDoor = new BlockInfo(197, "Dark Oak Door").SetDataLimits(0, 0, 0xF);

        // TODO details
        EndRod = new BlockInfo(198, "End Rod").SetDataLimits(0, 5, 0);
        ChorusPlant = new BlockInfo(199, "Chorus Plant");
        ChorusFlower = new BlockInfo(200, "Chorus Flower").SetDataLimits(0, 5, 0);
        PurpurBlock = new BlockInfo(201, "Purpur Block");
        PurpurPillar = new BlockInfo(202, "Purpur Pillar").SetDataLimits(0, 8, 0);
        PurpurStairs = new BlockInfo(203, "Purpur Stairs").SetDataLimits(0, 7, 0);
        PurpurDoubleSlab = new BlockInfo(204, "Purpur Double Slab");
        PurpurSlab = new BlockInfo(205, "Purpur Slab").SetDataLimits(0, 8, 0);
        EndStoneBricks = new BlockInfo(206, "End Stone Bricks");
        BeetrootSeeds = new BlockInfo(207, "Beetroot Seeds").SetDataLimits(0, 3, 0);
        GrassPath = new BlockInfo(208, "Grass Path");
        EndGateway = (BlockInfoEx) new BlockInfoEx(209, "End Gateway");

        RepeatingCommandBlock = (BlockInfoEx) new BlockInfoEx(210, "Repeating Command Block").SetDataLimits(0, 13, 0);
        ChainCommandBlock = (BlockInfoEx) new BlockInfoEx(211, "Chain Command Block").SetDataLimits(0, 13, 0);
        FrostedIce = new BlockInfo(212, "Frosted Ice").SetDataLimits(0, 3, 0);
        MagmaBlock = new BlockInfo(213, "Magma Block");
        NetherWartBlock = new BlockInfo(214, "Nether Wart Block");
        RedNetherBrick = new BlockInfo(215, "Red Nether Brick");
        BoneBlock = new BlockInfo(216, "Bone Block").SetDataLimits(0, 8, 0);
        StructureVoid = new BlockInfo(217, "Structure Void");
        Observer = new BlockInfo(218, "Observer").SetDataLimits(0, 13, 0);
        WhiteShulkerBox = (BlockInfoEx) new BlockInfoEx(219, "White Shulker Box").SetDataLimits(0, 5, 0);

        OrangeShulkerBox = (BlockInfoEx) new BlockInfoEx(220, "Orange Shulker Box").SetDataLimits(0, 5, 0);
        MagentaShulkerBox = (BlockInfoEx) new BlockInfoEx(221, "Magenta Shulker Box").SetDataLimits(0, 5, 0);
        LightBlueShulkerBox = (BlockInfoEx) new BlockInfoEx(222, "Light Blue Shulker Box").SetDataLimits(0, 5, 0);
        YellowShulkerBox = (BlockInfoEx) new BlockInfoEx(223, "Yellow Shulker Box").SetDataLimits(0, 5, 0);
        LimeShulkerBox = (BlockInfoEx) new BlockInfoEx(224, "Lime Shulker Box").SetDataLimits(0, 5, 0);
        PinkShulkerBox = (BlockInfoEx) new BlockInfoEx(225, "Pink Shulker Box").SetDataLimits(0, 5, 0);
        GrayShulkerBox = (BlockInfoEx) new BlockInfoEx(226, "Gray Shulker Box").SetDataLimits(0, 5, 0);
        LightGrayShulkerBox = (BlockInfoEx) new BlockInfoEx(227, "Light Gray Shulker Box").SetDataLimits(0, 5, 0);
        CyanShulkerBox = (BlockInfoEx) new BlockInfoEx(228, "Cyan Shulker Box").SetDataLimits(0, 5, 0);
        PurpleShulkerBox = (BlockInfoEx) new BlockInfoEx(229, "Purple Shulker Box").SetDataLimits(0, 5, 0);

        BlueShulkerBox = (BlockInfoEx) new BlockInfoEx(230, "Blue Shulker Box").SetDataLimits(0, 5, 0);
        BrownShulkerBox = (BlockInfoEx) new BlockInfoEx(231, "Brown Shulker Box").SetDataLimits(0, 5, 0);
        GreenShulkerBox = (BlockInfoEx) new BlockInfoEx(232, "Green Shulker Box").SetDataLimits(0, 5, 0);
        RedShulkerBox = (BlockInfoEx) new BlockInfoEx(233, "Red Shulker Box").SetDataLimits(0, 5, 0);
        BlackShulkerBox = (BlockInfoEx) new BlockInfoEx(234, "Black Shulker Box").SetDataLimits(0, 5, 0);
        WhiteGlazedTerracotta = new BlockInfo(235, "White Glazed Terracotta").SetDataLimits(0, 3, 0);
        OrangeGlazedTerracotta = new BlockInfo(236, "Orange Glazed Terracotta").SetDataLimits(0, 3, 0);
        MagentaGlazedTerracotta = new BlockInfo(237, "Magenta Glazed Terracotta").SetDataLimits(0, 3, 0);
        LightBlueGlazedTerracotta = new BlockInfo(238, "Light Blue Glazed Terracotta").SetDataLimits(0, 3, 0);
        YellowGlazedTerracotta = new BlockInfo(239, "Yellow Glazed Terracotta").SetDataLimits(0, 3, 0);

        LimeGlazedTerracotta = new BlockInfo(240, "Lime Glazed Terracotta").SetDataLimits(0, 3, 0);
        PinkGlazedTerracotta = new BlockInfo(241, "Pink Glazed Terracotta").SetDataLimits(0, 3, 0);
        GrayGlazedTerracotta = new BlockInfo(242, "Gray Glazed Terracotta").SetDataLimits(0, 3, 0);
        LightGrayGlazedTerracotta = new BlockInfo(243, "Light Gray Glazed Terracotta").SetDataLimits(0, 3, 0);
        CyanGlazedTerracotta = new BlockInfo(244, "Cyan Glazed Terracotta").SetDataLimits(0, 3, 0);
        PurpleGlazedTerracotta = new BlockInfo(245, "Purple Glazed Terracotta").SetDataLimits(0, 3, 0);
        BlueGlazedTerracotta = new BlockInfo(246, "Blue Glazed Terracotta").SetDataLimits(0, 3, 0);
        BrownGlazedTerracotta = new BlockInfo(247, "Brown Glazed Terracotta").SetDataLimits(0, 3, 0);
        GreenGlazedTerracotta = new BlockInfo(248, "Green Glazed Terracotta").SetDataLimits(0, 3, 0);
        RedGlazedTerracotta = new BlockInfo(249, "Red Glazed Terracotta").SetDataLimits(0, 3, 0);

        BlackGlazedTerracotta = new BlockInfo(250, "Black Glazed Terracotta").SetDataLimits(0, 3, 0);
        Concrete = new BlockInfo(251, "Concrete").SetDataLimits(0, 15, 0);
        ConcretePowder = new BlockInfo(252, "Concrete Powder").SetDataLimits(0, 15, 0);
        StructureBlock = (BlockInfoEx) new BlockInfoEx(255, "Structure Block").SetDataLimits(0, 3, 0);

        for(int i = 0; i < MAX_BLOCKS; i++) {
            if(blockTable[i] == null) {
                blockTable[i] = new BlockInfo(i);
            }
        }

        // Override default light transmission rules

        /*Lava.SetLightTransmission(false);
        StationaryLava.SetLightTransmission(false);
        StoneSlab.SetLightTransmission(false);
        WoodStairs.SetLightTransmission(false);
        Farmland.SetLightTransmission(false);
        CobbleStairs.SetLightTransmission(false);
        BrickStairs.SetLightTransmission(false);
        StoneBrickStairs.SetLightTransmission(false);
        NetherBrickStairs.SetLightTransmission(false);
        WoodSlab.SetLightTransmission(false);
        SandstoneStairs.SetLightTransmission(false);
        SpruceWoodStairs.SetLightTransmission(false);
        BirchWoodStairs.SetLightTransmission(false);
        JungleWoodStairs.SetLightTransmission(false);
        AcaciaWoodStairs.SetLightTransmission(false);
        DarkOakWoodStairs.SetLightTransmission(false);
        QuartzStairs.SetLightTransmission(false);
        Carpet.SetLightTransmission(false);
        LargeFlowers.SetLightTransmission(false);

        // Override default fluid blocking rules

        SignPost.SetBlocksFluid(true);
        WallSign.SetBlocksFluid(true);
        Cactus.SetBlocksFluid(false);

        // Set Tile Entity Data

        Dispenser.SetTileEntity("Trap");
        NoteBlock.SetTileEntity("Music");
        PistonMoving.SetTileEntity("Piston");
        MonsterSpawner.SetTileEntity("MobSpawner");
        Chest.SetTileEntity("Chest");
        Furnace.SetTileEntity("Furnace");
        BurningFurnace.SetTileEntity("Furnace");
        SignPost.SetTileEntity("Sign");
        WallSign.SetTileEntity("Sign");
        EnchantmentTable.SetTileEntity("EnchantTable");
        BrewingStand.SetTileEntity("Cauldron");
        EndPortal.SetTileEntity("Airportal");
        EnderChest.SetTileEntity("EnderChest");
        CommandBlock.SetTileEntity("Control");
        BeaconBlock.SetTileEntity("Beacon");
        TrappedChest.SetTileEntity("Chest");
        Hopper.SetTileEntity("Hopper");
        Dropper.SetTileEntity("Dropper");

        // Set Data Limits

        Wood.SetDataLimits(0, 15, 0);
        Leaves.SetDataLimits(0, 15, 0);
        Jukebox.SetDataLimits(0, 2, 0);
        Sapling.SetDataLimits(0, 15, 0);
        Cactus.SetDataLimits(0, 15, 0);
        SugarCane.SetDataLimits(0, 15, 0);
        Water.SetDataLimits(0, 7, 0x8);
        Lava.SetDataLimits(0, 7, 0x8);
        TallGrass.SetDataLimits(0, 2, 0);
        Crops.SetDataLimits(0, 7, 0);
        PoweredRail.SetDataLimits(0, 5, 0x8);
        DetectorRail.SetDataLimits(0, 5, 0x8);
        StickyPiston.SetDataLimits(1, 5, 0x8);
        Piston.SetDataLimits(1, 5, 0x8);
        PistonHead.SetDataLimits(1, 5, 0x8);
        Wool.SetDataLimits(0, 15, 0);
        Torch.SetDataLimits(1, 5, 0);
        RedstoneTorch.SetDataLimits(0, 5, 0);
        RedstoneTorchOn.SetDataLimits(0, 5, 0);
        Rails.SetDataLimits(0, 9, 0);
        Ladder.SetDataLimits(2, 5, 0);
        WoodStairs.SetDataLimits(0, 3, 0x4);
        CobbleStairs.SetDataLimits(0, 3, 0x4);
        Lever.SetDataLimits(0, 6, 0x8);
        WoodDoor.SetDataLimits(0, 3, 0xC);
        IronDoor.SetDataLimits(0, 3, 0xC);
        StoneButton.SetDataLimits(1, 4, 0x8);
        Snow.SetDataLimits(0, 7, 0);
        SignPost.SetDataLimits(0, 15, 0);
        WallSign.SetDataLimits(2, 5, 0);
        Furnace.SetDataLimits(2, 5, 0);
        BurningFurnace.SetDataLimits(2, 5, 0);
        Dispenser.SetDataLimits(0, 11, 0);
        Pumpkin.SetDataLimits(0, 3, 0);
        JackOLantern.SetDataLimits(0, 3, 0);
        StonePlate.SetDataLimits(0, 0, 0x1);
        WoodPlate.SetDataLimits(0, 0, 0x1);
        StoneSlab.SetDataLimits(0, 5, 0);
        DoubleStoneSlab.SetDataLimits(0, 5, 0x8);
        Cactus.SetDataLimits(0, 5, 0);
        Bed.SetDataLimits(0, 3, 0x8);
        RedstoneRepeater.SetDataLimits(0, 0, 0xF);
        RedstoneRepeaterOn.SetDataLimits(0, 0, 0xF);
        Trapdoor.SetDataLimits(0, 3, 0x4);
        StoneBrick.SetDataLimits(0, 2, 0);
        HugeRedMushroom.SetDataLimits(0, 10, 0);
        HugeBrownMushroom.SetDataLimits(0, 10, 0);
        Vines.SetDataLimits(0, 0, 0xF);
        FenceGate.SetDataLimits(0, 3, 0x4);
        SilverfishStone.SetDataLimits(0, 2, 0);
        BrewingStand.SetDataLimits(0, 0, 0x7);
        Cauldron.SetDataLimits(0, 3, 0);
        EndPortalFrame.SetDataLimits(0, 0, 0x7);
        WoodSlab.SetDataLimits(0, 5, 0);
        DoubleWoodSlab.SetDataLimits(0, 5, 0x8);
        TripwireHook.SetDataLimits(0, 3, 0xC);
        Tripwire.SetDataLimits(0, 0, 0x5);
        Anvil.SetDataLimits(0, 0, 0xD);
        QuartzBlock.SetDataLimits(0, 4, 0);
        QuartzStairs.SetDataLimits(0, 3, 0x4);
        Carpet.SetDataLimits(0, 15, 0);
        Dropper.SetDataLimits(0, 5, 0);
        Hopper.SetDataLimits(0, 5, 0);
        Leaves2.SetDataLimits(0, 13, 0);
        Wood2.SetDataLimits(0, 7, 0);
        LargeFlowers.SetDataLimits(0, 47, 0);*/
    }
}

/// <summary>
/// An extended <see cref="BlockInfo"/> that includes <see cref="TileEntity"/> information.
/// </summary>
public class BlockInfoEx : BlockInfo {
    private string tileEntityName;

    /// <summary>
    /// Gets the name of the <see cref="TileEntity"/> type associated with this block type.
    /// </summary>
    public string TileEntityName {
        get { return this.tileEntityName; }
    }

    internal BlockInfoEx(int id) : base(id) { }

    /// <summary>
    /// Constructs a new <see cref="BlockInfoEx"/> with a given block id and name.
    /// </summary>
    /// <param name="id">The id of the block type.</param>
    /// <param name="name">The name of the block type.</param>
    public BlockInfoEx(int id, string name) : base(id, name) { }

    /// <summary>
    /// Sets the name of the <see cref="TileEntity"/> type associated with this block type.
    /// </summary>
    /// <param name="name">The name of a registered <see cref="TileEntity"/> type.</param>
    /// <returns>The object instance used to invoke this method.</returns>
    /// <seealso cref="TileEntityFactory"/>
    public BlockInfo SetTileEntity(string name) {
        this.tileEntityName = name;
        return this;
    }
}
