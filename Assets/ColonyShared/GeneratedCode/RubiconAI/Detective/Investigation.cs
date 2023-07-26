using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Src.Detective
{
    public class Investigation
    {
        public Investigation(Event startingEvent)
        {
            _currentlyInvestigatedEvents.Push(startingEvent);
        }
        Stack<Event> _currentlyInvestigatedEvents = new Stack<Event>();
        public Event CurrentEvent => _currentlyInvestigatedEvents.Peek();
        public void Investigate(Event e)
        {
            CurrentEvent.SubEvents.Add(e);
        }

        public InvestigationToken<T> BeginInvestigating<T>(T e) where T : Event
        {
            CurrentEvent.SubEvents.Add(e);
            return new InvestigationToken<T>(this, e);
        }
        
        public class InvestigationToken<T> : IDisposable where  T : Event
        {
            public T Event => (T) _e;
            private readonly Investigation _i;
            private readonly Event _e;

            public InvestigationToken(Investigation i, Event e)
            {
                _i = i;
                _e = e;
                _i._currentlyInvestigatedEvents.Push(e);
            }

            public void Dispose()
            {
                _i._currentlyInvestigatedEvents.Pop();
            }
        }
    }

    

}
