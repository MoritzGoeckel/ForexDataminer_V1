using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader.Model
{
    public class TradePosition
    {
        public TradePosition(PositionType type, long timestampOpen, double priceOpen, long timestampClose, double priceClose, string instrument)
        {
            this.timestampOpen = timestampOpen;
            this.timestampClose = timestampClose;

            this.instrument = instrument;

            this.priceOpen = priceOpen;
            this.priceClose = priceClose;

            this.type = type;
        }

        public TradePosition(long timestampOpen, double priceOpen, PositionType type, string instrument)
        {
            this.instrument = instrument;
            this.timestampOpen = timestampOpen;
            this.priceOpen = priceOpen;
            this.type = type;
        }

        public double getDifference()
        {
            if (priceClose == -1)
                throw new Exception("TradePosition getDifference: Position not yet closed");

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

        public double getDifference(Tickdata data)
        {
            return getDifference(data.bid, data.ask);
        }

        public long timestampOpen = -1;
        public long timestampClose = -1;
        public double priceOpen;
        public double priceClose = -1;
        public PositionType type;
        public string instrument;

        public enum PositionType{
            longPosition, shortPosition
        }
    }
}
