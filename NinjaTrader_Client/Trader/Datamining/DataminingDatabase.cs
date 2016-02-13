using NinjaTrader_Client.Trader.Indicators;
using NinjaTrader_Client.Trader.MainAPIs;
using NinjaTrader_Client.Trader.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader.Datamining
{
    public interface DataminingDatabase
    {
        void importPair(string pair, long start, long end, Database database);
        void addOutcome(long timeframeSeconds);
        void addIndicator(WalkerIndicator indicator, string instrument, string id);
        void addData(string dataname, Database database);
        void addMetaIndicator(string[] ids, double[] weights, string id);
        void getCorrelation(string indicatorId, int outcomeTimeframe, CorrelationCondition condition);
        void getCorrelationTable();
        string getOutcomeIndicatorSampling(double min, double max, int steps, string indicatorId, int outcomeTimeframeSeconds, string instument = null);

        ProgressDict getProgress();
    }
}
