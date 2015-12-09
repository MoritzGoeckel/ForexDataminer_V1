using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader
{
    public class MongoFacade
    {
        private MongoDB.Driver.MongoDatabase database;
        private MongoServer server;
        private MongoClient client;
        private Process serverProcess = null;

        private Dictionary<string, MongoCollection> collections;

        public MongoFacade(string exePath, string dataPath, string dbName)
        {
            //If mongod not running
            Process[] pname = Process.GetProcessesByName("mongod");
            if (pname.Length == 0)
            {
                //Start mongod
                serverProcess = new Process();
                serverProcess.StartInfo.FileName = exePath;
                serverProcess.StartInfo.Arguments = "--dbpath \"" + dataPath + "\"";

                serverProcess.Start();
            }

            client = new MongoClient();
            server = client.GetServer();
            database = server.GetDatabase(dbName);
        }

        public MongoDB.Driver.MongoDatabase getDatabase()
        {
            return database;
        }

        public MongoCollection getCollection(String name){
            if(collections == null)
                collections = new Dictionary<string, MongoCollection>();

            if(collections.ContainsKey(name) == false)
                collections.Add(name, database.GetCollection(name));

            return collections[name];
        }

        public List<MongoCollection> getCollections()
        {
            List<MongoCollection> list = new List<MongoCollection>();
            foreach(string collectionName in database.GetCollectionNames())
                list.Add(getCollection(collectionName));

            return list;
        }

        public void shutdown()
        {
            //If he started the server
            if (serverProcess != null)
            {
                server.Shutdown();
                while (serverProcess.HasExited == false)
                {
                    serverProcess.Kill();
                    System.Threading.Thread.Sleep(300);
                }
            }
        }

        /*
            //Möglichst Nahes finden
            var docsDarunter = collection.Find(Query.LT("nr", 4)).SetSortOrder(SortBy.Descending("nr")).SetLimit(1);
            BsonDocument darunter = docsDarunter.ToList<BsonDocument>()[0];

            var docsDarüber = collection.Find(Query.GT("nr", 4)).SetSortOrder(SortBy.Ascending("nr")).SetLimit(1);
            BsonDocument darüber = docsDarüber.ToList<BsonDocument>()[0];

            Console.WriteLine(darunter["str"] + " " + darüber["str"]);



            //Bereich auslesen
            var docsBerreich = collection.Find(Query.And(Query.GT("nr", 1), Query.LT("nr", 5))).SetSortOrder(SortBy.Ascending("nr"));
            foreach (var d in docsBerreich)
                Console.WriteLine(d["str"]);



            //Count
            Console.WriteLine("Count: " + collection.FindAll().Count());
        */

        //Insert
        /*BsonDocument doc = new BsonDocument();
        doc["nr"] = 5;
        doc["str"] = "fünf";

        collection.Insert(doc);*/

        //db["collection"].Find().SetSortOrder(SortBy.Ascending("SortByMe"));

        //http://docs.mongodb.org/manual/reference/operator/query-comparison/
        //gt greater
        //lt less
        //gte greater or equal
        //eq equal
    }
}
