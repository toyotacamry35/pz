using UnityEngine;
using System.Reflection;
using Core.Reflection;

/// <summary>
/// Debug Extension
/// 	- Static class that extends Unity's debugging functionallity.
/// 	- Attempts to mimic Unity's existing debugging behaviour for ease-of-use.
/// 	- Includes gizmo drawing methods for less memory-intensive debug visualization.
/// </summary>

public static class DebugExtension
{
    private const string DebugDrawKey = "DebugDraw";

    public static bool Draw
    {
        get { return PlayerPrefs.GetInt(DebugDrawKey) == 1; }
        set { PlayerPrefs.SetInt(DebugDrawKey, value ? 1 : 0); }
    }
    #region DebugDrawFunctions

    /// <summary>
    /// 	- Debugs a point.
    /// </summary>
    /// <param name='position'>
    /// 	- The point to debug.
    /// </param>
    /// <param name='color'>
    /// 	- The color of the point.
    /// </param>
    /// <param name='scale'>
    /// 	- The size of the point.
    /// </param>
    /// <param name='duration'>
    /// 	- How long to draw the point.
    /// </param>
    /// <param name='depthTest'>
    /// 	- Whether or not this point should be faded when behind other objects.
    /// </param>
    public static void DebugPoint(Vector3 position, Color color, float scale = 1.0f, float duration = 0, bool depthTest = true)
    {
        if (!Draw)
            return;
        color = (color == default(Color)) ? Color.white : color;

        Debug.DrawRay(position + (Vector3.up * (scale * 0.5f)), -Vector3.up * scale, color, duration, depthTest);
        Debug.DrawRay(position + (Vector3.right * (scale * 0.5f)), -Vector3.right * scale, color, duration, depthTest);
        Debug.DrawRay(position + (Vector3.forward * (scale * 0.5f)), -Vector3.forward * scale, color, duration, depthTest);
    }

    /// <summary>
    /// 	- Debugs a point.
    /// </summary>
    /// <param name='position'>
    /// 	- The point to debug.
    /// </param>
    /// <param name='scale'>
    /// 	- The size of the point.
    /// </param>
    /// <param name='duration'>
    /// 	- How long to draw the point.
    /// </param>
    /// <param name='depthTest'>
    /// 	- Whether or not this point should be faded when behind other objects.
    /// </param>
    public static void DebugPoint(Vector3 position, float scale = 1.0f, float duration = 0, bool depthTest = true)
    {
        if (!Draw)
            return;
        DebugPoint(position, Color.white, scale, duration, depthTest);
    }

    /// <summary>
    /// 	- Debugs an axis-aligned bounding box.
    /// </summary>
    /// <param name='bounds'>
    /// 	- The bounds to debug.
    /// </param>
    /// <param name='color'>
    /// 	- The color of the bounds.
    /// </param>
    /// <param name='duration'>
    /// 	- How long to draw the bounds.
    /// </param>
    /// <param name='depthTest'>
    /// 	- Whether or not the bounds should be faded when behind other objects.
    /// </param>
    public static void DebugBounds(Bounds bounds, Color color, float duration = 0, bool depthTest = true)
    {
        if (!Draw)
            return;
        Vector3 center = bounds.center;

        float x = bounds.extents.x;
        float y = bounds.extents.y;
        float z = bounds.extents.z;

        Vector3 ruf = center + new Vector3(x, y, z);
        Vector3 rub = center + new Vector3(x, y, -z);
        Vector3 luf = center + new Vector3(-x, y, z);
        Vector3 lub = center + new Vector3(-x, y, -z);

        Vector3 rdf = center + new Vector3(x, -y, z);
        Vector3 rdb = center + new Vector3(x, -y, -z);
        Vector3 lfd = center + new Vector3(-x, -y, z);
        Vector3 lbd = center + new Vector3(-x, -y, -z);

        Debug.DrawLine(ruf, luf, color, duration, depthTest);
        Debug.DrawLine(ruf, rub, color, duration, depthTest);
        Debug.DrawLine(luf, lub, color, duration, depthTest);
        Debug.DrawLine(rub, lub, color, duration, depthTest);

        Debug.DrawLine(ruf, rdf, color, duration, depthTest);
        Debug.DrawLine(rub, rdb, color, duration, depthTest);
        Debug.DrawLine(luf, lfd, color, duration, depthTest);
        Debug.DrawLine(lub, lbd, color, duration, depthTest);

        Debug.DrawLine(rdf, lfd, color, duration, depthTest);
        Debug.DrawLine(rdf, rdb, color, duration, depthTest);
        Debug.DrawLine(lfd, lbd, color, duration, depthTest);
        Debug.DrawLine(lbd, rdb, color, duration, depthTest);
    }
    public static void DebugBox(Vector3 center, Vector3 extents, Quaternion rotation, Color color, float duration = 0, bool depthTest = true)
    {
        if (!Draw)
            return;
        float x = extents.x;
        float y = extents.y;
        float z = extents.z;

        Vector3 ruf = center + rotation * new Vector3(x, y, z);
        Vector3 rub = center + rotation * new Vector3(x, y, -z);
        Vector3 luf = center + rotation * new Vector3(-x, y, z);
        Vector3 lub = center + rotation * new Vector3(-x, y, -z);

        Vector3 rdf = center + rotation * new Vector3(x, -y, z);
        Vector3 rdb = center + rotation * new Vector3(x, -y, -z);
        Vector3 lfd = center + rotation * new Vector3(-x, -y, z);
        Vector3 lbd = center + rotation * new Vector3(-x, -y, -z);

        Debug.DrawLine(ruf, luf, color, duration, depthTest);
        Debug.DrawLine(ruf, rub, color, duration, depthTest);
        Debug.DrawLine(luf, lub, color, duration, depthTest);
        Debug.DrawLine(rub, lub, color, duration, depthTest);

        Debug.DrawLine(ruf, rdf, color, duration, depthTest);
        Debug.DrawLine(rub, rdb, color, duration, depthTest);
        Debug.DrawLine(luf, lfd, color, duration, depthTest);
        Debug.DrawLine(lub, lbd, color, duration, depthTest);

        Debug.DrawLine(rdf, lfd, color, duration, depthTest);
        Debug.DrawLine(rdf, rdb, color, duration, depthTest);
        Debug.DrawLine(lfd, lbd, color, duration, depthTest);
        Debug.DrawLine(lbd, rdb, color, duration, depthTest);
    }
    /// <summary>
    /// 	- Debugs an axis-aligned bounding box.
    /// </summary>
    /// <param name='bounds'>
    /// 	- The bounds to debug.
    /// </param>
    /// <param name='duration'>
    /// 	- How long to draw the bounds.
    /// </param>
    /// <param name='depthTest'>
    /// 	- Whether or not the bounds should be faded when behind other objects.
    /// </param>
    public static void DebugBounds(Bounds bounds, float duration = 0, bool depthTest = true)
    {
        if (!Draw)
            return;
        DebugBounds(bounds, Color.white, duration, depthTest);
    }

    /// <summary>
    /// 	- Debugs a local cube.
    /// </summary>
    /// <param name='transform'>
    /// 	- The transform that the cube will be local to.
    /// </param>
    /// <param name='size'>
    /// 	- The size of the cube.
    /// </param>
    /// <param name='color'>
    /// 	- Color of the cube.
    /// </param>
    /// <param name='center'>
    /// 	- The position (relative to transform) where the cube will be debugged.
    /// </param>
    /// <param name='duration'>
    /// 	- How long to draw the cube.
    /// </param>
    /// <param name='depthTest'>
    /// 	- Whether or not the cube should be faded when behind other objects.
    /// </param>
    public static void DebugLocalCube(Transform transform, Vector3 size, Color color, Vector3 center = default(Vector3), float duration = 0, bool depthTest = true)
    {
        if (!Draw)
            return;
        Vector3 lbb = transform.TransformPoint(center + ((-size) * 0.5f));
        Vector3 rbb = transform.TransformPoint(center + (new Vector3(size.x, -size.y, -size.z) * 0.5f));

        Vector3 lbf = transform.TransformPoint(center + (new Vector3(size.x, -size.y, size.z) * 0.5f));
        Vector3 rbf = transform.TransformPoint(center + (new Vector3(-size.x, -size.y, size.z) * 0.5f));

        Vector3 lub = transform.TransformPoint(center + (new Vector3(-size.x, size.y, -size.z) * 0.5f));
        Vector3 rub = transform.TransformPoint(center + (new Vector3(size.x, size.y, -size.z) * 0.5f));

        Vector3 luf = transform.TransformPoint(center + ((size) * 0.5f));
        Vector3 ruf = transform.TransformPoint(center + (new Vector3(-size.x, size.y, size.z) * 0.5f));

        Debug.DrawLine(lbb, rbb, color, duration, depthTest);
        Debug.DrawLine(rbb, lbf, color, duration, depthTest);
        Debug.DrawLine(lbf, rbf, color, duration, depthTest);
        Debug.DrawLine(rbf, lbb, color, duration, depthTest);

        Debug.DrawLine(lub, rub, color, duration, depthTest);
        Debug.DrawLine(rub, luf, color, duration, depthTest);
        Debug.DrawLine(luf, ruf, color, duration, depthTest);
        Debug.DrawLine(ruf, lub, color, duration, depthTest);

        Debug.DrawLine(lbb, lub, color, duration, depthTest);
        Debug.DrawLine(rbb, rub, color, duration, depthTest);
        Debug.DrawLine(lbf, luf, color, duration, depthTest);
        Debug.DrawLine(rbf, ruf, color, duration, depthTest);
    }

    /// <summary>
    /// 	- Debugs a local cube.
    /// </summary>
    /// <param name='transform'>
    /// 	- The transform that the cube will be local to.
    /// </param>
    /// <param name='size'>
    /// 	- The size of the cube.
    /// </param>
    /// <param name='center'>
    /// 	- The position (relative to transform) where the cube will be debugged.
    /// </param>
    /// <param name='duration'>
    /// 	- How long to draw the cube.
    /// </param>
    /// <param name='depthTest'>
    /// 	- Whether or not the cube should be faded when behind other objects.
    /// </param>
    public static void DebugLocalCube(Transform transform, Vector3 size, Vector3 center = default(Vector3), float duration = 0, bool depthTest = true)
    {
        if (!Draw)
            return;
        DebugLocalCube(transform, size, Color.white, center, duration, depthTest);
    }

    /// <summary>
    /// 	- Debugs a local cube.
    /// </summary>
    /// <param name='space'>
    /// 	- The space the cube will be local to.
    /// </param>
    /// <param name='size'>
    ///		- The size of the cube.
    /// </param>
    /// <param name='color'>
    /// 	- Color of the cube.
    /// </param>
    /// <param name='center'>
    /// 	- The position (relative to transform) where the cube will be debugged.
    /// </param>
    /// <param name='duration'>
    /// 	- How long to draw the cube.
    /// </param>
    /// <param name='depthTest'>
    /// 	- Whether or not the cube should be faded when behind other objects.
    /// </param>
    public static void DebugLocalCube(Matrix4x4 space, Vector3 size, Color color, Vector3 center = default(Vector3), float duration = 0, bool depthTest = true)
    {
        if (!Draw)
            return;
        color = (color == default(Color)) ? Color.white : color;

        Vector3 lbb = space.MultiplyPoint3x4(center + ((-size) * 0.5f));
        Vector3 rbb = space.MultiplyPoint3x4(center + (new Vector3(size.x, -size.y, -size.z) * 0.5f));

        Vector3 lbf = space.MultiplyPoint3x4(center + (new Vector3(size.x, -size.y, size.z) * 0.5f));
        Vector3 rbf = space.MultiplyPoint3x4(center + (new Vector3(-size.x, -size.y, size.z) * 0.5f));

        Vector3 lub = space.MultiplyPoint3x4(center + (new Vector3(-size.x, size.y, -size.z) * 0.5f));
        Vector3 rub = space.MultiplyPoint3x4(center + (new Vector3(size.x, size.y, -size.z) * 0.5f));

        Vector3 luf = space.MultiplyPoint3x4(center + ((size) * 0.5f));
        Vector3 ruf = space.MultiplyPoint3x4(center + (new Vector3(-size.x, size.y, size.z) * 0.5f));

        Debug.DrawLine(lbb, rbb, color, duration, depthTest);
        Debug.DrawLine(rbb, lbf, color, duration, depthTest);
        Debug.DrawLine(lbf, rbf, color, duration, depthTest);
        Debug.DrawLine(rbf, lbb, color, duration, depthTest);

        Debug.DrawLine(lub, rub, color, duration, depthTest);
        Debug.DrawLine(rub, luf, color, duration, depthTest);
        Debug.DrawLine(luf, ruf, color, duration, depthTest);
        Debug.DrawLine(ruf, lub, color, duration, depthTest);

        Debug.DrawLine(lbb, lub, color, duration, depthTest);
        Debug.DrawLine(rbb, rub, color, duration, depthTest);
        Debug.DrawLine(lbf, luf, color, duration, depthTest);
        Debug.DrawLine(rbf, ruf, color, duration, depthTest);
    }

    /// <summary>
    /// 	- Debugs a local cube.
    /// </summary>
    /// <param name='space'>
    /// 	- The space the cube will be local to.
    /// </param>
    /// <param name='size'>
    ///		- The size of the cube.
    /// </param>
    /// <param name='center'>
    /// 	- The position (relative to transform) where the cube will be debugged.
    /// </param>
    /// <param name='duration'>
    /// 	- How long to draw the cube.
    /// </param>
    /// <param name='depthTest'>
    /// 	- Whether or not the cube should be faded when behind other objects.
    /// </param>
    public static void DebugLocalCube(Matrix4x4 space, Vector3 size, Vector3 center = default(Vector3), float duration = 0, bool depthTest = true)
    {
        if (!Draw)
            return;
        DebugLocalCube(space, size, Color.white, center, duration, depthTest);
    }

    /// <summary>
    /// 	- Debugs a circle.
    /// </summary>
    /// <param name='position'>
    /// 	- Where the center of the circle will be positioned.
    /// </param>
    /// <param name='up'>
    /// 	- The direction perpendicular to the surface of the circle.
    /// </param>
    /// <param name='color'>
    /// 	- The color of the circle.
    /// </param>
    /// <param name='radius'>
    /// 	- The radius of the circle.
    /// </param>
    /// <param name='duration'>
    /// 	- How long to draw the circle.
    /// </param>
    /// <param name='depthTest'>
    /// 	- Whether or not the circle should be faded when behind other objects.
    /// </param>
    public static void DebugCircle(Vector3 position, Vector3 up, Color color, float radius = 1.0f, float duration = 0, bool depthTest = true)
    {
        if (!Draw)
            return;
        Vector3 _up = up.normalized * radius;
        Vector3 _forward = Vector3.Slerp(_up, -_up, 0.5f);
        Vector3 _right = Vector3.Cross(_up, _forward).normalized * radius;

        Matrix4x4 matrix = new Matrix4x4();

        matrix[0] = _right.x;
        matrix[1] = _right.y;
        matrix[2] = _right.z;

        matrix[4] = _up.x;
        matrix[5] = _up.y;
        matrix[6] = _up.z;

        matrix[8] = _forward.x;
        matrix[9] = _forward.y;
        matrix[10] = _forward.z;

        Vector3 _lastPoint = position + matrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(0), 0, Mathf.Sin(0)));
        Vector3 _nextPoint = Vector3.zero;

        color = (color == default(Color)) ? Color.white : color;

        for (var i = 0; i < 91; i++)
        {
            _nextPoint.x = Mathf.Cos((i * 4) * Mathf.Deg2Rad);
            _nextPoint.z = Mathf.Sin((i * 4) * Mathf.Deg2Rad);
            _nextPoint.y = 0;

            _nextPoint = position + matrix.MultiplyPoint3x4(_nextPoint);

            Debug.DrawLine(_lastPoint, _nextPoint, color, duration, depthTest);
            _lastPoint = _nextPoint;
        }
    }

    /// <summary>
    /// 	- Debugs a circle.
    /// </summary>
    /// <param name='position'>
    /// 	- Where the center of the circle will be positioned.
    /// </param>
    /// <param name='color'>
    /// 	- The color of the circle.
    /// </param>
    /// <param name='radius'>
    /// 	- The radius of the circle.
    /// </param>
    /// <param name='duration'>
    /// 	- How long to draw the circle.
    /// </param>
    /// <param name='depthTest'>
    /// 	- Whether or not the circle should be faded when behind other objects.
    /// </param>
    public static void DebugCircle(Vector3 position, Color color, float radius = 1.0f, float duration = 0, bool depthTest = true)
    {
        if (!Draw)
            return;
        DebugCircle(position, Vector3.up, color, radius, duration, depthTest);
    }

    /// <summary>
    /// 	- Debugs a circle.
    /// </summary>
    /// <param name='position'>
    /// 	- Where the center of the circle will be positioned.
    /// </param>
    /// <param name='up'>
    /// 	- The direction perpendicular to the surface of the circle.
    /// </param>
    /// <param name='radius'>
    /// 	- The radius of the circle.
    /// </param>
    /// <param name='duration'>
    /// 	- How long to draw the circle.
    /// </param>
    /// <param name='depthTest'>
    /// 	- Whether or not the circle should be faded when behind other objects.
    /// </param>
    public static void DebugCircle(Vector3 position, Vector3 up, float radius = 1.0f, float duration = 0, bool depthTest = true)
    {
        if (!Draw)
            return;
        DebugCircle(position, up, Color.white, radius, duration, depthTest);
    }

    /// <summary>
    /// 	- Debugs a circle.
    /// </summary>
    /// <param name='position'>
    /// 	- Where the center of the circle will be positioned.
    /// </param>
    /// <param name='radius'>
    /// 	- The radius of the circle.
    /// </param>
    /// <param name='duration'>
    /// 	- How long to draw the circle.
    /// </param>
    /// <param name='depthTest'>
    /// 	- Whether or not the circle should be faded when behind other objects.
    /// </param>
    public static void DebugCircle(Vector3 position, float radius = 1.0f, float duration = 0, bool depthTest = true)
    {
        if (!Draw)
            return;
        DebugCircle(position, Vector3.up, Color.white, radius, duration, depthTest);
    }

    /// <summary>
    /// 	- Debugs a wire sphere.
    /// </summary>
    /// <param name='position'>
    /// 	- The position of the center of the sphere.
    /// </param>
    /// <param name='color'>
    /// 	- The color of the sphere.
    /// </param>
    /// <param name='radius'>
    /// 	- The radius of the sphere.
    /// </param>
    /// <param name='duration'>
    /// 	- How long to draw the sphere.
    /// </param>
    /// <param name='depthTest'>
    /// 	- Whether or not the sphere should be faded when behind other objects.
    /// </param>
    public static void DebugWireSphere(Vector3 position, Color color, float radius = 1.0f, float duration = 0, bool depthTest = true)
    {
        if (!Draw)
            return;
        float angle = 10.0f;

        Vector3 x = new Vector3(position.x, position.y + radius * Mathf.Sin(0), position.z + radius * Mathf.Cos(0));
        Vector3 y = new Vector3(position.x + radius * Mathf.Cos(0), position.y, position.z + radius * Mathf.Sin(0));
        Vector3 z = new Vector3(position.x + radius * Mathf.Cos(0), position.y + radius * Mathf.Sin(0), position.z);

        Vector3 new_x;
        Vector3 new_y;
        Vector3 new_z;

        for (int i = 1; i < 37; i++)
        {

            new_x = new Vector3(position.x, position.y + radius * Mathf.Sin(angle * i * Mathf.Deg2Rad), position.z + radius * Mathf.Cos(angle * i * Mathf.Deg2Rad));
            new_y = new Vector3(position.x + radius * Mathf.Cos(angle * i * Mathf.Deg2Rad), position.y, position.z + radius * Mathf.Sin(angle * i * Mathf.Deg2Rad));
            new_z = new Vector3(position.x + radius * Mathf.Cos(angle * i * Mathf.Deg2Rad), position.y + radius * Mathf.Sin(angle * i * Mathf.Deg2Rad), position.z);

            Debug.DrawLine(x, new_x, color, duration, depthTest);
            Debug.DrawLine(y, new_y, color, duration, depthTest);
            Debug.DrawLine(z, new_z, color, duration, depthTest);

            x = new_x;
            y = new_y;
            z = new_z;
        }
    }

    /// <summary>
    /// 	- Debugs a wire sphere.
    /// </summary>
    /// <param name='position'>
    /// 	- The position of the center of the sphere.
    /// </param>
    /// <param name='radius'>
    /// 	- The radius of the sphere.
    /// </param>
    /// <param name='duration'>
    /// 	- How long to draw the sphere.
    /// </param>
    /// <param name='depthTest'>
    /// 	- Whether or not the sphere should be faded when behind other objects.
    /// </param>
    public static void DebugWireSphere(Vector3 position, float radius = 1.0f, float duration = 0, bool depthTest = true)
    {
        if (!Draw)
            return;
        DebugWireSphere(position, Color.white, radius, duration, depthTest);
    }

    /// <summary>
    /// 	- Debugs a cylinder.
    /// </summary>
    /// <param name='start'>
    /// 	- The position of one end of the cylinder.
    /// </param>
    /// <param name='end'>
    /// 	- The position of the other end of the cylinder.
    /// </param>
    /// <param name='color'>
    /// 	- The color of the cylinder.
    /// </param>
    /// <param name='radius'>
    /// 	- The radius of the cylinder.
    /// </param>
    /// <param name='duration'>
    /// 	- How long to draw the cylinder.
    /// </param>
    /// <param name='depthTest'>
    /// 	- Whether or not the cylinder should be faded when behind other objects.
    /// </param>
    public static void DebugCylinder(Vector3 start, Vector3 end, Color color, float radius = 1, float duration = 0, bool depthTest = true)
    {
        if (!Draw)
            return;
        Vector3 up = (end - start).normalized * radius;
        Vector3 forward = Vector3.Slerp(up, -up, 0.5f);
        Vector3 right = Vector3.Cross(up, forward).normalized * radius;

        //Radial circles
        DebugExtension.DebugCircle(start, up, color, radius, duration, depthTest);
        DebugExtension.DebugCircle(end, -up, color, radius, duration, depthTest);
        DebugExtension.DebugCircle((start + end) * 0.5f, up, color, radius, duration, depthTest);

        //Side lines
        Debug.DrawLine(start + right, end + right, color, duration, depthTest);
        Debug.DrawLine(start - right, end - right, color, duration, depthTest);

        Debug.DrawLine(start + forward, end + forward, color, duration, depthTest);
        Debug.DrawLine(start - forward, end - forward, color, duration, depthTest);

        //Start endcap
        Debug.DrawLine(start - right, start + right, color, duration, depthTest);
        Debug.DrawLine(start - forward, start + forward, color, duration, depthTest);

        //End endcap
        Debug.DrawLine(end - right, end + right, color, duration, depthTest);
        Debug.DrawLine(end - forward, end + forward, color, duration, depthTest);
    }

    /// <summary>
    /// 	- Debugs a cylinder.
    /// </summary>
    /// <param name='start'>
    /// 	- The position of one end of the cylinder.
    /// </param>
    /// <param name='end'>
    /// 	- The position of the other end of the cylinder.
    /// </param>
    /// <param name='radius'>
    /// 	- The radius of the cylinder.
    /// </param>
    /// <param name='duration'>
    /// 	- How long to draw the cylinder.
    /// </param>
    /// <param name='depthTest'>
    /// 	- Whether or not the cylinder should be faded when behind other objects.
    /// </param>
    public static void DebugCylinder(Vector3 start, Vector3 end, float radius = 1, float duration = 0, bool depthTest = true)
    {
        if (!Draw)
            return;
        DebugCylinder(start, end, Color.white, radius, duration, depthTest);
    }

    /// <summary>
    /// 	- Debugs a cone.
    /// </summary>
    /// <param name='position'>
    /// 	- The position for the tip of the cone.
    /// </param>
    /// <param name='direction'>
    /// 	- The direction for the cone gets wider in.
    /// </param>
    /// <param name='angle'>
    /// 	- The angle of the cone.
    /// </param>
    /// <param name='color'>
    /// 	- The color of the cone.
    /// </param>
    /// <param name='duration'>
    /// 	- How long to draw the cone.
    /// </param>
    /// <param name='depthTest'>
    /// 	- Whether or not the cone should be faded when behind other objects.
    /// </param>
    public static void DebugCone(Vector3 position, Vector3 direction, Color color, float angle = 45, float duration = 0, bool depthTest = true)
    {
        if (!Draw)
            return;
        float length = direction.magnitude;

        Vector3 _forward = direction;
        Vector3 _up = Vector3.Slerp(_forward, -_forward, 0.5f);
        Vector3 _right = Vector3.Cross(_forward, _up).normalized * length;

        direction = direction.normalized;

        Vector3 slerpedVector = Vector3.Slerp(_forward, _up, angle / 90.0f);

        float dist;
        var farPlane = new Plane(-direction, position + _forward);
        var distRay = new Ray(position, slerpedVector);

        farPlane.Raycast(distRay, out dist);

        Debug.DrawRay(position, slerpedVector.normalized * dist, color);
        Debug.DrawRay(position, Vector3.Slerp(_forward, -_up, angle / 90.0f).normalized * dist, color, duration, depthTest);
        Debug.DrawRay(position, Vector3.Slerp(_forward, _right, angle / 90.0f).normalized * dist, color, duration, depthTest);
        Debug.DrawRay(position, Vector3.Slerp(_forward, -_right, angle / 90.0f).normalized * dist, color, duration, depthTest);

        DebugExtension.DebugCircle(position + _forward, direction, color, (_forward - (slerpedVector.normalized * dist)).magnitude, duration, depthTest);
        DebugExtension.DebugCircle(position + (_forward * 0.5f), direction, color, ((_forward * 0.5f) - (slerpedVector.normalized * (dist * 0.5f))).magnitude, duration, depthTest);
    }

    /// <summary>
    /// 	- Debugs a cone.
    /// </summary>
    /// <param name='position'>
    /// 	- The position for the tip of the cone.
    /// </param>
    /// <param name='direction'>
    /// 	- The direction for the cone gets wider in.
    /// </param>
    /// <param name='angle'>
    /// 	- The angle of the cone.
    /// </param>
    /// <param name='duration'>
    /// 	- How long to draw the cone.
    /// </param>
    /// <param name='depthTest'>
    /// 	- Whether or not the cone should be faded when behind other objects.
    /// </param>
    public static void DebugCone(Vector3 position, Vector3 direction, float angle = 45, float duration = 0, bool depthTest = true)
    {
        if (!Draw)
            return;
        DebugCone(position, direction, Color.white, angle, duration, depthTest);
    }

    /// <summary>
    /// 	- Debugs a cone.
    /// </summary>
    /// <param name='position'>
    /// 	- The position for the tip of the cone.
    /// </param>
    /// <param name='angle'>
    /// 	- The angle of the cone.
    /// </param>
    /// <param name='color'>
    /// 	- The color of the cone.
    /// </param>
    /// <param name='duration'>
    /// 	- How long to draw the cone.
    /// </param>
    /// <param name='depthTest'>
    /// 	- Whether or not the cone should be faded when behind other objects.
    /// </param>
    public static void DebugCone(Vector3 position, Color color, float angle = 45, float duration = 0, bool depthTest = true)
    {
        if (!Draw)
            return;
        DebugCone(position, Vector3.up, color, angle, duration, depthTest);
    }

    /// <summary>
    /// 	- Debugs a cone.
    /// </summary>
    /// <param name='position'>
    /// 	- The position for the tip of the cone.
    /// </param>
    /// <param name='angle'>
    /// 	- The angle of the cone.
    /// </param>
    /// <param name='duration'>
    /// 	- How long to draw the cone.
    /// </param>
    /// <param name='depthTest'>
    /// 	- Whether or not the cone should be faded when behind other objects.
    /// </param>
    public static void DebugCone(Vector3 position, float angle = 45, float duration = 0, bool depthTest = true)
    {
        if (!Draw)
            return;
        DebugCone(position, Vector3.up, Color.white, angle, duration, depthTest);
    }
    
    /// <summary>
    /// 	- Debugs an line segment.
    /// </summary>
    /// <param name='bgn'>
    /// 	- The start position of the line.
    /// </param>
    /// <param name='end'>
    /// 	- The end position of the line.
    /// </param>
    /// <param name='color'>
    /// 	- The color of the line.
    /// </param>
    /// <param name='duration'>
    /// 	- How long to draw the line.
    /// </param>
    /// <param name='depthTest'>
    /// 	- Whether or not the line should be faded when behind other objects. 
    /// </param>
    public static void DebugLine(Vector3 bgn, Vector3 end, Color color, float duration = 0, bool depthTest = true)
    {
        if (!Draw)
            return;
        Debug.DrawLine(bgn, end, color, duration, depthTest);
    }
    /// <summary>
    /// 	- Debugs an arrow.
    /// </summary>
    /// <param name='position'>
    /// 	- The start position of the arrow.
    /// </param>
    /// <param name='direction'>
    /// 	- The direction the arrow will point in.
    /// </param>
    /// <param name='color'>
    /// 	- The color of the arrow.
    /// </param>
    /// <param name='duration'>
    /// 	- How long to draw the arrow.
    /// </param>
    /// <param name='depthTest'>
    /// 	- Whether or not the arrow should be faded when behind other objects. 
    /// </param>
    public static void DebugArrow(Vector3 position, Vector3 direction, Color color, float duration = 0, bool depthTest = true)
    {
        if (!Draw)
            return;
        Debug.DrawRay(position, direction, color, duration, depthTest);
        DebugExtension.DebugCone(position + direction, -direction * 0.333f, color, 15, duration, depthTest);
    }

    /// <summary>
    /// 	- Debugs an arrow.
    /// </summary>
    /// <param name='position'>
    /// 	- The start position of the arrow.
    /// </param>
    /// <param name='direction'>
    /// 	- The direction the arrow will point in.
    /// </param>
    /// <param name='duration'>
    /// 	- How long to draw the arrow.
    /// </param>
    /// <param name='depthTest'>
    /// 	- Whether or not the arrow should be faded when behind other objects. 
    /// </param>
    public static void DebugArrow(Vector3 position, Vector3 direction, float duration = 0, bool depthTest = true)
    {
        if (!Draw)
            return;
        DebugArrow(position, direction, Color.white, duration, depthTest);
    }

    /// <summary>
    /// 	- Debugs a capsule.
    /// </summary>
    /// <param name='start'>
    /// 	- The position of one end of the capsule.
    /// </param>
    /// <param name='end'>
    /// 	- The position of the other end of the capsule.
    /// </param>
    /// <param name='color'>
    /// 	- The color of the capsule.
    /// </param>
    /// <param name='radius'>
    /// 	- The radius of the capsule.
    /// </param>
    /// <param name='duration'>
    /// 	- How long to draw the capsule.
    /// </param>
    /// <param name='depthTest'>
    /// 	- Whether or not the capsule should be faded when behind other objects.
    /// </param>
    public static void DebugCapsule(Vector3 start, Vector3 end, Color color, float radius = 1, float duration = 0, bool depthTest = true)
    {
        if (!Draw)
            return;
        Vector3 up = (end - start).normalized * radius;
        Vector3 forward = Vector3.Slerp(up, -up, 0.5f);
        Vector3 right = Vector3.Cross(up, forward).normalized * radius;

        float height = (start - end).magnitude;
        float sideLength = Mathf.Max(0, (height * 0.5f) - radius);
        Vector3 middle = (end + start) * 0.5f;

        start = middle + ((start - middle).normalized * sideLength);
        end = middle + ((end - middle).normalized * sideLength);

        //Radial circles
        DebugExtension.DebugCircle(start, up, color, radius, duration, depthTest);
        DebugExtension.DebugCircle(end, -up, color, radius, duration, depthTest);

        //Side lines
        Debug.DrawLine(start + right, end + right, color, duration, depthTest);
        Debug.DrawLine(start - right, end - right, color, duration, depthTest);

        Debug.DrawLine(start + forward, end + forward, color, duration, depthTest);
        Debug.DrawLine(start - forward, end - forward, color, duration, depthTest);

        for (int i = 1; i < 26; i++)
        {

            //Start endcap
            Debug.DrawLine(Vector3.Slerp(right, -up, i / 25.0f) + start, Vector3.Slerp(right, -up, (i - 1) / 25.0f) + start, color, duration, depthTest);
            Debug.DrawLine(Vector3.Slerp(-right, -up, i / 25.0f) + start, Vector3.Slerp(-right, -up, (i - 1) / 25.0f) + start, color, duration, depthTest);
            Debug.DrawLine(Vector3.Slerp(forward, -up, i / 25.0f) + start, Vector3.Slerp(forward, -up, (i - 1) / 25.0f) + start, color, duration, depthTest);
            Debug.DrawLine(Vector3.Slerp(-forward, -up, i / 25.0f) + start, Vector3.Slerp(-forward, -up, (i - 1) / 25.0f) + start, color, duration, depthTest);

            //End endcap
            Debug.DrawLine(Vector3.Slerp(right, up, i / 25.0f) + end, Vector3.Slerp(right, up, (i - 1) / 25.0f) + end, color, duration, depthTest);
            Debug.DrawLine(Vector3.Slerp(-right, up, i / 25.0f) + end, Vector3.Slerp(-right, up, (i - 1) / 25.0f) + end, color, duration, depthTest);
            Debug.DrawLine(Vector3.Slerp(forward, up, i / 25.0f) + end, Vector3.Slerp(forward, up, (i - 1) / 25.0f) + end, color, duration, depthTest);
            Debug.DrawLine(Vector3.Slerp(-forward, up, i / 25.0f) + end, Vector3.Slerp(-forward, up, (i - 1) / 25.0f) + end, color, duration, depthTest);
        }
    }

    /// <summary>
    /// 	- Debugs a capsule.
    /// </summary>
    /// <param name='start'>
    /// 	- The position of one end of the capsule.
    /// </param>
    /// <param name='end'>
    /// 	- The position of the other end of the capsule.
    /// </param>
    /// <param name='radius'>
    /// 	- The radius of the capsule.
    /// </param>
    /// <param name='duration'>
    /// 	- How long to draw the capsule.
    /// </param>
    /// <param name='depthTest'>
    /// 	- Whether or not the capsule should be faded when behind other objects.
    /// </param>
    public static void DebugCapsule(Vector3 start, Vector3 end, float radius = 1, float duration = 0, bool depthTest = true)
    {
        if (!Draw)
            return;
        DebugCapsule(start, end, Color.white, radius, duration, depthTest);
    }

    public static void DebugCapsule(Vector3 start, Vector3 end, float radius, Color color, float duration = 0, bool depthTest = true)
    {
        if (!Draw)
            return;
        DebugCapsule(start, end, color, radius, duration, depthTest);
    }

    public static void DrawCollider(Collider collider, Vector3 position, Quaternion rotation, Color color, float duration = 5, bool depthTest = true)
    {
        DrawCollider(collider, position, rotation, Vector2.one, color, false, duration, depthTest);
    }

    public static void DrawCollider(Collider collider, Vector3 position, Quaternion rotation, Color color, bool positionIsCenter, float duration = 5, bool depthTest = true)
    {
        DrawCollider(collider, position, rotation, Vector2.one, color, positionIsCenter, duration, depthTest);
    }
    
    public static void DrawCollider(Collider collider, Vector3 position, Quaternion rotation, Vector3 scale, Color color, bool positionIsCenter, float duration = 5, bool depthTest = true)
    {
        if (!Draw) return;
            
        color = new Color(color.r, color.g, color.b, 0.5f);
        switch (collider)
        {
            case SphereCollider sphere:
            {
                Vector3 center = positionIsCenter ? position : position + rotation * Vector3.Scale(sphere.center, scale); 
                DebugWireSphere(center, color, sphere.radius * scale.MaxComponent(), duration, depthTest);
                break;
            }

            case CapsuleCollider capsule:
            {
                Vector3 center = positionIsCenter ? position : position + rotation * Vector3.Scale(capsule.center, scale); 
                var radius = Vector3.Scale(capsule.GetCapsuleSides() * capsule.radius, scale).MaxComponent();
                var offset = rotation * Vector3.Scale(capsule.height * 0.5f * capsule.GetCapsuleAxis(), scale);
                DebugCapsule(center - offset, center + offset, radius, color, duration, depthTest);
                break;
            }

            case CharacterController capsule:
            {
                Vector3 center = positionIsCenter ? position : position + rotation * Vector3.Scale(capsule.center, scale); 
                var radius = Vector3.Scale((Vector3.right + Vector3.forward) * capsule.radius, scale).MaxComponent();
                var offset = rotation * Vector3.Scale(capsule.height * 0.5f * Vector2.up, scale);
                DebugCapsule(center - offset, center + offset, radius, color, duration, depthTest);
                break;
            }

            case BoxCollider box:
            {
                Vector3 center = positionIsCenter ? position : position + rotation * Vector3.Scale(box.center, scale); 
                DebugBox(center, Vector3.Scale(box.size * 0.5f, scale), rotation, color, duration, depthTest);
                break;
            }

            default:
                DebugBounds(collider.bounds, color, duration, depthTest);
                break;
        }
    }

    private static Vector3 GetCapsuleAxis(this CapsuleCollider capsule)
    {
        switch (capsule.direction)
        {
            case 0: return Vector3.right;
            case 1: return Vector3.up;
            case 2: return Vector3.forward;
        }

        return Vector3.up;
    }
    
    private static Vector3 GetCapsuleSides(this CapsuleCollider capsule)
    {
        switch (capsule.direction)
        {
            case 0: return Vector3.up + Vector3.forward;
            case 1: return Vector3.right + Vector3.forward;
            case 2: return Vector3.up + Vector3.right;
        }

        return Vector3.zero;
    }

    private static float MaxComponent(this Vector3 v) => Mathf.Max(v.x, v.y, v.z);

    #endregion

    #region GizmoDrawFunctions

    /// <summary>
    /// 	- Draws a point.
    /// </summary>
    /// <param name='position'>
    /// 	- The point to draw.
    /// </param>
    ///  <param name='color'>
    /// 	- The color of the drawn point.
    /// </param>
    /// <param name='scale'>
    /// 	- The size of the drawn point.
    /// </param>
    public static void DrawPoint(Vector3 position, Color color, float scale = 1.0f)
    {
        if (!Draw)
            return;
        Color oldColor = Gizmos.color;

        Gizmos.color = color;
        Gizmos.DrawRay(position + (Vector3.up * (scale * 0.5f)), -Vector3.up * scale);
        Gizmos.DrawRay(position + (Vector3.right * (scale * 0.5f)), -Vector3.right * scale);
        Gizmos.DrawRay(position + (Vector3.forward * (scale * 0.5f)), -Vector3.forward * scale);

        Gizmos.color = oldColor;
    }

    /// <summary>
    /// 	- Draws a point.
    /// </summary>
    /// <param name='position'>
    /// 	- The point to draw.
    /// </param>
    /// <param name='scale'>
    /// 	- The size of the drawn point.
    /// </param>
    public static void DrawPoint(Vector3 position, float scale = 1.0f)
    {
        if (!Draw)
            return;
        DrawPoint(position, Color.white, scale);
    }

    /// <summary>
    /// 	- Draws an axis-aligned bounding box.
    /// </summary>
    /// <param name='bounds'>
    /// 	- The bounds to draw.
    /// </param>
    /// <param name='color'>
    /// 	- The color of the bounds.
    /// </param>
    public static void DrawBounds(Bounds bounds, Color color)
    {
        if (!Draw)
            return;
        Vector3 center = bounds.center;

        float x = bounds.extents.x;
        float y = bounds.extents.y;
        float z = bounds.extents.z;

        Vector3 ruf = center + new Vector3(x, y, z);
        Vector3 rub = center + new Vector3(x, y, -z);
        Vector3 luf = center + new Vector3(-x, y, z);
        Vector3 lub = center + new Vector3(-x, y, -z);

        Vector3 rdf = center + new Vector3(x, -y, z);
        Vector3 rdb = center + new Vector3(x, -y, -z);
        Vector3 lfd = center + new Vector3(-x, -y, z);
        Vector3 lbd = center + new Vector3(-x, -y, -z);

        Color oldColor = Gizmos.color;
        Gizmos.color = color;

        Gizmos.DrawLine(ruf, luf);
        Gizmos.DrawLine(ruf, rub);
        Gizmos.DrawLine(luf, lub);
        Gizmos.DrawLine(rub, lub);

        Gizmos.DrawLine(ruf, rdf);
        Gizmos.DrawLine(rub, rdb);
        Gizmos.DrawLine(luf, lfd);
        Gizmos.DrawLine(lub, lbd);

        Gizmos.DrawLine(rdf, lfd);
        Gizmos.DrawLine(rdf, rdb);
        Gizmos.DrawLine(lfd, lbd);
        Gizmos.DrawLine(lbd, rdb);

        Gizmos.color = oldColor;
    }

    /// <summary>
    /// 	- Draws an axis-aligned bounding box.
    /// </summary>
    /// <param name='bounds'>
    /// 	- The bounds to draw.
    /// </param>
    public static void DrawBounds(Bounds bounds)
    {
        if (!Draw)
            return;
        DrawBounds(bounds, Color.white);
    }

    /// <summary>
    /// 	- Draws a local cube.
    /// </summary>
    /// <param name='transform'>
    /// 	- The transform the cube will be local to.
    /// </param>
    /// <param name='size'>
    /// 	- The local size of the cube.
    /// </param>
    /// <param name='center'>
    ///		- The local position of the cube.
    /// </param>
    /// <param name='color'>
    /// 	- The color of the cube.
    /// </param>
    public static void DrawLocalCube(Transform transform, Vector3 size, Color color, Vector3 center = default(Vector3))
    {
        if (!Draw)
            return;
        Color oldColor = Gizmos.color;
        Gizmos.color = color;

        Vector3 lbb = transform.TransformPoint(center + ((-size) * 0.5f));
        Vector3 rbb = transform.TransformPoint(center + (new Vector3(size.x, -size.y, -size.z) * 0.5f));

        Vector3 lbf = transform.TransformPoint(center + (new Vector3(size.x, -size.y, size.z) * 0.5f));
        Vector3 rbf = transform.TransformPoint(center + (new Vector3(-size.x, -size.y, size.z) * 0.5f));

        Vector3 lub = transform.TransformPoint(center + (new Vector3(-size.x, size.y, -size.z) * 0.5f));
        Vector3 rub = transform.TransformPoint(center + (new Vector3(size.x, size.y, -size.z) * 0.5f));

        Vector3 luf = transform.TransformPoint(center + ((size) * 0.5f));
        Vector3 ruf = transform.TransformPoint(center + (new Vector3(-size.x, size.y, size.z) * 0.5f));

        Gizmos.DrawLine(lbb, rbb);
        Gizmos.DrawLine(rbb, lbf);
        Gizmos.DrawLine(lbf, rbf);
        Gizmos.DrawLine(rbf, lbb);

        Gizmos.DrawLine(lub, rub);
        Gizmos.DrawLine(rub, luf);
        Gizmos.DrawLine(luf, ruf);
        Gizmos.DrawLine(ruf, lub);

        Gizmos.DrawLine(lbb, lub);
        Gizmos.DrawLine(rbb, rub);
        Gizmos.DrawLine(lbf, luf);
        Gizmos.DrawLine(rbf, ruf);

        Gizmos.color = oldColor;
    }

    /// <summary>
    /// 	- Draws a local cube.
    /// </summary>
    /// <param name='transform'>
    /// 	- The transform the cube will be local to.
    /// </param>
    /// <param name='size'>
    /// 	- The local size of the cube.
    /// </param>
    /// <param name='center'>
    ///		- The local position of the cube.
    /// </param>	
    public static void DrawLocalCube(Transform transform, Vector3 size, Vector3 center = default(Vector3))
    {
        if (!Draw)
            return;
        DrawLocalCube(transform, size, Color.white, center);
    }

    /// <summary>
    /// 	- Draws a local cube.
    /// </summary>
    /// <param name='space'>
    /// 	- The space the cube will be local to.
    /// </param>
    /// <param name='size'>
    /// 	- The local size of the cube.
    /// </param>
    /// <param name='center'>
    /// 	- The local position of the cube.
    /// </param>
    /// <param name='color'>
    /// 	- The color of the cube.
    /// </param>
    public static void DrawLocalCube(Matrix4x4 space, Vector3 size, Color color, Vector3 center = default(Vector3))
    {
        if (!Draw)
            return;
        Color oldColor = Gizmos.color;
        Gizmos.color = color;

        Vector3 lbb = space.MultiplyPoint3x4(center + ((-size) * 0.5f));
        Vector3 rbb = space.MultiplyPoint3x4(center + (new Vector3(size.x, -size.y, -size.z) * 0.5f));

        Vector3 lbf = space.MultiplyPoint3x4(center + (new Vector3(size.x, -size.y, size.z) * 0.5f));
        Vector3 rbf = space.MultiplyPoint3x4(center + (new Vector3(-size.x, -size.y, size.z) * 0.5f));

        Vector3 lub = space.MultiplyPoint3x4(center + (new Vector3(-size.x, size.y, -size.z) * 0.5f));
        Vector3 rub = space.MultiplyPoint3x4(center + (new Vector3(size.x, size.y, -size.z) * 0.5f));

        Vector3 luf = space.MultiplyPoint3x4(center + ((size) * 0.5f));
        Vector3 ruf = space.MultiplyPoint3x4(center + (new Vector3(-size.x, size.y, size.z) * 0.5f));

        Gizmos.DrawLine(lbb, rbb);
        Gizmos.DrawLine(rbb, lbf);
        Gizmos.DrawLine(lbf, rbf);
        Gizmos.DrawLine(rbf, lbb);

        Gizmos.DrawLine(lub, rub);
        Gizmos.DrawLine(rub, luf);
        Gizmos.DrawLine(luf, ruf);
        Gizmos.DrawLine(ruf, lub);

        Gizmos.DrawLine(lbb, lub);
        Gizmos.DrawLine(rbb, rub);
        Gizmos.DrawLine(lbf, luf);
        Gizmos.DrawLine(rbf, ruf);

        Gizmos.color = oldColor;
    }

    /// <summary>
    /// 	- Draws a local cube.
    /// </summary>
    /// <param name='space'>
    /// 	- The space the cube will be local to.
    /// </param>
    /// <param name='size'>
    /// 	- The local size of the cube.
    /// </param>
    /// <param name='center'>
    /// 	- The local position of the cube.
    /// </param>
    public static void DrawLocalCube(Matrix4x4 space, Vector3 size, Vector3 center = default(Vector3))
    {
        if (!Draw)
            return;
        DrawLocalCube(space, size, Color.white, center);
    }

    /// <summary>
    /// 	- Draws a circle.
    /// </summary>
    /// <param name='position'>
    /// 	- Where the center of the circle will be positioned.
    /// </param>
    /// <param name='up'>
    /// 	- The direction perpendicular to the surface of the circle.
    /// </param>
    /// <param name='color'>
    /// 	- The color of the circle.
    /// </param>
    /// <param name='radius'>
    /// 	- The radius of the circle.
    /// </param>
    public static void DrawCircle(Vector3 position, Vector3 up, Color color, float radius = 1.0f)
    {
        if (!Draw)
            return;
        up = ((up == Vector3.zero) ? Vector3.up : up).normalized * radius;
        Vector3 _forward = Vector3.Slerp(up, -up, 0.5f);
        Vector3 _right = Vector3.Cross(up, _forward).normalized * radius;

        Matrix4x4 matrix = new Matrix4x4();

        matrix[0] = _right.x;
        matrix[1] = _right.y;
        matrix[2] = _right.z;

        matrix[4] = up.x;
        matrix[5] = up.y;
        matrix[6] = up.z;

        matrix[8] = _forward.x;
        matrix[9] = _forward.y;
        matrix[10] = _forward.z;

        Vector3 _lastPoint = position + matrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(0), 0, Mathf.Sin(0)));
        Vector3 _nextPoint = Vector3.zero;

        Color oldColor = Gizmos.color;
        Gizmos.color = (color == default(Color)) ? Color.white : color;

        for (var i = 0; i < 91; i++)
        {
            _nextPoint.x = Mathf.Cos((i * 4) * Mathf.Deg2Rad);
            _nextPoint.z = Mathf.Sin((i * 4) * Mathf.Deg2Rad);
            _nextPoint.y = 0;

            _nextPoint = position + matrix.MultiplyPoint3x4(_nextPoint);

            Gizmos.DrawLine(_lastPoint, _nextPoint);
            _lastPoint = _nextPoint;
        }

        Gizmos.color = oldColor;
    }

    /// <summary>
    /// 	- Draws a circle.
    /// </summary>
    /// <param name='position'>
    /// 	- Where the center of the circle will be positioned.
    /// </param>
    /// <param name='color'>
    /// 	- The color of the circle.
    /// </param>
    /// <param name='radius'>
    /// 	- The radius of the circle.
    /// </param>
    public static void DrawCircle(Vector3 position, Color color, float radius = 1.0f)
    {
        if (!Draw)
            return;
        DrawCircle(position, Vector3.up, color, radius);
    }

    /// <summary>
    /// 	- Draws a circle.
    /// </summary>
    /// <param name='position'>
    /// 	- Where the center of the circle will be positioned.
    /// </param>
    /// <param name='up'>
    /// 	- The direction perpendicular to the surface of the circle.
    /// </param>
    /// <param name='radius'>
    /// 	- The radius of the circle.
    /// </param>
    public static void DrawCircle(Vector3 position, Vector3 up, float radius = 1.0f)
    {
        if (!Draw)
            return;
        DrawCircle(position, position, Color.white, radius);
    }

    /// <summary>
    /// 	- Draws a circle.
    /// </summary>
    /// <param name='position'>
    /// 	- Where the center of the circle will be positioned.
    /// </param>
    /// <param name='radius'>
    /// 	- The radius of the circle.
    /// </param>
    public static void DrawCircle(Vector3 position, float radius = 1.0f)
    {
        if (!Draw)
            return;
        DrawCircle(position, Vector3.up, Color.white, radius);
    }

    //Wiresphere already exists

    /// <summary>
    /// 	- Draws a cylinder.
    /// </summary>
    /// <param name='start'>
    /// 	- The position of one end of the cylinder.
    /// </param>
    /// <param name='end'>
    /// 	- The position of the other end of the cylinder.
    /// </param>
    /// <param name='color'>
    /// 	- The color of the cylinder.
    /// </param>
    /// <param name='radius'>
    /// 	- The radius of the cylinder.
    /// </param>
    public static void DrawCylinder(Vector3 start, Vector3 end, Color color, float radius = 1.0f)
    {

        if (!Draw)
            return;
        Vector3 up = (end - start).normalized * radius;
        Vector3 forward = Vector3.Slerp(up, -up, 0.5f);
        Vector3 right = Vector3.Cross(up, forward).normalized * radius;

        //Radial circles
        DebugExtension.DrawCircle(start, up, color, radius);
        DebugExtension.DrawCircle(end, -up, color, radius);
        DebugExtension.DrawCircle((start + end) * 0.5f, up, color, radius);

        Color oldColor = Gizmos.color;
        Gizmos.color = color;

        //Side lines
        Gizmos.DrawLine(start + right, end + right);
        Gizmos.DrawLine(start - right, end - right);

        Gizmos.DrawLine(start + forward, end + forward);
        Gizmos.DrawLine(start - forward, end - forward);

        //Start endcap
        Gizmos.DrawLine(start - right, start + right);
        Gizmos.DrawLine(start - forward, start + forward);

        //End endcap
        Gizmos.DrawLine(end - right, end + right);
        Gizmos.DrawLine(end - forward, end + forward);

        Gizmos.color = oldColor;
    }

    /// <summary>
    /// 	- Draws a cylinder.
    /// </summary>
    /// <param name='start'>
    /// 	- The position of one end of the cylinder.
    /// </param>
    /// <param name='end'>
    /// 	- The position of the other end of the cylinder.
    /// </param>
    /// <param name='radius'>
    /// 	- The radius of the cylinder.
    /// </param>
    public static void DrawCylinder(Vector3 start, Vector3 end, float radius = 1.0f)
    {
        if (!Draw)
            return;
        DrawCylinder(start, end, Color.white, radius);
    }

    /// <summary>
    /// 	- Draws a cone.
    /// </summary>
    /// <param name='position'>
    /// 	- The position for the tip of the cone.
    /// </param>
    /// <param name='direction'>
    /// 	- The direction for the cone to get wider in.
    /// </param>
    /// <param name='color'>
    /// 	- The color of the cone.
    /// </param>
    /// <param name='angle'>
    /// 	- The angle of the cone.
    /// </param>
    public static void DrawCone(Vector3 position, Vector3 direction, Color color, float angle = 45)
    {
        if (!Draw)
            return;
        float length = direction.magnitude;

        Vector3 _forward = direction;
        Vector3 _up = Vector3.Slerp(_forward, -_forward, 0.5f);
        Vector3 _right = Vector3.Cross(_forward, _up).normalized * length;

        direction = direction.normalized;

        Vector3 slerpedVector = Vector3.Slerp(_forward, _up, angle / 90.0f);

        float dist;
        var farPlane = new Plane(-direction, position + _forward);
        var distRay = new Ray(position, slerpedVector);

        farPlane.Raycast(distRay, out dist);

        Color oldColor = Gizmos.color;
        Gizmos.color = color;

        Gizmos.DrawRay(position, slerpedVector.normalized * dist);
        Gizmos.DrawRay(position, Vector3.Slerp(_forward, -_up, angle / 90.0f).normalized * dist);
        Gizmos.DrawRay(position, Vector3.Slerp(_forward, _right, angle / 90.0f).normalized * dist);
        Gizmos.DrawRay(position, Vector3.Slerp(_forward, -_right, angle / 90.0f).normalized * dist);

        DebugExtension.DrawCircle(position + _forward, direction, color, (_forward - (slerpedVector.normalized * dist)).magnitude);
        DebugExtension.DrawCircle(position + (_forward * 0.5f), direction, color, ((_forward * 0.5f) - (slerpedVector.normalized * (dist * 0.5f))).magnitude);

        Gizmos.color = oldColor;
    }

    /// <summary>
    /// 	- Draws a cone.
    /// </summary>
    /// <param name='position'>
    /// 	- The position for the tip of the cone.
    /// </param>
    /// <param name='direction'>
    /// 	- The direction for the cone to get wider in.
    /// </param>
    /// <param name='angle'>
    /// 	- The angle of the cone.
    /// </param>
    public static void DrawCone(Vector3 position, Vector3 direction, float angle = 45)
    {
        if (!Draw)
            return;
        DrawCone(position, direction, Color.white, angle);
    }

    /// <summary>
    /// 	- Draws a cone.
    /// </summary>
    /// <param name='position'>
    /// 	- The position for the tip of the cone.
    /// </param>
    /// <param name='color'>
    /// 	- The color of the cone.
    /// </param>
    /// <param name='angle'>
    /// 	- The angle of the cone.
    /// </param>
    public static void DrawCone(Vector3 position, Color color, float angle = 45)
    {
        if (!Draw)
            return;
        DrawCone(position, Vector3.up, color, angle);
    }

    /// <summary>
    /// 	- Draws a cone.
    /// </summary>
    /// <param name='position'>
    /// 	- The position for the tip of the cone.
    /// </param>
    /// <param name='angle'>
    /// 	- The angle of the cone.
    /// </param>
    public static void DrawCone(Vector3 position, float angle = 45)
    {
        if (!Draw)
            return;
        DrawCone(position, Vector3.up, Color.white, angle);
    }

    /// <summary>
    /// 	- Draws an arrow.
    /// </summary>
    /// <param name='position'>
    /// 	- The start position of the arrow.
    /// </param>
    /// <param name='direction'>
    /// 	- The direction the arrow will point in.
    /// </param>
    /// <param name='color'>
    /// 	- The color of the arrow.
    /// </param>
    public static void DrawArrow(Vector3 position, Vector3 direction, Color color)
    {
        if (!Draw)
            return;
        Color oldColor = Gizmos.color;
        Gizmos.color = color;

        Gizmos.DrawRay(position, direction);
        DebugExtension.DrawCone(position + direction, -direction * 0.333f, color, 15);

        Gizmos.color = oldColor;
    }

    /// <summary>
    /// 	- Draws an arrow.
    /// </summary>
    /// <param name='position'>
    /// 	- The start position of the arrow.
    /// </param>
    /// <param name='direction'>
    /// 	- The direction the arrow will point in.
    /// </param>
    public static void DrawArrow(Vector3 position, Vector3 direction)
    {
        if (!Draw)
            return;
        DrawArrow(position, direction, Color.white);
    }

    /// <summary>
    /// 	- Draws a capsule.
    /// </summary>
    /// <param name='start'>
    /// 	- The position of one end of the capsule.
    /// </param>
    /// <param name='end'>
    /// 	- The position of the other end of the capsule.
    /// </param>
    /// <param name='color'>
    /// 	- The color of the capsule.
    /// </param>
    /// <param name='radius'>
    /// 	- The radius of the capsule.
    /// </param>
    public static void DrawCapsule(Vector3 start, Vector3 end, Color color, float radius = 1)
    {
        if (!Draw)
            return;
        Vector3 up = (end - start).normalized * radius;
        Vector3 forward = Vector3.Slerp(up, -up, 0.5f);
        Vector3 right = Vector3.Cross(up, forward).normalized * radius;

        Color oldColor = Gizmos.color;
        Gizmos.color = color;

        float height = (start - end).magnitude;
        float sideLength = Mathf.Max(0, (height * 0.5f) - radius);
        Vector3 middle = (end + start) * 0.5f;

        start = middle + ((start - middle).normalized * sideLength);
        end = middle + ((end - middle).normalized * sideLength);

        //Radial circles
        DebugExtension.DrawCircle(start, up, color, radius);
        DebugExtension.DrawCircle(end, -up, color, radius);

        //Side lines
        Gizmos.DrawLine(start + right, end + right);
        Gizmos.DrawLine(start - right, end - right);

        Gizmos.DrawLine(start + forward, end + forward);
        Gizmos.DrawLine(start - forward, end - forward);

        for (int i = 1; i < 26; i++)
        {

            //Start endcap
            Gizmos.DrawLine(Vector3.Slerp(right, -up, i / 25.0f) + start, Vector3.Slerp(right, -up, (i - 1) / 25.0f) + start);
            Gizmos.DrawLine(Vector3.Slerp(-right, -up, i / 25.0f) + start, Vector3.Slerp(-right, -up, (i - 1) / 25.0f) + start);
            Gizmos.DrawLine(Vector3.Slerp(forward, -up, i / 25.0f) + start, Vector3.Slerp(forward, -up, (i - 1) / 25.0f) + start);
            Gizmos.DrawLine(Vector3.Slerp(-forward, -up, i / 25.0f) + start, Vector3.Slerp(-forward, -up, (i - 1) / 25.0f) + start);

            //End endcap
            Gizmos.DrawLine(Vector3.Slerp(right, up, i / 25.0f) + end, Vector3.Slerp(right, up, (i - 1) / 25.0f) + end);
            Gizmos.DrawLine(Vector3.Slerp(-right, up, i / 25.0f) + end, Vector3.Slerp(-right, up, (i - 1) / 25.0f) + end);
            Gizmos.DrawLine(Vector3.Slerp(forward, up, i / 25.0f) + end, Vector3.Slerp(forward, up, (i - 1) / 25.0f) + end);
            Gizmos.DrawLine(Vector3.Slerp(-forward, up, i / 25.0f) + end, Vector3.Slerp(-forward, up, (i - 1) / 25.0f) + end);
        }

        Gizmos.color = oldColor;
    }

    /// <summary>
    /// 	- Draws a capsule.
    /// </summary>
    /// <param name='start'>
    /// 	- The position of one end of the capsule.
    /// </param>
    /// <param name='end'>
    /// 	- The position of the other end of the capsule.
    /// </param>
    /// <param name='radius'>
    /// 	- The radius of the capsule.
    /// </param>
    public static void DrawCapsule(Vector3 start, Vector3 end, float radius = 1)
    {
        if (!Draw)
            return;
        DrawCapsule(start, end, Color.white, radius);
    }

    #endregion

    #region DebugFunctions

    /// <summary>
    /// 	- Gets the methods of an object.
    /// </summary>
    /// <returns>
    /// 	- A list of methods accessible from this object.
    /// </returns>
    /// <param name='obj'>
    /// 	- The object to get the methods of.
    /// </param>
    /// <param name='includeInfo'>
    /// 	- Whether or not to include each method's method info in the list.
    /// </param>
    public static string MethodsOfObject(System.Object obj, bool includeInfo = false)
    {
        string methods = "";
        MethodInfo[] methodInfos = obj.GetType().GetMethodsSafe(BindingFlags.Default);
        for (int i = 0; i < methodInfos.Length; i++)
        {
            if (includeInfo)
            {
                methods += methodInfos[i] + "\n";
            }

            else
            {
                methods += methodInfos[i].Name + "\n";
            }
        }

        return (methods);
    }

    /// <summary>
    /// 	- Gets the methods of a type.
    /// </summary>
    /// <returns>
    /// 	- A list of methods accessible from this type.
    /// </returns>
    /// <param name='type'>
    /// 	- The type to get the methods of.
    /// </param>
    /// <param name='includeInfo'>
    /// 	- Whether or not to include each method's method info in the list.
    /// </param>
    public static string MethodsOfType(System.Type type, bool includeInfo = false)
    {
        string methods = "";
        MethodInfo[] methodInfos = type.GetMethodsSafe(BindingFlags.Default);
        for (var i = 0; i < methodInfos.Length; i++)
        {
            if (includeInfo)
            {
                methods += methodInfos[i] + "\n";
            }

            else
            {
                methods += methodInfos[i].Name + "\n";
            }
        }

        return (methods);
    }

    #endregion
}


public class UnityDrawImpl
{
    public Material material;
    public float _scale = 5f;
    public Vector2 _offset = Vector2.zero;
    public bool Draw = false;
    public bool DrawUI = true;
    public void Circle(CircleShapeHandle handle)
    {
        if (!Draw)
            return;
        GL.PushMatrix();
        GL.LoadIdentity();
        var size = new Vector2(handle.Radius * 2, handle.Radius * 2);
        FormModelView(handle.Position, 0, size, new Vector2(0, 0));
        material.SetPass(0);
        var color = handle.OutlineColor;
        DrawRect(Vector2.zero, new Vector2(size.x, handle.OutlineThickness), color);
        DrawRect(Vector2.zero + new Vector2(0, size.y), new Vector2(size.x, handle.OutlineThickness), color);
        DrawRect(Vector2.zero, new Vector2(handle.OutlineThickness, size.y), color);
        DrawRect(Vector2.zero + new Vector2(size.x, 0), new Vector2(handle.OutlineThickness, size.y), color);
        GL.PopMatrix();
    }
    public void Rect(RectShapeHandle handle)
    {
        if (!Draw)
            return;
        GL.PushMatrix();
        GL.LoadIdentity();
        FormModelView(handle.Position, handle.Rotation, handle.Size, handle.Pivot);
        material.SetPass(0);
        var size = handle.Size;
        var color = handle.OutlineColor;
        DrawRect(Vector2.zero, new Vector2(size.x, handle.OutlineThickness), color);
        DrawRect(Vector2.zero + new Vector2(0, size.y), new Vector2(size.x, handle.OutlineThickness), color);
        DrawRect(Vector2.zero, new Vector2(handle.OutlineThickness, size.y), color);
        DrawRect(Vector2.zero + new Vector2(size.x, 0), new Vector2(handle.OutlineThickness, size.y), color);
        GL.PopMatrix();
    }

    private void FormModelView(Vector2 position, float rotation, Vector2 size, Vector2 pivot)
    {
        GL.modelview = GL.modelview
            * Matrix4x4.Translate(new Vector3(_offset.x, _offset.y / 2, 0))
            * Matrix4x4.Translate(new Vector3(Screen.width / 2, Screen.height / 2, 0))
            * Matrix4x4.Scale(new Vector3(_scale, -_scale))
            * Matrix4x4.Translate(new Vector3(position.x, position.y, 0))
            * Matrix4x4.Rotate(Quaternion.Euler(0, 0, rotation))
            * Matrix4x4.Translate(new Vector3(-size.x * pivot.x, -size.y * pivot.y, 0)); material.SetPass(0);
    }
    private void DrawRect(Vector2 position, Vector2 size, Color color)
    {
        if (!Draw)
            return;
        GL.invertCulling = true;
        //material.color = color;
        GL.Begin(GL.QUADS);
        GL.Color(color);
        GL.Vertex3(position.x + size.x, position.y, 0);
        GL.Vertex3(position.x + size.x, position.y + size.y, 0);
        GL.Vertex3(position.x, position.y + size.y, 0);
        GL.Vertex3(position.x, position.y, 0);
        GL.End();
        GL.invertCulling = false;
    }

    public void Sprite(SpriteHandle handle)
    {
        if (!Draw)
            return;
        GL.PushMatrix();
        GL.LoadIdentity();
        FormModelView(handle.Position, handle.Rotation, handle.Size, handle.Pivot);
        var position = handle.Position;
        var size = handle.Size;
        var color = handle.Color;
        //var fillColor = (Color)new Color32(handle.FillColor.R, handle.FillColor.G, handle.FillColor.B, handle.FillColor.A);
        // Optimization hint: 
        // Consider Graphics.DrawMeshNow
        //DrawRect(position, size, fillColor);
        DrawRect(Vector2.zero, new Vector2(size.x, 1), color);
        DrawRect(Vector2.zero + new Vector2(0, size.y), new Vector2(size.x, 1), color);
        DrawRect(Vector2.zero, new Vector2(1, size.y), color);
        DrawRect(Vector2.zero + new Vector2(size.x, 0), new Vector2(1, size.y), color);
        GL.PopMatrix();
    }

    public void SpriteGUI(SpriteHandle handle)
    {
        if (!DrawUI)
            return;
        GL.PushMatrix();
        GL.LoadIdentity();
        var position = handle.Position;
        var size = handle.Size;
        var color = (Color)handle.Color;
        //var fillColor = (Color)new Color32(handle.FillColor.R, handle.FillColor.G, handle.FillColor.B, handle.FillColor.A);
        // Optimization hint: 
        // Consider Graphics.DrawMeshNow
        //DrawRect(position, size, fillColor);
        DrawRect(position, new Vector2(size.x, 1), color);
        DrawRect(position + new Vector2(0, size.y), new Vector2(size.y, 1), color);
        DrawRect(position, new Vector2(1, size.y), color);
        DrawRect(position + new Vector2(size.x, 0), new Vector2(1, size.y), color);
        GL.PopMatrix();
    }

    public void Text(TextHandle sprite)
    {
        if (!DrawUI)
            return;
        GL.modelview = GL.modelview
            * Matrix4x4.Translate(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        GUI.Label(new UnityEngine.Rect(sprite.Position.x, sprite.Position.y, 200, 200), new GUIContent() { text = sprite.Text });
    }
}


public struct SpriteHandle
{
    public Vector2 Position { get; set; }
    public Vector2 Pivot { get; set; }
    public float Rotation { get; set; }
    public Vector2 Origin { get; set; }
    public Vector2 Scale { get; set; }
    public Vector2 TextureRect { get; set; }
    public Vector2 Size => new Vector2(TextureRect.x * Scale.x, TextureRect.y * Scale.y);
    public Texture Texture { get; set; }
    public Color Color { get; set; }
}


public struct CircleShapeHandle
{
    public float Radius { get; set; }
    public Vector2 Position { get; set; }
    public Color FillColor { get; set; }
    public float OutlineThickness { get; set; }
    public Color OutlineColor { get; set; }
}
public struct RectShapeHandle
{
    public Vector2 Size { get; set; }
    public Vector2 Position { get; set; }
    public float Rotation { get; set; }
    public Color FillColor { get; set; }
    public float OutlineThickness { get; set; }
    public Color OutlineColor { get; set; }
    public Vector2 Pivot { get; internal set; }
}
public struct TextHandle
{
    public Vector2 Position { get; set; }
    public string Text { get; set; }
}