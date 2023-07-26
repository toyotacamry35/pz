using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LowScope.DuplicateMultiple
{
    public class LS_DuplicateMultiple : EditorWindow
    {
        [System.Serializable]
        public class Settings
        {
            public Vector3 translate;
            public Vector3 rotate;
            public Vector3 scale = Vector3.one;
            public int copies = 1;
            public string[] Space = new string[2] { "World", "Local" };
            public int selectedSpace;
            public string suffix;
        }

        private Settings settings = new Settings();
        private bool hasLoadedSettings;
        private static GameObject pivotObject;

        private GUIStyle labelSkin;

        [MenuItem("Edit/Duplicate Multiple/Window")]
        static void Init()
        {
            LS_DuplicateMultiple window = (LS_DuplicateMultiple)EditorWindow.GetWindow(typeof(LS_DuplicateMultiple), true, "Duplicate Multiple");
            window.minSize = new Vector2(340, 195);
            window.maxSize = new Vector2(340, 195);
            window.Show();
        }

        [MenuItem("Edit/Duplicate Multiple/Apply %#D")]
        static void Apply()
        {
            string saveData = EditorPrefs.GetString("LSDM_Translate");

            if (!string.IsNullOrEmpty(saveData))
            {
                ApplyDuplicateSpecial(JsonUtility.FromJson<Settings>(saveData));
            }
        }

        private void OnGUI()
        {
            if (!hasLoadedSettings)
            {
                string saveData = EditorPrefs.GetString("LSDM_Translate");

                if (!string.IsNullOrEmpty(saveData))
                {
                    settings = JsonUtility.FromJson<Settings>(saveData);
                }

                hasLoadedSettings = true;
            }

            if (labelSkin == null)
            {
                labelSkin = GUI.skin.label;
                labelSkin.alignment = TextAnchor.UpperRight;
            }

            EditorGUI.BeginChangeCheck();

            EditorGUI.LabelField(new Rect(15, 5, 115, 45), "Translate (Offset):", labelSkin);
            settings.translate = EditorGUI.Vector3Field(new Rect(135, 5, 200, 45), "", settings.translate);

            EditorGUI.LabelField(new Rect(15, 25, 115, 45), "Rotate (Offset):", labelSkin);
            settings.rotate = EditorGUI.Vector3Field(new Rect(135, 25, 200, 45), "", settings.rotate);

            EditorGUI.LabelField(new Rect(15, 45, 115, 45), "Scale (Offset):", labelSkin);
            settings.scale = EditorGUI.Vector3Field(new Rect(135, 45, 200, 45), "", settings.scale);

            EditorGUI.LabelField(new Rect(15, 65, 115, 45), "Duplicate Count:", labelSkin);
            settings.copies = EditorGUI.IntField(new Rect(135, 65, 50, 15), "", settings.copies);
            settings.copies = (int)GUI.HorizontalSlider(new Rect(195, 65, 140, 15), settings.copies, 1, 100);

            EditorGUI.LabelField(new Rect(15, 90, 115, 45), "Object space:", labelSkin);
            settings.selectedSpace = EditorGUI.Popup(new Rect(135, 90, 200, 15), settings.selectedSpace, new string[2] { "World", "Local" });

            EditorGUI.LabelField(new Rect(15, 110, 115, 45), "Add name suffix:", labelSkin);
            settings.suffix = EditorGUI.TextField(new Rect(135, 110, 200, 17.5f), settings.suffix);

            EditorGUI.LabelField(new Rect(15, 130, 115, 45), "Pivot Object:", labelSkin);
            pivotObject = (GameObject)EditorGUI.ObjectField(new Rect(135, 130, 200, 17.5f), pivotObject, typeof(GameObject), true);

            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetString("LSDM_Translate", JsonUtility.ToJson(settings));
            }

            if (GUI.Button(new Rect(5, 155, 162.5f, 25), "Apply (Ctrl+Shift+D)"))
            {
                ApplyDuplicateSpecial(settings);
            }

            if (GUI.Button(new Rect(172.5f, 155, 162.5f, 25), "Reset"))
            {
                settings = new Settings();
                pivotObject = null;
                EditorPrefs.SetString("LSDM_Translate", JsonUtility.ToJson(settings));
            }
        }

        static void ApplyDuplicateSpecial(Settings settings)
        {
            GameObject[] selectedObjects = Selection.gameObjects;

            if (selectedObjects.Length == 0)
                return;

            for (int i2 = 0; i2 < selectedObjects.Length; i2++)
            {
                for (int i = 0; i < settings.copies; i++)
                {
                    GameObject newObject = null;

                    if (selectedObjects[i2] == null)
                        return;

                    if (selectedObjects[i2].transform.parent != null)
                    {
                        newObject = GameObject.Instantiate(selectedObjects[i2], selectedObjects[i2].transform.parent) as GameObject;
                    }
                    else
                    {
                        newObject = GameObject.Instantiate(selectedObjects[i2]);
                    }

                    Undo.RegisterCreatedObjectUndo(newObject, "Duplicate Special Operation");

                    Vector3 newProperty;

                    if (settings.translate != Vector3.zero)
                    {
                        if (settings.selectedSpace == 0)
                        {
                            newProperty = newObject.transform.position;
                        }
                        else
                        {
                            newProperty = newObject.transform.localPosition;
                        }

                        newProperty += settings.translate * (i + 1);

                        if (settings.selectedSpace == 0)
                        {
                            newObject.transform.position = newProperty;
                        }
                        else
                        {
                            newObject.transform.localPosition = newProperty;
                        }
                    }

                    if (settings.rotate != Vector3.zero)
                    {
                        if (pivotObject != null)
                        {
                            Quaternion oldPivotRotation = pivotObject.transform.rotation;
                            Transform oldParent = newObject.transform.parent;
                            newObject.transform.SetParent(pivotObject.transform);
                            pivotObject.transform.Rotate(settings.rotate * (i + 1));
                            newObject.transform.SetParent(oldParent);
                            pivotObject.transform.rotation = oldPivotRotation;
                        }
                        else
                        {
                            newObject.gameObject.transform.Rotate(settings.rotate * (i + 1));
                        }
                    }

                    if (settings.scale != Vector3.zero)
                    {
                        if (pivotObject != null)
                        {
                            Vector3 oldScale = pivotObject.transform.localScale;
                            Transform oldParent = newObject.transform.parent;
                            newObject.transform.SetParent(pivotObject.transform);
                            pivotObject.transform.localScale = Vector3.one + ((settings.scale - Vector3.one)* (i + 1));
                            newObject.transform.SetParent(oldParent);
                            pivotObject.transform.localScale = oldScale;
                        }
                        else
                        {
                            newObject.gameObject.transform.localScale = Vector3.one + ((settings.scale - Vector3.one) * (i + 1));
                        }
                    }


                    newObject.name = selectedObjects[i2].name + "_" + settings.suffix + (i + 1).ToString();
                }
            }
        }
    }
}