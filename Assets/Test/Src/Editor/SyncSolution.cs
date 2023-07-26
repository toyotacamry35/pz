using UnityEngine;
using System;
using System.Reflection;
using UnityEditor;

namespace Assets.Test.Src.Editor
{
    public static class SyncSolution
    {
        [MenuItem("Assets/Sync C# Project")]
        public static void Sync()
        {
            var editor = Type.GetType("UnityEditor.SyncVS, UnityEditor");
            var SyncSolution = editor.GetMethod("SyncSolution", BindingFlags.Public | BindingFlags.Static);
            SyncSolution.Invoke(null, null);
            Debug.Log("Solution synced!");
        }
    }
}