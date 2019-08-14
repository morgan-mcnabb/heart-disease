using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartDisease
{
    class Matrix
    {
        public int Rows;
        public int Columns;
        public double[,] m;
        public Random r;
        public Matrix(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            r = new Random();
            this.m = new double[Rows, Columns];
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {

                    this.m[i, j] = r.NextDouble() - 0.5;

                }
            }
        }

        public double[,] MatrixScale(int scalar)
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    m[i, j] = m[i, j] * scalar;
                }
            }
            return m;
        }

        public double[,] MatrixAdd(Matrix other)
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    m[i, j] += other.m[i, j];
                }
            }
            return m;
        }
        public Matrix MatrixMultiplication(Matrix other)
        {
            Matrix multiply = new Matrix(Rows, other.Columns);
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < other.Columns; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < Columns; k++)
                    {
                        sum += m[i, k] * other.m[k, j];
                    }
                    multiply.m[i, j] = sum;
                }
            }
            return multiply;
        }
    }
}
