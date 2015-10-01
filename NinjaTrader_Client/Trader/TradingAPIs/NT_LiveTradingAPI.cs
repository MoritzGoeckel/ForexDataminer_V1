using NinjaTrader_Client.Model;
using NinjaTrader_Client.Trader.TradingAPIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader.TradingAPI
{
    class NT_LiveTradingAPI : ITradingAPI
    {
        private NinjaTraderAPI api;

        public NT_LiveTradingAPI(NinjaTraderAPI api)
        {
            this.api = api;
            api.tickdataArrived += api_tickdataArrived;
        }

        private Dictionary<string, PairData> pairData = new Dictionary<string,PairData>();

        void api_tickdataArrived(NinjaTrader_Client.Model.Tickdata data, string instrument)
        {
            if (pairData.ContainsKey(instrument) == false)
                pairData.Add(instrument, new PairData());

            pairData[instrument].lastTickData = data;
        }

        public override bool openLong(string instrument)
        {
            if (api.submitOrder(instrument, Model.TradePosition.PositionType.longPosition, 1, pairData[instrument].lastTickData.ask))
            {
                pairData[instrument].lastLongPosition = new Model.TradePosition(getNow(), pairData[instrument].lastTickData.ask, Model.TradePosition.PositionType.longPosition, instrument);
                return true;
            }
            return false;
        }

        public override bool openShort(string instrument)
        {
            if (api.submitOrder(instrument, Model.TradePosition.PositionType.shortPosition, 1, pairData[instrument].lastTickData.bid))
            {
                pairData[instrument].lastShortPosition = new Model.TradePosition(getNow(), pairData[instrument].lastTickData.bid, Model.TradePosition.PositionType.shortPosition, instrument);
                return true;
            }
            else
                return false;
        }

        public override bool closePositions(string instrument)
        {
            return api.closePosition(instrument);
        }

        public override bool closeShort(string instrument)
        {
            return closePositions(instrument);
        }

        public override bool closeLong(string instrument)
        {
            return closePositions(instrument);
        }

        public override double getBid(string instrument)
        {
            if (pairData.ContainsKey(instrument))
                return pairData[instrument].lastTickData.bid;
            else
                return -1;
        }

        public override double getAsk(string instrument)
        {
            if (pairData.ContainsKey(instrument))
                return pairData[instrument].lastTickData.ask;
            else
                return -1;
        }

        public override long getNow()
        {
            return Timestamp.getNow();
        }

        public override Model.TradePosition getLongPosition(string instrument)
        {
            if (pairData.ContainsKey(instrument))
                return pairData[instrument].lastLongPosition;
            else
                return null;
        }

        public override Model.TradePosition getShortPosition(string instrument)
        {
            if (pairData.ContainsKey(instrument))
                return pairData[instrument].lastShortPosition;
            else
                return null;
        }

        public override List<Model.TradePosition> getHistory(string instrument)
        {
            throw new NotImplementedException();
        }

        public override bool isUptodate(string instrument)
        {
            if (pairData.ContainsKey(instrument) && pairData[instrument].lastTickData != null && getNow() - pairData[instrument].lastTickData.timestamp < 1000 * 60 * 3)
                return true;
            else
                return false;
        }
    }
}
