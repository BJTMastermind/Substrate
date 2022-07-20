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
