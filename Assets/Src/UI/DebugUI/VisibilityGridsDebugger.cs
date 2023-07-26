using Assets.Src.Lib.ProfileTools;
using Assets.Src.Wizardry;
using GeneratedCode.Repositories;
using SharedCode.Entities;
using SharedCode.MovementSync;
using SharedCode.Refs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Src.DebugUI
{
    public static class VisibilityGridsDebugger
    {
        static UnityDrawImpl _drawer = new UnityDrawImpl();

        public static void UpdateInputs()
        {
            if (!_drawer.Draw)
                return;
            if (UnityEngine.Input.mouseScrollDelta.y > 0)
                _drawer._scale += 0.1f;
            else if (UnityEngine.Input.mouseScrollDelta.y < 0)
                _drawer._scale -= 0.1f;
            _drawer._scale = Mathf.Clamp(_drawer._scale, 0.1f, 10);
            int inputs = 0;
            var offsetSpeed = 10f * _drawer._scale;
            if (UnityEngine.Input.GetKey(KeyCode.UpArrow))
            {
                inputs++;
                _drawer._offset += new Vector2(0, offsetSpeed);
            }

            if (UnityEngine.Input.GetKey(KeyCode.DownArrow))
            {
                inputs++;
                _drawer._offset += new Vector2(0, -offsetSpeed);
            }

            if (UnityEngine.Input.GetKey(KeyCode.LeftArrow))
            {
                inputs++;
                _drawer._offset += new Vector2(offsetSpeed, 0);
            }

            if (UnityEngine.Input.GetKey(KeyCode.RightArrow))
            {
                inputs++;
                _drawer._offset += new Vector2(-offsetSpeed, 0);
            }
            if (inputs == 4)
                _drawer._offset = Vector2.zero;
        }
        static bool _drawServerRepo = true;
        static bool _drawClientRepo = true;
        static bool _drawUnityServerRepo = true;
        internal static void DrawVisibilityGrids()
        {
            if (_drawer.material == null)
            {
                _drawer.material = UnityEngine.Object.Instantiate(Profile.Load<Material>("VertexColorMaterial"));
            }
            if (UnityEngine.Event.current.type != UnityEngine.EventType.Repaint)
            {
                if (UnityEngine.Event.current.type == EventType.KeyDown && UnityEngine.Event.current.keyCode == KeyCode.V && UnityEngine.Event.current.control)
                    _drawer.Draw = !_drawer.Draw;
                
                if (UnityEngine.Event.current.type == EventType.KeyDown && UnityEngine.Event.current.keyCode == KeyCode.S && UnityEngine.Event.current.control)
                    _drawServerRepo = !_drawServerRepo;
                if (UnityEngine.Event.current.type == EventType.KeyDown && UnityEngine.Event.current.keyCode == KeyCode.C && UnityEngine.Event.current.control)
                    _drawClientRepo = !_drawClientRepo;
                if (UnityEngine.Event.current.type == EventType.KeyDown && UnityEngine.Event.current.keyCode == KeyCode.U && UnityEngine.Event.current.control)
                    _drawUnityServerRepo = !_drawUnityServerRepo;
            }
            if (_drawer.Draw)
            {
                foreach (var grid in VisibilityGrid._grids)
                {
                    if (grid.Key.Item2.CloudNodeType == SharedCode.Cloud.CloudNodeType.Client && !_drawClientRepo)
                        continue;
                    if (grid.Key.Item2.CloudNodeType == SharedCode.Cloud.CloudNodeType.Server && !_drawServerRepo)
                        continue;
                    foreach (var cell in grid.Value._samplingSpatialHash._cells)
                    {
                        foreach (var ent in cell.Value.CellsDictionary)
                        {
                            _drawer.Rect(new RectShapeHandle()
                            {
                                Position = new UnityEngine.Vector2(ent.Value.Pos.x, ent.Value.Pos.z),
                                FillColor = Color.clear,
                                OutlineColor = grid.Key.Item2.CloudNodeType == SharedCode.Cloud.CloudNodeType.Client ? Color.green : Color.red,
                                OutlineThickness = 1,
                                Size = new Vector2(5, 5),
                            });

                            //_drawer.Text(new TextHandle()
                            //{
                            //    Position = new UnityEngine.Vector2(ent.Value.Pos.x, ent.Value.Pos.z),
                            //    Text = $"{ent.Value.Pos} {ent.Value.Def?.GetType().Name}"
                            //});
                        }
                    }
                    foreach (var entRef in ((EntitiesRepository)grid.Key.Item2).GetAllEntitiesDebug())
                    {
                        var ent = ((IEntityRefExt)entRef).GetEntity();
                        var positioned = PositionedObjectHelper.GetPositioned(ent);
                        if (positioned != null)
                        {
                            _drawer.Rect(new RectShapeHandle()
                            {
                                Position = new UnityEngine.Vector2(positioned.Position.x, positioned.Position.z),
                                FillColor = Color.clear,
                                OutlineColor = grid.Key.Item2.CloudNodeType == SharedCode.Cloud.CloudNodeType.Client ? Color.yellow : Color.magenta,
                                OutlineThickness = 1,
                                Size = new Vector2(3, 3),
                            });
                        }
                    }
                }
            }
        }
    }
}
