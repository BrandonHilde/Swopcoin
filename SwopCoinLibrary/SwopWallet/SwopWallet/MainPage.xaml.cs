using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using Xamarin.Forms.StyleSheets;
using SwopCoinLibrary.Wallet;

namespace SwopWallet
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
   // [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            Wallet SimulatedWallet = new Wallet();

            SimulatedWallet.WalletName = "test wallet";

            SimulatedWallet.Balances.Add(new CryptoBalance { Symbol = "BTC", Amount = 0.314 });
            SimulatedWallet.Balances.Add(new CryptoBalance { Symbol = "ETH", Amount = 0.888 });
            SimulatedWallet.Balances.Add(new CryptoBalance {Symbol = "LTC",  Amount = 4.55 });
            SimulatedWallet.Balances.Add(new CryptoBalance {Symbol = "XMR",  Amount = 0.12 });

            curencyList.ItemsSource = SimulatedWallet.GetDisplay();
        }
    }
}
