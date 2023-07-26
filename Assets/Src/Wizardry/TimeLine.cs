using Assets.Src.Lib.ProfileTools;
using UnityEngine;

namespace Assets.Src.Wizardry
{
    public static class GUIExtensions
    {
        static Material _material;
        public static void DrawRectangle(Rect position, Color color)
        {
            if (Event.current == null || Event.current.type != EventType.Repaint)
                return;
            // Please assign a material that is using position and color.
            if (_material == null)
                _material = UnityEngine.Object.Instantiate(Profile.Load<Material>("GUISolidMaterial"));

            _material.color = color;

            _material.SetPass(0);

            // Optimization hint: 
            // Consider Graphics.DrawMeshNow
            GL.Color(color);
            GL.Begin(GL.QUADS);
            GL.Vertex3(position.x, position.y, 0);
            GL.Vertex3(position.x + position.width, position.y, 0);
            GL.Vertex3(position.x + position.width, position.y + position.height, 0);
            GL.Vertex3(position.x, position.y + position.height, 0);
            GL.End();
        }
        public static void DrawLine(Vector3 start, Vector3 end, Color color)
        {
            if (Event.current == null || Event.current.type != EventType.Repaint)
                return;
            // Please assign a material that is using position and color.
            if (_material == null)
                _material = UnityEngine.Object.Instantiate(Profile.Load<Material>("GUISolidMaterial"));

            _material.color = color;

            _material.SetPass(0);

            // Optimization hint: 
            // Consider Graphics.DrawMeshNow
            GL.Color(color);
            GL.Begin(GL.LINES);
            GL.Vertex3(start.x, start.y, 0);
            GL.Vertex3(end.x , end.y, 0);
            GL.End();
        }
    }

}
