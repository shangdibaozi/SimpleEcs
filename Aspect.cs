using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.IL2CPP.CompilerServices;

namespace SimpleEcs
{
    public interface IAspect
    {
    }

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public abstract partial class Aspect<A> : BaseAspect, IAspect where A : IAspect
    {
        private readonly SArray<Entity> _recycled;
        private readonly QueryBuilder<A> _queryBuilder;
        private readonly SArray<Query> _queries;
        private readonly Config _cfg;
        private readonly EntityPool entityPool;

        protected Aspect(Config cfg)
        {
            _cfg = cfg;
            _queryBuilder = new QueryBuilder<A>(this);
            _queries = new SArray<Query>(16);
            var capacity = cfg.EntityCapacity > 0 ? cfg.EntityCapacity : Config.DefaultEntityCapacity;
            entityGens = new SArray<EntityMeta>(capacity);
            entityGens.Add(new EntityMeta
            {
                version = ushort.MaxValue // 默认的Entity进行IsAlive测试时会返回false
            });
            capacity = cfg.RecycledEntityCapacity > 0
                ? cfg.RecycledEntityCapacity
                : Config.DefaultRecycledEntityCapacity;
            _recycled = new SArray<Entity>(capacity);
            _cPools = new SArray<ACPool>(cfg.CPoolCapacity > 0 ? cfg.CPoolCapacity : Config.DefaultCPoolCapacity);
            edges = new Edge[_cPools.Capacity];
            edgeIndex = 0;
#if DEBUG
            aspectId = (byte)Aspects.Length;
            AspectName = typeof(A).Name;
            Queries.Add(_queries);
            Aspects.Add(this);
#endif
            Inject();
            entityPool = new EntityPool();
            entityPool.Init(this);
        }

        internal Query TryAddQuery(SArray<ushort> incTypes, SArray<ushort> excTypes)
        {
            for (int i = 0; i < _queries.Length; i++)
            {
                var q = _queries[i];
                if (q.IsSameQuery(incTypes, excTypes))
                {
                    return q;
                }
            }

            var incPools = new ACPool[incTypes.Length];
            var excPools = new ACPool[excTypes.Length];

            var queryDesc = "";
#if DEBUG
            queryDesc = GetQueryDesc(incTypes, excTypes);
#endif
            Query query = new Query(entityGens, incPools, excPools, queryDesc);
            _queries.Add(query);
            for (int i = 0; i < incTypes.Length; i++)
            {
                incPools[i] = _cPools[incTypes[i]];
#if DEBUG
                if (incPools[i] == null)
                {
                    throw new Exception($"{typeof(A).Name}中没有AddCPool({AspectComponentBase.Types[incTypes[i]]})");
                }

                if (incPools[i].len != 0)
                {
                    throw new Exception(
                        $"{typeof(A).Name}中的CPool({AspectComponentBase.Types[incTypes[i]]})不为空（需要在创建实体前先创建Query）");
                }
#endif
                incPools[i].AddQuery(query);
            }

            for (int i = 0; i < excTypes.Length; i++)
            {
                var pool = _cPools[excTypes[i]];
#if DEBUG
                if (pool == null)
                {
                    throw new Exception($"{typeof(A).Name}中没有AddCPool({AspectComponentBase.Types[excTypes[i]]})");
                }
#endif
                excPools[i] = pool;
                pool.AddQuery(query);
            }

            return query;
        }

#if DEBUG
        private static string GetQueryDesc(SArray<ushort> incTypes, SArray<ushort> excTypes)
        {
            var sb = new StringBuilder();
            sb.Append("Inc[");

            for (int i = 0; i < incTypes.Length; i++)
            {
                sb.Append(AspectComponentBase.Types[incTypes[i]].Name);
                if (i + 1 < incTypes.Length)
                {
                    sb.Append(",");
                }
            }

            sb.Append("]");

            if (excTypes.Length > 0)
            {
                sb.Append(" Exc[");
                for (int i = 0; i < excTypes.Length; i++)
                {
                    sb.Append(AspectComponentBase.Types[excTypes[i]].Name);
                    if (i + 1 < excTypes.Length)
                    {
                        sb.Append(",");
                    }
                }

                sb.Append("]");
            }

            return sb.ToString();
        }
#endif

        public void Clear()
        {
            _tmpNewEntity = default;
            entityGens.Clear();
            entityGens.Add(new EntityMeta
            {
                version = ushort.MaxValue
            });
            _recycled.Clear();
            for (var i = 0; i < AspectComponentBase.Counter; i++)
            {
#if DEBUG
                if (i >= _cPools.Length)
                {
                    throw new Exception($"{typeof(A).Name}没有添加Pool:{AspectComponentBase.Types[i]}");
                }
#endif
                _cPools[i].Clear();
            }

            for (var i = 0; i < _queries.Length; i++)
            {
                _queries[i].Clear();
            }
            
            entityPool.Clear();
        }

        public CPool<T> AddCPool<T>() where T : struct
        {
            var compId = AspectComponent<T>.Id;
            if (compId < _cPools.Length && _cPools[compId] != null)
            {
                return CPool<T>();
            }

            var pool = new CPool<T>();
            pool.Init<A>(this);
            _cPools.Add(pool);
            AddEdge(compId);
            return pool;
        }
        
        public CTagPool<T> AddCTagPool<T>() where T : struct
        {
            var compId = AspectComponent<T>.Id;
            if (compId < _cPools.Length && _cPools[compId] != null)
            {
                return CTagPool<T>();
            }

            var pool = new CTagPool<T>();
            pool.Init<A>(this);
            _cPools.Add(pool);
            AddEdge(compId);
            return pool;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CPool<T> CPool<T>() where T : struct
        {
            var compId = AspectComponent<T>.Id;
#if DEBUG
            if (compId >= _cPools.Length || _cPools[compId] == null)
            {
                throw new Exception($"{typeof(A).Name}没有添加Pool:{typeof(T).Name}");
            }
#endif

            return _cPools[compId] as CPool<T>;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CTagPool<T> CTagPool<T>() where T : struct
        {
            var compId = AspectComponent<T>.Id;
#if DEBUG
            if (compId >= _cPools.Length || _cPools[compId] == null)
            {
                throw new Exception($"{typeof(A).Name}没有添加Pool:{typeof(T).Name}");
            }
#endif

            return _cPools[compId] as CTagPool<T>;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort TypeId<T>() where T : struct
        {
            return AspectComponent<T>.Id;
        }

        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
        internal class AspectComponentBase
        {
            internal static ushort Counter;
#if DEBUG
            internal static Type[] Types = new Type[64];
#endif
        }

        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
        internal abstract class AspectComponent<T> : AspectComponentBase where T : struct
        {
            public static readonly ushort Id;

            static AspectComponent()
            {
                Id = Counter++;
#if DEBUG
                Types[Id] = typeof(T);
                if (Counter >= Types.Length)
                {
                    Array.Resize(ref Types, Types.Length << 1);
                }
#endif
            }
        }
    }

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct Config
    {
        public int EntityCapacity;
        public int RecycledEntityCapacity;
        public int CPoolCapacity;

        internal const int DefaultEntityCapacity = 256;
        internal const int DefaultRecycledEntityCapacity = 256;
        internal const int DefaultCPoolCapacity = 64;
    }
}