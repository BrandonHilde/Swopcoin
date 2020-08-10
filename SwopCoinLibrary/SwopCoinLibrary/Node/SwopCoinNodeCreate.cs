using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;
using NBitcoin.Payment;
using NBitcoin.OpenAsset;
using SwopCoinLibrary;
using SwopCoinLibrary.Node;

namespace SwopCoinLibrary.Node
{
    public class SwopCoinNodeCreate
    {
        // use to issue swopcoin
        // some of the code looks obsolete 

        public void IssueSwopCoin()
        {
           // ColoredCoin coin = new ColoredCoin();
            //coin.
        }
    }
}


/*
 * 
 * https://www.codeproject.com/Articles/835098/NBitcoin-Build-Them-All
 * 
 * 
 * var issuanceCoinsTransaction
    = new Transaction()
    {
        Outputs =
        {
            new TxOut("1.0", goldGuy.Key.PubKey),
            new TxOut("1.0", silverGuy.Key.PubKey),
            new TxOut("1.0", nico.Key.PubKey),
            new TxOut("1.0", satoshi.Key.PubKey),
        }
    };

IssuanceCoin[] issuanceCoins = issuanceCoinsTransaction 
                        .Outputs 
                        .Take(2) 
                        .Select((o, i) => new Coin(new OutPoint(issuanceCoinsTransaction.GetHash(), i), o))
                        .Select(c => new IssuanceCoin(c))
                        .ToArray();

var goldIssuanceCoin = issuanceCoins[0]; 
var silverIssuanceCoin = issuanceCoins[1]; 
var nicoCoin = new Coin(new OutPoint(issuanceCoinsTransaction, 2), issuanceCoinsTransaction.Outputs[2]); 
var satoshiCoin = new Coin(new OutPoint(issuanceCoinsTransaction, 3), issuanceCoinsTransaction.Outputs[3]);

var goldId = goldIssuanceCoin.AssetId; 
var silverId = silverIssuanceCoin.AssetId;


var txRepo = new NoSqlTransactionRepository();
txRepo.Put(issuanceCoinsTransaction.GetHash(), issuanceCoinsTransaction);


txBuilder = new TransactionBuilder();
tx = txBuilder
    .AddKeys(goldGuy.Key)
    .AddCoins(goldIssuanceCoin)
    .IssueAsset(satoshi.GetAddress(), new Asset(goldId, 20))
    .IssueAsset(nico.GetAddress(), new Asset(goldId, 30))
    .SetChange(goldGuy.Key.PubKey)
    .BuildTransaction(true);
Assert(txBuilder.Verify(tx));
txRepo.Put(tx.GetHash(), tx);

var ctx = tx.GetColoredTransaction(ctxRepo);


var coloredCoins = ColoredCoin.Find(tx, ctx).ToArray();
var satoshiGold = coloredCoins[0];
var nicoGold = coloredCoins[1];


txBuilder = new TransactionBuilder();
tx = txBuilder
    .AddKeys(silverGuy.Key)
    .AddCoins(silverIssuanceCoin)
    .IssueAsset(alice.GetAddress(), new Asset(silverId, 10))
    .SetChange(silverGuy.Key.PubKey)
    .BuildTransaction(true);
Assert(txBuilder.Verify(tx));
txRepo.Put(tx.GetHash(), tx);

var aliceSilver = ColoredCoin.Find(tx, ctxRepo).First();



txBuilder = new TransactionBuilder();
tx = txBuilder
    .AddCoins(nicoGold)
    .AddKeys(nico.Key)
    .SendAsset(satoshi.GetAddress(), new Asset(goldId, 1))
    .SetChange(nico.GetAddress())
    .BuildTransaction(true);
Assert(txBuilder.Verify(tx));
txRepo.Put(tx.GetHash(), tx);

ctx = tx.GetColoredTransaction(ctxRepo);

txBuilder = new TransactionBuilder();
tx = txBuilder
        .AddCoins(aliceSilver)
        .AddCoins(aliceCoins)
        .AddKeys(alice.Key)
        .SendAsset(satoshi.GetAddress(), new Asset(silverId, 9))
        .Send(satoshi.GetAddress(), "0.5")
        .SetChange(alice.GetAddress())
        .Then()
        .AddCoins(satoshiGold)
        .AddCoins(satoshiCoin)
        .AddKeys(satoshi.Key)
        .SendAsset(alice.GetAddress(), new Asset(goldId, 10))
        .SetChange(satoshi.GetAddress())
        .BuildTransaction(true);
Assert(txBuilder.Verify(tx));
txRepo.Put(tx.GetHash(), tx);

ctx = tx.GetColoredTransaction(ctxRepo);





*/
