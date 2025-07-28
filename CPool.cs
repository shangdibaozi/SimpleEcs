using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;
using UnityEngine.Scripting;

namespace SimpleEcs
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public abstract class ACPool
    {
        internal BaseAspect _baseAspect;
        internal ushort compId;
#if SIMPLE_ECS_ENABLE_INT
        internal int[] _sparse;
        internal int len;
#else
        internal ushort[] _sparse;
        internal ushort len;
#endif
        internal SArray<Query> _queries;
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => len;
        }

#if DEBUG
        internal byte _aspectId;
        public string typeName;
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void AddQuery(Query query)
        {
            for (int i = 0; i < _queries.Length; i++)
            {
                if (_queries[i] == query)
                {
                    return;
                }
            }

            _queries.Add(query);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has(in Entity entity)
        {
#if DEBUG
            if (entity.aspectId != _aspectId)
            {
                throw new Exception(
                    $"CPool<{typeName}>.Has 实体属于{BaseAspect.Aspects[entity.aspectId].AspectName}，不能在{BaseAspect.Aspects[_aspectId].AspectName}进行操作");
            }
#endif

            return entity.Index < _sparse.Length && _sparse[entity.Index] > 0;
        }

#if DEBUG
        internal void CheckAlive(in Entity entity)
        {
            if (!_baseAspect.IsAlive(entity))
            {
                throw new Exception($"CPool<{typeName}>无法对已销毁的实体[{entity}]进行操作");
            }
        }
#endif

        internal abstract ushort Init<A>(BaseAspect baseAspect) where A : IAspect;
        internal abstract void IAdd(in Entity entity);
        internal abstract void InnerDel(in Entity entity);
        internal abstract void IClone(in Entity from, in Entity to);
        internal abstract void ICloneFirst(in Entity entity, in Entity to);
        internal abstract void Clear();

#if DEBUG
        public abstract object Get(in Entity entity);
        public abstract void Set(in Entity entity, object value);
#endif
    }

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public class CPool<T> : ACPool where T : struct
    {
        private SArray<EntityMeta> _entityGens;
        private T[] _data;
        
#if SIMPLE_ECS_ENABLE_INT
        internal int[] _recycledItems;
        internal int _recycledItemsCount;
#else
        internal ushort[] _recycledItems;
        internal ushort _recycledItemsCount;
#endif

        [Preserve]
        public CPool()
        {
        }

        internal override ushort Init<A>(BaseAspect baseAspect)
        {
            compId = Aspect<A>.AspectComponent<T>.Id;
            _baseAspect = baseAspect;
#if DEBUG
            typeName = typeof(T).Name;
            _aspectId = baseAspect.aspectId;
#endif

            _queries = new SArray<Query>(1);
            _entityGens = baseAspect.entityGens;

            var capacity = _entityGens.Capacity;
            _data = new T[capacity];
            len = 0;

#if SIMPLE_ECS_ENABLE_INT
            _sparse = new int[capacity];
            _recycledItems = new int[capacity];
#else
            _sparse = new ushort[capacity];
            _recycledItems = new ushort[capacity];
#endif
            _recycledItemsCount = 0;
            return compId;
        }

        public ref T this[in Entity entity]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
#if DEBUG
                if (entity.aspectId != _aspectId)
                {
                    throw new Exception(
                        $"实体[{entity}]属于{BaseAspect.Aspects[entity.aspectId].AspectName}，不能在{BaseAspect.Aspects[_aspectId].AspectName}进行操作");
                }

                if (_entityGens[entity.Index].version != entity.Version)
                {
                    throw new Exception($"{BaseAspect.Aspects[_aspectId].AspectName}中的实体[{entity}]已销毁");
                }

                if (!Has(entity))
                {
                    throw new Exception($"{BaseAspect.Aspects[_aspectId].AspectName}中的实体[{entity}]没有组件：{typeof(T).Name}");
                }
#endif
                return ref _data[_sparse[entity.Index]];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Add(in Entity entity)
        {
#if DEBUG
            if (entity.aspectId != _aspectId)
            {
                throw new Exception(
                    $"CPool<{typeof(T).Name}>.Add 实体属于{BaseAspect.Aspects[entity.aspectId].AspectName}:{entity.aspectId}，不能在{BaseAspect.Aspects[_aspectId].AspectName}:{_aspectId}进行操作");
            }

            CheckAlive(entity);

            if (Has(entity))
            {
                throw new Exception(
                    $"{BaseAspect.Aspects[_aspectId].AspectName}中的实体[{entity}]重复添加组件\"{typeof(T).Name}\"");
            }
#endif
            if (entity.Index >= _sparse.Length)
            {
                Array.Resize(ref _sparse, _entityGens.Capacity);
            }

            ref var idx = ref _sparse[entity.Index];
            if (_recycledItemsCount > 0)
            {
                idx = _recycledItems[--_recycledItemsCount];
                len++;
            }
            else
            {
                idx = ++len;
                if (idx >= _data.Length)
                {
                    Array.Resize(ref _data, len << 1);
                }
            }

            
            _baseAspect.Add(entity, compId);
            RefreshQuery(entity);
            return ref _data[idx];
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref T AddFirst(in Entity entity)
        {
#if DEBUG
            if (entity.aspectId != _aspectId)
            {
                throw new Exception(
                    $"CPool<{typeof(T).Name}>.Add 实体属于{BaseAspect.Aspects[entity.aspectId].AspectName}:{entity.aspectId}，不能在{BaseAspect.Aspects[_aspectId].AspectName}:{_aspectId}进行操作");
            }
            
            CheckAlive(entity);

            if (Has(entity))
            {
                throw new Exception(
                    $"{BaseAspect.Aspects[_aspectId].AspectName}中的实体[{entity}]重复添加组件\"{typeof(T).Name}\"");
            }
#endif
            if (entity.Index >= _sparse.Length)
            {
                Array.Resize(ref _sparse, _entityGens.Capacity);
            }

            ref var idx = ref _sparse[entity.Index];
            if (_recycledItemsCount > 0)
            {
                idx = _recycledItems[--_recycledItemsCount];
                len++;
            }
            else
            {
                idx = ++len;
                if (idx >= _data.Length)
                {
                    Array.Resize(ref _data, len << 1);
                }
            }
            
            RefreshQuery(entity);
            return ref _data[idx];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryAdd(in Entity entity)
        {
            if (Has(entity))
            {
                return false;
            }

            Add(entity);
            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetOrAddRef(in Entity entity)
        {
            if (Has(entity))
            {
                return ref this[entity];
            }

            return ref Add(entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal override void IAdd(in Entity entity)
        {
            Add(entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal override void IClone(in Entity entity, in Entity to)
        {
#if DEBUG
            if (entity.aspectId != _aspectId)
            {
                throw new Exception(
                    $"CPool<{typeof(T).Name}>.Add 实体属于{BaseAspect.Aspects[entity.aspectId].AspectName}，不能在{BaseAspect.Aspects[_aspectId].AspectName}进行操作");
            }
            
            CheckAlive(entity);

            if (!Has(entity))
            {
                throw new Exception(
                    $"{BaseAspect.Aspects[_aspectId].AspectName}中的实体[{entity}]没有组件\"{typeof(T).Name}\"");
            }
#endif

            Add(to) = _data[_sparse[entity.Index]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal override void ICloneFirst(in Entity entity, in Entity to)
        {
#if DEBUG
            if (entity.aspectId != _aspectId)
            {
                throw new Exception(
                    $"CPool<{typeof(T).Name}>.Add 实体属于{BaseAspect.Aspects[entity.aspectId].AspectName}，不能在{BaseAspect.Aspects[_aspectId].AspectName}进行操作");
            }
            
            CheckAlive(entity);

            if (!Has(entity))
            {
                throw new Exception(
                    $"{BaseAspect.Aspects[_aspectId].AspectName}中的实体[{entity}]没有组件\"{typeof(T).Name}\"");
            }
#endif

            AddFirst(to) = _data[_sparse[entity.Index]];
        }
        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryDel(in Entity entity)
        {
            if (!Has(entity))
            {
                return false;
            }

            Del(entity);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Del(in Entity entity)
        {
#if DEBUG
            if (entity.aspectId != _aspectId)
            {
                throw new Exception(
                    $"CPool<{typeName}>.Del 实体[{entity}]属于{BaseAspect.Aspects[entity.aspectId].AspectName}，不能在{BaseAspect.Aspects[_aspectId].AspectName}进行操作");
            }
            
            CheckAlive(entity);

            if (!Has(entity))
            {
                throw new Exception(
                    $"{BaseAspect.Aspects[_aspectId].AspectName}中的实体[{entity}]不包含组件\"{typeof(T).Name}\"");
            }
#endif
            ref var idx = ref _sparse[entity.Index];
            if (_recycledItemsCount >= _recycledItems.Length)
            {
                Array.Resize(ref _recycledItems, _recycledItems.Length << 1);
            }

            _data[idx] = default;
            _recycledItems[_recycledItemsCount++] = idx;
            len--;
            idx = 0;

            _baseAspect.Remove(entity, compId);
            RefreshQuery(entity);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal override void InnerDel(in Entity entity)
        {
#if DEBUG
            if (entity.aspectId != _aspectId)
            {
                throw new Exception(
                    $"CPool<{typeName}>.Del 实体[{entity}]属于{BaseAspect.Aspects[entity.aspectId].AspectName}，不能在{BaseAspect.Aspects[_aspectId].AspectName}进行操作");
            }
            
            CheckAlive(entity);

            if (!Has(entity))
            {
                throw new Exception(
                    $"{BaseAspect.Aspects[_aspectId].AspectName}中的实体[{entity}]不包含组件\"{typeof(T).Name}\"");
            }
#endif
            ref var idx = ref _sparse[entity.Index];
            if (_recycledItemsCount >= _recycledItems.Length)
            {
                Array.Resize(ref _recycledItems, _recycledItems.Length << 1);
            }

            _data[idx] = default;
            _recycledItems[_recycledItemsCount++] = idx;
            len--;
            idx = 0;
            for (var i = 0; i < _queries.Length; i++)
            {
                if (_queries[i].HasEntity(entity))
                {
                    _queries[i].DelEntity(entity);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RefreshQuery(in Entity entity)
        {
            var compCount = _baseAspect.edges[_entityGens[entity.Index].edgeIndex].pools.Length;
            for (var i = 0; i < _queries.Length; i++)
            {
                var query = _queries[i];
                if (compCount >= query._incPools.Length && query.IsMatch(entity))
                {
                    query.AddEntity(entity);
                }
                else if (query.HasEntity(entity))
                {
                    query.DelEntity(entity);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal override void Clear()
        {
            Array.Clear(_data, 0, _data.Length);
            Array.Clear(_sparse, 0, _sparse.Length);
            Array.Clear(_recycledItems, 0, _recycledItems.Length);
            len = 0;
            _recycledItemsCount = 0;
        }

#if DEBUG
        public override object Get(in Entity entity)
        {
            return this[entity];
        }

        public override void Set(in Entity entity, object value)
        {
            if (value is T t)
            {
                this[entity] = t;
            }
            else
            {
                throw new Exception($"CPool<{typeName}>.Set 传入的值类型不匹配，期望类型为{typeof(T).Name}，实际类型为{value.GetType().Name}");
            }
        }
#endif
    }
}
