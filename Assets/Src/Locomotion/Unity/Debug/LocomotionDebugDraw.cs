using System;
using System.Collections.Generic;
using ColonyShared.SharedCode;
using UnityEngine;
using static Src.Locomotion.DebugTag;

namespace Src.Locomotion.Unity
{
    public class LocomotionDebugDraw : ILocomotionDebugAgent
    {
        private readonly float _duration;
        private readonly bool _depthTest;
        private readonly List<Entry> _buffer = new List<Entry>();
        private Color _color = Color.white;
        private Vector3 _size = Vector3.one;

        public LocomotionDebugDraw(float duration = 5, bool depthTest = false)
        {
            _duration = duration;
            _depthTest = depthTest;
        }

        public bool IsActive => false;

        public void Add(DebugTag id, Value entry)
        {
            switch (id)
            {
                case DrawColor:
                    _color = entry.Color;
                    break;
                case DrawSize:
                    _size = entry.Vector3.ToUnity();
                    break;
                case DrawPoint:
                    _buffer.Add(new Entry{ Instr = Instr.DrawPoint, Origin = entry.Vector3.ToUnity(), Color = _color, Size = _size});
                    break;
                case DrawSphere:
                    _buffer.Add(new Entry{ Instr = Instr.DrawSphere, Origin = entry.Vector3.ToUnity(), Color = _color, Size = _size});
                    break;
                case DrawBox:
                    _buffer.Add(new Entry{ Instr = Instr.DrawBox, Origin = entry.Vector3.ToUnity(), Color = _color, Size = _size});
                    break;
            }
        }

        public void BeginOfFrame()
        {}

        public void EndOfFrame()
        {
            foreach (var entry in _buffer)
                DrawEntry(entry);
            _buffer.Clear();
            _color = Color.white;
            _size = Vector3.one;
        }

        private void DrawEntry(Entry entry)
        {
            switch (entry.Instr)
            {
                case Instr.DrawPoint:
                    DebugExtension.DebugPoint(entry.Origin, entry.Color, entry.Size.x, _duration, _depthTest);                    
                    break;
                case Instr.DrawSphere:
                    DebugExtension.DebugWireSphere(entry.Origin, entry.Color, entry.Size.x, _duration, _depthTest);                    
                    break;
                case Instr.DrawBox:
                    DebugExtension.DebugBox(entry.Origin, entry.Size, Quaternion.identity, entry.Color, _duration, _depthTest);                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private enum Instr { DrawPoint, DrawSphere, DrawBox }

        private struct Entry
        {
            public Instr Instr;
            public Color Color;
            public Vector3 Origin;
            public Vector3 Size;
        }
    }
}