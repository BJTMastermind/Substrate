namespace Substrate.Core;

using System.Text.RegularExpressions;
using Ionic.Zlib;

public class RegionFile : IDisposable {
    private static Regex namePattern = new Regex("r\\.(-?[0-9]+)\\.(-?[0-9]+)\\.mc[ar]$");

    private const int VERSION_GZIP = 1;
    private const int VERSION_DEFLATE = 2;

    private const int SECTOR_BYTES = 4096;
    private const int SECTOR_INTS = SECTOR_BYTES / 4;

    const int CHUNK_HEADER_SIZE = 5;

    private static byte[] emptySector = new byte[4096];

    protected string fileName;
    private FileStream file;

    /// <summary>
    /// The file lock used so that we do not seek in different areas
    /// of the same file at the same time. All file access should lock this
    /// object before moving the file pointer.
    /// The lock should also surround all access to the sectorFree free variable.
    /// </summary>
    private object fileLock = new object();

    private int[] offsets;
    private int[] chunkTimestamps;
    private List<Boolean> sectorFree;
    private int sizeDelta;
    private long lastModified = 0;

    private bool disposed = false;

    public RegionFile(string path) {
        this.offsets = new int[SectorInts];
        this.chunkTimestamps = new int[SectorInts];

        this.fileName = path;
        Debugln("REGION LOAD " + this.fileName);

        this.sizeDelta = 0;

        ReadFile();
    }

    ~RegionFile() {
        Dispose(false);
    }

    public void Dispose() {
        Dispose(true);
        System.GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
        if(!this.disposed) {
            if(disposing) {
                // Cleanup managed resources
            }

            // Cleanup unmanaged resources
            if(this.file != null) {
                lock(this.fileLock) {
                    this.file.Close();
                    this.file = null;
                }
            }
        }
        this.disposed = true;
    }

    protected void ReadFile() {
        if(this.disposed) {
            throw new ObjectDisposedException("RegionFile", "Attempting to use a RegionFile after it has been disposed.");
        }

        // Get last udpate time
        long newModified = -1;
        try {
            if(File.Exists(this.fileName)) {
                newModified = Timestamp(File.GetLastWriteTime(this.fileName));
            }
        } catch(UnauthorizedAccessException e) {
            Console.WriteLine(e.Message);
            return;
        }

        // If it hasn't been modified, we don't need to do anything
        if(newModified == this.lastModified) {
            return;
        }

        try {
            lock(this.fileLock) {
                this.file = new FileStream(this.fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

                //using(file) {
                if(this.file.Length < SectorBytes) {
                    byte[] int0 = BitConverter.GetBytes((int) 0);

                    /* we need to write the chunk offset table */
                    for(int i = 0; i < SectorInts; ++i) {
                        this.file.Write(int0, 0, 4);
                    }
                    // write another sector for the timestamp info
                    for(int i = 0; i < SectorInts; ++i) {
                        this.file.Write(int0, 0, 4);
                    }

                    this.file.Flush();

                    this.sizeDelta += SectorBytes * 2;
                }

                if((this.file.Length & 0xfff) != 0) {
                    /* the file size is not a multiple of 4KB, grow it */
                    this.file.Seek(0, SeekOrigin.End);
                    for(int i = 0; i < (this.file.Length & 0xfff); ++i) {
                        this.file.WriteByte(0);
                    }

                    this.file.Flush();
                }

                /* set up the available sector map */
                int nSectors = (int) this.file.Length / SectorBytes;
                this.sectorFree = new List<Boolean>(nSectors);

                for(int i = 0; i < nSectors; ++i) {
                    this.sectorFree.Add(true);
                }

                this.sectorFree[0] = false; // chunk offset table
                this.sectorFree[1] = false; // for the last modified info

                this.file.Seek(0, SeekOrigin.Begin);
                for(int i = 0; i < SectorInts; ++i) {
                    byte[] offsetBytes = new byte[4];
                    this.file.Read(offsetBytes, 0, 4);

                    if(BitConverter.IsLittleEndian) {
                        Array.Reverse(offsetBytes);
                    }
                    int offset = BitConverter.ToInt32(offsetBytes, 0);

                    this.offsets[i] = offset;
                    if(offset != 0 && (offset >> 8) + (offset & 0xFF) <= this.sectorFree.Count) {
                        for(int sectorNum = 0; sectorNum < (offset & 0xFF); ++sectorNum) {
                            this.sectorFree[(offset >> 8) + sectorNum] = false;
                        }
                    }
                }
                for(int i = 0; i < SectorInts; ++i) {
                    byte[] modBytes = new byte[4];
                    this.file.Read(modBytes, 0, 4);

                    if(BitConverter.IsLittleEndian) {
                        Array.Reverse(modBytes);
                    }
                    int lastModValue = BitConverter.ToInt32(modBytes, 0);

                    this.chunkTimestamps[i] = lastModValue;
                }
            }
        } catch(IOException e) {
            System.Console.WriteLine(e.Message);
            System.Console.WriteLine(e.StackTrace);
        }
    }

    /* the modification date of the region file when it was first opened */
    public long LastModified() {
        return this.lastModified;
    }

    /* gets how much the region file has grown since it was last checked */
    public int GetSizeDelta() {
        int ret = this.sizeDelta;
        this.sizeDelta = 0;
        return ret;
    }

    // various small debug printing helpers
    private void Debug(String str) {
        //        System.Consle.Write(str);
    }

    private void Debugln(String str) {
        Debug(str + "\n");
    }

    private void Debug(String mode, int x, int z, String str) {
        Debug("REGION " + mode + " " + this.fileName + "[" + x + "," + z + "] = " + str);
    }

    private void Debug(String mode, int x, int z, int count, String str) {
        Debug("REGION " + mode + " " + this.fileName + "[" + x + "," + z + "] " + count + "B = " + str);
    }

    private void Debugln(String mode, int x, int z, String str) {
        Debug(mode, x, z, str + "\n");
    }

    /*
     * gets an(uncompressed) stream representing the chunk data returns null if
     * the chunk is not found or an error occurs
     */
    public Stream GetChunkDataInputStream(int x, int z) {
        if(this.disposed) {
            throw new ObjectDisposedException("RegionFile", "Attempting to use a RegionFile after it has been disposed.");
        }

        if(OutOfBounds(x, z)) {
            Debugln("READ", x, z, "out of bounds");
            return null;
        }

        try {
            int offset = GetOffset(x, z);
            if(offset == 0) {
                // Debugln("READ", x, z, "miss");
                return null;
            }

            int sectorNumber = offset >> 8;
            int numSectors = offset & 0xFF;

            lock(this.fileLock) {
                if(sectorNumber + numSectors > this.sectorFree.Count) {
                    Debugln("READ", x, z, "invalid sector");
                    return null;
                }

                this.file.Seek(sectorNumber * SectorBytes, SeekOrigin.Begin);
                byte[] lengthBytes = new byte[4];
                this.file.Read(lengthBytes, 0, 4);

                if(BitConverter.IsLittleEndian) {
                    Array.Reverse(lengthBytes);
                }
                int length = BitConverter.ToInt32(lengthBytes, 0);

                if(length > SectorBytes * numSectors) {
                    Debugln("READ", x, z, "invalid length: " + length + " > 4096 * " + numSectors);
                    return null;
                }

                byte version = (byte) this.file.ReadByte();
                if(version == VERSION_GZIP) {
                    byte[] data = new byte[length - 1];
                    this.file.Read(data, 0, data.Length);
                    Stream ret = new GZipStream(new MemoryStream(data), CompressionMode.Decompress);

                    return ret;
                } else if(version == VERSION_DEFLATE) {
                    byte[] data = new byte[length - 1];
                    this.file.Read(data, 0, data.Length);

                    Stream ret = new ZlibStream(new MemoryStream(data), CompressionMode.Decompress, true);
                    return ret;

                    /*MemoryStream sinkZ = new MemoryStream();
                    ZlibStream zOut = new ZlibStream(sinkZ, CompressionMode.Decompress, true);
                    zOut.Write(data, 0, data.Length);
                    zOut.Flush();
                    zOut.Close();

                    sinkZ.Seek(0, SeekOrigin.Begin);
                    return sinkZ;*/
                }

                Debugln("READ", x, z, "unknown version " + version);
                return null;
            }
        } catch(IOException) {
            Debugln("READ", x, z, "exception");
            return null;
        }
    }

    public Stream GetChunkDataOutputStream(int x, int z) {
        if(OutOfBounds(x, z)) {
            return null;
        }

        return new ZlibStream(new ChunkBuffer(this, x, z), CompressionMode.Compress);
    }

    public Stream GetChunkDataOutputStream(int x, int z, int timestamp) {
        if(OutOfBounds(x, z)) {
            return null;
        }

        return new ZlibStream(new ChunkBuffer(this, x, z, timestamp), CompressionMode.Compress);
    }

    /*
     * lets chunk writing be multithreaded by not locking the whole file as a
     * chunk is serializing -- only writes when serialization is over
     */
    class ChunkBuffer : MemoryStream {
        private int x, z;
        private RegionFile region;

        private int? timestamp;

        public ChunkBuffer(RegionFile r, int x, int z) : base(8096) {
            this.region = r;
            this.x = x;
            this.z = z;
        }

        public ChunkBuffer(RegionFile r, int x, int z, int timestamp) : this(r, x, z) {
            this.timestamp = timestamp;
        }

        public override void Close() {
            if(this.timestamp == null) {
                this.region.Write(this.x, this.z, this.GetBuffer(), (int) this.Length);
            } else {
                this.region.Write(this.x, this.z, this.GetBuffer(), (int) this.Length, (int) this.timestamp);
            }
        }
    }

    protected void Write(int x, int z, byte[] data, int length) {
        Write(x, z, data, length, Timestamp());
    }

    /* write a chunk at(x,z) with length bytes of data to disk */
    protected void Write(int x, int z, byte[] data, int length, int timestamp) {
        if(this.disposed) {
            throw new ObjectDisposedException("RegionFile", "Attempting to use a RegionFile after it has been disposed.");
        }

        try {
            int offset = GetOffset(x, z);
            int sectorNumber = offset >> 8;
            int sectorsAllocated = offset & 0xFF;
            int sectorsNeeded = (length + CHUNK_HEADER_SIZE) / SectorBytes + 1;

            // maximum chunk size is 1MB
            if(sectorsNeeded >= 256) {
                return;
            }

            if(sectorNumber != 0 && sectorsAllocated == sectorsNeeded) {
                /* we can simply overwrite the old sectors */
                Debug("SAVE", x, z, length, "rewrite");
                Write(sectorNumber, data, length);
            } else {
                /* we need to allocate new sectors */

                lock(this.fileLock) {
                    /* mark the sectors previously used for this chunk as free */
                    for(int i = 0; i < sectorsAllocated; ++i) {
                        this.sectorFree[sectorNumber + i] = true;
                    }

                    /* scan for a free space large enough to store this chunk */
                    int runStart = this.sectorFree.FindIndex(b => b == true);
                    int runLength = 0;
                    if(runStart != -1) {
                        for(int i = runStart; i < this.sectorFree.Count; ++i) {
                            if(runLength != 0) {
                                if(this.sectorFree[i]) {
                                    runLength++;
                                } else {
                                    runLength = 0;
                                }
                            } else if(this.sectorFree[i]) {
                                runStart = i;
                                runLength = 1;
                            }
                            if(runLength >= sectorsNeeded) {
                                break;
                            }
                        }
                    }

                    if(runLength >= sectorsNeeded) {
                        /* we found a free space large enough */
                        Debug("SAVE", x, z, length, "reuse");
                        sectorNumber = runStart;
                        SetOffset(x, z, (sectorNumber << 8) | sectorsNeeded);
                        for(int i = 0; i < sectorsNeeded; ++i) {
                            this.sectorFree[sectorNumber + i] = false;
                        }
                        Write(sectorNumber, data, length);
                    } else {
                        /*
                         * no free space large enough found -- we need to grow the
                         * file
                         */
                        Debug("SAVE", x, z, length, "grow");
                        this.file.Seek(0, SeekOrigin.End);
                        sectorNumber = this.sectorFree.Count;
                        for(int i = 0; i < sectorsNeeded; ++i) {
                            this.file.Write(emptySector, 0, emptySector.Length);
                            this.sectorFree.Add(false);
                        }
                        this.sizeDelta += SectorBytes * sectorsNeeded;

                        Write(sectorNumber, data, length);
                        SetOffset(x, z, (sectorNumber << 8) | sectorsNeeded);
                    }
                }
            }
            SetTimestamp(x, z, timestamp);
        } catch(IOException e) {
            Console.WriteLine(e.StackTrace);
        }
    }

    /* write a chunk data to the region file at specified sector number */
    private void Write(int sectorNumber, byte[] data, int length) {
        lock(this.fileLock) {
            Debugln(" " + sectorNumber);
            this.file.Seek(sectorNumber * SectorBytes, SeekOrigin.Begin);

            byte[] bytes = BitConverter.GetBytes(length + 1);
            if(BitConverter.IsLittleEndian) {
                Array.Reverse(bytes);
            }
            this.file.Write(bytes, 0, 4); // chunk length
            this.file.WriteByte(VERSION_DEFLATE); // chunk version number
            this.file.Write(data, 0, length); // chunk data
        }
    }

    public void DeleteChunk(int x, int z) {
        lock(this.fileLock) {
            int offset = GetOffset(x, z);
            int sectorNumber = offset >> 8;
            int sectorsAllocated = offset & 0xFF;

            this.file.Seek(sectorNumber * SectorBytes, SeekOrigin.Begin);
            for(int i = 0; i < sectorsAllocated; i++) {
                this.file.Write(emptySector, 0, SectorBytes);
            }

            SetOffset(x, z, 0);
            SetTimestamp(x, z, 0);
        }
    }

    /* is this an invalid chunk coordinate? */
    private bool OutOfBounds(int x, int z) {
        return x < 0 || x >= 32 || z < 0 || z >= 32;
    }

    private int GetOffset(int x, int z) {
        return this.offsets[x + z * 32];
    }

    public bool HasChunk(int x, int z) {
        return GetOffset(x, z) != 0;
    }

    private void SetOffset(int x, int z, int offset) {
        lock(this.fileLock) {
            this.offsets[x + z * 32] = offset;
            this.file.Seek((x + z * 32) * 4, SeekOrigin.Begin);

            byte[] bytes = BitConverter.GetBytes(offset);
            if(BitConverter.IsLittleEndian) {
                Array.Reverse(bytes);
            }
            this.file.Write(bytes, 0, 4);
        }
    }

    private int Timestamp() {
        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return (int) ((DateTime.UtcNow - epoch).Ticks / (10000L * 1000L));
    }

    private int Timestamp(DateTime time) {
        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return (int) ((time - epoch).Ticks / (10000L * 1000L));
    }

    public int GetTimestamp(int x, int z) {
        return this.chunkTimestamps[x + z * 32];
    }

    public void SetTimestamp(int x, int z, int value) {
        lock(this.fileLock) {
            this.chunkTimestamps[x + z * 32] = value;
            this.file.Seek(SectorBytes + (x + z * 32) * 4, SeekOrigin.Begin);

            byte[] bytes = BitConverter.GetBytes(value);
            if(BitConverter.IsLittleEndian) {
                Array.Reverse(bytes);
            }

            this.file.Write(bytes, 0, 4);
        }
    }

    public void Close() {
        lock(this.fileLock) {
            this.file.Close();
        }
    }

    public virtual RegionKey parseCoordinatesFromName() {
        int x = 0;
        int z = 0;

        Match match = namePattern.Match(this.fileName);
        if(!match.Success) {
            return RegionKey.InvalidRegion;
        }

        x = Int32.Parse(match.Groups[1].Value);
        z = Int32.Parse(match.Groups[2].Value);

        return new RegionKey(x, z);
    }

    protected virtual int SectorBytes {
        get { return SECTOR_BYTES; }
    }

    protected virtual int SectorInts {
        get { return SECTOR_BYTES / 4; }
    }

    protected virtual byte[] EmptySector {
        get { return emptySector; }
    }
}
