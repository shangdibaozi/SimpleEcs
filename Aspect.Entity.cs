using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace SimpleEcs
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    internal struct EntityMeta
    {
        internal ushort edgeIndex;
        internal ushort version;
    }

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public abstract partial class Aspect<A> where A : IAspect
    {
        public Entity _tmpNewEntity;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Entity NewEntity()
        {
            Entity entity = default;
            if (_recycled.Length > 0)
            {
                entity = _recycled.RemoveLast();
                entity.Version++;
            }
            else
            {
#if SIMPLE_ECS_ENABLE_INT
                e.Id = _entityGens.Length;
#else
                entity.Index = (ushort)entityGens.Length;
                
#if DEBUG
                if (entity.Index == ushort.MaxValue - 1)
                {
                    throw new Exception("实体数量超过上限");
                }
#endif
                
#endif
                entity.Version = 1;

#if DEBUG
                entity.aspectId = aspectId;
#endif
                entityGens.Add(default);
            }

            ref var entityMeta = ref entityGens[entity.Index];
            entityMeta.version = entity.Version;
            entityMeta.edgeIndex = ushort.MaxValue;
            return entity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void DestroyEntity(in Entity entity)
        {
            if (entity == default)
            {
                return;
            }

            ref var entityMeta = ref entityGens[entity.Index];
#if DEBUG
            if (entity.Index != 0 && entity.aspectId != aspectId)
            {
                throw new Exception($"当前Aspect[{AspectNames[aspectId]}] != Aspect[{AspectNames[entity.aspectId]}]");
            }
            
            if (entityMeta.version == 0)
            {
                throw new Exception($"重复删除: {entity}");
            }
#endif
            var pools = edges[entityMeta.edgeIndex].pools;
            for (var i = 0; i < pools.Length; i++)
            {
                pools[i].InnerDel(entity);
            }
            
            entityMeta.version = 0;
            _recycled.Add(entity);
        }

#if DEBUG
        private void CheckAddTagComponent<T>() where T : struct
        {
            var fields  = typeof(T).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (fields.Length == 0)
            {
                throw new Exception($"使用WithTag添加{nameof(T)}组件");
            }
        }
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Aspect<A> NewEntityWith<T>(in T data) where T : struct
        {
#if DEBUG
            CheckAddTagComponent<T>();   
#endif
            _tmpNewEntity = NewEntity();
            entityGens[_tmpNewEntity.Index].edgeIndex = AspectComponent<T>.Id;
            CPool<T>().AddFirst(_tmpNewEntity) = data;
            return this;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Aspect<A> NewEntityWith<T>() where T : struct
        {
            _tmpNewEntity = NewEntity();
            entityGens[_tmpNewEntity.Index].edgeIndex = AspectComponent<T>.Id;
            CPool<T>().AddFirst(_tmpNewEntity);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Aspect<A> With<T>(in T data) where T : struct
        {
#if DEBUG
            if (CTagPool<T>() != null && CPool<T>() == null)
            {
                throw new Exception($"{typeof(T)} is missing fields, requires WithTag");
            }
            CheckAddTagComponent<T>();
#endif
            CPool<T>().Add(_tmpNewEntity) = data;
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Aspect<A> With<T>() where T : struct
        {
#if DEBUG
            if (CTagPool<T>() != null && CPool<T>() == null)
            {
                throw new Exception($"{typeof(T)}  can't add to CPool");
            }
            CheckAddTagComponent<T>();
#endif
            CPool<T>().Add(_tmpNewEntity);
            return this;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Aspect<A> WithTag<T>() where T : struct
        {
#if DEBUG
            if (CPool<T>() != null)
            {
                throw new Exception($"{typeof(T)} can't add to CPool");
            }

            if (CTagPool<T>() == null)
            {
                throw new Exception($"{typeof(T)} CTagPool is null");
            }
#endif
            CTagPool<T>().Add(_tmpNewEntity);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Aspect<A> With(ushort compId)
        {
            _cPools[compId].IAdd(_tmpNewEntity);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity EndWith()
        {
            return _tmpNewEntity;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity FromId(int id)
        {
            return new Entity
            {
#if SIMPLE_ECS_ENABLE_INT
                Index = index,
#else
                Index = (ushort)(id & 0x0000ffff),
#endif
                Version = (ushort)(id >> 16),
#if DEBUG

                aspectId = aspectId
#endif
            };
        }

        public Entity Clone(in Entity entity)
        {
#if DEBUG
            if (entity.Index != 0 && entity.aspectId != aspectId)
            {
                throw new Exception($"当前Aspect[{AspectNames[aspectId]}] != Aspect[{AspectNames[entity.aspectId]}]");
            }
#endif
            var newEntity = NewEntity();

            ref var entityMeta = ref entityGens[entity.Index];
            var pools = edges[entityMeta.edgeIndex].pools;
            entityGens[newEntity.Index].edgeIndex = pools[0].compId;
            pools[0].ICloneFirst(entity, newEntity);
            for (var i = 1; i < pools.Length; i++)
            {
                pools[i].IClone(entity, newEntity);
            }

            return newEntity;
        }

#if DEBUG
        public SArray<ACPool> GetPools(in Entity entity)
        {
            ref var entityMeta = ref entityGens[entity.Index];
            return edges[entityMeta.edgeIndex].pools;
        }
#endif
    }
}