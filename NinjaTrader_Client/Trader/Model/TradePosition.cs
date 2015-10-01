using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader.Model
{
    public class TradePosition
    {
        public TradePosition(long timestampOpen, double priceOpen, PositionType type, string pair)
        {
            this.timestampOpen = timestampOpen;
            this.priceOpen = priceOpen;
            this.type = type;
            this.pair = pair;
        }

        public double getDifference()
        {
            if (type == PositionType.shortPosition)
                return priceOpen - priceClose;
            else
                return priceClose - priceOpen;
        }

        public long timestampOpen = -1;
        public long timestampClose = -1;
        public double priceOpen;
        public double priceClose;
        public PositionType type;

        public string pair;

        public enum PositionType{
            longPosition, shortPosition
        }
    }
}
