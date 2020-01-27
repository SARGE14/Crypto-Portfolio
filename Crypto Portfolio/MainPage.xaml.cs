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
using System.Net;
using System.Web;
using LiteDB;
using Newtonsoft.Json;

namespace Crypto_Portfolio
{
    public class Coin
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public string Price { get; set; }
        public string Amount { get; set; }
        public string curPrice { get; set; }
        public string Profit { get; set; }
        public string curPriceUsd { get; set; }
        public string profitUsd { get; set; }
        public string Margin { get; set; }
    }
    public sealed partial class MainPage : Page
    {
        readonly WebClient webClient;
        public List<Coin> Coins { get; set; }
        private string filename { get { return " credentials"; } }
        private string DBName { get { return " myData.db"; } }
        public string url;
        public string exchange;
        public string currencyPair;
        public string priceBidAsk;
        public bool errorLivecoin;
        public bool errorHitBtc;
        public bool errorYobit;
        public double newPrice;
        double btc;
        double sumUsd = 0;
        double sumBtc = 0;
        double total = 0;
        double totalMargin;
        bool updateTotal;
        RootObject json;
        public class RootObject
        {
            /*  public double last { get; set; }
              public double high { get; set; }
              public double low { get; set; }
              public double volume { get; set; }
              public double vwap { get; set; }
              public double max_bid { get; set; }
              public double min_ask { get; set; }*/
           // public double best_bid { get; set; }
           // public double best_ask { get; set; }
            public double ask { get; set; }
            public double bid { get; set; }
            public double highestBid { get; set; }
            public double lowestAsk { get; set; }
        }

        public MainPage()
        {
            this.InitializeComponent();
            /*    if (Coins != null)
                {
                    Coins = new List<Coin>();
                }*/
            webClient = new WebClient();
            var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var folderPath = localFolder.Path;
            var filePath = Path.Combine(folderPath, this.DBName);
            using (var db = new LiteDatabase(filePath))
            {
                var col = db.GetCollection<Coin>("coins");
                Coins = new List<Coin>();
                for (int i = 1; i <= col.Count(); i++)
                {
                    var results = col.FindById(i);
                    Coins.Add(results);
                }
            }
                //   dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = Coins;
           

        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
              Coins = new List<Coin>
              {
                         new Coin
                         {
                             Name = "XRP", Count = 2000, Price = "0,00002606", Amount = "1"
                         }
              };
            //   Coins.Add(new Coin { Id = count+1, Name = "XRPs", Count = 2000, Price = "0,00002606", Amount = "1"});

            var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var folderPath = localFolder.Path;
            var filePath = Path.Combine(folderPath, this.DBName);
            using (var db = new LiteDatabase(filePath))
            {
                var col = db.GetCollection<Coin>("coins");
                var coin = Coins;
                col.Insert(coin);
                Coins = new List<Coin>();
                for (int i = 1; i <= col.Count(); i++)
                {
                    var results = col.FindById(i);
                    Coins.Add(results);
                }
                var result = col.FindById(2);
                if (result != null)
                {
                    text.Text = result.Amount;
                }
            }
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = Coins;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            double result;
            for (int i = 0; i < Coins.Count; i++)
            {
                result = Coins[i].Count * double.Parse(Coins[i].Price);
                Coins[i].Amount = result.ToString("F8");
            }
            updateDbDg();
        }
        private void WebTest()
        {
            using (WebClient wc = webClient)
            {
                string webPage = null;

                try
                {
                    webPage = wc.DownloadString(@url);
                    if (exchange == "HitBTC")
                    {
                        errorHitBtc = false;
                    }
                    json = null;
                    json = JsonConvert.DeserializeObject<RootObject>(webPage);


                }
                catch (WebException)
                {
               
                    if (exchange == "HitBTC")
                    {
                        errorHitBtc = true;
                        text.Text = "API HitBTC недоступно" + url ;
                    }
                  
                    url = null;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string urlExchange;
            double result;
            /*HitBTC*/
            exchange = "HitBTC";
            urlExchange = "https://api.hitbtc.com/api/2/public/ticker/";
            currencyPair = "BTCUSD";
            url = urlExchange + currencyPair;
            WebTest();
            newPrice = (json.ask + json.bid) / 2;
            btc = newPrice;
            for (int i = 0; i < Coins.Count; i++)
            {
                if (Coins[i].Name != "LEO" && Coins[i].Name != "CVC")
                {
                    currencyPair = Coins[i].Name + "BTC";
                    url = urlExchange + currencyPair;
                    WebTest();
                    newPrice = (json.ask + json.bid) / 2;
                    Coins[i].curPrice = newPrice.ToString("F8");
                }
                else
                    if (Coins[i].Name == "LEO")
                {
                    string urlExchangeLeo = "https://data.gateio.co/api2/1/ticker/";
                    currencyPair = Coins[i].Name + "_BTC";
                    url = urlExchangeLeo + currencyPair.ToLower();
                    WebTest();
                    newPrice = (json.lowestAsk + json.highestBid) / 2;
                    Coins[i].curPrice = newPrice.ToString("F8");
                }
                else
                    if (Coins[i].Name == "CVC")
                {

                    currencyPair = Coins[i].Name + "USD";
                    url = urlExchange + currencyPair;
                    WebTest();
                    newPrice = (json.ask + json.bid) / 2 / btc;
                    Coins[i].curPrice = newPrice.ToString("F8");
                }
                result = Coins[i].Count * double.Parse(Coins[i].curPrice) - double.Parse(Coins[i].Amount);
                sumBtc += result;
                Coins[i].Profit = result.ToString("F8");

                result = (double.Parse(Coins[i].curPrice) / double.Parse(Coins[i].Price)-1)*100;
                Coins[i].Margin = result.ToString("F2") + "%";

                result = double.Parse(Coins[i].curPrice) * btc;
                Coins[i].curPriceUsd = result.ToString("F2") + " USD";

                result = double.Parse(Coins[i].Profit) * btc;
                sumUsd += result;
                Coins[i].profitUsd= result.ToString("F2") + " USD";

                total += double.Parse(Coins[i].Amount);

                totalMargin = (sumBtc / total)*100;
                updateTotal = true;

            }
            updateDbDg();
        }
        public void updateDbDg()
        {
            var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var folderPath = localFolder.Path;
            var filePath = Path.Combine(folderPath, this.DBName);
            using (var db = new LiteDatabase(filePath))
            {
                var col = db.GetCollection<Coin>("coins");
                var coin = Coins;
                col.Update(coin);
            }
            if (total !=0 && updateTotal)
            {
                Coins.Add(new Coin {Name = "Суммы", profitUsd = sumUsd.ToString("F2")+ " USD", Profit = sumBtc.ToString("F8"), Amount = total.ToString("F8"), Margin = totalMargin.ToString("F2") + "%" });
                updateTotal = false;
            }
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = Coins;
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            Coins.RemoveAt(Coins.Count-1);
        }
    }
}
