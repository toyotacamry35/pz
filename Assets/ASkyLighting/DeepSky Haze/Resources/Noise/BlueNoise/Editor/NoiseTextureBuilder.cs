using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System.IO;

public class NoiseTextureBuilder : EditorWindow {

    [MenuItem("GameObject/DeepSky Haze/Build Noise Textures", false, 14)]
    public static void ShowWindow()
    {
        NoiseTextureBuilder window = EditorWindow.GetWindow<NoiseTextureBuilder>("DeepSky Haze Noise Texture Builder") as NoiseTextureBuilder;
        window.Show();
    }

    private Texture2D _firstTexture = null;
    private int _range = 16;

    void OnGUI()
    {
        _firstTexture = (Texture2D)EditorGUILayout.ObjectField("First texture:", _firstTexture, typeof(Texture2D), false);
        _range = EditorGUILayout.IntField("Range:", _range);

        if (GUILayout.Button("Do Stuff!"))
        {
            MakeNoiseTexture();
        }
    }

    void MakeNoiseTexture()
    {
        string asset = AssetDatabase.GetAssetPath(_firstTexture);
        string dirPath = Path.GetDirectoryName(asset);

        string[] assets = Directory.GetFiles(dirPath, "*.png");

        List<Color> output = new List<Color>();

        Texture2D tmp;
        int width = -1;
        int height = -1;
        for (int i = 0; i < assets.Length; i++)
        {
            tmp = AssetDatabase.LoadAssetAtPath<Texture2D>(assets[i]);
            output.AddRange(tmp.GetPixels());

            if (width == -1)
            {
                width = tmp.width;
                height = tmp.height;
            }
        }

        Texture3D noise = new Texture3D(width, height, assets.Length, TextureFormat.RGB24, false);
        noise.SetPixels(output.ToArray());

        AssetDatabase.CreateAsset(noise, Path.ChangeExtension(asset, "asset"));
    }

}
