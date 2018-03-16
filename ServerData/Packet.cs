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
        RegisterId,                        // (✔)
        SchülerLogin,                      // (✔)
        SchülerRegistraition,              // (✔)
        LehrerLogin,                       // (✔)
        LehrerRegistraition,               // (✔)
        Klassenwahl,                        // (✔)
        //--------------------------------

        //Datenpackete++++++++++++++++++++
        GetSchülerInKurs,                   //Liste der Schüler in einem Kurs (✔)
        GetSchülerInKlasse,                 // (✔)
        GetAlleKurse,                       // (✔)
        GetGewählteKurse,                   // (✔)
        KursUpdate,                         // (✔)
        SendChatNachricht,                  // (✔)
        GetChat,                            // (✔)
        GetKlasse,                          // (✔)
        GetEreignisse,                      // (✔)
        SendEreigniss,                      // (✔)
        GetLehrerofKurs,
        
        //--------------------------------

        //Admin oder Lehrerpakete+++++++++
        UpdateSchülerInKurs,
        UpdateSchülerInKlasse,
        KlasseErstellen,                    // (✔)
        KlasseLöschen,
        CreateKurs,
        GetLehrer,
        DeleteEreignis,
        EditEreigniss,
        //--------------------------------
        //SystemError
        SystemError,
        //Update
        UpdateChat,
        UpdateAll,
    }


    [Serializable]
    public class Packet
    {
        public PacketType PacketType;   //klassifizierung

        public ListDictionary Data;
        public bool Success;
        public string MessageString;

        public string SenderId;

        //WaitPacket
        public Packet(PacketType type)
        {
            PacketType = type;
        }
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //standard Client Packet
        public Packet(PacketType type, ListDictionary data, string id)
        {
            PacketType = type;
            Data = data;
            Success = true;
            MessageString = "";
            SenderId = id;
        }
        //dateTAble als Daten
        public Packet(PacketType type, DataTable data, string id)
        {
            PacketType = type;
            Data = PacketHandler.ConvertTableToList(data);
            Success = true;
            MessageString = "";
            SenderId = id;
        }

        //client id
        public Packet(PacketType type, string id)
        {
            PacketType = type;
            SenderId = id;
        }

        //---------------------------------------------------------------------------Server

        //Server default packet
        public Packet(PacketType type, DataTable data, bool success, string message)
        {
            PacketType = type;
            Data = PacketHandler.ConvertTableToList(data);
            Success = success;
            MessageString = message;
        }

        //Messages++++++++++++++++++++
        public Packet(string error)
        {
            PacketType = PacketType.SystemError;
            SenderId = "server";
            MessageString = error;
        }

        public Packet Copy()
        {
            Packet p = new Packet(PacketType);
            p.Data = Data;
            p.Success = Success;
            p.MessageString = MessageString;
            p.SenderId = SenderId;

            return p;
        }
    }
}