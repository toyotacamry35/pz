using UnityEditor;
using UnityEngine;

internal static class EditorGUIUtils
{
	public static void DrawError(string message)
	{
		GUI.color = Color.red;
		EditorGUILayout.HelpBox(message, MessageType.Error, true);
		GUI.color = Color.white;
	}

	public static void DrawWarning(string message)
	{
		GUI.color = Color.yellow;
		EditorGUILayout.HelpBox(message, MessageType.Warning, true);
		GUI.color = Color.white;
	}
}