using System;

namespace ALS
{
    class Program
    {
        static void Main(string[] args)
        {
            //Zapełnienie macierzy 12x12 = 95% ; 120x120 = 34% ; 1200x1200 = 5%;
            Parser p = new Parser();
            p.takeMostRatedProducts(1200);
            //p.printProductList(p.MostRatedProducts);
            p.takeMostActiveUsers(1200);
           // p.printProductList(p.MostRatedProducts);
            Matrix Rmatrix = p.returnMatrix();
           // Console.WriteLine(Rmatrix);

        }
    }
}
