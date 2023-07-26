
namespace Assets.ColonyShared.SharedCode.Utils
{
    public class CycleList<T>
    {
        public int Capacity { get; }

        private readonly T[] _arr;
        private readonly int _indexOfLast;
        private int _currIndex;

        public CycleList(int capacity)
        {
            Capacity = capacity;
            _indexOfLast = capacity - 1;
            _arr = new T[Capacity];
        }

        public int Push(T elem)
        {
            if (_currIndex < _indexOfLast)
                ++_currIndex;
            else
                _currIndex = 0;

            _arr[_currIndex] = elem;
            return _currIndex;
        }

        public string ToStringCustom(string delim)
        {
            return string.Join(delim, _arr);
        }

        public override string ToString()
        {
            return ToStringCustom(";  ");
        }
    }
}
