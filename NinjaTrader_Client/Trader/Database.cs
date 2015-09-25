using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Newtonsoft.Json.Linq;
using NinjaTrader_Client.Model;
using NinjaTrader_Client.Trader.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader
{
    public class Database
    {
        public int errors = 0;
        MongoFacade mongodb;
        public Database(MongoFacade mongoDbFacade)
        {
            mongodb = mongoDbFacade;
        }

        public Tickdata getPrice(long timestamp, string instrument)
        {
            var collection = mongodb.getCollection(instrument);

            var docsDarunter = collection.Find(Query.LT("timestamp", timestamp + 1)).SetSortOrder(SortBy.Descending("timestamp")).SetLimit(1);
            BsonDocument darunter = docsDarunter.ToList<BsonDocument>()[0];

            return new Tickdata(darunter["timestamp"].AsInt64, darunter["last"].AsDouble, darunter["bid"].AsDouble, darunter["ask"].AsDouble);
        }

        public List<Tickdata> getPrices(long startTimestamp, long endTimestamp, string instrument)
        {
            var collection = mongodb.getCollection(instrument);
            var docs = collection.Find(Query.And(Query.LT("timestamp", endTimestamp + 1), Query.GT("timestamp", startTimestamp - 1))).SetSortOrder(SortBy.Ascending("timestamp"));

            List<Tickdata> output = new List<Tickdata>();
            foreach (BsonDocument doc in docs)
            {
                output.Add(new Tickdata(doc["timestamp"].AsInt64, doc["last"].AsDouble, doc["bid"].AsDouble, doc["ask"].AsDouble));
            }

            return output;
        }

        public void setPrice(Tickdata td, string instrument)
        {
            try
            {
                BsonDocument input = new BsonDocument();
                input.SetElement(new BsonElement("timestamp", td.timestamp));
                input.SetElement(new BsonElement("last", td.last));
                input.SetElement(new BsonElement("bid", td.bid));
                input.SetElement(new BsonElement("ask", td.ask));

                mongodb.getCollection(instrument).Insert(input);
            }
            catch { errors++; }
        }

        public void setIndicator(IndicatorData data, string indicatorName, string instrument) 
        {
            try
            {
                BsonDocument input = new BsonDocument();
                input.SetElement(new BsonElement("timestamp", data.timestamp));
                input.SetElement(new BsonElement("value", data.value));

                mongodb.getCollection(instrument + "_" + indicatorName).Insert(input);
            }
            catch { errors++; }
        }

        public IndicatorData getIndicator(long timestamp, string indicatorName, string instrument)
        {
            var collection = mongodb.getCollection(instrument + "_" + indicatorName);

            var docsDarunter = collection.Find(Query.LT("timestamp", timestamp + 1L)).SetSortOrder(SortBy.Descending("timestamp")).SetLimit(1);
            BsonDocument darunter = docsDarunter.ToList<BsonDocument>()[0];

            return new IndicatorData(darunter["timestamp"].AsInt64, darunter["value"].AsDouble);
        }

        public List<IndicatorData> getIndicators(long startTimestamp, long endTimestamp, string indicatorName, string instrument)
        {
            var collection = mongodb.getCollection(instrument + "_" + indicatorName);
            var docs = collection.Find(Query.And(Query.LT("timestamp", endTimestamp + 1L), Query.GT("timestamp", startTimestamp - 1L))).SetSortOrder(SortBy.Ascending("timestamp"));

            List<IndicatorData> output = new List<IndicatorData>();
            foreach (BsonDocument doc in docs)
            {
                output.Add(new IndicatorData(doc["timestamp"].AsInt64, doc["value"].AsDouble));
            }

            return output;
        }

        public BsonDocument getExportData(long startTime)
        {
            BsonDocument exportData = new BsonDocument();

            List<MongoCollection<BsonDocument>> list = mongodb.getCollections();
            foreach (MongoCollection<BsonDocument> collection in list)
            {
                if (collection.Name.Contains("system.indexes") == false)
                {
                    var docs = collection.Find(Query.GT("timestamp", startTime - 1L)).SetSortOrder(SortBy.Ascending("timestamp"));

                    var array = new BsonArray();
                    foreach (var item in docs)
                    {
                        array.Add(item);
                    }

                    exportData[collection.Name] = array;
                }
            }

            return exportData;
        }

        public void importData(string data)
        {
            var options = new MongoInsertOptions() { Flags = InsertFlags.ContinueOnError };

            BsonDocument doc = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(data);
            foreach(string name in doc.Names)
            {
                var result = mongodb.getCollection(name).InsertBatch(doc[name].AsBsonArray, options);
                    //.Insert(doc[name].AsBsonArray); //Untested
            }

            /*int errors = 0;
            foreach(string name in doc.Names)
            {
                foreach (BsonDocument entry in doc[name].AsBsonArray)
                    try
                    {
                        mongodb.getCollection(name).Insert(entry);
                    }
                    catch { errors++;  }
            }*/
        }

        public long getSetsCount()
        {
            long all = 0;
            foreach (MongoCollection<BsonDocument> collection in mongodb.getCollections())
                all += collection.Count(); //Could be done more effictient by counting inserts

            return all;
        }

        public long getLastTimestamp()
        {
            long lastTimestamp = 0;
            List<MongoCollection<BsonDocument>> list = mongodb.getCollections();
            foreach (MongoCollection<BsonDocument> collection in list)
            {
                if (collection.Name.Contains("system.indexes") == false)
                {
                    var docs = collection.Find(Query.Exists("timestamp")).SetSortOrder(SortBy.Descending("timestamp")).SetLimit(1);
                    long currentTs = docs.ToList<BsonDocument>()[0]["timestamp"].AsInt64;

                    if (currentTs > lastTimestamp)
                        lastTimestamp = currentTs;
                }
            }

            return lastTimestamp;
        }

        public void megrate()
        {
            //Implement migrations ???
            List<MongoCollection<BsonDocument>> list = mongodb.getCollections();

            //Create Index
            /*foreach (MongoCollection<BsonDocument> collection in list)
            {
                if (collection.Name.Contains("system.indexes") == false)
                    collection.CreateIndex("timestamp");
            }*/

            //SSI vor 1442832915118 muss - 0.5 und dann * 2!
            //Hat nicht funktioniert!
            /*foreach (MongoCollection<BsonDocument> collection in list)
            {
                if (collection.Name.EndsWith("_ssi-mt4"))
                {
                    var docs = collection.Find(Query.LT("timestamp", 1442883030534));

                    foreach (BsonDocument doc in docs)
                    {
                        double value = doc["value"].AsDouble;
                        value -= 0.5d;
                        value = value * 2;

                        collection.Update(Query.EQ("timestamp", doc["timestamp"].AsInt64), Update.Set("value", value));
                    }
                }
            }*/

            //Lösche Daten von SSI-win vor 1442883027534
            /*foreach(MongoCollection<BsonDocument> collection in list)
            {
                if(collection.Name.EndsWith("_ssi-win-mt4"))
                    collection.Remove(Query.LT("timestamp", 1442883030534));
            }*/

            /*foreach (MongoCollection<BsonDocument> collection in list)
            {
                if (collection.Name.EndsWith("_ssi-mt4"))
                    collection.Remove(Query.LT("timestamp", 1442883104095));
            }*/

            int i = 0;
        }
    }
}
