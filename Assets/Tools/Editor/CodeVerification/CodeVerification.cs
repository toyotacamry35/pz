using System;
using System.Linq;
using System.Reflection;
using Assets.ColonyShared.GeneratedCode.Shared.Arithmetic;
using Assets.ColonyShared.SharedCode.Arithmetic;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers.Cluster;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates.Cluster;
using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using Assets.ColonyShared.SharedCode.Utils;
using Core.Reflection;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.CodeVerification
{

    public static class CodeVerification
    {
        [InitializeOnLoadMethod, UsedImplicitly]
        private static void OnCompilationFinished()
        {
            System.Diagnostics.Stopwatch w = new System.Diagnostics.Stopwatch();
            w.Start();
            // Check we 've `CalcItemsChancesDo` for every `LootTableBaseDef` heir.
            string problemCalcerTypeName = null;
            foreach (var asm in AppDomain.CurrentDomain.GetAssembliesSafe())
                foreach (var type in asm.GetTypesSafe())
                {
                    if (type.IsAbstract)
                        continue;

                    // 1. Check `ComputableStateMachinePredicateCalcer`: 
                    if (typeof(ComputableStateMachinePredicateDef).IsAssignableFrom(type))
                    {
                        if (!ComputableStateMachinePredicateCalcer.Implementations.ContainsKey(type))
                            problemCalcerTypeName = nameof(ComputableStateMachinePredicateCalcer);
                    }
                    // 2. Check `LootItemChanceWeightCalcerCalcer`:
                    else if (typeof(LootItemChanceWeightCalcerDef).IsAssignableFrom(type))
                    {
                        if (!LootItemChanceWeightCalcerCalcer.Implementations.ContainsKey(type))
                            problemCalcerTypeName = nameof(LootItemChanceWeightCalcerCalcer);
                    }
                    // 3. Check `LootTableCalcer`:
                    else if (typeof(LootTableBaseDef).IsAssignableFrom(type))
                    {
                        if (!LootTableCalcer.Implementations.ContainsKey(type))
                            problemCalcerTypeName = nameof(LootTableCalcer);
                    }
                    // 4. Check `LootTablePredicateCalcer`:
                    else if (typeof(LootTablePredicateDef).IsAssignableFrom(type))
                    {
                        if (!LootTablePredicateCalcer.Implementations.ContainsKey(type))
                            problemCalcerTypeName = nameof(LootTablePredicateCalcer);
                    }

                    // Print err. if met:
                    if (problemCalcerTypeName != null)
                        Debug.LogError($"`{problemCalcerTypeName}` hasn't implementation of polymorphic calc method for argument `{type.Name}` type.");
                    problemCalcerTypeName = null;
                }

            try
            {
                // verificationMethods =   from assembly in AppDomain.CurrentDomain.GetAssembliesSafe()
                //                         from type in assembly.GetTypesSafe()
                //                         let methods = type.GetMethodsSafe(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)
                //                         from method in methods
                //                         where method.GetCustomAttributesSafe<VerifyAfterCompilationAttribute>(false).Any()
                //                         select method;

                var verificationMethods = AppDomain.CurrentDomain.GetAssembliesSafe()
                    .SelectMany(x => x.GetTypesSafe())
                    .SelectMany(t => t.GetMethodsSafe(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly))
                    .Where(m => m.GetCustomAttributesSafe<VerifyAfterCompilationAttribute>(false).Any());

                bool allSucceed = true;
                foreach (var m in verificationMethods)
                {
                    if (m.GetParameters().Length > 0 || m.ReturnType != typeof(bool))
                    {
                        Debug.LogError($"`CodeVerification`. Some of methods with {nameof(VerifyAfterCompilationAttribute)} has not-empty parameters list || return type is not bool: {m}");
                        continue;
                    }

                    var isSucceed = (bool) m.Invoke(null, null);
                    allSucceed &= isSucceed;
                    if (!isSucceed)
                        Debug.LogError($"`CodeVerification`. Verification failed! Method: {m.DeclaringType}.{m}.");
                }
                if (allSucceed)
                    Debug.Log("`CodeVerification`. Verification finished successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"`CodeVerification` got an exception while verification: \"{e.Message}\"");
            }
            w.Stop();
            Debug.Log($"Code verification finished in {w.Elapsed.TotalSeconds}");

        }
    }
}