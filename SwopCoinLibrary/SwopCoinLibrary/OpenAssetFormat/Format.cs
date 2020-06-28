using System;
using System.Collections.Generic;
using System.Text;

namespace SwopCoinLibrary.OpenAssetFormat
{
    public enum AddressType { BTC, OAF }
    public class Format
    {
        
    }

    public class Address
    {
        AddressType AddressType { get; set; }

        public byte[] AddressBytes { get; set; }

        public byte[] CheckSum
        {
            get { return CalculateCheckSum(AddressBytes); }
        }

        public byte[] CalculateCheckSum(byte[] Formated)
        {
            return null;// fill later with code
        }
    }
}
