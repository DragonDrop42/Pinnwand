using ServerData;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ClientClassLib
{
    public class Client
    {
        static Socket masterS;
        static string id;
        List<Packet> lst_PacketResponse = new List<Packet>();

        private readonly EventWaitHandle waitHandle = new AutoResetEvent(false);

        TCP_connection.DataManagerCallback packetCallback;
        TCP_connection.ErrorMessageCallback errorCallback;

        public Client(TCP_connection.ErrorMessageCallback errorDel)
        {
            packetCallback = new TCP_connection.DataManagerCallback(PacketManager);
            errorCallback = errorDel;
        }
    

        #region Connect to Server
        //try to connect to server
        public bool Connect(string ip, int port)
        {
            string ipHost = ip;

            masterS = TCP_connection.SocketSetup;

            try
            {
                IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse(ipHost), port);
                masterS.Connect(ipEnd);
            }
            catch (Exception exc)
            {
                Thread.Sleep(500);
                errorCallback("Konnte nicht verbinden || " + exc.Message);
                return false;
            }

            Thread dataINThread = new Thread(DataIN);
            dataINThread.Start(masterS);

            errorCallback("Erfolgreich verbunden");
            return true;
        }
        #endregion

        #region Daten Empgfangen und Senden
        //packete empfangen+++++++++++++++++++++++++++++++++++++++++++++++++++
        public void DataIN(object cSocket)
        {
            TCP_connection.Data_IN(cSocket, packetCallback, new TCP_connection.ExceptionCallback(SocketDisconnectedException));
        }
        //packet senden++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public void SendPacket(Packet p)
        {
            TCP_connection.SendPacket(masterS, p, new TCP_connection.ExceptionCallback(SocketDisconnectedException));
        }
        private void SocketDisconnectedException(Socket socket, Exception exc)
        {
            errorCallback("Socket Error: " + exc.Message);
            Disconnect();
        }
        //Disconnect Client
        public void Disconnect()
        {
            try
            {
                masterS.Shutdown(SocketShutdown.Both);
                masterS.Disconnect(true);
                masterS.Close();
            }
            catch
            {
                errorCallback("Socket konnte nicht geschlossen werden");
            }

        }
        #endregion


        private Packet WaitForPacketResponse(Packet waitPacket)
        { 
            waitHandle.WaitOne(2000);   //2sec. Timeout
            waitHandle.Reset();

            foreach (Packet p in lst_PacketResponse)
            {
                if (p.packetType == waitPacket.packetType)
                {
                    lst_PacketResponse.Remove(p);
                    return p;
                }
            }

            return new Packet("Zeitüberschreitung oder Packet nicht gefunden");
        }


        //PacketManager++++++++++++++++++++++++++++++++++++++++++++++++++++
        private void PacketManager(Packet packet)
        {
            //kein Client Event
            if (packet.packetType == PacketType.Register_ID)
            {
                ID = packet.Data["id"].ToString();
                return;
            }
            else if (packet.packetType == PacketType.SystemError)
            {
                errorCallback(packet.MessageString);
                return;
            }
            //----------------

            //Client Events
            lst_PacketResponse.Add(packet);
            //event auslösen
            waitHandle.Set();
            //------------
            return;
        }

        //---------------------------------------------------------
        #region Login/Register
        //Register+++++++++++++++++++++++++++++++++++++++++++++++++++++++   
        public Packet Register_Schüler(string name, string vname, string phone, string klasse, string email, string passwort)
        {
            //check
            if (name == "" || vname == "" || email == "" || passwort == "" || klasse == "")
            {
                throw new Exception("Bitte alle notwendigen Felder ausfüllen!");
            }
            if (check_email(email) && (passwort = check_password(passwort)) != null)
            {
                //Packet senden
                SendSchülerRegisterPacket(name, vname, phone, klasse, email, passwort);
                //Auf Antwort warten
                return WaitForPacketResponse(new Packet(PacketType.Schüler_Registraition)); 
            }
            return null;
        }
        
        public Packet Register_Lehrer(string vname, string name, string anrede, string email, string passwort, string titel)
        {
            //check
            if (name == "" || vname == "" || email == "" || passwort == "" || anrede == "")
            {
                throw new Exception("Bitte alle notwendigen Felder ausfüllen!");
            }
            if (check_email(email) && (passwort = check_password(passwort)) != null)
            {
                //Packet senden
                if (titel == null) titel = "";
                SendLehrerRegisterPacket(name, vname, anrede, email, passwort, titel);
                //Auf Antwort warten
                return WaitForPacketResponse(new Packet(PacketType.Lehrer_Registraition)); 
            }
            return null;
        }

        //Login+++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public Packet Login(string email, string passwort,bool schüler)
        {
            //check
            if (email == "" || passwort == "")
            {
                throw new Exception("Bitte Anmeldedaten eintragen!");
            }
            if (check_email(email) == true && (passwort = check_password(passwort)) != null)
            {
                //Packet senden
                SendLoginPacket(email, passwort,schüler);
                //auf response warten
                return schüler ? WaitForPacketResponse(new Packet(PacketType.Schüler_Login)):
                    WaitForPacketResponse(new Packet(PacketType.Lehrer_Login));
            }
            return null;
        }
        //Get Klassen
        public Packet GetKlassen()
        {
            SendKlassenPacket();
            return WaitForPacketResponse(new Packet(PacketType.Klassenwahl));
        }
        //Kurswahl+++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public Packet Kurswahl()
        {
            SendKurswahlPacket();
            return WaitForPacketResponse(new Packet(PacketType.Kurswahl));
        }
        //-----------------------------------------------------------
        private bool check_email(string email)
        {
            //check
            if (email.Contains('@') == false)// || (email.Contains(".com")||email.Contains(".net")||email.Contains(".de")) == false)
            {
                throw new Exception("Die eingetragene email ist fehlerhaft!");
            }

            return true;
        }
        private string check_password(string pass)
        {
            if (pass.Length < 5)
            {
                //errorCallback("Passwort ist zu kurz (>5)");
                throw new Exception("Passwort ist zu kurz (>5)");
            }

            //passwort verschlüsslung
            string salt = "492";
            System.Security.Cryptography.SHA1 sha = System.Security.Cryptography.SHA1.Create();
            byte[] preHash = Encoding.UTF32.GetBytes(pass + salt);
            byte[] hash = sha.ComputeHash(preHash);
            string password = Convert.ToBase64String(hash, 0, hash.Length);  //immer 15 Stellen lang
            //
            //errorCallback(password);
            return password;
        }
        //----------------------------------------------------------------------------
        #endregion
        //=>
        #region Communication (Packets)

        private void SendSchülerRegisterPacket(string name, string vname, string phone, string klasse, string email, string passwort)
        {
            ListDictionary data = new ListDictionary{
                {"name", name},
                {"vname", vname},
                {"phone", phone},
                {"klasse", klasse},
                {"email", email},
                {"passwort", passwort}
            };
            Packet register = new Packet(PacketType.Schüler_Registraition, data, id);
            SendPacket(register);
        }
        private void SendLehrerRegisterPacket(string name, string vname, string anrede, string email, string passwort, string titel)
        {
            ListDictionary data = new ListDictionary{
                {"name", name},
                {"vname", vname},
                {"anrede", anrede},
                {"titel", titel},
                {"email", email},
                {"passwort", passwort}
            };
            Packet register = new Packet(PacketType.Lehrer_Registraition, data, id);
            SendPacket(register);
        }

        private void SendLoginPacket(string email, string passwort, bool schüler)
        {
            ListDictionary data = new ListDictionary
            {
                {"email", email},
                {"passwort", passwort}
            };
            SendPacket(schüler ? new Packet(PacketType.Schüler_Login, data, id) : new Packet(PacketType.Lehrer_Login, data, id));
        }

        private void SendKlassenPacket()
        {
            Packet klassen = new Packet(PacketType.Klassenwahl, id);
            SendPacket(klassen);
        }

        private void SendKurswahlPacket()
        {
            Packet kurswahl = new Packet(PacketType.Kurswahl, id);
            SendPacket(kurswahl);
        }
        #endregion

        //Property Set new ID
        public string ID
        {
            set
            {
                id = value;
            }
        }
    }

}
