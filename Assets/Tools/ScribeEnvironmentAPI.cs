using SharedCode.Scribe;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Tools
{
    public class ScribeEnvironmentAPI : IScribeEnvironmentAPI
    {
        Stopwatch _watch;
        public ScribeEnvironmentAPI()
        {
            _watch = new Stopwatch();
            _watch.Start();
        }
        public long CurrentTime
        {
            get
            {
                return _watch.ElapsedTicks;
            }
        }

        public float ToSeconds(long time)
        {
            return (float)time / (float)10000000;
        }
        public long FromSeconds(float time)
        {
            return (long)(time  * 10000000);
        }
    }
}
