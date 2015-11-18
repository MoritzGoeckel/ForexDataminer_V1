﻿using NinjaTrader_Client.Trader.Model;
using NinjaTrader_Client.Trader.Strategies;
using NinjaTrader_Client.Trader.TradingAPIs;
using System.Collections.Generic;
using System.Threading;

namespace NinjaTrader_Client.Trader
{
    public class Main
    {
        private MongoFacade mongodb;
        private Database database;
        private NinjaTraderAPI api;
        private SSI_Downloader ssi;
        private Config config;

        public delegate void UIDataChangedHandler(UIData uiData);
        public event UIDataChangedHandler uiDataChanged;

        public Main(string startupPath)
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

            config = new Config(startupPath);
            mongodb = new MongoFacade(config.mongodbExePath, config.mongodbDataPath, "nt_trader");
            
            database = new Database(mongodb);
            api = new NinjaTraderAPI(instruments);
            ssi = new SSI_Downloader(instruments);
        }

        public Database getDatabase()
        {
            return database;
        }

        public NinjaTraderAPI getAPI()
        {
            return api;
        }

        private bool isDownloadingUpdates = false;
        public void startDownloadingUpdates()
        {
            if (isDownloadingUpdates == false)
            {
                ssi.sourceDataArrived += ssi_sourceDataArrived;
                ssi.start();
                api.tickdataArrived += api_tickdataArrived;
                isDownloadingUpdates = true;
            }
        }

        private bool continueLiveTradingThread = false;
        private int tradingTick = 0;

        public void startTradingLive(Strategy strat, string pair, int interval = 500)
        {
            List<string> pairs = new List<string>();
            pairs.Add(pair);

            startTradingLive(strat, pairs, interval);
        }

        public void startTradingLive(Strategy strat, List<string> tradablePairs, int interval = 500)
        {
            strat.setAPI(NTLiveTradingAPI.getTheInstace());

            if (isDownloadingUpdates == false)
                startDownloadingUpdates();

            continueLiveTradingThread = true;
            Thread autoTradingTimerThread = new Thread(delegate()
            {
                while (continueLiveTradingThread)
                {
                    tradingTick++;
                    if (tradingTick > 1000)
                        tradingTick = 0;

                    foreach (string instrument in tradablePairs)
                        strat.doTick(instrument);

                    Thread.Sleep(interval);
                }
            });
            autoTradingTimerThread.Start();
        }

        private void ssi_sourceDataArrived(double value, long timestamp, string sourceName, string instrument)
        {
            database.setData(new TimeValueData(timestamp, value), sourceName, instrument);
        }

        int insertedSets = 0;
        private void api_tickdataArrived(Tickdata data, string instrument)
        {
            database.setPrice(data, instrument);

            UIData uiData = new UIData();
            uiData.dbErrors = database.errors;
            uiData.dataSets = insertedSets++;
            uiData.tradingTick = tradingTick;

            if (uiDataChanged != null)
                uiDataChanged(uiData);
        }

        public void stop()
        {
            if (continueLiveTradingThread)
                continueLiveTradingThread = false;

            ssi.stop();
            api.stop();
            mongodb.shutdown();
        }
    }
}
