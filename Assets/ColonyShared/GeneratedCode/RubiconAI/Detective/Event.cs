using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Src.Detective
{
    public class Event 
    {
        public List<Event> SubEvents = new List<Event>();

        public List<T> GetAllEventsOfType<T>() where T : Event
        {
            var list = new List<T>();
            foreach(var subEvent in SubEvents)
            {
                if (subEvent is T)
                    list.Add((T)subEvent);
                GetAllEventsOfType(list);
            }
            return list;
        }
        public void GetAllEventsOfType<T>(List<T> list) where T : Event
        {
            foreach (var subEvent in SubEvents)
            {
                if (subEvent is T)
                    list.Add((T)subEvent);
                GetAllEventsOfType<T>(list);
            }
        }
    }
}
