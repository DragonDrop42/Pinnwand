using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerData
{
    public static class TCP_connection
    {
        public delegate void DataManagerCallback(Packet p);
        public delegate void ErrorMessageCallback(string s);
        public delegate void ExceptionCallback(Socket socket, Exception exc);

        public static Socket SocketSetup
        {
            get
            {
                Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // Set the receive buffer size to 8k
                s.ReceiveBufferSize = 8192;
                // Set the send buffer size to 8k.
                s.SendBufferSize = 8192;

                return s;
            }
        }

        #region Data_IN
        //Clientdata thread receives data from Clients
        public static void Data_IN(object cSocket, DataManagerCallback dataCallback, ExceptionCallback excCallback) 
        {
            Socket socket = (Socket)cSocket;

            //Packete werden aufgeteilt und nacheinander gesendet/empfangen
            SocketError error = SocketError.Success;

            try
            {
                while (socket.Connected)
                {
                    byte[] sizeBuf = new byte[4];

                    int byteLenngth = socket.Receive(sizeBuf, 0, sizeBuf.Length, SocketFlags.None, out error);

                    if (error == SocketError.Success && byteLenngth > 0)
                    {
                        uint size = BitConverter.ToUInt32(sizeBuf, 0);  //bytes to Int

                        Packet p = Read_Data_Stream(socket, size); //Read_Data_Stream(clientSocket, size);

                        if (p != null)
                        {
                            dataCallback(p);  //an Hauptklasse weiterleiten
                        }
                    }
                    else
                    {
                        throw new Exception("Socket disconnected!");
                    }
                }
            }
            catch (Exception exp)
            {
                excCallback(socket, exp);
                return;
            }
        }

        private static Packet Read_Data_Stream(Socket clientSocket, uint size)
        {
            MemoryStream ms = new MemoryStream();
            uint predictedSize = size;

            while (size > 0)
            {
                Thread.Sleep(1);    //buffer muss sich erst füllen
                byte[] buffer;
                if (size < clientSocket.ReceiveBufferSize)
                {
                    buffer = new byte[size];
                }
                else
                {
                    buffer = new byte[clientSocket.ReceiveBufferSize];
                }

                uint rec = Convert.ToUInt32(clientSocket.Receive(buffer, 0, buffer.Length, 0));
                size -= rec;

                ms.Write(buffer, 0, buffer.Length);
            }
            ms.Close();
            byte[] recData = ms.ToArray();
            ms.Dispose();

            if (predictedSize != recData.Length)
            {
                throw new Exception("Packetdaten verloren!---------------------");
            }
            try
            {
                Packet p = PacketHandler.DeserializePacket(recData);
                return p;
            }
            catch
            {
                throw new Exception("Packet konnte nicht entpackt werden!");
            }
        }
        #endregion

        public static void SendPacket(Socket socket, Packet p, ExceptionCallback excCall)
        {
            try
            {
                byte[] data = PacketHandler.SerializePacket(p);

                socket.SendBufferSize = data.Length;
                socket.Send(BitConverter.GetBytes(data.Length), 0, 4, 0);  //byte Anzahl senden
                socket.Send(data); //byte array senden
            }
            catch(Exception exc)
            {
                excCall(socket, exc);
                return;
            }
        }
    }
}
