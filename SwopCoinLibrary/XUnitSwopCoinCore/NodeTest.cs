using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using SwopCoinLibrary;
using SwopCoinLibrary.Node;
using NBitcoin.Protocol;
using NBitcoin.Tests;
using NBitcoin;
using Xunit.Sdk;
using System.Diagnostics;

namespace XUnitSwopCoinCore
{

    public class NodeFixture : IDisposable
    {
        public BtcNodeCreate create = new BtcNodeCreate();
        public NodeFixture()
        {
            BtcNodeCreate.ActionStatus res = create.SetUpBuilder(NodeDownloadData.Bitcoin.v0_18_0, Network.RegTest);
        }
        public void Dispose()
        {
            Process[] p = Process.GetProcessesByName("bitcoind.exe");

            if (p != null)
            {
                if(p.Length > 0)
                {
                    foreach(Process pr in p)
                    {
                        pr.Kill();
                    }
                }
            }

            create.EndNetwork();
            create = null;
            // ... clean up test data ...
        }
    }

    [Collection("Sequential")]
    public class NodeTest : IClassFixture<NodeFixture>
    {
        NodeFixture fixture;

        public NodeTest(NodeFixture fixture)
        {
            this.fixture = fixture;
        }
  
        [Fact]
        public void SetupNetwork()
        {
            BtcNodeCreate create = new BtcNodeCreate();

            BtcNodeCreate.ActionStatus res = create.SetUpBuilder(NodeDownloadData.Bitcoin.v0_18_0, Network.RegTest);

            Assert.True(res.Success);
        }

        [Fact] 
        public void CreateNodes()
        {
            BtcNodeCreate.ActionStatus resN = fixture.create.CreateNodeSet(3);

            Assert.True(resN.Success);
        }

        [Theory]
        [InlineData("miner")]
        [InlineData("alice")]
        [InlineData("bob")]
        public void NameNodes(string name)
        {
            BtcNodeCreate.ActionStatus resN = fixture.create.CreateNodeSet(3);

            Assert.True(resN.Success);

            fixture.create.NameNextNode(name);

            Assert.NotNull(fixture.create.GetByName(name));
        }

        [Fact]
        public void NetworkCreate()
        {


            BtcNodeCreate.ActionStatus resN = fixture.create.CreateNodeSet(3);

            if (resN.Success)
            {
                fixture.create.NameNextNode("miner");

                if (fixture.create.GetByName("miner") != null)
                {
                    List<uint256> list = fixture.create.StartNetwork();

                    if (list != null)
                    {
                        if (list.Count > 1)
                        {
                            for (int i = 1; i < list.Count; i++)
                            {
                                Assert.True(list[i] == list[i - 1]);
                            }
                        }
                    }
                }
            }
        }

        [Fact]
        public void CreateAddresses()
        {
            BtcNodeCreate.ActionStatus res = fixture.create.AddAddresses();

            Assert.True(res.Success);
        }
       
        /* [Fact]
       public void SendBitcoin()
        {
            BtcNodeCreate create = new BtcNodeCreate();

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

            BtcNodeCreate.ActionStatus resM = create.MineNetwork(create.GetByName("miner"), 1);

            Assert.True(resM.Success);

            BtcNodeCreate.ActionStatus resS = create.SendBitcoin(
                create.GetByName("miner"), 
                Money.Coins(20m), 
                create.GetByName("alice"));

            Assert.True(resS.Success);
        }

        [Fact]
        public void MineBitcoin()
        {
            BtcNodeCreate create = new BtcNodeCreate();

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

            BtcNodeCreate.ActionStatus resM = create.MineNetwork(create.GetByName("miner"), 1);

            Assert.True(resM.Success);
        }

        [Fact]
        public void SyncNode()
        {
            BtcNodeCreate create = new BtcNodeCreate();

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

            BtcNodeCreate.ActionStatus resM = create.MineNetwork(create.GetByName("miner"), 1);

            Assert.True(resM.Success);

            BtcNodeCreate.ActionStatus resSync = create.SyncNodes(create.GetByName("alice"), create.GetByName("miner"));

            Assert.True(resSync.Success);
        }*/
    }

    [Collection("Sequential")]
    public class SecondTestSet
    {

    }
}
