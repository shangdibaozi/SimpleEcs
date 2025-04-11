using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace SimpleEcs
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public class Query
    {
        private readonly SArray<EntityMeta> _entityGens;

        internal readonly ACPool[] _incPools;
        private readonly ACPool[] _excPools;
        
        internal Entity[] _entities;
        public Entity[] Entities
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _entities;
        }
#if SIMPLE_ECS_ENABLE_INT
        private uint[] _sparse;
        private int _len;
#else
        private ushort[] _sparse;
        internal ushort _len;
#endif
        internal int _id;
        
        private readonly string _queryDesc;
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _len;
        }

#if DEBUG
        public Action<Entity> OnAddEntity;
        public Action<Entity> OnRemoveEntity;
        internal bool _isLock;
#endif
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Query(SArray<EntityMeta> entityGens, ACPool[] incPools, ACPool[] excPools, string queryDesc)
        {
            _entityGens = entityGens;
            _incPools = incPools;
            _excPools = excPools;
            _queryDesc = queryDesc;
            _entities = new Entity[2];
            
#if SIMPLE_ECS_ENABLE_INT
            _sparse = new uint[entityGens.Length];
#else
            _sparse = new ushort[entityGens.Length];
#endif
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool IsMatch(in Entity entity)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < _incPools.Length; i++)
            {
                if (!_incPools[i].Has(entity))
                {
                    return false;
                }
            }

            if (_excPools.Length > 0)
            {
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < _excPools.Length; i++)
                {
                    if (_excPools[i].Has(entity))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        internal bool IsSameQuery(SArray<ushort> incTypes, SArray<ushort> excTypes)
        {
            if (incTypes.Length != _incPools.Length || excTypes.Length != _excPools.Length)
            {
                return false;
            }

            return Compare(incTypes, _incPools) && Compare(excTypes, _excPools);

            bool Compare(SArray<ushort> typeIds, ACPool[] pools)
            {
                for (int i = 0; i < typeIds.Length; i++)
                {
                    if (typeIds[i] != pools[i].compId)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void AddEntity(in Entity entity)
        {
            if (_entities.Length == _len)
            {
                Array.Resize(ref _entities, _len << 1);
            }

            if (entity.Index >= _sparse.Length)
            {
                Array.Resize(ref _sparse, _entityGens.Capacity);
            }
            
            _entities[_len] = entity;
            _len++;
            _sparse[entity.Index] = _len;
#if DEBUG
            OnAddEntity?.Invoke(entity);   
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void DelEntity(in Entity entity)
        {
#if DEBUG
            if (entity.Index >= _sparse.Length || _sparse[entity.Index] == 0)
            {
                throw new Exception("entity不在query里面");
            }
#endif
            
            ref var idx = ref _sparse[entity.Index];
            _len--;
            _entities[idx - 1] = _entities[_len];
            _sparse[_entities[idx - 1].Index] = idx;
            idx = 0;
            // 在foreach循环时，连续删除entity
            if (_len < _id)
            {
                _id = _len;
            }
#if DEBUG
            _entities[_len] = default;
            OnRemoveEntity?.Invoke(entity);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool HasEntity(in Entity entity)
        {
            return entity.Index < _sparse.Length && _sparse[entity.Index] > 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Begin()
        {
            _id = _len;
#if DEBUG
            if (_isLock)
            {
                throw new Exception("Query不能在foreach中嵌套遍历");
            }
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool Next()
        {
            _id--;
#if DEBUG
            _isLock = _id >= 0;
#endif
            return _id >= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void End()
        {
#if DEBUG
            _isLock = false;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Clear()
        {
            if (_len == 0)
            {
                return;
            }
            Array.Clear(_entities, 0, _entities.Length);
            Array.Clear(_sparse, 0, _sparse.Length);
            _len = 0;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public QueryEnumerator GetEnumerator()
        {
            return new QueryEnumerator(this);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => _queryDesc;
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public readonly ref struct QueryEnumerator
    {
        private readonly Query _query;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public QueryEnumerator(Query query)
        {
            _query = query;
            _query.Begin();
        }

        public Entity Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _query._entities[_query._id];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => _query.Next();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() => _query.End();
    }
    
}
