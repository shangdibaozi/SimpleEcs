using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;
using UnityEngine.Scripting;

namespace SimpleEcs
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public class CTagPool<T> : ACPool where T : struct
    {
        private SArray<EntityMeta> _entityGens;
        private BaseAspect _baseAspect;

        [Preserve]
        public CTagPool()
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
            len = 0;

#if SIMPLE_ECS_ENABLE_INT
            _sparse = new int[capacity];
#else
            _sparse = new ushort[capacity];
#endif
            return compId;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(in Entity entity)
        {
#if DEBUG
            if (entity.aspectId != _aspectId)
            {
                throw new Exception(
                    $"CTagPool<{typeof(T).Name}>.Add 实体属于{BaseAspect.AspectNames[entity.aspectId]}:{entity.aspectId}，不能在{BaseAspect.AspectNames[_aspectId]}:{_aspectId}进行操作");
            }

            if (Has(entity))
            {
                throw new Exception(
                    $"{BaseAspect.AspectNames[_aspectId]}中的实体[{entity}]重复添加组件\"{typeof(T).Name}\"");
            }
#endif
            if (entity.Index >= _sparse.Length)
            {
                Array.Resize(ref _sparse, _entityGens.Capacity);
            }

            _sparse[entity.Index] = 1;
            len++;
            
            _baseAspect.Add(entity, compId);
            RefreshQuery(entity);
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
                    $"CPool<{typeof(T).Name}>.Add 实体属于{BaseAspect.AspectNames[entity.aspectId]}，不能在{BaseAspect.AspectNames[_aspectId]}进行操作");
            }

            if (!Has(entity))
            {
                throw new Exception(
                    $"{BaseAspect.AspectNames[_aspectId]}中的实体[{entity}]没有组件\"{typeof(T).Name}\"");
            }
#endif

            Add(to);
        }

        internal override void ICloneFirst(in Entity entity, in Entity to)
        {
#if DEBUG
            if (entity.aspectId != _aspectId)
            {
                throw new Exception(
                    $"CTagPool<{typeof(T).Name}>.Add 实体属于{BaseAspect.AspectNames[entity.aspectId]}:{entity.aspectId}，不能在{BaseAspect.AspectNames[_aspectId]}:{_aspectId}进行操作");
            }

            if (Has(entity))
            {
                throw new Exception(
                    $"{BaseAspect.AspectNames[_aspectId]}中的实体[{entity}]重复添加组件\"{typeof(T).Name}\"");
            }
#endif
            if (entity.Index >= _sparse.Length)
            {
                Array.Resize(ref _sparse, _entityGens.Capacity);
            }

            _sparse[entity.Index] = 1;
            len++;
            
            RefreshQuery(entity);
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
                    $"CPool<{typeName}>.Del 实体[{entity}]属于{BaseAspect.AspectNames[entity.aspectId]}，不能在{BaseAspect.AspectNames[_aspectId]}进行操作");
            }

            if (!Has(entity))
            {
                throw new Exception(
                    $"{BaseAspect.AspectNames[_aspectId]}中的实体[{entity}]不包含组件\"{typeof(T).Name}\"");
            }
#endif
            _sparse[entity.Index] = 0;
            len--;

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
                    $"CPool<{typeName}>.Del 实体[{entity}]属于{BaseAspect.AspectNames[entity.aspectId]}，不能在{BaseAspect.AspectNames[_aspectId]}进行操作");
            }

            if (!Has(entity))
            {
                throw new Exception(
                    $"{BaseAspect.AspectNames[_aspectId]}中的实体[{entity}]不包含组件\"{typeof(T).Name}\"");
            }
#endif
            _sparse[entity.Index] = 0;
            len--;
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
            Array.Clear(_sparse, 0, _sparse.Length);
            len = 0;
        }

#if DEBUG
        public override object Get(in Entity entity)
        {
            return null;
        }
#endif
    }
}