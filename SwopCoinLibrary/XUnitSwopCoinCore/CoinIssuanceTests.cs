﻿using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using SwopCoinLibrary;
using SwopCoinLibrary.Node;
using NBitcoin.Protocol;
using NBitcoin.Tests;
using NBitcoin;
using System.Diagnostics;

namespace XUnitSwopCoinCore
{
    public class CoinIssuanceTests
    {
        [Fact]
        public void IssueCoin()
        {
            //temp testing area

            SwopCoinNodeCreate create = new SwopCoinNodeCreate(null);

            create.IssueSwopCoin();
        }
    }
}
