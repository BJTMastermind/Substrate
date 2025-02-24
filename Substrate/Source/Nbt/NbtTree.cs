﻿namespace Substrate.Nbt;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Substrate.Core;

/// <summary>
/// Contains the root node of an NBT tree and handles IO of tree nodes.
/// </summary>
/// <remarks>
/// NBT, or Named Byte Tag, is a tree-based data structure for storing most Minecraft data.
/// NBT_Tree is more of a helper class for NBT trees that handles reading and writing nodes to data streams.
/// Most of the API takes a TagValue or derived node as the root of the tree, rather than an NBT_Tree object itself.
/// </remarks>
public class NbtTree : ICopyable<NbtTree> {
    private Stream stream = null;
    private TagNodeCompound root = null;
    private string rootName = "";

    private static TagNodeNull nulltag = new TagNodeNull();

    /// <summary>
    /// Gets the root node of this tree.
    /// </summary>
    public TagNodeCompound Root {
        get { return this.root; }
    }

    /// <summary>
    /// Gets or sets the name of the tree's root node.
    /// </summary>
    public string Name {
        get { return this.rootName; }
        set { this.rootName = value; }
    }

    /// <summary>
    /// Constructs a wrapper around a new NBT tree with an empty root node.
    /// </summary>
    public NbtTree() {
        this.root = new TagNodeCompound();
    }

    /// <summary>
    /// Constructs a wrapper around another NBT tree.
    /// </summary>
    /// <param name="tree">The root node of an NBT tree.</param>
    public NbtTree(TagNodeCompound tree) {
        this.root = tree;
    }

    /// <summary>
    /// Constructs a wrapper around another NBT tree and gives it a name.
    /// </summary>
    /// <param name="tree">The root node of an NBT tree.</param>
    /// <param name="name">The name for the root node.</param>
    public NbtTree(TagNodeCompound tree, string name) {
        this.root = tree;
        this.rootName = name;
    }

    /// <summary>
    /// Constructs and wrapper around a new NBT tree parsed from a source data stream.
    /// </summary>
    /// <param name="s">An open, readable data stream containing NBT data.</param>
    public NbtTree(Stream s) {
        ReadFrom(s);
    }

    /// <summary>
    /// Rebuild the internal NBT tree from a source data stream.
    /// </summary>
    /// <param name="s">An open, readable data stream containing NBT data.</param>
    public void ReadFrom(Stream s) {
        if(s != null) {
            this.stream = s;
            this.root = ReadRoot();
            this.stream = null;
        }
    }

    /// <summary>
    /// Writes out the internal NBT tree to a destination data stream.
    /// </summary>
    /// <param name="s">An open, writable data stream.</param>
    public void WriteTo(Stream s) {
        if(s != null) {
            this.stream = s;

            if(this.root != null) {
                WriteTag(this.rootName, this.root);
            }

            this.stream = null;
        }
    }

    private TagNode ReadValue(TagType type) {
        switch(type) {
            case TagType.TAG_END:
                return null;

            case TagType.TAG_BYTE:
                return ReadByte();

            case TagType.TAG_SHORT:
                return ReadShort();

            case TagType.TAG_INT:
                return ReadInt();

            case TagType.TAG_LONG:
                return ReadLong();

            case TagType.TAG_FLOAT:
                return ReadFloat();

            case TagType.TAG_DOUBLE:
                return ReadDouble();

            case TagType.TAG_BYTE_ARRAY:
                return ReadByteArray();

            case TagType.TAG_STRING:
                return ReadString();

            case TagType.TAG_LIST:
                return ReadList();

            case TagType.TAG_COMPOUND:
                return ReadCompound();

            case TagType.TAG_INT_ARRAY:
                return ReadIntArray();

            case TagType.TAG_LONG_ARRAY:
                return ReadLongArray();

            case TagType.TAG_SHORT_ARRAY:
                return ReadShortArray();
        }

        throw new Exception();
    }

    private TagNode ReadByte() {
        int gzByte = this.stream.ReadByte();
        if(gzByte == -1) {
            throw new NBTException(NBTException.MSG_GZIP_ENDOFSTREAM);
        }

        TagNodeByte val = new TagNodeByte((byte) gzByte);

        return val;
    }

    private TagNode ReadShort() {
        byte[] gzBytes = new byte[2];
        this.stream.Read(gzBytes, 0, 2);

        if(BitConverter.IsLittleEndian) {
            Array.Reverse(gzBytes);
        }

        TagNodeShort val = new TagNodeShort(BitConverter.ToInt16(gzBytes, 0));

        return val;
    }

    private TagNode ReadInt() {
        byte[] gzBytes = new byte[4];
        this.stream.Read(gzBytes, 0, 4);

        if(BitConverter.IsLittleEndian) {
            Array.Reverse(gzBytes);
        }

        TagNodeInt val = new TagNodeInt(BitConverter.ToInt32(gzBytes, 0));

        return val;
    }

    private TagNode ReadLong() {
        byte[] gzBytes = new byte[8];
        this.stream.Read(gzBytes, 0, 8);

        if(BitConverter.IsLittleEndian) {
            Array.Reverse(gzBytes);
        }

        TagNodeLong val = new TagNodeLong(BitConverter.ToInt64(gzBytes, 0));

        return val;
    }

    private TagNode ReadFloat() {
        byte[] gzBytes = new byte[4];
        this.stream.Read(gzBytes, 0, 4);

        if(BitConverter.IsLittleEndian) {
            Array.Reverse(gzBytes);
        }

        TagNodeFloat val = new TagNodeFloat(BitConverter.ToSingle(gzBytes, 0));

        return val;
    }

    private TagNode ReadDouble() {
        byte[] gzBytes = new byte[8];
        this.stream.Read(gzBytes, 0, 8);

        if(BitConverter.IsLittleEndian) {
            Array.Reverse(gzBytes);
        }

        TagNodeDouble val = new TagNodeDouble(BitConverter.ToDouble(gzBytes, 0));

        return val;
    }

    private TagNode ReadByteArray() {
        byte[] lenBytes = new byte[4];
        this.stream.Read(lenBytes, 0, 4);

        if(BitConverter.IsLittleEndian) {
            Array.Reverse(lenBytes);
        }

        int length = BitConverter.ToInt32(lenBytes, 0);
        if(length < 0) {
            throw new NBTException(NBTException.MSG_READ_NEG);
        }

        byte[] data = new byte[length];
        this.stream.Read(data, 0, length);

        TagNodeByteArray val = new TagNodeByteArray(data);

        return val;
    }

    private TagNode ReadString() {
        byte[] lenBytes = new byte[2];
        this.stream.Read(lenBytes, 0, 2);

        if(BitConverter.IsLittleEndian) {
            Array.Reverse(lenBytes);
        }

        short len = BitConverter.ToInt16(lenBytes, 0);
        if(len < 0) {
            throw new NBTException(NBTException.MSG_READ_NEG);
        }

        byte[] strBytes = new byte[len];
        this.stream.Read(strBytes, 0, len);

        System.Text.Encoding str = Encoding.UTF8;

        TagNodeString val = new TagNodeString(str.GetString(strBytes));

        return val;
    }

    private TagNode ReadList() {
        int gzByte = this.stream.ReadByte();
        if(gzByte == -1) {
            throw new NBTException(NBTException.MSG_GZIP_ENDOFSTREAM);
        }

        TagNodeList val = new TagNodeList((TagType) gzByte);
        if(val.ValueType > (TagType) Enum.GetValues(typeof(TagType)).GetUpperBound(0)) {
            throw new NBTException(NBTException.MSG_READ_TYPE);
        }

        byte[] lenBytes = new byte[4];
        this.stream.Read(lenBytes, 0, 4);

        if(BitConverter.IsLittleEndian) {
            Array.Reverse(lenBytes);
        }

        int length = BitConverter.ToInt32(lenBytes, 0);
        if(length < 0) {
            throw new NBTException(NBTException.MSG_READ_NEG);
        }

        if(val.ValueType == TagType.TAG_END) {
            return new TagNodeList(TagType.TAG_BYTE);
        }

        for(int i = 0; i < length; i++) {
            val.Add(ReadValue(val.ValueType));
        }

        return val;
    }

    private TagNode ReadCompound() {
        TagNodeCompound val = new TagNodeCompound();

        while(ReadTag(val)) {
            ;
        }

        return val;
    }

    private TagNode ReadIntArray() {
        byte[] lenBytes = new byte[4];
        this.stream.Read(lenBytes, 0, 4);

        if(BitConverter.IsLittleEndian) {
            Array.Reverse(lenBytes);
        }

        int length = BitConverter.ToInt32(lenBytes, 0);
        if(length < 0) {
            throw new NBTException(NBTException.MSG_READ_NEG);
        }

        int[] data = new int[length];
        byte[] buffer = new byte[4];
        for(int i = 0; i < length; i++) {
            this.stream.Read(buffer, 0, 4);
            if(BitConverter.IsLittleEndian) {
                Array.Reverse(buffer);
            }
            data[i] = BitConverter.ToInt32(buffer, 0);
        }

        TagNodeIntArray val = new TagNodeIntArray(data);

        return val;
    }

    private TagNode ReadLongArray() {
        byte[] lenBytes = new byte[4];
        this.stream.Read(lenBytes, 0, 4);

        if(BitConverter.IsLittleEndian) {
            Array.Reverse(lenBytes);
        }

        int length = BitConverter.ToInt32(lenBytes, 0);
        if(length < 0) {
            throw new NBTException(NBTException.MSG_READ_NEG);
        }

        long[] data = new long[length];
        byte[] buffer = new byte[8];
        for(int i = 0; i < length; i++) {
            this.stream.Read(buffer, 0, 8);
            if(BitConverter.IsLittleEndian) {
                Array.Reverse(buffer);
            }
            data[i] = BitConverter.ToInt64(buffer, 0);
        }

        TagNodeLongArray val = new TagNodeLongArray(data);

        return val;
    }

    private TagNode ReadShortArray() {
        byte[] lenBytes = new byte[4];
        this.stream.Read(lenBytes, 0, 4);

        if(BitConverter.IsLittleEndian) {
            Array.Reverse(lenBytes);
        }

        int length = BitConverter.ToInt32(lenBytes, 0);
        if(length < 0) {
            throw new NBTException(NBTException.MSG_READ_NEG);
        }

        short[] data = new short[length];
        byte[] buffer = new byte[2];
        for(int i = 0; i < length; i++) {
            this.stream.Read(buffer, 0, 2);
            if(BitConverter.IsLittleEndian) {
                Array.Reverse(buffer);
            }
            data[i] = BitConverter.ToInt16(buffer, 0);
        }

        TagNodeShortArray val = new TagNodeShortArray(data);

        return val;
    }

    private TagNodeCompound ReadRoot() {
        TagType type = (TagType) this.stream.ReadByte();
        if(type == TagType.TAG_COMPOUND) {
            this.rootName = ReadString().ToTagString().Data; // name
            return ReadValue(type) as TagNodeCompound;
        }

        return null;
    }

    private bool ReadTag(TagNodeCompound parent) {
        TagType type = (TagType) this.stream.ReadByte();
        if(type != TagType.TAG_END) {
            string name = ReadString().ToTagString().Data;
            parent[name] = ReadValue(type);
            return true;
        }

        return false;
    }

    private void WriteValue(TagNode val) {
        switch(val.GetTagType()) {
            case TagType.TAG_END:
                break;

            case TagType.TAG_BYTE:
                WriteByte(val.ToTagByte());
                break;

            case TagType.TAG_SHORT:
                WriteShort(val.ToTagShort());
                break;

            case TagType.TAG_INT:
                WriteInt(val.ToTagInt());
                break;

            case TagType.TAG_LONG:
                WriteLong(val.ToTagLong());
                break;

            case TagType.TAG_FLOAT:
                WriteFloat(val.ToTagFloat());
                break;

            case TagType.TAG_DOUBLE:
                WriteDouble(val.ToTagDouble());
                break;

            case TagType.TAG_BYTE_ARRAY:
                WriteByteArray(val.ToTagByteArray());
                break;

            case TagType.TAG_STRING:
                WriteString(val.ToTagString());
                break;

            case TagType.TAG_LIST:
                WriteList(val.ToTagList());
                break;

            case TagType.TAG_COMPOUND:
                WriteCompound(val.ToTagCompound());
                break;

            case TagType.TAG_INT_ARRAY:
                WriteIntArray(val.ToTagIntArray());
                break;

            case TagType.TAG_LONG_ARRAY:
                WriteLongArray(val.ToTagLongArray());
                break;

            case TagType.TAG_SHORT_ARRAY:
                WriteShortArray(val.ToTagShortArray());
                break;
        }
    }

    private void WriteByte(TagNodeByte val) {
        this.stream.WriteByte(val.Data);
    }

    private void WriteShort(TagNodeShort val) {
        byte[] gzBytes = BitConverter.GetBytes(val.Data);

        if(BitConverter.IsLittleEndian) {
            Array.Reverse(gzBytes);
        }

        this.stream.Write(gzBytes, 0, 2);
    }

    private void WriteInt(TagNodeInt val) {
        byte[] gzBytes = BitConverter.GetBytes(val.Data);

        if(BitConverter.IsLittleEndian) {
            Array.Reverse(gzBytes);
        }

        this.stream.Write(gzBytes, 0, 4);
    }

    private void WriteLong(TagNodeLong val) {
        byte[] gzBytes = BitConverter.GetBytes(val.Data);

        if(BitConverter.IsLittleEndian) {
            Array.Reverse(gzBytes);
        }

        this.stream.Write(gzBytes, 0, 8);
    }

    private void WriteFloat(TagNodeFloat val) {
        byte[] gzBytes = BitConverter.GetBytes(val.Data);

        if(BitConverter.IsLittleEndian) {
            Array.Reverse(gzBytes);
        }

        this.stream.Write(gzBytes, 0, 4);
    }

    private void WriteDouble(TagNodeDouble val) {
        byte[] gzBytes = BitConverter.GetBytes(val.Data);

        if(BitConverter.IsLittleEndian) {
            Array.Reverse(gzBytes);
        }

        this.stream.Write(gzBytes, 0, 8);
    }

    private void WriteByteArray(TagNodeByteArray val) {
        byte[] lenBytes = BitConverter.GetBytes(val.Length);

        if(BitConverter.IsLittleEndian) {
            Array.Reverse(lenBytes);
        }

        this.stream.Write(lenBytes, 0, 4);
        this.stream.Write(val.Data, 0, val.Length);
    }

    private void WriteString(TagNodeString val) {
        System.Text.Encoding str = Encoding.UTF8;
        byte[] gzBytes = str.GetBytes(val.Data);

        byte[] lenBytes = BitConverter.GetBytes((short) gzBytes.Length);

        if(BitConverter.IsLittleEndian) {
            Array.Reverse(lenBytes);
        }

        this.stream.Write(lenBytes, 0, 2);

        this.stream.Write(gzBytes, 0, gzBytes.Length);
    }

    private void WriteList(TagNodeList val) {
        byte[] lenBytes = BitConverter.GetBytes(val.Count);

        if(BitConverter.IsLittleEndian) {
            Array.Reverse(lenBytes);
        }

        this.stream.WriteByte((byte) val.ValueType);
        this.stream.Write(lenBytes, 0, 4);

        foreach(TagNode v in val) {
            WriteValue(v);
        }
    }

    private void WriteCompound(TagNodeCompound val) {
        foreach(KeyValuePair<string, TagNode> item in val) {
            WriteTag(item.Key, item.Value);
        }

        WriteTag(null, nulltag);
    }

    private void WriteIntArray(TagNodeIntArray val) {
        byte[] lenBytes = BitConverter.GetBytes(val.Length);

        if(BitConverter.IsLittleEndian) {
            Array.Reverse(lenBytes);
        }

        this.stream.Write(lenBytes, 0, 4);

        byte[] data = new byte[val.Length * 4];
        for(int i = 0; i < val.Length; i++) {
            byte[] buffer = BitConverter.GetBytes(val.Data[i]);
            if(BitConverter.IsLittleEndian) {
                Array.Reverse(buffer);
            }
            Array.Copy(buffer, 0, data, i * 4, 4);
        }

        this.stream.Write(data, 0, data.Length);
    }

    private void WriteLongArray(TagNodeLongArray val) {
        byte[] lenBytes = BitConverter.GetBytes(val.Length);

        if(BitConverter.IsLittleEndian) {
            Array.Reverse(lenBytes);
        }

        this.stream.Write(lenBytes, 0, 4);

        byte[] data = new byte[val.Length * 8];
        for(int i = 0; i < val.Length; i++) {
            byte[] buffer = BitConverter.GetBytes(val.Data[i]);
            if(BitConverter.IsLittleEndian) {
                Array.Reverse(buffer);
            }
            Array.Copy(buffer, 0, data, i * 8, 8);
        }

        this.stream.Write(data, 0, data.Length);
    }

    private void WriteShortArray(TagNodeShortArray val) {
        byte[] lenBytes = BitConverter.GetBytes(val.Length);

        if(BitConverter.IsLittleEndian) {
            Array.Reverse(lenBytes);
        }

        this.stream.Write(lenBytes, 0, 4);

        byte[] data = new byte[val.Length * 2];
        for(int i = 0; i < val.Length; i++) {
            byte[] buffer = BitConverter.GetBytes(val.Data[i]);
            if(BitConverter.IsLittleEndian) {
                Array.Reverse(buffer);
            }
            Array.Copy(buffer, 0, data, i * 2, 2);
        }

        this.stream.Write(data, 0, data.Length);
    }

    private void WriteTag(string name, TagNode val) {
        this.stream.WriteByte((byte) val.GetTagType());

        if(val.GetTagType() != TagType.TAG_END) {
            WriteString(name);
            WriteValue(val);
        }
    }

    #region ICopyable<NBT_Tree> Members

    /// <summary>
    /// Creates a deep copy of the NBT_Tree and underlying nodes.
    /// </summary>
    /// <returns>A new NBTtree.</returns>
    public NbtTree Copy() {
        NbtTree tree = new NbtTree();
        tree.root = this.root.Copy() as TagNodeCompound;

        return tree;
    }

    #endregion
}

// TODO: Revise exceptions?
public class NBTException : Exception {
    public const String MSG_GZIP_ENDOFSTREAM = "Gzip Error: Unexpected end of stream";

    public const String MSG_READ_NEG = "Read Error: Negative length";
    public const String MSG_READ_TYPE = "Read Error: Invalid value type";

    public NBTException() { }

    public NBTException(String msg) : base(msg) { }

    public NBTException(String msg, Exception innerException) : base(msg, innerException) { }
}

public class InvalidNBTObjectException : Exception { }

public class InvalidTagException : Exception { }

public class InvalidValueException : Exception { }
