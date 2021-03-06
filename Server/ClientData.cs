﻿using System;
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


        public PinnwandDBAdapter db_Manager;

        //person data
        public string id;
        public string email;
        public string vname = "";
        public string name = "";
        public string autor = "";
        public bool hasRights = false;

        public ClientData(Socket clientSocket)
        {
            id = Guid.NewGuid().ToString();
            email = null;

            db_Manager = new PinnwandDBAdapter();

            this.clientSocket = clientSocket;

            clientThread = new Thread(ClientHandler.DataIN);
            clientThread.Start(clientSocket);

            SendRegistrationPacket();
        }

        public void SendRegistrationPacket()
        {
            ListDictionary list = new ListDictionary();
            list.Add("id", id);

            Packet p = new Packet(PacketType.RegisterId, list, "server");
            ClientHandler.SendSinglePacket(this, p);

            //ClientHandler.Ausgabe("Debug", id);
        }


        public void SetAutorKürzel()
        {
            autor = "";
            autor += vname.Length < 2 ? vname.Substring(0, vname.Length) : vname.Substring(0, 2);
            autor += "." + name;
        }
    }
}
