using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Newtonsoft.Json.Linq;
using NinjaTrader_Client.Trader.Model;
using NinjaTrader_Client.Trader.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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

        private Dictionary<string, Tickdata> cachedPrices = new Dictionary<string, Tickdata>();
        private long chacheLastClearedTimestamp = 0;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Tickdata getPrice(long timestamp, string instrument)
        {
            if (Timestamp.getNow() - chacheLastClearedTimestamp > 1000 * 60 * 60 * 1) //Nach einer Stunde, verhindert überlaufen
            {
                chacheLastClearedTimestamp = Timestamp.getNow();
                cachedPrices.Clear();
            }

            if (cachedPrices.ContainsKey(timestamp + instrument)) //Simple Caching
                return cachedPrices[timestamp + instrument];
            else
            {
                var collection = mongodb.getCollection(instrument);

                var docsDarunter = collection.Find(Query.LT("timestamp", timestamp + 1)).SetSortOrder(SortBy.Descending("timestamp")).SetLimit(1);
                BsonDocument darunter = docsDarunter.ToList<BsonDocument>()[0];

                Tickdata data = new Tickdata(darunter["timestamp"].AsInt64, darunter["last"].AsDouble, darunter["bid"].AsDouble, darunter["ask"].AsDouble);
                cachedPrices.Add(timestamp + instrument, data);

                return data;
            }
        }

        //Chache too?
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

        public void setData(TimeValueData data, string dataName, string instrument) 
        {
            try
            {
                BsonDocument input = new BsonDocument();
                input.SetElement(new BsonElement("timestamp", data.timestamp));
                input.SetElement(new BsonElement("value", data.value));

                mongodb.getCollection(instrument + "_" + dataName).Insert(input);
            }
            catch { errors++; }
        }

        //cache it too?
        public TimeValueData getData(long timestamp, string dataName, string instrument)
        {
            var collection = mongodb.getCollection(instrument + "_" + dataName);

            var docsDarunter = collection.Find(Query.LT("timestamp", timestamp + 1L)).SetSortOrder(SortBy.Descending("timestamp")).SetLimit(1);
            BsonDocument darunter = docsDarunter.ToList<BsonDocument>()[0]; //Will throw interrupt when the timestamp is to early in history. Should be handled ???

            return new TimeValueData(darunter["timestamp"].AsInt64, darunter["value"].AsDouble);
        }

        public List<TimeValueData> getDataInRange(long startTimestamp, long endTimestamp, string dataName, string instrument)
        {
            var collection = mongodb.getCollection(instrument + "_" + dataName);
            var docs = collection.Find(Query.And(Query.LT("timestamp", endTimestamp + 1L), Query.GT("timestamp", startTimestamp - 1L))).SetSortOrder(SortBy.Ascending("timestamp"));

            List<TimeValueData> output = new List<TimeValueData>();
            foreach (BsonDocument doc in docs)
            {
                output.Add(new TimeValueData(doc["timestamp"].AsInt64, doc["value"].AsDouble));
            }

            return output;
        }

        public void exportData(long startTimestamp, string basePath)
        {
            if (Directory.Exists(basePath + "/export") == false)
                Directory.CreateDirectory(basePath + "/export");

            string now = Timestamp.getNow().ToString();
            string nowReadable = DateTime.Now.ToString("yyyy-MM-dd");

            int i = 0;
            foreach(MongoCollection<BsonDocument> collection in mongodb.getCollections())
            {
                if (collection.Name.Contains("system.indexes") == false)
                {
                    string export_str = getExportData(startTimestamp, collection).ToString();
                    export_str = StringCompressor.CompressString(export_str);

                    File.WriteAllText(basePath + "/export/database_" + nowReadable + "_" + startTimestamp + "_" + now + "_PART-" + i + ".json", export_str); //1443611923418
                    i++;
                }
            }
        }

        private BsonDocument getExportData(long startTime, MongoCollection<BsonDocument> collection)
        {
            BsonDocument exportData = new BsonDocument();
            
            var docs = collection.Find(Query.GT("timestamp", startTime - 1L)).SetSortOrder(SortBy.Ascending("timestamp"));

            var array = new BsonArray();
            foreach (var item in docs)
            {
                array.Add(item);
            }

            exportData[collection.Name] = array;

            return exportData;
        }

        public void importData(string data)
        {
            var options = new MongoInsertOptions() { Flags = InsertFlags.ContinueOnError };

            BsonDocument doc = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(data);
            foreach(string name in doc.Names)
            {
                try
                {
                    var result = mongodb.getCollection(name).InsertBatch(doc[name].AsBsonArray, options);
                }
                catch { errors++; }
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
                if (collection.Name.Contains("system.indexes") == false && collection.Name.Contains("_") == false)
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

            //remove bid and ask == 0
        }
    }
}
