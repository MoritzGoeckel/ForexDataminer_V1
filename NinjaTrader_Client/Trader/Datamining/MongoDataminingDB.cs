using NinjaTrader_Client.Trader.Datamining;
using NinjaTrader_Client.Trader.MainAPIs;
using NinjaTrader_Client.Trader.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using NinjaTrader_Client.Trader.Indicators;
using System.Threading;
using MongoDB.Driver.Builders;
using MongoDB.Bson;
using MongoDB.Driver;
using NinjaTrader_Client.Trader.Utils;

namespace NinjaTrader_Client.Trader
{
    public class MongoDataminingDB : DataminingDatabase
    {

        MongoDatabase database;
        MongoFacade mongodb;

        int threadsCount = 12;

        ProgressDict progress = new ProgressDict();

        Random z = new Random();
        
        public MongoDataminingDB(MongoFacade mongoDbFacade)
        {
            mongodb = mongoDbFacade;
            database = new MongoDatabase(mongodb);
        }

        void DataminingDatabase.importPair(string pair, long start, long end, Database otherDatabase)
        {
            long timeframe = Convert.ToInt64(Convert.ToDouble(end - start) / Convert.ToDouble(threadsCount));
            int threadId = 0;
            
            while (threadId < threadsCount)
            {
                long threadBeginning = start + timeframe * threadId;
                long threadEnd = threadBeginning + timeframe;

                new Thread(delegate ()
                {
                    string name = "import " + " ID_" + threadBeginning + ":" + threadEnd;
                    progress.setProgress(name, 0);
                    int done = 0;

                    List<Tickdata> data = otherDatabase.getPrices(threadBeginning, threadEnd, pair);
                    long count = data.Count();

                    foreach (Tickdata d in data)
                    { 
                        database.setPrice(d, pair);
                        done++;
                        progress.setProgress(name, Convert.ToInt32(Convert.ToDouble(done) / Convert.ToDouble(count) * 100d));
                    }

                    progress.remove(name);
                }).Start();

                threadId++;
            }
        }

        void DataminingDatabase.addOutcome(long timeframeSeconds)
        {
            //Do in walker? Faster? Todo...
            //{ outcome_max_1800: { $exists: true } }

            long start = database.getFirstTimestamp();
            long end = database.getLastTimestamp();

            long timeframe = (end - start) / threadsCount;
            int threadId = 0;

            var collection = mongodb.getDB().GetCollection("prices");

            while (threadId < threadsCount)
            {
                long threadBeginning = start + (timeframe * threadId);
                long threadEnd = threadBeginning + timeframe;

                new Thread(delegate () {

                    string name = "outcome " + timeframeSeconds + " ID_" + threadBeginning + ":" + threadEnd;
                    progress.setProgress(name, 0);
                    int done = 0;
                    long count = 0;

                    var docs = collection.FindAs<BsonDocument>(Query.And(Query.LT("timestamp", threadEnd), Query.GTE("timestamp", threadBeginning))).SetSortOrder(SortBy.Ascending("timestamp"));
                    docs.SetFlags(QueryFlags.NoCursorTimeout);

                    count = docs.Count();
                    foreach (BsonDocument doc in docs)
                    {
                        done++;
                        progress.setProgress(name, Convert.ToInt32(Convert.ToDouble(done) / Convert.ToDouble(count) * 100d));

                        try
                        {
                            if (doc.ContainsValue("outcome_max_" + timeframeSeconds))
                                continue;

                            Tickdata inTimeframe = database.getPrice(doc["timestamp"].AsInt64 + (timeframeSeconds * 1000), doc["instrument"].AsString);

                            var min_doc = collection.FindAs<BsonDocument>(Query.And(Query.EQ("instrument", doc["instrument"].AsString), Query.LT("timestamp", doc["timestamp"].AsInt64 + (timeframeSeconds * 1000)), Query.GT("timestamp", doc["timestamp"].AsInt64)))
                             .SetSortOrder(SortBy.Ascending("bid"))
                             .SetLimit(1)
                             .SetFields("bid")
                             .Single();

                            var max_doc = collection.FindAs<BsonDocument>(Query.And(Query.EQ("instrument", doc["instrument"].AsString), Query.LT("timestamp", doc["timestamp"].AsInt64 + (timeframeSeconds * 1000)), Query.GT("timestamp", doc["timestamp"].AsInt64)))
                             .SetSortOrder(SortBy.Descending("ask"))
                             .SetLimit(1)
                             .SetFields("ask")
                             .Single();

                            string s = max_doc["ask"].AsDouble.ToString();
                            s = s + "";

                            collection.FindAndModify(new FindAndModifyArgs()
                            {
                                Query = Query.EQ("_id", doc["_id"]),
                                Update = Update.Combine(
                                        Update.Set("outcome_max_" + timeframeSeconds, max_doc["ask"]),
                                        Update.Set("outcome_min_" + timeframeSeconds, min_doc["bid"]),
                                        Update.Set("outcome_actual_" + timeframeSeconds, inTimeframe.getAvg())
                                    )
                            });
                        }
                        catch { }
                    }

                    progress.remove(name);

                }).Start();

                threadId++;
            }
        }

        public void addData(string dataname, Database database)
        {
            long start = database.getFirstTimestamp();
            long end = database.getLastTimestamp();

            long timeframe = (end - start) / threadsCount;
            int threadId = 0;

            var collection = mongodb.getDB().GetCollection("prices");

            while (threadId < threadsCount)
            {
                long threadBeginning = start + (timeframe * threadId);
                long threadEnd = threadBeginning + timeframe;

                new Thread(delegate () {

                    string name = "data " + dataname + " ID_" + threadBeginning + ":" + threadEnd;
                    progress.setProgress(name, 0);
                    int done = 0;
                    long count = 0;

                    var docs = collection.FindAs<BsonDocument>(Query.And(Query.LT("timestamp", threadEnd), Query.GTE("timestamp", threadBeginning))).SetSortOrder(SortBy.Ascending("timestamp"));
                    docs.SetFlags(QueryFlags.NoCursorTimeout);

                    count = docs.Count();
                    foreach (BsonDocument doc in docs)
                    {
                        done++;
                        progress.setProgress(name, Convert.ToInt32(Convert.ToDouble(done) / Convert.ToDouble(count) * 100d));

                        if (doc.ContainsValue(dataname))
                            continue;

                        try {
                            TimeValueData data = database.getData(doc["timestamp"].AsInt64, dataname, doc["instrument"].AsString);

                            collection.FindAndModify(new FindAndModifyArgs()
                            {
                                Query = Query.EQ("_id", doc["_id"]),
                                Update = Update.Set(dataname, data.value)
                            });
                        }
                        catch { }
                    }

                    progress.remove(name);

                }).Start();

                threadId++;
            }
        }

        string DataminingDatabase.getOutcomeIndicatorSampling(double min, double max, int steps, string indicatorId, int outcomeTimeframeSeconds, string instument)
        {
            string seperator = "\t";

            List<string> returnedValues = new List<string>(); 

            var collection = mongodb.getDB().GetCollection("prices");
            double stepsize = (Convert.ToDouble(max - min) / Convert.ToDouble(steps));

            double current = min;
            while(current <= max)
            {
                double valueMin = current;
                double valueMax = current + stepsize;
                new Thread(delegate ()
                {
                    instument = null;

                    IMongoQuery query = Query.And(Query.Exists(indicatorId), Query.Exists("last"), Query.Exists("outcome_max_" + outcomeTimeframeSeconds), Query.Exists("outcome_min_" + outcomeTimeframeSeconds), Query.LT(indicatorId, valueMax), Query.GTE(indicatorId, valueMin));
                    if (instument != null)
                        query = Query.And(query, Query.EQ("instrument", instument));

                    var docs = collection.FindAs<BsonDocument>(query);
                    double sumMax = 0, sumMin = 0;
                    long count = docs.Count();
                    foreach (BsonDocument doc in docs)
                    {
                        double onePercent = doc["last"].AsDouble / 100d;

                        double maxDiff = doc["outcome_max_" + outcomeTimeframeSeconds].AsDouble / onePercent - 100; //calculate percent difference
                        double minDiff = doc["outcome_min_" + outcomeTimeframeSeconds].AsDouble / onePercent - 100;

                        sumMax += maxDiff;
                        sumMin += minDiff;
                    }

                    if (count == 0)
                        returnedValues.Add(valueMin + seperator + valueMax + seperator + count + seperator + "-" + seperator + "-");
                    else
                        returnedValues.Add(valueMin + seperator + valueMax + seperator + count + seperator + sumMax / count + seperator + sumMin / count);

                }).Start();

                current += stepsize;
            }

            //Darstellungsthread
            while (returnedValues.Count < steps)
                Thread.Sleep(300);

            //Create Graphics -> Todo!! ????
            string output = "MinValue"+ seperator + "MaxValue"+ seperator + "Count"+ seperator + "AvgMax"+ seperator + "AvgMin" + Environment.NewLine;
            foreach(string entry in returnedValues)
            {
                output += entry + Environment.NewLine;
            }

            return output;
        }

        ProgressDict DataminingDatabase.getProgress()
        {
            return progress;
        }

        void DataminingDatabase.addIndicator(WalkerIndicator indicator, string instrument, string fieldId)
        {
            var collection = mongodb.getDB().GetCollection("prices");

            long start = database.getFirstTimestamp();
            long end = database.getLastTimestamp();

            string name = "Indicator " + instrument + " " + fieldId;
            progress.setProgress(name, 0);
            int done = 0;
            long count = 0;

            new Thread(delegate ()
            {
                var docs = collection.FindAs<BsonDocument>(Query.And(Query.Exists(fieldId), Query.EQ("instrument", instrument), Query.LT("timestamp", end), Query.GTE("timestamp", start))).SetSortOrder(SortBy.Ascending("timestamp"));
                docs.SetFlags(QueryFlags.NoCursorTimeout);
                count = docs.Count();

                foreach(var doc in docs)
                {
                    progress.setProgress(name, Convert.ToInt32(Convert.ToDouble(done) / Convert.ToDouble(count) * 100d));
                    done++;

                    indicator.setNextData(doc["timestamp"].AsInt64, doc[fieldId].AsDouble);

                    collection.FindAndModify(new FindAndModifyArgs()
                    {
                        Query = Query.EQ("_id", doc["_id"]),
                        Update = Update.Set(indicator.getName() + "_" + fieldId, indicator.getIndicator().value)
                    });
                }

                progress.remove(name);

            }).Start();
        }

        void DataminingDatabase.addMetaIndicator(string[] ids, double[] weights, string fieldName)
        {
            long start = database.getFirstTimestamp();
            long end = database.getLastTimestamp();

            long timeframe = (end - start) / threadsCount;
            int threadId = 0;

            var collection = mongodb.getDB().GetCollection("prices");

            IMongoQuery fieldsExistQuery = Query.NotExists(fieldName);
            foreach (string id in ids)
                fieldsExistQuery = Query.And(fieldsExistQuery, Query.Exists(id));

            while (threadId < threadsCount)
            {
                long threadBeginning = start + (timeframe * threadId);
                long threadEnd = threadBeginning + timeframe;

                new Thread(delegate () {

                    string name = "Metaindicator" + " ID_" + threadBeginning + ":" + threadEnd;
                    progress.setProgress(name, 0);
                    int done = 0;
                    long count = 0;

                    var docs = collection.FindAs<BsonDocument>(Query.And(fieldsExistQuery, Query.LT("timestamp", threadEnd), Query.GTE("timestamp", threadBeginning)));
                    docs.SetFlags(QueryFlags.NoCursorTimeout);

                    count = docs.Count();
                    foreach (BsonDocument doc in docs)
                    {
                        done++;
                        progress.setProgress(name, Convert.ToInt32(Convert.ToDouble(done) / Convert.ToDouble(count) * 100d));

                        double value = 0;
                        for (int i = 0; i < ids.Length; i++)
                            value += doc[ids[i]].AsDouble * weights[i];

                        collection.FindAndModify(new FindAndModifyArgs()
                        {
                            Query = Query.EQ("_id", doc["_id"]),
                            Update = Update.Set(fieldName, value)
                        });
                    }

                    progress.remove(name);

                }).Start();

                threadId++;
            }
        }

        void DataminingDatabase.getCorrelation(string indicatorId, int outcomeTimeframe, CorrelationCondition condition)
        {
            throw new NotImplementedException();
        }

        void DataminingDatabase.getCorrelationTable()
        {
            throw new NotImplementedException();
        }
    }
}
