using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;
using NBitcoin.Protocol;

namespace SwopCoinLibrary.Components
{
    public class Account
    {
        public enum Role { Admin, User, Investor }
        public Network Net { get; set; }
        public List<Node> ActiveNodes { get; set; }

        public BitcoinSecret Secret { get; set; }

        public string SaveFileLocation { get; set; }
        //class for detailing the active users settings and info
    }
}
