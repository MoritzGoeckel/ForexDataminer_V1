using NinjaTrader_Client.Model;
using NinjaTrader_Client.Trader.Backtest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader.Strategies
{
    public class FastMovement_Strategy : Strategy
    {
        public FastMovement_Strategy(Database database, int preTime = 1000 * 60 * 7, int postTime = 1000 * 60 * 60, double threshold = 0.0017, double closeOnWin = 0.0000, double closeOnLoose = 0.0023) : base(database)
        {
            this.threshold = threshold;
            this.postTime = postTime;
            this.preTime = preTime;
            this.closeOnWin = closeOnWin;
            this.closeOnLoose = closeOnLoose;
        }

        public override Strategy copy()
        {
            return new FastMovement_Strategy(database, preTime, postTime, threshold, closeOnWin, closeOnLoose);
        }

        public override string getName()
        {
            return "FastMovement-Strategy";
        }

        public override BacktestResult addCustomVariables(BacktestResult given)
        {
            given.setParameter("PostT", ((double)postTime / 1000d / 60d).ToString());
            given.setParameter("PostT", ((double)preTime / 1000d / 60d).ToString());
            given.setParameter("Threshold", threshold.ToString());
            given.setParameter("CloseOnWin", closeOnWin.ToString());
            given.setParameter("CloseOnLoose", closeOnLoose.ToString());

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

        private double threshold;
        private int preTime, postTime;
        private double closeOnWin, closeOnLoose;

        private int hitSL = 0, hitTP = 0, hitTO = 0;

        public override void doTick()
        {
            //Remove old ones in the back
            //Now the tick old_tickdata[old_tickdata.Count - 1] is 5 or less minutes old
            while (old_tickdata.Count != 0 && api.getNow() - old_tickdata[old_tickdata.Count - 1].timestamp > preTime) //Warum sollte old_tickdata[old_tickdata.Count - 1] == null sein?
                old_tickdata.RemoveAt(old_tickdata.Count - 1);

            Tickdata nowTick = new Tickdata(api.getNow(), 0, api.getBid(), api.getAsk());

            if (old_tickdata.Count != 0)
            {
                Tickdata pastTick = old_tickdata[old_tickdata.Count - 1]; //The tick in preTime

                //New Orders if movement in preTime is > threshold
                //Long
                if (api.getLongPosition() == null && nowTick.bid - pastTick.ask > threshold)
                {
                    api.openLong();
                }

                //Short
                if (api.getShortPosition() == null && pastTick.bid - nowTick.ask > threshold)
                {
                    api.openShort();
                }
            }

            //Close orders after postTime elapsed
            //Long
            if (api.getLongPosition() != null && api.getNow() - api.getLongPosition().timestampOpen > postTime)
            {
                api.closeLong();
                hitTO++;
            }

            //Short
            if (api.getShortPosition() != null && api.getNow() - api.getShortPosition().timestampOpen > postTime)
            {
                api.closeShort();
                hitTO++;
            }

            //Close on win
            if (closeOnWin != 0)
            {
                if (api.getLongPosition() != null && api.getBid() > api.getLongPosition().priceOpen + closeOnWin)
                {
                    api.closeLong();
                    hitTP++;
                }

                if (api.getShortPosition() != null && api.getAsk() + closeOnWin < api.getShortPosition().priceOpen)
                {
                    api.closeShort();
                    hitTP++;
                }
            }

            //Close on loose
            if (closeOnLoose != 0)
            {
                if (api.getLongPosition() != null && api.getBid() + closeOnLoose < api.getLongPosition().priceOpen)
                {
                    api.closeLong();
                    hitSL++;
                }

                if (api.getShortPosition() != null && api.getAsk() > api.getShortPosition().priceOpen + closeOnLoose)
                {
                    api.closeShort();
                    hitSL++;
                }
            }

            //add newest in front
            old_tickdata.Insert(0, nowTick);
        }
    }
}
