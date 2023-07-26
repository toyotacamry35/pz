using System;
using System.Collections;
using UnityEngine;
using Assets.Src.Telemetry;

public class UIFps : MonoBehaviour
{
    public Material TextMaterial;
    public float TextSpacing = 0;
    public int CharsCountInTexture = 16;
    public int ZeroIndexInTexture = 1;
    public Rect Rect = new Rect(0,0,0.1f,0.1f);
    public Rect Rect2 = new Rect(0,0,0.1f,0.1f);
    public Vector2 Margins = Vector2.zero;
    public int CharsCount = 3;
    public Color _bgColor;
    public Gradient _gradient;
    public int MinFps = 10;
    public int MaxFps = 60;

    private float _fps;
    private bool _useRect2;
    private Coroutine _coroutine;
    private readonly WaitForEndOfFrame _waiter = new WaitForEndOfFrame();

    public void SetFps(float fps)
    {
        _fps = fps;
    }

    public void SetUseRect2(bool use)
    {
        _useRect2 = use;
    }

    private Rect CurrentRect => _useRect2 ? Rect2 : Rect;
    
    private void OnEnable()
    {
        if (_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(RenderLoop());
    }

    private void OnDisable()
    {
        if (_coroutine != null) StopCoroutine(_coroutine);
    }
    
    private IEnumerator RenderLoop()
    {
        while (true)
        {
            yield return _waiter;
            Render();
        }
    }
    
    private void Render()
    {
        if (TelemetrySystem.Telemetry == null)
            return;
        
        try
        {
            GL.PushMatrix();
            GL.LoadPixelMatrix();
            TextMaterial.SetPass(0);
            GL.Begin(GL.QUADS);
            var fps = (int)_fps;
            var color = _gradient.Evaluate(Mathf.InverseLerp(MinFps, MaxFps, fps));
            var rect = TransformRect(CurrentRect);
            GL.Color(_bgColor);
            DrawRect(rect);
            GL.Color(color);
            var margins = TransformVector(Margins);
            rect = Rect.MinMaxRect(rect.xMin + margins.x, rect.yMin + margins.y, rect.xMax - margins.x, rect.yMax - margins.y);
            DrawNumber(fps, rect, TextSpacing);
        }
        finally
        {
            GL.End();
            GL.PopMatrix();
        }
    }

    private Rect TransformRect(Rect rect)
    {
        return new Rect(
            Mathf.FloorToInt(Screen.width * rect.xMin) + 0.5f,
            Mathf.FloorToInt(Screen.height * rect.yMin),
            Mathf.CeilToInt(Screen.width * rect.width),
            Mathf.CeilToInt(Screen.height * rect.height));
    }

    private Vector2 TransformVector(Vector2 v)
    {
        return new Vector2(Screen.width * v.x, Screen.height * v.y);
    }
    
    private void DrawNumber(int value, Rect rect, float spacing)
    {
        int count = Math.Max((int)Math.Floor(Math.Log10(value)) + 1, 1);
        int charsMaxCount = CharsCount != -1 ? CharsCount : count;  
        float charWdt = (rect.width - (charsMaxCount - 1) * spacing) / charsMaxCount;
        for (int i = 0; i < count; ++i)
        {
            int nextValue = value / 10;
            int digit = (value - nextValue * 10);
            value = nextValue;
            DrawDigit(digit, new Rect(rect.xMax - (i + 1) * charWdt - i * spacing, rect.yMin, charWdt, rect.height));
        }
    }
    
    private void DrawRect(Rect rect)
    {
        GL.TexCoord(Vector3.one);
        GL.Vertex(new Vector3(rect.xMax, rect.yMin));
        GL.Vertex(new Vector3(rect.xMin, rect.yMin));
        GL.Vertex(new Vector3(rect.xMin, rect.yMax));
        GL.Vertex(new Vector3(rect.xMax, rect.yMax));
    }
    
    private void DrawDigit(int digit, Rect rect)
    {
        var uv = new Rect(1.0f / CharsCountInTexture * (digit + ZeroIndexInTexture), 0, 1.0f / CharsCountInTexture, 1);
        GL.TexCoord(new Vector3(uv.xMax, uv.yMin));
        GL.Vertex(new Vector3(rect.xMax, rect.yMin));
        GL.TexCoord(new Vector3(uv.xMin, uv.yMin));
        GL.Vertex(new Vector3(rect.xMin, rect.yMin));
        GL.TexCoord(new Vector3(uv.xMin, uv.yMax));
        GL.Vertex(new Vector3(rect.xMin, rect.yMax));
        GL.TexCoord(new Vector3(uv.xMax, uv.yMax));
        GL.Vertex(new Vector3(rect.xMax, rect.yMax));
    }
}