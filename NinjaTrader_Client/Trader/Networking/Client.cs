using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Networking
{
    class Client
    {
        Thread clientThread;
        public Client(int port)
        {
            clientThread = new Thread(listen);
            clientThread.Start();
        }

        private void listen()
        {

        }

        private void onMessageRecieved(string data)
        {

        }

        public void sendMessage(string data)
        {

        }

        public void stop()
        {

        }
    }
}
