using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace PiaLib
{
    public class DatenbankArgs
    {
        public bool Success { get; internal set; }
        public string Error { get; internal set; }
        public DataTable Data { get; internal set; }

        public DatenbankArgs(DataTable Data)
        {
            this.Success = true;
            this.Data = Data;
            this.Error = "";
        }
        public DatenbankArgs(string Error)
        {
            this.Success = false;
            this.Error = Error;
            this.Data = new DataTable();
        }
        public DatenbankArgs()
        {
            this.Success = true;
            this.Error = "";
            this.Data = new DataTable();
        }

        public string DataDebug
        {
            get
            {
                string debug = Success.ToString() + " " + Error + " " + Data.ToString() + "\n";
                List<int> lenghts = new List<int>();

                foreach (DataColumn col in Data.Columns)
                {
                    lenghts.Add(col.ColumnName.Length);
                }

                foreach (DataRow row in Data.Rows)
                {
                    for (int i = 0; i < row.Table.Columns.Count; i++)
                    {
                        if (row[i].ToString().Length > lenghts[i])
                        {
                            lenghts[i] = row[i].ToString().Length;
                        }
                    }
                }
                int ig = 0;
                foreach (DataColumn column in Data.Columns)
                {
                    debug += "|" + column.ColumnName + new String(Convert.ToChar(" "), lenghts[ig] - column.ColumnName.Length);
                    ig++;
                }
                debug += "\n";
                foreach (DataRow row in Data.Rows)
                {
                    for (int i = 0; i < row.Table.Columns.Count; i++)
                    {
                        debug +="|"+ row[i] + new String(Convert.ToChar(" "),lenghts[i]-row[i].ToString().Length);
                    }
                    debug += "\n";
                }
                return debug;
            }
        }
    }
}
