using NinjaTrader_Client.Trader.Backtest;
using NinjaTrader_Client.Trader.MainAPIs;
using NinjaTrader_Client.Trader.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader.Strategies
{
    public class BinaryStrategy : Strategy
    {
        int version = 0;

        private double percentTakeprofit, percentStoploss, thresholdPercent;
        private bool followTrend;
        private int time;
        private int hitSL = 0, hitTP = 0;

        public BinaryStrategy(Database database, double percentStoploss, double percentTakeprofit, int time, double thresholdPercent, bool followTrend)
            : base(database)
        {
            this.percentStoploss = percentStoploss;
            this.percentTakeprofit = percentTakeprofit;
            this.time = time;
            this.thresholdPercent = thresholdPercent;
            this.followTrend = followTrend;
        }

        public BinaryStrategy(Database database, Dictionary<string, string> parameters) 
            : base(database)
        {
            percentStoploss = Double.Parse(parameters["sl"]);
            percentTakeprofit = Double.Parse(parameters["tp"]);
            time = Convert.ToInt32(parameters["time"]);
            followTrend = Boolean.Parse(parameters["followTrend"]);
            thresholdPercent = Double.Parse(parameters["threshold"]);
        }

        public override Strategy copy()
        {
            return new BinaryStrategy(database, percentStoploss, percentTakeprofit, time, thresholdPercent, followTrend);
        }

        public override string getName()
        {
            return "Binary-Strategy" + "_V" + version;
        }

        public override Dictionary<string, string> getParameters()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("tp", percentTakeprofit.ToString());
            parameters.Add("sl", percentStoploss.ToString());
            parameters.Add("time", time.ToString());
            parameters.Add("threshold", thresholdPercent.ToString());
            parameters.Add("followTrend", followTrend.ToString());

            return parameters;
        }

        public override Dictionary<string, string> getResult()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add("hitSl", hitSL.ToString());
            result.Add("hitTp", hitTP.ToString());

            return result;
        }

        private List<Tickdata> old_tickdata = new List<Tickdata>();

        public override void doTick(string instrument)
        {
            if (api.isUptodate(instrument) == false)
                return;

            double takeprofit = api.getAvgPrice(instrument) * percentTakeprofit / 100d;
            double stoploss = api.getAvgPrice(instrument) * percentStoploss / 100d;
            double threshold = api.getAvgPrice(instrument) * thresholdPercent / 100d;

            Tickdata nowTick = new Tickdata(api.getNow(), 0, api.getBid(instrument), api.getAsk(instrument));

            while (old_tickdata.Count != 0 && api.getNow() - old_tickdata[old_tickdata.Count - 1].timestamp > time)
                old_tickdata.RemoveAt(old_tickdata.Count - 1);

            double askSum = 0, bidSum = 0; //Lässt sich optimieren ???
            foreach(Tickdata tick in old_tickdata)
            {
                askSum += tick.ask;
                bidSum += tick.bid;
            }

            double avgBid = bidSum / Convert.ToDouble(old_tickdata.Count);
            double avgAsk = askSum / Convert.ToDouble(old_tickdata.Count);

            if (api.getShortPosition(instrument).getDifference() > takeprofit)
                api.closeShort(instrument);

            if (api.getLongPosition(instrument).getDifference() > takeprofit)
                api.closeLong(instrument);

            if (api.getShortPosition(instrument).getDifference() < -stoploss)
                api.closeShort(instrument);

            if (api.getLongPosition(instrument).getDifference() < -stoploss)
                api.closeLong(instrument);

            if (old_tickdata.Count > 0 && nowTick.timestamp - old_tickdata[old_tickdata.Count - 1].timestamp < time * 0.90)
                return;

            if (api.getShortPosition(instrument) == null && api.getLongPosition(instrument) != null)
            {
                if (nowTick.bid + threshold > avgBid && followTrend)
                    api.openLong(instrument);

                if (nowTick.ask - threshold < avgAsk && followTrend)
                    api.openShort(instrument);

                if (nowTick.bid - threshold < avgBid && followTrend == false)
                    api.openLong(instrument);

                if (nowTick.ask + threshold > avgAsk && followTrend == false)
                    api.openShort(instrument);
            }

            old_tickdata.Insert(0, nowTick);
        }
    }
}
