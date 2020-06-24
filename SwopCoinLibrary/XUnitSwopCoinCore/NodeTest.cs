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

namespace XUnitSwopCoinCore
{
    public class NodeTest
    {
        BtcNodeCreate create = new BtcNodeCreate();

        [Fact]
        public void SetupNetwork()
        {
            BtcNodeCreate.ActionStatus res = create.SetUpBuilder(NodeDownloadData.Bitcoin.v0_18_0, Network.RegTest);

            Assert.True(res.Success);
        }

        [Fact] 
        public void CreateNodes()
        {
            BtcNodeCreate.ActionStatus res = create.CreateNodeSet(3);

            Assert.True(res.Success);
        }

        [Fact]
        public void CreateAddresses()
        {
            BtcNodeCreate.ActionStatus res = create.AddAddresses();

            Assert.True(res.Success);
        }

        [Fact]
        public void NetworkCreate()
        {
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
        }

        [Theory]
        [InlineData("miner")]
        [InlineData("alice")]
        [InlineData("bob")]
        public void NameNodes(string name)
        {
            create.NameNextNode(name);

            Assert.NotNull(create.GetByName(name));
        }

        [Fact]
        public void SendBitcoin()
        {
            BtcNodeCreate.ActionStatus res = create.SendBitcoin(
                create.GetByName("miner"), 
                Money.Coins(20m), 
                create.GetByName("alice"));

            Assert.True(res.Success);
        }

        [Fact]
        public void MineBitcoin()
        {
            BtcNodeCreate.ActionStatus res = create.MineNetwork(create.GetByName("miner"), 1);

            Assert.True(res.Success);
        }

        [Fact]
        public void SyncNode()
        {
            BtcNodeCreate.ActionStatus res = create.SyncNodes(create.GetByName("alice"), create.GetByName("miner"));

            Assert.True(res.Success);
        }
    }
}
