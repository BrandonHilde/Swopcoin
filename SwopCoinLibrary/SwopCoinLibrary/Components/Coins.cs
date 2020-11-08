using System;
using System.Collections.Generic;
using System.Text;

namespace SwopCoinLibrary
{
    public class Coins

    {

        public Decimal MarketValue;



        public static Decimal MarketValueSum;



        public static List<Coins> GenericCoinList = new List<Coins>();



        public Coins(Decimal MarketValue)
        { 
            MarketValueSum += MarketValue;
            GenericCoinList.Add(this);
        }
    }



    public class SWOPCoins : Coins
    {
        public Decimal FaceValue;
        public static Decimal FaceValueSum;
        public static List<SWOPCoins> SWOPCoinList = new List<SWOPCoins>();

        public SWOPCoins(UInt64 FaceValue) : base(FaceValue)
        {
            FaceValueSum += FaceValue;

            SWOPCoinList.Add(this);
        }
    }

    public class USDCoins : Coins
    {
        public Decimal FaceValue;
        public const Decimal ParValue = 32;
        public static Decimal FaceValueSum;
        public static List<USDCoins> USDCoinList = new List<USDCoins>();

        public USDCoins(Decimal FaceValue) : base(FaceValue * ParValue)
        {
            FaceValueSum += FaceValue;
            USDCoinList.Add(this);
        }
    }

    public class BTCCoins : Coins
    {
        public Decimal FaceValue;
        public static Decimal FaceValueSum;
        public static List<BTCCoins> BTCCoinList = new List<BTCCoins>();
        
        public BTCCoins(Decimal FaceValue) : base(0)
        {
            FaceValueSum += FaceValue;
            BTCCoinList.Add(this);
        }
    }

    public class AuthorizedCoins : SWOPCoins
    {
        public Decimal SubscribedSum;
        public static Decimal AuthorizedSum;
        public static List<AuthorizedCoins> AuthorizedList = new List<AuthorizedCoins>();

        public AuthorizedCoins(Decimal FaceValue) : base(0)
        {
            AuthorizedSum += FaceValue;
            AuthorizedList.Add(this);
        }
    }

    public class SubscribedCoins : USDCoins
    {
        public static Decimal SubscribedSum;
        public static List<SubscribedCoins> SubscribedList = new List<SubscribedCoins>();

        public SubscribedCoins(AuthorizedCoins Authorization, Decimal USDValue, Decimal SWOPValue) : base(SWOPValue)
        {
            if ((Authorization.SubscribedSum + MarketValue) < Authorization.FaceValue)
            {
                SubscribedSum += USDValue;
                Authorization.SubscribedSum += MarketValue;
                SubscribedList.Add(this);
            }

            else
            {
                throw new Exception("Error: Subscribed SWOP exceeds Authorized SWOP");
            }
        }
    }

    public class ContributedCoins : BTCCoins
    {
        public static Decimal ContributedSum;
        public static List<ContributedCoins> ContributedList = new List<ContributedCoins>();

        public ContributedCoins(AuthorizedCoins Authorization, Decimal BTCValue, Decimal SWOPValue) : base(SWOPValue)
        {
            if (SWOPValue == Authorization.FaceValue)
            {
                ContributedSum += BTCValue;
                Authorization.SubscribedSum += MarketValue;

                ContributedList.Add(this);
            }
            else
            {
                throw new Exception("Error: The Contributed SWOP is not equal to the Authorized SWOP");
            }
        }
    }

    public class IssuedCoins : BTCCoins
    {
        public static Decimal IssuedSum;

        public static List<ContributedCoins> IssuedList = new List<ContributedCoins>();

        public IssuedCoins(ContributedCoins Contribution, SubscribedCoins Subscription) : base(Contribution.FaceValue / 2)
        {
            /*
            if (SWOPValue == Authorization.FaceValue)
            {
                ContributedSum += BTCValue;
                Authorization.SubscribedSum += MarketValue;

                IssuedList.Add(this);
            }
            else
            {
                throw new Exception("Error: The Issue SWOP is not equal to the Authorized SWOP");
            }*/
        }
    }
}
