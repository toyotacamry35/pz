using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Src.Cartographer.Editor
{
    class ClassificatorPredicateManager<GameObject>
    {
        List<ObjectsWithPredicateList<GameObject>> _allPredicateClasses;

        public ClassificatorPredicateManager()
        {
            //LoadFromJSON();
        }

        bool IgnorePropertiesPredicate(GameObject obj, ObjectsWithPredicateList<GameObject> objClass)
        {
            HashSet<string> disregardPropertiesHash = new HashSet<string>(objClass._ignoredProperties);

            var modifiedProperties = PrefabUtility.GetPropertyModifications(obj as UnityEngine.GameObject);
            bool add = false;
            if (modifiedProperties != null)
            {
                foreach (var prop in modifiedProperties)
                {
                    if (prop.target?.name.Contains("LOD") == true || prop.propertyPath.StartsWith("Children.Array"))
                        continue;
                    add = false;
                    foreach (var name in disregardPropertiesHash)
                    {
                        if (!disregardPropertiesHash.Contains(prop.propertyPath))
                        {
                            add = true;
                            break;
                        }
                    }
                    if (add == true)
                    {
                        break;
                    }
                }
            }
            return add;
        }

        private void LoadFromJSON()
        {
            throw new NotImplementedException();
        }

        public List<ObjectsWithPredicateList<GameObject>> DivideToModifiedAndNonmodified(List<GameObject> objects)
        {
            var predicateList = new ObjectsWithPredicateList<GameObject>();
            predicateList._className = "Modified";
            predicateList._ignoredProperties = new List<string>{ "m_LocalPosition.x", "m_LocalPosition.y", "m_LocalPosition.z",
                "m_LocalRotation.x", "m_LocalRotation.y", "m_LocalRotation.z", "m_LocalRotation.w",
                "m_LocalScale.x", "m_LocalScale.y", "m_LocalScale.z", "m_RootOrder", "m_LocalEulerAnglesHint.x", "m_LocalEulerAnglesHint.y", "m_LocalEulerAnglesHint.z",
                "m_Center.x", "m_Center.y", "m_Center.z", "m_Enabled", "m_Name", "m_Radius",
                "currentLodLevel", "relativeDistance", "absoluteDistance", "m_StaticEditorFlags" };
            ObjectsWithPredicateList<GameObject>.Match EPtest = IgnorePropertiesPredicate;
            predicateList.AddPredicate(EPtest);
            var modAndNotMod = new List<ObjectsWithPredicateList<GameObject>>();
            modAndNotMod.Add(predicateList);
            MatchAgainstAllConditions(objects, modAndNotMod);
            return modAndNotMod;
        }

        public List<ObjectsWithPredicateList<GameObject>> DoMatch(List<GameObject> objects)
        {
            return MatchAgainstAllConditions(objects, _allPredicateClasses);
        }

        public List<ObjectsWithPredicateList<GameObject>> MatchAgainstAllConditions(List<GameObject> objects, List<ObjectsWithPredicateList<GameObject>> predicateClassesList)
        {
            if (predicateClassesList == null)
            {
                throw new ArgumentNullException("predicateClassesList");
            }
            List<GameObject> nonmatched = new List<GameObject>();
            foreach (var obj in objects)
            {
                bool matchedAnyClass = false;
                foreach (var predicateClass in predicateClassesList)
                {
                    bool matchedCurrentClass = true;
                    foreach (var predicate in predicateClass.Predicates)
                    {
                        if (!predicate(obj, predicateClass))
                        {
                            matchedCurrentClass = false;
                            break;
                        }
                    }
                    if (matchedCurrentClass == true)
                    {
                        matchedAnyClass = true;
                        predicateClass.AddObject(obj);
                    }
                }
                if (matchedAnyClass == false)
                {
                    nonmatched.Add(obj);
                }
            }

            if (nonmatched.Count > 0)
            {
                var nonmatchedClass = new ObjectsWithPredicateList<GameObject>();
                nonmatchedClass.AddObjectRange(nonmatched);
                predicateClassesList.Add(nonmatchedClass);
            }

            return predicateClassesList;
        }
    }

    class ObjectsWithPredicateList<T>
    {
        public delegate bool Match(T obj, ObjectsWithPredicateList<T> objectWithPredicateList);
        public List<Match> Predicates { get; } = new List<Match>();
        public List<T> _objects = new List<T>();
        public List<string> _ignoredProperties = new List<string>();
        public string _className;

        internal void AddPredicate(Match toAddPredicate)
        {
            Predicates.Add(toAddPredicate);
        }

        internal void AddObject(T toAddObject)
        {
            _objects.Add(toAddObject);
        }

        internal void AddObjectRange(List<T> toAddObjects)
        {
            _objects.AddRange(toAddObjects);
        }

        internal List<PropertyModification> GetValuableModifiedPropertiesList(GameObject gameObject)
        {
            var modifiedProperties = PrefabUtility.GetPropertyModifications(gameObject);
            HashSet<string> disregardPropertiesHash = new HashSet<string>(_ignoredProperties);
            List<PropertyModification> result = new List<PropertyModification>();

            foreach (var prop in modifiedProperties)
            {
                if (prop.target?.name.Contains("LOD") == true || disregardPropertiesHash.Contains(prop.propertyPath) || prop.propertyPath.StartsWith("Children.Array"))
                    continue;
                else
                    result.Add(prop);
            }
            return result;
        }
    }

}
