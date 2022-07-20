﻿namespace Substrate.Data;

using Substrate.Core;

public class MapFile : NBTFile {
    public MapFile(string path) : base(path) { }

    public MapFile(string path, int id) : base("") {
        if(!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }

        string file = "map_" + id + ".dat";
        FileName = Path.Combine(path, file);
    }

    public static int IdFromFilename(string filename) {
        if(filename.EndsWith(".dat")) {
            return Int32.Parse(filename.Substring(4).Remove(filename.Length - 4));
        }

        throw new FormatException("Filename '" + filename + "' is not a .dat file");
    }
}
