﻿using System;

namespace ALS
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser p = new Parser();
            p.parse();
            p.printProductList(p.ProductList);
        }
    }
}
