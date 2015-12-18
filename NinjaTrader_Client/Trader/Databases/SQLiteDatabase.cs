using NinjaTrader_Client.Trader.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Threading;

namespace NinjaTrader_Client.Trader.MainAPIs
{
    public class SQLiteDatabase : Database
    {
        string myConnectionString;
        string path;

        public SQLiteDatabase(string path)
        {
            this.path = path;
            myConnectionString = "Data Source=" + path + ";Version=3;"; //In memory ????
        }

        private int timeout = 10;

        private SQLiteConnection getConnection()
        {
            SQLiteConnection con = new SQLiteConnection(myConnectionString);
            con.Open();

            return con;
        }

        private void minorErrorOccurred()
        {
            Thread.Sleep(500);
        }

        public override Tickdata getPrice(long timestamp, string instrument, bool caching = true)
        {
            SQLiteConnection connection = null;
            Tickdata output = null;

            bool done = false;
            while (done == false)
            {
                try
                {
                    connection = getConnection();

                    SQLiteCommand command = new SQLiteCommand("SELECT * FROM prices WHERE instrument = @instrument AND timestamp < @timestamp ORDER BY timestamp DESC LIMIT 1", connection);
                    command.Parameters.AddWithValue("@timestamp", timestamp);
                    command.Parameters.AddWithValue("@instrument", instrument);
                    command.Prepare();

                    command.CommandTimeout = timeout;

                    SQLiteDataReader Reader = command.ExecuteReader();

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
            SQLiteConnection connection = null;

            bool done = false;
            while (done == false)
            {
                try
                {
                    connection = getConnection();

                    SQLiteCommand command = new SQLiteCommand("SELECT * FROM prices WHERE instrument = @instrument AND timestamp > @startTimestamp AND timestamp < @endTimestamp ORDER BY timestamp DESC", connection);
                    command.Parameters.AddWithValue("@endTimestamp", endTimestamp);
                    command.Parameters.AddWithValue("@startTimestamp", startTimestamp);
                    command.Parameters.AddWithValue("@instrument", instrument);
                    command.Prepare();

                    command.CommandTimeout = timeout;

                    SQLiteDataReader Reader = command.ExecuteReader();
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
            SQLiteConnection connection = getConnection();

            SQLiteCommand command = new SQLiteCommand("INSERT INTO prices (instrument, timestamp, bid, ask, last) VALUES (@instrument, @timestamp, @bid, @ask, @last)", connection);
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
            SQLiteConnection connection = getConnection();

            SQLiteCommand command = new SQLiteCommand("SELECT * FROM timevaluepair WHERE instrument = @instrument AND name = @name AND timestamp < @timestamp ORDER BY timestamp DESC LIMIT 1", connection);
            command.Parameters.AddWithValue("@timestamp", timestamp);
            command.Parameters.AddWithValue("@name", dataName);
            command.Parameters.AddWithValue("@instrument", instrument);
            command.Prepare();

            command.CommandTimeout = timeout;

            SQLiteDataReader Reader = command.ExecuteReader();
            Reader.Read();
            TimeValueData output =  new TimeValueData((long)Reader["timestamp"], (double)Reader["value"]);
            Reader.Close();
            connection.Close();

            return output;
        }

        public override List<TimeValueData> getDataInRange(long startTimestamp, long endTimestamp, string dataName, string instrument)
        {
            SQLiteConnection connection = getConnection();

            List<TimeValueData> output = new List<TimeValueData>();

            SQLiteCommand command = new SQLiteCommand("SELECT * FROM timevaluepair WHERE instrument = @instrument AND name = @name AND timestamp > @startTimestamp AND timestamp < @endTimestamp ORDER BY timestamp DESC", connection);
            command.Parameters.AddWithValue("@startTimestamp", startTimestamp);
            command.Parameters.AddWithValue("@endTimestamp", endTimestamp);
            command.Parameters.AddWithValue("@name", dataName);
            command.Parameters.AddWithValue("@instrument", instrument);
            command.Prepare();

            command.CommandTimeout = timeout;

            SQLiteDataReader Reader = command.ExecuteReader();
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
            SQLiteConnection connection = getConnection();

            SQLiteCommand command = new SQLiteCommand("INSERT INTO timevaluepair(instrument, name, timestamp, value) VALUES(@instrument, @dataName, @timestamp, @value)", connection);
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
            SQLiteConnection connection = getConnection();

            SQLiteCommand command = new SQLiteCommand("SELECT * FROM prices ORDER BY timestamp ASC LIMIT 1", connection);
            command.Prepare();

            command.CommandTimeout = timeout;

            SQLiteDataReader Reader = command.ExecuteReader();
            Reader.Read();
            Reader.Close();

            long ts = (long)Reader["timestamp"];

            connection.Close();

            return ts;
        }

        public override long getLastTimestamp()
        {
            SQLiteConnection connection = getConnection();

            SQLiteCommand command = new SQLiteCommand("SELECT * FROM prices ORDER BY timestamp DESC LIMIT 1", connection);
            command.Prepare();

            command.CommandTimeout = timeout;

            SQLiteDataReader Reader = command.ExecuteReader();
            Reader.Read();
            long output = (long)Reader["timestamp"];
            Reader.Close();

            connection.Close();

            return output;
        }

        public override long getSetsCount()
        {
            SQLiteConnection connection = getConnection();

            SQLiteCommand command = new SQLiteCommand("SELECT COUNT(*) FROM prices", connection);
            command.Prepare();

            command.CommandTimeout = timeout;

            SQLiteDataReader Reader = command.ExecuteReader();
            Reader.Read();
            Reader.Close();

            int count = (int)Reader["count"];

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

        //Migrate from MySQL ???
    }
}
