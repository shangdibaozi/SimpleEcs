using System.Runtime.CompilerServices;

namespace SimpleEcs
{
    public sealed partial class EcsSystemGroup
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update<S1>() where S1 : IEcsSystem, new()
        {
            System<S1>.inst.OnUpdate();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update<S1, S2>() where S1 : IEcsSystem, new() where S2 : IEcsSystem, new()
        {
            System<S1>.inst.OnUpdate();
            System<S2>.inst.OnUpdate();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update<S1, S2, S3>() where S1 : IEcsSystem, new() where S2 : IEcsSystem, new() where S3 : IEcsSystem, new()
        {
            System<S1>.inst.OnUpdate();
            System<S2>.inst.OnUpdate();
            System<S3>.inst.OnUpdate();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update<S1, S2, S3, S4>()
            where S1 : IEcsSystem, new() 
            where S2 : IEcsSystem, new() 
            where S3 : IEcsSystem, new()
            where S4 : IEcsSystem, new()
        {
            System<S1>.inst.OnUpdate();
            System<S2>.inst.OnUpdate();
            System<S3>.inst.OnUpdate();
            System<S4>.inst.OnUpdate();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update<S1, S2, S3, S4, S5>()
            where S1 : IEcsSystem, new() 
            where S2 : IEcsSystem, new() 
            where S3 : IEcsSystem, new()
            where S4 : IEcsSystem, new()
            where S5 : IEcsSystem, new()
        {
            System<S1>.inst.OnUpdate();
            System<S2>.inst.OnUpdate();
            System<S3>.inst.OnUpdate();
            System<S4>.inst.OnUpdate();
            System<S5>.inst.OnUpdate();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update<S1, S2, S3, S4, S5, S6>()
            where S1 : IEcsSystem, new() 
            where S2 : IEcsSystem, new() 
            where S3 : IEcsSystem, new()
            where S4 : IEcsSystem, new()
            where S5 : IEcsSystem, new()
            where S6 : IEcsSystem, new()
        {
            System<S1>.inst.OnUpdate();
            System<S2>.inst.OnUpdate();
            System<S3>.inst.OnUpdate();
            System<S4>.inst.OnUpdate();
            System<S5>.inst.OnUpdate();
            System<S6>.inst.OnUpdate();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update<S1, S2, S3, S4, S5, S6, S7>()
            where S1 : IEcsSystem, new() 
            where S2 : IEcsSystem, new() 
            where S3 : IEcsSystem, new()
            where S4 : IEcsSystem, new()
            where S5 : IEcsSystem, new()
            where S6 : IEcsSystem, new()
            where S7 : IEcsSystem, new()
        {
            System<S1>.inst.OnUpdate();
            System<S2>.inst.OnUpdate();
            System<S3>.inst.OnUpdate();
            System<S4>.inst.OnUpdate();
            System<S5>.inst.OnUpdate();
            System<S6>.inst.OnUpdate();
            System<S7>.inst.OnUpdate();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update<S1, S2, S3, S4, S5, S6, S7, S8>()
            where S1 : IEcsSystem, new() 
            where S2 : IEcsSystem, new() 
            where S3 : IEcsSystem, new()
            where S4 : IEcsSystem, new()
            where S5 : IEcsSystem, new()
            where S6 : IEcsSystem, new()
            where S7 : IEcsSystem, new()
            where S8 : IEcsSystem, new()
        {
            System<S1>.inst.OnUpdate();
            System<S2>.inst.OnUpdate();
            System<S3>.inst.OnUpdate();
            System<S4>.inst.OnUpdate();
            System<S5>.inst.OnUpdate();
            System<S6>.inst.OnUpdate();
            System<S7>.inst.OnUpdate();
            System<S8>.inst.OnUpdate();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update<S1, S2, S3, S4, S5, S6, S7, S8, S9>()
            where S1 : IEcsSystem, new() 
            where S2 : IEcsSystem, new() 
            where S3 : IEcsSystem, new()
            where S4 : IEcsSystem, new()
            where S5 : IEcsSystem, new()
            where S6 : IEcsSystem, new()
            where S7 : IEcsSystem, new()
            where S8 : IEcsSystem, new()
            where S9 : IEcsSystem, new()
        {
            System<S1>.inst.OnUpdate();
            System<S2>.inst.OnUpdate();
            System<S3>.inst.OnUpdate();
            System<S4>.inst.OnUpdate();
            System<S5>.inst.OnUpdate();
            System<S6>.inst.OnUpdate();
            System<S7>.inst.OnUpdate();
            System<S8>.inst.OnUpdate();
            System<S9>.inst.OnUpdate();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10>()
            where S1 : IEcsSystem, new() 
            where S2 : IEcsSystem, new() 
            where S3 : IEcsSystem, new()
            where S4 : IEcsSystem, new()
            where S5 : IEcsSystem, new()
            where S6 : IEcsSystem, new()
            where S7 : IEcsSystem, new()
            where S8 : IEcsSystem, new()
            where S9 : IEcsSystem, new()
            where S10 : IEcsSystem, new()
        {
            System<S1>.inst.OnUpdate();
            System<S2>.inst.OnUpdate();
            System<S3>.inst.OnUpdate();
            System<S4>.inst.OnUpdate();
            System<S5>.inst.OnUpdate();
            System<S6>.inst.OnUpdate();
            System<S7>.inst.OnUpdate();
            System<S8>.inst.OnUpdate();
            System<S9>.inst.OnUpdate();
            System<S10>.inst.OnUpdate();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11>()
            where S1 : IEcsSystem, new() 
            where S2 : IEcsSystem, new() 
            where S3 : IEcsSystem, new()
            where S4 : IEcsSystem, new()
            where S5 : IEcsSystem, new()
            where S6 : IEcsSystem, new()
            where S7 : IEcsSystem, new()
            where S8 : IEcsSystem, new()
            where S9 : IEcsSystem, new()
            where S10 : IEcsSystem, new()
            where S11 : IEcsSystem, new()
        {
            System<S1>.inst.OnUpdate();
            System<S2>.inst.OnUpdate();
            System<S3>.inst.OnUpdate();
            System<S4>.inst.OnUpdate();
            System<S5>.inst.OnUpdate();
            System<S6>.inst.OnUpdate();
            System<S7>.inst.OnUpdate();
            System<S8>.inst.OnUpdate();
            System<S9>.inst.OnUpdate();
            System<S10>.inst.OnUpdate();
            System<S11>.inst.OnUpdate();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12>()
            where S1 : IEcsSystem, new() 
            where S2 : IEcsSystem, new() 
            where S3 : IEcsSystem, new()
            where S4 : IEcsSystem, new()
            where S5 : IEcsSystem, new()
            where S6 : IEcsSystem, new()
            where S7 : IEcsSystem, new()
            where S8 : IEcsSystem, new()
            where S9 : IEcsSystem, new()
            where S10 : IEcsSystem, new()
            where S11 : IEcsSystem, new()
            where S12 : IEcsSystem, new()
        {
            System<S1>.inst.OnUpdate();
            System<S2>.inst.OnUpdate();
            System<S3>.inst.OnUpdate();
            System<S4>.inst.OnUpdate();
            System<S5>.inst.OnUpdate();
            System<S6>.inst.OnUpdate();
            System<S7>.inst.OnUpdate();
            System<S8>.inst.OnUpdate();
            System<S9>.inst.OnUpdate();
            System<S10>.inst.OnUpdate();
            System<S11>.inst.OnUpdate();
            System<S12>.inst.OnUpdate();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13>()
            where S1 : IEcsSystem, new() 
            where S2 : IEcsSystem, new() 
            where S3 : IEcsSystem, new()
            where S4 : IEcsSystem, new()
            where S5 : IEcsSystem, new()
            where S6 : IEcsSystem, new()
            where S7 : IEcsSystem, new()
            where S8 : IEcsSystem, new()
            where S9 : IEcsSystem, new()
            where S10 : IEcsSystem, new()
            where S11 : IEcsSystem, new()
            where S12 : IEcsSystem, new()
            where S13 : IEcsSystem, new()
        {
            System<S1>.inst.OnUpdate();
            System<S2>.inst.OnUpdate();
            System<S3>.inst.OnUpdate();
            System<S4>.inst.OnUpdate();
            System<S5>.inst.OnUpdate();
            System<S6>.inst.OnUpdate();
            System<S7>.inst.OnUpdate();
            System<S8>.inst.OnUpdate();
            System<S9>.inst.OnUpdate();
            System<S10>.inst.OnUpdate();
            System<S11>.inst.OnUpdate();
            System<S12>.inst.OnUpdate();
            System<S13>.inst.OnUpdate();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14>()
            where S1 : IEcsSystem, new() 
            where S2 : IEcsSystem, new() 
            where S3 : IEcsSystem, new()
            where S4 : IEcsSystem, new()
            where S5 : IEcsSystem, new()
            where S6 : IEcsSystem, new()
            where S7 : IEcsSystem, new()
            where S8 : IEcsSystem, new()
            where S9 : IEcsSystem, new()
            where S10 : IEcsSystem, new()
            where S11 : IEcsSystem, new()
            where S12 : IEcsSystem, new()
            where S13 : IEcsSystem, new()
            where S14 : IEcsSystem, new()
        {
            System<S1>.inst.OnUpdate();
            System<S2>.inst.OnUpdate();
            System<S3>.inst.OnUpdate();
            System<S4>.inst.OnUpdate();
            System<S5>.inst.OnUpdate();
            System<S6>.inst.OnUpdate();
            System<S7>.inst.OnUpdate();
            System<S8>.inst.OnUpdate();
            System<S9>.inst.OnUpdate();
            System<S10>.inst.OnUpdate();
            System<S11>.inst.OnUpdate();
            System<S12>.inst.OnUpdate();
            System<S13>.inst.OnUpdate();
            System<S14>.inst.OnUpdate();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15>()
            where S1 : IEcsSystem, new() 
            where S2 : IEcsSystem, new() 
            where S3 : IEcsSystem, new()
            where S4 : IEcsSystem, new()
            where S5 : IEcsSystem, new()
            where S6 : IEcsSystem, new()
            where S7 : IEcsSystem, new()
            where S8 : IEcsSystem, new()
            where S9 : IEcsSystem, new()
            where S10 : IEcsSystem, new()
            where S11 : IEcsSystem, new()
            where S12 : IEcsSystem, new()
            where S13 : IEcsSystem, new()
            where S14 : IEcsSystem, new()
            where S15 : IEcsSystem, new()
        {
            System<S1>.inst.OnUpdate();
            System<S2>.inst.OnUpdate();
            System<S3>.inst.OnUpdate();
            System<S4>.inst.OnUpdate();
            System<S5>.inst.OnUpdate();
            System<S6>.inst.OnUpdate();
            System<S7>.inst.OnUpdate();
            System<S8>.inst.OnUpdate();
            System<S9>.inst.OnUpdate();
            System<S10>.inst.OnUpdate();
            System<S11>.inst.OnUpdate();
            System<S12>.inst.OnUpdate();
            System<S13>.inst.OnUpdate();
            System<S14>.inst.OnUpdate();
            System<S15>.inst.OnUpdate();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16>()
            where S1 : IEcsSystem, new() 
            where S2 : IEcsSystem, new() 
            where S3 : IEcsSystem, new()
            where S4 : IEcsSystem, new()
            where S5 : IEcsSystem, new()
            where S6 : IEcsSystem, new()
            where S7 : IEcsSystem, new()
            where S8 : IEcsSystem, new()
            where S9 : IEcsSystem, new()
            where S10 : IEcsSystem, new()
            where S11 : IEcsSystem, new()
            where S12 : IEcsSystem, new()
            where S13 : IEcsSystem, new()
            where S14 : IEcsSystem, new()
            where S15 : IEcsSystem, new()
            where S16 : IEcsSystem, new()
        {
            System<S1>.inst.OnUpdate();
            System<S2>.inst.OnUpdate();
            System<S3>.inst.OnUpdate();
            System<S4>.inst.OnUpdate();
            System<S5>.inst.OnUpdate();
            System<S6>.inst.OnUpdate();
            System<S7>.inst.OnUpdate();
            System<S8>.inst.OnUpdate();
            System<S9>.inst.OnUpdate();
            System<S10>.inst.OnUpdate();
            System<S11>.inst.OnUpdate();
            System<S12>.inst.OnUpdate();
            System<S13>.inst.OnUpdate();
            System<S14>.inst.OnUpdate();
            System<S15>.inst.OnUpdate();
            System<S16>.inst.OnUpdate();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16, S17>()
            where S1 : IEcsSystem, new() 
            where S2 : IEcsSystem, new() 
            where S3 : IEcsSystem, new()
            where S4 : IEcsSystem, new()
            where S5 : IEcsSystem, new()
            where S6 : IEcsSystem, new()
            where S7 : IEcsSystem, new()
            where S8 : IEcsSystem, new()
            where S9 : IEcsSystem, new()
            where S10 : IEcsSystem, new()
            where S11 : IEcsSystem, new()
            where S12 : IEcsSystem, new()
            where S13 : IEcsSystem, new()
            where S14 : IEcsSystem, new()
            where S15 : IEcsSystem, new()
            where S16 : IEcsSystem, new()
            where S17 : IEcsSystem, new()
        {
            System<S1>.inst.OnUpdate();
            System<S2>.inst.OnUpdate();
            System<S3>.inst.OnUpdate();
            System<S4>.inst.OnUpdate();
            System<S5>.inst.OnUpdate();
            System<S6>.inst.OnUpdate();
            System<S7>.inst.OnUpdate();
            System<S8>.inst.OnUpdate();
            System<S9>.inst.OnUpdate();
            System<S10>.inst.OnUpdate();
            System<S11>.inst.OnUpdate();
            System<S12>.inst.OnUpdate();
            System<S13>.inst.OnUpdate();
            System<S14>.inst.OnUpdate();
            System<S15>.inst.OnUpdate();
            System<S16>.inst.OnUpdate();
            System<S17>.inst.OnUpdate();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16, S17, S18>()
            where S1 : IEcsSystem, new() 
            where S2 : IEcsSystem, new() 
            where S3 : IEcsSystem, new()
            where S4 : IEcsSystem, new()
            where S5 : IEcsSystem, new()
            where S6 : IEcsSystem, new()
            where S7 : IEcsSystem, new()
            where S8 : IEcsSystem, new()
            where S9 : IEcsSystem, new()
            where S10 : IEcsSystem, new()
            where S11 : IEcsSystem, new()
            where S12 : IEcsSystem, new()
            where S13 : IEcsSystem, new()
            where S14 : IEcsSystem, new()
            where S15 : IEcsSystem, new()
            where S16 : IEcsSystem, new()
            where S17 : IEcsSystem, new()
            where S18 : IEcsSystem, new()
        {
            System<S1>.inst.OnUpdate();
            System<S2>.inst.OnUpdate();
            System<S3>.inst.OnUpdate();
            System<S4>.inst.OnUpdate();
            System<S5>.inst.OnUpdate();
            System<S6>.inst.OnUpdate();
            System<S7>.inst.OnUpdate();
            System<S8>.inst.OnUpdate();
            System<S9>.inst.OnUpdate();
            System<S10>.inst.OnUpdate();
            System<S11>.inst.OnUpdate();
            System<S12>.inst.OnUpdate();
            System<S13>.inst.OnUpdate();
            System<S14>.inst.OnUpdate();
            System<S15>.inst.OnUpdate();
            System<S16>.inst.OnUpdate();
            System<S17>.inst.OnUpdate();
            System<S18>.inst.OnUpdate();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16, S17, S18, S19>()
            where S1 : IEcsSystem, new() 
            where S2 : IEcsSystem, new() 
            where S3 : IEcsSystem, new()
            where S4 : IEcsSystem, new()
            where S5 : IEcsSystem, new()
            where S6 : IEcsSystem, new()
            where S7 : IEcsSystem, new()
            where S8 : IEcsSystem, new()
            where S9 : IEcsSystem, new()
            where S10 : IEcsSystem, new()
            where S11 : IEcsSystem, new()
            where S12 : IEcsSystem, new()
            where S13 : IEcsSystem, new()
            where S14 : IEcsSystem, new()
            where S15 : IEcsSystem, new()
            where S16 : IEcsSystem, new()
            where S17 : IEcsSystem, new()
            where S18 : IEcsSystem, new()
            where S19 : IEcsSystem, new()
        {
            System<S1>.inst.OnUpdate();
            System<S2>.inst.OnUpdate();
            System<S3>.inst.OnUpdate();
            System<S4>.inst.OnUpdate();
            System<S5>.inst.OnUpdate();
            System<S6>.inst.OnUpdate();
            System<S7>.inst.OnUpdate();
            System<S8>.inst.OnUpdate();
            System<S9>.inst.OnUpdate();
            System<S10>.inst.OnUpdate();
            System<S11>.inst.OnUpdate();
            System<S12>.inst.OnUpdate();
            System<S13>.inst.OnUpdate();
            System<S14>.inst.OnUpdate();
            System<S15>.inst.OnUpdate();
            System<S16>.inst.OnUpdate();
            System<S17>.inst.OnUpdate();
            System<S18>.inst.OnUpdate();
            System<S19>.inst.OnUpdate();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update<S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16, S17, S18, S19, S20>()
            where S1 : IEcsSystem, new() 
            where S2 : IEcsSystem, new() 
            where S3 : IEcsSystem, new()
            where S4 : IEcsSystem, new()
            where S5 : IEcsSystem, new()
            where S6 : IEcsSystem, new()
            where S7 : IEcsSystem, new()
            where S8 : IEcsSystem, new()
            where S9 : IEcsSystem, new()
            where S10 : IEcsSystem, new()
            where S11 : IEcsSystem, new()
            where S12 : IEcsSystem, new()
            where S13 : IEcsSystem, new()
            where S14 : IEcsSystem, new()
            where S15 : IEcsSystem, new()
            where S16 : IEcsSystem, new()
            where S17 : IEcsSystem, new()
            where S18 : IEcsSystem, new()
            where S19 : IEcsSystem, new()
            where S20 : IEcsSystem, new()
        {
            System<S1>.inst.OnUpdate();
            System<S2>.inst.OnUpdate();
            System<S3>.inst.OnUpdate();
            System<S4>.inst.OnUpdate();
            System<S5>.inst.OnUpdate();
            System<S6>.inst.OnUpdate();
            System<S7>.inst.OnUpdate();
            System<S8>.inst.OnUpdate();
            System<S9>.inst.OnUpdate();
            System<S10>.inst.OnUpdate();
            System<S11>.inst.OnUpdate();
            System<S12>.inst.OnUpdate();
            System<S13>.inst.OnUpdate();
            System<S14>.inst.OnUpdate();
            System<S15>.inst.OnUpdate();
            System<S16>.inst.OnUpdate();
            System<S17>.inst.OnUpdate();
            System<S18>.inst.OnUpdate();
            System<S19>.inst.OnUpdate();
            System<S20>.inst.OnUpdate();
        }
    }
}