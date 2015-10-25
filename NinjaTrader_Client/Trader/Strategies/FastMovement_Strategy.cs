using NinjaTrader_Client.Trader.Backtest;
using NinjaTrader_Client.Trader.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader.Strategies
{
    public class FastMovement_Strategy : Strategy
    {
        public FastMovement_Strategy(Database database, int preTime = 1000 * 60 * 7, int postTime = 1000 * 60 * 60, double thresholdPercent = 0.07, double takeprofitPercent = 0.04, double stoplossPercent = 0.02, bool follow_trend = true)
            : base(database)
        {
            this.thresholdPercent = thresholdPercent;
            this.postTime = postTime;
            this.preTime = preTime;
            this.takeprofitPercent = takeprofitPercent;
            this.stoplossPercent = stoplossPercent;
            this.follow_trend = follow_trend;
        }

        public override Strategy copy()
        {
            return new FastMovement_Strategy(database, preTime, postTime, thresholdPercent, takeprofitPercent, stoplossPercent, follow_trend);
        }

        public override string getName()
        {
            return "FastMovement-Strategy";
        }

        public override BacktestResult addCustomVariables(BacktestResult given)
        {
            given.setParameter("PostT", ((double)postTime / 1000d / 60d).ToString());
            given.setParameter("PreT", ((double)preTime / 1000d / 60d).ToString());
            given.setParameter("Threshold", thresholdPercent.ToString());
            given.setParameter("takeprofit", takeprofitPercent.ToString());
            given.setParameter("stoploss", stoplossPercent.ToString());
            given.setParameter("FollowTrend", follow_trend.ToString());

            //Real results
            given.setResult("HitSL", hitSL.ToString());
            given.setResult("HitTP", hitTP.ToString());
            given.setResult("HitTO", hitTO.ToString());

            return given;
        }

        public override void reset()
        {
            hitSL = 0;
            hitTO = 0;
            hitTP = 0;
            old_tickdata = new List<Tickdata>();
        }

        private List<Tickdata> old_tickdata;

        private double thresholdPercent;
        private int preTime, postTime;
        private double takeprofitPercent, stoplossPercent;
        private bool follow_trend;
        private int hitSL = 0, hitTP = 0, hitTO = 0;

        public override void doTick(string instrument)
        {
            if (api.isUptodate(instrument) == false)
                return;

            double threshold = api.getAvgPrice(instrument) * thresholdPercent / 100d;
            double takeprofit = api.getAvgPrice(instrument) * takeprofitPercent / 100d;
            double stoploss = api.getAvgPrice(instrument) * stoplossPercent / 100d;

            //Remove old ones in the back
            //Now the tick old_tickdata[old_tickdata.Count - 1] is 5 or less minutes old
            while (old_tickdata.Count != 0 && api.getNow() - old_tickdata[old_tickdata.Count - 1].timestamp > preTime) //Warum sollte old_tickdata[old_tickdata.Count - 1] == null sein?
                old_tickdata.RemoveAt(old_tickdata.Count - 1);

            Tickdata nowTick = new Tickdata(api.getNow(), 0, api.getBid(instrument), api.getAsk(instrument));

            if (old_tickdata.Count != 0)
            {
                Tickdata pastTick = old_tickdata[old_tickdata.Count - 1]; //The tick in preTime
                bool longSignal = nowTick.bid - pastTick.ask > threshold;
                bool shortSignal = pastTick.bid - nowTick.ask > threshold;

                if (follow_trend == false)
                {
                    bool oldLong = longSignal;

                    longSignal = shortSignal;
                    shortSignal = oldLong;
                }

                //New Orders if movement in preTime is > threshold
                //Long
                if (api.getLongPosition(instrument) == null && longSignal)
                    api.openLong(instrument);

                //Short
                if (api.getShortPosition(instrument) == null && shortSignal)
                    api.openShort(instrument);

                //Close orders after postTime elapsed
                //Long
                if (api.getLongPosition(instrument) != null && api.getNow() - api.getLongPosition(instrument).timestampOpen > postTime && longSignal == false)
                {
                    api.closeLong(instrument);
                    hitTO++;
                }

                //Short
                if (api.getShortPosition(instrument) != null && api.getNow() - api.getShortPosition(instrument).timestampOpen > postTime && shortSignal == false)
                {
                    api.closeShort(instrument);
                    hitTO++;
                }

                //Close on win
                if (takeprofit != 0)
                {
                    if (api.getLongPosition(instrument) != null && api.getBid(instrument) > api.getLongPosition(instrument).priceOpen + takeprofit && longSignal == false)
                    {
                        api.closeLong(instrument);
                        hitTP++;
                    }

                    if (api.getShortPosition(instrument) != null && api.getAsk(instrument) + takeprofit < api.getShortPosition(instrument).priceOpen && shortSignal == false)
                    {
                        api.closeShort(instrument);
                        hitTP++;
                    }
                }

                //Close on loose
                if (stoploss != 0)
                {
                    if (api.getLongPosition(instrument) != null && api.getBid(instrument) + stoploss < api.getLongPosition(instrument).priceOpen && longSignal == false)
                    {
                        api.closeLong(instrument);
                        hitSL++;
                    }

                    if (api.getShortPosition(instrument) != null && api.getAsk(instrument) > api.getShortPosition(instrument).priceOpen + stoploss && shortSignal == false)
                    {
                        api.closeShort(instrument);
                        hitSL++;
                    }
                }
            }

            //add newest in front
            old_tickdata.Insert(0, nowTick);
        }
    }
}
