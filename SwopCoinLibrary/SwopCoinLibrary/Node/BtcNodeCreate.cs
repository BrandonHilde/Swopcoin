using System;
using System.Collections.Generic;
using System.Linq;
//using System.Reflection.Metadata.Ecma335;
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
        List<NodeIdentity> Nodes { get; set; }

        public BtcNodeCreate()
        {
            Nodes = new List<NodeIdentity>();
        }

        public ActionStatus NameNextNode(string name)
        {
            if (Nodes != null)
            {
                foreach (NodeIdentity ni in Nodes)
                {
                    if (ni.Name == string.Empty)
                    {
                        ni.Name = name;

                        return new ActionStatus
                        {
                            Status = "Named " + name,
                            Success = true
                        };
                    }
                }
            }

            return new ActionStatus
            {
                Status = "Failed: Unnamed Node Not Available",
                Success = false
            };
        }

        public ActionStatus SetUpBuilder(string root, string path)
        {
            builder = new NodeBuilder(root, path);
            builder.ConfigParameters.Add("printtoconsole", "0");

            return new ActionStatus
            {
                Status = "Setup",
                Success = true
            };
        }

        public ActionStatus SetUpBuilder(NodeDownloadData data, Network net)
        {
            builder = NodeBuilder.Create(data, net);
            builder.ConfigParameters.Add("printtoconsole", "0");

            return new ActionStatus
            {
                Status = "Setup",
                Success = true
            };
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
                    MineNetwork(Nodes.First(), 101); //miner generate blocks

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

        public ActionStatus EndNetwork()
        {
            foreach(CoreNode n in builder.Nodes)
            {
                n.Kill();
            }

            builder.Dispose();

            return new ActionStatus { Status = "end", Success = true };
        }

        public ActionStatus AddAddresses()
        {
            if (Nodes != null)
            {
                foreach (NodeIdentity ni in Nodes)
                {
                    if (ni.Client != null)
                    {
                        BitcoinAddress addr = ni.Client.GetNewAddress();

                        ni.Addresses.Add(addr);
                    }
                }
            }

            return new ActionStatus
            {
                Status = "Complete Address Creation",
                Success = true
            };
        }

        public ActionStatus SendBitcoin(NodeIdentity Person, Money m, NodeIdentity SendTo)
        {
            Person.Client.SendToAddress(SendTo.MainAddress, m);

            return new ActionStatus
            {
                Status = "Complete",
                Success = true
            };
        }

        public ActionStatus SendBitcoin(NodeIdentity Person, Money m, BitcoinAddress SendToAddress)
        {
            Person.Client.SendToAddress(SendToAddress, m);

            return new ActionStatus
            {
                Status = "Complete",
                Success = true
            };
        }

        public ActionStatus MineNetwork(NodeIdentity Miner, int blocks)
        {
            Miner.Node.Generate(blocks);

            return new ActionStatus
            {
                Status = "Complete",
                Success = true
            };
        }

        public ActionStatus SyncNodes(NodeIdentity BtcNode, NodeIdentity SyncNode)
        {
            BtcNode.Node.Sync(SyncNode.Node);

            return new ActionStatus
            {
                Status = "Complete",
                Success = true
            };
        }

        public ActionStatus CreateNodeSet(int number, string root = "", string path = "")
        {
            if (builder == null) SetUpBuilder(root, path);

            if (Nodes == null) Nodes = new List<NodeIdentity>();

            for (int x = 0; x < number; x++)
            {
                Nodes.Add(new NodeIdentity
                {
                    Node = builder.CreateNode()
                });
            }

            return new ActionStatus
            {
                Status = "Complete",
                Success = true
            };
        }

        public ActionStatus SimulateNetwork()
        {
            SetUpBuilder(NodeDownloadData.Bitcoin.v0_18_0, Network.RegTest);

            string status = "";

            if (CreateNodeSet(3).Success)
            {
                status += "Created Nodes\r\n";

                List<uint256> list = StartNetwork();

                if (list != null)
                {
                    if (list.Count > 1)
                    {
                        for (int i = 1; i < list.Count; i++)
                        {
                            if (list[i] != list[i - 1])
                            {
                                return new ActionStatus
                                {
                                    Status = "Network Failed To Start"
                                };
                            }
                        }
                    }
                }

                status += "Network Started\r\n";

                if (AddAddresses().Success)
                {
                    status += "Created Addresses\r\n";

                    NameNextNode("miner");
                    NameNextNode("alice");
                    NameNextNode("bob");

                    status += "Nodes Named\r\n";

                    if (SendBitcoin(GetByName("miner"), Money.Coins(20m), GetByName("alice")).Success)
                    {
                        status += "Bitcoin Sent From Miner\r\n";
                    }
                    else
                    {
                        return new ActionStatus
                        {
                            Status = "Failed To Send BTC"
                        };
                    }

                    if (MineNetwork(GetByName("miner"), 1).Success)
                    {
                        status += "Bitcoin Mined\r\n";
                    }
                    else
                    {
                        return new ActionStatus
                        {
                            Status = "Failed To Mine BTC"
                        };
                    }

                    if (SyncNodes(GetByName("alice"), GetByName("miner")).Success)
                    {
                        status += "Alice Balance: " + GetByName("alice").Client.GetBalance().ToString();
                    }
                    else
                    {
                        return new ActionStatus
                        {
                            Status = "Failed To Sync BTC"
                        };
                    }

                    if (SendBitcoin(GetByName("alice"), Money.Coins(1m), GetByName("bob")).Success)
                    {
                        status += "Bitcoin Sent From Alice\r\n";
                    }
                    else
                    {
                        return new ActionStatus
                        {
                            Status = "Failed To Send BTC"
                        };
                    }

                    if (MineNetwork(GetByName("alice"), 1).Success)
                    {
                        status += "Bitcoin Mined\r\n";
                    }
                    else
                    {
                        return new ActionStatus
                        {
                            Status = "Failed To Mine BTC"
                        };
                    }

                    if (SyncNodes(GetByName("alice"), GetByName("bob")).Success)
                    {
                        status += "Alice Balance: " + GetByName("alice").Client.GetBalance().ToString();
                        status += "Bob Balance: " + GetByName("bob").Client.GetBalance().ToString();
                    }
                    else
                    {
                        return new ActionStatus
                        {
                            Status = "Failed To Sync BTC"
                        };
                    }
                }
                else
                {
                    return new ActionStatus
                    {
                        Status = "Failed To Create Addresses"
                    };
                }

                return new ActionStatus
                {
                    Status = status,
                    Success = true
                };
            }

            return new ActionStatus
            {
                Status = status,
                Success = false
            };
        }

        public class ActionStatus
        {
            public string Status = string.Empty;
            public bool Success = false;
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
