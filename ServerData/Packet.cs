using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerData;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Net;
using System.Data;
using System.Collections.Specialized;

namespace ServerData
{
    public enum PacketType  //Für alle sichtbar
    {
        Default,

        //Authentication++++++++++++++++++
        Register_ID,
        Login,
        Registraition,
        Klassenwahl,
        //--------------------------------

        //Datenpackete++++++++++++++++++++
        Kurswahl,
        //--------------------------------

        //SystemError
        System_Error
    }

    [Serializable]
    public class Packet
    {
        public PacketType packetType;   //klassifizierung

        public ListDictionary Data;
        public bool Success;
        public string MessageString;

        public string senderID;

        //WaitPacket
        public Packet(PacketType type)
        {
            this.packetType = type;
        }
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //standard Client Packet
        public Packet(PacketType type, ListDictionary data, string id)
        {
            this.packetType = type;
            this.Data = data;
            this.Success = true;
            this.MessageString = "";
            this.senderID = id;
        }

        //client id
        public Packet(PacketType type, string id)
        {
            this.packetType = type;
            this.senderID = id;
        }

        //---------------------------------------------------------------------------Server

        //Server default packet
        public Packet(PacketType type, DataTable data, bool success, string message)
        {
            this.packetType = type;
            this.Data = PacketHandler.ConvertTableToList(data);
            this.Success = success;
            this.MessageString = message;
        }

        //Messages++++++++++++++++++++
        public Packet(string error)
        {
            this.packetType = PacketType.System_Error;
            this.senderID = "server";
            this.MessageString = error;
        }
    }
}