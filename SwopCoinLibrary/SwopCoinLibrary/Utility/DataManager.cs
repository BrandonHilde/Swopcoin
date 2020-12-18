using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SwopCoinLibrary.Components;
using System.Xml;
using NBitcoin;
using System.Xml.Serialization;

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
        
        public bool SaveIssuanceCoin(IssuanceCoin coin, string CoinName)
        {
            XmlIssuanceCoin xmlcoin = new XmlIssuanceCoin();

            xmlcoin.Coin = coin;
            xmlcoin.Name = CoinName;

            string serial = xmlcoin.Serialize();

            File.WriteAllText(CoinName + ".CoinID", serial);

            return File.Exists(CoinName + ".CoinID");
        }

        public IssuanceCoin LoadCoin(string CoinName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(XmlIssuanceCoin));

            XmlIssuanceCoin xmlcoin = null;

            using (Stream reader = new FileStream(CoinName + ".CoinID", FileMode.Open))
            {
                xmlcoin = (XmlIssuanceCoin)serializer.Deserialize(reader);
            }

            if(xmlcoin != null)
                return xmlcoin.Coin;
            else
            {
                return null;
            }
        }

        // public bool SaveXml()
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

    public class XmlIssuanceCoin
    {
        public string Name { get; set; }
        public IssuanceCoin Coin { get; set; }
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

    public static class ExtendUtility
    {
        public static string Serialize<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            try
            {
                var xmlserializer = new XmlSerializer(typeof(T));
                XmlWriterSettings wSet = new XmlWriterSettings { Indent = true };
                var stringWriter = new StringWriter();
                //xw.WriteDocType("profile", null, "criteria_profile.dtd", null);
                using (var writer = XmlWriter.Create(stringWriter, wSet))
                {
                    xmlserializer.Serialize(writer, value);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }
    }
}
