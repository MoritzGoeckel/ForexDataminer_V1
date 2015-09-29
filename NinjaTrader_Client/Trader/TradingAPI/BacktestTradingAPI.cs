using NinjaTrader_Client.Model;
using NinjaTrader_Client.Trader.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader
{
    class BacktestTradingAPI : TradingAPI
    {
        private TradePosition lastShortPosition, lastLongPosition;
        private List<TradePosition> oldPositions = new List<TradePosition>();
        private long now;
        private Database database;
        private string instrument;
        private double bid, ask;

        public BacktestTradingAPI(long startTimestamp, Database database, string instrument)
        {
            now = startTimestamp;
            this.database = database;
            this.instrument = instrument;
        }

        private bool uptodate = false;
        public void setNow(long now)
        {
            this.now = now;
            Tickdata data = database.getPrice(now, instrument);
            this.bid = data.bid;
            this.ask = data.ask;

            if (now - data.timestamp > 1000 * 60 * 3) //Wenn daten älter als 3 minuten -> Nicht mehr uptodate
                uptodate = false;
            else
                uptodate = true;
        }

        public override bool isUptodate()
        {
            return uptodate;
        }

        public override bool openLong()
        {
            if (lastLongPosition != null)
                return false;

            lastLongPosition = new TradePosition(now, getAsk(), TradePosition.PositionType.longPosition);
            return true;
        }

        public override bool openShort()
        {
            if (lastShortPosition != null)
                return false;

            lastShortPosition = new TradePosition(now, getBid(), TradePosition.PositionType.shortPosition);
            return true;
        }

        public override bool closePositions()
        {
           return closeLong() || closeShort();
        }

        public override bool closeLong()
        {
            if (lastLongPosition != null)
            {
                lastLongPosition.timestampClose = now;
                lastLongPosition.priceClose = database.getPrice(now, instrument).bid;
                oldPositions.Add(lastLongPosition);
                lastLongPosition = null;
                return true;
            }
            else
                return false;
        }

        public override bool closeShort()
        {
            if (lastShortPosition != null)
            {
                lastShortPosition.timestampClose = now;
                lastShortPosition.priceClose = database.getPrice(now, instrument).ask;
                oldPositions.Add(lastShortPosition);
                lastShortPosition = null;
                return true;
            }
            else
                return false;
        }

        public override double getBid()
        {
            return bid;
        }

        public override double getAsk()
        {
            return ask;
        }

        public override long getNow()
        {
            return now;
        }

        public override TradePosition getLongPosition()
        {
            return lastLongPosition; //Better copy
        }

        public override TradePosition getShortPosition()
        {
            return lastShortPosition; //Better copy
        }

        public override List<TradePosition> getHistory()
        {
            return oldPositions;
        }

        public override string getInstrument()
        {
            return instrument;
        }
    }
}
