using PiaLib.PinnwandDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace PiaLib
{
    public class PinnwandDBAdapter
    {
        public schüler Schüler = new schüler();
        public lehrer Lehrer = new lehrer();
        public kurse Kurse = new kurse();
        public klasse Klasse = new klasse();


        public PinnwandDBAdapter()
        {
        }

        public class schüler
        {
            private SchülerTableAdapter sta = new SchülerTableAdapter();
            private KursTableAdapter kta = new KursTableAdapter();
            private Verbund_Kurs_SchülerTableAdapter vta = new Verbund_Kurs_SchülerTableAdapter();
            private KlasseTableAdapter klasse = new KlasseTableAdapter();

            //public int getIDBy(string Search_String, string Field)
            //{
            //    DataTable data = kta.GetData();

            //    foreach (DataRow row in data.Rows)
            //    {
            //        if (row[Field].ToString() == Search_String)
            //        {
            //            return (int)row["L_ID"];
            //        }
            //    }
            //    return 0;
            //}

            public DatenbankArgs getby(string Search_string, string Field)
            {
                try
                {
                    DataTable sds = sta.GetData();
                    DataTable data = sds.Clone();
                    foreach (DataRow row in sds.Rows)
                    {
                        string f = Convert.ToString(row[Field]);
                        if (Search_string == f)
                        {
                            data.ImportRow(row);
                        }
                    }
                    if (data.Rows.Count == 0)
                    {
                        throw new Exception("Schüler nicht gefunden");
                    }
                    else
                    {
                        return new DatenbankArgs(data);
                    }
                }
                catch (Exception ex) { return new DatenbankArgs(ex.Message); };
            }

            public DatenbankArgs add(string S_Name, string S_Vorname, string S_Telefonnummer, string S_Email, string Kl_Name, string S_Passwort)
            {
                try
                {
                    if (getby(S_Email, "S_Email").Success == false)
                    {
                        klasse Klasse = new klasse();
                        sta.Insert(S_Name, S_Vorname, S_Telefonnummer, S_Email, Klasse.getIDByName(Kl_Name) , S_Passwort);
                        DataTable data = sta.GetData();
                        DataTable dataout = data.Clone();
                        dataout.ImportRow(data.Rows[data.Rows.Count - 1]);
                        return new DatenbankArgs(dataout);
                    }
                    else
                    {
                        throw new Exception("Email bereits registriert");
                    }
                }
                catch (Exception ex)
                {
                    return new DatenbankArgs(ex.Message);
                }
            }

            public DatenbankArgs login(string Email, string Passwort)
            {
                try
                {
                    DatenbankArgs user = getby(Email, "S_Email");
                    if (user.Success == false)
                    {
                        throw new Exception(user.Error);
                    }
                    else
                    {
                        string c = (string)user.Data.Rows[0]["S_Passwort"];
                        if (c == Passwort)
                        {
                            return new DatenbankArgs(user.Data);
                        }
                        else
                        {
                            throw new Exception("Passwort Falsch");
                        }
                    }
                }
                catch (Exception ex)
                {
                    return new DatenbankArgs(ex.Message);
                }
            }

            public DatenbankArgs getKlassenNamen()
            {
                try
                {
                    return new DatenbankArgs(klasse.GetKlassenNamen());
                }
                catch (Exception ex)
                {
                    return new DatenbankArgs(ex.Message);
                }
            }

            public DatenbankArgs getKurse(int S_ID)
            {
                try
                {
                    return new DatenbankArgs(vta.GetDataBy1(S_ID));
                }
                catch (Exception ex)
                {
                    return new DatenbankArgs(ex.Message);
                }
            }

            public DatenbankArgs updateKurse(int S_ID, List<string> K_IDS)
            {
                try
                {
                    vta.DeleteSchülerKurse(S_ID);
                    foreach (string K_ID in K_IDS)
                    {
                        vta.Insert(S_ID, Convert.ToInt16(K_ID));
                    }
                    DatenbankArgs data = getKurse(S_ID);
                    if (data.Success)
                    {
                        return new DatenbankArgs(data.Data);
                    }
                    else { return data; }
                }
                catch (Exception ex)
                {
                    return new DatenbankArgs(ex.Message);
                }
            }
        }

        public class lehrer
        {
            LehrerTableAdapter lta = new LehrerTableAdapter();
            KursTableAdapter kta = new KursTableAdapter();

            //public int getIDBy(string Search_String, string Field)
            //{
            //    DataTable data = kta.GetData();

            //    foreach (DataRow row in data.Rows)
            //    {
            //        if (row[Field].ToString() == Search_String)
            //        {
            //            return (int)row["K_ID"];
            //        }
            //    }
            //    return 0;
            //}

            public DatenbankArgs getby(string Search_string, string Field)
            {
                try
                {
                    DataTable lds = lta.GetData();
                    DataTable data = lds.Clone();
                    foreach (DataRow row in lds.Rows)
                    {
                        string f = Convert.ToString(row[Field]);
                        if (Search_string == f)
                        {
                            data.ImportRow(row);
                        }
                    }
                    if (data.Rows.Count == 0)
                    {
                        throw new Exception("Lehrer nicht gefunden");
                    }
                    else
                    {
                        return new DatenbankArgs(data);
                    }
                }
                catch (Exception ex) { return new DatenbankArgs(ex.Message); };

            }

            public DatenbankArgs add(string L_Vorname, string L_Name, string L_Anrede, string L_Email, string L_Passwort, string L_Titel)
            {
                try
                {
                    if (getby(L_Email, "L_Email").Success == false)
                    {
                        lta.Insert(L_Vorname, L_Name, L_Anrede, L_Email, L_Passwort, L_Titel);
                        DataTable data = lta.GetData();
                        DataTable dataout = data.Clone();
                        dataout.ImportRow(data.Rows[data.Rows.Count - 1]);
                        return new DatenbankArgs(dataout);
                    }
                    else
                    {
                        throw new Exception("Email bereits registriert");
                    }
                }
                catch (Exception ex)
                {
                    return new DatenbankArgs(ex.Message);
                }
            }

            public DatenbankArgs add(string L_Vorname, string L_Name, string L_Anrede, string L_Email, string L_Passwort)
            {
                return add(L_Vorname, L_Name, L_Anrede, L_Email, L_Passwort, "");
            }

            public DatenbankArgs add(string L_Name, string L_Anrede, string L_Email, string L_Passwort)
            {
                return add("", L_Name, L_Anrede, L_Email, L_Passwort, "");
            }

            public DatenbankArgs login(string Email, string Passwort)
            {
                try
                {
                    DatenbankArgs user = getby(Email, "L_Email");
                    if (user.Success == false)
                    {
                        throw new Exception(user.Error);
                    }
                    else
                    {
                        if ((string)(user.Data.Rows[0]["L_Passwort"]) == Passwort)
                        {
                            return new DatenbankArgs(user.Data);
                        }
                        else
                        {
                            throw new Exception("Passwort Falsch");
                        }
                    }
                }
                catch (Exception ex)
                {
                    return new DatenbankArgs(ex.Message);
                }
            }

            public DatenbankArgs getKurse(int L_ID)
            {
                try
                {
                    DataTable data = kta.GetDataBy(L_ID);
                    if (data.Rows.Count == 0)
                    {
                        throw new Exception("Keine Kurse vorhanden oder Klasse nicht gefunden!");
                    }
                    return new DatenbankArgs(data);
                }
                catch (Exception ex)
                {
                    return new DatenbankArgs(ex.Message);
                }

            }
        }

        public class kurse
        {
            KursTableAdapter kta = new KursTableAdapter();
            ChatnachrichtenTableAdapter cta = new ChatnachrichtenTableAdapter();
            Verbund_Kurs_SchülerTableAdapter vksta = new Verbund_Kurs_SchülerTableAdapter();

            //public int getIDBy(string Search_String, string Field)
            //{
            //    DataTable data = kta.GetData();

            //    foreach (DataRow row in data.Rows)
            //    {
            //        if (row[Field].ToString() == Search_String)
            //        {
            //            return (int)row["K_ID"];
            //        }
            //    }
            //    return 0;
            //}

            public DatenbankArgs getByKlasse(int Kl_ID)
            {
                try
                {
                    DataTable data = kta.GetDataByKl_ID(Kl_ID);
                    if (data.Rows.Count == 0)
                    {
                        throw new Exception("Keine Kurse vorhanden oder Klasse nicht gefunden!");
                    }
                    return new DatenbankArgs(data);
                }
                catch (Exception ex)
                {
                    return new DatenbankArgs(ex.Message);
                }
            }

            public DatenbankArgs getChat(int K_ID)
            {
                try
                {
                    DataTable data = cta.GetData();
                    return new DatenbankArgs(data);
                }
                catch (Exception e)
                {
                    return new DatenbankArgs(e.Message);
                }
            }

            public DatenbankArgs sendChat(string C_Sendername,String C_Inhalt,int K_ID)
            {
                try
                {
                    cta.Insert(C_Sendername,C_Inhalt,K_ID);
                    return new DatenbankArgs(new DataTable());
                }
                catch (Exception e)
                {
                    return new DatenbankArgs(e.Message);
                }
            }

            public DatenbankArgs getSchüler(int K_ID)
            {
                try
                {
                    DataTable data = vksta.getSchülerInKurs(K_ID);
                    if (data.Rows.Count == 0)
                    {
                        throw new Exception("Keine Schüler vorhanden oder Kurs nicht gefunden!");
                    }
                    return new DatenbankArgs(data);
                }
                catch (Exception ex)
                {
                    return new DatenbankArgs(ex.Message);
                }
            }

            public DatenbankArgs getByLehrer(int L_ID)
            {
                try
                {
                    DataTable data = kta.GetDataBy(L_ID);
                    if (data.Rows.Count == 0)
                    {
                        throw new Exception("Keine Kurse vorhanden.");
                    }
                    return new DatenbankArgs(data);
                }
                catch (Exception ex)
                {
                    return new DatenbankArgs(ex.Message);
                }
            }

            public DatenbankArgs add(string K_Name, int L_ID, int Kl_ID)
            {
                try
                {
                    kta.Insert(K_Name, Kl_ID, L_ID);
                    return new DatenbankArgs(new DataTable());
                }
                catch (Exception ex)
                {
                    return new DatenbankArgs(ex.Message);
                }
            }
        }

        public class klasse
        {
            KlasseTableAdapter klta = new KlasseTableAdapter();



            public int getIDByName(string Name)
            {
                DataTable data = klta.GetData();

                foreach (DataRow row in data.Rows)
                {
                    if (row["Kl_Name"].ToString() == Name)
                    {
                        return Math.Abs((int)row["Kl_ID"]);
                    }
                }
                return 0;
            }

            public DatenbankArgs add() { return new DatenbankArgs(); }
        }
    }
}
