using System;
using System.Reflection;
using Unity.IL2CPP.CompilerServices;

namespace SimpleEcs
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public abstract partial class Aspect<A> where A : IAspect
    {
        private void Inject()
        {
            var iCPoolType = typeof(ACPool);
            foreach (var f in GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (iCPoolType.IsAssignableFrom(f.FieldType))
                {
                    var pool = (ACPool)f.GetValue(this);
                    if (pool == null)
                    {
                        pool = (ACPool)Activator.CreateInstance(f.FieldType);
                    }
                    else
                    {
                        throw new Exception($"{typeof(A).Name}中{f.FieldType}已创建实例");
                    }

                    var compId = pool.Init<A>(this);
#if DEBUG
                    if (compId < _cPools.Length && _cPools[compId] != null)
                    {
                        throw new Exception($"{typeof(A).Name}重复创建{f.FieldType}");
                    }
#endif
                    _cPools.Add(pool);
                    f.SetValue(this, pool);
                    AddEdge(compId);
                }
            }
        }
    }
}