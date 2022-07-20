// This example is a tool to delete all entities of a given type (e.g., "pig")
// on a map.  It optionally can be restricted to boxed region in block coords.
// Only 10% of the effort is actually spend purging anything.
namespace PurgeEntities;

using System;
using Substrate;
using Substrate.Core;

class Program {
    static void Main(string[] args) {
        // Process arguments
        if(args.Length != 2 && args.Length != 6) {
            Console.WriteLine("Usage: PurgeEntities <world> <entityID> [<x1> <z1> <x2> <z2>]");
            return;
        }
        string dest = args[0];
        string eid = args[1];

        // Our initial bounding box is "infinite"
        int x1 = BlockManager.MIN_X;
        int x2 = BlockManager.MAX_X;
        int z1 = BlockManager.MIN_Z;
        int z2 = BlockManager.MAX_Z;

        // If we have all coordinate parameters, set the bounding box
        if(args.Length == 6) {
            x1 = Int32.Parse(args[2]);
            z1 = Int32.Parse(args[3]);
            x2 = Int32.Parse(args[4]);
            z2 = Int32.Parse(args[5]);
        }

        // Load world
        NbtWorld world = NbtWorld.Open(dest);
        IChunkManager cm = world.GetChunkManager();

        // Remove entities
        foreach(ChunkRef chunk in cm) {
            // Skip chunks that don't cover our selected area
            if(((chunk.X + 1) * chunk.Blocks.XDim < x1) ||
                (chunk.X * chunk.Blocks.XDim >= x2) ||
                ((chunk.Z + 1) * chunk.Blocks.ZDim < z1) ||
                (chunk.Z * chunk.Blocks.ZDim >= z2)) {
                continue;
            }

            // Delete the specified entities
            chunk.Entities.RemoveAll(eid);
            cm.Save();
        }
    }
}
