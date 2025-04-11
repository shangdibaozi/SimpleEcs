using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace SimpleEcs
{
    public interface IEcsSystem
    {
        public void OnInit()
        {

        }

        public void OnUpdate()
        {

        }
    }

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    public class ObserverArrayBase
    {
        internal int _len;
        internal int _id;
        
#if DEBUG
        private bool _isLock;
#endif
        
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
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    public class ObserverArray<T> : ObserverArrayBase
    {
        internal T[] arr;
        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObserverEnumerator<T> GetEnumerator()
        {
            return new ObserverEnumerator<T>(this);
        }
    } 

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    public sealed partial class EcsSystemGroup : IEcsSystem
    {
        private static readonly Dictionary<Type, ObserverArrayBase> events = new();
        private readonly List<IEcsSystem> systems = new();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EcsSystemGroup Add<T>() where T : IEcsSystem, new()
        {
            systems.Add(Sys<T>());
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EcsSystemGroup Add(EcsSystemGroup group)
        {
            systems.Add(group);
            return this;
        }

        /// <summary>
        /// 添加事件接口系统
        ///
        /// 比如T为
        ///     interface IData
        ///     {
        ///         void Run();
        ///     }
        /// </summary>
        /// <typeparam name="S">System</typeparam>
        /// <typeparam name="T">事件接口</typeparam>
        /// <returns></returns>
        public EcsSystemGroup Add<S, T>() where S : IEcsSystem, T, new()
        {
            var t = typeof(T);
            if (!events.ContainsKey(t))
            {
                events[t] = new ObserverArray<T>
                {
                    arr = new T[1],
                    _len = 0
                };
            }
            
            var e = (ObserverArray<T>)events[t];
            if (e._len >= e.arr.Length)
            {
                Array.Resize(ref e.arr, e.arr.Length << 1);
            }

            e.arr[e._len++] = Sys<S>();
            if (!systems.Contains(Sys<S>()))
            {
                Add<S>();
            }
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ObserverArray<T> GetObservers<T>()
        {
            if (events.TryGetValue(typeof(T), out var e))
            {
                return (ObserverArray<T>)e;
            }

            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnInit()
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < systems.Count; i++)
            {
                systems[i].OnInit();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnUpdate()
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < systems.Count; i++)
            {
                systems[i].OnUpdate();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            systems.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Sys<T>() where T : IEcsSystem, new()
        {
            return System<T>.inst;
        }
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    public readonly ref struct ObserverEnumerator<T>
    {
        private readonly ObserverArray<T> _observers;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObserverEnumerator(ObserverArray<T> observers)
        {
            _observers = observers;
            observers.Begin();
        }

        public T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _observers.arr[_observers._id];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => _observers.Next();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() => _observers.End();
    }

    [Il2CppSetOption(Option.NullChecks, false)]
    internal abstract class System<T> where T : IEcsSystem, new()
    {
        internal static readonly T inst;

        static System()
        {
            inst = new T();
        }
    }
}