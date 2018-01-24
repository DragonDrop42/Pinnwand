using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Runtime.CompilerServices;
//eigene Bibliotheken
using ServerData;
using ClientClassLib;

namespace Client_frm
{
    public partial class frm_client : Form
    {
        Client client;

        frm_login loginFrm;
        frm_register registerFrm;

        public frm_client()
        {
            InitializeComponent();

            TCP_connection.DataManagerCallback PacketCallback = new TCP_connection.DataManagerCallback(RemainOnUIThread);
            TCP_connection.ErrorMessageCallback ErrorCallback = new TCP_connection.ErrorMessageCallback(Fehler_Ausgabe);

            client = new Client(ErrorCallback, PacketCallback); //Verbindung zur Klasse herstellen
        }

        //Verbindung zum Server
        private void cmd_connect_Click(object sender, EventArgs e)
        {
            string ipHost = "10.1.50.28";// PacketHandler.GetIPAddress();
            int port = 4444;

            if (client.Connect(ipHost, port))
            {
                cmd_connect.Enabled = false;
            }
        }

        //login+++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        private void cmd_loginFrm_Click(object sender, EventArgs e)
        {
            //this.Enabled = false;
            registerFrm = new frm_register(client);
            frm_login loginDialog = new frm_login(client, registerFrm);
            loginDialog.Show();
        }
       
        

        #region Packete verarbeiten
        private void RemainOnUIThread(Packet p) //UI Thread Syncronisation
        {
            this.Invoke((MethodInvoker)delegate //Thread Sysncronisation durchführen
            {
                // Running on the UI thread
                try
                {
                    DataManager(p);
                }
                catch(Exception exc)
                {
                    Fehler_Ausgabe(exc.Message);
                }
            });
        }
        //Packet verarbeiten
        public void DataManager(Packet packet)
        {
            switch (packet.packetType)
            {
                //Type Authentication-----------------------------
                case PacketType.Authentication:
                    switch (packet.authState_SERVER)
                    {
                        case AuthenticationState_SERVER_Events.SERVER_Register_ID:

                            Ausgabe("Server is Responding!");
                            client.ID = (packet.auth_keyList["id"].ToString());
                            break;

                        case AuthenticationState_SERVER_Events.SERVER_Klassenwahl_Response:
                            registerFrm.LoadKlassen(packet.lst_TableDictionary);
                            break;

                        case AuthenticationState_SERVER_Events.SERVER_Login_Accepted:
                            Ausgabe("Erfolgreich eingeloggt");
                            if(loginFrm != null)
                            {
                                loginFrm.Close();
                            }
                            this.Enabled = true;
                            break;

                        case AuthenticationState_SERVER_Events.SERVER_Registraition_Accepted:
                            Ausgabe("Erfolgreich registriert");
                        
                            break;
                    }
                    return;
                //------------------------------
                case PacketType.DataTable:
                    switch (packet.tableType_SERVER)
                    {
                        case DataTableType_SERVER_Events.SERVER_Kurswahl_Response:
                            frm_Kurswahl kurswahl = new frm_Kurswahl(client, packet.lst_TableDictionary);
                            kurswahl.Show();
                            break;
                    }
                    break;

                case PacketType.System_Error:
                    Fehler_Ausgabe("Server Error: " + packet.informationString);
                    break;

                
                default:
                    Fehler_Ausgabe("Unbekanntes Packet");
                    break;
            }
        }
        #endregion

        public void Ausgabe(string text)
        {
            lst_chat.Items.Add(text);
        }

        public void Fehler_Ausgabe(string text)
        {

            MessageBox.Show("Client::> " + text);

        }

        private void frm_client_FormClosing(object sender, FormClosingEventArgs e)
        {
            client.Disconnect();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            client.Kurswahl();
        }

        private void frm_client_Load(object sender, EventArgs e)
        {

        }
    }
}
