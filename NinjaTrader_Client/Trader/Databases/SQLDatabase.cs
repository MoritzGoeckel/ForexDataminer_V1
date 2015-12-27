using MySql.Data.MySqlClient;
using NinjaTrader_Client.Trader.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;

namespace NinjaTrader_Client.Trader.MainAPIs
{
    public class SQLDatabase : Database
    {
        string myConnectionString = "SERVER=localhost;" +
                            "DATABASE=tradingsystem;" +
                            "UID=root;" +
                            "PASSWORD=;" +
                            "MinimumPoolSize=20;" +
                            "MaximumPoolSize=100;";

        public SQLDatabase()
        {
            MySqlConnection connection = getConnection();
            MySqlCommand cmd = new MySqlCommand("set net_write_timeout=2000; set net_read_timeout=2000", connection);
            cmd.ExecuteNonQuery();

            connection.Close();
        }

        Dictionary<string, long> firstTimestamps = new Dictionary<string, long>();

        private int timeout = 2000;

        private MySqlConnection getConnection()
        {
            MySqlConnection con = new MySqlConnection(myConnectionString);
            con.Open();

            return con;
        }

        private void minorErrorOccurred()
        {
            Thread.Sleep(500);
        }

        public override Tickdata getPrice(long timestamp, string instrument, bool caching = true)
        {
            MySqlConnection connection = null;
            Tickdata output = null;

            if (firstTimestamps.ContainsKey(instrument) == false)
                firstTimestamps.Add(instrument, getFirstTimestampInternal(instrument));

            if (firstTimestamps[instrument] > timestamp)
                return null;

            bool done = false;
            while (done == false)
            {
                try
                {
                    connection = getConnection();

                    MySqlCommand command = new MySqlCommand("SELECT * FROM prices WHERE instrument = @instrument AND timestamp < @timestamp AND timestamp > @timestampMin ORDER BY timestamp DESC LIMIT 1", connection);
                    command.Parameters.AddWithValue("@timestamp", timestamp);
                    command.Parameters.AddWithValue("@instrument", instrument);
                    command.Parameters.AddWithValue("@timestampMin", timestamp - (3 * 60 * 1000));
                    command.Prepare();

                    command.CommandTimeout = timeout;

                    MySqlDataReader Reader = command.ExecuteReader();

                    if (Reader.Read())
                        output = new Tickdata((long)Reader["timestamp"], (double)Reader["last"], (double)Reader["bid"], (double)Reader["ask"]);
                    else
                        output = null;

                    Reader.Close();
                    done = true;
                }
                catch(Exception) { minorErrorOccurred(); }
                finally
                {
                    try { connection.Close(); } catch (Exception e) { }
                }
            }

            return output;
        }

        public override List<Tickdata> getPrices(long startTimestamp, long endTimestamp, string instrument)
        {
            List<Tickdata> output = new List<Tickdata>();
            MySqlConnection connection = null;

            try {
                if (firstTimestamps.ContainsKey(instrument) == false)
                    firstTimestamps.Add(instrument, getFirstTimestampInternal(instrument));
            }
            catch (Exception) { }

            if (firstTimestamps[instrument] > startTimestamp)
                return null;

            bool done = false;
            while (done == false)
            {
                try
                {
                    connection = getConnection();

                    MySqlCommand command = new MySqlCommand("SELECT * FROM prices WHERE instrument = @instrument AND timestamp > @startTimestamp AND timestamp < @endTimestamp ORDER BY timestamp DESC", connection);
                    command.Parameters.AddWithValue("@endTimestamp", endTimestamp);
                    command.Parameters.AddWithValue("@startTimestamp", startTimestamp);
                    command.Parameters.AddWithValue("@instrument", instrument);
                    command.Prepare();

                    command.CommandTimeout = timeout;

                    MySqlDataReader Reader = command.ExecuteReader();
                    while (Reader.Read())
                    {
                        output.Add(new Tickdata((long)Reader["timestamp"], (double)Reader["last"], (double)Reader["bid"], (double)Reader["ask"]));
                    }
                    Reader.Close();
                    done = true;
                }
                catch (Exception) { minorErrorOccurred();  }
                finally
                {
                    try { connection.Close(); } catch (Exception) { }
                }
            }

            return output;
        }

        public override void setPrice(Tickdata td, string instrument)
        {
            MySqlConnection connection = getConnection();

            MySqlCommand command = new MySqlCommand("INSERT INTO prices (instrument, timestamp, bid, ask, last) VALUES (@instrument, @timestamp, @bid, @ask, @last)", connection);
            command.Parameters.AddWithValue("@last", format(td.last));
            command.Parameters.AddWithValue("@ask", format(td.ask));
            command.Parameters.AddWithValue("@bid", format(td.bid));
            command.Parameters.AddWithValue("@timestamp", format(td.timestamp));
            command.Parameters.AddWithValue("@instrument", instrument);
            command.Prepare();

            command.CommandTimeout = timeout;

            command.ExecuteNonQuery();
            connection.Close();
        }

        public override TimeValueData getData(long timestamp, string dataName, string instrument)
        {
            MySqlConnection connection = getConnection();

            MySqlCommand command = new MySqlCommand("SELECT * FROM timevaluepair WHERE instrument = @instrument AND name = @name AND timestamp < @timestamp AND timestamp > @timestampMin ORDER BY timestamp DESC LIMIT 1", connection);
            command.Parameters.AddWithValue("@timestamp", timestamp);
            command.Parameters.AddWithValue("@name", dataName);
            command.Parameters.AddWithValue("@instrument", instrument);
            command.Parameters.AddWithValue("@timestampMin", timestamp - (3 * 60 * 1000));
            command.Prepare();

            command.CommandTimeout = timeout;

            TimeValueData output = null;

            MySqlDataReader Reader = command.ExecuteReader();

            if(Reader.Read())
                output = new TimeValueData((long)Reader["timestamp"], (double)Reader["value"]);

            Reader.Close();
            connection.Close();

            return output;
        }

        public override List<TimeValueData> getDataInRange(long startTimestamp, long endTimestamp, string dataName, string instrument)
        {
            MySqlConnection connection = getConnection();

            List<TimeValueData> output = new List<TimeValueData>();

            MySqlCommand command = new MySqlCommand("SELECT * FROM timevaluepair WHERE instrument = @instrument AND name = @name AND timestamp > @startTimestamp AND timestamp < @endTimestamp ORDER BY timestamp DESC", connection);
            command.Parameters.AddWithValue("@startTimestamp", startTimestamp);
            command.Parameters.AddWithValue("@endTimestamp", endTimestamp);
            command.Parameters.AddWithValue("@name", dataName);
            command.Parameters.AddWithValue("@instrument", instrument);
            command.Prepare();

            command.CommandTimeout = timeout;

            MySqlDataReader Reader = command.ExecuteReader();
            while (Reader.Read())
            {
                output.Add(new TimeValueData((long)Reader["timestamp"], (double)Reader["value"]));
            }
            Reader.Close();
            connection.Close();

            return output;
        }

        public override void setData(TimeValueData data, string dataName, string instrument)
        {
            MySqlConnection connection = getConnection();

            MySqlCommand command = new MySqlCommand("INSERT INTO timevaluepair(instrument, name, timestamp, value) VALUES(@instrument, @dataName, @timestamp, @value)", connection);
            command.Parameters.AddWithValue("@value", format(data.value));
            command.Parameters.AddWithValue("@timestamp", data.timestamp);
            command.Parameters.AddWithValue("@dataName", dataName);
            command.Parameters.AddWithValue("@instrument", instrument);
            command.Prepare();

            command.CommandTimeout = timeout;

            command.ExecuteNonQuery();

            connection.Close();
        }

        public override long getFirstTimestamp()
        {
            return getFirstTimestampInternal();
        }

        private long getFirstTimestampInternal(string pair = null)
        {
            MySqlConnection connection = getConnection();

            string where = "";
            if (pair != null)
                where = "WHERE instrument = '"+ pair + "'";

            MySqlCommand command = new MySqlCommand("SELECT MIN(timestamp) FROM prices " + where, connection);
            command.Prepare();

            command.CommandTimeout = timeout;

            MySqlDataReader Reader = command.ExecuteReader();
            Reader.Read();
            long ts = (long)Reader["MIN(timestamp)"];
            Reader.Close();

            connection.Close();

            return ts;
        }

        public override long getLastTimestamp()
        {
            MySqlConnection connection = getConnection();

            MySqlCommand command = new MySqlCommand("SELECT MAX(timestamp) FROM prices", connection);
            command.Prepare();

            command.CommandTimeout = timeout;

            MySqlDataReader Reader = command.ExecuteReader();
            Reader.Read();
            long output = (long)Reader["MAX(timestamp)"];
            Reader.Close();

            connection.Close();

            return output;
        }

        public override long getSetsCount()
        {
            MySqlConnection connection = getConnection();

            MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM prices", connection);
            command.Prepare();

            command.CommandTimeout = timeout;

            MySqlDataReader Reader = command.ExecuteReader();
            Reader.Read();

            int count = (int)Reader["count"];

            Reader.Close();
            connection.Close();

            return count;
        }

        private string format(double d)
        {
            return d.ToString().Replace(',', '.');
        }

        public override void shutdown()
        {
            
        }
    }
}
