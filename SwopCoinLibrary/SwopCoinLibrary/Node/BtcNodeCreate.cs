using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using NBitcoin;
using NBitcoin.RPC;
using NBitcoin.Tests;
using SwopCoinLibrary.OpenAssetFormat;

namespace SwopCoinLibrary.Node
{
    public class BtcNodeCreate
    {
        NodeBuilder builder { get; set; }
        List<NodeIdentity> Nodes { get; set;}

        public BtcNodeCreate()
        {
            Nodes = new List<NodeIdentity>();
        }

        public void NameNextNode(string name)
        {
            if (Nodes != null)
            {
                foreach (NodeIdentity ni in Nodes)
                {
                    if (ni.Name == string.Empty) ni.Name = name;
                }
            }
        }

        public void SetUpBuilder(string root, string path)
        {
            builder = new NodeBuilder(root, path);
            builder.ConfigParameters.Add("printtoconsole", "0");
        }

        public NodeIdentity GetByName(string name)
        {
            if (Nodes != null)
            {
                foreach (NodeIdentity ni in Nodes)
                {
                    if (ni.Name == name) return ni;
                }
            }

            return null;
        }

        public List<uint256> StartNetwork()
        {
            List<uint256> outputs = new List<uint256>();

            if (builder != null)
            {
                builder.StartAll();

                if (Nodes.Count > 0)
                {
                    Nodes.First().Node.Generate(101); //miner generate blocks

                    for (int n = 1; n < Nodes.Count; n++)
                    {
                        Nodes[n].Node.Sync(Nodes.First().Node, true); //sync other nodes
                    }

                    foreach (NodeIdentity nd in Nodes)
                    {
                        nd.Client = nd.Node.CreateRPCClient();
                    }

                    foreach (NodeIdentity nd in Nodes)
                    {
                        outputs.Add(nd.Client.GetBestBlockHash()); //get ouputs for checking sync
                    }
                }
            }

            return outputs;
        }

        public TestResult AddAddresses()
        {
            if (Nodes != null)
            {
                foreach (NodeIdentity ni in Nodes)
                {
                    if (ni.Client != null)
                    {
                        BitcoinAddress addr = ni.Client.GetNewAddress();

                        ni.Addresses.Add(addr);

                        return new TestResult { Status = "Address: " + addr.ToString() };
                    }
                }
            }

            return new TestResult { Status = "Failed" };
        }

        public TestResult CreateNodeSet(int number, string root, string path)
        {
            if (builder == null) SetUpBuilder(root, path);

            if(Nodes == null) Nodes = new List<NodeIdentity>();

            for (int x = 0; x < number; x++)
            {
                Nodes.Add(new NodeIdentity
                {
                    Node = builder.CreateNode()
                });
            }

            return new TestResult { Status = "Complete" };
        }
        public class TestResult
        {
            public string Status = string.Empty;
        }

        public class NodeIdentity
        {
            public string Name = string.Empty;
            public CoreNode Node { get; set; }
            public RPCClient Client { get; set; }
            public List<BitcoinAddress> Addresses = new List<BitcoinAddress>();
            public BitcoinAddress MainAddress { get { return Addresses.First(); } }
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
