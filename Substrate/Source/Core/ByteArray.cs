namespace Substrate.Core;

public interface IDataArray {
    int this[int i] { get; set; }
    int Length { get; }
    int DataWidth { get; }

    void Clear();
}

public interface IDataArray2 : IDataArray {
    int this[int x, int z] { get; set; }

    int XDim { get; }
    int ZDim { get; }
}

public interface IDataArray3 : IDataArray {
    int this[int x, int y, int z] { get; set; }

    int XDim { get; }
    int YDim { get; }
    int ZDim { get; }

    int GetIndex(int x, int y, int z);
    void GetMultiIndex(int index, out int x, out int y, out int z);
}

public class ByteArray : IDataArray, ICopyable<ByteArray> {
    protected readonly byte[] dataArray;

    public ByteArray(int length) {
        this.dataArray = new byte[length];
    }

    public ByteArray(byte[] data) {
        this.dataArray = data;
    }

    public int this[int i] {
        get { return this.dataArray[i]; }
        set { this.dataArray[i] = (byte) value; }
    }

    public int Length {
        get { return this.dataArray.Length; }
    }

    public int DataWidth {
        get { return 8; }
    }

    public void Clear() {
        for(int i = 0; i < this.dataArray.Length; i++) {
            this.dataArray[i] = 0;
        }
    }

    #region ICopyable<ByteArray> Members
    public virtual ByteArray Copy() {
        byte[] data = new byte[this.dataArray.Length];
        this.dataArray.CopyTo(data, 0);

        return new ByteArray(data);
    }
    #endregion
}

public sealed class XZYByteArray : ByteArray, IDataArray3 {
    private readonly int xdim;
    private readonly int ydim;
    private readonly int zdim;

    public XZYByteArray(int xdim, int ydim, int zdim) : base(xdim * ydim * zdim) {
        this.xdim = xdim;
        this.ydim = ydim;
        this.zdim = zdim;
    }

    public XZYByteArray(int xdim, int ydim, int zdim, byte[] data) : base(data) {
        this.xdim = xdim;
        this.ydim = ydim;
        this.zdim = zdim;

        if(xdim * ydim * zdim != data.Length) {
            throw new ArgumentException("Product of dimensions must equal length of data");
        }
    }

    public int this[int x, int y, int z] {
        get {
            int index = this.ydim * (x * this.zdim + z) + y;
            return this.dataArray[index];
        }
        set {
            int index = this.ydim * (x * this.zdim + z) + y;
            this.dataArray[index] = (byte) value;
        }
    }

    public int XDim {
        get { return this.xdim; }
    }

    public int YDim {
        get { return this.ydim; }
    }

    public int ZDim {
        get { return this.zdim; }
    }

    public int GetIndex(int x, int y, int z) {
        return this.ydim * (x * this.zdim + z) + y;
    }

    public void GetMultiIndex(int index, out int x, out int y, out int z) {
        int yzdim = this.ydim * this.zdim;
        x = index / yzdim;

        int zy = index - (x * yzdim);
        z = zy / this.ydim;
        y = zy - (z * this.ydim);
    }

    #region ICopyable<XZYByteArray> Members
    public override ByteArray Copy() {
        byte[] data = new byte[this.dataArray.Length];
        this.dataArray.CopyTo(data, 0);

        return new XZYByteArray(this.xdim, this.ydim, this.zdim, data);
    }
    #endregion
}

public sealed class YZXByteArray : ByteArray, IDataArray3 {
    private readonly int xdim;
    private readonly int ydim;
    private readonly int zdim;

    public YZXByteArray(int xdim, int ydim, int zdim) : base(xdim * ydim * zdim) {
        this.xdim = xdim;
        this.ydim = ydim;
        this.zdim = zdim;
    }

    public YZXByteArray(int xdim, int ydim, int zdim, byte[] data) : base(data) {
        this.xdim = xdim;
        this.ydim = ydim;
        this.zdim = zdim;

        if(xdim * ydim * zdim != data.Length) {
            throw new ArgumentException("Product of dimensions must equal length of data");
        }
    }

    public int this[int x, int y, int z] {
        get {
            int index = this.xdim * (y * this.zdim + z) + x;
            return this.dataArray[index];
        }

        set {
            int index = this.xdim * (y * this.zdim + z) + x;
            this.dataArray[index] = (byte) value;
        }
    }

    public int XDim {
        get { return this.xdim; }
    }

    public int YDim {
        get { return this.ydim; }
    }

    public int ZDim {
        get { return this.zdim; }
    }

    public int GetIndex(int x, int y, int z) {
        return this.xdim * (y * this.zdim + z) + x;
    }

    public void GetMultiIndex(int index, out int x, out int y, out int z) {
        int xzdim = this.xdim * this.zdim;
        y = index / xzdim;

        int zx = index - (y * xzdim);
        z = zx / this.xdim;
        x = zx - (z * this.xdim);
    }

    #region ICopyable<YZXByteArray> Members
    public override ByteArray Copy() {
        byte[] data = new byte[this.dataArray.Length];
        this.dataArray.CopyTo(data, 0);

        return new YZXByteArray(this.xdim, this.ydim, this.zdim, data);
    }
    #endregion
}

public sealed class ZXByteArray : ByteArray, IDataArray2 {
    private readonly int xdim;
    private readonly int zdim;

    public ZXByteArray(int xdim, int zdim) : base(xdim * zdim) {
        this.xdim = xdim;
        this.zdim = zdim;
    }

    public ZXByteArray(int xdim, int zdim, byte[] data) : base(data) {
        this.xdim = xdim;
        this.zdim = zdim;

        if(xdim * zdim != data.Length) {
            throw new ArgumentException("Product of dimensions must equal length of data");
        }
    }

    public int this[int x, int z] {
        get {
            int index = z * this.xdim + x;
            return this.dataArray[index];
        }

        set {
            int index = z * this.xdim + x;
            this.dataArray[index] = (byte) value;
        }
    }

    public int XDim {
        get { return this.xdim; }
    }

    public int ZDim {
        get { return this.zdim; }
    }

    #region ICopyable<ZXByteArray> Members
    public override ByteArray Copy() {
        byte[] data = new byte[this.dataArray.Length];
        this.dataArray.CopyTo(data, 0);

        return new ZXByteArray(this.xdim, this.zdim, data);
    }
    #endregion
}

public class IntArray : IDataArray, ICopyable<IntArray> {
    protected readonly int[] dataArray;

    public IntArray(int length) {
        this.dataArray = new int[length];
    }

    public IntArray(int[] data) {
        this.dataArray = data;
    }

    public int this[int i] {
        get { return this.dataArray[i]; }
        set { this.dataArray[i] = value; }
    }

    public int Length {
        get { return this.dataArray.Length; }
    }

    public int DataWidth {
        get { return 32; }
    }

    public void Clear() {
        for(int i = 0; i < this.dataArray.Length; i++) {
            this.dataArray[i] = 0;
        }
    }

    #region ICopyable<ByteArray> Members
    public virtual IntArray Copy() {
        int[] data = new int[this.dataArray.Length];
        this.dataArray.CopyTo(data, 0);

        return new IntArray(data);
    }
    #endregion
}

public sealed class ZXIntArray : IntArray, IDataArray2 {
    private readonly int xdim;
    private readonly int zdim;

    public ZXIntArray(int xdim, int zdim) : base(xdim * zdim) {
        this.xdim = xdim;
        this.zdim = zdim;
    }

    public ZXIntArray(int xdim, int zdim, int[] data) : base(data) {
        this.xdim = xdim;
        this.zdim = zdim;

        if(xdim * zdim != data.Length) {
            throw new ArgumentException("Product of dimensions must equal length of data");
        }
    }

    public int this[int x, int z] {
        get {
            int index = z * this.xdim + x;
            return this.dataArray[index];
        }

        set {
            int index = z * this.xdim + x;
            this.dataArray[index] = value;
        }
    }

    public int XDim {
        get { return this.xdim; }
    }

    public int ZDim {
        get { return this.zdim; }
    }

    #region ICopyable<ZXByteArray> Members
    public override IntArray Copy() {
        int[] data = new int[this.dataArray.Length];
        this.dataArray.CopyTo(data, 0);

        return new ZXIntArray(this.xdim, this.zdim, data);
    }
    #endregion
}
