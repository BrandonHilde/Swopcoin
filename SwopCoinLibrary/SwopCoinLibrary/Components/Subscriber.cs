using System;
using System.Collections.Generic;
using System.Text;

namespace SwopCoinLibrary.Components
{
    public class Subscriber
    {
        public string Email { get; set; }
        public SubscribedCoins Coin { get; set; }
        public Subscriber(string email, SubscribedCoins coins)
        {
            Email = email;
            Coin = coins;
        }

        public bool SendRedemptionEmail()
        {
            throw new NotImplementedException();
        }

        public bool SaveSubscription()
        {
            throw new NotImplementedException();
        }
    }
}
