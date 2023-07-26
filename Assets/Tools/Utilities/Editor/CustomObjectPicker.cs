using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace Utilities
{

    public delegate void ObjectSelectionHandler(UnityEngine.Object obj);
    public class ObjectPickerWindow : EditorWindow
    {
        public List<UnityEngine.Object> objs = new List<UnityEngine.Object>();

        /// <summary>
        /// A flag to indicate if the editor window has been setup or not.
        /// </summary>
        private bool isSetup = false;

        private ObjectSelectionHandler handler;
        private Action onExitHandler;
        #region Setup

        /// <summary>
        /// Attempts to setup the editor by reading in textures from specified path.
        /// </summary>
        /// <param name='path'>
        /// Path to load images from.
        /// </param>
        public void Setup(string path, ObjectSelectionHandler functionHandler, Action onExitAction)
        {
            string[] paths = new string[] { path };
            Setup(paths, functionHandler, onExitAction);
        } // eo Setup

        /// <summary>
        /// Attempts to setup the editor by reading in all textures specified
        /// by the various paths. Supports multiple paths of textures.
        /// </summary>
        /// <param name='paths'>
        /// Paths of textures to read in.
        /// </param>
        public void Setup(string[] paths, ObjectSelectionHandler functionHandler, Action onExitAction)
        {
            objs.Clear();
            isSetup = true;
            ReadInAllTextures(paths);
            handler = functionHandler;
            onExitHandler = onExitAction;
        } // eo Setup

        #endregion Setup

        void OnDisable()
        {
            if(onExitHandler != null)
            onExitHandler();
        }
        #region GUI

        Vector2 _scrollPosition = Vector2.zero;
        void OnGUI()
        {
            if (!isSetup)
                return;

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            // create a button for each image loaded in, 4 buttons in width
            // calls the handler when a new image is selected.
            int counter = 0;
            int widthSize = 6;
            foreach (UnityEngine.Object obj in objs)
            {
                if (counter % widthSize == 0 || counter == 0)
                    EditorGUILayout.BeginHorizontal();
                ++counter;

                if (GUILayout.Button(obj.name))
                {
                    // tell handler about new image, close selection window
                    handler(obj);
                    EditorWindow.focusedWindow.Close();
                }

                if (counter % widthSize == 0)
                    EditorGUILayout.EndHorizontal();
            }


            EditorGUILayout.EndScrollView();
        } // eo OnGUI

        #endregion GUI

        #region Utility

        /// <summary>
        /// Reads the in all textures from the paths.
        /// </summary>
        /// <param name='paths'>
        /// The paths to read images from.
        /// </param>
        void ReadInAllTextures(string[] paths)
        {
            foreach (string path in paths)
            {
                string[] allFilesInPath = Directory.GetFiles(path, "*",SearchOption.AllDirectories);
                foreach (string filePath in allFilesInPath)
                {
                    UnityEngine.Object obj = (UnityEngine.Object)AssetDatabase.LoadAssetAtPath(filePath, typeof(UnityEngine.Object));
                    if (obj is UnityEngine.Object)
                    {
                        objs.Add(obj as UnityEngine.Object);
                    }
                }
            }
        } // eo ReadInAllTextures

        #endregion Utility

    } // End TexturePickerEditor

    public static class EditorUtils
    {
        /// <summary>
        /// Shows a texture with a label and a button to select a new image
        /// from a list of images loaded from the path specified. This allows
        /// a selection of an image from a subset of images, unlike the UnityEditor.ObjectField
        /// that pulls all images from /Assets/
        /// </summary>
        /// <param name='label'>
        /// Label to display.
        /// </param>
        /// <param name='selectedImage'>
        /// Selected image that shows in the interface.
        /// </param>
        /// <param name='yPosition'>
        /// How far down in the interface to show this tool.
        /// </param>
        /// <param name='textureFilePath'>
        /// Texture file path containing the images to load.
        /// </param>
        /// <param name='functionHandler'>
        /// The function to handle the selection of a new texture.
        /// </param>
        public static void TexturePreviewWithSelection(string label, Texture selectedImage, float yPosition,
            string textureFilePaths, ObjectSelectionHandler functionHandler, Action onExitHandler)
        {
            TexturePreviewWithSelection(label, selectedImage, yPosition, new string[] { textureFilePaths }, functionHandler, onExitHandler);
        } // eo TexturePreviewWithSelection

        /// <summary>
        /// Shows a texture with a label and a button to select a new image
        /// from a list of images loaded from the paths specified. This allows
        /// a selection of an image from a subset of images, unlike the UnityEditor.ObjectField
        /// that pulls all images from /Assets/
        /// </summary>
        /// <param name='label'>
        /// Label to display.
        /// </param>
        /// <param name='selectedImage'>
        /// Selected image that shows in the interface.
        /// </param>
        /// <param name='yPosition'>
        /// How far down in the interface to show this tool.
        /// </param>
        /// <param name='textureFilePaths'>
        /// Texture file paths containing the images to load.
        /// </param>
        /// <param name='functionHandler'>
        /// The function to handle the selection of a new texture.
        /// </param>
        public static ObjectPickerWindow TexturePreviewWithSelection(string label, Texture selectedImage, float yPosition,
            string[] textureFilePaths, ObjectSelectionHandler functionHandler, Action onExitHandler)
        {
            ObjectPickerWindow window = null;

            EditorGUILayout.BeginVertical(GUILayout.Height(125));
            {
                EditorGUILayout.LabelField(label);
                EditorGUI.DrawPreviewTexture(new Rect(50, yPosition, 100, 100), selectedImage);

                // used to center the select texture button
                EditorGUILayout.BeginVertical();
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button("Select Texture", GUILayout.MaxWidth(100)))
                {
                    window = ObjectPicker(textureFilePaths, functionHandler, onExitHandler);
                }
                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
            return window;
        } // eo TexturePreviewWithSelection

        public static ObjectPickerWindow ObjectPicker(string path, ObjectSelectionHandler functionHandler, Action onExitHandler)
        {
            return ObjectPicker(new string[] { path }, functionHandler, onExitHandler);
        } // eo TexturePicker

        /// <summary>
        /// Creates a window with buttons to select a new image. 
        /// </summary>
        /// <param name='paths'>
        /// Paths to load images from.
        /// </param>
        /// <param name='functionHandler'>
        /// How to handle the new image selection.
        /// </param>
        public static ObjectPickerWindow ObjectPicker(string[] paths, ObjectSelectionHandler functionHandler, Action onExitHandler)
        {
            ObjectPickerWindow picker = (ObjectPickerWindow)EditorWindow.GetWindow(typeof(ObjectPickerWindow), true, "Object Picker");
            picker.Setup(paths, functionHandler, onExitHandler);
            return picker;
        } // eo TexturePicker
    }


}
