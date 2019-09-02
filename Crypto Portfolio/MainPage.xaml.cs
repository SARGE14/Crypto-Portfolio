using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using LiteDB;

namespace Crypto_Portfolio
{
    public class Coin
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public string Price { get; set; }
        public string Amount { get; set; }
    }
    public sealed partial class MainPage : Page
    {
        public List<Coin> Coins { get; set; }
        private string filename { get { return " credentials"; } }
        private string DBName { get { return " myData.db"; } }

        public MainPage()
        {
            this.InitializeComponent();
            Coins = new List<Coin>();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Coins = new List<Coin>
               {
                       new Coin
                       {
                           Name = "XRP", Count = 2000, Price = "0,00002606", Amount = "1"
                       },
                       new Coin
                       {
                           Name = "EMC", Count = 250, Price = "0,00001006", Amount = "1"
                       },
                       new Coin
                       {
                           Name = "PLBT", Count = 8000, Price = "0,00015606", Amount = "1"
                       }
               };
            var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var folderPath = localFolder.Path;
            var filePath = Path.Combine(folderPath, this.DBName);
            using (var db = new LiteDatabase(filePath))
            {
                // Get a collection (or create, if doesn't exist)
                var col = db.GetCollection<Coin>("coins");

                //   var coins = new List<Coin>();
                var coin = Coins;
                /*   var coin = new Coin
                   {
                       Rank = 1,
                       Name = "XRP",
                       Count = 2000,
                       Price = "0,00002606",
                       Amount = "1"
                   };*/


                // col.Delete(coin);
                /*    for (int i = 1; i < 3; i++)
                    {
                        col.Delete(i);
                    }*/
                col.Insert(coin);

                // Index document using document Name property
                //   col.EnsureIndex(x => x.Name);

                // Use LINQ to query documents

                // var results = col.Find(x => x.Name.StartsWith("EMC"));
                var results = col.FindById(2);
                if (results != null)
                {
                    text.Text = results.Amount;
                }

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var folderPath = localFolder.Path;
            var filePath = Path.Combine(folderPath, this.DBName);
            using (var db = new LiteDatabase(filePath))
            {
                // Get a collection (or create, if doesn't exist)
                var col = db.GetCollection<Coin>("coins");
                // var coin = Coins;
                Coins = new List<Coin>();
                for (int i = 1; i <= 3; i++)
                {
                    var results = col.FindById(i);
                    Coins.Add(results);
                }
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            double result;
            for (int i = 0; i < Coins.Count; i++)
            {
                result = Coins[i].Count * double.Parse(Coins[i].Price);
                Coins[i].Amount = result.ToString("F8");
                //  Coins[i].Amount
                //  text.Text += Coins[i].Amount;
            }
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = Coins;
            var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var folderPath = localFolder.Path;
            var filePath = Path.Combine(folderPath, this.DBName);
            using (var db = new LiteDatabase(filePath))
            {
                // Get a collection (or create, if doesn't exist)
                var col = db.GetCollection<Coin>("coins");
                var coin = Coins;

                col.Update(coin);
                var results = col.FindById(2);
                if (results != null)
                {
                    text.Text = results.Amount;
                }
            }
        }
    }
}
