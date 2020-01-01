using System;

namespace ALS
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser p = new Parser();
            p.takeMostRatedProducts(50);
            p.printProductList(p.MostRatedProducts);
            p.takeMostActiveUsers(50);

            
        }
    }
}
