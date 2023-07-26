using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GradientEditor : EditorWindow {

    GradientHDR gradient;
    const int borderSize = 10;
    const float keyWidth = 10;
    const float keyHeight = 20;

    Rect gradientPreviewRect;
    Rect dayPartsPreviewRect;
    Rect[] keyRects;
    bool mouseIsDownOverKey;
    int selectedKeyIndex;
    bool needsRepaint;

    private void OnGUI()
    {
        Draw();
        HandleInput();

        if (needsRepaint)
        {
            needsRepaint = false;
            Repaint();
        }
    }

    void Draw()
    {
        dayPartsPreviewRect = new Rect(borderSize, borderSize, position.width - borderSize * 2, 15);
        DrawDayParts(dayPartsPreviewRect);
        gradientPreviewRect = new Rect(borderSize, borderSize + 16, position.width - borderSize * 2, 25);
        GUI.DrawTexture(gradientPreviewRect, gradient.GetTexture((int)gradientPreviewRect.width));

        EditorGUI.DrawRect(new Rect(gradientPreviewRect.x + gradientPreviewRect.width * TOD.ASkyLighting._instance.timeline / 24f, dayPartsPreviewRect.y, 1, 41), Color.black);

        keyRects = new Rect[gradient.NumKeys];
        for (int i = 0; i < gradient.NumKeys; i++)
        {
            GradientHDR.ColourKey key = gradient.GetKey(i);
            Rect keyRect = new Rect(gradientPreviewRect.x + gradientPreviewRect.width * key.Time - keyWidth / 2f, gradientPreviewRect.yMax + borderSize, keyWidth, keyHeight);
            if (i == selectedKeyIndex)
            {
                EditorGUI.DrawRect(new Rect(keyRect.x - 2, keyRect.y - 2, keyRect.width + 4, keyRect.height + 4), Color.black);
            }
            EditorGUI.DrawRect(new Rect(keyRect.center.x - 1, keyRect.yMin - 10, 1, 10), Color.black);
            EditorGUI.DrawRect(keyRect, key.Colour);
            keyRects[i] = keyRect;
        }

        Rect settingsRect = new Rect(borderSize, keyRects[0].yMax + borderSize, position.width - borderSize * 2, position.height);

        
        GUILayout.BeginArea(settingsRect);
        GUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        Color newColour = EditorGUILayout.ColorField(new GUIContent("Color", "Color"), gradient.GetKey(selectedKeyIndex).Colour, true, false, true);
        if (EditorGUI.EndChangeCheck())
        {
            gradient.UpdateKeyColour(selectedKeyIndex, newColour);
        }

        //gradient.blendMode = (CustomGradient.BlendMode)EditorGUILayout.EnumPopup("Blend mode", gradient.blendMode);
        //gradient.randomizeColour = EditorGUILayout.Toggle("Randomize colour", gradient.randomizeColour);

        if (selectedKeyIndex > 0)
        {
            EditorGUI.BeginChangeCheck();

            float currentTime = gradient.GetKey(selectedKeyIndex).Time;
            EditorGUILayout.LabelField("Location", GUILayout.Width(100));
            currentTime = EditorGUILayout.FloatField("", currentTime, GUILayout.Width(100));
        


            if (EditorGUI.EndChangeCheck())
                gradient.UpdateKeyTime(selectedKeyIndex, currentTime);
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
        
    }

    void HandleInput()
    {
		Event guiEvent = Event.current;
		if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
		{
			for (int i = 0; i < keyRects.Length; i++)
			{
				if (keyRects[i].Contains(guiEvent.mousePosition))
				{
					mouseIsDownOverKey = true;
					selectedKeyIndex = i;
					needsRepaint = true;
					break;
				}
			}

			if (!mouseIsDownOverKey)
			{
				
				float keyTime = Mathf.InverseLerp(gradientPreviewRect.x, gradientPreviewRect.xMax, guiEvent.mousePosition.x);
                Color interpolatedColour = gradient.Evaluate(keyTime);
                Color randomColour = new Color(Random.value, Random.value, Random.value);

                selectedKeyIndex = gradient.AddKey(interpolatedColour, keyTime);
				mouseIsDownOverKey = true;
				needsRepaint = true;
			}
		}

		if (guiEvent.type == EventType.MouseUp && guiEvent.button == 0)
		{
			mouseIsDownOverKey = false;
		}

		if (mouseIsDownOverKey && guiEvent.type == EventType.MouseDrag && guiEvent.button == 0)
		{
            float keyTime = Mathf.InverseLerp(gradientPreviewRect.x, gradientPreviewRect.xMax, guiEvent.mousePosition.x);
			selectedKeyIndex = gradient.UpdateKeyTime(selectedKeyIndex, keyTime);
			needsRepaint = true;
		}

		if ((guiEvent.keyCode == KeyCode.Backspace || guiEvent.keyCode == KeyCode.Delete) && guiEvent.type == EventType.KeyDown)
		{
            if (selectedKeyIndex > 0 && selectedKeyIndex < gradient.NumKeys - 1)
            {
                gradient.RemoveKey(selectedKeyIndex);
                if (selectedKeyIndex >= gradient.NumKeys)
                {
                    selectedKeyIndex--;
                }
                needsRepaint = true;
            }
		}
    }

    void DrawDayParts(Rect rect)
    {
        if (TOD.ASkyLighting._instance == null)
            return;


        TOD.DayPart[] dayParts = TOD.ASkyLighting._instance.dayParts;
        for (int i = 0; i < dayParts.Length; i++)
        {

            if (dayParts[i].percent.x < dayParts[i].percent.y)
            {
                Rect current = new Rect(rect.x + rect.width * dayParts[i].percent.x, rect.y, (dayParts[i].percent.y - dayParts[i].percent.x) * rect.width, rect.height);
                EditorGUI.DrawRect(current, dayParts[i].color);

            }

            else
            {
                Rect current = new Rect(rect.x, rect.y, dayParts[i].percent.y * rect.width, rect.height);
                EditorGUI.DrawRect(current, dayParts[i].color);

                current = new Rect(rect.x + dayParts[i].percent.x * rect.width, rect.y, rect.width * (1 - dayParts[i].percent.x), rect.height);
                EditorGUI.DrawRect(current, dayParts[i].color);
 
            }

        }
    }

    public void SetGradient(GradientHDR gradient)
    {
        this.gradient = gradient;
    }

    private void OnEnable()
    {
        titleContent.text = "Gradient Editor";
        position.Set(position.x, position.y, 400, 150);
        minSize = new Vector2(200, 150);
        maxSize = new Vector2(1920, 150);
        selectedKeyIndex = 0;
    }

    private void OnDisable()
    {
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }
}