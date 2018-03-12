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

        //server setup+++++++++++
        static int port = 4444;
        static string ip = PacketHandler.GetIPAddress();
        static PinnwandDBAdapter DbAdapter = new PinnwandDBAdapter();

        static string lehrerPasswort = GlobalMethods.passwordToHash("teachersPassword");
        //-----------------------

        //Start Server
        static void Main(string[] args)
        {
            //Server starten
            ClientHandler.StartServer(ip, port);    //Server starten
            Console.WriteLine("Deleting old Data");
            DbAdapter.Klasse.deleteOld();
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

                ClientData client = ClientHandler.GetClientByID(p.SenderId);
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
                Console.WriteLine("received " + p.PacketType);
                switch (p.PacketType)
                {
                    case PacketType.Default:
                        //zum Testen
                        Thread.Sleep(500);
                        ClientHandler.Ausgabe("Debug", "packet Default received");
                        ClientHandler.SendSinglePacket(client, p);

                        break;

                    case PacketType.GetGewählteKurse:
                        GetKurse(client);
                        break;
                        
                    case PacketType.GetAlleKurse:
                        Kurswahl(client);
                        break;
                        
                    case PacketType.KursUpdate:
                        UpdateKurse(client, p);
                        break;

                    case PacketType.GetSchülerInKurs:
                        GetSchülerInKurs(client, p);
                        break;
                    case PacketType.GetSchülerInKlasse:
                        GetSchülerInKlasse(client, p);
                        break;
                    case PacketType.GetChat:
                        GetChat(client);
                        break;
                    case PacketType.SendChatNachricht:
                        SendChatNachricht(client, p);
                        break;
                    case PacketType.GetEreignisse:
                        GetEreignisse(client);
                        break;
                    case PacketType.SendEreigniss:
                        SendEreigniss(client, p);
                        break;
                    case PacketType.KlasseErstellen:
                        KlasseErstellen(client, p);
                        break;
                    case PacketType.GetKlasse:
                        getKlasse(client);
                        break;
                    case PacketType.CreateKurs:
                        CreateKurs(client, p);
                        break;
                    case PacketType.GetLehrer:
                        GetLehrer(client);
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

        private static void GetLehrer(ClientData client)
        {
            DatenbankArgs args = client.db_Manager.Lehrer.getAll();
            if (!args.Success)
            {
                ClientHandler.Ausgabe("GetLehrer", args.Error);
            }

            Packet response = new Packet(PacketType.GetLehrer, args.Data, args.Success, args.Error);
            ClientHandler.SendSinglePacket(client, response);
        }

        private static void CreateKurs(ClientData client, Packet p)
        {
            Packet response;
            if (client.hasRights)
            {
                DatenbankArgs args = client.db_Manager.Kurse.add(
                    (string)p.Data["K_Name"],
                    (int)p.Data["L_ID"],
                    (int)p.Data["Kl_ID"]);
                response = new Packet(PacketType.CreateKurs, args.Data, args.Success, args.Error);
            }
            else
            {
                response = new Packet(PacketType.CreateKurs, new DataTable() , false , "Nicht genügend Rechte");
            }
            ClientHandler.SendSinglePacket(client, response);
        }

        private static void GetSchülerInKlasse(ClientData client, Packet packet)
        {
            DatenbankArgs args = client.db_Manager.Klasse.getSchüler(
                Convert.ToInt32(packet.Data["Kl_ID"])
                );
            if (!args.Success)
            {
                ClientHandler.Ausgabe("GetSchülerInKlasse", "null");
            }

            Packet response = new Packet(PacketType.GetSchülerInKlasse, args.Data, args.Success, args.Error);
            ClientHandler.SendSinglePacket(client, response);
        }

        private static void getKlasse(ClientData client)
        {
            DatenbankArgs args;
            if (client.hasRights)
            {
                args = client.db_Manager.Lehrer.getby(client.email, "L_Email");
            }
            else
            {
                args = client.db_Manager.Schüler.getKlasse(client.email);
            }

            Packet response = new Packet(PacketType.GetKlasse,args.Data,args.Success,args.Error);
            ClientHandler.SendSinglePacket(client, response);
        }

        private static void UpdateAll()
        {
            Packet contentChanged = new Packet(PacketType.UpdateAll);
            ClientHandler.SendPacketToAllLoggedInClients(contentChanged);
        }

        private static void KlasseErstellen(ClientData client, Packet p)
        {
            Packet response;
            if (client.hasRights)
            {
                DatenbankArgs args = client.db_Manager.Klasse.add(
                    (string)p.Data["Kl_Name"],
                    (DateTime)p.Data["Kl_Abschlussdatum"]);
                response = new Packet(PacketType.KlasseErstellen, args.Data, args.Success, args.Error);
            }
            else
            {
                response = new Packet(PacketType.KlasseErstellen, new DataTable() , false , "Nicht genügend Rechte");
            }
            ClientHandler.SendSinglePacket(client, response);
        }

        private static void SendEreigniss(ClientData client, Packet p)
        {
            DatenbankArgs args = client.db_Manager.Kurse.sendEreigniss(
                (int)p.Data["K_ID"],
                (string)p.Data["E_Art"],
                (DateTime)p.Data["E_Fälligkeitsdatum"],
                client.autor, //(string)p.Data["E_Autor"],
                (string)p.Data["E_Beschreibung"]);
            if (!args.Success)
            {
                ClientHandler.Ausgabe("SendEreigniss", "null");
            }
            else
            {
                Console.WriteLine(client.autor + ": " + (string)p.Data["E_Beschreibung"]);
            }
            Packet response = new Packet(PacketType.SendEreigniss, args.Data, args.Success, args.Error);
            ClientHandler.SendSinglePacket(client, response);
        }

        private static void GetEreignisse(ClientData client)
        {
            DatenbankArgs args = client.db_Manager.Kurse.getEreignisse();
            if (!args.Success)
            {
                ClientHandler.Ausgabe("GetEreignisse", "null");
            }

            Packet response = new Packet(PacketType.GetEreignisse, args.Data, args.Success, args.Error);
            ClientHandler.SendSinglePacket(client, response);
        }

        //Case Methoden
        private static void SendChatNachricht(ClientData client, Packet p)
        {
            DatenbankArgs args = client.db_Manager.Kurse.sendChat(
                client.autor,           //(string)p.Data["C_Sendername"],
                (string)p.Data["C_Inhalt"],
                (int)p.Data["K_ID"]);
            if (!args.Success)
            {
                ClientHandler.Ausgabe("sendChat","null");
            }
            else
            {
                Console.WriteLine(client.autor + ": "+(string)p.Data["C_Inhalt"]);
                //An alle eingelogten Clients senden
                //UpdateAll();
            }
            Packet response = new Packet(PacketType.SendChatNachricht, null, args.Success, args.Error);
            ClientHandler.SendSinglePacket(client, response);

            //neue Nachricht wurde empfangen -> an alle Clients weiterleiten
            Thread.Sleep(50);
            //update Packet
            args = client.db_Manager.Kurse.getChat();
            if (!args.Success)
            {
                ClientHandler.Ausgabe("sendChat", "SendChatUpdate");
            }
            else
            {
                Packet updateChat = new Packet(PacketType.UpdateChat, args.Data, "server");
                ClientHandler.SendPacketToAllLoggedInClients(updateChat);
            }
           
        }

        private static void GetChat(ClientData client)
        {
            DatenbankArgs args = client.db_Manager.Kurse.getChat();
            if (!args.Success)
            {
                ClientHandler.Ausgabe("getChat", "null");
            }

            Console.WriteLine(client.email);
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
                if (client.hasRights)
                {
                    DatenbankArgs args = client.db_Manager.Lehrer.getby(client.email, "L_Email");
                    if (args.Success)
                    {
                        int id = (int) args.Data.Rows[0][0];

                        ClientHandler.Ausgabe("GetKurse", "Lehrer ID: " + args.Data.Rows[0][0]);
                        args = client.db_Manager.Lehrer.getKurse(id);
                    }

                    Packet response = new Packet(PacketType.GetGewählteKurse, args.Data, args.Success, args.Error);
                    ClientHandler.SendSinglePacket(client, response);
                
                }
                else
                {
                    DatenbankArgs args = client.db_Manager.Schüler.getby(client.email, "S_Email");
                    if (args.Success)
                    {
                        int id = (int) args.Data.Rows[0][0];

                        ClientHandler.Ausgabe("GetKurse", "Schueler ID: " + args.Data.Rows[0][0]);
                        args = client.db_Manager.Schüler.getKurse(id);
                    }

                    Packet response = new Packet(PacketType.GetGewählteKurse, args.Data, args.Success, args.Error);
                    ClientHandler.SendSinglePacket(client, response);
                }
            }
            catch(Exception exc)
            {
                throw new Exception(">>GetKurse: " + exc);
            }
        }

        public static void UpdateKurse(ClientData client, Packet packet)
        {
            try
            {
                //idAbfragen
                //kurse abfragen
                if (client.hasRights)
                {
                    int id = (int)client.db_Manager.Lehrer.getby(client.email, "L_Email").Data.Rows[0][0];
                    DataTable data = client.db_Manager.Kurse.getAll().Data;
                    List<DataRow> dataRows= new List<DataRow>();
                    foreach (string s in (List<string>) packet.Data["K_ID"])
                    {
                        foreach (DataRow row in data.Rows)
                        {
                            if (row["K_ID"].ToString() == s)
                            {
                                row["L_ID"] = id;
                                dataRows.Add(row); 
                            }
                        }
                    }

                    client.db_Manager.Kurse.changeLehrer(dataRows.ToArray());
                    Packet response = new Packet(PacketType.KursUpdate, new DataTable(), true, "");
                    ClientHandler.SendSinglePacket(client, response);
                }
                else
                {
                    DatenbankArgs args = client.db_Manager.Schüler.getby(client.email, "S_Email");
                    if (args.Success)
                    {
                        int id = (int) args.Data.Rows[0][0];

                        ClientHandler.Ausgabe("KursUpdate", "Schueler ID: " + args.Data.Rows[0][0]);
                        args = client.db_Manager.Schüler.updateKurse(id, (List<string>) packet.Data["K_ID"]);
                    }

                    Packet response = new Packet(PacketType.KursUpdate, args.Data, args.Success, args.Error);
                    ClientHandler.SendSinglePacket(client, response);
                }
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
                DatenbankArgs args;
                if (client.hasRights)
                {
                    args = client.db_Manager.Kurse.getAll();
                }
                else
                {
                    args = client.db_Manager.Schüler.getby(client.email, "S_Email");
                    if (args.Success)
                    {
                        int id = (int) args.Data.Rows[0][5];

                        ClientHandler.Ausgabe("Kurswahl", "Schueler ID: " + args.Data.Rows[0][0]);
                        args = client.db_Manager.Kurse.getByKlasse(id);
                    }
                }

                Packet response = new Packet(PacketType.GetAlleKurse, args.Data, args.Success, args.Error);
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

            switch (p.PacketType)
            {
                case PacketType.SchülerLogin:
                    //DB-----   Try Login
                    ClientHandler.Ausgabe("Auth", "Email: " + p.Data["email"] + " Passwort: " + p.Data["passwort"] + " try to login");

                    DatenbankArgs args = client.db_Manager.Schüler.login(p.Data["email"].ToString(), p.Data["passwort"].ToString());
                    if (args.Success)
                    {
                        ClientHandler.ClientLogin(client.id);   //In liste schreiben
                        //Console.WriteLine(ClientHandler.checkLoginState(client.id));
                        //Daten speichern
                        client.email = p.Data["email"].ToString();  //email als Erkennungsmerkmal setzen
                        client.name = (string)args.Data.Rows[0]["S_Name"];
                        client.vname = (string)args.Data.Rows[0]["S_Vorname"];
                        client.SetAutorKürzel();
                        ClientHandler.Ausgabe("Auth", client.vname + "." + client.name + "." + client.email + " (Schüler) eingeloggt");
                    }
                    else
                    {
                        ClientHandler.Ausgabe("Auth", p.Data["email"] + " Login fehlgeschlagen!");
                    }
                    response = new Packet(PacketType.SchülerLogin, args.Data, args.Success, args.Error);
                    //------
                    break;
                case PacketType.LehrerLogin:
                    //DB-----   Try Login
                    ClientHandler.Ausgabe("Auth", "Email: " + p.Data["email"] + " Passwort: " + p.Data["passwort"] + " try to login");

                    args = client.db_Manager.Lehrer.login(p.Data["email"].ToString(), p.Data["passwort"].ToString());
                    if (args.Success)
                    {
                        ClientHandler.ClientLogin(client.id);   //In liste schreiben
                        
                        client.email = p.Data["email"].ToString();  //email als Erkennungsmerkmal setzen
                        client.name = (string)args.Data.Rows[0]["L_Name"];
                        client.vname = (string)args.Data.Rows[0]["L_Vorname"];
                        client.SetAutorKürzel();
                        client.hasRights = true;
                        ClientHandler.Ausgabe("Auth", client.vname + "." + client.name + "." + client.email + " (Lehrer) eingeloggt");
                    }
                    else
                    {
                        ClientHandler.Ausgabe("Auth", p.Data["email"] + " Login fehlgeschlagen!");
                    }
                    response = new Packet(PacketType.LehrerLogin, args.Data, args.Success, args.Error);
                    //------
                    break;

                case PacketType.SchülerRegistraition: //Register Schüler

                    args = client.db_Manager.Schüler.add(p.Data["name"].ToString(), p.Data["vname"].ToString(), p.Data["phone"].ToString(), p.Data["email"].ToString(), p.Data["klasse"].ToString(), p.Data["passwort"].ToString());
                    if (args.Success)
                    {
                        ClientHandler.Ausgabe("Auth", p.Data["email"] + " wurde erfolgreich registriert");
                    }
                    else
                    {
                        ClientHandler.Ausgabe("Auth", p.Data["email"] + " Registrierung fehlgeschlagen!");
                    }
                    response = new Packet(PacketType.SchülerRegistraition, args.Data, args.Success, args.Error);
                    break;
                    
                case PacketType.LehrerRegistraition: //Register Lehrer

                    if ((string)p.Data["lehrerPasswort"] != lehrerPasswort)
                    {
                        args = new DatenbankArgs("Lehrer Passwort falsch.\n Versuchen Sie es erneut.");
                    }
                    else
                    {
                        args = client.db_Manager.Lehrer.add(p.Data["vname"].ToString(), p.Data["name"].ToString(), p.Data["anrede"].ToString(), p.Data["email"].ToString(), p.Data["passwort"].ToString(), p.Data["titel"].ToString());
                    }
                    if (args.Success)
                    {
                        ClientHandler.Ausgabe("Auth", p.Data["email"] + " wurde erfolgreich registriert");
                    }
                    else
                    {
                        ClientHandler.Ausgabe("Auth", p.Data["email"] + " Registrierung fehlgeschlagen!");
                    }
                    response = new Packet(PacketType.LehrerRegistraition, args.Data, args.Success, args.Error);
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
