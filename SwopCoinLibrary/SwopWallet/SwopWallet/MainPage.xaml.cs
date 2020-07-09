using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using Xamarin.Forms.StyleSheets;

namespace SwopWallet
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            List<Currency> cryptos = new List<Currency>();
            cryptos.Add(new Currency { Amount = 0.314, Symbol = "BTC" });
            cryptos.Add(new Currency { Amount = 2.14, Symbol = "ETH" });
            cryptos.Add(new Currency { Amount = 1.3, Symbol = "LTC" });
            cryptos.Add(new Currency { Amount = 3.54, Symbol = "SWOP" });
            curencyList.ItemsSource = cryptos;
        }
    }

    public class Currency
    {
        public string Symbol { get; set; }
        public double Amount { get; set; }
    }
}
