using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Src.Tools
{
    public class MatrixBase<T>
    {
        public class RowAccessor<R> : IEnumerable<MatrixBase<R>>
        {
            private MatrixBase<R> _dataView;
            public RowAccessor(MatrixBase<R> dataView)
            {
                _dataView = dataView;
            }

            public MatrixBase<R> this[Int32 rowIndex]
            {
                get
                {
                    return this.AsEnumerable().ElementAt(rowIndex);
                }
                set
                {
                    if (value.ColumnCount != _dataView.ColumnCount || value.RowCount != 1)
                        throw new DimensionMismatchException();

                    MatrixBase<R> targetRow = this.AsEnumerable().ElementAt(rowIndex);
                    for (int i = 0; i < value.ColumnCount; i++)
                        targetRow[i, 0] = value[i, 0];
                }
            }

            public IEnumerable<MatrixBase<R>> AsEnumerable()
            {
                int j = 0;
                while (j < _dataView.RowCount)
                    yield return _dataView.SubMatrix(new Int32Range(0, _dataView.ColumnCount), new Int32Range(j, ++j));
            }

            public IEnumerator<MatrixBase<R>> GetEnumerator()
            {
                return this.AsEnumerable().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        public class ColumnAccessor<C> : IEnumerable<MatrixBase<C>>
        {
            private MatrixBase<C> _dataView;

            public ColumnAccessor(MatrixBase<C> dataView)
            {
                _dataView = dataView;
            }

            public MatrixBase<C> this[Int32 columnIndex]
            {
                get
                {
                    return this.AsEnumerable().ElementAt(columnIndex);
                }
                set
                {
                    if (value.RowCount != _dataView.RowCount || value.ColumnCount != 1)
                        throw new DimensionMismatchException();

                    MatrixBase<C> targetColumn = this.AsEnumerable().ElementAt(columnIndex);
                    for (int j = 0; j < value.RowCount; j++)
                        targetColumn[0, j] = value[0, j];
                }
            }

            public IEnumerable<MatrixBase<C>> AsEnumerable()
            {
                int i = 0;
                while (i < _dataView.ColumnCount)
                    yield return _dataView.SubMatrix(new Int32Range(i, ++i), new Int32Range(0, _dataView.RowCount));
            }

            public IEnumerator<MatrixBase<C>> GetEnumerator()
            {
                return this.AsEnumerable().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        private Int32Range _dataColumnRange;
        private Int32Range _dataRowRange;
        private MatrixDataSource<T> _dataSource;

        public MatrixBase(Int32 columns, Int32 rows)
        {
            _dataSource = new MatrixDataSource<T>(columns, rows);
            _dataRowRange = new Int32Range(0, _dataSource.RowCount);
            _dataColumnRange = new Int32Range(0, _dataSource.ColumnCount);
        }

        public MatrixBase(T[,] elementArray)
        {
            _dataSource = new MatrixDataSource<T>(elementArray);
            _dataRowRange = new Int32Range(0, _dataSource.RowCount);
            _dataColumnRange = new Int32Range(0, _dataSource.ColumnCount);
        }

        public MatrixBase(MatrixBase<T> matrix) : this(matrix.ColumnCount, matrix.RowCount)
        {
            this.CopyFrom(matrix);
        }

        public MatrixBase(MatrixDataSource<T> matrixData)
        {
            _dataSource = matrixData;
            _dataRowRange = new Int32Range(0, matrixData.RowCount);
            _dataColumnRange = new Int32Range(0, matrixData.ColumnCount);
        }

        protected MatrixBase(MatrixDataSource<T> matrixData, Int32Range columnRange, Int32Range rowRange)
        {
            _dataSource = matrixData;
            _dataColumnRange = columnRange;
            _dataRowRange = rowRange;
        }

        public virtual T this[Int32 column, Int32 row]
        {
            get
            {
                if (!_dataColumnRange.Contains(column + _dataColumnRange.Start)) throw new ArgumentOutOfRangeException("column");
                if (!_dataRowRange.Contains(row + _dataRowRange.Start)) throw new ArgumentOutOfRangeException("row");
                return _dataSource[column + _dataColumnRange.Start, row + _dataRowRange.Start];
            }
            set
            {
                if (!_dataColumnRange.Contains(column + _dataColumnRange.Start)) throw new ArgumentOutOfRangeException("column");
                if (!_dataRowRange.Contains(row + _dataRowRange.Start)) throw new ArgumentOutOfRangeException("row");
                _dataSource[column + _dataColumnRange.Start, row + _dataRowRange.Start] = value;
            }
        }

        /// <summary>Gets the number of columns visible within the matrix data view</summary>
        public virtual Int32 ColumnCount { get { return _dataColumnRange.Length; } }

        /// <summary>Gets a column accessor for this matrix, enabling column-based operations</summary>
        public virtual ColumnAccessor<T> Columns { get { return new ColumnAccessor<T>(this); } }

        /// <summary>Gets a cloned copy of this matrix</summary>
        public virtual MatrixBase<T> Copy { get { return this.Clone(); } }

        /// <summary>Gets the range of columns visible within the matrix data view</summary>
        public virtual Int32Range DataColumnRange { get { return _dataColumnRange; } }

        /// <summary>Gets the range of rows visible within the matrix data view</summary>
        public virtual Int32Range DataRowRange { get { return _dataRowRange; } }

        /// <summary>Gets a reference to the data source underlying this matrix data view</summary>
        public virtual MatrixDataSource<T> DataSource { get { return _dataSource; } }

        /// <summary>Indicates whether this is a square matrix</summary>
        public virtual Boolean IsSquare { get { return this.RowCount == this.ColumnCount; } }

        public virtual Int32 RowCount { get { return _dataRowRange.Length; } }

        /// <summary>Gets a row accessor for this matrix, enabling row-based operations</summary>
        public virtual RowAccessor<T> Rows { get { return new RowAccessor<T>(this); } }

        /// <summary>Gets a transposed copy of this matrix (A^T)</summary>
        public virtual MatrixBase<T> Transposed { get { return MatrixBase<T>.Transpose(this); } }

        public delegate T ElementUnaryOperationDelegate(T element);
        public delegate T ElementBinaryOperationDelegate(T element1, T element2);
        public delegate T ElementPositionalUnaryOperationDelegate(T element, Int32 column, Int32 row);
        public delegate T ElementPositionalBinaryOperationDelegate(T element1, T element2, Int32 column, Int32 row);

        public virtual MatrixBase<T> Clone()
        {
            return new MatrixBase<T>(this);
        }

        public virtual void CopyInto(MatrixBase<T> targetMatrix)
        {
            this.CopyInto(targetMatrix, 0, 0);
        }

        public virtual void CopyInto(
            MatrixBase<T> targetMatrix,
            Int32 targetColumnOffset,
            Int32 targetRowOffset)
        {
            MatrixBase<T>.ElementWiseCopy(this, targetMatrix, targetColumnOffset, targetRowOffset);
        }

        public virtual void CopyFrom(MatrixBase<T> sourceMatrix)
        {
            this.CopyFrom(sourceMatrix, 0, 0);
        }

        public virtual void CopyFrom(
            MatrixBase<T> sourceMatrix,
            Int32 targetColumnOffset,
            Int32 targetRowOffset)
        {
            MatrixBase<T>.ElementWiseCopy(sourceMatrix, this, targetColumnOffset, targetRowOffset);
        }

        public virtual MatrixBase<T> SubMatrix(
            Int32Range columnRange,
            Int32Range rowRange)
        {
            return MatrixBase<T>.SubMatrix(this, columnRange, rowRange);
        }

        public override String ToString()
        {
            String result = String.Empty;
            result += String.Format("{0} columns x {1} rows\n", this.ColumnCount, this.RowCount);

            for (Int32 j = 0; j < this.RowCount; j++)
            {
                String rowStr = String.Empty;
                for (Int32 i = 0; i < this.ColumnCount; i++)
                    rowStr += String.Format(" {0} ", this[i, j].ToString());
                result += String.Format("[{0}]\n", rowStr);
            }
            return result;
        }

        public static Boolean DimensionsMatch(
            MatrixBase<T> matrix1,
            MatrixBase<T> matrix2)
        {
            if (matrix1 == null) throw new ArgumentNullException("matrix1");
            if (matrix2 == null) throw new ArgumentNullException("matrix2");
            return matrix1.ColumnCount == matrix2.ColumnCount
                   && matrix1.RowCount == matrix2.RowCount;
        }

        public static void ElementWiseCopy(
            MatrixBase<T> source,
            MatrixBase<T> target,
            Int32 targetColumnOffset,
            Int32 targetRowOffset)
        {
            for (Int32 i = 0; i < source.ColumnCount; i++)
                for (Int32 j = 0; j < source.RowCount; j++)
                    target[i + targetColumnOffset, j + targetRowOffset] = source[i, j];
        }

        public static MatrixBase<T> ElementWiseOperation(
            MatrixBase<T> matrix1,
            MatrixBase<T> matrix2,
            ElementBinaryOperationDelegate operation)
        {
            if (MatrixBase<T>.IsNull(matrix1)) throw new ArgumentNullException("matrix1");
            if (MatrixBase<T>.IsNull(matrix2)) throw new ArgumentNullException("matrix2");
            if (matrix1.ColumnCount != matrix2.ColumnCount) throw new DimensionMismatchException("The number of columns in matrix1 does not equal the number of columns in matrix2");
            if (matrix1.RowCount != matrix2.RowCount) throw new DimensionMismatchException("The number of rows in matrix1 does not equal the number of rows in matrix2");

            MatrixBase<T> result = new MatrixBase<T>(matrix1.ColumnCount, matrix1.RowCount);
            for (Int32 i = 0; i < result.ColumnCount; i++)
                for (Int32 j = 0; j < result.RowCount; j++)
                    result[i, j] = operation(matrix1[i, j], matrix2[i, j]);
            return result;
        }

        public static MatrixBase<T> ElementWiseOperation(
            MatrixBase<T> matrix,
            T scalar,
            ElementBinaryOperationDelegate operation)
        {
            if (MatrixBase<T>.IsNull(matrix)) throw new ArgumentNullException("matrix");

            MatrixBase<T> result = new MatrixBase<T>(matrix.ColumnCount, matrix.RowCount);
            for (Int32 i = 0; i < result.ColumnCount; i++)
                for (Int32 j = 0; j < result.RowCount; j++)
                    result[i, j] = operation(matrix[i, j], scalar);
            return result;
        }

        public static MatrixBase<T> ElementWiseOperation(
            MatrixBase<T> matrix,
            ElementUnaryOperationDelegate operation)
        {
            if (MatrixBase<T>.IsNull(matrix)) throw new ArgumentNullException("matrix");

            MatrixBase<T> result = new MatrixBase<T>(matrix.ColumnCount, matrix.RowCount);
            for (Int32 i = 0; i < result.ColumnCount; i++)
                for (Int32 j = 0; j < result.RowCount; j++)
                    result[i, j] = operation(matrix[i, j]);
            return result;
        }

        public static Boolean Equality(
            MatrixBase<T> matrix1,
            MatrixBase<T> matrix2)
        {
            if ((Object)matrix1 == null || (Object)matrix2 == null) return false;
            if (matrix1.ColumnCount != matrix2.ColumnCount) return false;
            if (matrix1.RowCount != matrix2.RowCount) return false;

            for (Int32 i = 0; i < matrix1.ColumnCount; i++)
                for (Int32 j = 0; j < matrix1.RowCount; j++)
                    if (!matrix1[i, j].Equals(matrix2[i, j])) return false;

            return true;
        }

        public static Boolean IsNull(MatrixBase<T> matrix)
        {
            return ((object)matrix == null);
        }

        public static MatrixBase<T> JoinHorizontal(
            IEnumerable<MatrixBase<T>> matricies)
        {
            int? rowCount = null;
            int totalColumnCount = 0;
            foreach (MatrixBase<T> matrix in matricies)
            {
                if (MatrixBase<T>.IsNull(matrix)) throw new ArgumentNullException();
                if (rowCount == null) rowCount = matrix.RowCount;
                else if ((int)rowCount != matrix.RowCount) throw new DimensionMismatchException();
                totalColumnCount += matrix.ColumnCount;
            }

            MatrixBase<T> resultMatrix = new MatrixBase<T>(totalColumnCount, (int)rowCount);

            int columnOffset = 0;
            foreach (MatrixBase<T> matrix in matricies)
            {
                MatrixBase<T>.ElementWiseCopy(matrix, resultMatrix, columnOffset, 0);
                columnOffset += matrix.ColumnCount;
            }

            return resultMatrix;
        }

        public static MatrixBase<T> JoinHorizontal(
            MatrixBase<T> leftMatrix,
            MatrixBase<T> rightMatrix)
        {
            return MatrixBase<T>.JoinHorizontal(new MatrixBase<T>[] { leftMatrix, rightMatrix });
        }

        public static MatrixBase<T> SubMatrix(
            MatrixBase<T> view,
            Int32Range columnRange,
            Int32Range rowRange)
        {
            if (!view.DataColumnRange.Contains(columnRange)) throw new ArgumentOutOfRangeException("columnRange");
            if (!view.DataRowRange.Contains(rowRange)) throw new ArgumentOutOfRangeException("rowRange");

            MatrixBase<T> result
                = new MatrixBase<T>(view.DataSource,
                       new Int32Range(
                           columnRange.Start + view.DataColumnRange.Start,
                           columnRange.End + view.DataColumnRange.Start),
                       new Int32Range(
                           rowRange.Start + view.DataRowRange.Start,
                           rowRange.End + view.DataRowRange.Start));
            return result;
        }

        public static void SwapRows(
            MatrixBase<T> matrix,
            Int32 row1,
            Int32 row2)
        {
            MatrixBase<T> rowTemp = matrix.Rows[row1].Clone();
            matrix.Rows[row1] = matrix.Rows[row2];
            matrix.Rows[row2] = rowTemp;
        }

        public static MatrixBase<T> Transpose(MatrixBase<T> matrix)
        {
            MatrixBase<T> result
                = new MatrixBase<T>(matrix.RowCount, matrix.ColumnCount);

            for (Int32 i = 0; i < matrix.ColumnCount; i++)
                for (Int32 j = 0; j < matrix.RowCount; j++)
                    result[j, i] = matrix[i, j];

            return result;
        }

        public static implicit operator MatrixBase<T>(T[,] dataArray)
        {
            return new MatrixBase<T>(dataArray);
        }
    }
}
