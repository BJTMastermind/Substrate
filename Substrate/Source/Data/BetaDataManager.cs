namespace Substrate.Data;

using Substrate.Core;
using Substrate.Nbt;

public class BetaDataManager : DataManager, INbtObject<BetaDataManager> {
    private static SchemaNodeCompound schema = new SchemaNodeCompound() {
        new SchemaNodeScaler("map", TagType.TAG_SHORT),
    };

    private TagNodeCompound source;

    private NbtWorld world;

    private short mapId;

    private MapManager maps;

    public BetaDataManager(NbtWorld world) {
        this.world = world;

        this.maps = new MapManager(this.world);
    }

    public override int CurrentMapId {
        get { return this.mapId; }
        set { this.mapId = (short) value; }
    }

    public new MapManager Maps {
        get { return this.maps; }
    }

    protected override IMapManager GetMapManager() {
        return this.maps;
    }

    public override bool Save() {
        if(this.world == null) {
            return false;
        }

        try {
            string path = Path.Combine(this.world.Path, this.world.DataDirectory);
            NBTFile nf = new NBTFile(Path.Combine(path, "idcounts.dat"));

            using(Stream zipstr = nf.GetDataOutputStream(CompressionType.None)) {
                if(zipstr == null) {
                    NbtIOException nex = new NbtIOException("Failed to initialize uncompressed NBT stream for output");
                    nex.Data["DataManager"] = this;
                    throw nex;
                }

                new NbtTree(BuildTree() as TagNodeCompound).WriteTo(zipstr);
            }
            return true;
        } catch(Exception ex) {
            Exception lex = new Exception("Could not save idcounts.dat file.", ex);
            lex.Data["DataManager"] = this;
            throw lex;
        }
    }

    #region INBTObject<DataManager>
    public virtual BetaDataManager LoadTree(TagNode tree) {
        TagNodeCompound ctree = tree as TagNodeCompound;
        if(ctree == null) {
            return null;
        }

        this.mapId = ctree["map"].ToTagShort();
        this.source = ctree.Copy() as TagNodeCompound;

        return this;
    }

    public virtual BetaDataManager LoadTreeSafe(TagNode tree) {
        if(!ValidateTree(tree)) {
            return null;
        }
        return LoadTree(tree);
    }

    public virtual TagNode BuildTree() {
        TagNodeCompound tree = new TagNodeCompound();
        tree["map"] = new TagNodeLong(this.mapId);

        if(this.source != null) {
            tree.MergeFrom(this.source);
        }
        return tree;
    }

    public virtual bool ValidateTree(TagNode tree) {
        return new NbtVerifier(tree, schema).Verify();
    }
    #endregion
}
