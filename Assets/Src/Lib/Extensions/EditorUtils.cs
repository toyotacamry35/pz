using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DG.Tweening.Plugins;
using UnityEngine;

namespace Assets.Src.Lib.Extensions
{
    public class EditorUtils
    {
        public static void DrawPoint(Vector3 p, Color color, float hatchLength = 0.05f, float duration = 0f)
        {
            var d = hatchLength / 2f;
            var p0 = new Vector3(p.x - d, p.y - d, p.z - d);
            var p1 = new Vector3(p.x + d, p.y + d, p.z + d);
            Debug.DrawLine(p0, p1, color, duration);
        }
    }
}
