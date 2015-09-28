using NinjaTrader_Client.Trader.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader
{
    public abstract class TradingAPI
    {
        public abstract bool openLong();
        public abstract bool openShort();

        public abstract bool closePositions();

        public abstract bool closeShort();
        public abstract bool closeLong();

        public abstract double getBid();
        public abstract double getAsk();
        public abstract long getNow();
        public abstract TradePosition getLongPosition();
        public abstract TradePosition getShortPosition();
        public abstract List<TradePosition> getHistory();

        public abstract bool isUptodate();
    }
}
