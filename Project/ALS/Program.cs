using System;
using System.Diagnostics;

namespace ALS
{
    class Program
    {
        static void Main(string[] args)
        {
            ALS s1 = new ALS(20,1100,11000,0.1,20);
            s1.startALS();
        }
    }
}
