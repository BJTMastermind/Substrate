﻿namespace Substrate;

using Substrate.Core;

public class AnvilRegionManager : RegionManager {
    public AnvilRegionManager(string regionDir, ChunkCache cache) : base(regionDir, cache) { }

    protected override IRegion CreateRegionCore(int rx, int rz) {
        return new AnvilRegion(this, this.chunkCache, rx, rz);
    }

    protected override RegionFile CreateRegionFileCore(int rx, int rz) {
        string fp = "r." + rx + "." + rz + ".mca";
        return new RegionFile(Path.Combine(this.regionPath, fp));
    }

    protected override void DeleteRegionCore(IRegion region) {
        AnvilRegion r = region as AnvilRegion;
        if(r != null) {
            r.Dispose();
        }
    }

    public override IRegion GetRegion(string filename) {
        int rx, rz;
        if(!AnvilRegion.ParseFileName(filename, out rx, out rz)) {
            throw new ArgumentException("Malformed region file name: " + filename, "filename");
        }
        return GetRegion(rx, rz);
    }
}
