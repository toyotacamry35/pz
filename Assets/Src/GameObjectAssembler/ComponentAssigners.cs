using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.ResourcesSystem.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Environment.Logging.Extension;
using NLog;
using UnityEngine;

namespace Assets.Src.GameObjectAssembler
{
    internal static class ComponentAssigners
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly IDictionary<(Type Src, Type Dst), Action<IComponentDef, Component, IReadOnlyDictionary<IComponentDef, Component>>> ComponentFillDelegates = new Dictionary<(Type,Type), Action<IComponentDef, Component, IReadOnlyDictionary<IComponentDef, Component>>>();

        private static PropertyInfo[] GetAllPropertiesUsedInAssemblage(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        }

        public static void CreateAssignComponentDelegate(Type src, Type dst)
        {
            var existing = CreateAssignComponentDelegateInternal(src, dst);
            if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Add delegate for key: ({src}, {dst})").Write();
            ComponentFillDelegates.Add((src, dst), existing);
        }

        public static Action<IComponentDef, Component, IReadOnlyDictionary<IComponentDef, Component>> GetAssignComponentDelegate(Type src, Type dst)
        {
            try
            {
                return ComponentFillDelegates[(src,dst)];
            }
            catch (Exception e)
            {
                throw new Exception(e.Message + $" Key: ({src}, {dst})");
            }
        }


        private static Action<IComponentDef, Component, IReadOnlyDictionary<IComponentDef, Component>> CreateAssignComponentDelegateInternal(Type src, Type dst)
        {
            List<Action<IComponentDef, Component, IReadOnlyDictionary<IComponentDef, Component>>> setActions = new List<Action<IComponentDef, Component, IReadOnlyDictionary<IComponentDef, Component>>>();

            var defReqType = dst.GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IFromDef<>));

            if (defReqType != null)
            {
                try
                {
                    var prop = dst.GetProperty(nameof(IFromDef<IComponentDef>.Def));
                    var action = DelegateCreator.CreateSetPropertyDelegateInexact<Component, IComponentDef>(prop);
                    if (action != null)
                    {
                        Action<IComponentDef, Component, IReadOnlyDictionary<IComponentDef, Component>> actionRes = (IComponentDef def, Component comp, IReadOnlyDictionary<IComponentDef, Component> index) => action(comp, def);
                        setActions.Add(actionRes);
                    }
                }
                catch (Exception e)
                {
                    //Logger.IfError()?.Message($"{dst.FullName} {e.ToString()}").Write(); // похоже к этому моменту NLog ещё не прочитал свой конфиг и все уровни лога выключены
                    throw new Exception($"Exception in ComponentAssigners for {dst.FullName}: ", e);
                }
            }

            var srcType = src;
            var srcFields = GetAllPropertiesUsedInAssemblage(srcType);
            foreach (var srcField in srcFields)
            {
                if (srcField.Name == nameof(IResource.Address))
                    continue;
                if (srcField.Name == nameof(IComponentDef.ReuseExisting))
                    continue;
                var lambda = FieldAssigners.CreateAssignFieldOrProperty(srcField, dst);
                setActions.Add(lambda);
            }

            return (IComponentDef def, Component comp, IReadOnlyDictionary<IComponentDef, Component> index) => setActions.ForEach(v => v(def, comp, index));
        }

    }
}
