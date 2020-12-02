using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;
using NBitcoin.Payment;
using NBitcoin.Protocol;
using NBitcoin.OpenAsset;
using NBitcoin.Crypto;
using NBitcoin.DataEncoders;
using NBitcoin.Policy;
using NBitcoin.Stealth;
using SwopCoinLibrary;
using SwopCoinLibrary.Node;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Linq;

namespace SwopCoinLibrary.Node
{
    public class SwopCoinNodeCreate
    {
        // use to issue swopcoin
        // some of the code looks obsolete 

		BitcoinSecret CoinIssuer { get; set; }

		Network Net = Network.TestNet;

		public SwopCoinNodeCreate(BitcoinSecret Issuer)
        {
			CoinIssuer = Issuer;
		}

		public void SendTransaction(Transaction tx)
        {
			NodeConnect connect = new NodeConnect();

			connect.Net = Net;
			connect.SendMessageToNetwork(tx);
        }

		public Transaction TransferCoin(Coins CoinType, long amount, BitcoinSecret From, BitcoinAddress To)
        {
			IssuanceCoin ic = CoinType.ConvertToCoin();

			TransactionBuilder txBuilder = Net.CreateTransactionBuilder();
			Transaction tx = txBuilder
					.AddCoins(ic)
					.AddKeys(From)
					.SendAsset(To, new AssetMoney(ic.AssetId, amount))
					.SetChange(From.GetAddress(ScriptPubKeyType.Legacy))
					.BuildTransaction(true);

			return tx;
		}

		public Transaction CreateTransferCoinMany(Coins CoinType, long amount, BitcoinSecret From, List<BitcoinAddress> To)
		{
			IssuanceCoin ic = CoinType.ConvertToCoin();

			TransactionBuilder txBuilder = Net.CreateTransactionBuilder();

			txBuilder.AddCoins(ic);
			txBuilder.AddKeys(From);

			foreach(BitcoinAddress add in To)
            {
				txBuilder.SendAsset(add, new AssetMoney(ic.AssetId, amount));
			}

			txBuilder.SetChange(From.GetAddress(ScriptPubKeyType.Legacy));
			Transaction tx = txBuilder.BuildTransaction(true);

			return tx;
		}

		public void IssueSpecificCoin(Coins CoinType, long amount, List<BitcoinAddress> IssueToAddresses)
        {
			if (IssueToAddresses != null)
			{
				if (IssueToAddresses.Count > 0)
				{
					Script issuerScript = CoinIssuer.GetAddress(ScriptPubKeyType.Legacy).ScriptPubKey;
					List<Script> scripts = GetScriptList(IssueToAddresses);

					Transaction txSwop = Transaction.Create(Net);

					txSwop.Outputs.Add(new TxOut("1.0", issuerScript));

					foreach (Script scptM in scripts) txSwop.Outputs.Add(new TxOut("1.0", scptM));

					List<IssuanceCoin> issuanceCoins = GetIssuance(txSwop);

					var swopIssuanceCoin = issuanceCoins.First();

					var swopID = swopIssuanceCoin.AssetId;

					var txRepo = new NoSqlTransactionRepository();
					txRepo.Put(txSwop.GetHash(), txSwop);

					var ctxRepo = new NoSqlColoredTransactionRepository(txRepo);

					Transaction tx = CreateMultipleIssuanceTransaction(swopIssuanceCoin, issuerScript, scripts, amount, swopID);

					txRepo.Put(tx.GetHash(), tx);

					var ctx = tx.GetColoredTransaction(ctxRepo);
				}
			}
        }
		public void IssueSpecificCoin(Coins CoinType, long amount, BitcoinAddress IssueToAddress)
		{
			if (IssueToAddress != null)
			{
				Script issuerScript = CoinIssuer.GetAddress(ScriptPubKeyType.Legacy).ScriptPubKey;
				Script script = IssueToAddress.ScriptPubKey;

				Transaction txSwop = Transaction.Create(Net);

				txSwop.Outputs.Add(new TxOut("1.0", issuerScript));

				txSwop.Outputs.Add(new TxOut("1.0", script));

				IssuanceCoin[] issuanceCoins = txSwop.Outputs.Take(1)
					.Select((o, i) => new Coin(new OutPoint(txSwop.GetHash(), i), o))
					.Select(c => new IssuanceCoin(c))
					.ToArray();

				var swopIssuanceCoin = issuanceCoins.First();

				var swopID = swopIssuanceCoin.AssetId;

				var txRepo = new NoSqlTransactionRepository();
				txRepo.Put(txSwop.GetHash(), txSwop);

				var ctxRepo = new NoSqlColoredTransactionRepository(txRepo);

				Transaction tx = CreateIssuanceTransaction(swopIssuanceCoin, issuerScript, script, amount, swopID);

				txRepo.Put(tx.GetHash(), tx);

				var ctx = tx.GetColoredTransaction(ctxRepo);
			}	
		}

		private List<Script> GetScriptList(List<BitcoinAddress> addresses)
        {
			return addresses.Select(x => x.ScriptPubKey).ToList();
		}

		private List<IssuanceCoin> GetIssuance(Transaction tx)
        {
			List<IssuanceCoin> issuanceCoins = tx.Outputs.Take(1)
					.Select((o, i) => new Coin(new OutPoint(tx.GetHash(), i), o))
					.Select(c => new IssuanceCoin(c))
					.ToList();

			return issuanceCoins;
		}

		private Transaction CreateIssuanceTransaction(IssuanceCoin coin,Script IssueFrom, Script IssueTo, long amount, AssetId id)
        {
			TransactionBuilder builder = Net.CreateTransactionBuilder();

			builder.AddKeys(CoinIssuer);
			builder.AddCoins(coin);

			builder.IssueAsset(IssueTo, new AssetMoney(id, (long)amount));

			builder.SetChange(IssueFrom);

			Transaction tx = builder.BuildTransaction(true);

			if (builder.Verify(tx)) { }

			return tx;
		}

		private Transaction CreateMultipleIssuanceTransaction(IssuanceCoin coin, Script IssueFrom, List<Script> IssueTo, long amount, AssetId id)
		{
			TransactionBuilder builder = Net.CreateTransactionBuilder();

			builder.AddKeys(CoinIssuer);
			builder.AddCoins(coin);

			foreach (Script add in IssueTo)
			{
				builder.IssueAsset(add, new AssetMoney(id, (long)amount));
			}

			builder.SetChange(IssueFrom);

			Transaction tx = builder.BuildTransaction(true);

			if (builder.Verify(tx)) { }

			return tx;
		}

		public void IssueSwopCoin()
        {
			BtcNodeCreate btcTest = new BtcNodeCreate();
			btcTest.SimulateNetwork();

			Script scptM = btcTest.GetByName("miner").MainAddress.ScriptPubKey;
			Script scptA = btcTest.GetByName("alice").MainAddress.ScriptPubKey;
			Script scptB = btcTest.GetByName("bob").MainAddress.ScriptPubKey;

			BitcoinSecret minerSecret = btcTest.GetByName("miner").Client.DumpPrivKey(btcTest.GetByName("miner").MainAddress);

			Transaction txSwop = Transaction.Create(Network.Main);

			txSwop.Outputs.Add(new TxOut("1.0", scptM));
			txSwop.Outputs.Add(new TxOut("1.0", scptA));
			txSwop.Outputs.Add(new TxOut("1.0", scptB));

			//txSwop.
			//txSwop.Outputs.Add(new OutPoint(txSwop.GetHash(), 1), scptA);
			//txSwop.Outputs.Add(new OutPoint(txSwop.GetHash(), 2), scptB);

			IssuanceCoin[] issuanceCoins = txSwop.Outputs.Take(1)
						.Select((o, i) => new Coin(new OutPoint(txSwop.GetHash(), i), o))
						.Select(c => new IssuanceCoin(c))
						.ToArray();

			var goldIssuanceCoin = issuanceCoins[0];
			//var SwopCoinIssue = new Coin(new OutPoint(txSwop, 1), txSwop.Outputs[1]);
			//var SwopCoinIssueTwo = new Coin(new OutPoint(txSwop, 2), txSwop.Outputs[2]);

			var goldId = goldIssuanceCoin.AssetId;


			var txRepo = new NoSqlTransactionRepository();
			txRepo.Put(txSwop.GetHash(), txSwop);

			var ctxRepo = new NoSqlColoredTransactionRepository(txRepo);

			TransactionBuilder builder = Network.Main.CreateTransactionBuilder();

			Transaction tx = builder
			.AddKeys(minerSecret)
			.AddCoins(goldIssuanceCoin)
			.IssueAsset(btcTest.GetByName("bob").MainAddress, new AssetMoney(goldId, 20))
			.IssueAsset(btcTest.GetByName("alice").MainAddress, new AssetMoney(goldId, 30))
			.SetChange(scptM)
			.BuildTransaction(true);
			//Assert(builder.Verify(tx));
			txRepo.Put(tx.GetHash(), tx);


			var ctx = tx.GetColoredTransaction(ctxRepo);

			var coloredCoins = ColoredCoin.Find(tx, ctx).ToArray();
			var satoshiGold = coloredCoins[0];
			var nicoGold = coloredCoins[1];

			Console.WriteLine(ctx);
			Console.WriteLine("-------------------------");
			Console.WriteLine(satoshiGold);
			Console.WriteLine(nicoGold);



			// coin.
			//t.
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


////////////////////////////////////////////////////////////////////////////////////////
///
[Fact]
		[Trait("UnitTest", "UnitTest")]
		//https://github.com/OpenAssets/open-assets-protocol/blob/master/specification.mediawiki
		public void CanColorizeSpecScenario()
		{
			var repo = new NoSqlColoredTransactionRepository();
			var dust = Money.Parse("0.00005");
			var colored = new ColoredTransaction();
			var a1 = new AssetKey();
			var a2 = new AssetKey();
			var h = new AssetKey();
			var sender = new Key().PubKey.GetAddress(ScriptPubKeyType.Legacy, Network.Main);
			var receiver = new Key().PubKey.GetAddress(ScriptPubKeyType.Legacy, Network.Main);

			colored.Marker = new ColorMarker(new ulong[] { 0, 10, 6, 0, 7, 3 });
			colored.Inputs.Add(new ColoredEntry(0, new AssetMoney(a1.Id, 3UL)));
			colored.Inputs.Add(new ColoredEntry(1, new AssetMoney(a1.Id, 2UL)));
			colored.Inputs.Add(new ColoredEntry(3, new AssetMoney(a1.Id, 5UL)));
			colored.Inputs.Add(new ColoredEntry(4, new AssetMoney(a1.Id, 3UL)));
			colored.Inputs.Add(new ColoredEntry(5, new AssetMoney(a2.Id, 9UL)));

			colored.Issuances.Add(new ColoredEntry(1, new AssetMoney(h.Id, 10UL)));
			colored.Transfers.Add(new ColoredEntry(3, new AssetMoney(a1.Id, 6UL)));
			colored.Transfers.Add(new ColoredEntry(5, new AssetMoney(a1.Id, 7UL)));
			colored.Transfers.Add(new ColoredEntry(6, new AssetMoney(a2.Id, 3UL)));
			var destroyed = colored.GetDestroyedAssets();
			Assert.True(destroyed.Length == 1);
			Assert.True(destroyed[0].Quantity == 6);
			Assert.True(destroyed[0].Id == a2.Id);
			colored = colored.Clone();
			destroyed = colored.GetDestroyedAssets();
			Assert.True(destroyed.Length == 1);
			Assert.True(destroyed[0].Quantity == 6);
			Assert.True(destroyed[0].Id == a2.Id);

			var prior = Network.Main.CreateTransaction();
			prior.Outputs.Add(new TxOut(dust, a1.ScriptPubKey));
			prior.Outputs.Add(new TxOut(dust, a2.ScriptPubKey));
			prior.Outputs.Add(new TxOut(dust, h.ScriptPubKey));
			repo.Transactions.Put(prior.GetHash(), prior);

			var issuanceA1 = Network.Main.CreateTransaction();
			issuanceA1.Inputs.Add(new TxIn(new OutPoint(prior.GetHash(), 0)));
			issuanceA1.Outputs.Add(new TxOut(dust, h.ScriptPubKey));
			issuanceA1.Outputs.Add(new TxOut(dust, sender));
			issuanceA1.Outputs.Add(new TxOut(dust, sender));
			issuanceA1.Outputs.Add(new TxOut(dust, sender));
			issuanceA1.Outputs.Add(new TxOut(dust, new ColorMarker(new ulong[] { 3, 2, 5, 3 }).GetScript()));
			repo.Transactions.Put(issuanceA1.GetHash(), issuanceA1);

			var issuanceA2 = Network.Main.CreateTransaction();
			issuanceA2.Inputs.Add(new TxIn(new OutPoint(prior.GetHash(), 1)));
			issuanceA2.Outputs.Add(new TxOut(dust, sender));
			issuanceA2.Outputs.Add(new TxOut(dust, new ColorMarker(new ulong[] { 9 }).GetScript()));
			repo.Transactions.Put(issuanceA2.GetHash(), issuanceA2);

			var testedTx = CreateSpecTransaction(repo, dust, receiver, prior, issuanceA1, issuanceA2);
			var actualColored = testedTx.GetColoredTransaction(repo);

			Assert.True(colored.ToBytes().SequenceEqual(actualColored.ToBytes()));


			//Finally, for each transfer output, if the asset units forming that output all have the same asset address, the output gets assigned that asset address. If any output contains units from more than one distinct asset address, the whole transaction is considered invalid, and all outputs are uncolored.

			var testedBadTx = CreateSpecTransaction(repo, dust, receiver, prior, issuanceA1, issuanceA2);
			testedBadTx.Outputs[2] = new TxOut(dust, new ColorMarker(new ulong[] { 0, 10, 6, 0, 6, 4 }).GetScript());
			repo.Transactions.Put(testedBadTx.GetHash(), testedBadTx);
			colored = testedBadTx.GetColoredTransaction(repo);

			destroyed = colored.GetDestroyedAssets();
			Assert.True(destroyed.Length == 2);
			Assert.True(destroyed[0].Id == a1.Id);
			Assert.True(destroyed[0].Quantity == 13);
			Assert.True(destroyed[1].Id == a2.Id);
			Assert.True(destroyed[1].Quantity == 9);


			//If there are more items in the  asset quantity list  than the number of colorable outputs, the transaction is deemed invalid, and all outputs are uncolored.
			testedBadTx = CreateSpecTransaction(repo, dust, receiver, prior, issuanceA1, issuanceA2);
			testedBadTx.Outputs[2] = new TxOut(dust, new ColorMarker(new ulong[] { 0, 10, 6, 0, 7, 4, 10, 10 }).GetScript());
			repo.Transactions.Put(testedBadTx.GetHash(), testedBadTx);

			colored = testedBadTx.GetColoredTransaction(repo);

			destroyed = colored.GetDestroyedAssets();
			Assert.True(destroyed.Length == 2);
			Assert.True(destroyed[0].Id == a1.Id);
			Assert.True(destroyed[0].Quantity == 13);
			Assert.True(destroyed[1].Id == a2.Id);
			Assert.True(destroyed[1].Quantity == 9);


/////////////////////////////////////////////////////////////////////////////////////////////////////
///
https://www.codeproject.com/Articles/1151054/Create-a-Bitcoin-transaction-by-hand

*/
