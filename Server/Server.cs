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
        //static PinnwandDBAdapter db_Manager;
        //+++++++++++++

        //Start Server
        static void Main(string[] args)
        {
            //DB++
            //db_Manager = new PinnwandDBAdapter();
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
                //warten bis Packet verarbeitet wird --> Socket bereit
                Thread.Sleep(100);
                //----------------

                ClientData client = ClientHandler.GetClientByID(p.senderID);
                if (client == null)
                {
                    return;
                }
                //Anmeldunsfreie Packete+++++++++++++++++++++++++++++++++++++++++++++++
                if(PublicPacketHandler(p, client))
                {
                    return; //packet ist bereits bearbeitet
                }

                //Anmeldungspflicht+++++++++++++++++++++++++++++++++++++++++++++++++++++
                //Console.WriteLine(client.id);
                //Console.WriteLine(ClientHandler.checkLoginState(client.id));
                if(!ClientHandler.checkLoginState(client.id))
                {
                    ClientHandler.Send_Error_Message_to_Client(client, "Bitte Anmeldung durchführen!");
                    return;
                }

                //Angemeldet: (gesicherter Bereich)
                switch (p.packetType)
                {
                    case PacketType.Default:
                        //zum Testen
                        Thread.Sleep(500);
                        ClientHandler.Ausgabe("Debug", "packet Default received");
                        ClientHandler.SendSinglePacket(client, p);

                        break;

                    case PacketType.GetKurseVonSchüler:
                        GetKurse(client);
                        break;
                        
                    case PacketType.GetVerfügbareKurse:
                        Kurswahl(client);
                        break;
                        
                    case PacketType.KursUpdate:
                        UpdateKlassen(client, p);
                        break;

                    case PacketType.GetSchülerInKurs:
                        GetSchülerInKurs(client, p);
                        break;
                    case PacketType.GetChat:
                        GetChat(client, p);
                        break;
                    case PacketType.SendChatNachricht:
                        SendChatNachricht(client, p);
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

        //Case Methoden
        private static void SendChatNachricht(ClientData client, Packet p)
        {
            DatenbankArgs args = client.db_Manager.Kurse.sendChat(
                client.name,           //(string)p.Data["C_Sendername"],
                (string)p.Data["C_Inhalt"],
                (int)p.Data["K_ID"]);
            if (!args.Success)
            {
                ClientHandler.Ausgabe("sendChat","null");
            }
            else
            {
                Console.WriteLine((string)p.Data["C_Sendername"]+": "+(string)p.Data["C_Inhalt"]);
            }
            Packet response = new Packet(PacketType.SendChatNachricht, args.Data, args.Success, args.Error);
            ClientHandler.SendSinglePacket(client, response);
        }

        private static void GetChat(ClientData client, Packet p)
        {
            DatenbankArgs args = client.db_Manager.Kurse.getChat((int)p.Data["K_ID"]);
            if (!args.Success)
            {
                ClientHandler.Ausgabe("getChat", "null");
            }

            Packet response = new Packet(PacketType.GetChat, args.Data, args.Success, args.Error);
            ClientHandler.SendSinglePacket(client, response);
        }

        private static void GetSchülerInKurs(ClientData client, Packet p)
        {
            DatenbankArgs args = client.db_Manager.Kurse.getSchüler((int)p.Data["K_ID"]);
            if (args.Success == false)
            {
                ClientHandler.Ausgabe("getSchülerInKurs", "null");
            }

            Packet response = new Packet(PacketType.GetSchülerInKurs, args.Data, args.Success, args.Error);
            ClientHandler.SendSinglePacket(client, response);
        }

        private static void GetKurse(ClientData client)
        {
            try
            {
                //idAbfragen
                //kurse abfragen
                DatenbankArgs args = client.db_Manager.Schüler.getby(client.email, "S_Email");
                if (args.Success)
                {
                    int id = (int)args.Data.Rows[0][0];

                    ClientHandler.Ausgabe("GetKurse", ("Schueler ID: " + args.Data.Rows[0][0]));
                    args = client.db_Manager.Schüler.getKurse(id);
                }

                Packet response = new Packet(PacketType.GetKurseVonSchüler, args.Data, args.Success, args.Error);
                ClientHandler.SendSinglePacket(client, response);
            }
            catch(Exception exc)
            {
                throw new Exception(">>GetKurse: " + exc);
            }
        }

        public static void UpdateKlassen(ClientData client, Packet packet)
        {
            try
            {
                //idAbfragen
                //kurse abfragen
                DatenbankArgs args = client.db_Manager.Schüler.getby(client.email, "S_Email");
                if (args.Success)
                {
                    int id = (int)args.Data.Rows[0][0];

                    ClientHandler.Ausgabe("KursUpdate", ("Schueler ID: " + args.Data.Rows[0][0]));
                    args = client.db_Manager.Schüler.updateKurse(id, (List<string>)packet.Data["K_ID"]);
                }

                Packet response = new Packet(PacketType.KursUpdate, args.Data, args.Success, args.Error);
                ClientHandler.SendSinglePacket(client, response);
            }
            catch (Exception exc)
            {
                throw new Exception(">>UpdateKlassen: " + exc);
            }
        }

        static Packet Klassenwahl(ClientData client)
        {
            try
            {
                //idAbfragen
                //kurse abfragen
                DatenbankArgs args = client.db_Manager.Schüler.getKlassenNamen();

                if (args.Success == false)
                {
                    ClientHandler.Ausgabe("Klassenwahl", "null");
                }

                return new Packet(PacketType.Klassenwahl, args.Data, args.Success, args.Error);
            }
            catch (Exception exc)
            {
                throw new Exception(">>Klassenwahl: " + exc);
            }
        }

        static void Kurswahl(ClientData client)
        {
            try
            {
                //idAbfragen
                //kurse abfragen
                DatenbankArgs args = client.db_Manager.Schüler.getby(client.email, "S_Email");
                if (args.Success)
                {
                    int id = (int)args.Data.Rows[0][5];

                    ClientHandler.Ausgabe("Kurswahl", ("Schueler ID: " + args.Data.Rows[0][0]));
                    args = client.db_Manager.Kurse.getByKlasse(id);
                }

                Packet response = new Packet(PacketType.GetVerfügbareKurse, args.Data, args.Success, args.Error);
                ClientHandler.SendSinglePacket(client, response);
            }
            catch (Exception exc)
            {
                throw new Exception(">>Kurswahl: " + exc);
            }
        }

        #endregion
        
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ Public Packets
        static bool PublicPacketHandler(Packet p, ClientData client)
        {
            Packet response = null;

            switch (p.packetType)
            {
                case PacketType.Schüler_Login:
                    //DB-----   Try Login
                    ClientHandler.Ausgabe("Auth", ("Email: " + p.Data["email"] + " Passwort: " + p.Data["passwort"]) + " try to login");

                    DatenbankArgs args = client.db_Manager.Schüler.login(p.Data["email"].ToString(), p.Data["passwort"].ToString());
                    if (args.Success)
                    {
                        ClientHandler.ClientLogin(client.id);   //In liste schreiben
                        //Console.WriteLine(ClientHandler.checkLoginState(client.id));
                        //Daten speichern
                        client.email = p.Data["email"].ToString();  //email als Erkennungsmerkmal setzen
                        client.name = (string)args.Data.Rows[0]["S_Name"];
                        client.vname = (string)args.Data.Rows[0]["S_Vorname"];
                        ClientHandler.Ausgabe("Auth", (client.vname + "." + client.name + "." + client.email + " (Schüler) eingeloggt"));
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
                    ClientHandler.Ausgabe("Auth", ("Email: " + p.Data["email"] + " Passwort: " + p.Data["passwort"]) + " try to login");

                    args = client.db_Manager.Lehrer.login(p.Data["email"].ToString(), p.Data["passwort"].ToString());
                    if (args.Success)
                    {
                        ClientHandler.ClientLogin(client.id);   //In liste schreiben
                        
                        client.email = p.Data["email"].ToString();  //email als Erkennungsmerkmal setzen
                        client.name = (string)args.Data.Rows[0]["L_Name"];
                        client.vname = (string)args.Data.Rows[0]["L_Vorname"];
                        ClientHandler.Ausgabe("Auth", (client.vname + "." + client.name + "." + client.email + " (Lehrer) eingeloggt"));
                    }
                    else
                    {
                        ClientHandler.Ausgabe("Auth", (p.Data["email"] + " Login fehlgeschlagen!"));
                    }
                    response = new Packet(PacketType.Lehrer_Login, args.Data, args.Success, args.Error);
                    //------
                    break;

                case PacketType.Schüler_Registraition: //Register Schüler

                    args = client.db_Manager.Schüler.add(p.Data["name"].ToString(), p.Data["vname"].ToString(), p.Data["phone"].ToString(), p.Data["email"].ToString(), p.Data["klasse"].ToString(), p.Data["passwort"].ToString());
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

                    args = client.db_Manager.Lehrer.add(p.Data["vname"].ToString(), p.Data["name"].ToString(), p.Data["anrede"].ToString(), p.Data["email"].ToString(), p.Data["passwort"].ToString(), p.Data["titel"].ToString());
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
                //ClientHandler.Ausgabe("Auth", "Anfrage wurde bearbeitet");
                return true;
            }
            return false;
        }

        
    }
}
