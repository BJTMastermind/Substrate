namespace Substrate;

/// <summary>
/// Provides read-only indexed access to an underlying resource.
/// </summary>
/// <typeparam name="T">The type of the underlying resource.</typeparam>
public interface ICacheTable<T> : IEnumerable<T> {
    /// <summary>
    /// Gets the value at the given index.
    /// </summary>
    /// <param name="index">The index to fetch.</param>
    T this[int index] { get; }
}

/*internal class CacheTableArray<T> : ICacheTable<T> {
    private T[] cache;

    public T this[int index] {
        get { return this.cache[index]; }
    }

    public CacheTableArray(T[] cache) {
        this.cache = cache;
    }
}

internal class CacheTableDictionary<T> : ICacheTable<T> {
    private Dictionary<int, T> cache;
    private static Random rand = new Random();

    public T this[int index] {
        get {
            T val;
            if(this.cache.TryGetValue(index, out val)) {
                return val;
            }
            return default(T);
        }
    }

    public CacheTableDictionary(Dictionary<int, T> cache) {
        this.cache = cache;
    }
}

/// <summary>
/// Provides read-only indexed access to an underlying resource.
/// </summary>
/// <typeparam name="T">The type of the underlying resource.</typeparam>
public class CacheTable<T> {
    ICacheTable<T> cache;

    /// <summary>
    /// Gets the value at the given index.
    /// </summary>
    /// <param name="index"></param>
    public T this[int index] {
        get { return this.cache[index]; }
    }

    internal CacheTable(T[] cache) {
        this.cache = new CacheTableArray<T>(cache);
    }

    internal CacheTable(Dictionary<int, T> cache) {
        this.cache = new CacheTableDictionary<T>(cache);
    }
}*/
