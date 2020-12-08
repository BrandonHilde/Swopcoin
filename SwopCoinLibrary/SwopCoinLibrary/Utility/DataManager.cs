using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SwopCoinLibrary.Components;
using System.Xml;

namespace SwopCoinLibrary.Utility
{
    public class DataManager
    {
        public List<Subscriber> GetSubscribers()
        {
            throw new NotImplementedException();
        }
        
        public bool SaveSubscriber(Subscriber sub)
        {
            throw new NotImplementedException();
        }
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
