﻿namespace Substrate;

using System.Text.RegularExpressions;
using Substrate.Core;
using Substrate.Nbt;

public class BetaRegion : Region {
    private static Regex namePattern = new Regex("r\\.(-?[0-9]+)\\.(-?[0-9]+)\\.mcr$");

    public BetaRegion(BetaRegionManager rm, ChunkCache cache, int rx, int rz) : base(rm, cache, rx, rz) { }

    /// <inherits />
    public override string GetFileName() {
        return "r." + this.rx + "." + this.rz + ".mcr";
    }

    /// <inherits />
    public override string GetFilePath() {
        return System.IO.Path.Combine(this.regionMan.GetRegionPath(), GetFileName());
    }

    /// <summary>
    /// Tests if the given filename conforms to the general naming pattern for any region.
    /// </summary>
    /// <param name="filename">The filename to test.</param>
    /// <returns>True if the filename is a valid region name; false if it does not conform to the pattern.</returns>
    public static bool TestFileName(string filename) {
        Match match = namePattern.Match(filename);
        if(!match.Success) {
            return false;
        }
        return true;
    }

    public static bool ParseFileName(string filename, out int x, out int z) {
        x = 0;
        z = 0;

        Match match = namePattern.Match(filename);
        if(!match.Success) {
            return false;
        }

        x = Int32.Parse(match.Groups[1].Value);
        z = Int32.Parse(match.Groups[2].Value);
        return true;
    }

    /// <summary>
    /// Parses the given filename to extract encoded region coordinates.
    /// </summary>
    /// <param name="filename">The region filename to parse.</param>
    /// <param name="x">This parameter will contain the X-coordinate of a region.</param>
    /// <param name="z">This parameter will contain the Z-coordinate of a region.</param>
    /// <returns>True if the filename could be correctly parse; false otherwise.</returns>
    protected override bool ParseFileNameCore(string filename, out int x, out int z) {
        return ParseFileName(filename, out x, out z);
    }

    protected override IChunk CreateChunkCore(int cx, int cz) {
        return AlphaChunk.Create(cx, cz);
    }

    protected override IChunk CreateChunkVerifiedCore(NbtTree tree) {
        return AlphaChunk.CreateVerified(tree);
    }
}
