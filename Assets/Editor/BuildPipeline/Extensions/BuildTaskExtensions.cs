using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build.Pipeline.Interfaces;

namespace Assets.Test.Src.Editor
{
    public static class BuildTaskExtensions
    {
        public static IList<T> InsertBefore<T>(this IList<T> tasks, Type type, T task) where T : IBuildTask
        {
            var nextDependency = tasks.Single(t => type.IsInstanceOfType(t));
            var nextDependencyIndex = tasks.IndexOf(nextDependency);
            tasks.Insert(nextDependencyIndex, task);
            return tasks;
        }

        public static IList<T> InsertAfter<T>(this IList<T> tasks, Type type, T task) where T : IBuildTask
        {
            var nextDependency = tasks.Single(t => type.IsInstanceOfType(t));
            var nextDependencyIndex = tasks.IndexOf(nextDependency);
            tasks.Insert(nextDependencyIndex + 1, task);
            return tasks;
        }
    }
}