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

namespace ClientClassLib
{
    public class Client
    {
        static Socket masterS;
        static string id;

        TCP_connection.DataManagerCallback packetCallback;
        TCP_connection.ErrorMessageCallback errorCallback;

        public Client(TCP_connection.ErrorMessageCallback errorDel, TCP_connection.DataManagerCallback packetDel)
        {
            packetCallback = packetDel;
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

        #region Login/Register
        //Register+++++++++++++++++++++++++++++++++++++++++++++++++++++++   
        public void Register_User(string name, string vname, string phone, string klasse, string email, string passwort)
        {
            //check
            if (name == "" || vname == "" || email == "" || passwort == "" || klasse == "")
            {
                throw new Exception("Bitte alle notwendigen Felder ausfüllen!");
            }
            if (check_email(email) == true && (passwort = check_password(passwort)) != null)
            {
                SendRegisterPacket(name, vname, phone, klasse, email, passwort);
            }
        }

        //Login+++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public void Login(string email, string passwort)
        {
            //check
            if (email == "" || passwort == "")
            {
                throw new Exception("Bitte Anmeldedaten eintragen!");
            }
            if (check_email(email) == true && (passwort = check_password(passwort)) != null)
            {
                SendLoginPacket(email, passwort);
            }
        }
        //Get Klassen
        public void GetKlassen(){
            SendKlassenPacket();
        }
        //Kurswahl+++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public void Kurswahl()
        {
            SendKurswahlPacket();
        }
        //-----------------------------------------------------------
        private bool check_email(string email)
        {
            //check
            if (email.Contains('@') == false)
            {
                //errorCallback("Die eingetragene email ist fehlerhaft!");
                throw new Exception("Die eingetragene email ist fehlerhaft!");
                return false;
            }

            return true;
        }
        private string check_password(string pass)
        {
            if (pass.Length < 5)
            {
                //errorCallback("Passwort ist zu kurz (>5)");
                throw new Exception("Passwort ist zu kurz (>5)");
                return null;
            }
            //passwort verschlüsslung
            string salt = "492";
            System.Security.Cryptography.SHA1 sha = System.Security.Cryptography.SHA1.Create();
            byte[] preHash = System.Text.Encoding.UTF32.GetBytes(pass + salt);
            byte[] hash = sha.ComputeHash(preHash);
            string password = System.Convert.ToBase64String(hash, 0, hash.Length);  //immer 15 Stellen lang
            //
            //errorCallback(password);
            return password;
        }
        //----------------------------------------------------------------------------
        #endregion
        //=>
        #region Communication (Packets)

        private void SendRegisterPacket(string name, string vname, string phone, string klasse, string email, string passwort)
        {
            ListDictionary data = new ListDictionary(){
                {"name", name},
                {"vname", vname},
                {"phone", phone},
                {"klasse", klasse},
                {"email", email},
                {"passwort", passwort}
            };
            Packet register = new Packet(AuthenticationState_CLIENT_Events.USER_Registration_Request, id, data);
            SendPacket(register);
        }

        private void SendLoginPacket(string email, string passwort)
        {
            ListDictionary data = new ListDictionary()
            {
                {"email", email},
                {"passwort", passwort}
            };
            Packet login = new Packet(AuthenticationState_CLIENT_Events.USER_Login_Request, id, data);
            SendPacket(login);
        }

        private void SendKlassenPacket()
        {
            Packet klassen = new Packet(AuthenticationState_CLIENT_Events.USER_Klassenwahl_Request, id, null);
            SendPacket(klassen);
        }

        private void SendKurswahlPacket()
        {
            Packet kurswahl = new Packet(DataTableType_CLIENT_Events.USER_Kurswahl_Request, id, new List<string>());
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
