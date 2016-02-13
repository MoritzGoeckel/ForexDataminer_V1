using NinjaTrader_Client.Trader.Indicators;
using NinjaTrader_Client.Trader.MainAPIs;
using NinjaTrader_Client.Trader.Model;
using NinjaTrader_Client.Trader.TradingAPIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader.TradingAPIs
{
    class BacktestTradingAPI : ITradingAPI
    {
        private Dictionary<string, PairData> pairData = new Dictionary<string, PairData>();
        private Database database;
        private long now;

        private WalkerIndicator tradingTime;

        public BacktestTradingAPI(long startTimestamp, Database database, List<string> tradablePairs)
        {
            tradingTime = new TradingTimeIndicator();
            now = startTimestamp;
            this.database = database;
            foreach (string pair in tradablePairs)
                pairData.Add(pair, new PairData());
        }

        public void setNow(long now)
        {
            this.now = now;
            foreach (KeyValuePair<string, PairData> pair in pairData)
            {
                if (tradingTime.getIndicator(now, 0).value != 0)  //No calculations on weekends ???
                { 
                    Tickdata tickData = database.getPrice(now, pair.Key);

                    if (tickData != null)
                        pairData[pair.Key].lastTickData = tickData;
                }
                else
                {
                    pairData[pair.Key].lastTickData.timestamp = now;
                }
            }
        }

        public override bool isUptodate(string instrument)
        {
            if (pairData.ContainsKey(instrument) == false)
                return false;

            if (pairData[instrument] == null)
                return false;

            Tickdata data = pairData[instrument].lastTickData;
            if (data != null)
                return now - data.timestamp < 1000 * 60 * 3;
            else
                return false;
        }

        public override bool openLong(string instrument)
        {
            if (pairData[instrument].lastLongPosition != null)
                return false;

            pairData[instrument].lastLongPosition = new TradePosition(now, getAsk(instrument), TradePosition.PositionType.longPosition, instrument);
            return true;
        }

        public override bool openShort(string instrument)
        {
            if (pairData[instrument].lastShortPosition != null)
                return false;

            pairData[instrument].lastShortPosition = new TradePosition(now, getBid(instrument), TradePosition.PositionType.shortPosition, instrument);
            return true;
        }

        public override bool closePositions(string instrument)
        {
           return closeLong(instrument) || closeShort(instrument);
        }

        public override bool closeLong(string instrument)
        {
            TradePosition lastLong = pairData[instrument].lastLongPosition;
            if (lastLong != null)
            {
                lastLong.timestampClose = now;
                lastLong.priceClose = getBid(instrument);
                pairData[instrument].oldPositions.Add(lastLong);
                pairData[instrument].lastLongPosition = null;
                return true;
            }
            else
                return false;
        }

        public override bool closeShort(string instrument)
        {
            TradePosition lastShort = pairData[instrument].lastShortPosition;
            if (lastShort != null)
            {
                lastShort.timestampClose = now;
                lastShort.priceClose = getAsk(instrument);
                pairData[instrument].oldPositions.Add(lastShort);
                pairData[instrument].lastShortPosition = null;
                return true;
            }
            else
                return false;
        }

        public override double getBid(string instrument)
        {
            return pairData[instrument].lastTickData.bid;
        }

        public override double getAsk(string instrument)
        {
            return pairData[instrument].lastTickData.ask;
        }

        public override long getNow()
        {
            return now;
        }

        public override TradePosition getLongPosition(string instrument)
        {
            if (pairData.ContainsKey(instrument))
                return pairData[instrument].lastLongPosition; //Better copy
            else
                return null;
        }

        public override TradePosition getShortPosition(string instrument)
        {
            if (pairData.ContainsKey(instrument))
                return pairData[instrument].lastShortPosition; //Better copy
            else
                return null;
        }

        public override List<TradePosition> getHistory(string instrument)
        {
            return pairData[instrument].oldPositions;
        }
    }
}
