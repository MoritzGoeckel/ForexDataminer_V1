using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NinjaTrader.Client;
using System.Threading;
using NinjaTrader_Client.Model;

namespace NinjaTrader_Client.Trader
{
    public class NinjaTraderAPI
    {
        private Thread updateDataThread;
        private Client ntClient;
        private Dictionary<string, Tickdata> instruments;

        private bool resume = true;

        public delegate void TickdataArrivedHandler(Tickdata data, string instrument);
        public event TickdataArrivedHandler tickdataArrived;

        private bool connected = false;
        public bool isConnected()
        {
            return connected;
        }

        public NinjaTraderAPI(List<string> instrumentNames)
        {
            instruments = new Dictionary<string,Tickdata>();

            foreach(string name in instrumentNames)
                instruments.Add(name, new Tickdata(0, 0, 0, 0));


            ntClient = new Client();
            if (ntClient.Connected(0) == 0)
            {
                foreach (KeyValuePair<string, Tickdata> pair in instruments)
                    if (ntClient.SubscribeMarketData(pair.Key) != 0)
                        throw new Exception("Can't subscribe to " + pair.Key);

                updateDataThread = new Thread(updateData);
                updateDataThread.Start();
                connected = true;

                ntClient.ConfirmOrders(0);
            }
            else
            {
                connected = false;
                //throw new Exception("Not connected to NT: " + ntClient.Connected(0));
            }
        }

        private void updateData()
        {
            while (resume)
            {
                List<string> instrumentNames = new List<string>(instruments.Keys);
                foreach (string instrumentName in instrumentNames)
                {
                    Tickdata newestData = new Tickdata(Timestamp.getNow(), ntClient.MarketData(instrumentName, 0), ntClient.MarketData(instrumentName, 1), ntClient.MarketData(instrumentName, 2));
                    // 0 = last, 1 = bid, 2 = ask

                    Tickdata oldData = instruments[instrumentName];
                    if (newestData.ask != oldData.ask || newestData.bid != oldData.bid || newestData.last != oldData.last)
                    {
                        if (tickdataArrived != null)
                            tickdataArrived(newestData, instrumentName);

                        instruments[instrumentName] = newestData;
                    }
                }
                Thread.Sleep(100);
            }
        }

        public void stop()
        {
            this.resume = false;

            if (isConnected())
            {
                foreach (KeyValuePair<string, Tickdata> pair in instruments)
                    if (ntClient.SubscribeMarketData(pair.Key) != 0)
                        throw new Exception("Can't unsubscribe to " + pair.Key);

                if (ntClient.TearDown() != 0)
                    throw new Exception("Can't teardown dll!");
            }
        }

        string account = "Sim101";
        public bool submitOrder(string instrument, NinjaTrader_Client.Trader.Model.TradePosition.PositionType type, int size, double price)
        {
            if (type == Model.TradePosition.PositionType.longPosition)
                return ntClient.Command("PLACE", account, instrument, "BUY", size, "MARKET", price, 0, "DAY", "", "", "", "") == 0;
            else
                return ntClient.Command("PLACE", account, instrument, "SELL", size, "MARKET", price, 0, "DAY", "", "", "", "") == 0;
        }

        public bool closePositions(string instrument)
        {
            return ntClient.Command("CLOSEPOSITION", account, instrument, "", 0, "", 0, 0, "", "", "", "", "") == 0;
        }

        public int getMarketPosition(string instrument)
        {
            return ntClient.MarketPosition(instrument, account);
        }

        public List<string> getOrders()
        {
            List<string> orders = new List<string>();
            orders.AddRange(ntClient.Orders(account).Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
            return orders;
        }

        public string orderStatus(string id)
        {
            return ntClient.OrderStatus(id);
        }
    }
}
