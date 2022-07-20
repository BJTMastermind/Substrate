namespace Substrate.Data;

using Substrate.Core;
using Substrate.Nbt;

/// <summary>
/// Represents the complete data of a Map item.
/// </summary>
public class Map : INbtObject<Map>, ICopyable<Map> {
    private static SchemaNodeCompound schema = new SchemaNodeCompound() {
        new SchemaNodeCompound("data") {
            new SchemaNodeScaler("scale", TagType.TAG_BYTE),
            new SchemaNodeScaler("dimension", TagType.TAG_BYTE),
            new SchemaNodeScaler("height", TagType.TAG_SHORT),
            new SchemaNodeScaler("width", TagType.TAG_SHORT),
            new SchemaNodeScaler("xCenter", TagType.TAG_INT),
            new SchemaNodeScaler("zCenter", TagType.TAG_INT),
            new SchemaNodeArray("colors"),
        },
    };

    private TagNodeCompound source;

    private NbtWorld world;
    private int id;

    private byte scale;
    private byte dimension;
    private short height;
    private short width;
    private int x;
    private int z;

    private byte[] colors;

    /// <summary>
    /// Creates a new default <see cref="Map"/> object.
    /// </summary>
    public Map() {
        this.scale = 3;
        this.dimension = 0;
        this.height = 128;
        this.width = 128;

        this.colors = new byte[this.width * this.height];
    }

    /// <summary>
    /// Creates a new <see cref="Map"/> object with copied data.
    /// </summary>
    /// <param name="p">A <see cref="Map"/> to copy data from.</param>
    protected Map(Map p) {
        this.world = p.world;
        this.id = p.id;

        this.scale = p.scale;
        this.dimension = p.dimension;
        this.height = p.height;
        this.width = p.width;
        this.x = p.x;
        this.z = p.z;

        this.colors = new byte[this.width * this.height];
        if(p.colors != null) {
            p.colors.CopyTo(this.colors, 0);
        }
    }

    /// <summary>
    /// Gets or sets the id value associated with this map.
    /// </summary>
    public int Id {
        get { return this.id; }
        set {
            if(this.id < 0 || this.id >= 65536) {
                throw new ArgumentOutOfRangeException("value", value, "Map Ids must be in the range [0, 65535].");
            }
            this.id = value;
        }
    }

    /// <summary>
    /// Gets or sets the scale of the map.  Acceptable values are 0(1:1) to 4(1:16).
    /// </summary>
    public int Scale {
        get { return this.scale; }
        set { this.scale = (byte) value; }
    }

    /// <summary>
    /// Gets or sets the(World) Dimension of the map.
    /// </summary>
    public int Dimension {
        get { return this.dimension; }
        set { this.dimension = (byte) value; }
    }

    /// <summary>
    /// Gets or sets the height of the map.
    /// </summary>
    /// <remarks>If the new height dimension is different, the map's color data will be reset.</remarks>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the new height value is zero or negative.</exception>
    public int Height {
        get { return this.height; }
        set {
            if(value <= 0) {
                throw new ArgumentOutOfRangeException("value", "Height must be a positive number");
            }
            if(this.height != value) {
                this.height = (short) value;
                this.colors = new byte[this.width * this.height];
            }
        }
    }

    /// <summary>
    /// Gets or sets the width of the map.
    /// </summary>
    /// <remarks>If the new width dimension is different, the map's color data will be reset.</remarks>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the new width value is zero or negative.</exception>
    public int Width {
        get { return this.width; }
        set {
            if(value <= 0) {
                throw new ArgumentOutOfRangeException("value", "Width must be a positive number");
            }
            if(this.width != value) {
                this.width = (short) value;
                this.colors = new byte[this.width * this.height];
            }
        }
    }

    /// <summary>
    /// Gets or sets the global X-coordinate that this map is centered on, in blocks.
    /// </summary>
    public int X {
        get { return this.x; }
        set { this.x = value; }
    }

    /// <summary>
    /// Gets or sets the global Z-coordinate that this map is centered on, in blocks.
    /// </summary>
    public int Z {
        get { return this.z; }
        set { this.z = value; }
    }

    /// <summary>
    /// Gets the raw byte array of the map's color index values.
    /// </summary>
    public byte[] Colors {
        get { return this.colors; }
    }

    /// <summary>
    /// Gets or sets a color index value within the map's internal colors bitmap.
    /// </summary>
    /// <param name="x">The X-coordinate to get or set.</param>
    /// <param name="z">The Z-coordinate to get or set.</param>
    /// <exception cref="IndexOutOfRangeException">Thrown when the X- or Z-coordinates exceed the map dimensions.</exception>
    public byte this[int x, int z] {
        get {
            if(x < 0 || x >= this.width || z < 0 || z >= this.height) {
                throw new IndexOutOfRangeException();
            }
            return this.colors[x + this.width * z];
        }
        set {
            if(x < 0 || x >= this.width || z < 0 || z >= this.height) {
                throw new IndexOutOfRangeException();
            }
            this.colors[x + this.width * z] = value;
        }
    }

    /// <summary>
    /// Saves a <see cref="Map"/> object to disk as a standard compressed NBT stream.
    /// </summary>
    /// <returns>True if the map was saved; false otherwise.</returns>
    /// <exception cref="Exception">Thrown when an error is encountered writing out the level.</exception>
    public bool Save() {
        if(this.world == null) {
            return false;
        }

        try {
            string path = Path.Combine(this.world.Path, this.world.DataDirectory);
            NBTFile nf = new NBTFile(Path.Combine(path, "map_" + this.id + ".dat"));

            using(Stream zipstr = nf.GetDataOutputStream()) {
                if(zipstr == null) {
                    NbtIOException nex = new NbtIOException("Failed to initialize compressed NBT stream for output");
                    nex.Data["Map"] = this;
                    throw nex;
                }

                new NbtTree(BuildTree() as TagNodeCompound).WriteTo(zipstr);
            }

            return true;
        } catch(Exception ex) {
            Exception mex = new Exception("Could not save map file.", ex); // TODO: Exception Type
            mex.Data["Map"] = this;
            throw mex;
        }
    }

    #region INBTObject<Map> Members
    /// <summary>
    /// Attempt to load a Map subtree into the <see cref="Map"/> without validation.
    /// </summary>
    /// <param name="tree">The root node of a Map subtree.</param>
    /// <returns>The <see cref="Map"/> returns itself on success, or null if the tree was unparsable.</returns>
    public virtual Map LoadTree(TagNode tree) {
        TagNodeCompound dtree = tree as TagNodeCompound;
        if(dtree == null) {
            return null;
        }

        TagNodeCompound ctree = dtree["data"].ToTagCompound();

        this.scale = ctree["scale"].ToTagByte();
        this.dimension = ctree["dimension"].ToTagByte();
        this.height = ctree["height"].ToTagShort();
        this.width = ctree["width"].ToTagShort();
        this.x = ctree["xCenter"].ToTagInt();
        this.z = ctree["zCenter"].ToTagInt();

        this.colors = ctree["colors"].ToTagByteArray();
        this.source = ctree.Copy() as TagNodeCompound;

        return this;
    }

    /// <summary>
    /// Attempt to load a Map subtree into the <see cref="Map"/> with validation.
    /// </summary>
    /// <param name="tree">The root node of a Map subtree.</param>
    /// <returns>The <see cref="Map"/> returns itself on success, or null if the tree failed validation.</returns>
    public virtual Map LoadTreeSafe(TagNode tree) {
        if(!ValidateTree(tree)) {
            return null;
        }

        Map map = LoadTree(tree);

        if(map != null) {
            if(map.colors.Length != map.width * map.height) {
                throw new Exception("Unexpected length of colors byte array in Map"); // TODO: Expception Type
            }
        }
        return map;
    }

    /// <summary>
    /// Builds a Map subtree from the current data.
    /// </summary>
    /// <returns>The root node of a Map subtree representing the current data.</returns>
    public virtual TagNode BuildTree() {
        TagNodeCompound data = new TagNodeCompound();
        data["scale"] = new TagNodeByte(this.scale);
        data["dimension"] = new TagNodeByte(this.dimension);
        data["height"] = new TagNodeShort(this.height);
        data["width"] = new TagNodeShort(this.width);
        data["xCenter"] = new TagNodeInt(this.x);
        data["zCenter"] = new TagNodeInt(this.z);
        data["colors"] = new TagNodeByteArray(this.colors);

        if(this.source != null) {
            data.MergeFrom(this.source);
        }

        TagNodeCompound tree = new TagNodeCompound();
        tree.Add("data", data);

        return tree;
    }

    /// <summary>
    /// Validate a Map subtree against a schema defintion.
    /// </summary>
    /// <param name="tree">The root node of a Map subtree.</param>
    /// <returns>Status indicating whether the tree was valid against the internal schema.</returns>
    public virtual bool ValidateTree(TagNode tree) {
        return new NbtVerifier(tree, schema).Verify();
    }
    #endregion

    #region ICopyable<Map> Members
    /// <summary>
    /// Creates a deep-copy of the <see cref="Map"/>.
    /// </summary>
    /// <returns>A deep-copy of the <see cref="Map"/>.</returns>
    public virtual Map Copy() {
        return new Map(this);
    }
    #endregion
}
