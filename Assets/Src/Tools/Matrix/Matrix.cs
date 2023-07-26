using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Src.Tools
{
    public class Matrix
    {
        public class RowAccessor : IEnumerable<Matrix>
        {
            private Matrix _matrix;

            public RowAccessor(Matrix matrix)
            {
                _matrix = matrix;
            }

            public Matrix this[Int32 rowIndex]
            {
                get
                {
                    return this.AsEnumerable().ElementAt(rowIndex);
                }
                set
                {
                    if (value.ColumnCount != _matrix.ColumnCount || value.RowCount != 1)
                        throw new DimensionMismatchException();

                    Matrix targetRow = this.AsEnumerable().ElementAt(rowIndex);
                    for (Int32 i = 0; i < value.ColumnCount; i++)
                        targetRow[i, 0] = value[i, 0];
                }
            }

            public IEnumerable<Matrix> AsEnumerable()
            {
                int j = 0;
                while (j < _matrix.RowCount)
                    yield return _matrix.SubMatrix(new Int32Range(0, _matrix.ColumnCount), new Int32Range(j, ++j));
            }

            public IEnumerator<Matrix> GetEnumerator()
            {
                return this.AsEnumerable().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        public class ColumnAccessor : IEnumerable<Matrix>
        {
            private Matrix _matrix;

            public ColumnAccessor(Matrix matrix)
            {
                _matrix = matrix;
            }

            public Matrix this[Int32 columnIndex]
            {
                get
                {
                    return this.AsEnumerable().ElementAt(columnIndex);
                }
                set
                {
                    if (value.RowCount != _matrix.RowCount || value.ColumnCount != 1)
                        throw new DimensionMismatchException();

                    Matrix targetColumn = this.AsEnumerable().ElementAt(columnIndex);
                    for (int j = 0; j < value.RowCount; j++)
                        targetColumn[0, j] = value[0, j];
                }
            }

            public IEnumerable<Matrix> AsEnumerable()
            {
                int i = 0;
                while (i < _matrix.ColumnCount)
                    yield return _matrix.SubMatrix(new Int32Range(i, ++i), new Int32Range(0, _matrix.RowCount));
            }

            public IEnumerator<Matrix> GetEnumerator()
            {
                return this.AsEnumerable().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        private MatrixBase<float> _matrix;

        public Matrix(int columns, int rows)
        {
            _matrix = new MatrixBase<float>(columns, rows);
        }

        public Matrix(float[,] doubleArray)
        {
            _matrix = new MatrixBase<float>(doubleArray);
        }

        public Matrix(MatrixBase<float> matrix)
        {
            _matrix = matrix;
        }

        public Matrix(Matrix matrix)
        {
            _matrix = matrix.InnerMatrix.Clone();
        }

        public virtual float this[Int32 column, Int32 row]
        {
            get { return _matrix[column, row]; }
            set { _matrix[column, row] = value; }
        }

        public virtual Int32 ColumnCount { get { return _matrix.ColumnCount; } }
        public virtual ColumnAccessor Columns { get { return new ColumnAccessor(this); } }

        public virtual MatrixDataSource<float> DataSource { get { return _matrix.DataSource; } }

        public MatrixBase<float> InnerMatrix { get { return _matrix; } }

        public Matrix Inverse { get { return Matrix.Invert(this); } }

        public virtual Int32 RowCount { get { return _matrix.RowCount; } }

        public virtual RowAccessor Rows { get { return new RowAccessor(this); } }

        public virtual Matrix Transposed { get { return new Matrix(_matrix.Transposed); } }

        public void SwapRows(Int32 row1, Int32 row2)
        {
            MatrixBase<float>.SwapRows(this, row1, row2);
        }

        public void CopyFrom(Matrix sourceMatrix)
        {
            this.InnerMatrix.CopyFrom(sourceMatrix.InnerMatrix, 0, 0);
        }

        public Matrix SubMatrix(Int32Range columnRange, Int32Range rowRange)
        {
            return new Matrix(MatrixBase<float>.SubMatrix(this.InnerMatrix, columnRange, rowRange));
        }

        public override string ToString()
        {
            return _matrix.ToString();
        }

        protected static Matrix Addition(Matrix matrix1, Matrix matrix2)
        {
            return new Matrix(MatrixBase<float>.ElementWiseOperation(matrix1.InnerMatrix, matrix2.InnerMatrix, delegate (float element1, float element2) { return element1 + element2; }));
        }

        protected static Matrix Addition(Matrix matrix, float scalar)
        {
            return new Matrix(MatrixBase<float>.ElementWiseOperation(matrix.InnerMatrix, scalar, delegate (float element1, float element2) { return element1 + element2; }));
        }

        protected static Matrix Subtraction(Matrix matrix1, Matrix matrix2)
        {
            return new Matrix(MatrixBase<float>.ElementWiseOperation(matrix1.InnerMatrix, matrix2.InnerMatrix, delegate (float element1, float element2) { return element1 - element2; }));
        }

        protected static Matrix Subtraction(Matrix matrix, float scalar)
        {
            return new Matrix(MatrixBase<float>.ElementWiseOperation(matrix.InnerMatrix, scalar, delegate (float element1, float element2) { return element1 - element2; }));
        }

        protected static Matrix Division(Matrix matrix, float scalar)
        {
            return new Matrix(MatrixBase<float>.ElementWiseOperation(matrix.InnerMatrix, scalar, delegate (float element1, float element2) { return element1 / element2; }));
        }

        protected static Matrix Multiplication(Matrix matrix, float scalar)
        {
            return new Matrix(MatrixBase<float>.ElementWiseOperation(matrix.InnerMatrix, scalar, delegate (float element1, float element2) { return element1 * element2; }));
        }

        protected static Matrix Multiplication(Matrix matrix1, Matrix matrix2)
        {
            if (matrix1.ColumnCount != matrix2.RowCount)
                throw new ArithmeticException("Number of columns in first matrix does not equal number of rows in second matrix.");

            Matrix result = new Matrix(matrix2.ColumnCount, matrix1.RowCount);

            for (int j = 0; j < result.RowCount; j++)
                for (int i = 0; i < result.ColumnCount; i++)
                {
                    float value = 0;
                    for (int k = 0; k < matrix1.ColumnCount; k++)
                        value += matrix1[k, j] * matrix2[i, k];
                    result[i, j] = value;
                }

            return result;
        }

        public static Matrix Identity(int columns, int rows)
        {
            Matrix identity = new Matrix(columns, rows);
            for (int i = 0; i < columns; i++)
                for (int j = 0; j < rows; j++)
                    identity[i, j] = (i == j ? 1 : 0);
            return identity;
        }

        protected static Matrix Invert(Matrix matrix)
        {
            if (matrix.RowCount != matrix.ColumnCount) throw new ArithmeticException("Cannot invert non-square matrix");

            // create an augmented matrix [A,I] with the input matrix I on the 
            // left hand side and the identity matrix I on the right hand side
            //
            //    [ 2 5 6 | 1 0 0 ]
            // eg [ 8 3 1 | 0 1 0 ]
            //    [ 2 9 2 | 0 0 1 ]
            //
            Matrix augmentedMatrix =
                Matrix.JoinHorizontal(new Matrix[] { matrix, Matrix.Identity(matrix.ColumnCount, matrix.RowCount) });

            for (int j1 = 0; j1 < augmentedMatrix.RowCount; j1++)
            {

                // check to see if any of the rows subsequent to i have a 
                // higher absolute value on the current diagonalOffset (i,i).
                // if so, switch them to minimize rounding errors
                //
                //    [ (2) 5  6  | 1 0 0 ]                    [ (8) 3  1  | 0 1 0 ]
                // eg [  8 (3) 1  | 0 1 0 ] -> SWAP(R1, R2) -> [  2 (5) 6  | 1 0 0 ] 
                //    [  2  9 (2) | 0 0 1 ]                    [  2  9 (2) | 0 0 1 ]
                //
                for (int j2 = j1 + 1; j2 < augmentedMatrix.RowCount; j2++)
                {
                    if (Math.Abs(augmentedMatrix[j1, j2]) > Math.Abs(augmentedMatrix[j1, j1]))
                    {
                        //Console.WriteLine("Swap [" + j2 + "] with [" + i + "]");
                        augmentedMatrix.SwapRows(j1, j2);
                    }
                }

                // normalize the row so the diagonalOffset value (i,i) is 1
                // if (i,i) is 0, this row is null (we have > 0 nullity for this matrix)
                //
                //    [ (8) 3  1  | 0 1 0 ]                   [ (1.0) 0.4  0.1 | 0.0 0.1 0.0 ]
                // eg [  2 (5) 6  | 1 0 0 ] -> R1 = R1 / 8 -> [ 2.0  (5.0) 6.0 | 1.0 0.0 0.0 ] 
                //    [  2  9 (2) | 0 0 1 ]                   [ 2.0   9.0 (2.0) | 0.0 0.0 1.0 ]  

                //Console.WriteLine("Divide [" + i + "] by " + augmentedMatrix[i, i].ToString("0.00"));
                augmentedMatrix.Rows[j1].CopyFrom(augmentedMatrix.Rows[j1] / augmentedMatrix[j1, j1]);


                // look at each pair of rows {i, r} to see if r is linearly
                // dependent on i. if r does contain some factor of i vector,
                // subtract it out to make {i, r} linearly independent
                for (int j2 = 0; j2 < augmentedMatrix.RowCount; j2++)
                {
                    if (j2 != j1)
                    {
                        //Console.WriteLine("Subtracting " + augmentedMatrix[i, j2].ToString("0.00") + " i [" + i + "] from [" + j2 + "]");
                        augmentedMatrix.Rows[j2].CopyFrom(new Matrix(augmentedMatrix.Rows[j2] - (augmentedMatrix[j1, j2] * augmentedMatrix.Rows[j1])));
                    }
                }
            }

            // separate the inverse from the right hand side of the augmented matrix
            //
            //    [ (1) 0  0  |     [ 2 5 6 ] 
            // eg [  0 (1) 0  | ~   [ 8 2 1 ] -> inverse
            //    [  0  0 (1) |     [ 5 2 2 ] 
            //
            Matrix inverse = augmentedMatrix.SubMatrix(new Int32Range(matrix.ColumnCount, matrix.ColumnCount + matrix.ColumnCount), new Int32Range(0, matrix.RowCount));
            return inverse;
        }

        public static Matrix JoinHorizontal(IEnumerable<Matrix> matricies)
        {
            List<MatrixBase<float>> innerMatricies = new List<MatrixBase<float>>();
            foreach (Matrix matrix in matricies)
                innerMatricies.Add(matrix.InnerMatrix);
            return new Matrix(MatrixBase<float>.JoinHorizontal(innerMatricies));
        }

        public static Matrix JoinHorizontal(Matrix leftMatrix, Matrix rightMatrix)
        {
            return Matrix.JoinHorizontal(new Matrix[] { leftMatrix, rightMatrix });
        }

        public static Matrix operator +(Matrix matrix1, Matrix matrix2)
        {
            return Matrix.Addition(matrix1, matrix2);
        }

        public static Matrix operator +(Matrix matrix, float scalar)
        {
            return Matrix.Addition(matrix, scalar);
        }

        public static Matrix operator +(float scalar, Matrix matrix)
        {
            return Matrix.Addition(matrix, scalar);
        }

        public static Matrix operator -(Matrix matrix1, Matrix matrix2)
        {
            return Matrix.Subtraction(matrix1, matrix2);
        }

        public static Matrix operator -(Matrix matrix, float scalar)
        {
            return Matrix.Subtraction(matrix, scalar);
        }

        public static Matrix operator *(Matrix matrix1, Matrix matrix2)
        {
            return Matrix.Multiplication(matrix1, matrix2);
        }

        public static Matrix operator *(float scalar, Matrix matrix)
        {
            return Matrix.Multiplication(matrix, scalar);
        }

        public static Matrix operator *(Matrix matrix, float scalar)
        {
            return Matrix.Multiplication(matrix, scalar);
        }

        public static Matrix operator /(Matrix matrix, float scalar)
        {
            return Matrix.Division(matrix, scalar);
        }

        public static implicit operator MatrixBase<float>(Matrix doubleMatrix)
        {
            return doubleMatrix.InnerMatrix;
        }

        public static implicit operator Matrix(float[,] dataArray)
        {
            return new Matrix(dataArray);
        }
    }
}
