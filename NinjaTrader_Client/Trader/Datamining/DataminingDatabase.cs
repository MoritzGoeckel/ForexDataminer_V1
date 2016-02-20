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
        void addData(string dataname, Database database);
        void addMetaIndicatorSum(string[] ids, double[] weights, string fieldName);
        void addMetaIndicatorDifference(string id, string id_subtract, string fieldName);
        void addOutcomeCode(double percentDifference, int outcomeTimeframeSeconds);

        void deleteAll();

        //Bound to an instrument
        void addIndicator(WalkerIndicator indicator, string instrument, string fieldId);
        string getOutcomeIndicatorSampling(double min, double max, int steps, string indicatorId, int outcomeTimeframeSeconds, string instument = null);

        //Not yet implemented
        void getCorrelation(string indicatorId, int outcomeTimeframe, CorrelationCondition condition);
        void getCorrelationTable();

        //Trivial
        ProgressDict getProgress();

        //Come up with a better Name! ???
        void doMachineLearning(string[] inputFields, string outcomeField, string instrument, string savePath = null);

        string getInfo();
        //void trainNeuralNetwork(ActivationNetwork ann, int iterations)
    }
}
