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
        Register_ID,                        // (✔)
        Schüler_Login,                      // (✔)
        Schüler_Registraition,              // (✔)
        Lehrer_Login,                       // (✔)
        Lehrer_Registraition,               // (✔)
        Klassenwahl,                        // (✔)
        //--------------------------------

        //Datenpackete++++++++++++++++++++
        GetSchülerInKurs,                   //Liste der Schüler in einem Kurs (✔)
        GetSchülerInKlasse,                 // ( )
        GetAlleKurse,                       // (✔)
        GetGewählteKurse,                   // (✔)
        KursUpdate,                         // (✔)
        SendChatNachricht,                  // (✔)
        GetChat,                            // (✔)
        GetEreignisse,                      // (✔)
        SendEreigniss,                      // (✔)
        //--------------------------------
        
        //Admin oder Lehrerpakete+++++++++
        UpdateSchülerInKurs,
        UpdateSchülerInKlasse,
        KlasseErstellen,                    // (✔)
        KlasseLöschen,
        //--------------------------------
        //SystemError
        SystemError,
        //Update
        UpdateChat,
        UpdateAll
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
        //dateTAble als Daten
        public Packet(PacketType type, DataTable data, string id)
        {
            this.packetType = type;
            this.Data = PacketHandler.ConvertTableToList(data);
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
            this.packetType = PacketType.SystemError;
            this.senderID = "server";
            this.MessageString = error;
        }




        public Packet Copy()
        {
            Packet p = new Packet(this.packetType);
            p.Data = this.Data;
            p.Success = this.Success;
            p.MessageString = this.MessageString;
            p.senderID = this.senderID;

            return p;
        }
    }
}