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

        public override string getName()
        {
            return "SSI-Strategy";
        }

        private long pauseTil = 0;
        public override void doTick()
        {
            double ssi = database.getIndicator(api.getNow(), "ssi-mt4", instrument).value;

            if (api.getNow() > pauseTil)
            {
                if (ssi > 0.2d)
                {
                    if (api.getShortPosition() == null)
                        api.openShort();
                }

                if (ssi < -0.2d)
                {
                    if (api.getLongPosition() == null)
                        api.openLong();
                }
            }

            if (api.getLongPosition() != null && ssi > 0.1d)
                api.closePositions();

            if (api.getShortPosition() != null && ssi < -0.1d)
                api.closePositions();

            /*if (api.getLongPosition() != null && api.getAsk() - api.getLongPosition().priceOpen >= 0.003)
            {
                api.closePositions();
                pauseTil = api.getNow() + 1000 * 60 * 10; 
            }

            if (api.getShortPosition() != null && api.getShortPosition().priceOpen - api.getBid() >= 0.003)
            {
                api.closePositions();
                pauseTil = api.getNow() + 1000 * 60 * 10; 
            }*/

            //Verhindere starkes minus durch pausieren bei -5 pips
            if (api.getLongPosition() != null && api.getAsk() - api.getLongPosition().priceOpen <= -0.005)
            {
                api.closePositions();
                pauseTil = api.getNow() + 1000 * 60 * 60; 
            }

            if (api.getShortPosition() != null && api.getShortPosition().priceOpen - api.getBid() <= -0.005)
            {
                api.closePositions();
                pauseTil = api.getNow() + 1000 * 60 * 60; 
            }
        }
    }
}
