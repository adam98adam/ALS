using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ALS {
    public class ALS {
        public readonly Matrix R, U, P;

        private readonly int countOfFactors;
        private Dictionary<Tuple<int, int>, double> TestSetValues;
        private double lambda;
        private int iterations;
        public ALS(int iloscFaktorow, int iloscProduktow, int iloscUserow, double lambda, int iterations)
        {
            Parser p = new Parser();
            p.takeMostRatedProducts(iloscProduktow);
            p.takeMostActiveUsers(iloscUserow);
            R = p.returnMatrix();
            U = new Matrix(iloscFaktorow, R.Data.GetLength(0));
            U.FillRandom();
            P = new Matrix(iloscFaktorow, R.Data.GetLength(1));
            P.FillRandom();
            countOfFactors = iloscFaktorow;
            this.lambda = lambda;
            this.iterations = iterations;
        }

        public void startALS() {
            Stopwatch s = new Stopwatch();
            TestSetValues = CreateTestSet();
            s.Start();
            Execute();

            double sumOfErrors = 0;
            
            foreach (var item in TestSetValues)
            {
                var expected = item.Value;

                int userID = item.Key.Item1;
                int productID = item.Key.Item2;

                double real = 0;
                for (int row = 0; row < countOfFactors; row++)
                {
                    real += U.Data[row, userID] * P.Data[row, productID];
                }

                double diff = Math.Abs(expected - real);
                
                Console.WriteLine("Oczekiwana: " + expected + ", Rzeczywista: " + real + ", różnica: " + diff);
                sumOfErrors += diff;
            }

            double averageDiff = sumOfErrors / TestSetValues.Count;
            
            Console.WriteLine("\nŚredni błąd: " + averageDiff);
            Console.WriteLine("Czas wykonania: " + ((double)s.ElapsedMilliseconds) / 1000 + "s");
            s.Stop();
        }
        
        public Dictionary<Tuple<int, int>, double> CreateTestSet()
        {
            var listOfTestRatings = new Dictionary<Tuple<int, int>, double>();
            double total = 0;
            double percent = 10;
            for (int i = 0; i < R.Data.GetLength(0); i++) {
                for (int j = 0; j < R.Data.GetLength(1); j++) {//i = users j = products
                    if (R.Data[i, j] != 0) {
                        if (total > 100)
                        {
                            listOfTestRatings.Add(Tuple.Create(i, j), R.Data[i, j]);
                            R.Data[i, j] = 0;
                            total -= 100;
                        }
                        total += percent;
                    }

                }
            }
            return listOfTestRatings;
        }
        

        public void Execute()
        {
            for (int k = 0; k < iterations; k++)
            {
                for (int u = 0; u < R.Data.GetLength(0); u++)
                {
                    List<int> productsRatedByU = R.FindAllProductsRatedByUser(u);

                    Matrix Piu = new Matrix(countOfFactors, productsRatedByU.Count);

                    for (int i = 0; i < productsRatedByU.Count; i++)
                    {
                        for (int row = 0; row < countOfFactors; row++)
                        {
                            Piu.Data[row, i] = P.Data[row, productsRatedByU[i]];
                        }
                    }
                    Matrix Au = Piu * Piu.GetTransposed();
                    Au.AddLambdaMatrix(lambda);

                    Matrix Vu = new Matrix(countOfFactors, 1);

                    for (int col = 0; col < Piu.ColumnCount; col++)
                    {
                        double rating = R.Data[u, productsRatedByU[col]];

                        for (int row = 0; row < countOfFactors; row++)
                        {
                            Vu.Data[row, 0] += rating * Piu.Data[row, col];
                        }
                    }

                    Matrix X = Gauss.Calculate(Au, Vu);

                    for (int row = 0; row < countOfFactors; row++)
                    {
                        U.Data[row, u] = X.Data[row, 0];
                    }

                }

                for (int p = 0; p < R.Data.GetLength(1); p++)
                {
                    List<int> usersWhoRatedP = R.FindAllUsersWhoRatedProduct(p);

                    Matrix Uip = new Matrix(countOfFactors, usersWhoRatedP.Count);

                    for (int i = 0; i < usersWhoRatedP.Count; i++)
                    {
                        for (int row = 0; row < countOfFactors; row++)
                        {
                            Uip.Data[row, i] = U.Data[row, usersWhoRatedP[i]];
                        }
                    }

                    Matrix Bu = Uip * Uip.GetTransposed();
                    Bu.AddLambdaMatrix(lambda);

                    Matrix Wp = new Matrix(countOfFactors, 1);

                    for (int col = 0; col < Uip.ColumnCount; col++)
                    {
                        double rating = R.Data[usersWhoRatedP[col], p];

                        for (int row = 0; row < countOfFactors; row++)
                        {
                            Wp.Data[row, 0] += rating * Uip.Data[row, col];
                        }
                    }

                    Matrix X = Gauss.Calculate(Bu, Wp);

                    for (int row = 0; row < countOfFactors; row++)
                    {
                        P.Data[row, p] = X.Data[row, 0];
                    }
                }

                ObjectiveFunction.Calculate(countOfFactors, R, U, P, lambda);
            }
        }
    }
}