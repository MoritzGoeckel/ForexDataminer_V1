using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader.Model
{
    public class TradePosition
    {
        public TradePosition(long timestampOpen, double priceOpen, PositionType type)
        {
            this.timestampOpen = timestampOpen;
            this.priceOpen = priceOpen;
            this.type = type;
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

        public enum PositionType{
            longPosition, shortPosition
        }
    }
}
