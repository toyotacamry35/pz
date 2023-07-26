using System;

namespace Assets.Src.Tools
{
    public class MatrixDataSource<T>
    {
        private T[,] _data;

        public MatrixDataSource(Int32 columns, Int32 rows)
        {
            _data = new T[columns, rows];
        }

        public MatrixDataSource(T[,] dataArray)
        {
            Int32 rowCount = dataArray.GetLength(0);
            Int32 columnCount = dataArray.GetLength(1);
            _data = new T[columnCount, rowCount];
            for (Int32 i = 0; i < columnCount; i++)
                for (Int32 j = 0; j < rowCount; j++)
                    _data[i, j] = dataArray[j, i];
        }

        public T this[Int32 column, Int32 row]
        {
            get { return _data[column, row]; }
            set { _data[column, row] = value; }
        }

        public Int32 RowCount { get { return _data.GetLength(1); } }
        public Int32 ColumnCount { get { return _data.GetLength(0); } }
    }

    public class Int32Range
    {
        private Int32 _end;
        private Int32 _start;

        public Int32 End
        {
            get { return _end; }
            set { _end = value; }
        }

        public Int32 Length
        {
            get { return _end - _start; }
        }

        public Int32 Start
        {
            get { return _start; }
            set { _start = value; }
        }

        public Int32Range(Int32 start, Int32 end)
        {
            _start = Math.Min(start, end);
            _end = Math.Max(start, end);
        }

        public bool Contains(Int32 value)
        {
            return value >= _start && value < _end;
        }

        public bool Contains(Int32Range range)
        {
            return range.Start >= _start && range.End <= _end;
        }

        public override string ToString()
        {
            return String.Format("{0} [{1} - {2}]\n", this.GetType().Name, _start, _end);
        }
    }

    public class DimensionMismatchException : ArithmeticException
    {
        public DimensionMismatchException() : base() { }
        public DimensionMismatchException(string message) : base(message) { }
        public DimensionMismatchException(string message, Exception innerException) : base(message, innerException) { }
    }
}
