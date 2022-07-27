namespace Substrate;

using Substrate.Core;

public class AnvilBiomeCollection {
    public const int OCEAN = 0;
    public const int PLAINS = 1;
    public const int DESERT = 2;
    public const int EXTREME_HILLS = 3;
    public const int FOREST = 4;
    public const int TAIGA = 5;
    public const int SWAMPLAND = 6;
    public const int RIVER = 7;
    public const int HELL = 8;
    public const int SKY = 9;
    public const int FROZEN_OCEAN = 10;
    public const int FROZEN_RIVER = 11;
    public const int ICE_PLAINS = 12;
    public const int ICE_MOUNTAINS = 13;
    public const int MUSHROOM_ISLAND = 14;
    public const int MUSHROOM_ISLAND_SHORE = 15;
    public const int BEACH = 16;
    public const int DESERT_HILLS = 17;
    public const int FOREST_HILLS = 18;
    public const int TAIGA_HILLS = 19;
    public const int EXTREME_HILLS_EDGE = 20;
    public const int JUNGLE = 21;
    public const int JUNGLE_HILLS = 22;
    public const int JUNGLE_EDGE = 23;
    public const int DEEP_OCEAN = 24;
    public const int STONE_BEACH = 25;
    public const int COLD_BEACH = 26;
    public const int BIRCH_FOREST = 27;
    public const int BIRCH_FOREST_HILLS = 28;
    public const int ROOFED_FOREST = 29;
    public const int COLD_TAIGA = 30;
    public const int COLD_TAIGA_HILLS = 31;
    public const int MEGA_TAIGA = 32;
    public const int MEGA_TAIGA_HILLS = 33;
    public const int EXTREME_HILLS_PLUS = 34;
    public const int SAVANNA = 35;
    public const int SAVANNA_PLATEAU = 36;
    public const int MESA = 37;
    public const int MESA_PLATEAU_F = 38;
    public const int MESA_PLATEAU = 39;
    public const int THE_VOID = 127;
    public const int SUNFLOWER_PLAINS = 129;
    public const int DESERT_M = 130;
    public const int EXTREME_HILLS_M = 131;
    public const int FLOWER_FOREST = 132;
    public const int TAIGA_M = 133;
    public const int SWAMPLAND_M = 134;
    public const int ICE_SPIKES = 140;
    public const int JUNGLE_M = 149;
    public const int JUNGLE_EDGE_M = 151;
    public const int BIRCH_FOREST_M = 155;
    public const int BIRCH_FOREST_HILLS_M = 156;
    public const int ROOFED_FOREST_M = 157;
    public const int COLD_TAIGA_M = 158;
    public const int MEGA_SPRUCE_TAIGA = 160;
    public const int TAIGA_HILLS_M = 161;
    public const int EXTREME_HILLS_PLUS_M = 162;
    public const int SAVANNA_M = 163;
    public const int SAVANNA_PLATEAU_M = 164;
    public const int MESA_BRYCE = 165;
    public const int MESA_PLATEAU_F_M = 166;
    public const int MESA_PLATEAU_M = 167;

    private readonly int xdim;
    private readonly int zdim;

    private IDataArray2 biomeMap;

    public AnvilBiomeCollection(IDataArray2 biomeMap) {
        this.biomeMap = biomeMap;

        this.xdim = this.biomeMap.XDim;
        this.zdim = this.biomeMap.ZDim;
    }

    public int GetBiome(int x, int z) {
        return this.biomeMap[x, z];
    }

    public void SetBiome(int x, int z, int newBiome) {
        this.biomeMap[x, z] = newBiome;
    }
}
