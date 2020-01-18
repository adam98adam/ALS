using System;

namespace ALS {
    public class ObjectiveFunction {
        public static void Calculate(int d, Matrix R, Matrix U, Matrix P, double lambda)
        {

            double sum1 = 0;
            
            for (int i = 0; i < R.Data.GetLength(0); i++) {
                for (int j = 0; j < R.Data.GetLength(1); j++) {//i = users j = products
                    if (R.Data[i, j] != 0) {
                        double vectorSum = 0;
                        for (int row = 0; row < d; row++)
                        {
                            vectorSum += U.Data[row, i] * P.Data[row, j];
                        }

                        sum1 += Math.Pow(R.Data[i, j] + vectorSum, 2);
                    }
                }
            }
            
            double sum2 = 0;

            for (int col = 0; col < U.ColumnCount; col++)
            {
                double vectorSum = 0;
                for (int row = 0; row < d; row++)
                {
                    
                    vectorSum += Math.Pow(U.Data[row, col], 2);
                }

                sum2 += vectorSum;
            }


            double sum3 = 0;

            for (int col = 0; col < P.ColumnCount; col++)
            {
                double vectorSum = 0;
                for (int row = 0; row < d; row++)
                {
                    vectorSum += Math.Pow(P.Data[row, col], 2);
                }

                sum3 += vectorSum;
            }


            double total = sum1 + lambda * (sum2 + sum3);

            //Console.WriteLine($"sum1 = ${sum1}, sum2 = ${sum2}, sum3 = ${sum3}, total = ${total}");
           Console.WriteLine(total);
        }
    }
}