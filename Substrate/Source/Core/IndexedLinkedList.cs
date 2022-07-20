namespace Substrate.Core;

using System.Collections;

internal class IndexedLinkedList<T> : ICollection<T>, ICollection {
    private LinkedList<T> list;
    private Dictionary<T, LinkedListNode<T>> index;

    public T First {
        get { return this.list.First.Value; }
    }

    public T Last {
        get { return this.list.Last.Value; }
    }

    public IndexedLinkedList() {
        this.list = new LinkedList<T>();
        this.index = new Dictionary<T, LinkedListNode<T>>();
    }

    public void AddFirst(T value) {
        LinkedListNode<T> node = this.list.AddFirst(value);
        this.index.Add(value, node);
    }

    public void AddLast(T value) {
        LinkedListNode<T> node = this.list.AddLast(value);
        this.index.Add(value, node);
    }

    public void RemoveFirst() {
        this.index.Remove(this.list.First.Value);
        this.list.RemoveFirst();
    }

    public void RemoveLast() {
        this.index.Remove(this.list.First.Value);
        this.list.RemoveLast();
    }

    #region ICollection<T> Members
    public void Add(T item) {
        AddLast(item);
    }

    public void Clear() {
        this.index.Clear();
        this.list.Clear();
    }

    public bool Contains(T item) {
        return this.index.ContainsKey(item);
    }

    public void CopyTo(T[] array, int arrayIndex) {
        this.list.CopyTo(array, arrayIndex);
    }

    public bool IsReadOnly {
        get { return false; }
    }

    public bool Remove(T value) {
        LinkedListNode<T> node;
        if(this.index.TryGetValue(value, out node)) {
            this.index.Remove(value);
            this.list.Remove(node);
            return true;
        }

        return false;
    }
    #endregion

    #region ICollection Members
    void ICollection.CopyTo(Array array, int index) {
        (this.list as ICollection).CopyTo(array, index);
    }

    public int Count {
        get { return this.list.Count; }
    }

    bool ICollection.IsSynchronized {
        get { return false; }
    }

    object ICollection.SyncRoot {
        get { return (this.list as ICollection).SyncRoot; }
    }
    #endregion

    #region IEnumerable<T> Members
    public IEnumerator<T> GetEnumerator() {
        return this.list.GetEnumerator();
    }
    #endregion

    #region IEnumerable Members
    IEnumerator IEnumerable.GetEnumerator() {
        return this.list.GetEnumerator();
    }
    #endregion
}
