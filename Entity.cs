using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace SimpleEcs
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [Serializable]
    public partial struct Entity : IEquatable<Entity>
    {
#if SIMPLE_ECS_ENABLE_INT
        internal int Index;
#else
        internal ushort Index;
#endif
#if DEBUG
        internal byte aspectId;
#endif
        internal ushort Version;

        public int Id
        {

#if SIMPLE_ECS_ENABLE_INT
            get => (Version << 16) | Index;
#else
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (Version << 16) + Index;
#endif
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => Id;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
#if DEBUG
            return $"{BaseAspect.AspectNames[aspectId]} Entity:{Index}_{Version} Id: {Id}";
#else
            
            return $"Entity:{Index}_{Version}";
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Entity rhs) => this == rhs;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in Entity lhs, in Entity rhs)
        {
#if DEBUG

            return lhs.Index == rhs.Index && lhs.Version == rhs.Version && lhs.aspectId == rhs.aspectId;
#else
            return lhs.Index == rhs.Index && lhs.Version == rhs.Version;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in Entity lhs, in Entity rhs)
        {
            return !(lhs == rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(Object obj)
        {
            return obj is Entity rhs && this == rhs;
        }
    }
}