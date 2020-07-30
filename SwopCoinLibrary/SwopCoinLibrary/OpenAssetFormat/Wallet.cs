using System;
using System.Collections.Generic;
using System.Text;

namespace SwopCoinLibrary.Wallet
{
    public enum SupportedCryptocurrency { BTC }
    public class Wallet
    {
        public string WalletName { get; set; }
        public List<CryptoBalance> Balances = new List<CryptoBalance>();

        List<DisplayBalance> displayInfo { get; set; }

        public List<DisplayBalance> GetDisplay()
        {
            displayInfo = new List<DisplayBalance>();

            foreach(CryptoBalance cb in Balances)
            {
                displayInfo.Add(cb.GetDisplay());
            }

            return displayInfo;
        }
    }

    public class DisplayBalance
    {
        public string Symbol { get; set; }
        public string Amount { get; set; }
    }

    public class CryptoBalance
    {
        public string Symbol { get; set; }
        public SupportedCryptocurrency CryptoType { get; set; }
        public double Amount = 0;

        public DisplayBalance GetDisplay()
        {
            return new DisplayBalance { Amount = this.Amount.ToString(), Symbol = this.Symbol };
        }
    }
}
