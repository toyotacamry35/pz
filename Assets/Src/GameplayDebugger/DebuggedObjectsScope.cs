using System;
using System.Collections.Generic;
using System.Linq;

namespace GameplayDebugger
{
    public class DebuggedObjectsScope
    {
        public static DebuggedObjectsScope Instance = new DebuggedObjectsScope();
        HashSet<object> _objects = new HashSet<object>();
        public event Action<object> ObjectAdded;
        public event Action<object> ObjectRemoved;
        public bool Contains(object obj)
        {
            lock (this)
                return _objects.Contains(obj);
        }
        public void IfContains(object obj, Action<FrameRecorder> act)
        {
            lock (this)
                if (_objects.Contains(obj))
                    act(FramesLog.Instance.GetFrameRecorder());
        }
        public void AddObjectToScope(object obj)
        {
            lock (this)
                _objects.Add(obj);
            ObjectAdded?.Invoke(obj);
        }

        public void RemoveObjectFromScope(object obj)
        {
            lock (this)
                _objects.Remove(obj);
            ObjectRemoved?.Invoke(obj);
        }
        public List<object> GetAllObjects()
        {
            lock (this)
                return _objects.ToList();
        }
    }

}
