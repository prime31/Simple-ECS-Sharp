using System;
using System.Collections.Generic;

namespace SimpleECS
{
    public class Registry
    {
        internal class GroupData
        {
            public int HashCode;
            public SparseSet Entities;
            IComponentStore[] componentStores;

            public GroupData(Registry registry, int hashCode, params IComponentStore[] components)
            {
                HashCode = hashCode;
                Entities = new SparseSet(registry.maxEntities);
                componentStores = components;
            }

            internal void OnEntityAdded(uint entityId)
            {
                if (!Entities.Contains(entityId))
                {
                    foreach (var store in componentStores)
                        if (!store.Contains(entityId)) return;
                    Entities.Add(entityId);
                }
            }

            internal void OnEntityRemoved(uint entityId)
            {
                if (Entities.Contains(entityId)) Entities.Remove(entityId);
            }
        }

        readonly uint maxEntities;
        Dictionary<Type, IComponentStore> data = new Dictionary<Type, IComponentStore>();
        uint nextEntity = 0;
        List<GroupData> Groups = new List<GroupData>();

        public Registry(uint maxEntities) => this.maxEntities = maxEntities;

        public ComponentStore<T> Assure<T>()
        {
            var type = typeof(T);
            if (data.TryGetValue(type, out var store)) return (ComponentStore<T>)data[type];

            var newStore = new ComponentStore<T>(maxEntities);
            data[type] = newStore;
            return newStore;
        }

        public Entity Create() => new Entity(nextEntity++);

        public void Destroy(Entity entity)
        {
            foreach (var store in data.Values)
                store.RemoveIfContains(entity.Id);
        }

        public void AddComponent<T>(Entity entity, T component) => Assure<T>().Add(entity, component);

        public ref T GetComponent<T>(Entity entity) => ref Assure<T>().Get(entity.Id);

        public bool TryGetComponent<T>(Entity entity, ref T component)
        {
            var store = Assure<T>();
            if (store.Contains(entity.Id))
            {
                component = store.Get(entity.Id);
                return true;
            }

            return false;
        }

        public void RemoveComponent<T>(Entity entity) => Assure<T>().RemoveIfContains(entity.Id);

        public View<T> View<T>() => new View<T>(this);

        public View<T, U> View<T, U>() => new View<T, U>(this);

        public View<T, U, V> View<T, U, V>() => new View<T, U, V>(this);

        public Group Group<T, U>()
        {
            var hash = System.HashCode.Combine(typeof(T), typeof(U));

            foreach (var group in Groups)
                if (group.HashCode == hash) return new Group(this, group);

            var groupData = new GroupData(this, hash, Assure<T>(), Assure<U>());
            Groups.Add(groupData);

            Assure<T>().OnAdd += groupData.OnEntityAdded;
            Assure<U>().OnAdd += groupData.OnEntityAdded;

            Assure<T>().OnRemove += groupData.OnEntityRemoved;
            Assure<U>().OnRemove += groupData.OnEntityRemoved;

            foreach (var entityId in View<T, U>()) groupData.Entities.Add(entityId);

            return new Group(this, groupData);
        }

        public Group Group<T, U, V>()
        {
            var hash = System.HashCode.Combine(typeof(T), typeof(U), typeof(V));

            foreach (var group in Groups)
                if (group.HashCode == hash) return new Group(this, group);

            var groupData = new GroupData(this, hash, Assure<T>(), Assure<U>(), Assure<V>());
            Groups.Add(groupData);

            Assure<T>().OnAdd += groupData.OnEntityAdded;
            Assure<U>().OnAdd += groupData.OnEntityAdded;
            Assure<V>().OnAdd += groupData.OnEntityAdded;

            Assure<T>().OnRemove += groupData.OnEntityRemoved;
            Assure<U>().OnRemove += groupData.OnEntityRemoved;
            Assure<V>().OnRemove += groupData.OnEntityRemoved;

            foreach (var entityId in View<T, U, V>()) groupData.Entities.Add(entityId);

            return new Group(this, groupData);
        }
    }

}