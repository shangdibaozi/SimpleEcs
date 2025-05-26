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
    
    /// <summary>
    /// 所有自定义事件的接口需要实现此接口才能通关代码生成器生成代码
    /// </summary>
    public interface IObserver
    {
        
    }

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    public sealed partial class EcsSystemGroup : IEcsSystem
    {
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
    internal abstract class System<T> where T : IEcsSystem, new()
    {
        internal static readonly T inst;

        static System()
        {
            inst = new T();
        }
    }
}