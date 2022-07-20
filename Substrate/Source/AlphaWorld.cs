namespace Substrate;

using Substrate.Core;
using Substrate.Nbt;
using IO = System.IO;

/// <summary>
/// Represents an Alpha-compatible (up to Beta 1.2) Minecraft world.
/// </summary>
public class AlphaWorld : NbtWorld {
    private const string _PLAYER_DIR = "players";
    private string levelFile = "level.dat";

    private Level level;

    private Dictionary<string, AlphaChunkManager> chunkMgrs;
    private Dictionary<string, BlockManager> blockMgrs;

    private PlayerManager playerMan;

    private AlphaWorld() {
        this.chunkMgrs = new Dictionary<string, AlphaChunkManager>();
        this.blockMgrs = new Dictionary<string, BlockManager>();
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

    /// <summary>
    /// Gets a <see cref="RegionChunkManager"/> for the default dimension.
    /// </summary>
    /// <returns>A <see cref="RegionChunkManager"/> tied to the default dimension in this world.</returns>
    /// <remarks>Get a <see cref="RegionChunkManager"/> if you you need to work with easily-digestible, bounded chunks of blocks.</remarks>
    public new AlphaChunkManager GetChunkManager() {
        return GetChunkManagerVirt(Dimension.DEFAULT) as AlphaChunkManager;
    }

    /// <summary>
    /// Gets a <see cref="RegionChunkManager"/> for the given dimension.
    /// </summary>
    /// <param name="dim">The id of the dimension to look up.</param>
    /// <returns>A <see cref="RegionChunkManager"/> tied to the given dimension in this world.</returns>
    /// <remarks>Get a <see cref="RegionChunkManager"/> if you you need to work with easily-digestible, bounded chunks of blocks.</remarks>
    public new AlphaChunkManager GetChunkManager(int dim) {
        return GetChunkManagerVirt(dim) as AlphaChunkManager;
    }

    /// <summary>
    /// Gets a <see cref="PlayerManager"/> for maanging players on multiplayer worlds.
    /// </summary>
    /// <returns>A <see cref="PlayerManager"/> for this world.</returns>
    /// <remarks>To manage the player of a single-player world, get a <see cref="Level"/> object for the world instead.</remarks>
    public new PlayerManager GetPlayerManager() {
        return GetPlayerManagerVirt() as PlayerManager;
    }

    /// <inherits />
    public override void Save() {
        this.level.Save();

        foreach(KeyValuePair<string, AlphaChunkManager> cm in this.chunkMgrs) {
            cm.Value.Save();
        }
    }

    /// <summary>
    /// Opens an existing Alpha-compatible Minecraft world and returns a new <see cref="AlphaWorld"/> to represent it.
    /// </summary>
    /// <param name="path">The path to the directory containing the world's level.dat, or the path to level.dat itself.</param>
    /// <returns>A new <see cref="AlphaWorld"/> object representing an existing world on disk.</returns>
    public static new AlphaWorld Open(string path) {
        return new AlphaWorld().OpenWorld(path) as AlphaWorld;
    }

    /// <summary>
    /// Creates a new Alpha-compatible Minecraft world and returns a new <see cref="AlphaWorld"/> to represent it.
    /// </summary>
    /// <param name="path">The path to the directory where the new world should be stored.</param>
    /// <returns>A new <see cref="AlphaWorld"/> object representing a new world.</returns>
    /// <remarks>This method will attempt to create the specified directory immediately if it does not exist, but will not
    /// write out any world data unless it is explicitly saved at a later time.</remarks>
    public static AlphaWorld Create(string path) {
        return new AlphaWorld().CreateWorld(path) as AlphaWorld;
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
        AlphaChunkManager rm;
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

        string path = IO.Path.Combine(Path, _PLAYER_DIR);

        this.playerMan = new PlayerManager(path);
        return this.playerMan;
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
        if(!String.IsNullOrEmpty(dim)) {
            path = IO.Path.Combine(path, dim);
        }

        if(!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }

        AlphaChunkManager cm = new AlphaChunkManager(path);
        BlockManager bm = new AlphaBlockManager(cm);

        this.chunkMgrs[dim] = cm;
        this.blockMgrs[dim] = bm;
    }

    private AlphaWorld OpenWorld(string path) {
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

    private AlphaWorld CreateWorld(string path) {
        if(!Directory.Exists(path)) {
            throw new DirectoryNotFoundException("Directory '" + path + "' not found");
        }

        Path = path;
        this.level = new Level(this);
        this.level.Version = 0;

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
            AlphaWorld world = new AlphaWorld().OpenWorld(e.Path);
            if(world == null) {
                return;
            }

            if(world.Level.Version != 0) {
                return;
            }

            e.AddHandler(Open);
        } catch(Exception) {
            return;
        }
    }
}
