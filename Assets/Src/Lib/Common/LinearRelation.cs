using UnityEngine;

public class LinearRelation
{
    private float a, b;
    private float minY, maxY;

    //=== Ctor ==============================================================

    public LinearRelation(float x1, float y1, float x2, float y2)
    {
        Recalculation(x1, y1, x2, y2);
    }


    //=== Public ==============================================================

    public void Recalculation(float x1, float y1, float x2, float y2)
    {
        //A*x + B = y
        a = (y2 - y1) / (x2 - x1);
        b = y1 - a * x1;

        if (y1 < y2)
        {
            minY = y1;
            maxY = y2;
        }
        else
        {
            minY = y2;
            maxY = y1;
        }
    }

    public float GetY(float x)
    {
        return a * x + b;
    }

    /// <summary>
    /// Возвращает значение не выходящее за диапазон y-значений, которыми задавалась зависимость
    /// </summary>
    public float GetClampedY(float x)
    {
        return Mathf.Clamp(GetY(x), minY, maxY);
    }

    public float GetX(float y)
    {
        return (y - b) / a; //проверку a на 0 не делаем - пусть падает, если так
    }


    //=== Static-calcer вариант ===============================================

    public static float GetY(Vector4 x1y1x2y2, float x)
    {
        return Mathf.Approximately(x1y1x2y2.w, x1y1x2y2.y)
            ? x1y1x2y2.y
            : (x - x1y1x2y2.x) / (x1y1x2y2.z - x1y1x2y2.x) * (x1y1x2y2.w - x1y1x2y2.y) + x1y1x2y2.y;
    }

    public static float GetClampedY(Vector4 x1y1x2y2, float x)
    {
        return Mathf.Clamp(GetY(x1y1x2y2, x), Mathf.Min(x1y1x2y2.y, x1y1x2y2.w), Mathf.Max(x1y1x2y2.y, x1y1x2y2.w));
    }

    public static float GetX(Vector4 x1y1x2y2, float y)
    {
        return Mathf.Approximately(x1y1x2y2.x, x1y1x2y2.z)
            ? x1y1x2y2.x
            : (y - x1y1x2y2.y) * (x1y1x2y2.z - x1y1x2y2.x) / (x1y1x2y2.w - x1y1x2y2.y) + x1y1x2y2.y;
    }
}