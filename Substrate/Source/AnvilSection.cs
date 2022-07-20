namespace Substrate;

using Substrate.Core;
using Substrate.Nbt;

public class AnvilSection : INbtObject<AnvilSection>, ICopyable<AnvilSection> {
    public static SchemaNodeCompound SectionSchema = new SchemaNodeCompound() {
        new SchemaNodeArray("Blocks", 4096),
        new SchemaNodeArray("Data", 2048),
        new SchemaNodeArray("SkyLight", 2048),
        new SchemaNodeArray("BlockLight", 2048),
        new SchemaNodeScaler("Y", TagType.TAG_BYTE),
        new SchemaNodeArray("Add", 2048, SchemaOptions.OPTIONAL),
    };

    private const int XDIM = 16;
    private const int YDIM = 16;
    private const int ZDIM = 16;

    private const int MIN_Y = 0;
    private const int MAX_Y = 15;

    private TagNodeCompound tree;

    private byte y;
    private YZXByteArray blocks;
    private YZXNibbleArray data;
    private YZXNibbleArray blockLight;
    private YZXNibbleArray skyLight;
    private YZXNibbleArray addBlocks;

    private AnvilSection() {
    }

    public AnvilSection(int y) {
        if(y < MIN_Y || y > MAX_Y) {
            throw new ArgumentOutOfRangeException();
        }

        this.y = (byte) y;
        BuildNbtTree();
    }

    public AnvilSection(TagNodeCompound tree) {
        LoadTree(tree);
    }

    public int Y {
        get { return this.y; }
        set {
            if(value < MIN_Y || value > MAX_Y) {
                throw new ArgumentOutOfRangeException();
            }

            this.y = (byte) value;
            this.tree["Y"].ToTagByte().Data = this.y;
        }
    }

    public YZXByteArray Blocks {
        get { return this.blocks; }
    }

    public YZXNibbleArray Data {
        get { return this.data; }
    }

    public YZXNibbleArray BlockLight {
        get { return this.blockLight; }
    }

    public YZXNibbleArray SkyLight {
        get { return this.skyLight; }
    }

    public YZXNibbleArray AddBlocks {
        get { return this.addBlocks; }
    }

    public bool CheckEmpty() {
        return CheckBlocksEmpty() && CheckAddBlocksEmpty();
    }

    private bool CheckBlocksEmpty() {
        for(int i = 0; i < this.blocks.Length; i++) {
            if(this.blocks[i] != 0) {
                return false;
            }
        }
        return true;
    }

    private bool CheckAddBlocksEmpty() {
        if(this.addBlocks != null) {
            for(int i = 0; i < this.addBlocks.Length; i++) {
                if(this.addBlocks[i] != 0) {
                    return false;
                }
            }
        }
        return true;
    }

    #region INbtObject<AnvilSection> Members
    public AnvilSection LoadTree(TagNode tree) {
        TagNodeCompound ctree = tree as TagNodeCompound;
        if(ctree == null) {
            return null;
        }

        this.y = ctree["Y"] as TagNodeByte;

        this.blocks = new YZXByteArray(XDIM, YDIM, ZDIM, ctree["Blocks"] as TagNodeByteArray);
        this.data = new YZXNibbleArray(XDIM, YDIM, ZDIM, ctree["Data"] as TagNodeByteArray);
        this.skyLight = new YZXNibbleArray(XDIM, YDIM, ZDIM, ctree["SkyLight"] as TagNodeByteArray);
        this.blockLight = new YZXNibbleArray(XDIM, YDIM, ZDIM, ctree["BlockLight"] as TagNodeByteArray);

        if(!ctree.ContainsKey("Add")) {
            ctree["Add"] = new TagNodeByteArray(new byte[2048]);
        }

        this.addBlocks = new YZXNibbleArray(XDIM, YDIM, ZDIM, ctree["Add"] as TagNodeByteArray);

        this.tree = ctree;

        return this;
    }

    public AnvilSection LoadTreeSafe(TagNode tree) {
        if(!ValidateTree(tree)) {
            return null;
        }
        return LoadTree(tree);
    }

    public TagNode BuildTree() {
        TagNodeCompound copy = new TagNodeCompound();
        foreach(KeyValuePair<string, TagNode> node in this.tree) {
            copy.Add(node.Key, node.Value);
        }

        if(CheckAddBlocksEmpty()) {
            copy.Remove("Add");
        }

        return copy;
    }

    public bool ValidateTree(TagNode tree) {
        NbtVerifier v = new NbtVerifier(tree, SectionSchema);
        return v.Verify();
    }
    #endregion

    #region ICopyable<AnvilSection> Members
    public AnvilSection Copy() {
        return new AnvilSection().LoadTree(this.tree.Copy());
    }
    #endregion

    private void BuildNbtTree() {
        int elements3 = XDIM * YDIM * ZDIM;

        TagNodeByteArray blocks = new TagNodeByteArray(new byte[elements3]);
        TagNodeByteArray data = new TagNodeByteArray(new byte[elements3 >> 1]);
        TagNodeByteArray skyLight = new TagNodeByteArray(new byte[elements3 >> 1]);
        TagNodeByteArray blockLight = new TagNodeByteArray(new byte[elements3 >> 1]);
        TagNodeByteArray addBlocks = new TagNodeByteArray(new byte[elements3 >> 1]);

        this.blocks = new YZXByteArray(XDIM, YDIM, ZDIM, blocks);
        this.data = new YZXNibbleArray(XDIM, YDIM, ZDIM, data);
        this.skyLight = new YZXNibbleArray(XDIM, YDIM, ZDIM, skyLight);
        this.blockLight = new YZXNibbleArray(XDIM, YDIM, ZDIM, blockLight);
        this.addBlocks = new YZXNibbleArray(XDIM, YDIM, ZDIM, addBlocks);

        TagNodeCompound tree = new TagNodeCompound();
        tree.Add("Y", new TagNodeByte(this.y));
        tree.Add("Blocks", blocks);
        tree.Add("Data", data);
        tree.Add("SkyLight", skyLight);
        tree.Add("BlockLight", blockLight);
        tree.Add("Add", addBlocks);

        this.tree = tree;
    }
}
