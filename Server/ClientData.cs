using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerData;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using PiaLib;
using System.Collections.Specialized;

namespace Server
{
    class ClientData
    {
        public Socket clientSocket;
        public Thread clientThread;
        public string id;
        public string email;

        public ClientData(Socket clientSocket)
        {
            id = Guid.NewGuid().ToString();
            email = null;

            this.clientSocket = clientSocket;

            clientThread = new Thread((ClientHandler.DataIN));
            clientThread.Start(clientSocket);

            SendRegistrationPacket();
        }

        public void SendRegistrationPacket()
        {
            ListDictionary list = new ListDictionary();
            list.Add("id", id);

            Packet p = new Packet(AuthenticationState_SERVER_Events.SERVER_Register_ID, list);
            ClientHandler.SendSinglePacket(this, p);
        }
    }
}
