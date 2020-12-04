using System;
using System.Collections.Generic;
using System.Text;
using SwopCoinLibrary.Interface;
using SwopCoinLibrary.Components;
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
using System.Linq;

namespace SwopCoinLibrary.Dealers
{
    public class BaseDealer : IChainDealing
    {
        // Blockchain Coins

        public Coins[] CirculatedCoins;

        public Coins[] UncirculatedCoins;

        public Coins[] IssuedCoins;

        public Coins[] SubscribedCoins;

        public Coins[] AuthorizedCoins;

        public Coins[] ContributedCoins;

		BitcoinSecret CoinIssuer { get; set; }

		Network Net = Network.TestNet;

		public BaseDealer(Network Network, BitcoinSecret Issuer = null)
        {
			CoinIssuer = Issuer;
			Net = Network;
        }

		public void Authorize(Coins coin)
        {
			if(CoinIssuer == null)
            {
				// throw exception 
            }
            else
            {
				coin = new AuthorizedCoins((decimal)coin.TotalCoinQuantity);
				Issue(CoinIssuer.GetAddress(ScriptPubKeyType.Legacy), coin);
			}
        }

        public void Contribute(Coins coin) { throw new NotImplementedException(); }

        public void Issue(BitcoinAddress Address, Coins coin) 
        {
			IssueSpecificCoin(coin, (long)coin.TotalCoinQuantity, Address);
        }

        public void Subscribe(string email, Coins coin) 
        {
            Subscriber sub = new Subscriber(email, (SubscribedCoins)coin);
            sub.SaveSubscription();
        }

        public void Uncirculate(Coins coin) { throw new NotImplementedException(); }

        public void Circulate(Coins coin) { throw new NotImplementedException(); }

        // Blockchain Orders
        public Orders[] NewOrders;
        public Orders[] ModifiedOrders;
        public Orders[] DeletedOrders;
        public Orders[] ReviewedOrders;
        public Orders[] ApprovedOrders;
        public Orders[] DispachedOrders;
        public Orders[] EndorsedOrders;
        public Orders[] ArchivedOrders;

        public void Approve(Orders order) { throw new NotImplementedException(); }

        public void Archive(Orders order) { throw new NotImplementedException(); }

        public void Browse(Orders order) { throw new NotImplementedException(); }

        public void Delete(Orders order) { throw new NotImplementedException(); }

        public void Dispatch(Orders order) { throw new NotImplementedException(); }

        public void Dispose() { throw new NotImplementedException(); }

        public void Endorse(Orders order) { throw new NotImplementedException(); }

        public void Modify(Orders order) { throw new NotImplementedException(); }

        public void New(Orders order) { throw new NotImplementedException(); }

        public void Review(Orders order) { throw new NotImplementedException(); }

        public void Search(Orders order) { throw new NotImplementedException(); }

        public Orders Start() { throw new NotImplementedException(); }

		#region Colored Coin Transaction Code
		/////////////////////////////////
		///Handle Coin Movement
		///////////////////////////////////


		/// <summary>
		/// Send a transaction to the network
		/// </summary>
		/// <param name="tx">The transaction</param>
		/// <param name="node">The node. Can be left null.</param>
		/// <returns></returns>
		public void SendTransaction(Transaction tx, NBitcoin.Protocol.Node node = null)
		{
			NodeConnect connect = new NodeConnect();
			connect.Net = Net;
			connect.SendMessageToNetwork(tx, node);
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

		/// <summary>
		/// Transfer a coin from one address to many.
		/// </summary>
		/// <param name="CoinType"></param>
		/// <param name="amount"></param>
		/// <param name="From"></param>
		/// <param name="To"></param>
		/// <returns></returns>
		public Transaction CreateTransferCoinMany(Coins CoinType, long amount, BitcoinSecret From, List<BitcoinAddress> To)
		{
			IssuanceCoin ic = CoinType.ConvertToCoin();

			TransactionBuilder txBuilder = Net.CreateTransactionBuilder();

			txBuilder.AddCoins(ic);
			txBuilder.AddKeys(From);

			foreach (BitcoinAddress add in To)
			{
				txBuilder.SendAsset(add, new AssetMoney(ic.AssetId, amount));
			}

			txBuilder.SetChange(From.GetAddress(ScriptPubKeyType.Legacy));
			Transaction tx = txBuilder.BuildTransaction(true);

			return tx;
		}

		/// <summary>
		/// Issue a specific coin type to multiple addresses
		/// </summary>
		/// <param name="CoinType"></param>
		/// <param name="amount"></param>
		/// <param name="IssueToAddresses"></param>
		public void IssueSpecificCoinToMany(Coins CoinType, long amount, List<BitcoinAddress> IssueToAddresses)
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
		/// <summary>
		/// Issue a specific coin type
		/// </summary>
		/// <param name="CoinType"></param>
		/// <param name="amount"></param>
		/// <param name="IssueToAddress"></param>
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

		/// <summary>
		/// Converts BitcoinAddress list to a script list
		/// </summary>
		/// <param name="addresses"></param>
		/// <returns></returns>
		private List<Script> GetScriptList(List<BitcoinAddress> addresses)
		{
			return addresses.Select(x => x.ScriptPubKey).ToList();
		}

		/// <summary>
		/// Select the created coin for issuance
		/// </summary>
		/// <param name="tx"></param>
		/// <returns></returns>
		private List<IssuanceCoin> GetIssuance(Transaction tx)
		{
			List<IssuanceCoin> issuanceCoins = tx.Outputs.Take(1)
					.Select((o, i) => new Coin(new OutPoint(tx.GetHash(), i), o))
					.Select(c => new IssuanceCoin(c))
					.ToList();

			return issuanceCoins;
		}

		/// <summary>
		/// Create a transaction for issuing coin
		/// </summary>
		/// <param name="coin"></param>
		/// <param name="IssueFrom"></param>
		/// <param name="IssueTo"></param>
		/// <param name="amount"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		private Transaction CreateIssuanceTransaction(IssuanceCoin coin, Script IssueFrom, Script IssueTo, long amount, AssetId id)
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

		/// <summary>
		/// Distribute coins to a list of contributers
		/// </summary>
		/// <param name="coin"></param>
		/// <param name="IssueFrom"></param>
		/// <param name="IssueTo"></param>
		/// <param name="amount"></param>
		/// <param name="id"></param>
		/// <returns></returns>
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

        #endregion

    }
}
