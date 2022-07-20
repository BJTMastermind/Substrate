namespace Substrate.Nbt;

/// <summary>
/// A concrete <see cref="SchemaNode"/> representing a <see cref="TagNodeShortArray"/>.
/// </summary>
public sealed class SchemaNodeShortArray : SchemaNode {
    private int length;

    /// <summary>
    /// Gets the expected length of the corresponding int array.
    /// </summary>
    public int Length {
        get { return this.length; }
    }

    /// <summary>
    /// Indicates whether there is an expected length of the corresponding int array.
    /// </summary>
    public bool HasExpectedLength {
        get { return this.length > 0; }
    }

    /// <summary>
    /// Constructs a new <see cref="SchemaNodeShortArray"/> representing a <see cref="TagNodeShortArray"/> named <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the corresponding <see cref="TagNodeIntArray"/>.</param>
    public SchemaNodeShortArray(string name)
        : base(name) {
        this.length = 0;
    }

    /// <summary>
    /// Constructs a new <see cref="SchemaNodeShortArray"/> with additional options.
    /// </summary>
    /// <param name="name">The name of the corresponding <see cref="TagNodeShortArray"/>.</param>
    /// <param name="options">One or more option flags modifying the processing of this node.</param>
    public SchemaNodeShortArray(string name, SchemaOptions options)
        : base(name, options) {
        this.length = 0;
    }

    /// <summary>
    /// Constructs a new <see cref="SchemaNodeShortArray"/> representing a <see cref="TagNodeShortArray"/> named <paramref name="name"/> with expected length <paramref name="length"/>.
    /// </summary>
    /// <param name="name">The name of the corresponding <see cref="TagNodeIntArray"/>.</param>
    /// <param name="length">The expected length of corresponding byte array.</param>
    public SchemaNodeShortArray(string name, int length)
        : base(name) {
        this.length = length;
    }

    /// <summary>
    /// Constructs a new <see cref="SchemaNodeShortArray"/> with additional options.
    /// </summary>
    /// <param name="name">The name of the corresponding <see cref="TagNodeShortArray"/>.</param>
    /// <param name="length">The expected length of corresponding byte array.</param>
    /// <param name="options">One or more option flags modifying the processing of this node.</param>
    public SchemaNodeShortArray(string name, int length, SchemaOptions options)
        : base(name, options) {
        this.length = length;
    }

    /// <summary>
    /// Constructs a default <see cref="TagNodeShortArray"/> satisfying the constraints of this node.
    /// </summary>
    /// <returns>A <see cref="TagNodeString"/> with a sensible default value.</returns>
    public override TagNode BuildDefaultTree() {
        return new TagNodeShortArray(new short[this.length]);
    }
}
