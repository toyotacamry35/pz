using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TOD;

namespace TOD
{
    [System.Serializable]
    public struct GradientPart
    {
        public DayName state;
        public GradientHDR gradient;

        public GradientPart(GradientHDR gradient, DayName state)
        {
            this.gradient = gradient;
            this.state = state;
        }
    }

    [System.Serializable]
    public struct CurvePart
    {
        public DayName state;
        [SerializeField]
        public Keyframe[] keys;

        public CurvePart(Keyframe[] keys, DayName state)
        {
            this.keys = keys;
            this.state = state;
        }
    }

    [System.Serializable]
    public struct ColorGradient
    {
        public bool use;
        public GradientHDR gradient;
        [ColorUsageAttribute(false, true)]
        public Color color;

        public List<GradientPart> parts;

        public void Evaluate(float _time)
        {
            if (use) color = gradient.Evaluate(_time);
        }

        public Color EvaluateF(float _time)
        {
            if (use)
                return gradient.Evaluate(_time);
            else
                return color;
        }

        public void ApplyToGradient()
        {
            if (!use)
            {
                gradient = new GradientHDR();
                gradient.UpdateKeyColour(0, color);
                gradient.UpdateKeyColour(1, color);

                use = true;
            }
        }

        public void GetGradient(ColorGradient _gradient)
        {
            gradient = new GradientHDR();
            for (int i = 0; i < _gradient.gradient.NumKeys; i++)
            {
                if (i == 0 || i == _gradient.gradient.NumKeys - 1)
                    gradient.UpdateKeyColour(i, _gradient.gradient.GetKey(i).Colour);
                else
                    gradient.AddKey(_gradient.gradient.GetKey(i).Colour, _gradient.gradient.GetKey(i).Time);
                   
            }
        }

        public void SplitGradientToParts(DayPart[] dayParts)
        {
            parts = new List<GradientPart>();

            GradientHDR temp = new GradientHDR();
            for (int i = 0; i < gradient.NumKeys; i++)
            {
                if (i == 0 || i == gradient.NumKeys - 1)
                    temp.UpdateKeyColour(i, gradient.GetKey(i).Colour);
                else
                    temp.AddKey(gradient.GetKey(i).Colour, gradient.GetKey(i).Time);
            }
            for (int i = 0; i < dayParts.Length; i++)
            {
                Color interpolatedColor = gradient.Evaluate(dayParts[i].percent.x);
                temp.AddKey(interpolatedColor, dayParts[i].percent.x);
            }

            for (int i=0; i< dayParts.Length; i++)
            {  
                if (dayParts[i].percent.x < dayParts[i].percent.y)
                {
                    float start = dayParts[i].percent.x;
                    float size = dayParts[i].percent.y - dayParts[i].percent.x;
                    GradientHDR part = new GradientHDR();

                    for (int j=0; j<temp.NumKeys; j++)
                    {
                        GradientHDR.ColourKey key = temp.GetKey(j);
                        if (key.Time >= dayParts[i].percent.x && key.Time <= dayParts[i].percent.y)
                        {
                            float newTime = (key.Time - start) / size;
                            if (newTime == 0 || newTime == 1)
                            {
                                if (newTime == 0)
                                    part.UpdateKeyColour(0, key.Colour);
                                else
                                    part.UpdateKeyColour(part.NumKeys-1, key.Colour);
                            }
                            else
                            part.AddKey(key.Colour, newTime);
                        }
                    }
                    parts.Add(new GradientPart(part, dayParts[i].state));
                }
                else
                {
                    for (int z = 0; z < 2; z++)
                    {
                        GradientHDR part = new GradientHDR();
                        for (int j = 0; j < temp.NumKeys; j++)
                        {
                            GradientHDR.ColourKey key = temp.GetKey(j);
                            if (key.Time >= dayParts[i].percent.x && z == 0)
                            {
                                float size = (1f - dayParts[i].percent.x);
                                float start = dayParts[i].percent.x;
                                float newTime = (key.Time - start) / size;
                                if (newTime == 0 || newTime == 1)
                                {
                                    if (newTime == 0)
                                        part.UpdateKeyColour(0, key.Colour);
                                    else
                                        part.UpdateKeyColour(part.NumKeys - 1, key.Colour);
                                }
                                else
                                    part.AddKey(key.Colour, newTime);
                            }
                            else
                            if (key.Time <= dayParts[i].percent.y && z == 1)
                            {
                                float size = dayParts[i].percent.y;
                                float newTime = key.Time / size;
                                if (newTime == 0 || newTime == 1)
                                {
                                    if (newTime == 0)
                                        part.UpdateKeyColour(0, key.Colour);
                                    else
                                        part.UpdateKeyColour(part.NumKeys - 1, key.Colour);
                                }
                                else
                                    part.AddKey(key.Colour, newTime);
                            }
                        }
                        if (z==1)
                            parts.Add(new GradientPart(part, dayParts[i].state));
                        else
                        parts.Add(new GradientPart(part, DayName.DarkNight));
                    }
                } 
            }

        }

        public void MergePartsToGradient(DayPart[] dayParts)
        {
            gradient = new GradientHDR();
            for (int i = 0; i < parts.Count; i++)
            {
                for (int z = 0; z < dayParts.Length; z++)
                {
                    if (parts[i].state == DayName.DarkNight)
                    {
                        if (dayParts[z].percent.x > dayParts[z].percent.y)
                        {
                            float start = dayParts[z].percent.x;
                            float size = 1f - dayParts[z].percent.x;
                            for (int j = 0; j < parts[i].gradient.NumKeys; j++)
                            {
                                GradientHDR.ColourKey key = parts[i].gradient.GetKey(j);
                                if (j != 0 && j != parts[i].gradient.NumKeys - 1)
                                {
                                    float newTime = key.Time * size + start;
                                    gradient.AddKey(key.Colour, newTime);
                                }
                            }
                            gradient.UpdateKeyColour(gradient.NumKeys - 1, parts[i].gradient.Evaluate(1));
                        }
                    }
                    else
                    {
                        if (parts[i].state == dayParts[z].state)
                        {
                            float start = 0;
                            float size = 1;

                            if (dayParts[z].percent.x < dayParts[z].percent.y)
                            {
                                start = dayParts[z].percent.x;
                                size = dayParts[z].percent.y - dayParts[z].percent.x;
                            }
                            else
                            {
                                start = 0;
                                size = dayParts[z].percent.y;
                                gradient.UpdateKeyColour(0, parts[i].gradient.Evaluate(0));
                            }
                            for (int j = 0; j < parts[i].gradient.NumKeys; j++)
                            {
                                GradientHDR.ColourKey key = parts[i].gradient.GetKey(j);
                                if (j != 0 && j != parts[i].gradient.NumKeys - 1)
                                {
                                    float newTime = key.Time * size + start;
                                    gradient.AddKey(key.Colour, newTime);
                                }
                            } 
                        }
                    }
                }
            }
            
            parts.Clear();
        }

        public void Eclipse()
        {
            color = Color.Lerp(color, Color.black, ASkyLighting.eclipsePower);
        }
    }

    [System.Serializable]
    public struct FloatCurve
    {
        public bool use;
        public AnimationCurve curve;
        public float value;

        public List<CurvePart> parts;

        public void Evaluate(float _time)
        {
            if (use) value = curve.Evaluate(_time);
        }

        public float EvaluateF(float _time)
        {
            if (use)
                return curve.Evaluate(_time);
            else
                return value;
        }

        public void ApplyToCurve()
        {
            if (!use) curve = new AnimationCurve(new Keyframe(0, value), new Keyframe(1, value));
            use = true;
        }

        public void GetCurve(FloatCurve _curve)
        {
            curve = new AnimationCurve();
            if (_curve.curve != null)
            {
                curve.keys = new Keyframe[_curve.curve.keys.Length];
                curve.keys = _curve.curve.keys;
            }
        }

        public void Eclipse()
        {
            value = Mathf.Lerp(value, 0, ASkyLighting.eclipsePower);
        }

        public void SplitCurveToParts(DayPart[] dayParts)
        {
            parts = new List<CurvePart>();
            for (int i = 0; i < dayParts.Length; i++)
            {
                if (dayParts[i].percent.x < dayParts[i].percent.y)
                {
                    float start = dayParts[i].percent.x;
                    float size = dayParts[i].percent.y - dayParts[i].percent.x;
                    List<Keyframe> keys = new List<Keyframe>();

                    for (int j = 0; j < curve.length; j++)
                    {
                        Keyframe key = curve.keys[j];
                        if (key.time >= dayParts[i].percent.x && key.time <= dayParts[i].percent.y)
                        {
                            float newTime = (key.time - start) / size;
                            key.time = newTime;
                            keys.Add(key);
                        }
                    }
                    parts.Add(new CurvePart(keys.ToArray(), dayParts[i].state));
                }
                
                else
                {
                    for (int z = 0; z < 2; z++)
                    {
                        List<Keyframe> keys = new List<Keyframe>();
                        for (int j = 0; j < curve.length; j++)
                        {
                            Keyframe key = curve.keys[j];

                            if (key.time >= dayParts[i].percent.x && z == 0)
                            {
                                float size = (1f - dayParts[i].percent.x);
                                float start = dayParts[i].percent.x;
                                float newTime = (key.time - start) / size;
                                key.time = newTime;
                                keys.Add(key);
                            }
                            else
                            if (key.time <= dayParts[i].percent.y && z == 1)
                            {
                                float size = dayParts[i].percent.y;
                                float newTime = key.time / size;
                                key.time = newTime;
                                keys.Add(key);
                            }
                        }
                        if (z == 1)
                            parts.Add(new CurvePart(keys.ToArray(), dayParts[i].state));
                        else
                            parts.Add(new CurvePart(keys.ToArray(), DayName.DarkNight));
                    }
                }
            }
        }

        public void MergePartsToCurve(DayPart[] dayParts)
        {
            curve = new AnimationCurve();
            
            for (int i = 0; i < parts.Count; i++)
            {
                for (int z = 0; z < dayParts.Length; z++)
                {
                    
                    if (parts[i].state == DayName.DarkNight)
                    {
                        if (dayParts[z].percent.x > dayParts[z].percent.y)
                        {
                            float start = dayParts[z].percent.x;
                            float size = 1f - dayParts[z].percent.x;
                            for (int j = 0; j < parts[i].keys.Length; j++)
                            {
                                Keyframe key = parts[i].keys[j];
                                float newTime = key.time * size + start;
                                key.time = newTime;
                                curve.AddKey(key);
                            }
                        }
                    }
                    else
                    {
                    
                        if (parts[i].state == dayParts[z].state)
                        {
                            float start = 0;
                            float size = 1;

                            if (dayParts[z].percent.x < dayParts[z].percent.y)
                            {
                                start = dayParts[z].percent.x;
                                size = dayParts[z].percent.y - dayParts[z].percent.x;
                            }
                            else
                            {
                                start = 0;
                                size = dayParts[z].percent.y;
                            }
                            for (int j = 0; j < parts[i].keys.Length; j++)
                            {
                                Keyframe key = parts[i].keys[j];
                                float newTime = key.time * size + start;
                                key.time = newTime;
                                curve.AddKey(key);
                            }

                        }
                    }

                }

            }

            parts.Clear();
        }
    }

    public class Utils
    {
        public static DayName GetCurrentDayTime()
        {
            if (TOD.ASkyLighting._instance == null)
                return DayName.Day;

            float currentTime;
            int currentIndex = 0;
            Utils.GetDayElementCurrent(TOD.ASkyLighting._instance.dayParts, out currentTime, out currentIndex);

            if (TOD.ASkyLighting._instance.context.isEclipse && ASkyLighting.eclipsePower == 1)
                return DayName.DarkNight;

            return (DayName)currentIndex;
        }
        public static void GetDayElementCurrent(DayPart[] dayElement, out float localTime, out int select)
        {
            localTime = ASkyLighting.CGTime;
            select = -1;
            for (int i = 0; i < dayElement.Length; i++)
            {
                if (i == 0)
                {
                    if (dayElement[i].percent.x <= ASkyLighting.CGTime || dayElement[i].percent.y > ASkyLighting.CGTime)
                    {
                        select = 0;

                        float lengthEnd = 1.0f - dayElement[i].percent.x;
                        float lengthStart = dayElement[i].percent.y;
                        float length = lengthStart + lengthEnd;
                        float diff = lengthEnd / (length);

                        if (ASkyLighting.CGTime > dayElement[i].percent.x)
                        {
                            float Xr = localTime - dayElement[i].percent.x;
                            float Xm = 1 / length;
                            float time = Xr * Xm;
                            localTime = time;
                        }
                        else
                        if (localTime < dayElement[i].percent.y)
                        {
                            float Xm = 1 / length;
                            float time = diff + localTime * Xm;
                            localTime = time;
                        }

                        break;
                    }
                }
                else
                if (dayElement[i].percent.x <= ASkyLighting.CGTime && dayElement[i].percent.y > ASkyLighting.CGTime)
                {
                    select = i;

                    float Xr = localTime - dayElement[i].percent.x;
                    float Xm = 1 / (dayElement[i].percent.y - dayElement[i].percent.x);
                    float time = Xr * Xm;
                    localTime = time;
                    break;
                }
            }
        }
    }
}

