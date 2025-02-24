﻿namespace Substrate.Tests;

using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Substrate;

[TestClass]
public class BlockTests {
    private int Y = 70;
    private int MinX = 1;
    private int MaxX = 180;
    private int MinZ = 1;
    private int MaxZ = 180;

    [TestMethod]
    public void BlockTest_1_8_3_debug() {
        NbtWorld world = NbtWorld.Open(@"../../../Data/1_8_3-debug/");
        Assert.IsNotNull(world);

        for(int x = this.MinX; x < this.MaxX; x += 2) {
            for(int z = this.MinZ; z < this.MaxZ; z += 2) {
                var blockRef = world.GetBlockManager().GetBlockRef(x, this.Y, z);
                var blockInfo = BlockInfo.BlockTable[blockRef.ID];

                Debug.WriteLine(String.Format("ID:{0} ({1}), Data:{2}", blockRef.ID, blockInfo.Name, blockRef.Data));

                Assert.IsTrue(blockInfo.Registered, "Block ID {0} has not been registered", blockRef.ID);
                Assert.IsTrue(blockInfo.TestData(blockRef.Data), "Data value '0x{0:X4}' not recognised for block '{1}' at {2},{3}", blockRef.Data, blockInfo.Name, x, z);
            }
        }
    }

    [TestMethod]
    public void BlockTest_1_9_2_debug() {
        NbtWorld world = NbtWorld.Open(@"../../../Data/1_9_2-debug/");
        Assert.IsNotNull(world);

        for(int x = this.MinX; x < this.MaxX; x += 2) {
            for(int z = this.MinZ; z < this.MaxZ; z += 2) {
                var blockRef = world.GetBlockManager().GetBlockRef(x, this.Y, z);
                var blockInfo = BlockInfo.BlockTable[blockRef.ID];

                Debug.WriteLine(String.Format("ID:{0} ({1}), Data:{2}", blockRef.ID, blockInfo.Name, blockRef.Data));

                Assert.IsTrue(blockInfo.Registered, "Block ID {0} has not been registered", blockRef.ID);
                if(!blockInfo.TestData(blockRef.Data)) {
                    Debug.WriteLine("Data value '0x{0:X4}' not recognised for block '{1}' at {2},{3}", blockRef.Data, blockInfo.Name, x, z);
                }
            }
        }
    }

    [TestMethod]
    public void BlockTest_1_12_2_debug() {
        NbtWorld world = NbtWorld.Open(@"../../../Data/1_12_2-debug/");
        Assert.IsNotNull(world);

        for(int x = this.MinX; x < this.MaxX; x += 2) {
            for(int z = this.MinZ; z < this.MaxZ; z += 2) {
                var blockRef = world.GetBlockManager().GetBlockRef(x, this.Y, z);
                var blockInfo = BlockInfo.BlockTable[blockRef.ID];

                Debug.WriteLine(String.Format("ID:{0} ({1}), Data:{2}", blockRef.ID, blockInfo.Name, blockRef.Data));

                Assert.IsTrue(blockInfo.Registered, "Block ID {0} has not been registered", blockRef.ID);
                if(!blockInfo.TestData(blockRef.Data)) {
                    Debug.WriteLine("Data value '0x{0:X4}' not recognised for block '{1}' at {2},{3}", blockRef.Data, blockInfo.Name, x, z);
                }
            }
        }
    }
}
