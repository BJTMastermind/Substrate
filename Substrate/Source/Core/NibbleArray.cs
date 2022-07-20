namespace Substrate.Core;

public class NibbleArray : IDataArray, ICopyable<NibbleArray> {
    private readonly byte[] data = null;

    public NibbleArray(int length) {
        this.data = new byte[(int) Math.Ceiling(length / 2.0)];
    }

    public NibbleArray(byte[] data) {
        this.data = data;
    }

    public int this[int index] {
        get {
            int subs = index >> 1;
            if((index & 1) == 0) {
                return (byte) (this.data[subs] & 0x0F);
            } else {
                return (byte) ((this.data[subs] >> 4) & 0x0F);
            }
        }
        set {
            int subs = index >> 1;
            if((index & 1) == 0) {
                this.data[subs] = (byte) ((this.data[subs] & 0xF0) | (value & 0x0F));
            } else {
                this.data[subs] = (byte) ((this.data[subs] & 0x0F) | ((value & 0x0F) << 4));
            }
        }
    }

    public int Length {
        get { return this.data.Length << 1; }
    }

    public int DataWidth {
        get { return 4; }
    }

    protected byte[] Data {
        get { return this.data; }
    }

    public void Clear() {
        for(int i = 0; i < this.data.Length; i++) {
            this.data[i] = 0;
        }
    }

    #region ICopyable<NibbleArray> Members
    public virtual NibbleArray Copy() {
        byte[] data = new byte[this.data.Length];
        this.data.CopyTo(data, 0);

        return new NibbleArray(data);
    }
    #endregion
}

public sealed class XZYNibbleArray : NibbleArray, IDataArray3 {
    private readonly int xdim;
    private readonly int ydim;
    private readonly int zdim;

    public XZYNibbleArray(int xdim, int ydim, int zdim) : base(xdim * ydim * zdim) {
        this.xdim = xdim;
        this.ydim = ydim;
        this.zdim = zdim;
    }

    public XZYNibbleArray(int xdim, int ydim, int zdim, byte[] data) : base(data) {
        this.xdim = xdim;
        this.ydim = ydim;
        this.zdim = zdim;

        if(xdim * ydim * zdim != data.Length * 2) {
            throw new ArgumentException("Product of dimensions must equal half length of raw data");
        }
    }

    public int this[int x, int y, int z] {
        get {
            int index = this.ydim * (x * this.zdim + z) + y;
            return this[index];
        }
        set {
            int index = this.ydim * (x * this.zdim + z) + y;
            this[index] = value;
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

    #region ICopyable<NibbleArray> Members
    public override NibbleArray Copy() {
        byte[] data = new byte[Data.Length];
        Data.CopyTo(data, 0);

        return new XZYNibbleArray(this.xdim, this.ydim, this.zdim, data);
    }
    #endregion
}

public sealed class YZXNibbleArray : NibbleArray, IDataArray3 {
    private readonly int xdim;
    private readonly int ydim;
    private readonly int zdim;

    public YZXNibbleArray(int xdim, int ydim, int zdim) : base(xdim * ydim * zdim) {
        this.xdim = xdim;
        this.ydim = ydim;
        this.zdim = zdim;
    }

    public YZXNibbleArray(int xdim, int ydim, int zdim, byte[] data) : base(data) {
        this.xdim = xdim;
        this.ydim = ydim;
        this.zdim = zdim;

        if(xdim * ydim * zdim != data.Length * 2) {
            throw new ArgumentException("Product of dimensions must equal half length of raw data");
        }
    }

    public int this[int x, int y, int z] {
        get {
            int index = this.xdim * (y * this.zdim + z) + x;
            return this[index];
        }
        set {
            int index = this.xdim * (y * this.zdim + z) + x;
            this[index] = value;
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

    #region ICopyable<NibbleArray> Members
    public override NibbleArray Copy() {
        byte[] data = new byte[Data.Length];
        Data.CopyTo(data, 0);

        return new YZXNibbleArray(this.xdim, this.ydim, this.zdim, data);
    }
    #endregion
}
