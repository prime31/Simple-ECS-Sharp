using System;
using System.Collections;
using System.Collections.Generic;

namespace SimpleECS
{
    public struct View<T> : IEnumerable<uint>
    {
        Registry registry;

        public View(Registry registry) => this.registry = registry;

        public IEnumerator<uint> GetEnumerator() => registry.Assure<T>().Set.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public struct View<T, U> : IEnumerable<uint>
    {
        struct Enumerator : IEnumerator<uint>
        {
            Registry registry;
            IComponentStore store;
            IEnumerator<uint> setEnumerator;

            public Enumerator(Registry registry)
            {
                this.registry = registry;
                var store1 = registry.Assure<T>();
                var store2 = registry.Assure<U>();

                if (store1.Count > store2.Count)
                {
                    setEnumerator = store2.Entities.GetEnumerator();
                    store = store1;
                }
                else
                {
                    setEnumerator = store1.Entities.GetEnumerator();
                    store = store2;
                }
            }

            public uint Current => setEnumerator.Current;

            object IEnumerator.Current => setEnumerator.Current;

            public void Dispose()
            {}

            public bool MoveNext()
            {
                while (setEnumerator.MoveNext())
                {
                    var entityId = setEnumerator.Current;
                    if (!store.Contains(entityId)) continue;
                    return true;
                }
                return false;
            }

            public void Reset() => setEnumerator.Reset();
        }

        Registry registry;

        public View(Registry registry) => this.registry = registry;

        public IEnumerator<uint> GetEnumerator() => new Enumerator(registry);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public struct View<T, U, V> : IEnumerable<uint>
    {
        struct Enumerator : IEnumerator<uint>
        {
            static IComponentStore[] sorter = new IComponentStore[3];
            Registry registry;
            IComponentStore store1;
            IComponentStore store2;
            IEnumerator<uint> setEnumerator;

            public Enumerator(Registry registry)
            {
                this.registry = registry;

                sorter[0] = registry.Assure<T>();
                sorter[1] = registry.Assure<U>();
                sorter[2] = registry.Assure<V>();
                Array.Sort(sorter, (first, second) => first.Entities.Count.CompareTo(second.Entities.Count));

                setEnumerator = sorter[0].Entities.GetEnumerator();
                store1 = sorter[1];
                store2 = sorter[2];
            }

            public uint Current => setEnumerator.Current;

            object IEnumerator.Current => setEnumerator.Current;

            public void Dispose()
            {}

            public bool MoveNext()
            {
                while (setEnumerator.MoveNext())
                {
                    var entityId = setEnumerator.Current;
                    if (!store1.Contains(entityId) || !store2.Contains(entityId)) continue;
                    return true;
                }
                return false;
            }

            public void Reset() => setEnumerator.Reset();
        }


        Registry registry;

        public View(Registry registry) => this.registry = registry;

        public IEnumerator<uint> GetEnumerator() => new Enumerator(registry);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

}