using System;
using System.Collections.Generic;
using System.Text;

namespace SwopCoinLibrary.Interface
{
    public interface IChainDealing : IDisposable
    {
        // Contribute the first half private issue SWOP coin
        // authorize -> issue
        void Contribute(Coins coin);

        // Authorize the public offering in 4 phases the angel, pre-seed, seed and initial SWOP coin offerings 
        // Generate colored coins in organization's control
        void Authorize(Coins coin);

        // Subscribions for the second half public issue SWOP coin 
        // Record amount owed to person
        void Subscribe(Coins coin);

        // Issues SWOP coin into a crypto coin blockchain
        // Sends colored coin to new address
        void Issue(Coins coin);

        // Remove SWOP coin from circulation
        void Uncirculate(Coins coin);

        // Add SWOP coin to circulation
        void Circulate(Coins coin);

        void New(Orders order);

        void Modify(Orders order);

        void Delete(Orders order);

        void Review(Orders order);

        void Approve(Orders order);

        void Dispatch(Orders order);

        void Endorse(Orders order);

        void Archive(Orders order);

        void Browse(Orders order);

        void Search(Orders order);
    }

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

        public void Issue(Coins coin) { throw new NotImplementedException(); }

        public void Subscribe(Coins coin) { throw new NotImplementedException(); }

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
