using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Assets.Src.Cartographer.Editor.TreeDataModel
{
    internal class TreeViewElement<T> : TreeViewItem where T : TreeElement
    {
        public T data { get; set; }

        public TreeViewElement(int id, int depth, string displayName, T data) : base(id, depth, displayName)
        {
            this.data = data;
        }
    }

    public class TreeViewWithTreeModel<T> : TreeView where T : TreeElement
    {
        private TreeModel<T> m_TreeModel;
        readonly List<TreeViewItem> m_Rows = new List<TreeViewItem>(100);
        public event Action treeChanged;

        public TreeModel<T> treeModel { get { return m_TreeModel; } }
        public event Action<IList<TreeViewItem>> beforeDroppingDraggedItems;

        public TreeViewWithTreeModel(TreeViewState state, TreeModel<T> model) : base(state)
        {
            Init(model);
        }

        public TreeViewWithTreeModel(TreeViewState state, MultiColumnHeader multiColumnHeader, TreeModel<T> model)
            : base(state, multiColumnHeader)
        {
            Init(model);
        }

        private void Init(TreeModel<T> model)
        {
            m_TreeModel = model;
            m_TreeModel.modelChanged += ModelChanged;
        }

        private void ModelChanged()
        {
            if (treeChanged != null)
                treeChanged();

            Reload();
        }

        protected override TreeViewItem BuildRoot()
        {
            int depthForHiddenRoot = -1;
            return new TreeViewElement<T>(m_TreeModel.root.id, depthForHiddenRoot, m_TreeModel.root.name, m_TreeModel.root);
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            if (m_TreeModel.root == null)
            {
                Debug.LogError("tree model root is null. did you call SetData()?");
            }

            m_Rows.Clear();
            if (!string.IsNullOrEmpty(searchString))
            {
                Search(m_TreeModel.root, searchString, m_Rows);
            }
            else
            {
                if (m_TreeModel.root.hasChildren)
                    AddChildrenRecursive(m_TreeModel.root, 0, m_Rows);
            }

            // We still need to setup the child parent information for the rows since this 
            // information is used by the TreeView internal logic (navigation, dragging etc)
            SetupParentsAndChildrenFromDepths(root, m_Rows);

            return m_Rows;
        }

        private void AddChildrenRecursive(T parent, int depth, IList<TreeViewItem> newRows)
        {
            foreach (T child in parent.children)
            {
                var item = new TreeViewElement<T>(child.id, depth, child.name, child);
                newRows.Add(item);

                if (child.hasChildren)
                {
                    if (IsExpanded(child.id))
                    {
                        AddChildrenRecursive(child, depth + 1, newRows);
                    }
                    else
                    {
                        item.children = CreateChildListForCollapsedParent();
                    }
                }
            }
        }

        private void Search(T searchFromThis, string search, List<TreeViewItem> result)
        {
            if (string.IsNullOrEmpty(search))
                throw new ArgumentException("Invalid search: cannot be null or empty", "search");

            const int kItemDepth = 0; // tree is flattened when searching

            Stack<T> stack = new Stack<T>();
            foreach (var element in searchFromThis.children)
                stack.Push((T)element);
            while (stack.Count > 0)
            {
                T current = stack.Pop();
                // Matches search?
                if (current.name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    result.Add(new TreeViewElement<T>(current.id, kItemDepth, current.name, current));
                }

                if (current.children != null && current.children.Count > 0)
                {
                    foreach (var element in current.children)
                    {
                        stack.Push((T)element);
                    }
                }
            }
            SortSearchResult(result);
        }

        protected virtual void SortSearchResult(List<TreeViewItem> rows)
        {
            rows.Sort((x, y) => EditorUtility.NaturalCompare(x.displayName, y.displayName)); // sort by displayName by default, can be overriden for multicolumn solutions
        }

        protected override IList<int> GetAncestors(int id)
        {
            return m_TreeModel.GetAncestors(id);
        }

        protected override IList<int> GetDescendantsThatHaveChildren(int id)
        {
            return m_TreeModel.GetDescendantsThatHaveChildren(id);
        }

        public void SetSelectionByNames(string[] names)
        {
            var IDs = new List<int>();
            foreach (string name in names)
            {
                foreach (var item in m_Rows)
                {
                    if (String.CompareOrdinal(name, item.displayName) == 0)
                    {
                        IDs.Add(item.id);
                    }
                }
            }
            this.SetSelection(IDs, TreeViewSelectionOptions.FireSelectionChanged);
        }

        public string[] GetNamesByID(IList<int> IDs)
        {
            var Names = new List<string>();
            foreach (var id in IDs)
            {
                var selectedName = FindItem(id, rootItem).displayName;
                Names.Add(selectedName);
            }
            return Names.ToArray();
        }
    }

    static class ExtensionMethods
    {
        public static IOrderedEnumerable<T> Order<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector, bool ascending)
        {
            if (ascending)
            {
                return source.OrderBy(selector);
            }
            else
            {
                return source.OrderByDescending(selector);
            }
        }

        public static IOrderedEnumerable<T> ThenBy<T, TKey>(this IOrderedEnumerable<T> source, Func<T, TKey> selector, bool ascending)
        {
            if (ascending)
            {
                return source.ThenBy(selector);
            }
            else
            {
                return source.ThenByDescending(selector);
            }
        }
    }
}
