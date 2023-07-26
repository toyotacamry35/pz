using Assets.ColonyShared.SharedCode.Scribe;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
namespace SharedCode.Scribe
{
    public class Scribe
    {
        long _lastFrameTime;
        public IScribeEnvironmentAPI API => _api;
        IScribeEnvironmentAPI _api;
        public Scribe(IScribeEnvironmentAPI api, IEnumerable<IScribeWriter> writers)
        {
            _lastFrameTime = api.CurrentTime;
            _api = api;
            var harmony = HarmonyInstance.Create("TheColonyScribe");
            foreach (var writer in writers)
            {
                writer.ConnectToCode(harmony);
            }
            harmony.PatchAll();
        }
        public Queue<ScribeFrameData> Frames = new Queue<ScribeFrameData>();
        ConcurrentQueue<ScribeEventMeta> _scribeEvents = new ConcurrentQueue<ScribeEventMeta>();
        public void NotifyOfEvent(ScribeEvent e)
        {
            var now = _api.CurrentTime;
            _scribeEvents.Enqueue(new ScribeEventMeta() { Event = e, CurrentTime = now });
        }

        public ScribeFrameData WriteDown()
        {
            int count = _scribeEvents.Count;
            ScribeEventMeta[] frameMetas = new ScribeEventMeta[count];
            for (int i = 0; i < count; i++)
            {
                ScribeEventMeta meta;
                if (_scribeEvents.TryDequeue(out meta))
                {
                    frameMetas[i] = meta;
                }
                else
                {
                    //error?
                }
            }
            ScribeFrameData frame = new ScribeFrameData();
            frame.EventsMetas = frameMetas;
            var now = _api.CurrentTime;
            frame.Delta = now - _lastFrameTime;
            frame.GotAt = _lastFrameTime;
            _lastFrameTime = now;
            Frames.Enqueue(frame);
            return frame;
        }
    }

    public interface IScribeEnvironmentAPI
    {
        long FromSeconds(float time);
        float ToSeconds(long time);
        long CurrentTime { get; }
    }

    public class ScribeFrameData
    {
        public FrameStatistics Statistics;
        public long GotAt;
        public long Delta;
        public ScribeEventMeta[] EventsMetas = Array.Empty<ScribeEventMeta>();
    }
    public struct ScribeEventMeta
    {
        public long CurrentTime { get { return Event.CurrentTime; } set { Event.CurrentTime = value; } }
        public ScribeEvent Event;
    }
    public interface ILongEvent
    {
        float ElapsedMilliseconds { get; }
    }
    public class ScribeEvent
    {
        public long CurrentTime;

    }
}
