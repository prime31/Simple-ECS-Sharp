using System;
using System.Collections;
using System.Collections.Generic;

namespace SimpleECS
{
    public class SparseSet : IEnumerable<uint>
    {
        readonly uint max;
        uint size;
        uint[] dense;
        uint[] sparse;

        public uint Count => size;

        public SparseSet(uint maxValue)
        {
            max = maxValue + 1;
            size = 0;
            dense = new uint[max];
            sparse = new uint[max];
        }

        public void Add(uint value)
        {
            if (value >= 0 && value < max && !Contains(value))
            {
                dense[size] = value;
                sparse[value] = size;
                size++;
            }
        }

        public void Remove(uint value)
        {
            if (Contains(value))
            {
                dense[sparse[value]] = dense[size - 1];
                sparse[dense[size - 1]] = sparse[value];
                size--;
            }
        }

        public uint Index(uint value) => sparse[value];

        public bool Contains(uint value)
        {
            if (value >= max || value < 0)
                return false;
            else
                return sparse[value] < size && dense[sparse[value]] == value;
        }

        public void Clear() => size = 0;

        public IEnumerator<uint> GetEnumerator()
        {
            var i = 0;
            while (i < size)
            {
                yield return dense[i];
                i++;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override bool Equals(object obj) => throw new Exception("Why are you comparing SparseSets?");

        public override int GetHashCode() => System.HashCode.Combine(max, size, dense, sparse, Count);
    }

}