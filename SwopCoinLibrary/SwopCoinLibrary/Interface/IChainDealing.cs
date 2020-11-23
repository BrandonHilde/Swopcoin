using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;

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
        void Subscribe(string Email, Coins coin);

        // Issues SWOP coin into a crypto coin blockchain
        // Sends colored coin to new address
        void Issue(BitcoinAddress Address, Coins coin);

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
}
