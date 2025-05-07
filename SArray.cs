using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace SimpleEcs
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public sealed class SArray<T>
    {
        internal T[] _data;
        private int _len;
        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _len;
        }

        public int Capacity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _data.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SArray(int initSize = 16)
        {
            _data = new T[initSize];
            _len = 0;
        }

        public ref T this[int idx]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
#if DEBUG
                if (idx < 0 || idx >= _len)
                {
                    throw new Exception($"idx[{idx}] out of range[{_len}]");
                }
#endif
                return ref _data[idx];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(in T v)
        {
            if (_len >= _data.Length)
            {
                Array.Resize(ref _data, _len << 1);
            }

            _data[_len++] = v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(SArray<T> range)
        {
            for (var i = 0; i < range.Length; i++)
            {
                Add(range[i]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T RemoveLast()
        {
#if DEBUG
            if (_len == 0)
            {
                throw new Exception("empty array");
            }
#endif
            _len--;
            return ref _data[_len];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveAt(int idx)
        {
            if (idx >= 0 && idx < _len)
            {
                _data[idx] = default;
                _len--;
                if (idx < _len)
                {
                    (_data[idx], _data[_len]) = (_data[_len], _data[idx]);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            Array.Clear(_data, 0, _len);
            _len = 0;
        }
    }
}
