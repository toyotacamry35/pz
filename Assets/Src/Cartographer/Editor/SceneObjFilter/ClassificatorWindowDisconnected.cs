﻿using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;
using Assets.Src.Cartographer.Editor.TreeDataModel;

namespace Assets.Src.Cartographer.Editor
{
    [System.Obsolete]
    class ClassificatorWindowDisconnected : EditorWindow
    {
        ClassificatorDataProcessor _dataModel;

        [SerializeField] TreeViewState _treeViewState = new TreeViewState();
        [SerializeField] MultiColumnHeaderState _multiColumnHeaderState;
        public ClassificatorBadTreeView _treeView { get; private set; }
        SearchField _searchField;

        [MenuItem("GameObject/Classificator/Disconnected prefabs")]
        public static ClassificatorWindowDisconnected GetWindow()
        {
            var window = GetWindow<ClassificatorWindowDisconnected>();
            window.titleContent = new GUIContent("Disconnected");
            window.Focus();
            window.Repaint();
            return window;
        }

        Rect MultiColumnTreeViewRect => new Rect(20, 30, position.width - 40, position.height - 30);

        Rect ToolbarRect => new Rect(20f, 10f, position.width - 105, 20f);

        Rect ButtonRect => new Rect(position.width - 80, 9f, 60, 20f);

        void DoBadTreeView(Rect rect)
        {
            _treeView.OnGUI(rect);
        }

        void DoSearchBar(Rect rect)
        {
            _treeView.searchString = _searchField.OnGUI(rect, _treeView.searchString);
        }

        void DoButton(Rect rect)
        {
            if (GUI.Button(rect, "Reload"))
            {
                _dataModel.Reload();
            }

        }

        public void OnEnable()
        {
            _dataModel = ClassificatorDataProcessor.Instance;
            _dataModel.OnDataChange += OnDataModelChange;

            var headerState = ClassificatorBadTreeView.CreateDefaultMultiColumnHeaderState(MultiColumnTreeViewRect.width);
            if (MultiColumnHeaderState.CanOverwriteSerializedFields(_multiColumnHeaderState, headerState))
                MultiColumnHeaderState.OverwriteSerializedFields(_multiColumnHeaderState, headerState);
            _multiColumnHeaderState = headerState;
            var multiColumnHeader = new MultiColumnHeader(headerState);
            multiColumnHeader.ResizeToFit();
            var treeModel = new TreeModel<BadMultiColumnTreeElement>(GetDisconnectedList());
            _treeView = new ClassificatorBadTreeView(_treeViewState, multiColumnHeader, treeModel);
            _treeView.InternalSelectionChanged += OnInternalSelectionChange;
            _searchField = new SearchField();
            _searchField.downOrUpArrowKeyPressed += _treeView.SetFocusAndEnsureSelectedItem;
        }

        private void OnDestroy()
        {
            _dataModel.OnDataChange -= OnDataModelChange;
        }

        private void OnDataModelChange()
        {
            var treeModel = new TreeModel<BadMultiColumnTreeElement>(GetDisconnectedList());
            var multiColumnHeader = new MultiColumnHeader(_multiColumnHeaderState);
            multiColumnHeader.ResizeToFit();
            _treeView = new ClassificatorBadTreeView(_treeViewState, multiColumnHeader, treeModel);
            _treeView.InternalSelectionChanged += OnInternalSelectionChange;
        }

        private void OnGUI()
        {
            DoBadTreeView(MultiColumnTreeViewRect);
            DoSearchBar(ToolbarRect);
            DoButton(ButtonRect);
        }

        private void OnInternalSelectionChange(IList<int> selectedIDs)
        {
            _dataModel.ClearCurrentSelection();
            _dataModel.SelectObjectsByID(selectedIDs);
        }

        IList<BadMultiColumnTreeElement> GetDisconnectedList()
        {
            var treeElements = new List<BadMultiColumnTreeElement>();
            var ins = new BadMultiColumnTreeElement("Root", -1, 0);
            treeElements.Add(ins);

            var disconnectedPrefabs = _dataModel.GetListOfDiconnectedPrefabs();
            foreach (var element in disconnectedPrefabs)
            {
                string displayedName = element.name;
                var id = element.GetInstanceID();
                ins = new BadMultiColumnTreeElement(displayedName, 0, id) { };
                treeElements.Add(ins);
            }

            return treeElements;
        }
    }
}
