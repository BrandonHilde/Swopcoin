using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SwopCoinLibrary.Components;
using System.Xml;
using NBitcoin;

namespace SwopCoinLibrary.Utility
{
    public class DataManager
    {
        private static string AdminFile = "Admin.swop";
        public List<Subscriber> GetSubscribers()
        {
            throw new NotImplementedException();
        }

        public bool SaveSubscriber(Subscriber sub)
        {
            throw new NotImplementedException();
        }

        public BitcoinSecret LoadAdmin(Network net)
        {
            // replace with other load methods later
            if (File.Exists(AdminFile))
            {
                byte[] bt = File.ReadAllBytes(AdminFile);

                Key k = new Key(bt);
                
                BitcoinSecret sec = k.GetBitcoinSecret(net);

                return sec;
            }
            else
            {
               return CreateAdmin(net);
            }
        }

        private BitcoinSecret CreateAdmin(Network net)
        {
            Key k = new Key();

            //replace later with better version
            File.WriteAllBytes(AdminFile, k.ToBytes()); // might need version

            BitcoinSecret sec = k.GetBitcoinSecret(net);
            return sec;
        }
    }

    public class Preferences
    {
        public string FolderLocation { get; set; }
    }

    public class XmlDataFile
    {
        Account ActiveAccount { get; set; }
        List<XmlSubscribers> Subscriptions { get; set; }
    }

    public class XmlSubscribers
    {
        public enum SubscriberType { Investor, User }

        public SubscriberType Type { get; set; }
        List<Subscriber> ActiveSubscribers { get; set; }
    }
}
