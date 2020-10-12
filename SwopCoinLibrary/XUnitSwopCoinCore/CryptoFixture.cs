using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using SwopCoinLibrary;
using SwopCoinLibrary.Node;
using NBitcoin.Protocol;
using NBitcoin.Tests;
using NBitcoin;
using System.Diagnostics;

namespace XUnitSwopCoinCore
{
    public class CryptoFixture : IDisposable
    {
        public BtcNodeCreate create = new BtcNodeCreate();
        public CryptoFixture()
        {
            ActionStatus res = create.SetUpBuilder(NodeDownloadData.Bitcoin.v0_18_0, Network.RegTest);

            if (res.Success)
            {
                ActionStatus resN = create.CreateNodeSet(3);

                if(resN.Success)
                {
                    create.NameNextNode("miner");
                    create.NameNextNode("alice");
                    create.NameNextNode("bob");

                    if(create.GetByName("miner") != null)
                    {
                        List<uint256> list = create.StartNetwork();

                        bool trigger = false;

                        if (list != null)
                        {
                            if (list.Count > 1)
                            {
                                for (int i = 1; i < list.Count; i++)
                                {
                                    if(list[i] != list[i - 1])
                                    {
                                        trigger = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if(!trigger)
                        {
                            create.AddAddresses();
                           // Trace.WriteLine("Ready");
                        }
                        else
                        {
                            //Trace.WriteLine("Failed Setup");
                        }
                    }
                    else
                    {
                        //Trace.WriteLine("Failed Setup");
                    }
                }
                else
                {
                    //Trace.WriteLine("Failed Setup");
                }
            }
            else
            {
                //Trace.WriteLine("Failed Setup");
            }
        }

        public void Dispose()
        {
            create.EndNetwork();
            create = null;
            // ... clean up test data ...
        }
    }

    [Collection("Sequential")]
    public class TestCryptoFixture : IClassFixture<CryptoFixture>
    {
        CryptoFixture fixture;

        public TestCryptoFixture(CryptoFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void SendBitcoin()
        {
            ActionStatus resM = 
                fixture.create.MineNetwork(fixture.create.GetByName("miner"), 50);

            Assert.True(resM.Success);

            ActionStatus resS = fixture.create.SendBitcoin(
                fixture.create.GetByName("miner"),
                Money.Coins(10m),
                fixture.create.GetByName("alice"));

            Assert.True(resS.Success);
        }

        [Fact]
        public void MineBitcoin()
        {
            ActionStatus resM = 
                fixture.create.MineNetwork(fixture.create.GetByName("miner"), 1);

            Assert.True(resM.Success);
        }

        [Fact]
        public void SyncNetwork()
        {
            ActionStatus resM = 
                fixture.create.MineNetwork(fixture.create.GetByName("miner"), 1);

            Assert.True(resM.Success);

            ActionStatus resSync = 
                fixture.create.SyncNodes(
                    fixture.create.GetByName("alice"), 
                    fixture.create.GetByName("miner"));

            Assert.True(resSync.Success);
        }

    }
}


/*
 * 
 * 
 * 
 * \
 * 
 * 
 * BtcNodeCreate create = new BtcNodeCreate();

            BtcNodeCreate.ActionStatus res = create.SetUpBuilder(NodeDownloadData.Bitcoin.v0_18_0, Network.RegTest);

            BtcNodeCreate.ActionStatus resN = create.CreateNodeSet(3);

            Assert.True(res.Success);
            Assert.True(resN.Success);

            create.NameNextNode("miner");
            create.NameNextNode("alice");

            Assert.NotNull(create.GetByName("miner"));
            Assert.NotNull(create.GetByName("alice"));

            List<uint256> list = create.StartNetwork();

            if (list != null)
            {
                if (list.Count > 1)
                {
                    for (int i = 1; i < list.Count; i++)
                    {
                        Assert.True(list[i] != list[i - 1]);
                    }
                }
            }
*/