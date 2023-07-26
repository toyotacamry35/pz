using System;
using System.Reflection;
using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using SharedCode.EntitySystem;
using UnityEngine;

namespace Assets.Src.Arithmetic
{
    public static class DelegateCreator
    {
        // --- for CalcerCalcer -----------------------------------------------------------------------
        #region for_CalcerCalcer

        private static Func<GameObject, BaseType, TReturn> MagicMethodHelper<BaseType, TargetType, TReturn>(MethodInfo method) where TargetType : BaseType
        {
            // Convert the slow MethodInfo into a fast, strongly typed, open delegate
            var func = (Func<GameObject, TargetType, TReturn>)Delegate.CreateDelegate
                (typeof(Func<GameObject, TargetType, TReturn>), method);

            // Now create a more weakly typed delegate which will call the strongly typed one
            Func<GameObject, BaseType, TReturn> ret = (GameObject obj, BaseType param) => func(obj, (TargetType)param);
            return ret;
        }

        public static Func<GameObject, BaseType, TReturn> MagicMethod<BaseType, TReturn>(MethodInfo method)
        {
            // First fetch the generic form
            MethodInfo genericHelper = MethodBase.GetCurrentMethod().DeclaringType.GetMethod(nameof(MagicMethodHelper),
                BindingFlags.Static | BindingFlags.NonPublic);

            // Now supply the type arguments
            MethodInfo constructedHelper = genericHelper.MakeGenericMethod(typeof(BaseType), method.GetParameters()[1].ParameterType, typeof(TReturn));

            // Now call it. The null argument is because it’s a static method.
            object ret = constructedHelper.Invoke(null, new object[] { method });

            // Cast the result to the right kind of delegate and return it
            return (Func<GameObject, BaseType, TReturn>)ret;
        }

        private static Func<TContext, BaseType, TReturn> MagicMethodHelperAsync<TContext, BaseType, TargetType, TReturn>(MethodInfo method) where TargetType : BaseType
        {
            // Convert the slow MethodInfo into a fast, strongly typed, open delegate
            var func = (Func<TContext, TargetType, TReturn>)Delegate.CreateDelegate
                (typeof(Func<TContext, TargetType, TReturn>), method);

            // Now create a more weakly typed delegate which will call the strongly typed one
            Func<TContext, BaseType, TReturn> ret = (context, param) => func(context, (TargetType)param);
            return ret;
        }

        public static Func<TContext, BaseType, TReturn> MagicMethodAsync<TContext, BaseType, TReturn>(MethodInfo method)
        {
            // First fetch the generic form
            MethodInfo genericHelper = MethodBase.GetCurrentMethod().DeclaringType.GetMethod(nameof(MagicMethodHelperAsync),
                BindingFlags.Static | BindingFlags.NonPublic);

            // Now supply the type arguments
            MethodInfo constructedHelper = genericHelper.MakeGenericMethod(typeof(TContext), typeof(BaseType), method.GetParameters()[1].ParameterType, typeof(TReturn));

            // Now call it. The null argument is because it’s a static method.
            object ret = constructedHelper.Invoke(null, new object[] { method });

            // Cast the result to the right kind of delegate and return it
            return (Func<TContext, BaseType, TReturn>)ret;
        }

        private static Func<BaseType, TReturn> CollectStatNotifiersMagicMethodHelperAsync<BaseType, TargetType, TReturn>(MethodInfo method) where TargetType : BaseType
        {
            // Convert the slow MethodInfo into a fast, strongly typed, open delegate
            var func = (Func<TargetType, TReturn>)Delegate.CreateDelegate
                (typeof(Func<TargetType, TReturn>), method);

            // Now create a more weakly typed delegate which will call the strongly typed one
            Func<BaseType, TReturn> ret = (param) => func((TargetType)param);
            return ret;
        }

        public static Func<BaseType, TReturn> CollectStatNotifiersMagicMethodAsync<BaseType, TReturn>(MethodInfo method)
        {
            // First fetch the generic form
            MethodInfo genericHelper = MethodBase.GetCurrentMethod().DeclaringType.GetMethod(nameof(CollectStatNotifiersMagicMethodHelperAsync),
                BindingFlags.Static | BindingFlags.NonPublic);
            var tp = method.GetParameters()[0].ParameterType;
            // Now supply the type arguments
            MethodInfo constructedHelper = genericHelper.MakeGenericMethod(typeof(BaseType), method.GetParameters()[0].ParameterType, typeof(TReturn));

            // Now call it. The null argument is because it’s a static method.
            object ret = constructedHelper.Invoke(null, new object[] { method });

            // Cast the result to the right kind of delegate and return it
            return (Func<BaseType, TReturn>)ret;
        }

        #endregion //for_CalcerCalcer

        // --- for LootTable -----------------------------------------------------------------------
        #region for_LootTable

        private static Func<BaseType, LootListRequest, Guid, int, IEntitiesRepository, TReturn> LootTableMagicMethodHelper<BaseType, TargetType, TReturn>(MethodInfo method) where TargetType : BaseType
        {
            // Convert the slow MethodInfo into a fast, strongly typed, open delegate
            var func = (Func<TargetType, LootListRequest, Guid, int, IEntitiesRepository, TReturn>)Delegate.CreateDelegate
                (typeof(Func<TargetType, LootListRequest, Guid, int, IEntitiesRepository, TReturn>), method);

            // Now create a more weakly typed delegate which will call the strongly typed one
            Func<BaseType, LootListRequest, Guid, int, IEntitiesRepository, TReturn> ret = (BaseType lootTable, LootListRequest lootRequest, Guid lootedEntityId, int lootedEntityTypeId, IEntitiesRepository repo) => func((TargetType)lootTable, lootRequest, lootedEntityId, lootedEntityTypeId, repo);
            return ret;
        }

        public static Func<BaseType, LootListRequest, Guid, int, IEntitiesRepository, TReturn> LootTableMagicMethod<BaseType, TReturn>(MethodInfo method)
        {
            // First fetch the generic form
            MethodInfo genericHelper = MethodBase.GetCurrentMethod().DeclaringType.GetMethod(nameof(LootTableMagicMethodHelper), BindingFlags.Static | BindingFlags.NonPublic);

            // Now supply the type arguments
            MethodInfo constructedHelper = genericHelper.MakeGenericMethod(typeof(BaseType), method.GetParameters()[0].ParameterType, typeof(TReturn));

            // Now call it. The null argument is because it’s a static method.
            object ret = constructedHelper.Invoke(null, new object[] { method });

            // Cast the result to the right kind of delegate and return it
            return (Func<BaseType, LootListRequest, Guid, int, IEntitiesRepository, TReturn>)ret;
        }

    #endregion //for_LootTable

    // --- for LootTable predicates-----------------------------------------------------------------------
    #region for_LootTable_predicates

        private static Func<BaseType, LootItemData, LootListRequest, Guid, int, IEntitiesRepository, TReturn> LootTablePredicateMagicMethodHelper<BaseType, TargetType, TReturn>(MethodInfo method) where TargetType : BaseType
        {
            // Convert the slow MethodInfo into a fast, strongly typed, open delegate
            var func = (Func<TargetType, LootItemData, LootListRequest, Guid, int, IEntitiesRepository, TReturn>)Delegate.CreateDelegate
                (typeof(Func<TargetType, LootItemData, LootListRequest, Guid, int, IEntitiesRepository, TReturn>), method);

            // Now create a more weakly typed delegate which will call the strongly typed one
            Func<BaseType, LootItemData, LootListRequest, Guid, int, IEntitiesRepository, TReturn> ret = (BaseType pred, LootItemData lootItem, LootListRequest lootRequest, Guid lootedEntityId, int lootedEntityTypeId, IEntitiesRepository repo) => func((TargetType)pred, lootItem, lootRequest, lootedEntityId, lootedEntityTypeId, repo);
            return ret;
        }

        public static Func<BaseType, LootItemData, LootListRequest, Guid, int, IEntitiesRepository, TReturn> LootTablePredicateMagicMethod<BaseType, TReturn>(MethodInfo method)
        {
            // First fetch the generic form
            MethodInfo genericHelper = MethodBase.GetCurrentMethod().DeclaringType.GetMethod(nameof(LootTablePredicateMagicMethodHelper), BindingFlags.Static | BindingFlags.NonPublic);

            // Now supply the type arguments
            MethodInfo constructedHelper = genericHelper.MakeGenericMethod(typeof(BaseType), method.GetParameters()[0].ParameterType, typeof(TReturn));

            // Now call it. The null argument is because it’s a static method.
            object ret = constructedHelper.Invoke(null, new object[] { method });

            // Cast the result to the right kind of delegate and return it
            return (Func<BaseType, LootItemData, LootListRequest, Guid, int, IEntitiesRepository, TReturn>)ret;
        }

        #endregion //for_LootTable_predicates

        // --- for LootItemChanceWeightCalcerCalcer -----------------------------------------------------------------------
        #region for_LootItemChanceWeightCalcerCalcer

        private static Func<BaseType, LootItemData, LootListRequest, Guid, IEntitiesRepository, TReturn> LootItemChanceWeightCalcerMagicMethodHelper<BaseType, TargetType, TReturn>(MethodInfo method) where TargetType : BaseType
        {
            // Convert the slow MethodInfo into a fast, strongly typed, open delegate
            var func = (Func<TargetType, LootItemData, LootListRequest, Guid, IEntitiesRepository, TReturn>)Delegate.CreateDelegate
                (typeof(Func<TargetType, LootItemData, LootListRequest, Guid, IEntitiesRepository, TReturn>), method);

            // Now create a more weakly typed delegate which will call the strongly typed one
            Func<BaseType, LootItemData, LootListRequest, Guid, IEntitiesRepository, TReturn> ret = (BaseType table, LootItemData lootItem, LootListRequest lootRequest, Guid lootedEntityId, IEntitiesRepository repo) => func((TargetType)table, lootItem, lootRequest, lootedEntityId, repo);
            return ret;
        }

        public static Func<BaseType, LootItemData, LootListRequest, Guid, IEntitiesRepository, TReturn> LootItemChanceWeightCalcerMagicMethod<BaseType, TReturn>(MethodInfo method)
        {
            // First fetch the generic form
            MethodInfo genericHelper = MethodBase.GetCurrentMethod().DeclaringType.GetMethod(nameof(LootItemChanceWeightCalcerMagicMethodHelper),
                BindingFlags.Static | BindingFlags.NonPublic);

            // Now supply the type arguments
            MethodInfo constructedHelper = genericHelper.MakeGenericMethod(typeof(BaseType), method.GetParameters()[0].ParameterType, typeof(TReturn));

            // Now call it. The null argument is because it’s a static method.
            object ret = constructedHelper.Invoke(null, new object[] { method });

            // Cast the result to the right kind of delegate and return it
            return (Func<BaseType, LootItemData, LootListRequest, Guid, IEntitiesRepository, TReturn>)ret;
        }

        #endregion //for_LootItemChanceWeightCalcerCalcer

    // --- for_ClusterPredicateCalcer -----------------------------------------------------------------------
    #region for_ClusterPredicateCalcer

        private static Func<BaseType, int, Guid, IEntitiesRepository, TReturn> PredicateCalcerMagicMethodHelper<BaseType, TargetType, TReturn>(MethodInfo method) where TargetType : BaseType
        {
            // Convert the slow MethodInfo into a fast, strongly typed, open delegate
            var func = (Func<TargetType, int, Guid, IEntitiesRepository, TReturn>)Delegate.CreateDelegate
                (typeof(Func<TargetType, int, Guid, IEntitiesRepository, TReturn>), method);

            // Now create a more weakly typed delegate which will call the strongly typed one
            Func<BaseType, int, Guid, IEntitiesRepository, TReturn> ret = (BaseType pred, int entityTypeId, Guid entityId, IEntitiesRepository repo) => func((TargetType)pred, entityTypeId, entityId, repo);
            return ret;
        }

        public static Func<BaseType, int, Guid, IEntitiesRepository, TReturn> PredicateCalcerMagicMethod<BaseType, TReturn>(MethodInfo method)
        {
            // First fetch the generic form
            MethodInfo genericHelper = MethodBase.GetCurrentMethod().DeclaringType.GetMethod(nameof(PredicateCalcerMagicMethodHelper),
                BindingFlags.Static | BindingFlags.NonPublic);

            // Now supply the type arguments
            MethodInfo constructedHelper = genericHelper.MakeGenericMethod(typeof(BaseType), method.GetParameters()[0].ParameterType, typeof(TReturn));

            // Now call it. The null argument is because it’s a static method.
            object ret = constructedHelper.Invoke(null, new object[] { method });

            // Cast the result to the right kind of delegate and return it
            return (Func<BaseType, int, Guid, IEntitiesRepository, TReturn>)ret;
        }

    #endregion //for_ClusterPredicateCalcer

    }
}
