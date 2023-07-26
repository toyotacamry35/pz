using System.Threading.Tasks;
 using ColonyShared.ManualDefsForSpells;
 using GeneratedCode.DeltaObjects;
 using SharedCode.EntitySystem;
 using SharedCode.Wizardry;
 
 namespace Assets.Src.Predicates
 {
     public class PredicateLogicalAnd : IPredicateBinding<PredicateLogicalAndDef>
     {
         public async ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repo, PredicateLogicalAndDef def)
         {
             foreach (var predicate in def.Predicates)
                 if (!await PredicateHelper.CheckPredicate(predicate, cast, repo))
                     return false;
             return true;
         }
     }
 }