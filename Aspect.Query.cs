using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace SimpleEcs
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public abstract partial class Aspect<A> where A : IAspect
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public QueryBuilder<A> Inc<T>() where T : struct
        {
            return _queryBuilder.Inc<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public QueryBuilder<A> Inc<T1, T2>() where T1 : struct where T2 : struct
        {
            _queryBuilder.Inc<T1>();
            _queryBuilder.Inc<T2>();
            return _queryBuilder;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public QueryBuilder<A> Inc<T1, T2, T3>() where T1 : struct where T2 : struct where T3 : struct
        {
            _queryBuilder.Inc<T1>();
            _queryBuilder.Inc<T2>();
            _queryBuilder.Inc<T3>();
            return _queryBuilder;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public QueryBuilder<A> Inc<T1, T2, T3, T4>() where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
        {
            _queryBuilder.Inc<T1>();
            _queryBuilder.Inc<T2>();
            _queryBuilder.Inc<T3>();
            _queryBuilder.Inc<T4>();
            return _queryBuilder;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public QueryBuilder<A> Inc<T1, T2, T3, T4, T5>() where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
        {
            _queryBuilder.Inc<T1>();
            _queryBuilder.Inc<T2>();
            _queryBuilder.Inc<T3>();
            _queryBuilder.Inc<T4>();
            _queryBuilder.Inc<T5>();
            return _queryBuilder;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public QueryBuilder<A> Inc<T1, T2, T3, T4, T5, T6>() where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
        {
            _queryBuilder.Inc<T1>();
            _queryBuilder.Inc<T2>();
            _queryBuilder.Inc<T3>();
            _queryBuilder.Inc<T4>();
            _queryBuilder.Inc<T5>();
            _queryBuilder.Inc<T6>();
            return _queryBuilder;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public QueryBuilder<A> Inc<T1, T2, T3, T4, T5, T6, T7>() where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct
        {
            _queryBuilder.Inc<T1>();
            _queryBuilder.Inc<T2>();
            _queryBuilder.Inc<T3>();
            _queryBuilder.Inc<T4>();
            _queryBuilder.Inc<T5>();
            _queryBuilder.Inc<T6>();
            _queryBuilder.Inc<T7>();
            return _queryBuilder;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public QueryBuilder<A> Inc<T1, T2, T3, T4, T5, T6, T7, T8>() where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
            where T7 : struct
            where T8 : struct

        {
            _queryBuilder.Inc<T1>();
            _queryBuilder.Inc<T2>();
            _queryBuilder.Inc<T3>();
            _queryBuilder.Inc<T4>();
            _queryBuilder.Inc<T5>();
            _queryBuilder.Inc<T6>();
            _queryBuilder.Inc<T7>();
            _queryBuilder.Inc<T8>();
            return _queryBuilder;
        }
    }
}