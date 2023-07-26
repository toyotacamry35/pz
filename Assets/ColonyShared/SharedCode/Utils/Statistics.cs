
namespace Assets.ColonyShared.SharedCode.Utils.Statistics
{
    public class MinMaxAvgRegistratorFloat
    {
        public long Count { get; private set; }
        public float Min { get; private set; }
        public float Max { get; private set; }
        /// <summary>
        /// Most recently registered value:
        /// </summary>
        public float Last { get; private set; }
        public float Avg => _summ / Count;
        private float _summ;

        public void BeginNewSample()
        {
            Clean();
        }

        public void RegisterNewValue(float value)
        {
            Last = value;
            ++Count;
            if (value > Max)
                Max = value;
            else if (value < Min)
                Min = value;

            _summ += value;
        }

        public void GetSampleData(out float min, out float max, out float last, out float avg)
        {
            min = Min;
            max = Max;
            avg = Avg;
            last = Last;
        }

        private void Clean()
        {
            Count = 0;
            _summ = 0f;
            Last  = 0f;
            Min = float.MaxValue;
            Max = float.MinValue;
        }

        public override string ToString()
        {
            return $"Avg: {Avg:##.000}, Min: {Min:##.000}, Max: {Max:##.000}, N: {Count}, Last: {Last:##.000}";
        }
    }

    public class MinMaxAvgRegistratorInt
    {
        public long Count { get; private set; }
        public int Min   { get; private set; }
        public int Max   { get; private set; }
        /// <summary>
        /// Most recently registered value:
        /// </summary>
        public int Last  { get; private set; }
        public float Avg => (float)_summ / Count;
        private int _summ;

        public void BeginNewSample()
        {
            Clean();
        }

        public void RegisterNewValue(int value)
        {
            Last = value;
            ++Count;
            if (value > Max)
                Max = value;
            else if (value < Min)
                Min = value;

            _summ += value;
        }

        public void GetSampleData(out int min, out int max, out int last, out float avg)
        {
            min = Min;
            max = Max;
            avg = Avg;
            last = Last;
        }

        private void Clean()
        {
            Count = 0;
            _summ = 0;
            Last  = 0;
            Min = int.MaxValue;
            Max = int.MinValue;
        }

        public override string ToString()
        {
            return $"Avg: {Avg}, Min: {Min}, Max: {Max}, N: {Count}, Last: {Last}";
        }
    }

}
