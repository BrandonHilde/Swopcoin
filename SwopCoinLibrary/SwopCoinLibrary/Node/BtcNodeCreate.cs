using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NBitcoin;
using NBitcoin.Tests;

namespace SwopCoinLibrary.Node
{
    public class BtcNodeCreate
    {
        NodeBuilder builder { get; set; }
        List<CoreNode> nodes { get; set; }

        public BtcNodeCreate()
        {
            nodes = new List<CoreNode>();
        }

        public void SetUpBuilder(string root, string path)
        {
            builder = new NodeBuilder(root, path);
            builder.ConfigParameters.Add("printtoconsole", "0");
        }

        public List<uint256> StartNetwork()
        {
            List<uint256> outputs = new List<uint256>();

            if (builder != null)
            {
                builder.StartAll();

                if (nodes.Count > 0)
                {
                    nodes.First().Generate(101);

                    for (int n = 1; n < nodes.Count; n++)
                    {
                        nodes[n].Sync(nodes.First());
                    }


                    foreach (CoreNode nd in nodes)
                    {
                        outputs.Add(nd.CreateRPCClient().GetBestBlockHash());
                    }
                }
            }

            return outputs;
        }

        public TestResult CreateNodeSet(int number, string root, string path)
        {
            if (builder == null) SetUpBuilder(root, path);

            nodes = new List<CoreNode>();

            for (int x = 0; x < number; x++)
            {
                nodes.Add(builder.CreateNode());
            }

            return new TestResult { Status = "Complete" };
        }
        public class TestResult
        {
            public string Status = string.Empty;
        }
    }
}


/*
 // During the first run, this will take time to run, as it download bitcoin core binaries (more than 40MB)
            using (var env = NodeBuilder.Create(NodeDownloadData.Bitcoin.v0_18_0, Network.RegTest))
            {
                // Removing node logs from output
                env.ConfigParameters.Add("printtoconsole", "0");

                var alice = env.CreateNode();
                var bob = env.CreateNode();
                var miner = env.CreateNode();
                env.StartAll();
                Console.WriteLine("Created 3 nodes (alice, bob, miner)");

                Console.WriteLine("Connect nodes to each other");
                miner.Sync(alice, true);
                miner.Sync(bob, true);

                Console.WriteLine("Generate 101 blocks so miner can spend money");
                var minerRPC = miner.CreateRPCClient();
                miner.Generate(101);
                
                var aliceRPC = alice.CreateRPCClient();
                var bobRPC = bob.CreateRPCClient();
                var bobAddress = bobRPC.GetNewAddress();

                Console.WriteLine("Alice gets money from miner");
                var aliceAddress = aliceRPC.GetNewAddress();
                minerRPC.SendToAddress(aliceAddress, Money.Coins(20m));

                Console.WriteLine("Mine a block and check that alice is now synched with the miner (same block height)");
                minerRPC.Generate(1);
                alice.Sync(miner);

                Console.WriteLine($"Alice Balance: {aliceRPC.GetBalance()}");

                Console.WriteLine("Alice send 1 BTC to bob");
                aliceRPC.SendToAddress(bobAddress, Money.Coins(1.0m));
                Console.WriteLine($"Alice mine her own transaction");
                aliceRPC.Generate(1);

                alice.Sync(bob);

                Console.WriteLine($"Alice Balance: {aliceRPC.GetBalance()}");
                Console.WriteLine($"Bob Balance: {bobRPC.GetBalance()}");
            }
 */
