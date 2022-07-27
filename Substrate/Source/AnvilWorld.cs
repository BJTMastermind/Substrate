//TODO: Exceptions (+ Alpha)
namespace Substrate;

using Substrate.Core;
using Substrate.Data;
using Substrate.Nbt;
using IO = System.IO;

/// <summary>
/// Represents an Anvil-compatible (Release 1.2 or higher) Minecraft world.
/// </summary>
public class AnvilWorld : NbtWorld {
    private const string REGION_DIR = "region";
    private const string PLAYER_DIR_OLD = "players";
    private const string PLAYER_DIR_1_7_6_PLUS = "playerdata";
    private string levelFile = "level.dat";

    private Level level;

    private Dictionary<string, AnvilRegionManager> regionMgrs;
    private Dictionary<string, RegionChunkManager> chunkMgrs;
    private Dictionary<string, BlockManager> blockMgrs;

    private Dictionary<string, ChunkCache> caches;

    private PlayerManager playerMan;
    private BetaDataManager dataMan;

    private int prefCacheSize = 256;

    private AnvilWorld() {
        this.regionMgrs = new Dictionary<string, AnvilRegionManager>();
        this.chunkMgrs = new Dictionary<string, RegionChunkManager>();
        this.blockMgrs = new Dictionary<string, BlockManager>();

        this.caches = new Dictionary<string, ChunkCache>();
    }

    /// <summary>
    /// Gets a reference to this world's <see cref="Level"/> object.
    /// </summary>
    public override Level Level {
        get { return this.level; }
    }

    /// <summary>
    /// Gets a <see cref="BlockManager"/> for the default dimension.
    /// </summary>
    /// <returns>A <see cref="BlockManager"/> tied to the default dimension in this world.</returns>
    /// <remarks>Get a <see cref="BlockManager"/> if you need to manage blocks as a global, unbounded matrix.  This abstracts away
    /// any higher-level organizational divisions.  If your task is going to be heavily performance-bound, consider getting a
    /// <see cref="RegionChunkManager"/> instead and working with blocks on a chunk-local level.</remarks>
    public new BlockManager GetBlockManager() {
        return GetBlockManagerVirt(Dimension.DEFAULT) as BlockManager;
    }

    /// <summary>
    /// Gets a <see cref="BlockManager"/> for the given dimension.
    /// </summary>
    /// <param name="dim">The id of the dimension to look up.</param>
    /// <returns>A <see cref="BlockManager"/> tied to the given dimension in this world.</returns>
    /// <remarks>Get a <see cref="BlockManager"/> if you need to manage blocks as a global, unbounded matrix.  This abstracts away
    /// any higher-level organizational divisions.  If your task is going to be heavily performance-bound, consider getting a
    /// <see cref="RegionChunkManager"/> instead and working with blocks on a chunk-local level.</remarks>
    public new BlockManager GetBlockManager(int dim) {
        return GetBlockManagerVirt(dim) as BlockManager;
    }

    public new BlockManager GetBlockManager(string dim) {
        return GetBlockManagerVirt(dim) as BlockManager;
    }

    /// <summary>
    /// Gets a <see cref="RegionChunkManager"/> for the default dimension.
    /// </summary>
    /// <returns>A <see cref="RegionChunkManager"/> tied to the default dimension in this world.</returns>
    /// <remarks>Get a <see cref="RegionChunkManager"/> if you you need to work with easily-digestible, bounded chunks of blocks.</remarks>
    public new RegionChunkManager GetChunkManager() {
        return GetChunkManagerVirt(Dimension.DEFAULT) as RegionChunkManager;
    }

    /// <summary>
    /// Gets a <see cref="RegionChunkManager"/> for the given dimension.
    /// </summary>
    /// <param name="dim">The id of the dimension to look up.</param>
    /// <returns>A <see cref="RegionChunkManager"/> tied to the given dimension in this world.</returns>
    /// <remarks>Get a <see cref="RegionChunkManager"/> if you you need to work with easily-digestible, bounded chunks of blocks.</remarks>
    public new RegionChunkManager GetChunkManager(int dim) {
        return GetChunkManagerVirt(dim) as RegionChunkManager;
    }

    public new RegionChunkManager GetChunkManager(string dim) {
        return GetChunkManagerVirt(dim) as RegionChunkManager;
    }

    /// <summary>
    /// Gets a <see cref="RegionManager"/> for the default dimension.
    /// </summary>
    /// <returns>A <see cref="RegionManager"/> tied to the defaul dimension in this world.</returns>
    /// <remarks>Regions are a higher-level unit of organization for blocks unique to worlds created in Beta 1.3 and beyond.
    /// Consider using the <see cref="RegionChunkManager"/> if you are interested in working with blocks.</remarks>
    public AnvilRegionManager GetRegionManager() {
        return GetRegionManager(Dimension.DEFAULT);
    }

    /// <summary>
    /// Gets a <see cref="RegionManager"/> for the given dimension.
    /// </summary>
    /// <param name="dim">The id of the dimension to look up.</param>
    /// <returns>A <see cref="RegionManager"/> tied to the given dimension in this world.</returns>
    /// <remarks>Regions are a higher-level unit of organization for blocks unique to worlds created in Beta 1.3 and beyond.
    /// Consider using the <see cref="RegionChunkManager"/> if you are interested in working with blocks.</remarks>
    public AnvilRegionManager GetRegionManager(int dim) {
        return GetRegionManager(DimensionFromInt(dim));
    }

    public AnvilRegionManager GetRegionManager(string dim) {
        AnvilRegionManager rm;
        if(this.regionMgrs.TryGetValue(dim, out rm)) {
            return rm;
        }

        OpenDimension(dim);
        return this.regionMgrs[dim];
    }

    /// <summary>
    /// Gets a <see cref="PlayerManager"/> for maanging players on multiplayer worlds.
    /// </summary>
    /// <returns>A <see cref="PlayerManager"/> for this world.</returns>
    /// <remarks>To manage the player of a single-player world, get a <see cref="Level"/> object for the world instead.</remarks>
    public new PlayerManager GetPlayerManager() {
        return GetPlayerManagerVirt() as PlayerManager;
    }

    /// <summary>
    /// Gets a <see cref="BetaDataManager"/> for managing data resources, such as maps.
    /// </summary>
    /// <returns>A <see cref="BetaDataManager"/> for this world.</returns>
    public new BetaDataManager GetDataManager() {
        return GetDataManagerVirt() as BetaDataManager;
    }

    /// <inherits />
    public override void Save() {
        this.level.Save();

        foreach(KeyValuePair<string, RegionChunkManager> cm in this.chunkMgrs) {
            cm.Value.Save();
        }
    }

    /// <summary>
    /// Gets the <see cref="ChunkCache"/> currently managing chunks in the default dimension.
    /// </summary>
    /// <returns>The <see cref="ChunkCache"/> for the default dimension, or null if the dimension was not found.</returns>
    public ChunkCache GetChunkCache() {
        return GetChunkCache(Dimension.DEFAULT);
    }

    /// <summary>
    /// Gets the <see cref="ChunkCache"/> currently managing chunks in the given dimension.
    /// </summary>
    /// <param name="dim">The id of a dimension to look up.</param>
    /// <returns>The <see cref="ChunkCache"/> for the given dimension, or null if the dimension was not found.</returns>
    public ChunkCache GetChunkCache(int dim) {
        return GetChunkCache(DimensionFromInt(dim));
    }

    public ChunkCache GetChunkCache(string dim) {
        if(this.caches.ContainsKey(dim)) {
            return this.caches[dim];
        }
        return null;
    }

    /// <summary>
    /// Opens an existing Beta-compatible Minecraft world and returns a new <see cref="BetaWorld"/> to represent it.
    /// </summary>
    /// <param name="path">The path to the directory containing the world's level.dat, or the path to level.dat itself.</param>
    /// <returns>A new <see cref="BetaWorld"/> object representing an existing world on disk.</returns>
    public static new AnvilWorld Open(string path) {
        return new AnvilWorld().OpenWorld(path) as AnvilWorld;
    }

    /// <summary>
    /// Opens an existing Beta-compatible Minecraft world and returns a new <see cref="BetaWorld"/> to represent it.
    /// </summary>
    /// <param name="path">The path to the directory containing the world's level.dat, or the path to level.dat itself.</param>
    /// <param name="cacheSize">The preferred cache size in chunks for each opened dimension in this world.</param>
    /// <returns>A new <see cref="BetaWorld"/> object representing an existing world on disk.</returns>
    public static AnvilWorld Open(string path, int cacheSize) {
        AnvilWorld world = new AnvilWorld().OpenWorld(path);
        world.prefCacheSize = cacheSize;
        return world;
    }

    /// <summary>
    /// Creates a new Beta-compatible Minecraft world and returns a new <see cref="BetaWorld"/> to represent it.
    /// </summary>
    /// <param name="path">The path to the directory where the new world should be stored.</param>
    /// <returns>A new <see cref="BetaWorld"/> object representing a new world.</returns>
    /// <remarks>This method will attempt to create the specified directory immediately if it does not exist, but will not
    /// write out any world data unless it is explicitly saved at a later time.</remarks>
    public static AnvilWorld Create(string path) {
        return new AnvilWorld().CreateWorld(path) as AnvilWorld;
    }

    /// <summary>
    /// Creates a new Beta-compatible Minecraft world and returns a new <see cref="BetaWorld"/> to represent it.
    /// </summary>
    /// <param name="path">The path to the directory where the new world should be stored.</param>
    /// <param name="cacheSize">The preferred cache size in chunks for each opened dimension in this world.</param>
    /// <returns>A new <see cref="BetaWorld"/> object representing a new world.</returns>
    /// <remarks>This method will attempt to create the specified directory immediately if it does not exist, but will not
    /// write out any world data unless it is explicitly saved at a later time.</remarks>
    public static AnvilWorld Create(string path, int cacheSize) {
        AnvilWorld world = new AnvilWorld().CreateWorld(path);
        world.prefCacheSize = cacheSize;
        return world;
    }

    /// <exclude/>
    protected override IBlockManager GetBlockManagerVirt(int dim) {
        return GetBlockManagerVirt(DimensionFromInt(dim));
    }

    protected override IBlockManager GetBlockManagerVirt(string dim) {
        BlockManager rm;
        if(this.blockMgrs.TryGetValue(dim, out rm)) {
            return rm;
        }

        OpenDimension(dim);
        return this.blockMgrs[dim];
    }

    /// <exclude/>
    protected override IChunkManager GetChunkManagerVirt(int dim) {
        return GetChunkManagerVirt(DimensionFromInt(dim));
    }

    protected override IChunkManager GetChunkManagerVirt(string dim) {
        RegionChunkManager rm;
        if(this.chunkMgrs.TryGetValue(dim, out rm)) {
            return rm;
        }

        OpenDimension(dim);
        return this.chunkMgrs[dim];
    }

    /// <exclude/>
    protected override IPlayerManager GetPlayerManagerVirt() {
        if(this.playerMan != null) {
            return this.playerMan;
        }

        string path = IO.Path.Combine(Path, PLAYER_DIR_OLD);

        if(!Directory.Exists(path)) {
            path = IO.Path.Combine(Path, PLAYER_DIR_1_7_6_PLUS);
        }

        this.playerMan = new PlayerManager(path);
        return this.playerMan;
    }

    /// <exclude/>
    protected override Data.DataManager GetDataManagerVirt() {
        if(this.dataMan != null) {
            return this.dataMan;
        }

        this.dataMan = new BetaDataManager(this);
        return this.dataMan;
    }

    private string DimensionFromInt(int dim) {
        if(dim == Dimension.DEFAULT) {
            return "";
        } else {
            return "DIM" + dim;
        }
    }

    private void OpenDimension(string dim) {
        string path = Path;
        if(String.IsNullOrEmpty(dim)) {
            path = IO.Path.Combine(path, REGION_DIR);
        } else {
            path = IO.Path.Combine(path, dim);
            path = IO.Path.Combine(path, REGION_DIR);
        }

        if(!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }

        ChunkCache cc = new ChunkCache(this.prefCacheSize);

        AnvilRegionManager rm = new AnvilRegionManager(path, cc);
        RegionChunkManager cm = new RegionChunkManager(rm, cc);
        BlockManager bm = new AnvilBlockManager(cm);

        this.regionMgrs[dim] = rm;
        this.chunkMgrs[dim] = cm;
        this.blockMgrs[dim] = bm;

        this.caches[dim] = cc;
    }

    private AnvilWorld OpenWorld(string path) {
        if(!Directory.Exists(path)) {
            if(File.Exists(path)) {
                this.levelFile = IO.Path.GetFileName(path);
                path = IO.Path.GetDirectoryName(path);
            } else {
                throw new DirectoryNotFoundException("Directory '" + path + "' not found");
            }
        }

        Path = path;

        string ldat = IO.Path.Combine(path, this.levelFile);
        if(!File.Exists(ldat)) {
            throw new FileNotFoundException("Data file '" + this.levelFile + "' not found in '" + path + "'", ldat);
        }

        if(!LoadLevel()) {
            throw new Exception("Failed to load '" + this.levelFile + "'");
        }

        return this;
    }

    private AnvilWorld CreateWorld(string path) {
        if(!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }

        string regpath = IO.Path.Combine(path, REGION_DIR);
        if(!Directory.Exists(regpath)) {
            Directory.CreateDirectory(regpath);
        }

        Path = path;
        this.level = new Level(this);

        return this;
    }

    private bool LoadLevel() {
        NBTFile nf = new NBTFile(IO.Path.Combine(Path, this.levelFile));
        NbtTree tree;

        using(Stream nbtstr = nf.GetDataInputStream()) {
            if(nbtstr == null) {
                return false;
            }

            tree = new NbtTree(nbtstr);
        }

        this.level = new Level(this);
        this.level = this.level.LoadTreeSafe(tree.Root);

        return this.level != null;
    }

    internal static void OnResolveOpen(object sender, OpenWorldEventArgs e) {
        try {
            AnvilWorld world = new AnvilWorld().OpenWorld(e.Path);
            if(world == null) {
                return;
            }

            string regPath = IO.Path.Combine(e.Path, REGION_DIR);
            if(!Directory.Exists(regPath)) {
                return;
            }

            if(world.Level.Version < 19133) {
                return;
            }

            e.AddHandler(Open);
        } catch(Exception) {
            return;
        }
    }
}
