using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ALS {
    public class Parser {
        public List<Product> ProductList;
        private StreamReader sr;
        public List<Product> mostRatedProducts;
        private List<string> asinList;

        public Parser() {
            sr = new StreamReader("../../../files/amazon.txt");
            ProductList = new List<Product>();
            parse();
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
            int productsWithNoReviews = 0;
            foreach (var product in list) {
                Console.WriteLine(product.id);
                foreach (var v in product.ratings) {
                    Console.WriteLine("Customer ASIN:\t" + v.Key + "\tRating:\t"+v.Value);
                }

                if (product.ratings.Count == 0) {
                    productsWithNoReviews++;
                }
            }
            Console.WriteLine("YO JEST " + productsWithNoReviews +" PRODUKTOW BEZ ZADNYCH OCEN");
        }
        
        public void takeMostRatedProducts(int amount) { //Bierzemy produkty z najwieksza iloscia ocen
            mostRatedProducts = ProductList.OrderByDescending(o=>o.ratings.Count).ToList();
            mostRatedProducts.RemoveRange(amount, mostRatedProducts.Count - amount);
        }
        public void takeMostActiveUsers(int amount) { //Bierzemy osoby które oceniły najwiecej produktów z listy powyżej (dictionary)
            Dictionary<String, int> mostActiveUsers = new Dictionary<string, int>();
            foreach (var product in mostRatedProducts) {
                foreach (var rating in product.ratings) {
                    if (mostActiveUsers.ContainsKey(rating.Key)) mostActiveUsers[rating.Key]++;
                    else mostActiveUsers.Add(rating.Key,1);
                }
            }

            mostActiveUsers = mostActiveUsers.OrderByDescending(o => o.Value).ToDictionary(pair => pair.Key, pair => pair.Value); //Sortowanie
            asinList = mostActiveUsers.Keys.ToList(); //Tworzenie listy ASIN
            asinList.RemoveRange(amount, asinList.Count - amount); // USUWANIE LISTY
            removeUnneededUsers();
            Console.WriteLine(asinList.Count);
            asignIDsToUsers();
        }

        private void removeUnneededUsers() {
            
            foreach (var product in mostRatedProducts) {
                var keysToRemove = product.ratings.Keys.Except(asinList).ToList();
                foreach (var key in keysToRemove) product.ratings.Remove(key);
            }
            
        }
        private void asignIDsToUsers(){
            for(int i = 0; i < asinList.Count; i++){
                foreach (var product in mostRatedProducts){
                    if (product.ratings.ContainsKey(asinList[i])){
                        product.ratings.Add(i.ToString(),product.ratings[asinList[i]]);
                        product.ratings.Remove(asinList[i]);
                    }
                }
            }
        }

        public Matrix returnMatrix() {
            Matrix Rmatrix = new Matrix(asinList.Count,mostRatedProducts.Count);
            Rmatrix.FillWithZeros();
            int KnownReviews = 0;
            for (int i = 0; i < mostRatedProducts.Count; i++) {
                foreach (var rating in mostRatedProducts[i].ratings) {
                    Rmatrix.Data[Int32.Parse(rating.Key), i] = rating.Value;
                    KnownReviews++;
                }
            }
            
            Console.WriteLine("Macierz jest zapełniona w " + KnownReviews * 100f/(asinList.Count * mostRatedProducts.Count) + "%");
            return Rmatrix;
        }
    }
    public class Product {
        
        public int id;
        public Dictionary<String, int> ratings;
        
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