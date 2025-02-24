﻿namespace Substrate.Core;

public struct ChunkKey : IEquatable<ChunkKey> {
    public readonly int cx;
    public readonly int cz;

    public ChunkKey(int cx, int cz) {
        this.cx = cx;
        this.cz = cz;
    }

    public bool Equals(ChunkKey ck) {
        return this.cx == ck.cx && this.cz == ck.cz;
    }

    public override bool Equals(Object o) {
        try {
            return this == (ChunkKey) o;
        } catch {
            return false;
        }
    }

    public override int GetHashCode() {
        int hash = 23;
        hash = hash * 37 + this.cx;
        hash = hash * 37 + this.cz;
        return hash;
    }

    public static bool operator ==(ChunkKey k1, ChunkKey k2) {
        return k1.cx == k2.cx && k1.cz == k2.cz;
    }

    public static bool operator !=(ChunkKey k1, ChunkKey k2) {
        return k1.cx != k2.cx || k1.cz != k2.cz;
    }

    public override string ToString() {
        return "(" + this.cx + ", " + this.cz + ")";
    }
}
