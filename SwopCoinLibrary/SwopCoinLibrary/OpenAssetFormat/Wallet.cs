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
    }

    public class CryptoBalance
    {
        public string Symbol { get; set; }
        public SupportedCryptocurrency CryptoType { get; set; }
        public double Balance = 0;
    }
}
