using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;
using NBitcoin.Protocol;

namespace SwopCoinLibrary.Node
{
    public class NodeConnect
    {
        public Network Net = Network.TestNet;
        public ActionStatus SendMessageToNetwork(Transaction tx)
        {
            using (NBitcoin.Protocol.Node node = NBitcoin.Protocol.Node.ConnectToLocal(Net)) //Connect to local if no node is set
            {
                node.VersionHandshake();

                node.SendMessage(new InvPayload(InventoryType.MSG_TX, tx.GetHash()));
                System.Threading.Thread.Sleep(1500); //Wait a bit
                node.SendMessage(tx.CreatePayload()); // broadcast message to send funds
                System.Threading.Thread.Sleep(1500); //Wait a bit
            }
            return null;
        }
    }
}


/* using (Node node = Node.ConnectToLocal(net)) //Connect to local if no node is set
                        {
                            node.VersionHandshake();

                            node.SendMessage(new InvPayload(InventoryType.MSG_TX, builtTx.GetHash()));

                            node.SendMessage(builtTx.CreatePayload()); // broadcast message to send funds
                            System.Threading.Thread.Sleep(500); //Wait a bit
                        }
*/