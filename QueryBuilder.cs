using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace SimpleEcs
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public class QueryBuilder<A> where A : IAspect
    {
        private readonly Aspect<A> _aspect;
        private readonly SArray<ushort> _incTypes = new(64);
        private readonly SArray<ushort> _excTypes = new(64);
        
        private readonly SortedSet<ushort> _incSortedSet = new();
        private readonly SortedSet<ushort> _excSortedSet = new();

        public QueryBuilder(Aspect<A> aspect)
        {
            _aspect = aspect;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public QueryBuilder<A> Inc<T>() where T : struct
        {
            var fields  = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (fields.Length == 0)
            {
                _aspect.AddCTagPool<T>();
            }
            else
            {
                _aspect.AddCPool<T>();
            }
            _incSortedSet.Add(_aspect.TypeId<T>());
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public QueryBuilder<A> Exc<T>() where T : struct
        {
            var fields  = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (fields.Length == 0)
            {
                _aspect.AddCTagPool<T>();
            }
            else
            {
                _aspect.AddCPool<T>();
            }
            _excSortedSet.Add(_aspect.TypeId<T>());
            return this;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public QueryBuilder<A> Exc<T1, T2>() where T1 : struct where T2 : struct
        {
            Exc<T1>().Exc<T2>();
            return this;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public QueryBuilder<A> Exc<T1, T2, T3>() where T1 : struct where T2 : struct where T3 : struct
        {
            Exc<T1>().Exc<T2>().Exc<T3>();
            return this;
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Query End()
        {
            if (_incSortedSet.Count == 0)
            {
                throw new Exception("没有设置需要筛选的组件");
            }

            foreach (var val in _incSortedSet)
            {
                _incTypes.Add(val);
            }

            foreach (var val in _excSortedSet)
            {
                _excTypes.Add(val);
            }
            
            var query = _aspect.TryAddQuery(_incTypes, _excTypes);
            _incSortedSet.Clear();
            _excSortedSet.Clear();
            _incTypes.Clear();
            _excTypes.Clear();
            return query;
        }
    }
}