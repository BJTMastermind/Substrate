namespace Substrate.Core;

public class CompositeDataArray3 : IDataArray3 {
    private IDataArray3[] sections;

    public CompositeDataArray3(IDataArray3[] sections) {
        for(int i = 0; i < sections.Length; i++) {
            if(sections[i] == null) {
                throw new ArgumentException("sections argument cannot have null entries.");
            }
        }

        for(int i = 0; i < sections.Length; i++) {
            if(sections[i].Length != sections[0].Length
                || sections[i].XDim != sections[0].XDim
                || sections[i].YDim != sections[0].YDim
                || sections[i].ZDim != sections[0].ZDim) {
                throw new ArgumentException("All elements in sections argument must have same metrics.");
            }
        }
        this.sections = sections;
    }

    #region IByteArray3 Members
    public int this[int x, int y, int z] {
        get {
            int ydiv = y / this.sections[0].YDim;
            int yrem = y - (ydiv * this.sections[0].YDim);
            return this.sections[ydiv][x, yrem, z];
        }
        set {
            int ydiv = y / this.sections[0].YDim;
            int yrem = y - (ydiv * this.sections[0].YDim);
            this.sections[ydiv][x, yrem, z] = value;
        }
    }

    public int XDim {
        get { return this.sections[0].XDim; }
    }

    public int YDim {
        get { return this.sections[0].YDim * this.sections.Length; }
    }

    public int ZDim {
        get { return this.sections[0].ZDim; }
    }

    public int GetIndex(int x, int y, int z) {
        int ydiv = y / this.sections[0].YDim;
        int yrem = y - (ydiv * this.sections[0].YDim);
        return (ydiv * this.sections[0].Length) + this.sections[ydiv].GetIndex(x, yrem, z);
    }

    public void GetMultiIndex(int index, out int x, out int y, out int z) {
        int idiv = index / this.sections[0].Length;
        int irem = index - (idiv * this.sections[0].Length);
        this.sections[idiv].GetMultiIndex(irem, out x, out y, out z);
        y += idiv * this.sections[0].YDim;
    }
    #endregion

    #region IByteArray Members
    public int this[int i] {
        get {
            int idiv = i / this.sections[0].Length;
            int irem = i - (idiv * this.sections[0].Length);
            return this.sections[idiv][irem];
        }
        set {
            int idiv = i / this.sections[0].Length;
            int irem = i - (idiv * this.sections[0].Length);
            this.sections[idiv][irem] = value;
        }
    }

    public int Length {
        get { return this.sections[0].Length * this.sections.Length; }
    }

    public int DataWidth {
        get { return this.sections[0].DataWidth; }
    }

    public void Clear() {
        for(int i = 0; i < this.sections.Length; i++) {
            this.sections[i].Clear();
        }
    }
    #endregion
}
