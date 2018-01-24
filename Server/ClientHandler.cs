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
        static bool run_flag = true;
        //-------------

        public static void StartServer(string ip, int port)
        {
            IPAddress IP = IPAddress.Parse(ip);
            Console.WriteLine("--Server--\nStart Server on IPv4: " + ip);

            listenerSocket = TCP_connection.SocketSetup;    //Setup Socket
            //Liste für Clients
            lst_clients = new List<ClientData>();

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
                    Ausgabe("Socketlistener Accepted\nEs sind " + lst_clients.Count() + " Clients online");
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

        #region Data_IN
        ////Clientdata thread receives data from Clients
        //public static void Data_IN(object cSocket)  //Client Socket
        //{
        //    Socket clientSocket = (Socket)cSocket;  

        //    //Packete werden aufgeteilt und nacheinander gesendet/empfangen
        //    SocketError error = SocketError.Success;

        //    try
        //    {
        //        while (clientSocket.Connected)
        //        {
        //            byte[] sizeBuf = new byte[4];
        //            //clientSocket.Receive(sizeBuf, 0, sizeBuf.Length, 0);

        //            int byteLenngth = clientSocket.Receive(sizeBuf, 0, sizeBuf.Length, SocketFlags.None, out error);

        //            if (error == SocketError.Success && byteLenngth > 0)
        //            {
        //                uint size = BitConverter.ToUInt32(sizeBuf, 0);  //bytes to Int

        //                Packet p = TCP_connection.Read_Data_Stream(clientSocket, size); //Read_Data_Stream(clientSocket, size);

        //                if (p != null)
        //                {
        //                    Server.DataManager(p);  //an Hauptklasse weiterleiten
        //                }
        //            }
        //            else
        //            {
        //                throw new Exception("Clientverbindung getrennt! (Socket disconnected)");
        //            }
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        Ausgabe(exp.Message);
        //        //client löschen
        //        RemoveClient(GetIDfromSocket(clientSocket));
        //        return;
        //    }
        //}
        #endregion


        //public static void SendSingle(ClientData client, Packet p)
        //{
        //    try
        //    {
        //        byte[] data = PacketHandler.SerializePacket(p);

        //        client.clientSocket.SendBufferSize = data.Length;
        //        client.clientSocket.Send(BitConverter.GetBytes(data.Length), 0, 4, 0);  //byte Anzahl senden
        //        client.clientSocket.Send(data); //byte array senden
        //    }
        //    catch
        //    {
        //        client.clientSocket.Shutdown(SocketShutdown.Both);
        //        client.clientSocket.Close();
        //        RemoveClient(client.id);
        //        Ausgabe("Fehler beim Senden an Client Socket!");
        //    }
        //}

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
            Ausgabe("Client nicht gefunden");
            return (-1);
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
            Ausgabe("Client nicht gefunden");
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
            Console.Write("Server::> ");
        }
        //Ausgabe mit Verweis auf Herkunft
        public static void Ausgabe(string parent, string text)
        {
            Console.WriteLine("\n" + parent + "::> " + text);
            Console.Write("Server::> ");
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
    }
}
