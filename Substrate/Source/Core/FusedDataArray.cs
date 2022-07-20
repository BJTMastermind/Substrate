namespace Substrate.Core;

public class FusedDataArray3 : IDataArray3 {
    private IDataArray3 array0;
    private IDataArray3 array1;

    private int mask1;

    public FusedDataArray3(IDataArray3 array0, IDataArray3 array1) {
        if(array0 == null || array1 == null) {
            throw new ArgumentException("arguments cannot be null");
        }

        if(array0.XDim != array1.XDim || array0.YDim != array1.YDim || array0.ZDim != array1.ZDim) {
            throw new ArgumentException("array0 and array1 must have matching dimensions");
        }

        this.array0 = array0;
        this.array1 = array1;

        this.mask1 = (1 << this.array1.DataWidth) - 1;
    }

    public int this[int x, int y, int z] {
        get { return (this.array0[x, y, z] << this.array1.DataWidth) + this.array1[x, y, z]; }
        set {
            this.array0[x, y, z] = value >> this.array1.DataWidth;
            this.array1[x, y, z] = value & this.mask1;
        }
    }

    public int XDim {
        get { return this.array1.XDim; }
    }

    public int YDim {
        get { return this.array1.YDim; }
    }

    public int ZDim {
        get { return this.array1.ZDim; }
    }

    public int GetIndex(int x, int y, int z) {
        return this.array1.GetIndex(x, y, z);
    }

    public void GetMultiIndex(int index, out int x, out int y, out int z) {
        this.array1.GetMultiIndex(index, out x, out y, out z);
    }

    public int this[int i] {
        get { return (this.array0[i] << this.array1.DataWidth) + this.array1[i]; }
        set {
            this.array0[i] = value >> this.array1.DataWidth;
            this.array1[i] = value & this.mask1;
        }
    }

    public int Length {
        get { return this.array1.Length; }
    }

    public int DataWidth {
        get { return this.array0.DataWidth + this.array1.DataWidth; }
    }

    public void Clear() {
        this.array0.Clear();
        this.array1.Clear();
    }
}
