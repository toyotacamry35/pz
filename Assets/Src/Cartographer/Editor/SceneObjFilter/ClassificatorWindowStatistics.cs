using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;
using Assets.Src.Cartographer.Editor.TreeDataModel;

namespace Assets.Src.Cartographer.Editor
{
    [System.Obsolete]
    class ClassificatorWindowStatistics : EditorWindow
    {
        ClassificatorDataProcessor _dataModel;

        [SerializeField] TreeViewState _freqTreeViewState = new TreeViewState(); // unity class for storing tree state
        [SerializeField] MultiColumnHeaderState _freqMultiColumnHeaderState; //unity class for storing tree header state
        public ClassificatorFreqTreeView _freqTreeView { get; private set; }
        SearchField _freqSearchField; // unity class, работает из коробки вместе в tree view

        [MenuItem("GameObject/Classificator/Statistics")]
        public static ClassificatorWindowStatistics GetWindow()
        {
            var window = GetWindow<ClassificatorWindowStatistics>();
            window.titleContent = new GUIContent("Statistics");
            window.Focus();
            window.Repaint();
            return window;
        }

        Rect MultiColumnTreeViewRect => new Rect(20, 30, position.width - 40, position.height - 30);

        Rect ToolbarRect => new Rect(20f, 10f, position.width - 185, 20f);

        Rect ButtonRect => new Rect(position.width - 160, 9f, 140, 20f);

        void DoFreqTreeView(Rect rect)
        {
            _freqTreeView.OnGUI(rect);
        }
        void DoSearchBar(Rect rect)
        {
            _freqTreeView.searchString = _freqSearchField.OnGUI(ToolbarRect, _freqTreeView.searchString); // связь с деревом
        }

        void DoButton(Rect rect)
        {
            var width = rect.width / 2 - 1;
            var buttonRect0 = new Rect(rect.x, rect.y, width - 1, rect.height);
            var buttonRect1 = new Rect(rect.x + width + 2, rect.y, width, rect.height);

            if (GUI.Button(buttonRect0, "Reload"))
            {
                _dataModel.Reload();
            }
            if (GUI.Button(buttonRect1, "Delete"))
            {
                _dataModel.DeleteGameObjectsInCurrentSelection();
                _dataModel.Reload();
            }
        }

        private void OnEnable()
        {
            _dataModel = ClassificatorDataProcessor.Instance;
            _dataModel.OnDataChange += OnDataModelChange;

            // ???
            {
                var headerState = ClassificatorFreqTreeView.CreateDefaultMultiColumnHeaderState(MultiColumnTreeViewRect.width);
                if (MultiColumnHeaderState.CanOverwriteSerializedFields(_freqMultiColumnHeaderState, headerState))
                    MultiColumnHeaderState.OverwriteSerializedFields(_freqMultiColumnHeaderState, headerState);
                _freqMultiColumnHeaderState = headerState;
            }
            // ???

            var multiColumnHeader = new MultiColumnHeader(_freqMultiColumnHeaderState);
            multiColumnHeader.ResizeToFit();
            var treeModel = new TreeModel<FreqMultiColumnTreeElement>(GetFreqData());
            _freqTreeView = new ClassificatorFreqTreeView(_freqTreeViewState, multiColumnHeader, treeModel);
            _freqTreeView.InternalSelectionChanged += OnInternalSelectionChange;
            _freqSearchField = new SearchField();
            _freqSearchField.downOrUpArrowKeyPressed += _freqTreeView.SetFocusAndEnsureSelectedItem; // связь с деревом
        }

        private void OnDestroy()
        {
            _dataModel.OnDataChange -= OnDataModelChange;
        }

        private void OnDataModelChange()
        {
            var treeModel = new TreeModel<FreqMultiColumnTreeElement>(GetFreqData());
            var multiColumnHeader = new MultiColumnHeader(_freqMultiColumnHeaderState);
            multiColumnHeader.ResizeToFit();
            if (_freqTreeView != null)
            {
                _freqTreeView.InternalSelectionChanged -= OnInternalSelectionChange;
            }
            _freqTreeView = new ClassificatorFreqTreeView(_freqTreeViewState, multiColumnHeader, treeModel);
            _freqTreeView.InternalSelectionChanged += OnInternalSelectionChange;
        }

        IList<FreqMultiColumnTreeElement> GetFreqData()
        {
            var treeElements = new List<FreqMultiColumnTreeElement>();
            var root = new FreqMultiColumnTreeElement("Root", -1, 0);
            treeElements.Add(root);
            FreqMultiColumnTreeElement el;

            var prefabUseCounters = _dataModel.GetFreqTable();
            foreach (var prefUseCounter in prefabUseCounters)
            {
                string displayedName = prefUseCounter.Key.name;
                int displayedValue = prefUseCounter.Value.Count;
                var id = prefUseCounter.Key.GetInstanceID();
                el = new FreqMultiColumnTreeElement(displayedName, 0, id) { Count = displayedValue };
                treeElements.Add(el);
                for (int k = 0; k < displayedValue; k++)
                {
                    var insertedSubElement = prefUseCounter.Value[k];
                    id = insertedSubElement.GetInstanceID();
                    el = new FreqMultiColumnTreeElement(insertedSubElement.name, 1, id) { Count = 1 };
                    treeElements.Add(el);
                }
            }
            return treeElements;
        }

        private void OnGUI()
        {
            try
            {
                DoFreqTreeView(MultiColumnTreeViewRect);
                DoSearchBar(ToolbarRect);
                DoButton(ButtonRect);
            }
            catch
            {
                if (_freqTreeViewState == null)
                    Debug.Log("_freqTreeViewState was not initialized before OnGUI call. Try to restart Unity or \"Statistics\" window.");
                else
                    Debug.Log("Something went wrong in OnGUI procedure of Statictics window. Please report this error.");
            }
        }

        // ### UPDATE TABLE ON EXTERNAL SELECTION ###
        //private void OnSelectionChange()
        //{
        //    var selectedObjectNames = data.GetNamesOfExternallySelected();
        //    _treeView.SetSelectionByNames(selectedObjectNames);
        //}

        private void OnInternalSelectionChange(IList<int> selectedIDs)
        {
            _dataModel.ClearCurrentSelection();
            _dataModel.SelectObjectsByID(selectedIDs);
        }
    }
}
