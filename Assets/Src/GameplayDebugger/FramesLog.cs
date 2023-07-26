using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace GameplayDebugger
{
    public class FramesLog
    {
        public static FramesLog Instance = new FramesLog();
        List<Frame> _frames = new List<Frame>();
        Frame _currentRecordedFrame;
        FrameRecorder _recorder = new FrameRecorder();
        public FrameRecorder GetFrameRecorder()
        {
            if (_currentRecordedFrame == null)
            {
                lock (this)
                    _currentRecordedFrame = new Frame() { StartUtcTick = DateTime.UtcNow.Ticks };
            }
            return _recorder;
        }

        public int RecordedFramesCount = 1000;
        public void AddFrames(IEnumerable<Frame> frames)
        {
            lock (this)
                _frames.AddRange(frames);
        }

        public Frame[] GetFrames()
        {
            lock (this)
                return _frames.ToArray();
        }
        public Frame GetLastRecordedFrame()
        {
            lock (this)
            {
                if (_frames.Count == 0)
                    return null;
                return _frames.Last();
            }
        }
        public void FinishFrame()
        {
            lock (this)
            {
                var newFrame = new Frame() { StartUtcTick = DateTime.UtcNow.Ticks };
                GetFrameRecorder().FixateFrame(_currentRecordedFrame);
                _currentRecordedFrame.EndUtcTick = DateTime.UtcNow.Ticks;
                _frames.Add(_currentRecordedFrame);
                _currentRecordedFrame = newFrame;
                if (_frames.Count > RecordedFramesCount)
                    _frames.RemoveAt(0);
            }
        }


    }

    public class FrameRecorder
    {
        ConcurrentQueue<Event> _events = new ConcurrentQueue<Event>();
        ConcurrentQueue<ObjectSnapshot> _snapshots = new ConcurrentQueue<ObjectSnapshot>();
        public void NotifyOfEvent(Event e)
        {
            _events.Enqueue(e);
        }
        public void StoreSnapshot(ObjectSnapshot snapshot)
        {
            _snapshots.Enqueue(snapshot);
        }
        public void FixateFrame(Frame currentFrame)
        {
            var eventsCount = _events.Count;
            var snapshotsCount = _snapshots.Count;
            currentFrame.Events = new Event[eventsCount];
            currentFrame.ObjectSnapshots = new ObjectSnapshot[snapshotsCount];

            for (int i = 0; i < eventsCount; i++)
            {
                Event e;
                _events.TryDequeue(out e);
                currentFrame.Events[i] = e;
            }
            for (int i = 0; i < snapshotsCount; i++)
            {
                ObjectSnapshot s;
                _snapshots.TryDequeue(out s);
                currentFrame.ObjectSnapshots[i] = s;
            }
        }
    }
}

