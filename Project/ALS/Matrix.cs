using System;
using System.Collections.Generic;
using System.Text;

namespace ALS
{
    class Matrix
    {
        public double[,] Data { get; set; }
        public int RowCount { get; set; }
        public int ColumnCount { get; set; }

        public Matrix(int rows, int columns)
        {
            RowCount = rows;
            ColumnCount = columns;
            Data = new double[rows, columns];
        }


        public void FillRandom()
        {
            Random random = new Random();
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColumnCount; j++)
                {
                    Data[i, j] = random.NextDouble();
                }
            }
        }

        public static Matrix operator *(Matrix leftFactor, Matrix rightFactor)
        {
            if (leftFactor.ColumnCount != rightFactor.RowCount)
            {
                throw new ArgumentException("Cannot multiply, invalid size of matrices");
            }

            Matrix product = new Matrix(leftFactor.RowCount, rightFactor.ColumnCount);
            for (int i = 0; i < leftFactor.RowCount; i++)
            {
                for (int j = 0; j < rightFactor.ColumnCount; j++)
                {
                    double sum = 0;

                    for (int k = 0; k < rightFactor.RowCount; k++)
                    {
                        double leftValue = leftFactor.Data[i, k];
                        double rightValue = rightFactor.Data[k, j];

                        sum += leftValue * rightValue;
                    }
                    product.Data[i, j] = sum;
                }
            }

            return product;
        }

        public void SwapRows(int row1, int row2)
        {
            for (int i = 0; i < ColumnCount; i++)
            {
                double tmp = Data[row1, i];
                Data[row1, i] = Data[row2, i];
                Data[row2, i] = tmp;
            }
        }

        public Matrix GetTransposed()
        {
            var transposedMatrix = new Matrix(ColumnCount, RowCount);

            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColumnCount; j++)
                {
                    transposedMatrix.Data[j, i] = Data[i, j];
                }
            }

            return transposedMatrix;
        }

        public void AddLambdaMatrix(double lambda)
        {
            for (int i = 0; i < Math.Min(RowCount, ColumnCount); i++)
            {
                Data[i, i] += lambda;
            }
        }
    }
}
