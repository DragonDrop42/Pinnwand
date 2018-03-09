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
        
        Packet currentPacket;   //Zwichenspeicher

        //Login
        //password Hash
        private string salt = "492";   //random seed
        private Random rand = new Random();

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
                errorCallback("Es konnte keine Verbindung zu "+ ip +" hergestellt werden. \n>> " + exc.Message);
                return false;
            }

            Thread dataINThread = new Thread(DataIN);
            dataINThread.Start(masterS);

            errorCallback("Erfolgreich mit " + ip +" verbunden.");
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
            if (p.senderID == null)
            {
                errorCallback("Dieses Packet besitzt keine gültige ID!");
                return;
            }
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

        private Packet WaitForPacketResponseHandler(Packet send_waitPacket)
        {
            try
            {
                Packet response = null;

                for (int i = 0; i < 2; i++){    //zwei Versuche
                    response = WaitForPacketResponse(send_waitPacket);

                    if(response != null){
                        break;
                    }
                    Thread.Sleep(rand.Next(1,100));
                }

                if(response != null){
                    return response;
                }
                else{
                    return new Packet("Zeitüberschreitung oder Packet nicht gefunden. Versuchen Sie es erneut!");
                }
			}     
            catch(Exception exc)
            {
                //throw new Exception("Fehler: WaitForPacket >>" + exc.Message);
                errorCallback("Fehler: WaitForPacket >>" + exc.Message);
                return null;
            }
        }

        private Packet WaitForPacketResponse(Packet send_waitPacket)
        {
            SendPacket(send_waitPacket); //Packet an Server senden
            //auf Packet warten
            waitHandle.WaitOne(1500);   //1.5sec. Timeout
            waitHandle.Reset();

            if (currentPacket == null)  //Fehler oder Timeout
            {
                //throw new Exception("Received Packet == null");
                return null;
            }

            if (currentPacket.packetType == send_waitPacket.packetType)
            {
                Packet tmp = currentPacket.Copy();
                currentPacket = null;
                return tmp;   //könnte zu problemen führen
            }
            return null;//new Packet("Zeitüberschreitung oder Packet nicht gefunden. Versuchen Sie es erneut!");
        }
        

        //PacketManager++++++++++++++++++++++++++++++++++++++++++++++++++++
        private void PacketManager(Packet packet)
        {
            //kein Client Event
            if (packet.packetType == PacketType.Register_ID)
            {
                ID = packet.Data["id"].ToString();
                //errorCallback("id: " + ID);
                return;
            }
            else if (packet.packetType == PacketType.SystemError)
            {
                errorCallback(packet.MessageString);
                return;
            }
            else if (packet.packetType == PacketType.UpdateAll)
            {
                //Ereigniss auslösen
            }
            //----------------

            //Client Events
            //lst_PacketResponse.Add(packet);
            currentPacket = packet; //Packet Zwischenspeicher

            //event auslösen
            waitHandle.Set();
            //------------
            //errorCallback("Packet " + packet.packetType);
            return;
        }

        //---------------------------------------------------------
        #region Login/Register
        //Register+++++++++++++++++++++++++++++++++++++++++++++++++++++++   
        public Packet Register_Schüler(ListDictionary dataRegister)
        {
            //check
            if ((string)dataRegister["name"] == "" || (string)dataRegister["vname"] == "" || (string)dataRegister["email"] == "" || (string)dataRegister["passswort"] == "" || (string)dataRegister["klasse"] == "")
            {
                throw new Exception("Bitte alle notwendigen Felder ausfüllen!");
            }
            if (check_email(dataRegister["email"].ToString()) && (dataRegister["passwort"] = check_password(dataRegister["passwort"].ToString())) != null)
            {
                //Packet senden
                Packet sendP = new Packet(PacketType.Schüler_Registraition, dataRegister, id);
                //Auf Antwort warten
                return WaitForPacketResponse(sendP); 
            }
            return null;
        }

        public Packet Register_Lehrer(ListDictionary dataRegister)
        {
            //check
            if ((string)dataRegister["name"] == "" || (string)dataRegister["vname"] == "" || (string)dataRegister["email"] == "" || (string)dataRegister["passswort"] == "" || (string)dataRegister["anrede"] == "")
            {
                throw new Exception("Bitte alle notwendigen Felder ausfüllen!");
            }
            if (check_email(dataRegister["email"].ToString()) && (dataRegister["passwort"] = check_password(dataRegister["passwort"].ToString())) != null)
            {
                //Packet senden
                if (dataRegister["titel"] == null) dataRegister["titel"] = "";
                Packet sendP = new Packet(PacketType.Lehrer_Registraition, dataRegister, id);
                //Auf Antwort warten
                return WaitForPacketResponse(sendP); 
            }
            return null;
        }

        //Login+++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public Packet Login(ListDictionary dataLogin, bool schüler)
        {
            //check
            if ((string)dataLogin["email"] == "" || (string)dataLogin["passwort"] == "")
            {
                throw new Exception("Bitte Anmeldedaten eintragen!");
            }
            if (check_email(dataLogin["email"].ToString()) == true && (dataLogin["passwort"] = check_password(dataLogin["passwort"].ToString())) != null)
            {
                //Packet senden
                Packet sendP = (schüler ? new Packet(PacketType.Schüler_Login, dataLogin, id) : new Packet(PacketType.Lehrer_Login, dataLogin, id));
                //auf response warten
                return WaitForPacketResponse(sendP);
            }
            return null;
        }
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
        //spezielle Packete+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //private Packet SendSchülerRegisterPacket(string name, string vname, string phone, string klasse, string email, string passwort)
        //{
        //    ListDictionary data = new ListDictionary{
        //        {"name", name},
        //        {"vname", vname},
        //        {"phone", phone},
        //        {"klasse", klasse},
        //        {"email", email},
        //        {"passwort", passwort}
        //    };
        //    Packet register = new Packet(PacketType.Schüler_Registraition, data, id);
        //    return register;
        //}
        //private Packet SendLehrerRegisterPacket(string name, string vname, string anrede, string email, string passwort, string titel)
        //{
        //    ListDictionary data = new ListDictionary{
        //        {"name", name},
        //        {"vname", vname},
        //        {"anrede", anrede},
        //        {"titel", titel},
        //        {"email", email},
        //        {"passwort", passwort}
        //    };
        //    Packet register = new Packet(PacketType.Lehrer_Registraition, data, id);
        //    return register;
        //}

        //private Packet SendLoginPacket(string email, string passwort, bool schüler)
        //{
        //    ListDictionary data = new ListDictionary
        //    {
        //        {"email", email},
        //        {"passwort", passwort}
        //    };
        //    Packet login = (schüler ? new Packet(PacketType.Schüler_Login, data, id) : new Packet(PacketType.Lehrer_Login, data, id));
        //    return login;
        //}

        public Packet SendKursUpdatePacket(List<string> Kurse)
        {
            ListDictionary data = new ListDictionary
            {
                {"K_ID",Kurse}
            };
            Packet sendP = new Packet(PacketType.KursUpdate,data,id);
            return WaitForPacketResponse(sendP);
        }

        //universelle Packete+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public Packet SendAndWaitForResponse(PacketType packetType)
        {
            Packet sendP = new Packet(packetType, id);
            return WaitForPacketResponse(sendP);
        }
        public Packet SendAndWaitForResponse(PacketType packetType, ListDictionary data)
        {
            return WaitForPacketResponse(new Packet(packetType, data, id));
        }
        #endregion

        //Property Set new ID
        public string ID
        {
            set
            {
                id = value;
            }
            get
            {
                return id;
            }
        }
    }

}
