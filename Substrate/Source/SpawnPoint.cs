namespace Substrate;

/// <summary>
/// Represents the spawn point of a player or world.
/// </summary>
/// <remarks><see cref="SpawnPoint"/> values are immutable.  To change an existing spawn point, create a new instance with
/// the new coordinate(s).  Since some spawn points are optional in Minecraft, this helps safegaurd against saving a partial
/// spawn point.</remarks>
public struct SpawnPoint : IEquatable<SpawnPoint> {
    private readonly int x;
    private readonly int y;
    private readonly int z;

    /// <summary>
    /// Gets the global X-coordinate of the spawn point (in blocks).
    /// </summary>
    public int X {
        get { return this.x; }
    }

    /// <summary>
    /// Gets the global Y-coordinate of the spawn point (in blocks).
    /// </summary>
    public int Y {
        get { return this.y; }
    }

    /// <summary>
    /// Gets the global Z-coordinate of the spawn point (in blocks).
    /// </summary>
    public int Z {
        get { return this.z; }
    }

    /// <summary>
    /// Creates a new spawn point.
    /// </summary>
    /// <param name="x">The global X-coordinate of the spawn point.</param>
    /// <param name="y">The global Y-coordinate of the spawn point.</param>
    /// <param name="z">The global Z-coordinate of the spawn point.</param>
    public SpawnPoint(int x, int y, int z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    /// <summary>
    /// Checks if two <see cref="SpawnPoint"/> objects are considered equal.
    /// </summary>
    /// <param name="spawn">A <see cref="SpawnPoint"/> to compare against.</param>
    /// <returns>True if the two <see cref="SpawnPoint"/> objects are equal; false otherwise.</returns>
    public bool Equals(SpawnPoint spawn) {
        return this.x == spawn.x && this.y == spawn.y && this.z == spawn.z;
    }

    /// <summary>
    /// Checks if two <see cref="SpawnPoint"/> objects are considered equal.
    /// </summary>
    /// <param name="o">An to compare against.</param>
    /// <returns>True if the two <see cref="SpawnPoint"/> objects are equal; false otherwise.</returns>
    public override bool Equals(Object o) {
        if(o is SpawnPoint) {
            return this == (SpawnPoint) o;
        } else {
            return false;
        }
    }

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance.</returns>
    public override int GetHashCode() {
        int hash = 23;
        hash = hash * 37 + this.x;
        hash = hash * 37 + this.y;
        hash = hash * 37 + this.z;
        return hash;
    }

    /// <summary>
    /// Checks if two <see cref="SpawnPoint"/> objects are considered equal.
    /// </summary>
    /// <param name="k1">The first <see cref="SpawnPoint"/> in the comparison.</param>
    /// <param name="k2">The second <see cref="SpawnPoint"/> in the comparison.</param>
    /// <returns>True if the two <see cref="SpawnPoint"/> objects are equal; false otherwise.</returns>
    public static bool operator ==(SpawnPoint k1, SpawnPoint k2) {
        return k1.x == k2.x && k1.y == k2.y && k1.z == k2.z;
    }

    /// <summary>
    /// Checks if two <see cref="SpawnPoint"/> objects are considered unequal.
    /// </summary>
    /// <param name="k1">The first <see cref="SpawnPoint"/> in the comparison.</param>
    /// <param name="k2">The second <see cref="SpawnPoint"/> in the comparison.</param>
    /// <returns>True if the two <see cref="SpawnPoint"/> objects are not equal; false otherwise.</returns>
    public static bool operator !=(SpawnPoint k1, SpawnPoint k2) {
        return k1.x != k2.x || k1.y != k2.y || k1.z != k2.z;
    }

    /// <summary>
    /// Returns a string representation of the <see cref="SpawnPoint"/>.
    /// </summary>
    /// <returns>A string representing this <see cref="SpawnPoint"/>.</returns>
    public override string ToString() {
        return "(" + this.x + ", " + this.y + ", " + this.z + ")";
    }
}
