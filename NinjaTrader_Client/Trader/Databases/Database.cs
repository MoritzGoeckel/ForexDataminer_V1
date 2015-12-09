using NinjaTrader_Client.Trader.Model;
using System.Collections.Generic;

namespace NinjaTrader_Client.Trader.MainAPIs
{
    public abstract class Database
    {
        public abstract Tickdata getPrice(long timestamp, string instrument, bool caching = true);
        public abstract List<Tickdata> getPrices(long startTimestamp, long endTimestamp, string instrument);
        public abstract void setPrice(Tickdata td, string instrument);
        public abstract void setData(TimeValueData data, string dataName, string instrument);
        public abstract TimeValueData getData(long timestamp, string dataName, string instrument);
        public abstract List<TimeValueData> getDataInRange(long startTimestamp, long endTimestamp, string dataName, string instrument);
        public abstract long getSetsCount();
        public abstract long getLastTimestamp();
        public abstract long getFirstTimestamp();

        public abstract void shutdown();

        //public abstract int getCacheingAccessPercent();
        //public abstract int getCacheFilledPercent();
        //private abstract Tickdata getPriceInternal(long timestamp, string instrument);
        //public abstract void exportData(long startTimestamp, string basePath);
        //private abstract BsonDocument getExportData(long startTime, MongoCollection<BsonDocument> collection);
        //public abstract void importData(string data);
        //public abstract void megrate();
    }
}
