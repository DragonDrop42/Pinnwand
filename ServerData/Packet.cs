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
        Authentication,
        DataTable,
        System_Error
    }
    public enum AuthenticationState_SERVER_Events  //Für alle sichtbar || Anmeldung
    {
        Default,
        SERVER_Register_ID, //first contact with server
        //Response
        SERVER_Login,
        SERVER_Registraition,
        SERVER_Klassenwahl_Response,

    }
    public enum AuthenticationState_CLIENT_Events  //Für alle sichtbar || Anmeldung
    {
        Default,
        USER_Registration_Request,
        USER_Login_Request,

        //Get al Classnamens
        USER_Klassenwahl_Request
    }

    public enum DataTableType_SERVER_Events  //Für alle sichtbar
    {
        Default,
        //USER_Kurswahl_Response,
        SERVER_Kurswahl_Response
    }
    public enum DataTableType_CLIENT_Events  //Für alle sichtbar
    {
        Default,
        //Kurswahl
        USER_Kurswahl_Request,
    }

    [Serializable]
    public class Packet
    {
        public PacketType packetType;   //klassifizierung
        public AuthenticationState_SERVER_Events authState_SERVER;   //LoginState
        public bool success;
        public DataTableType_SERVER_Events tableType_SERVER;    //dataState

        public AuthenticationState_CLIENT_Events authState_CLIENT;   //LoginState
        public DataTableType_CLIENT_Events tableType_CLIENT;

        public ListDictionary lst_Dir_Auth;
        public string informationString;

        public ListDictionary lst_Dir_DataTable;  //anstatt von DataTable
        public string senderID;

        //WaitPacket
        public Packet(AuthenticationState_SERVER_Events authT, DataTableType_SERVER_Events dataT)
        {
            authState_SERVER = authT;
            tableType_SERVER = dataT;
        }

        //Messages++++++++++++++++++++
        public Packet(string _informationString)
        {
            informationString = _informationString;
            senderID = "server";
            packetType = PacketType.System_Error;
        }

        //Send auth++++++++++++++++++++
        //Client
        public Packet(AuthenticationState_CLIENT_Events type, string ID, ListDictionary authData) //Authentication Packet
        {
            authState_CLIENT = type;
            packetType = PacketType.Authentication;
            lst_Dir_Auth = authData;   //Informationen wie Passwörter Username...
            senderID = ID;
        }
        //Server
        public Packet(AuthenticationState_SERVER_Events type, ListDictionary authData) //Authentication Packet
        {
            authState_SERVER = type;
            packetType = PacketType.Authentication;
            lst_Dir_Auth = authData;   //Informationen wie Passwörter Username...
            senderID = "server";
        }

        public Packet(AuthenticationState_SERVER_Events type, string error, bool flag) //Authentication Packet
        {
            authState_SERVER = type;
            packetType = PacketType.Authentication;
            informationString = error;
            success = flag;
            senderID = "server";
        }

        //Send data as list+++++++++++++
        public Packet(DataTableType_CLIENT_Events type, string ID, List<string> dataL) //Tabellen Packet
        {
            tableType_CLIENT = type;
            packetType = PacketType.DataTable;
            lst_Dir_DataTable = new ListDictionary();
            lst_Dir_DataTable.Add("lst<string>", dataL);
            senderID = ID;
        }
        public Packet(DataTableType_SERVER_Events type, List<string> dataL, string error, bool flag) //Tabellen Packet
        {
            tableType_SERVER = type;
            packetType = PacketType.DataTable;
            lst_Dir_DataTable = new ListDictionary();
            lst_Dir_DataTable.Add("lst<string>", dataL);
            senderID = "server";
            
            informationString = error;
            success = flag;
        }

        //Send DataTable+++++++++++++++++
        public Packet(DataTableType_CLIENT_Events type, string ID, DataTable table) //Tabellen Packet
        {
            tableType_CLIENT = type;
            packetType = PacketType.DataTable;

            lst_Dir_DataTable = PacketHandler.ConvertTableToList(table);  //Tabelle
            senderID = ID;
        }
        public Packet(DataTableType_SERVER_Events type, DataTable table, string error, bool flag) //Tabellen Packet
        {
            tableType_SERVER = type;
            packetType = PacketType.DataTable;

            lst_Dir_DataTable = PacketHandler.ConvertTableToList(table);  //Tabelle
            senderID = "server";

            informationString = error;
            success = flag;
        }
    }
}