namespace Substrate;

// Block Data
public enum WoodType {
    OAK = 0,
    SPRUCE = 1,
    BIRCH = 2,
    JUNGLE = 3,
}

public enum LeafType {
    OAK = 0,
    SPRUCE = 1,
    BIRCH = 2,
    JUNGLE = 3,
}

[Flags]
public enum LeafState {
    PERMANENT = 0x4,
    DECAY_CHECK = 0x8,
}

public enum SaplingType {
    OAK = 0,
    SPRUCE = 1,
    BIRCH = 2,
    JUNGLE = 3,
    ACACIA = 4,
    DARK_OAK = 5,
}

public enum WaterFlow {
    SOURCE = 0,
    FLOWING_LEVEL_7 = 1,
    FLOWING_LEVEL_6 = 2,
    FLOWING_LEVEL_5 = 3,
    FLOWING_LEVEL_4 = 4,
    FLOWING_LEVEL_3 = 5,
    FLOWING_LEVEL_2 = 6,
    FLOWING_LEVEL_1 = 7,
}

public enum LavaFlow {
    SOURCE = 0,
    FLOWING_LEVEL_7 = 1,
    FLOWING_LEVEL_6 = 2,
    FLOWING_LEVEL_5 = 3,
    FLOWING_LEVEL_4 = 4,
    FLOWING_LEVEL_3 = 5,
    FLOWING_LEVEL_2 = 6,
    FLOWING_LEVEL_1 = 7,
}

[Flags]
public enum LiquidState {
    FALLING = 0x08,
}

public enum DoorFacing {
    EAST = 0,
    SOUTH = 1,
    WEST = 2,
    NORTH = 3,
}

[Flags]
public enum DoorState {
    OPEN = 4,
    TOPHALF = 8,
}

public enum WoolColor {
    WHITE = 0,
    ORANGE = 1,
    MAGENTA = 2,
    LIGHT_BLUE = 3,
    YELLOW = 4,
    LIGHT_GREEN = 5,
    PINK = 6,
    GRAY = 7,
    LIGHT_GRAY = 8,
    CYAN = 9,
    PURPLE = 10,
    BLUE = 11,
    BROWN = 12,
    DARK_GREEN = 13,
    RED = 14,
    BLACK = 15
}

public enum TorchOrientation {
    EAST = 1,
    WEST = 2,
    SOUTH = 3,
    NORTH = 4,
    FLOOR = 5,
}

public enum RailOrientation {
    NORTHSOUTH = 0,
    EASTWEST = 1,
    ASCEND_EAST = 2,
    ASCEND_WEST = 3,
    ASCEND_NORTH = 4,
    ASCEND_SOUTH = 5,
    SOUTHEAST = 6,
    SOUTHWEST = 7,
    NORTHWEST = 8,
    NORTHEAST = 9
}

public enum PoweredRailOrientation {
    NORTH_SOUTH = 0,
    EAST_WEST = 1,
    ASCEND_EAST = 2,
    ASCEND_WEST = 3,
    ASCEND_NORTH = 4,
    ASCEND_SOUTH = 5,
}

[Flags]
public enum PoweredRailState {
    POWERED = 8,
}

public enum LadderOrientation {
    NORTH = 2,
    SOUTH = 3,
    WEST = 4,
    EAST = 5,
}

public enum StairOrientation {
    ASCEND_EAST = 0,
    ASCEND_WEST = 1,
    ASCEND_SOUTH = 2,
    ASCEND_NORTH = 3,
}

[Flags]
public enum StairInversion {
    Inverted = 4,
}

public enum LeverOrientation {
    CEILING_EASTWEST = 0,
    WALL_EAST = 1,
    WALL_WEST = 2,
    WALL_SOUTH = 3,
    WALL_NORTH = 4,
    GROUND_NORTHSOUTH = 5,
    GROUND_EASTWEST = 6,
    CEILING_NORTHSOUTH = 7,
}

[Flags]
public enum LeverState {
    POWERED = 8,
}

public enum ButtonOrientation {
    CEILING = 0,
    EAST = 1,
    WEST = 2,
    SOUTH = 3,
    NORTH = 4,
    GROUND = 5,
}

[Flags]
public enum ButtonState {
    PRESSED = 8,
}

public enum SignPostOrientation {
    SOUTH = 0,
    SOUTH_SOUTHWEST = 1,
    SOUTHWEST = 2,
    WEST_SOUTHWEST = 3,
    WEST = 4,
    WEST_NORTHWEST = 5,
    NORTHWEST = 6,
    NORTH_NORTHWEST = 7,
    NORTH = 8,
    NORTH_NORTHEAST = 9,
    NORTHEAST = 10,
    EAST_NORTHEAST = 11,
    EAST = 12,
    EAST_SOUTHEAST = 13,
    SOUTHEAST = 14,
    SOUTH_SOUTHEAST = 15,
}

public enum WallSignOrientation {
    NORTH = 2,
    SOUTH = 3,
    WEST = 4,
    EAST = 5,
}

public enum FurnaceOrientation {
    NORTH = 2,
    SOUTH = 3,
    WEST = 4,
    EAST = 5,
}

public enum PumpkinOrientation {
    SOUTH = 0,
    WEST = 1,
    NORTH = 2,
    EAST = 3,
}

[Flags]
public enum PressurePlateState {
    PRESSED = 1,
}

public enum SlabType {
    STONE = 0,
    SANDSTONE = 1,
    WOOD = 2,
    COBBLESTONE = 3,
    BRICK = 4,
    STONE_BRICK = 5,
    NETHER_BRICK = 6,
    QUARTZ = 7,
}

[Flags]
public enum SlabInversion {
    Inverted = 8,
}

public enum BedOrientation {
    SOUTH = 0,
    WEST = 1,
    NORTH = 2,
    EAST = 3,
}

[Flags]
public enum BedState {
    HEAD = 8,
}

public enum CakeState {
    BITES_0 = 0,
    BITES_1 = 1,
    BITES_2 = 2,
    BITES_3 = 3,
    BITES_4 = 4,
    BITES_5 = 5,
    BITES_6 = 6,
}

public enum RepeaterOrientation {
    SOUTH = 0,
    WEST = 1,
    NORTH = 2,
    EAST = 3,
}

public enum RepeaterDelay {
    DELAY_1_TICK = 0,
    DELAY_2_TICK = 4,
    DELAY_3_TICK = 8,
    DELAY_4_TICK = 12,
}

public enum TallGrassType {
    DEAD_SHRUB = 0,
    TALL_GRASS = 1,
    FERN = 2,
}

public enum TrapdoorOrientation {
    NORTH = 0,
    SOUTH = 1,
    WEST = 2,
    EAST = 3,
}

[Flags]
public enum TrapdoorState {
    OPEN = 4,
}

public enum PistonOrientation {
    DOWN = 0,
    UP = 1,
    NORTH = 2,
    SOUTH = 3,
    WEST = 4,
    EAST = 5,
}

[Flags]
public enum PistonBodyState {
    EXTENDED = 8,
}

[Flags]
public enum PistonHeadState {
    STICKY = 8,
}

public enum StoneBrickType {
    NORMAL = 0,
    MOSSY = 1,
    CRACKED = 2,
    CHISELED = 3,
}

public enum HugeMushroomType {
    ALL_INSIDE = 0,
    NORTH_WEST = 1,
    NORTH = 2,
    NORTH_EAST = 3,
    WEST = 4,
    TOP = 5,
    EAST = 6,
    SOUTH_WEST = 7,
    SOUTH = 8,
    SOUTH_EAST = 9,
    STEM = 10,
}

[Flags]
public enum VineCoverageState {
    SOUTH = 1,
    WEST = 2,
    NORTH = 4,
    EAST = 8,
}

public enum FenceGateOrientation {
    SOUTH = 0,
    WEST = 1,
    NORTH = 2,
    EAST = 3,
}

[Flags]
public enum FenceGateState {
    OPEN = 4,
}

public enum SilverfishBlockType {
    STONE = 0,
    COBBLESTONE = 1,
    STONE_BRICK = 2,
    MOSSY_STONE_BRICK = 3,
    CRACKED_STONE_BRICK = 4,
    CHISELED_STONE_BRICK = 5,
}

[Flags]
public enum BrewingStandState {
    NONE = 0,
    SLOT_EAST = 1,
    SLOT_NORTHWEST = 2,
    SLOT_SOUTHWEST = 4,
}

public enum CauldronLevel {
    EMPTY = 0,
    LEVEL_1 = 1,
    LEVEL_2 = 2,
    FULL = 3,
}

[Flags]
public enum EndPortalState {
    EYE_OF_ENDER = 4,
}

public enum CocoaPlantDirection {
    SOUTH = 0,
    WEST = 1,
    NORTH = 2,
    EAST = 3,
}

public enum CocoaPlantSize {
    SMALL = 0,
    MEDIUM = 4,
    LARGE = 8,
}

public enum TripwireHookDirection {
    SOUTH = 0,
    WEST = 1,
    NORTH = 2,
    EAST = 3,
}

[Flags]
public enum TripwireHookState {
    ATTACHED = 4,
    NOT_ATTACHED = 8,
}

[Flags]
public enum TripwireState {
    POWERED = 1,
    ATTACHED = 4,
    DISARMED = 8,
}

public enum CobblestoneWallType {
    COBBLESTONE = 0,
    MOSSY_COBBLESTONE = 1,
}

public enum FlowerPotType {
    EMPTY = 0,
    ROSE = 1,
    DANDELION = 2,
    OAK_SAPLING = 3,
    SPRUCE_SAPLING = 4,
    BIRCH_SAPLING = 5,
    JUNGLE_SAPLING = 6,
    RED_MUSHROOM = 7,
    BROWN_MUSHROOM = 8,
    CACTUS = 9,
    DEAD_BUSH = 10,
    FERN = 11,
}

public enum SkullType {
    SKELETON = 0,
    WITHER_SKELETON = 1,
    ZOMBIE = 2,
    PLAYER = 3,
    CREEPER = 4,
    ENDER_DRAGON = 5,
}

// Item Data
public enum CoalType {
    COAL = 0,
    CHARCOAL = 1
}

public enum DyeType {
    INK_SAC = 0,
    ROSE_RED = 1,
    CACTUS_GREEN = 2,
    COCOA_BEANS = 3,
    LAPIS_LAZULI = 4,
    PURPLE_DYE = 5,
    CYAN_DYE = 6,
    LIGHT_GRAY_DYE = 7,
    GRAY_DYE = 8,
    PINK_DYE = 9,
    LIME_DYE = 10,
    DANDELION_YELLOW = 11,
    LIGHT_BLUE_DYE = 12,
    MAGENTA_DYE = 13,
    ORANGE_DYE = 14,
    BONE_MEAL = 15
}
