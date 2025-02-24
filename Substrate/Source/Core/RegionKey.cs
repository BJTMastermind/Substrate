﻿namespace Substrate.Core;

public struct RegionKey : IEquatable<RegionKey> {
    public static RegionKey InvalidRegion = new RegionKey(int.MinValue, int.MinValue);

    readonly int rx;
    readonly int rz;

    public int X {
        get { return this.rx; }
    }

    public int Z {
        get { return this.rz; }
    }

    public RegionKey(int rx, int rz) {
        this.rx = rx;
        this.rz = rz;
    }

    public bool Equals(RegionKey ck) {
        return this.rx == ck.rx && this.rz == ck.rz;
    }

    public override bool Equals(Object o) {
        try {
            return this == (RegionKey) o;
        } catch {
            return false;
        }
    }

    public override int GetHashCode() {
        int hash = 23;
        hash = hash * 37 + this.rx;
        hash = hash * 37 + this.rz;
        return hash;
    }

    public static bool operator ==(RegionKey k1, RegionKey k2) {
        return k1.rx == k2.rx && k1.rz == k2.rz;
    }

    public static bool operator !=(RegionKey k1, RegionKey k2) {
        return k1.rx != k2.rx || k1.rz != k2.rz;
    }

    public override string ToString() {
        return "(" + this.rx + ", " + this.rz + ")";
    }
}
