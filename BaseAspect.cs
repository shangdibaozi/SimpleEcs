using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace SimpleEcs
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    internal class Edge
    {
        internal ushort index;
        /// <summary>
        /// 查看当前Edge是哪个组件
        /// </summary>
        internal ushort compId;
        internal SArray<ACPool> pools;
        internal Edge[] addEdges;
        internal Edge[] removeEdges;
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public class BaseAspect
    {
        internal Edge[] edges;
        internal ushort edgeIndex;
#if DEBUG
        public static readonly SArray<string> AspectNames = new SArray<string>(4);
        public static List<SArray<Query>> Queries = new List<SArray<Query>>();
        internal byte aspectId;
#endif
        internal SArray<EntityMeta> entityGens;
        internal SArray<ACPool> _cPools;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsAlive(in Entity entity)
        {
#if DEBUG
            if (entity.Index >= entityGens.Length)
            {
                throw new Exception($"out of range: {entity.Index}");
            }
            
            if (entity.Index != 0 && entity.aspectId != aspectId)
            {
                throw new Exception($"当前Aspect[{AspectNames[aspectId]}] != Aspect[{AspectNames[entity.aspectId]}]");
            }
#endif
            return entityGens[entity.Index].version == entity.Version;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void AddEdge(ushort compId)
        {
            if (compId >= edges.Length)
            {
                Array.Resize(ref edges, edgeIndex << 1);
            }

            edges[compId] = new Edge
            {
                index = compId,
                compId = compId,
                pools = new SArray<ACPool>(1),
                addEdges = Array.Empty<Edge>(),
                removeEdges = Array.Empty<Edge>()
            };
            edges[compId].pools.Add(_cPools[compId]);
            if (compId >= edgeIndex)
            {
                edgeIndex = (ushort)(compId + 1);
            }
        }
        
        internal void Add(in Entity entity, ushort compId)
        {
#if DEBUG
            if (edges[compId] == null)
            {
                throw new NullReferenceException($"No edge[{compId}] found");
            }
#endif
            ref var entityMeta = ref entityGens[entity.Index];
            // if (entityMeta.edgeIndex == ushort.MaxValue)
            // {
            //     entityMeta.edgeIndex = compId;
            //     return;
            // }
            var edge = edges[entityMeta.edgeIndex];
            
            if (compId >= edge.addEdges.Length)
            {
                Array.Resize(ref edge.addEdges, compId + 1);
            }
            
            if (edge.addEdges[compId] == null)
            {
                var newEdge = new Edge
                {
                    index = edgeIndex,
                    compId = compId,
                    pools = new SArray<ACPool>(edge.pools.Length + 1),
                    addEdges = Array.Empty<Edge>(),
                    removeEdges = new Edge[compId + 1]
                };
                if (edgeIndex >= edges.Length)
                {
                    Array.Resize(ref edges, edgeIndex << 1);
                }
                edges[edgeIndex++] = newEdge;
                edge.addEdges[compId] = newEdge;
                
                newEdge.removeEdges[compId] = edge;
                
                newEdge.pools.AddRange(edge.pools);
                newEdge.pools.Add(_cPools[compId]);
            }

            entityMeta.edgeIndex = edge.addEdges[compId].index;
        }

        internal void Remove(in Entity entity, ushort compId)
        {
            ref var entityMeta = ref entityGens[entity.Index];
            var edge = edges[entityMeta.edgeIndex];
            if (edge.index == compId)
            {
                entityMeta.edgeIndex = ushort.MaxValue;
                return;
            }
            if (compId >= edge.removeEdges.Length)
            {
                Array.Resize(ref edge.removeEdges, compId + 1);
            }

            if (edge.removeEdges[compId] == null)
            {
                var newEdge = new Edge
                {
                    index = edgeIndex,
                    compId = compId,
                    pools = new SArray<ACPool>(edge.pools.Length - 1),
                    addEdges = new Edge[compId + 1],
                    removeEdges = Array.Empty<Edge>()
                };
                
                if (edgeIndex >= edges.Length)
                {
                    Array.Resize(ref edges, edgeIndex << 1);
                }
                edges[edgeIndex++] = newEdge;
                edge.removeEdges[compId] = newEdge;
                
                newEdge.addEdges[compId] = edge;

                for (var i = 0; i < edge.pools.Length; i++)
                {
                    if (edge.pools[i].compId != compId)
                    {
                        newEdge.pools.Add(edge.pools[i]);
                    }
                }
            }
            entityMeta.edgeIndex = edge.removeEdges[compId].index;
        }
    }
}