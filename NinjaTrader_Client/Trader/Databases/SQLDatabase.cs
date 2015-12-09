using MySql.Data.MySqlClient;
using NinjaTrader_Client.Trader.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader.MainAPIs
{
    public class SQLDatabase : Database
    {
        string myConnectionString = "SERVER=localhost;" +
                            "DATABASE=tradingsystem;" +
                            "UID=root;" +
                            "PASSWORD=;" +
                            "MaximumPoolSize=30;";

        public SQLDatabase()
        {
            new Thread(delegate()
            {
                checkThreadTimeout();
                Thread.Sleep(1000 * 60);
            }).Start();
        }

        Dictionary<string, MySqlConnection> connections = new Dictionary<string, MySqlConnection>();
        Dictionary<string, DateTime> threadLastAccess = new Dictionary<string, DateTime>();

        private MySqlConnection getConnection()
        {
            string threadName = Thread.CurrentThread.ManagedThreadId.ToString();

            if (connections.ContainsKey(threadName) == false)
            {
                MySqlConnection con = new MySqlConnection(myConnectionString);
                con.Open();

                connections.Add(threadName, con);
                threadLastAccess.Add(threadName, DateTime.Now);
            }

            threadLastAccess[threadName] = DateTime.Now;

            return connections[threadName];
        }

        private void checkThreadTimeout()
        {
            List<string> threadsToDelete = new List<string>();
            foreach (KeyValuePair<string, DateTime> pair in threadLastAccess)
            {
                if (DateTime.Now - pair.Value > new TimeSpan(0, 5, 0))
                    threadsToDelete.Add(pair.Key);
            }

            foreach (string threadId in threadsToDelete)
            {
                connections[threadId].Close();

                connections.Remove(threadId);
                threadLastAccess.Remove(threadId);
            }
        }

        public override Tickdata getPrice(long timestamp, string instrument, bool caching = true)
        {
            MySqlCommand command = getConnection().CreateCommand();
            command.CommandText = "SELECT * FROM prices WHERE instrument = '" + instrument + "' AND timestamp < " + timestamp + " ORDER BY timestamp DESC LIMIT 1";
            MySqlDataReader Reader = command.ExecuteReader();
            Reader.Read();
            Reader.Close();
            return new Tickdata((long)Reader["timestamp"], (double)Reader["last"], (double)Reader["bid"], (double)Reader["ask"]);
        }

        public override List<Tickdata> getPrices(long startTimestamp, long endTimestamp, string instrument)
        {
            List<Tickdata> output = new List<Tickdata>();
            MySqlCommand command = getConnection().CreateCommand();
            command.CommandText = "SELECT * FROM prices WHERE instrument = '" + instrument + "' AND timestamp > " + startTimestamp + " AND timestamp < "+ endTimestamp + " ORDER BY timestamp DESC";
            MySqlDataReader Reader = command.ExecuteReader();
            while(Reader.Read())
            {
                output.Add(new Tickdata((long)Reader["timestamp"], (double)Reader["last"], (double)Reader["bid"], (double)Reader["ask"]));
            }
            Reader.Close();

            return output;
        }

        List<string> insertQue = new List<string>();
        public override void setPrice(Tickdata td, string instrument, int buffer = 0)
        {
            insertQue.Add("INSERT INTO prices (instrument, timestamp, bid, ask, last) VALUES ('" + instrument + "', " + format(td.timestamp) + ", " + format(td.bid) + ", " + format(td.ask) + ", " + format(td.last) + ")");
            if (insertQue.Count > buffer)
                sendInsertBuffer();
        }

        public override void sendInsertBuffer()
        {
            if (insertQue.Count != 0)
            {
                StringBuilder builder = new StringBuilder();
                foreach (string s in insertQue) //Changed???
                    builder.Append(s + ";");

                MySqlCommand command = getConnection().CreateCommand();
                command.CommandText = builder.ToString();
                command.ExecuteReader().Close();

                insertQue.Clear();
            }
        }

        public override TimeValueData getData(long timestamp, string dataName, string instrument)
        {
            MySqlCommand command = getConnection().CreateCommand();
            command.CommandText = "SELECT * FROM timevaluepair WHERE instrument = '" + instrument + "' AND name = '" + dataName + "' AND timestamp < " + timestamp + " ORDER BY timestamp DESC LIMIT 1";
            MySqlDataReader Reader = command.ExecuteReader();
            Reader.Read();
            TimeValueData output =  new TimeValueData((long)Reader["timestamp"], (double)Reader["value"]);
            Reader.Close();
            return output;
        }

        public override List<TimeValueData> getDataInRange(long startTimestamp, long endTimestamp, string dataName, string instrument)
        {
            List<TimeValueData> output = new List<TimeValueData>();
            MySqlCommand command = getConnection().CreateCommand();
            command.CommandText = "SELECT * FROM timevaluepair WHERE instrument = '" + instrument + "' AND name = '" + dataName + "' AND timestamp > " + startTimestamp + " AND timestamp < " + endTimestamp + " ORDER BY timestamp DESC";
            MySqlDataReader Reader = command.ExecuteReader();
            while (Reader.Read())
            {
                output.Add(new TimeValueData((long)Reader["timestamp"], (double)Reader["value"]));
            }
            Reader.Close();

            return output;
        }

        public override void setData(TimeValueData data, string dataName, string instrument, int buffer = 0)
        {
            insertQue.Add("INSERT INTO timevaluepair (instrument, name, timestamp, value) VALUES ('" + instrument + "', '" + dataName + "', " + data.timestamp + ", " + format(data.value) + ")");

            if (insertQue.Count > buffer)
                sendInsertBuffer();
        }

        public override long getFirstTimestamp()
        {
            MySqlCommand command = getConnection().CreateCommand();
            command.CommandText = "SELECT * FROM prices ORDER BY timestamp ASC LIMIT 1";
            MySqlDataReader Reader = command.ExecuteReader();
            Reader.Read();
            Reader.Close();
            return (long)Reader["timestamp"];
        }

        public override long getLastTimestamp()
        {
            MySqlCommand command = getConnection().CreateCommand();
            command.CommandText = "SELECT * FROM prices ORDER BY timestamp DESC LIMIT 1";
            MySqlDataReader Reader = command.ExecuteReader();
            Reader.Read();
            long output = (long)Reader["timestamp"];
            Reader.Close();
            return output;
        }

        public override long getSetsCount()
        {
            MySqlCommand command = getConnection().CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM prices";
            MySqlDataReader Reader = command.ExecuteReader();
            Reader.Read();
            Reader.Close();
            return (int)Reader["count"];
        }

        private string format(double d)
        {
            return d.ToString().Replace(',', '.');
        }

        private void closeConnections()
        {
            foreach (KeyValuePair<string, MySqlConnection> pair in connections)
                pair.Value.Close();

            connections.Clear();
        }

        public override void shutdown()
        {
            closeConnections();
        }
    }
}
