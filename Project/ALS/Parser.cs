using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ALS {
    public class Parser {
        public List<Product> ProductList;
        private StreamReader sr;
        
        public Parser() {
            sr = new StreamReader("../../../files/amazon.txt");
            ProductList = new List<Product>();
        }

        public void parse() {
            bool isDVD = false;
            int idCounter = 0;
            while (!sr.EndOfStream) {
                String ln = sr.ReadLine();
                if (Regex.IsMatch(ln, @"group:\s+DVD")) {
                    isDVD = true;
                    ProductList.Add(new Product(idCounter));
                    idCounter++;
                }

                if (isDVD && Regex.IsMatch(ln,"cutomer:")) {
                    String[] parts = ln.Split(":");
                    String customerASIN = parts[1].Replace("rating", "");
                    customerASIN = customerASIN.Trim();
                    String rat = parts[2].Replace("votes", "");
                    rat.Trim();
                    int rating = Int32.Parse(rat);
                    ProductList[ProductList.Count - 1].addRate(customerASIN,rating);
                }
                
                if (Regex.IsMatch(ln, @"group:\s+(?!DVD)")) {
                    isDVD = false;
                }
            }
            
        }

        public void printProductList(List<Product> list) {
            foreach (var product in list) {
                Console.WriteLine(product.Id);
                foreach (var v in product.Ratings) {
                    Console.WriteLine("Customer ASIN:\t" + v.Key + "\tRating:\t"+v.Value);
                }
            }
        }
    }

    public class Product {
        private int id;

        public int Id => id;

        private Dictionary<String, int> ratings;

        public Dictionary<string, int> Ratings => ratings;

        public Product(int id) {
            this.id = id;
            ratings = new Dictionary<string, int>();
        }

        public void addRate(String ASIN, int Rating) {
            if (!ratings.ContainsKey(ASIN)) {
                ratings.Add(ASIN,Rating);
            }
        }
        
    }

}