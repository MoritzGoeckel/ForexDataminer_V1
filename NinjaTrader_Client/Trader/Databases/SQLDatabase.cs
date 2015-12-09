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
            MySqlCommand command = new MySqlCommand("SELECT * FROM prices WHERE instrument = @instrument AND timestamp < @timestamp ORDER BY timestamp DESC LIMIT 1", getConnection());
            command.Parameters.AddWithValue("@timestamp", timestamp);
            command.Parameters.AddWithValue("@instrument", instrument);
            command.Prepare();

            MySqlDataReader Reader = command.ExecuteReader();
            Reader.Read();
            Reader.Close();
            return new Tickdata((long)Reader["timestamp"], (double)Reader["last"], (double)Reader["bid"], (double)Reader["ask"]);
        }

        public override List<Tickdata> getPrices(long startTimestamp, long endTimestamp, string instrument)
        {
            List<Tickdata> output = new List<Tickdata>();

            MySqlCommand command = new MySqlCommand("SELECT * FROM prices WHERE instrument = @instrument AND timestamp > @startTimestamp AND timestamp < @endTimestamp ORDER BY timestamp DESC", getConnection());
            command.Parameters.AddWithValue("@endTimestamp", endTimestamp);
            command.Parameters.AddWithValue("@startTimestamp", startTimestamp);
            command.Parameters.AddWithValue("@instrument", instrument);
            command.Prepare();

            MySqlDataReader Reader = command.ExecuteReader();
            while(Reader.Read())
            {
                output.Add(new Tickdata((long)Reader["timestamp"], (double)Reader["last"], (double)Reader["bid"], (double)Reader["ask"]));
            }
            Reader.Close();

            return output;
        }

        public override void setPrice(Tickdata td, string instrument)
        {
            MySqlCommand command = new MySqlCommand("INSERT INTO prices (instrument, timestamp, bid, ask, last) VALUES (@instrument, @timestamp, @bid, @ask, @last)", getConnection());
            command.Parameters.AddWithValue("@last", format(td.last));
            command.Parameters.AddWithValue("@ask", format(td.ask));
            command.Parameters.AddWithValue("@bid", format(td.bid));
            command.Parameters.AddWithValue("@timestamp", format(td.timestamp));
            command.Parameters.AddWithValue("@instrument", instrument);
            command.Prepare();

            command.ExecuteReader().Close();
        }

        public override TimeValueData getData(long timestamp, string dataName, string instrument)
        {
            MySqlCommand command = new MySqlCommand("SELECT * FROM timevaluepair WHERE instrument = @instrument AND name = @name AND timestamp < @timestamp ORDER BY timestamp DESC LIMIT 1", getConnection());
            command.Parameters.AddWithValue("@timestamp", timestamp);
            command.Parameters.AddWithValue("@name", dataName);
            command.Parameters.AddWithValue("@instrument", instrument);
            command.Prepare();

            MySqlDataReader Reader = command.ExecuteReader();
            Reader.Read();
            TimeValueData output =  new TimeValueData((long)Reader["timestamp"], (double)Reader["value"]);
            Reader.Close();
            return output;
        }

        public override List<TimeValueData> getDataInRange(long startTimestamp, long endTimestamp, string dataName, string instrument)
        {
            List<TimeValueData> output = new List<TimeValueData>();

            MySqlCommand command = new MySqlCommand("SELECT * FROM timevaluepair WHERE instrument = @instrument AND name = @name AND timestamp > @startTimestamp AND timestamp < @endTimestamp ORDER BY timestamp DESC", getConnection());
            command.Parameters.AddWithValue("@startTimestamp", startTimestamp);
            command.Parameters.AddWithValue("@endTimestamp", endTimestamp);
            command.Parameters.AddWithValue("@name", dataName);
            command.Parameters.AddWithValue("@instrument", instrument);
            command.Prepare();

            MySqlDataReader Reader = command.ExecuteReader();
            while (Reader.Read())
            {
                output.Add(new TimeValueData((long)Reader["timestamp"], (double)Reader["value"]));
            }
            Reader.Close();

            return output;
        }

        public override void setData(TimeValueData data, string dataName, string instrument)
        {
            MySqlCommand command = new MySqlCommand("INSERT INTO timevaluepair(instrument, name, timestamp, value) VALUES(@instrument, @dataName, @timestamp, @value)", getConnection());
            command.Parameters.AddWithValue("@value", format(data.value));
            command.Parameters.AddWithValue("@timestamp", data.timestamp);
            command.Parameters.AddWithValue("@dataName", dataName);
            command.Parameters.AddWithValue("@instrument", instrument);
            command.Prepare();

            command.ExecuteReader().Close();
        }

        public override long getFirstTimestamp()
        {
            MySqlCommand command = new MySqlCommand("SELECT * FROM prices ORDER BY timestamp ASC LIMIT 1", getConnection());
            command.Prepare();

            MySqlDataReader Reader = command.ExecuteReader();
            Reader.Read();
            Reader.Close();
            return (long)Reader["timestamp"];
        }

        public override long getLastTimestamp()
        {
            MySqlCommand command = new MySqlCommand("SELECT * FROM prices ORDER BY timestamp DESC LIMIT 1", getConnection());
            command.Prepare();

            MySqlDataReader Reader = command.ExecuteReader();
            Reader.Read();
            long output = (long)Reader["timestamp"];
            Reader.Close();
            return output;
        }

        public override long getSetsCount()
        {
            MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM prices", getConnection());
            command.Prepare();

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
