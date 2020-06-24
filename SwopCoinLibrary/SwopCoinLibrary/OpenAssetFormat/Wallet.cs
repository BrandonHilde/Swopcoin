using System;
using System.Collections.Generic;
using System.Text;

namespace SwopCoinLibrary.OpenAssetFormat
{
    public enum SupportedCryptocurrency { BTC }
    public class Wallet
    {
        public string WalletName { get; set; }
        public List<CryptoBalance> Balances = new List<CryptoBalance>();
    }

    public class CryptoBalance
    {
        public SupportedCryptocurrency CryptoType { get; set; }
        public double Balance = 0;
    }
}
