using NinjaTrader_Client.Trader.Backtest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader.Strategies
{
    public class SSIStrategy : Strategy
    {
        public SSIStrategy(Database database) : base(database)
        {
            
        }

        public override Strategy copy()
        {
            return new SSIStrategy(database);
        }

        public override string getName()
        {
            return "SSIStrategy";
        }

        public override BacktestResult addCustomVariables(BacktestResult given)
        {
            //Todo: !!
            return given;
        }

        public override void reset()
        {
            
        }

        private long pauseTil = 0;
        public override void doTick(string instrument)
        {
            if (api.isUptodate(instrument) == false)
                return;

            double ssi = database.getData(api.getNow(), "ssi-mt4", instrument).value;

            if (api.getNow() > pauseTil)
            {
                if (ssi > 0.2d)
                {
                    if (api.getShortPosition(instrument) == null)
                        api.openShort(instrument);
                }

                if (ssi < -0.2d)
                {
                    if (api.getLongPosition(instrument) == null)
                        api.openLong(instrument);
                }
            }

            if (api.getLongPosition(instrument) != null && ssi > 0.1d)
                api.closePositions(instrument);

            if (api.getShortPosition(instrument) != null && ssi < -0.1d)
                api.closePositions(instrument);

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
            /*if (api.getLongPosition() != null && api.getAsk() - api.getLongPosition().priceOpen <= -0.005)
            {
                api.closePositions();
                pauseTil = api.getNow() + 1000 * 60 * 60; 
            }

            if (api.getShortPosition() != null && api.getShortPosition().priceOpen - api.getBid() <= -0.005)
            {
                api.closePositions();
                pauseTil = api.getNow() + 1000 * 60 * 60; 
            }*/
        }
    }
}
