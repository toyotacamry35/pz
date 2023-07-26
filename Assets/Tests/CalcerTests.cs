using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using ColonyShared.GeneratedCode;
using ColonyShared.SharedCode.Utils;
using NUnit.Framework;

public static class CalcerTests
{
    [Test]
    public static void TestAllCalcersImplemented()
    {
        var defs = new CalcerDefsCollector();
        var bindings = new CalcerBindingCollector();
        TypesCollector.CollectTypes(defs, bindings);
        foreach (var calcer in defs.Collection.Where(x => !bindings.Collection.TryGetValue(x, out _) && (x.BaseType == null || !bindings.Collection.TryGetValue(x.BaseType, out _))))
            Assert.Fail("Calcer {0} is not implemented", calcer.FullNiceName());
    }

    private class CalcerDefsCollector : ITypesCollectorAgent
    {
        public List<Type> Collection { get;  } = new List<Type>();

        public void Init() {}

        public void CollectType(Type type)
        {
            if (!type.IsAbstract && !type.IsGenericTypeDefinition && typeof(CalcerDef).IsAssignableFrom(type))
                Collection.Add(type);
        }
        
        public void Dump(StringBuilder sb) {}
    }
}
