namespace Substrate.Core;

using System.Collections;

internal class LRUCache<TKey, TValue> : IDictionary<TKey, TValue> {
    public class CacheValueArgs : EventArgs {
        private TKey key;
        private TValue value;

        public TKey Key {
            get { return this.key; }
        }

        public TValue Value {
            get { return this.value; }
        }

        public CacheValueArgs(TKey key, TValue value) {
            this.key = key;
            this.value = value;
        }
    }

    public event EventHandler<CacheValueArgs> RemoveCacheValue;

    private Dictionary<TKey, TValue> data;
    private IndexedLinkedList<TKey> index;

    private int capacity;

    public LRUCache(int capacity) {
        if(capacity <= 0) {
            throw new ArgumentException("Cache capacity must be positive");
        }

        this.capacity = capacity;

        this.data = new Dictionary<TKey, TValue>();
        this.index = new IndexedLinkedList<TKey>();
    }

    #region IDictionary<TKey,TValue> Members
    public void Add(TKey key, TValue value) {
        if(this.data.ContainsKey(key)) {
            throw new ArgumentException("Attempted to insert a duplicate key");
        }

        this.data[key] = value;
        this.index.Add(key);

        if(this.data.Count > this.capacity) {
            OnRemoveCacheValue(new CacheValueArgs(this.index.First, this.data[this.index.First]));

            this.data.Remove(this.index.First);
            this.index.RemoveFirst();
        }
    }

    public bool ContainsKey(TKey key) {
        return this.data.ContainsKey(key);
    }

    public ICollection<TKey> Keys {
        get { return this.data.Keys; }
    }

    public bool Remove(TKey key) {
        if(this.data.Remove(key)) {
            this.index.Remove(key);
            return true;
        }
        return false;
    }

    public bool TryGetValue(TKey key, out TValue value) {
        if(!this.data.TryGetValue(key, out value)) {
            return false;
        }
        this.index.Remove(key);
        this.index.Add(key);

        return true;
    }

    public ICollection<TValue> Values {
        get { return this.data.Values; }
    }

    public TValue this[TKey key] {
        get {
            TValue value = this.data[key];
            this.index.Remove(key);
            this.index.Add(key);
            return value;
        }
        set {
            this.data[key] = value;
            this.index.Remove(key);
            this.index.Add(key);

            if(this.data.Count > this.capacity) {
                OnRemoveCacheValue(new CacheValueArgs(this.index.First, this.data[this.index.First]));

                this.data.Remove(this.index.First);
                this.index.RemoveFirst();
            }
        }
    }
    #endregion

    #region ICollection<KeyValuePair<TKey,TValue>> Members
    public void Add(KeyValuePair<TKey, TValue> item) {
        Add(item.Key, item.Value);
    }

    public void Clear() {
        this.data.Clear();
        this.index.Clear();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item) {
        return ((ICollection<KeyValuePair<TKey, TValue>>) this.data).Contains(item);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
        ((ICollection<KeyValuePair<TKey, TValue>>) this.data).CopyTo(array, arrayIndex);
    }

    public int Count {
        get { return this.data.Count; }
    }

    public bool IsReadOnly {
        get { return false; }
    }

    public bool Remove(KeyValuePair<TKey, TValue> item) {
        if(((ICollection<KeyValuePair<TKey, TValue>>) this.data).Remove(item)) {
            this.index.Remove(item.Key);
            return true;
        }
        return false;
    }
    #endregion

    #region IEnumerable<KeyValuePair<TKey,TValue>> Members
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
        return this.data.GetEnumerator();
    }
    #endregion

    #region IEnumerable Members
    IEnumerator IEnumerable.GetEnumerator() {
        return this.data.GetEnumerator();
    }
    #endregion

    private void OnRemoveCacheValue(CacheValueArgs e) {
        if(RemoveCacheValue != null) {
            RemoveCacheValue(this, e);
        }
    }
}
