namespace Substrate.Core;

using System.Collections;
using System.Text.RegularExpressions;

/// <summary>
/// Manages the regions of a Beta-compatible world.
/// </summary>
public abstract class RegionManager : IRegionManager {
    protected string regionPath;

    protected Dictionary<RegionKey, IRegion> cache;

    protected ChunkCache chunkCache;

    protected abstract IRegion CreateRegionCore(int rx, int rz);

    protected abstract RegionFile CreateRegionFileCore(int rx, int rz);

    protected abstract void DeleteRegionCore(IRegion region);

    public abstract IRegion GetRegion(string filename);

    /// <summary>
    /// Creates a new instance of a <see cref="RegionManager"/> for the given region directory and chunk cache.
    /// </summary>
    /// <param name="regionDir">The path to a directory containing region files.</param>
    /// <param name="cache">The shared chunk cache to hold chunk data in.</param>
    public RegionManager(string regionDir, ChunkCache cache) {
        this.regionPath = regionDir;
        this.chunkCache = cache;
        this.cache = new Dictionary<RegionKey, IRegion>();
    }

    /// <inherits />
    public IRegion GetRegion(int rx, int rz) {
        RegionKey k = new RegionKey(rx, rz);
        IRegion r;

        try {
            if(this.cache.TryGetValue(k, out r) == false) {
                r = CreateRegionCore(rx, rz);
                this.cache.Add(k, r);
            }
            return r;
        } catch(FileNotFoundException) {
            this.cache.Add(k, null);
            return null;
        }
    }

    /// <inherits />
    public bool RegionExists(int rx, int rz) {
        IRegion r = GetRegion(rx, rz);
        return r != null;
    }

    /// <inherits />
    public IRegion CreateRegion(int rx, int rz) {
        IRegion r = GetRegion(rx, rz);
        if(r == null) {
            string fp = "r." + rx + "." + rz + ".mca";
            using(RegionFile rf = CreateRegionFileCore(rx, rz)) {

            }

            r = CreateRegionCore(rx, rz);

            RegionKey k = new RegionKey(rx, rz);
            this.cache[k] = r;
        }

        return r;
    }

    /// <summary>
    /// Get the current region directory path.
    /// </summary>
    /// <returns>The path to the region directory.</returns>
    public string GetRegionPath() {
        return this.regionPath;
    }

    // XXX: Exceptions
    /// <inherits />
    public bool DeleteRegion(int rx, int rz) {
        IRegion r = GetRegion(rx, rz);
        if(r == null) {
            return false;
        }

        RegionKey k = new RegionKey(rx, rz);
        this.cache.Remove(k);

        DeleteRegionCore(r);

        try {
            File.Delete(r.GetFilePath());
        } catch(Exception e) {
            Console.WriteLine("NOTICE: " + e.Message);
            return false;
        }

        return true;
    }

    #region IEnumerable<IRegion> Members
    /// <summary>
    /// Returns an enumerator that iterates over all of the regions in the underlying dimension.
    /// </summary>
    /// <returns>An enumerator instance.</returns>
    public IEnumerator<IRegion> GetEnumerator() {
        return new Enumerator(this);
    }
    #endregion

    #region IEnumerable Members
    /// <summary>
    /// Returns an enumerator that iterates over all of the regions in the underlying dimension.
    /// </summary>
    /// <returns>An enumerator instance.</returns>
    IEnumerator IEnumerable.GetEnumerator() {
        return new Enumerator(this);
    }
    #endregion

    private struct Enumerator : IEnumerator<IRegion> {
        private List<IRegion> regions;
        private int pos;

        public Enumerator(RegionManager rm) {
            this.regions = new List<IRegion>();
            this.pos = -1;

            if(!Directory.Exists(rm.GetRegionPath())) {
                throw new DirectoryNotFoundException();
            }

            List<string> files = new List<string>(Directory.GetFiles(rm.GetRegionPath()));
            this.regions.Capacity = files.Count;

            files.Sort(RegionSort);

            foreach(string file in files) {
                try {
                    IRegion r = rm.GetRegion(file);
                    this.regions.Add(r);
                } catch(ArgumentException) {
                    continue;
                }
            }
        }

        public bool MoveNext() {
            this.pos++;
            return (this.pos < this.regions.Count);
        }

        public void Reset() {
            this.pos = -1;
        }

        void IDisposable.Dispose() { }

        object IEnumerator.Current {
            get { return Current; }
        }

        IRegion IEnumerator<IRegion>.Current {
            get { return Current; }
        }

        public IRegion Current {
            get {
                try {
                    return this.regions[this.pos];
                } catch(IndexOutOfRangeException) {
                    throw new InvalidOperationException();
                }
            }
        }

        private int RegionSort(string A, string B) {
            Regex R = new Regex(".+r\\.(?<x>-?\\d+)\\.(?<y>-?\\d+)\\.(mca|mcr)", RegexOptions.None);
            Match MC = R.Match(A);
            if(!MC.Success) {
                return 0;
            }

            int AX = int.Parse(MC.Groups["x"].Value);
            int AZ = int.Parse(MC.Groups["y"].Value);

            MC = R.Match(B);
            if(!MC.Success) {
                return 0;
            }

            int BX = int.Parse(MC.Groups["x"].Value);
            int BZ = int.Parse(MC.Groups["y"].Value);

            if(AZ < BZ) {
                return -1;
            }

            if(AZ > BZ) {
                return 1;
            }

            if(AX < BX) {
                return -1;
            }

            if(AX > BX) {
                return 1;
            }
            return 0;
        }
    }
}
