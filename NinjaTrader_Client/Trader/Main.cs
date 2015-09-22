using NinjaTrader_Client.Model;
using NinjaTrader_Client.Trader.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader
{
    public class Main
    {
        MongoFacade mongodb;
        Database priceDatabase;
        NinjaTraderAPI api;
        SSI_Downloader ssi;

        public delegate void UIDataChangedHandler(UIData uiData);
        public event UIDataChangedHandler uiDataChanged;

        public Main()
        {
            List<string> instruments = new List<string>();
            instruments.Add("EURUSD");
            instruments.Add("GBPUSD");
            instruments.Add("USDJPY");
            instruments.Add("USDCHF");
            instruments.Add("AUDUSD");
            instruments.Add("NZDUSD");
            instruments.Add("USDCAD");
            instruments.Add("EURGBP");
            instruments.Add("EURJPY");
            instruments.Add("AUDCAD");
            instruments.Add("CHFJPY");
            instruments.Add("EURCHF");
            instruments.Add("AUDJPY");
            instruments.Add("GBPCHF");
            instruments.Add("GBPJPY");


            mongodb = new MongoFacade(@"E:\Programmieren\C# Neu\NinjaTrader_Client\Output\Database\mongo\mongod.exe", @"E:\Programmieren\C# Neu\NinjaTrader_Client\Output\Database\data", "nt_trader");
            priceDatabase = new Database(mongodb);
            api = new NinjaTraderAPI(instruments);
            ssi = new SSI_Downloader(instruments);
        }

        public Database getDatabase()
        {
            return priceDatabase;
        }

        public void startDownloadingUpdates()
        {
            ssi.sourceDataArrived += ssi_sourceDataArrived;
            ssi.start();
            api.tickdataArrived += api_tickdataArrived;
        }

        void ssi_sourceDataArrived(double value, long timestamp, string sourceName, string instrument)
        {
            priceDatabase.setIndicator(new IndicatorData(timestamp, value), sourceName, instrument);
        }

        int insertedSets = 0;
        private void api_tickdataArrived(Tickdata data, string instrument)
        {
            priceDatabase.setPrice(data, instrument);

            UIData uiData = new UIData();
            uiData.dbErrors = priceDatabase.errors;
            uiData.dataSets = insertedSets++;

            if (uiDataChanged != null)
                uiDataChanged(uiData);
        }

        public void stop()
        {
            ssi.stop();
            api.stop();
            mongodb.shutdown();
        }
    }
}
