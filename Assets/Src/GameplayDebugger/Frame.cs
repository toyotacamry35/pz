using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayDebugger
{
    public class Frame
    {
        public long StartUtcTick { get; set; }
        public long EndUtcTick { get; set; }
        public Event[] Events { get; set; } = Array.Empty<Event>();
        public ObjectSnapshot[] ObjectSnapshots { get; set; } = Array.Empty<ObjectSnapshot>();

    }

    public class Event
    {
        public long EventUtcTick;
    }

    public class ObjectSnapshot
    {
        public Type PosterType;
        public ObjectId ObjectId;
        public Action DrawSelf;
        public Action GizmoSelf;
    }

    public abstract class ObjectId
    {
        public abstract object Get();
        public abstract bool Is(object obj);
        public string Name;
    }
}
