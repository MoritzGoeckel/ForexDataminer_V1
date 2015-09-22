using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Networking
{
    class Server
    {
        Thread serverThread;
        public Server(int port)
        {
            serverThread = new Thread(listen);
            serverThread.Start();
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
