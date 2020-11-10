using System;
using System.Collections.Generic;
using System.Text;

namespace SwopCoinLibrary
{
    public class Orders
    {
        public enum OrderType { Market, Limit, Stop};
        public enum OrderAction { Buy, Sell };

        public DateTime TimeStamp { get; set; }
        public OrderType Type { get; set; }
        public OrderAction Action { get; set; }

        public Coins OrderCoins { get; set; }
        
        public Orders(Coins Coin, OrderType oType = OrderType.Market, OrderAction oAction = OrderAction.Buy)
        {
            SetTimeStamp();
            OrderCoins = Coin;
            Type = oType;
            Action = oAction;
        }
        private void SetTimeStamp()
        {
            TimeStamp = DateTime.Now;
        }
    }
}
