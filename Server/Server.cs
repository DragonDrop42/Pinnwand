using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
//Projekte einbinden
using ServerData;
using PiaLib;
using System.Data;
using System.Collections.Specialized;

namespace Server
{
    class Server
    {
        //DB+++++++++++
        static PinnwandDBAdapter db_Manager;
        //+++++++++++++

        //Start Server
        static void Main(string[] args)
        {
            //DB++
            db_Manager = new PinnwandDBAdapter();
            //---
            //Settings
            int port = 4444;
            string ip = PacketHandler.GetIPAddress();

            ClientHandler.StartServer(ip, port);    //Server starten
        }


        #region Anfrage bearbeiten
        //Data Manager
        //gillt für alle verbundenen Clients
        public static void DataManager(Packet p)
        {
            try
            {
                ClientData client = ClientHandler.GetClientByID(p.senderID);
                if (client == null)
                {
                    return;
                }

                //Login check------------------------------------------------------------
                if (client.email == null || p.packetType == PacketType.Authentication)
                {
                    CheckAuthState(p, client);
                    return;
                }
                //-----------------------------------------------------------------------

                //Status: LoggedIn = true
                switch (p.packetType)
                {
                    case PacketType.DataTable:
                        switch (p.tableType_CLIENT)
                        {
                            case DataTableType_CLIENT_Events.USER_Kurswahl_Request:
                                Kurswahl(client);
                                break;
                        }

                        break;

                    default:
                        ClientHandler.Send_Error_Message_to_Client(client, "Unerwartetes Packet!!!");
                        break;
                }
            }
            catch(Exception exc)
            {
                ClientHandler.Ausgabe("PacketManager", exc.Message);
            }
        }
        #endregion

        static void Klassenwahl(ClientData client)
        {
            //idAbfragen
            //kurse abfragen
            DataTable tableKlassen;
            DatenbankArgs args = db_Manager.Schüler.getKlassenNamen();

            Console.WriteLine(args.DataDebug);

            if (args.Success == false)
            {
                ClientHandler.Ausgabe("Klassenwahl", "null");
                return;
            }
            tableKlassen = args.Data;

            //Packet response = new Packet(AuthenticationState_SERVER_Events.SERVER_Klassenwahl_Response, PacketHandler.ConvertTableToList(args.Data)[0],args.Error,args.Success);
            //ClientHandler.SendSinglePacket(client, response);

        }

        static void Kurswahl(ClientData client)
        {
            //idAbfragen
            //kurse abfragen
            DatenbankArgs args = db_Manager.Schüler.getby(client.email, "S_Email");
            if (args.Success)
            {
                int id = (int) args.Data.Rows[0][0];

                ClientHandler.Ausgabe("Kurswahl", ("Schueler ID: " + args.Data.Rows[0][0]));
                args = db_Manager.Schüler.getKurse(id);
            }

            Packet response = new Packet(DataTableType_SERVER_Events.SERVER_Kurswahl_Response, args.Data, args.Error, args.Success);
            ClientHandler.SendSinglePacket(client, response);
        }
        
        static void CheckAuthState(Packet p, ClientData client)
        {
            Packet response = null;

            switch (p.authState_CLIENT)
            {
                case AuthenticationState_CLIENT_Events.USER_Klassenwahl_Request:
                    Klassenwahl(client);
                    break;

                case AuthenticationState_CLIENT_Events.USER_Login_Request:
                    //DB-----   Try Login

                    ClientHandler.Ausgabe("Auth", ("Email: " + p.lst_Dir_Auth["email"].ToString() + " Passwort: " + p.lst_Dir_Auth["passwort"]) + " try to login");

                    DatenbankArgs args = db_Manager.Schüler.login(p.lst_Dir_Auth["email"].ToString(), p.lst_Dir_Auth["passwort"].ToString());
                    if (args.Success)
                    {
                        ClientHandler.Ausgabe("Auth", (p.lst_Dir_Auth["email"] + " wurde erfolgreich eingeloggt"));
                        client.email = p.lst_Dir_Auth["email"].ToString();  //email als Erkennungsmerkmal setzen

                        response = new Packet(AuthenticationState_SERVER_Events.SERVER_Login, "", true);
                    }
                    else
                    {
                        response = new Packet(AuthenticationState_SERVER_Events.SERVER_Login, args.Error, false);
                    }
                    //------
                    break;
                case AuthenticationState_CLIENT_Events.USER_Registration_Request: //Register user

                    args = db_Manager.Schüler.add(p.lst_Dir_Auth["name"].ToString(), p.lst_Dir_Auth["vname"].ToString(), p.lst_Dir_Auth["phone"].ToString(), p.lst_Dir_Auth["email"].ToString(), p.lst_Dir_Auth["klasse"].ToString(), p.lst_Dir_Auth["passwort"].ToString());
                    if (args.Success)
                    {
                        //client.email = p.auth_keyList["email"].ToString();  //email als Erkennungsmerkmal setzen
                        ClientHandler.Ausgabe("Auth", (p.lst_Dir_Auth["email"] + " wurde erfolgreich registriert"));
                        response = new Packet(AuthenticationState_SERVER_Events.SERVER_Registraition, "", true);
                    }
                    else
                    {
                        response = new Packet(AuthenticationState_SERVER_Events.SERVER_Registraition, args.Error, false);
                    }
                    break;
                default:
                    response = new Packet("Bitte führe eine Registrierung oder Anmeldung durch!");
                    break;
            }

            if (response != null)
            {
                ClientHandler.SendSinglePacket(client, response);
            }

        }
    }
}
