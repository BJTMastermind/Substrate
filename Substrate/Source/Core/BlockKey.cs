namespace Substrate.Core;

public struct BlockKey : IEquatable<BlockKey> {
    public readonly int x;
    public readonly int y;
    public readonly int z;

    public BlockKey(int x, int y, int z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public bool Equals(BlockKey bk) {
        return this.x == bk.x && this.y == bk.y && this.z == bk.z;
    }

    public override bool Equals(Object o) {
        try {
            return this == (BlockKey) o;
        } catch {
            return false;
        }
    }

    public override int GetHashCode() {
        int hash = 23;
        hash = hash * 37 + this.x;
        hash = hash * 37 + this.y;
        hash = hash * 37 + this.z;
        return hash;
    }

    public static bool operator ==(BlockKey k1, BlockKey k2) {
        return k1.x == k2.x && k1.y == k2.y && k1.z == k2.z;
    }

    public static bool operator !=(BlockKey k1, BlockKey k2) {
        return k1.x != k2.x || k1.y != k2.y || k1.z != k2.z;
    }

    public override string ToString() {
        return "(" + this.x + ", " + this.y + ", " + this.z + ")";
    }
}
