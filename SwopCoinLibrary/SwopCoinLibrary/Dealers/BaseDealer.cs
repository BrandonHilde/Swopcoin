using System;
using System.Collections.Generic;
using System.Text;
using SwopCoinLibrary.Interface;
using SwopCoinLibrary.Components;
using NBitcoin;

namespace SwopCoinLibrary.Dealers
{
    public class BaseDealer : IChainDealing
    {
        // Blockchain Coins

        public Coins[] CirculatedCoins;

        public Coins[] UncirculatedCoins;

        public Coins[] IssuedCoins;

        public Coins[] SubscribedCoins;

        public Coins[] AuthorizedCoins;

        public Coins[] ContributedCoins;

        public void Authorize(Coins coin)
        {
            // two line example below used to call this method (copy to test code to call this method)
            coin = new AuthorizedCoins(25600000);
            Authorize(coin);
        }

        public void Contribute(Coins coin) { throw new NotImplementedException(); }

        public void Issue(BitcoinAddress Address, Coins coin) 
        { 
             
        }

        public void Subscribe(string email, Coins coin) 
        {
            Subscriber sub = new Subscriber(email, (SubscribedCoins)coin);
            sub.SaveSubscription();
        }

        public void Uncirculate(Coins coin) { throw new NotImplementedException(); }

        public void Circulate(Coins coin) { throw new NotImplementedException(); }

        // Blockchain Orders
        public Orders[] NewOrders;
        public Orders[] ModifiedOrders;
        public Orders[] DeletedOrders;
        public Orders[] ReviewedOrders;
        public Orders[] ApprovedOrders;
        public Orders[] DispachedOrders;
        public Orders[] EndorsedOrders;
        public Orders[] ArchivedOrders;

        public void Approve(Orders order) { throw new NotImplementedException(); }

        public void Archive(Orders order) { throw new NotImplementedException(); }

        public void Browse(Orders order) { throw new NotImplementedException(); }

        public void Delete(Orders order) { throw new NotImplementedException(); }

        public void Dispatch(Orders order) { throw new NotImplementedException(); }

        public void Dispose() { throw new NotImplementedException(); }

        public void Endorse(Orders order) { throw new NotImplementedException(); }

        public void Modify(Orders order) { throw new NotImplementedException(); }

        public void New(Orders order) { throw new NotImplementedException(); }

        public void Review(Orders order) { throw new NotImplementedException(); }

        public void Search(Orders order) { throw new NotImplementedException(); }

        public Orders Start() { throw new NotImplementedException(); }

    }
}
