﻿namespace Substrate;

using Substrate.Nbt;

/// <summary>
/// Functions to query and manage a collection of entities.
/// </summary>
public class EntityCollection : IEnumerable<TypedEntity> {
    private TagNodeList entities;
    private bool dirty;

    /// <summary>
    /// Gets or sets a value indicating whether this collection contains unsaved changes.
    /// </summary>
    public bool IsDirty {
        get { return this.dirty; }
        set { this.dirty = value; }
    }

    /// <summary>
    /// Creates a new <see cref="EntityCollection"/> around a <see cref="TagNodeList"/> containing Entity nodes.
    /// </summary>
    /// <param name="entities">A <see cref="TagNodeList"/> containing Entity nodes.</param>
    public EntityCollection(TagNodeList entities) {
        this.entities = entities;
    }

    /// <summary>
    /// Gets a list of all entities in the collection that match a given id (type).
    /// </summary>
    /// <param name="id">The id (type) of entities that should be returned.</param>
    /// <returns>A list of <see cref="TypedEntity"/> objects matching the given id (type).</returns>
    public List<TypedEntity> FindAll(string id) {
        List<TypedEntity> set = new List<TypedEntity>();

        foreach(TagNodeCompound ent in this.entities) {
            TagNode eid;
            if(!ent.TryGetValue("id", out eid)) {
                continue;
            }

            if(eid.ToTagString().Data != id) {
                continue;
            }

            TypedEntity obj = EntityFactory.Create(ent);
            if(obj != null) {
                set.Add(obj);
            }
        }
        return set;
    }

    /// <summary>
    /// Gets a list of all entities in the collection that match a given condition.
    /// </summary>
    /// <param name="match">A <see cref="Predicate{T}"/> defining the matching condition.</param>
    /// <returns>A list of <see cref="TypedEntity"/> objects matching the given condition.</returns>
    public List<TypedEntity> FindAll(Predicate<TypedEntity> match) {
        List<TypedEntity> set = new List<TypedEntity>();

        foreach(TagNodeCompound ent in this.entities) {
            TypedEntity obj = EntityFactory.Create(ent);
            if(obj == null) {
                continue;
            }

            if(match(obj)) {
                set.Add(obj);
            }
        }
        return set;
    }

    /// <summary>
    /// Adds a <see cref="TypedEntity"/> to the collection.
    /// </summary>
    /// <param name="ent">The <see cref="TypedEntity"/> object to add.</param>
    /// <remarks>It is up to the developer to ensure that the <see cref="TypedEntity"/> being added to the collection has a position that
    /// is within acceptable range of the collection.  <see cref="EntityCollection"/> transparently back other objects such as
    /// <see cref="IChunk"/> objects, which have a well-defined position in global space.  The <see cref="EntityCollection"/> itself has
    /// no concept of position and will not enforce constraints on the positions of <see cref="TypedEntity"/> objects being added.</remarks>
    public void Add(TypedEntity ent) {
        this.entities.Add(ent.BuildTree());
        this.dirty = true;
    }

    /// <summary>
    /// Removes all entities matching the given id (type) from the collection.
    /// </summary>
    /// <param name="id">The id (type) of entities that should be removed.</param>
    /// <returns>A count of the number of entities that were removed.</returns>
    public int RemoveAll(string id) {
        int rem = this.entities.RemoveAll(val => {
            TagNodeCompound cval = val as TagNodeCompound;
            if(cval == null) {
                return false;
            }

            TagNode sval;
            if(!cval.TryGetValue("id", out sval)) {
                return false;
            }

            return (sval.ToTagString().Data == id);
        });

        if(rem > 0) {
            this.dirty = true;
        }
        return rem;
    }

    /// <summary>
    /// Removes all entities matching the given condition from the collection.
    /// </summary>
    /// <param name="match">A <see cref="Predicate{T}"/> defining the matching condition.</param>
    /// <returns>A count of the number of entities that were removed.</returns>
    public int RemoveAll(Predicate<TypedEntity> match) {
        int rem = this.entities.RemoveAll(val => {
            TagNodeCompound cval = val as TagNodeCompound;
            if(cval == null) {
                return false;
            }

            TypedEntity obj = EntityFactory.Create(cval);
            if(obj == null) {
                return false;
            }

            return match(obj);
        });

        if(rem > 0) {
            this.dirty = true;
        }
        return rem;
    }

    #region IEnumerable<Entity> Members
    /// <summary>
    /// Returns an enumerator that iterates through all entities.
    /// </summary>
    /// <returns>An <see cref="Enumerator"/> for this object.</returns>
    public IEnumerator<TypedEntity> GetEnumerator() {
        return new Enumerator(this.entities);
    }
    #endregion

    #region IEnumerable Members
    /// <summary>
    /// Returns an enumerator that iterates through all entities.
    /// </summary>
    /// <returns>An <see cref="Enumerator"/> for this object.</returns>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
        return new Enumerator(this.entities);
    }
    #endregion

    /// <summary>
    /// Enumerates the entities within an <see cref="EntityCollection"/>.
    /// </summary>
    private struct Enumerator : IEnumerator<TypedEntity> {
        private IEnumerator<TagNode> enum_;

        private bool next;
        private TypedEntity cur;

        internal Enumerator(TagNodeList entities) {
            this.enum_ = entities.GetEnumerator();
            this.cur = null;
            this.next = false;
        }

        #region IEnumerator<Entity> Members
        /// <summary>
        /// Gets the <see cref="TypedEntity"/> at the current position of the enumerator.
        /// </summary>
        public TypedEntity Current {
            get {
                if(this.next == false) {
                    throw new InvalidOperationException();
                }
                return this.cur;
            }
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Releases all resources used by the <see cref="Enumerator"/>.
        /// </summary>
        public void Dispose() { }
        #endregion

        #region IEnumerator Members
        /// <summary>
        /// Gets the <see cref="TypedEntity"/> at the current position of the enumerator.
        /// </summary>
        object System.Collections.IEnumerator.Current {
            get { return Current; }
        }

        /// <summary>
        /// Advances the enumerator to the next <see cref="TypedEntity"/> in the <see cref="EntityCollection"/>.
        /// </summary>
        /// <returns>True if the enumerator was successfully advanced to the next position; false if the enumerator advanced past the end of the collection.</returns>
        public bool MoveNext() {
            if(!this.enum_.MoveNext()) {
                this.next = false;
                return false;
            }

            this.cur = EntityFactory.Create(this.enum_.Current.ToTagCompound());
            if(this.cur == null) {
                this.cur = EntityFactory.CreateGeneric(this.enum_.Current.ToTagCompound());
            }

            this.next = true;

            return true;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first <see cref="TypedEntity"/> in the collection.
        /// </summary>
        void System.Collections.IEnumerator.Reset() {
            this.cur = null;
            this.next = false;
            this.enum_.Reset();
        }
        #endregion
    }
}
