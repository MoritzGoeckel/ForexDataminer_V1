using NinjaTrader_Client.Model;
using NinjaTrader_Client.Trader.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader.TradingAPIs
{
    class PairData
    {
        public TradePosition lastLongPosition, lastShortPosition;
        public List<TradePosition> oldPositions = new List<TradePosition>();
        public Tickdata lastTickData;
    }
}
