using System;
using System.Collections.Generic;
using System.Text;

namespace SwopCoinLibrary.Components
{
    public class Subscriber
    {
        public string Email { get; set; }
        public SubscribedCoins Coin { get; set; }
        public DateTime RedemptionEmailTimeStamp { get; set; }

        private double SendFrequency = 1024;
        public Subscriber(string email, SubscribedCoins coins)
        {
            Email = email;
            Coin = coins;
        }

        public bool SendRedemptionEmail()
        {
            if (RedemptionEmailTimeStamp != null)
            {
                if((DateTime.Now - RedemptionEmailTimeStamp).TotalSeconds < SendFrequency) // limit number of emails by time
                {
                    RedemptionEmailTimeStamp = DateTime.Now;
                }
            }
            else
            {
                RedemptionEmailTimeStamp = DateTime.Now;
            }
            throw new NotImplementedException();
        }

        public bool SaveSubscription()
        {
            throw new NotImplementedException();
        }
    }
}
