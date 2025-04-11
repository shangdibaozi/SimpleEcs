using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace SimpleEcs
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    public class EntityPool
    {
        private Relation[] _relations;
        private ushort[] _sparse;
        private ushort _len;
        private ushort[] _recycledItems;
        private ushort _recycledItemsCount;
        private SArray<EntityMeta> _entityGens;
        
#if DEBUG
        private byte _aspectId;
#endif
        
        internal void Init(BaseAspect baseAspect)
        {
            _entityGens = baseAspect.entityGens;
            var capacity = _entityGens.Capacity;
            _sparse = new ushort[capacity];
            _recycledItems = new ushort[capacity];
            _recycledItemsCount = 0;
            _len = 0;
            _relations = new Relation[4];
#if DEBUG
            _aspectId = baseAspect.aspectId;
#endif
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has(in Entity entity)
        {
            return entity.Index < _sparse.Length && _sparse[entity.Index] > 0;
        }

        public Relation Add(in Entity entity)
        {
            
#if DEBUG
            if (Has(entity))
            {
                throw new Exception($"已经添加了{entity}");
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
                _len++;
            }
            else
            {
                idx = ++_len;
                if (idx >= _relations.Length)
                {
                    Array.Resize(ref _relations, _len << 1);
                }
                _relations[idx] = new Relation(8);
            }
            return _relations[idx];
        }

        public void Remove(in Entity entity)
        {
#if DEBUG
            if (!Has(entity))
            {
                throw new Exception($"不包含{entity}");
            }
#endif
            ref var idx = ref _sparse[entity.Index];
            if (_recycledItemsCount >= _recycledItems.Length)
            {
                Array.Resize(ref _recycledItems, _recycledItems.Length << 1);
            }
            _relations[idx].Clear();
            _recycledItems[_recycledItemsCount++] = idx;
            _len--;
            idx = 0;
        }

        public void Clear()
        {
            
        }
        
        public ref Relation this[in Entity entity]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
#if DEBUG
                if (_entityGens[entity.Index].version != entity.Version)
                {
                    throw new Exception($"{BaseAspect.AspectNames[_aspectId]}中的实体[{entity}]已销毁");
                }

                if (!Has(entity))
                {
                    throw new Exception($"{entity}没有EntityPool");
                }
#endif
                return ref _relations[_sparse[entity.Index]];
            }
        }
    }

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    public class Relation
    {
        internal Entity[] _entities;
        private short _len;
        internal short _id;
        private short[] _sparse;
        
        public int count 
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _len;
        }
#if DEBUG
        private bool _isLock;
#endif
        public Relation(int capacity)
        {
            _entities = new Entity[capacity];
            _sparse = new short[capacity];
            _len = 0;
            _id = 0;
        }

        public void Add(in Entity entity)
        {
            if (entity.Index >= _sparse.Length)
            {
                Array.Resize(ref _sparse, entity.Index << 1);
            }

            ref var idx = ref _sparse[entity.Index];
            idx = _len++;
            if (idx >= _entities.Length)
            {
                Array.Resize(ref _entities, _len << 1);
            }
            _entities[idx] = entity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Del(in Entity entity)
        {
#if DEBUG
            if (!Has(entity))
            {
                throw new Exception($"未关联实体：{entity}");
            }   
#endif
            _len--;
            if (_len < _id)
            {
                _id = _len;
            }
            ref var idx = ref _sparse[entity.Index];
            _entities[idx] = _entities[_len];
            _entities[_len] = default;
            _sparse[_entities[idx].Index] = idx;
            idx = -1;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has(in Entity entity)
        {
            return entity.Index < _sparse.Length && _sparse[entity.Index] >= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            Array.Clear(_entities, 0, _entities.Length);
            _len = 0;
            _id = 0;
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
        public RelationsEnumerator GetEnumerator()
        {
            return new RelationsEnumerator(this);
        }

    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    public readonly ref struct RelationsEnumerator
    {
        private readonly Relation _relation;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RelationsEnumerator(Relation relation)
        {
            _relation = relation;
            _relation.Begin();
        }

        public Entity Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _relation._entities[_relation._id];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => _relation.Next();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() => _relation.End();
    }
}