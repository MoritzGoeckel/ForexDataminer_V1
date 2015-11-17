using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader.Model
{
    public class TradePosition
    {
        public TradePosition(PositionType type, long timestampOpen, double priceOpen, long timestampClose, double priceClose)
        {
            this.timestampOpen = timestampOpen;
            this.timestampClose = timestampClose;

            this.priceOpen = priceOpen;
            this.priceClose = priceClose;

            this.type = type;
        }

        public TradePosition(long timestampOpen, double priceOpen, PositionType type, string pair)
        {
            this.timestampOpen = timestampOpen;
            this.priceOpen = priceOpen;
            this.type = type;
            this.pair = pair;
        }

        public double getDifference()
        {
            if (priceClose == -1)
                throw new Exception();

            if (type == PositionType.shortPosition)
                return priceOpen - priceClose;
            else
                return priceClose - priceOpen;
        }

        public double getDifference(double currentBid, double currentAsk)
        {
            if (type == PositionType.shortPosition)
                return priceOpen - currentAsk;
            else
                return currentBid - priceOpen;
        }

        public long timestampOpen = -1;
        public long timestampClose = -1;
        public double priceOpen;
        public double priceClose = -1;
        public PositionType type;

        public string pair;

        public enum PositionType{
            longPosition, shortPosition
        }
    }
}
