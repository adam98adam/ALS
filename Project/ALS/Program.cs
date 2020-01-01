using System;

namespace ALS
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser p = new Parser();
            p.takeMostRatedProducts(150);
            p.printProductList(p.MostRatedProducts);
        }
    }
}
