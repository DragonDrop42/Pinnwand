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
                //Anmeldunsfreie Packete+++++++++++++++++++++++++++++++++++++++++++++++
                if(PublicPacketHandler(p, client))
                {
                    return; //packet ist bbereits bearbeitet
                }

                //Anmeldungspflicht+++++++++++++++++++++++++++++++++++++++++++++++++++++
                if(ClientHandler.checkLoginState(client.id))
                {
                    ClientHandler.Send_Error_Message_to_Client(client, "Bitte Anmeldung durchführen!");
                    return;
                }

                //Angemeldet: (gesicherter Bereich)
                switch (p.packetType)
                {
                    case PacketType.Kurswahl:
                        Kurswahl(client);
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
          
        static bool PublicPacketHandler(Packet p, ClientData client)
        {
            Packet response = null;

            switch (p.packetType)
            {
                case PacketType.Schüler_Login:
                    //DB-----   Try Login
                    ClientHandler.Ausgabe("Auth", ("Email: " + p.Data["email"].ToString() + " Passwort: " + p.Data["passwort"]) + " try to login");

                    DatenbankArgs args = db_Manager.Schüler.login(p.Data["email"].ToString(), p.Data["passwort"].ToString());
                    if (args.Success)
                    {
                        ClientHandler.Ausgabe("Auth", (p.Data["email"] + " wurde erfolgreich eingeloggt"));
                        client.email = p.Data["email"].ToString();  //email als Erkennungsmerkmal setzen

                        ClientHandler.ClientLogin(client.id);   //In liste schreiben
                    }
                    else
                    {
                        ClientHandler.Ausgabe("Auth", (p.Data["email"] + " Login fehlgeschlagen!"));
                    }
                    response = new Packet(PacketType.Schüler_Login, args.Data, args.Success, args.Error);
                    //------
                    break;
                case PacketType.Lehrer_Login:
                    //DB-----   Try Login
                    ClientHandler.Ausgabe("Auth", ("Email: " + p.Data["email"].ToString() + " Passwort: " + p.Data["passwort"]) + " try to login");

                    args = db_Manager.Lehrer.login(p.Data["email"].ToString(), p.Data["passwort"].ToString());
                    if (args.Success)
                    {
                        ClientHandler.Ausgabe("Auth", (p.Data["email"] + " wurde erfolgreich eingeloggt"));
                        client.email = p.Data["email"].ToString();  //email als Erkennungsmerkmal setzen

                        ClientHandler.ClientLogin(client.id);   //In liste schreiben
                    }
                    else
                    {
                        ClientHandler.Ausgabe("Auth", (p.Data["email"] + " Login fehlgeschlagen!"));
                    }
                    response = new Packet(PacketType.Lehrer_Login, args.Data, args.Success, args.Error);
                    //------
                    break;

                case PacketType.Schüler_Registraition: //Register Schüler

                    args = db_Manager.Schüler.add(p.Data["name"].ToString(), p.Data["vname"].ToString(), p.Data["phone"].ToString(), p.Data["email"].ToString(), p.Data["klasse"].ToString(), p.Data["passwort"].ToString());
                    if (args.Success)
                    {
                        ClientHandler.Ausgabe("Auth", (p.Data["email"] + " wurde erfolgreich registriert"));
                    }
                    else
                    {
                        ClientHandler.Ausgabe("Auth", (p.Data["email"] + " Registrierung fehlgeschlagen!"));
                    }
                    response = new Packet(PacketType.Schüler_Registraition, args.Data, args.Success, args.Error);
                    break;
                    
                case PacketType.Lehrer_Registraition: //Register Lehrer

                    args = db_Manager.Lehrer.add(p.Data["vname"].ToString(), p.Data["name"].ToString(), p.Data["anrede"].ToString(), p.Data["email"].ToString(), p.Data["passwort"].ToString(), p.Data["titel"].ToString());
                    if (args.Success)
                    {
                        ClientHandler.Ausgabe("Auth", p.Data["email"] + " wurde erfolgreich registriert");
                    }
                    else
                    {
                        ClientHandler.Ausgabe("Auth", p.Data["email"] + " Registrierung fehlgeschlagen!");
                    }
                    response = new Packet(PacketType.Lehrer_Registraition, args.Data, args.Success, args.Error);
                    break;

                case PacketType.Klassenwahl:
                    response = Klassenwahl(client);
                    break;
            }

            if (response != null)
            {
                ClientHandler.SendSinglePacket(client, response);
                return true;
            }
            return false;
        }

        static Packet Klassenwahl(ClientData client)
        {
            //idAbfragen
            //kurse abfragen
            DatenbankArgs args = db_Manager.Schüler.getKlassenNamen();

            if (args.Success == false)
            {
                ClientHandler.Ausgabe("Klassenwahl", "null");
            }

            return new Packet(PacketType.Klassenwahl, args.Data, args.Success, args.Error);
        }

        static void Kurswahl(ClientData client)
        {
            //idAbfragen
            //kurse abfragen
            DatenbankArgs args = db_Manager.Schüler.getby(client.email, "S_Email");
            if (args.Success)
            {
                int id = (int)args.Data.Rows[0][0];

                ClientHandler.Ausgabe("Kurswahl", ("Schueler ID: " + args.Data.Rows[0][0]));
                args = db_Manager.Schüler.getKurse(id);
            }

            Packet response = new Packet(PacketType.Kurswahl, args.Data, args.Success, args.Error);
            ClientHandler.SendSinglePacket(client, response);
        }
    }
}
