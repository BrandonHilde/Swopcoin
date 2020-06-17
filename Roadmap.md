Swopcoin Service (gRPC using .NET core)
 

Build a gRPC service node. Copies of the node that will run on multiple computers to form the Swopcoin Network.

 

The network will allow anyone to join the network that owns crypto in an enabled cryptocurrency.

 

The network will communicate by a gossip type protocol.

 

Each message on the network will have an index that starts at zero for a new message and every time a message gets passed along the index is incremented.

 

Each node will determine the network sequence order of new messages using a set of rules that allows all the nodes to agree on order.

 

Each node will determine by rules that they are next to finalize an exchange transaction.

 

Each node will also be a node for the cryptocurrencies that they deal in.

 

The messages will contain crypto transactions native to particular blockchains that have not been signed but have been constructed to satisfy an exchange when the message is confirmed then the transactions in the messages will be signed and broadcasted.

 

Swopcoin Client API (class library using .NET core)
 

Build a Swopcoin Wallet API that allows for the determination of the crypto balances in the wallet and allows for the initiation of exchange. It would also allow for the addition of crypto.

 

 

Swopcoin Testing (xUnit that tests the API)
 

Swopcoin will be issued on bitcoin using Open Asset Protocol (but later will move beyond that into the Swopcoin protocol).

 

Test making random bitcoin transactions among a local test network of BTC nodes.

 

Test making random bitcoin transactions on the Swopcoin network and then put them in order of size and execute from smallest to largest. One transaction per block.