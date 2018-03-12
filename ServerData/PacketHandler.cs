using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ServerData
{
    public static class PacketHandler
    {
        public static byte[] SerializePacket(Object o)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter serializer = new BinaryFormatter();

            serializer.Serialize(ms, o);

            //XmlSerializer SerializerObj = new XmlSerializer(typeof(ServerData.Packet));
            //SerializerObj.Serialize(ms, (Packet)(o));


            byte[] bytes = ms.ToArray();
            ms.Close();

            return bytes;
        }

        public static Packet DeserializePacket(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            BinaryFormatter serializer = new BinaryFormatter();
            //XmlSerializer SerializerObj = new XmlSerializer(typeof(Packet));

            Packet p = (Packet)serializer.Deserialize(ms);

            ms.Close();
            return p;
        }

        public static string GetIPAddress()
        {
            IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());

            foreach (IPAddress ip in ips)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "127.0.0.1"; //Loopback
        }

        public static ListDictionary ConvertTableToList(DataTable table)
        {
            if (table == null)
            {
                return new ListDictionary();
            }

            ListDictionary list = new ListDictionary();

            for (int i = 0; i < table.Columns.Count; i++)
            {
                List<string> col = new List<string>();
                for (int j = 0; j < table.Rows.Count; j++)
                {
                    col.Add(Convert.ToString(table.Rows[j].ItemArray[i]));
                }

                list.Add(table.Columns[i].ColumnName, col);
            }

            return list;
        }
    }
}
