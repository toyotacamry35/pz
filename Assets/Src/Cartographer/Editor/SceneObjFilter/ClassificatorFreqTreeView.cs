using UnityEngine;
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.Assertions;
using Assets.Src.Cartographer.Editor.TreeDataModel;

namespace Assets.Src.Cartographer.Editor
{
    public class ClassificatorFreqTreeView : TreeViewWithTreeModel<FreqMultiColumnTreeElement>
    {
        const float DefaultRowHeight = 20.0f;

        enum Columns
        {
            Name,
            Count,
        }

        public enum SortOption
        {
            Name,
            Count,
        }

        SortOption[] ColumnSortOptions =
        {
            SortOption.Name,
            SortOption.Count
        };

        public event Action<IList<int>> InternalSelectionChanged;

        public static void TreeToList(TreeViewItem root, IList<TreeViewItem> result)
        {
            if (root == null)
            {
                throw new NullReferenceException("The input 'T root' is null");
            }
            if (result == null)
            {
                throw new NullReferenceException("The input 'IList<T> result' list is null"); ;
            }

            result.Clear();

            if (root.children == null)
            {
                return;
            }

            Stack<TreeViewItem> stack = new Stack<TreeViewItem>();
            for (var i = root.children.Count - 1; i >= 0; i--)
            {
                stack.Push(root.children[i]);
            }

            while (stack.Count > 0)
            {
                TreeViewItem current = stack.Pop();
                result.Add(current);

                if (current.hasChildren && current.children[0] != null)
                {
                    for (int i = current.children.Count - 1; i >= 0; i--)
                    {
                        stack.Push(current.children[i]);
                    }
                }
            }
        }

        public ClassificatorFreqTreeView(TreeViewState state, MultiColumnHeader multicolumnHeader, TreeModel<FreqMultiColumnTreeElement> model) : base(state, multicolumnHeader, model)
        {
            Assert.AreEqual(ColumnSortOptions.Length, Enum.GetValues(typeof(Columns)).Length, "Ensure number of sort options are in sync with number of MyColumns enum values");

            rowHeight = DefaultRowHeight;
            columnIndexForTreeFoldouts = 0;
            multicolumnHeader.sortingChanged += OnSortingChanged;

            Reload();
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            var rows = base.BuildRows(root);
            SortIfNeeded(root, rows);
            return rows;
        }

        void OnSortingChanged(MultiColumnHeader multiColumnHeader)
        {
            SortIfNeeded(rootItem, GetRows());
        }

        void SortIfNeeded(TreeViewItem root, IList<TreeViewItem> rows)
        {
            if (rows.Count <= 1)
                return;

            if (multiColumnHeader.sortedColumnIndex == -1)
            {
                return;
            }
            SortByMultipleColumns();
            TreeToList(root, rows);
            Repaint();
        }

        void SortByMultipleColumns()
        {
            var sortedColumns = multiColumnHeader.state.sortedColumns;

            if (sortedColumns.Length == 0)
                return;

            var myTypes = rootItem.children.Cast<TreeViewElement<FreqMultiColumnTreeElement>>();
            var orderedQuery = InitialOrder(myTypes, sortedColumns);
            for (int i = 1; i < sortedColumns.Length; i++)
            {
                SortOption sortOption = ColumnSortOptions[sortedColumns[i]];
                bool ascending = multiColumnHeader.IsSortedAscending(sortedColumns[i]);

                switch (sortOption)
                {
                    case SortOption.Name:
                        orderedQuery = orderedQuery.ThenBy(l => l.data.name, ascending);
                        break;
                    case SortOption.Count:
                        orderedQuery = orderedQuery.ThenBy(l => l.data.Count, ascending);
                        break;
                }
            }
            rootItem.children = orderedQuery.Cast<TreeViewItem>().ToList();
        }

        IOrderedEnumerable<TreeViewElement<FreqMultiColumnTreeElement>> InitialOrder(IEnumerable<TreeViewElement<FreqMultiColumnTreeElement>> myTypes, int[] history)
        {
            SortOption sortOption = ColumnSortOptions[history[0]];
            bool ascending = multiColumnHeader.IsSortedAscending(history[0]);
            switch (sortOption)
            {
                case SortOption.Name:
                    return myTypes.Order(l => l.data.name, ascending);
                case SortOption.Count:
                    return myTypes.Order(l => l.data.Count, ascending);
                default:
                    Assert.IsTrue(false, "Unhandled enum");
                    break;
            }

            // default
            return myTypes.Order(l => l.data.name, ascending);
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var item = (TreeViewElement<FreqMultiColumnTreeElement>)args.item;

            for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
            {
                CellGUI(args.GetCellRect(i), item, (Columns)args.GetColumn(i), ref args);
            }
        }

        void CellGUI(Rect cellRect, TreeViewElement<FreqMultiColumnTreeElement> item, Columns column, ref RowGUIArgs args)
        {
            // Center cell rect vertically (makes it easier to place controls, icons etc in the cells)
            CenterRectUsingSingleLineHeight(ref cellRect);

            switch (column)
            {
                case Columns.Name:
                {
                    args.rowRect = cellRect;
                    base.RowGUI(args);
                }
                break;

                case Columns.Count:
                    string value = item.data.Count.ToString();
                    DefaultGUI.LabelRightAligned(cellRect, value, args.selected, args.focused);
                    break;
            }
        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return true;
        }

        public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState(float treeViewWidth)
        {
            var columns = new[]
            {
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Name"),
                    headerTextAlignment = TextAlignment.Left,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Center,
                    width = 150,
                    minWidth = 60,
                    autoResize = true,
                    allowToggleVisibility = false
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Count", "How much times each prefab is used"),
                    headerTextAlignment = TextAlignment.Right,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Center,
                    width = 110,
                    minWidth = 60,
                    autoResize = true
                }
            };

            Assert.AreEqual(columns.Length, Enum.GetValues(typeof(Columns)).Length, "Number of columns should match number of enum values: You probably forgot to update one of them.");

            var state = new MultiColumnHeaderState(columns);
            return state;
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            InternalSelectionChanged?.Invoke(selectedIds);
        }
    }

    [Serializable]
    public class FreqMultiColumnTreeElement : TreeElement
    {
        public int Count;

        public FreqMultiColumnTreeElement(string name, int depth, int id) : base(name, depth, id) { }
    }
}
