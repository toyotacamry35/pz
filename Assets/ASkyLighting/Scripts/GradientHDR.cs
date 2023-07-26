using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GradientHDR {

    public enum BlendMode { Linear, Discrete };

    [SerializeField]
    List<ColourKey> keys = new List<ColourKey>();

    public GradientHDR()
    {
        AddKey(Color.white, 0);
        AddKey(Color.black, 1);
    }

    public Color Evaluate(float time)
    {
        var count = keys.Count;
        var indexLeft = 0;
        var indexRight = count - 1;

        for (int i = 0; i < count; ++i)
        {
            var keyTime = keys[i].Time;
            if (keyTime < time)
            {
                indexLeft = i;
            }
            else if (keyTime > time)
            {
                indexRight = i;
                break;
            }
        }

        var keyLeft = keys[indexLeft];
        var keyRight = keys[indexRight];

        var blendTime = Mathf.InverseLerp(keyLeft.Time, keyRight.Time, time);
        return Color.Lerp(keyLeft.Colour, keyRight.Colour, blendTime);
    }

    public int AddKey(Color colour, float time)
    {
        ColourKey newKey = new ColourKey(colour, time);
        for (int i = 0; i < keys.Count; i++)
        {
            if (newKey.Time < keys[i].Time)
            {
                keys.Insert(i, newKey);
                return i;
            }
        }

        keys.Add(newKey);
        return keys.Count - 1;
    }

    public void RemoveKey(int index)
    {
        if (keys.Count >= 2)
        {
            keys.RemoveAt(index);
        }
    }

    public int UpdateKeyTime(int index, float time)
    {
        Color col = keys[index].Colour;
        RemoveKey(index);
        return AddKey(col, time);
    }

    public void UpdateKeyColour(int index, Color col)
    {
        keys[index] = new ColourKey(col, keys[index].Time);
    }

    public int NumKeys
    {
        get
        {
            return keys.Count;
        }
    }

    public ColourKey GetKey(int i)
    {
        return keys[i];
    }

    public Texture2D GetTexture(int width)
    {
        Texture2D texture = new Texture2D(width, 1);
        Color[] colours = new Color[width];
        for (int i = 0; i < width; i++)
        {
            colours[i] = Evaluate((float)i / (width - 1));
        }
        texture.SetPixels(colours);
        texture.Apply();
        return texture;
    }

    public void Import(Gradient gradient)
    {
       // keys = new List<ColourKey>();
        //AddKey(gradient.Evaluate(0), 0);
        //AddKey(gradient.Evaluate(1), 1);
        UpdateKeyColour(0, gradient.Evaluate(0));
        UpdateKeyColour(1, gradient.Evaluate(1));
        for (int i = 0; i < gradient.colorKeys.Length; i++)
        {
            if (gradient.colorKeys[i].time > 0 || gradient.colorKeys[i].time < 1)
            AddKey(gradient.colorKeys[i].color, gradient.colorKeys[i].time);
        }
    }

    [System.Serializable]
    public struct ColourKey
    {
        [SerializeField]
        [ColorUsage(false, true)]
        Color colour;
        [SerializeField]
        float time;

        public ColourKey(Color colour, float time)
        {
            this.colour = colour;
            this.time = time;
        }

        public Color Colour
        {
            get
            {
                return colour;
            }
        }

        public float Time
        {
            get
            {
                return time;
            }
        }
    }

}