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
        }

        public override bool openLong(string instrument)
        {
            
        }

        public override bool openShort(string instrument)
        {
            throw new NotImplementedException();
        }

        public override bool closePositions(string instrument)
        {
            throw new NotImplementedException();
        }

        public override bool closeShort(string instrument)
        {
            throw new NotImplementedException();
        }

        public override bool closeLong(string instrument)
        {
            throw new NotImplementedException();
        }

        public override double getBid(string instrument)
        {
            throw new NotImplementedException();
        }

        public override double getAsk(string instrument)
        {
            throw new NotImplementedException();
        }

        public override long getNow()
        {
            throw new NotImplementedException();
        }

        public override Model.TradePosition getLongPosition(string instrument)
        {
            throw new NotImplementedException();
        }

        public override Model.TradePosition getShortPosition(string instrument)
        {
            throw new NotImplementedException();
        }

        public override List<Model.TradePosition> getHistory(string instrument)
        {
            throw new NotImplementedException();
        }

        public override bool isUptodate(string instrument)
        {
            throw new NotImplementedException();
        }
    }
}
