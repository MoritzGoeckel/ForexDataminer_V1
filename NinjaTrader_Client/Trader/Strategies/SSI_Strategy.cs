using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader.Strategies
{
    public class SSI_Strategy : Strategy
    {
        public SSI_Strategy(Database database, TradingAPI api, string instrument) : base(database, api, instrument)
        {
            
        }

        public override void doTick()
        {
            double ssi = database.getIndicator(api.getNow(), "ssi-mt4", instrument).value;
            if(ssi > 0.2d)
            {
                if (api.getShortPosition() == null)
                    api.openShort();
            }

            if(ssi < -0.2d)
            {
                if (api.getLongPosition() == null)
                    api.openLong();
            }

            if (ssi > -0.15d && ssi < 0.15d)
                api.closePositions();

            if (api.getLongPosition() != null && api.getAsk() - api.getLongPosition().priceOpen >= 0.0005)
                api.closePositions();

            if (api.getShortPosition() != null && api.getShortPosition().priceOpen - api.getBid() >= 0.0005)
                api.closePositions();
        }
    }
}
