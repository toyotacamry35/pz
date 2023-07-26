using NLog;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ColonyDI
{
    public static class DependencyInjection
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger("DependencySystem");

        public static void InjectRoot(IDependencyNode root)
        {
            Inject(root, null);
            CallAfterDepsInjected(root);
        }

        public static void LinkExtraChild(IDependencyNode node, IDependencyNode parent)
        {
            Inject(node, parent);
            CallAfterDepsInjected(node);
        }

        public static void LinkExtraChildren(IEnumerable<IDependencyNode> nodes, IDependencyNode parent)
        {
            foreach (var node in nodes)
            {
                Inject(node, parent);
            }

            foreach (var node in nodes)
            {
                CallAfterDepsInjected(node);
            }
        }

        public static T GetObject<T>(IDependencyNode node) where T : IDependencyNode
        {
            return (T) GetObject(node, typeof(T));
        }

        private static object GetObject(IDependencyNode node, Type typeOfObject)
        {
            return GetObjects(node, typeOfObject).SingleOrDefault();
        }

        private static IEnumerable<object> GetObjects(IDependencyNode node, Type typeOfObject)
        {
            while (node.Parent != null)
                node = node.Parent;

            foreach (var obj in GetObjectsFromNode(node, typeOfObject)) 
                yield return obj;
        }

        private static IEnumerable<IDependencyNode> GetObjectsFromNode(IDependencyNode node, Type typeOfObject)
        {
            if (typeOfObject.IsInstanceOfType(node))
                yield return node;

            if (node.Children != null)
                foreach (var child in node.Children)
                foreach (var obj in GetObjectsFromNode(child, typeOfObject))
                    yield return obj;
        }
        
        private static void Inject(IDependencyNode node, IDependencyNode parent)
        {
            node.Parent = parent;
            Inject(node);
            InjectChildren(node);


            node.AfterDependenciesInjected();
        }

        private static void InjectChildren(IDependencyNode node)
        {
            foreach (var child in node.Children)
                Inject(child, node);
        }

        private static void CallAfterDepsInjected(IDependencyNode node)
        {
            try
            {
                node.AfterDependenciesInjectedOnAllProviders();
            }
            catch (Exception e)
            {
                Logger.Error($"{nameof(IDependencyNode.AfterDependenciesInjectedOnAllProviders)}(node={node}) " +
                             $"Exception: {e.Message}\n{e.StackTrace}");
            }

            foreach (var child in node.Children)
                CallAfterDepsInjected(child);
        }

        private static void Inject(IDependencyNode node)
        {
            Type inspectedType = node.GetType();

            var propertyInfos =
                inspectedType.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic |
                                            BindingFlags.Instance);

            foreach (var propertyInfo in propertyInfos)
            {
                if (!Attribute.IsDefined(propertyInfo, typeof(DependencyAttribute)))
                    continue;

                if (!propertyInfo.CanRead || !propertyInfo.CanWrite)
                {
                    Logger.Error(
                        $"{nameof(Inject)}() Unable to resolve dependency for " +
                        $"'{inspectedType.NiceName()}.{propertyInfo.Name}': must have get-set");
                    continue;
                }

                try
                {
                    if (propertyInfo.PropertyType.IsArray)
                    {
                        var elemType = propertyInfo.PropertyType.GetElementType();
                        var elements = GetObjects(node, elemType).ToArray();

                        propertyInfo.SetValue(node, elements, null);
                    }
                    else
                    {
                        var valueObj = GetObject(node, propertyInfo.PropertyType);
                        if (valueObj == null)
                            Logger.Error("Cant find dependecy object of type {0} for property {1}.{2}",
                                propertyInfo.PropertyType.NiceName(), inspectedType.NiceName(), propertyInfo.Name);
                        else
                            propertyInfo.SetValue(node, valueObj, null);
                    }
                }
                catch (Exception exception)
                {
                    Logger.Error().Exception(exception).Message($"{nameof(Inject)}() '{inspectedType.NiceName()}.{propertyInfo.Name}'")
                        .Write();
                }
            }
        }
    }
}