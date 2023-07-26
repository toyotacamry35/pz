using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ColonyShared.GeneratedCode;
using GeneratedCode.DeltaObjects;
using ResourceSystem.Aspects.ManualDefsForSpells;
using SharedCode.Wizardry;

namespace Assets.Tests
{

    public class EffectCollectorTest
    {
        private static readonly Type[] TestEffects = 
        {
            typeof(TestSpellEffectDef),
            typeof(EffectTestDef)
        };
        
        [Test]
        public void GetEffects()
        {
            var effectDefs = new EffectsDefsCollector();
            var effects = new SpellEffects.EffectBindingsCollector();
            TypesCollector.CollectTypes(effectDefs, effects);

            string[] aggregate; 
                
            aggregate = effectDefs.Collection.Where(x => !effects.Collection.TryGetValue(x, out _)).Select(x => $"Effect {x.FullNiceName()} is not implemented").ToArray();
            if (aggregate.Length > 0)
                Assert.Fail(string.Join("\n", aggregate));
            
            aggregate = effectDefs.Collection.Select(x => (Def: x, Binding: effects.Collection[x])).Where(t => t.Binding is IClientOnlyEffectBinding && !effects.ClientOnlyEffects.Contains(t.Def)).Select(x => $"Effect {x.Binding.GetType().FullNiceName()} is not client only").ToArray();
            if (aggregate.Length > 0)
                Assert.Fail(string.Join("\n", aggregate));
        }
        
        private class EffectsDefsCollector : TypesCollectorAgent
        {
            public List<Type> Collection { get;  } = new List<Type>();
            public override void CollectType(Type type)
            {
                if (!type.IsAbstract && !type.IsGenericTypeDefinition && typeof(SpellEffectDef).IsAssignableFrom(type) && !TestEffects.Contains(type))
                    Collection.Add(type);
            }
        }
    }
}
