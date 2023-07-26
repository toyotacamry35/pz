using SharedCode.EntitySystem;
using System.Collections;
using System.Collections.Generic;

namespace GeneratedCode.Manual.AsyncStack
{
    public struct AsyncStackEnumerable : IEnumerable<EntitiesContainer>, IEnumerator<EntitiesContainer>
    {
        private static readonly EntitiesContainer NullElement = new EntitiesContainer(null, null, null, null);
        public enum Direction
        {
            HeadToTail,
            TailToHead,
            Single
        }

        private readonly Direction _direction;
        private readonly EntitiesContainer _start;

        public AsyncStackEnumerable GetEnumerator() => this;
        IEnumerator<EntitiesContainer> IEnumerable<EntitiesContainer>.GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;

        private AsyncStackEnumerable(EntitiesContainer start, Direction direction)
        {
            _start = start;
            _direction = direction;
            Current = NullElement;
        }

        public EntitiesContainer Current { get; private set; }

        object IEnumerator.Current => Current;

        public void Dispose() { }

        private EntitiesContainer Next
        {
            get
            {
                switch (_direction)
                {
                    case Direction.HeadToTail:
                        return Current.Parent;
                    case Direction.TailToHead:
                        return Current.Child;
                }
                return null;
            }
        }

        public bool MoveNext()
        {
            if (Current == NullElement)
            {
                Current = _start;
                return Current != null;
            }

            var next = Next;
            if (next == null)
                return false;

            Current = next;
            return true;
        }

        public void Reset() => Current = NullElement;

        public static AsyncStackEnumerable ToChildren(EntitiesContainer start) => new AsyncStackEnumerable(start, Direction.TailToHead);
        public static AsyncStackEnumerable ToParents(EntitiesContainer start) => new AsyncStackEnumerable(start, Direction.HeadToTail);
        public static AsyncStackEnumerable Single(EntitiesContainer start) => new AsyncStackEnumerable(start, Direction.Single);
    }
}
