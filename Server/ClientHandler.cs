using ServerData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    static class ClientHandler
    {
        //Server+++++++
        static Socket listenerSocket;
        static List<ClientData> lst_clients;
        static List<string> lst_loggedIn;

        static bool run_flag = true;
        //-------------

        public static void StartServer(string ip, int port)
        {
            IPAddress IP = IPAddress.Parse(ip);
            Console.WriteLine("--Server--\nStart Server on IPv4: " + ip);

            listenerSocket = TCP_connection.SocketSetup;    //Setup Socket
            //Liste für Clients
            lst_clients = new List<ClientData>();
            lst_loggedIn = new List<string>();

            //Verbindungsknoten des Servers
            IPEndPoint ipEnd = new IPEndPoint(IP, port);
            listenerSocket.Bind(ipEnd);

            //Server starten
            Thread listenerThread = new Thread(ListenThread);
            listenerThread.Start(); //wartet auf Verbindungen

            Thread consolThread = new Thread(ServerConsole);
            consolThread.Start(); //console
        }

        //Listener wait for Clients
        static void ListenThread()
        {
            while (run_flag) //Endlosschleife 
            {
                if (lst_clients.Count < 50) //max 50 clients
                {
                    listenerSocket.Listen(0);
                    Socket newClientSocket = listenerSocket.Accept();

                    lst_clients.Add(new ClientData(newClientSocket)); //neuen Client zur Liste hinzufügen
                    Ausgabe("Socketlistener Accepted\nEs sind " + lst_clients.Count() + " Client(s) online");
                }
                else
                {
                    Ausgabe("Client Maximum erreicht!!!");
                }
            }
        }

        //packete empfangen+++++++++++++++++++++++++++++++++++++++++++++++++++
        public static void DataIN(object cSocket)
        {
            TCP_connection.Data_IN(cSocket, new TCP_connection.DataManagerCallback(Server.DataManager), new TCP_connection.ExceptionCallback(SocketDisconnectedException));
        }
        private static void SocketDisconnectedException(Socket socket, Exception exc)
        {
            Ausgabe("Socket", exc.Message);
            RemoveClient(GetIDfromSocket(socket));
        }
        //packet senden++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public static void SendSinglePacket(ClientData client, Packet p)
        {
            TCP_connection.SendPacket(client.clientSocket, p, new TCP_connection.ExceptionCallback(SocketDisconnectedException));
        }
        public static void SendPacketToAllLoggedInClients(Packet p)
        {
            foreach (ClientData client in lst_clients)
            {
                if (checkLoginState(client.id))
                {
                    SendSinglePacket(client, p);
                }
            }
        }

        #region getSocketby...
        static int GetClientIndexbyID(string ID)
        {
            int i = 0;
            foreach (ClientData client in lst_clients)
            {
                if (client.id == ID)
                {
                    return i;
                }
                i++;
            }
            Ausgabe("Client nicht gefunden (by ID)");
            return -1;
        }
        public static ClientData GetClientByID(string id)
        {
            ClientData client = lst_clients[GetClientIndexbyID(id)];
            return client;
        }
        private static string GetIDfromSocket(Socket socket)
        {
            foreach (ClientData client in lst_clients)
            {
                if (client.clientSocket == socket)
                {
                    return client.id;
                }
            }
            Ausgabe("Client nicht gefunden (from Socket)");
            return "";
        }
        #endregion

        //Client aus Liste löschen
        private static void RemoveClient(string ID)
        {
            foreach (ClientData client in lst_clients)
            {
                if (client.id == ID)
                {
                    try
                    {
                        client.clientSocket.Shutdown(SocketShutdown.Both);
                        client.clientSocket.Disconnect(true);
                        client.clientSocket.Close();
                    }
                    catch
                    {
                        Ausgabe("Socket konnte nicht geschlossen werden");
                    }
                    lst_loggedIn.Remove(client.id);
                    lst_clients.Remove(client);
                    Ausgabe("Client wurde entfernt");
                    return;
                }
            }
        }

        //Error zum Client weiterleiten
        public static void Send_Error_Message_to_Client(ClientData c, string msg)
        {
            Packet p = new Packet(msg);
            SendSinglePacket(c, p);
        }
        //Normale Server ausgabe
        private static void Ausgabe(string text)
        {
            Console.WriteLine("Server::>" + text);
            //Console.Write("Server::> ");
        }
        //Ausgabe mit Verweis auf Herkunft
        public static void Ausgabe(string parent, string text)
        {
            Console.WriteLine( parent + "::> " + text);
            //Console.Write("Server::> ");
        }

        //Server Console
        static void ServerConsole()
        {
            while (run_flag)
            {
                Console.Write("Server::> ");
                string input = Console.ReadLine();
                Execute(input);
            }
        }

        //Server Befehle
        public static void Execute(string input)
        {
            string[] command = input.Split(new Char[] { ' ' });

            switch (command[0])
            {
                case "/list":
                    int i = 0;
                    foreach (ClientData client in lst_clients)
                    {
                        Console.WriteLine("Index: " + i + " ID: " +  client.id + " Email: " + client.email);
                        i++;
                    }
                    break;
                case "/kick":
                    RemoveClient(command[1]);
                    break;
                default:
                    Console.WriteLine("Ungueltiger Befehl!");
                    break;
            }
        }

        public static void ClientLogin(string id)
        {
            lst_loggedIn.Add(id);
        }
        public static bool checkLoginState(string id)
        {
            if (lst_loggedIn.Contains(id))
            {
                return true;
            }
            return false;
        }

    }
}
