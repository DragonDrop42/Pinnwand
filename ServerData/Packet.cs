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
        SERVER_Login_Accepted,
        SERVER_Login_Failed,
        SERVER_Registraition_Accepted,
        SERVER_Registraition_Failed,
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
        public DataTableType_SERVER_Events tableType_SERVER;    //dataState

        public AuthenticationState_CLIENT_Events authState_CLIENT;   //LoginState
        public DataTableType_CLIENT_Events tableType_CLIENT;

        public ListDictionary auth_keyList;
        public string informationString;

        public List<string> lst_data;
        public ListDictionary lst_TableDictionary;  //anstatt von DataTable
        public string senderID;

        //DB
        //public List<string> loginData;

        //public Packet() { }

        //public Packet(PacketType type, string ID)
        //{
        //    lst_data = new List<string>();
        //    senderID = ID;
        //    packetType = type;
        //}
        //Messages++++++++++++++++++++
        public Packet(string _informationString)
        {
            lst_data = new List<string>();
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
            lst_data = new List<string>();
            auth_keyList = authData;   //Informationen wie Passwörter Username...
            senderID = ID;
        }
        //Server
        public Packet(AuthenticationState_SERVER_Events type, ListDictionary authData) //Authentication Packet
        {
            authState_SERVER = type;
            packetType = PacketType.Authentication;
            lst_data = new List<string>();
            auth_keyList = authData;   //Informationen wie Passwörter Username...
            senderID = "server";
        }

        public Packet(AuthenticationState_SERVER_Events type, string error) //Authentication Packet
        {
            authState_SERVER = type;
            packetType = PacketType.Authentication;
            informationString = error;
            senderID = "server";
        }

        //Send data as list+++++++++++++
        public Packet(DataTableType_CLIENT_Events type, string ID, List<string> dataL) //Tabellen Packet
        {
            tableType_CLIENT = type;
            packetType = PacketType.DataTable;

            lst_data = dataL;  //Tabelle
            senderID = ID;
        }
        public Packet(DataTableType_SERVER_Events type, List<string> dataL) //Tabellen Packet
        {
            tableType_SERVER = type;
            packetType = PacketType.DataTable;

            lst_data = dataL;  //Tabelle
            senderID = "server";
        }

        //Send DataTable+++++++++++++++++
        public Packet(DataTableType_CLIENT_Events type, string ID, DataTable table) //Tabellen Packet
        {
            tableType_CLIENT = type;
            packetType = PacketType.DataTable;

            lst_TableDictionary = PacketHandler.ConvertTableToList(table);  //Tabelle
            senderID = ID;
        }
        public Packet(DataTableType_SERVER_Events type, DataTable table) //Tabellen Packet
        {
            tableType_SERVER = type;
            packetType = PacketType.DataTable;

            lst_TableDictionary = PacketHandler.ConvertTableToList(table);  //Tabelle
            senderID = "server";
        }
    }
}