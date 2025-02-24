﻿//TODO: Benchmark struct vs. class.  If no difference, prefer class.
namespace Substrate;

using Substrate.Core;

/// <summary>
/// A reference to a single Alpha-compatible block in an <see cref="AlphaBlockCollection"/>.
/// </summary>
/// <remarks><para>The <see cref="AlphaBlockRef"/> type provides a reasonably lightweight reference to an individual block in a
/// <see cref="AlphaBlockCollection"/>.  The <see cref="AlphaBlockRef"/> does not store any of the data itself.  If the referenced
/// block in the <see cref="AlphaBlockCollection"/> is updated externally, those changes will be automatically reflected in the
/// <see cref="AlphaBlockRef"/>, and any changes made via the <see cref="AlphaBlockRef"/> will be applied directly to the corresponding
/// block within the <see cref="AlphaBlockCollection"/>.  Such changes will also set the dirty status of the <see cref="AlphaBlockCollection"/>,
/// which can make this type particularly useful.</para>
/// <para>Despite being lightweight, using an <see cref="AlphaBlockRef"/> to get and set block data is still more expensive then directly
/// getting and setting data in the <see cref="AlphaBlockCollection"/> object, and can be significantly slow in a tight loop
/// (<see cref="AlphaBlockCollection"/> does not provide an interface for enumerating <see cref="AlphaBlockRef"/> objects specifically
/// to discourage this kind of use).</para>
/// <para><see cref="AlphaBlockRef"/> objects are most appropriate in cases where looking up an object requires expensive checks, such as
/// accessing blocks through a derived <see cref="BlockManager"/> type with enhanced block filtering.  By getting an <see cref="AlphaBlockRef"/>,
/// any number of block attributes can be read or written to while only paying the lookup cost once to get the reference.  Using the
/// <see cref="BlockManager"/> (or similar) directly would incur the expensive lookup on each operation.  See NBToolkit for an example of this
/// use case.</para>
/// <para>Unlike the <see cref="AlphaBlock"/> object, this type exposed access to context-dependent data such as lighting.</para></remarks>
public struct AlphaBlockRef : IVersion10BlockRef {
    private readonly AlphaBlockCollection collection;
    private readonly int index;

    internal AlphaBlockRef(AlphaBlockCollection collection, int index) {
        this.collection = collection;
        this.index = index;
    }

    /// <summary>
    /// Checks if this object is currently a valid ref into another <see cref="AlphaBlockCollection"/>.
    /// </summary>
    public bool IsValid {
        get { return this.collection != null; }
    }

    #region IBlock Members
    /// <summary>
    /// Gets information on the type of the block.
    /// </summary>
    public BlockInfo Info {
        get { return this.collection.GetInfo(this.index); }
    }

    /// <summary>
    /// Gets or sets the id (type) of the block.
    /// </summary>
    public int ID {
        get { return this.collection.GetID(this.index); }
        set { this.collection.SetID(this.index, value); }
    }
    #endregion

    #region IDataBlock Members
    /// <summary>
    /// Gets or sets the supplementary data value of the block.
    /// </summary>
    public int Data {
        get { return this.collection.GetData(this.index); }
        set { this.collection.SetData(this.index, value); }
    }
    #endregion

    #region ILitBlock Members
    /// <summary>
    /// Gets or sets the block-source light component of the block.
    /// </summary>
    public int BlockLight {
        get { return this.collection.GetBlockLight(this.index); }
        set { this.collection.SetBlockLight(this.index, value); }
    }

    /// <summary>
    /// Gets or sets the sky-source light component of the block.
    /// </summary>
    public int SkyLight {
        get { return this.collection.GetSkyLight(this.index); }
        set { this.collection.SetSkyLight(this.index, value); }
    }
    #endregion

    #region IPropertyBlock Members
    /// <summary>
    /// Gets the Tile Entity record of the block if it has one.
    /// </summary>
    /// <returns>The <see cref="TileEntity"/> attached to this block, or null if the block type does not require a Tile Entity.</returns>
    public TileEntity GetTileEntity() {
        return this.collection.GetTileEntity(this.index);
    }

    /// <summary>
    /// Sets a new Tile Entity record for the block.
    /// </summary>
    /// <param name="te">A Tile Entity record compatible with the block's type.</param>
    public void SetTileEntity(TileEntity te) {
        this.collection.SetTileEntity(this.index, te);
    }

    /// <summary>
    /// Creates a default Tile Entity record appropriate for the block.
    /// </summary>
    public void CreateTileEntity() {
        this.collection.CreateTileEntity(this.index);
    }

    /// <summary>
    /// Removes any Tile Entity currently attached to the block.
    /// </summary>
    public void ClearTileEntity() {
        this.collection.ClearTileEntity(this.index);
    }
    #endregion

    #region IActiveBlock Members
    public int TileTickValue {
        get { return this.collection.GetTileTickValue(this.index); }
        set { this.collection.SetTileTickValue(this.index, value); }
    }

    /// <summary>
    /// Gets the <see cref="TileTick"/> record of the block if it has one.
    /// </summary>
    /// <returns>The <see cref="TileTick"/> attached to this block, or null if the block type does not require a Tile Entity.</returns>
    public TileTick GetTileTick() {
        return this.collection.GetTileTick(this.index);
    }

    /// <summary>
    /// Sets a new <see cref="TileTick"/> record for the block.
    /// </summary>
    /// <param name="te">A <see cref="TileTick"/> record compatible with the block's type.</param>
    public void SetTileTick(TileTick te) {
        this.collection.SetTileTick(this.index, te);
    }

    /// <summary>
    /// Creates a default <see cref="TileTick"/> record appropriate for the block.
    /// </summary>
    public void CreateTileTick() {
        this.collection.CreateTileTick(this.index);
    }

    /// <summary>
    /// Removes any <see cref="TileTick"/> currently attached to the block.
    /// </summary>
    public void ClearTileTick() {
        this.collection.ClearTileTick(this.index);
    }
    #endregion
}
