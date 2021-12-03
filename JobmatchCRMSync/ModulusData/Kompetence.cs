using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobmatchCRMSync.LAKAServices;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;

namespace JobmatchCRMSync.ModulusData
{
    class Kompetence
    {
        public Int32 IndividID { get; set; }
        public Int32 FagID { get; set; }
        public Char LinjefagJN { get; set; }
        public Int32 ErfaringID { get; set; }

        public static List<Kompetence> HentFagKompetencer()
        {
            List<Kompetence> KompList = new List<Kompetence>();
            LAKAServicesSoapClient client = new LAKAServicesSoapClient();
            OracleConnection con = new OracleConnection(client.GetDataSource(ConfigurationManager.AppSettings["DataSource"].ToString()));
            using (con)
            {
                string sql;
                con.Open();
                sql = "select individ_id, erfaring, linjefagid, linjefag_jn From v_jobmatch_CRM_SYNC where linjefag_jn ='J' and rownum < 0";
                //sql = "select 146258 as individ_id, 1 as erfaring, 18 as linjefagid, 'N' as linjefag_jn from dual";
                OracleCommand cmd = new OracleCommand(sql, con);
                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Kompetence komp = new Kompetence
                    {
                        IndividID = Convert.ToInt32(dr["individ_id"]),
                        FagID = Convert.ToInt32(dr["linjefagid"]),
                        ErfaringID = Convert.ToInt32(dr["erfaring"]),
                        LinjefagJN = Convert.ToChar(dr["linjefag_jn"])
                    };
                    KompList.Add(komp);
                }
            }
            return KompList;
        }
    }

    class AndenKompetence
    {
        public string Kompetencenavn { get; set; }
        public int Kompetenceid { get; set; }
        public int ErfaringID { get; set; }
        public string Erfaringnavn { get; set; }
        public int IndividID { get; set; }
        public string Uddybning { get; set; }

        public static List<AndenKompetence> HentAndreKompetencer()
        {
            List<AndenKompetence> AndenKompList = new List<AndenKompetence>();
            LAKAServicesSoapClient client = new LAKAServicesSoapClient();
            string sql;
            OracleConnection con = new OracleConnection(client.GetDataSource(ConfigurationManager.AppSettings["DataSource"].ToString()));
            con.Open();
            using (con)
            {
                sql = "select kompetencenavn,kompetenceid, erfaring,erfaringnavn, individ_id, uddybning from kundedlfa.MV_JOBMATCH_MEDLEM_ANDREKOMPETENCER where individ_id=2819 and kompetenceid=1";
                OracleCommand cmd = new OracleCommand(sql, con);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    AndenKompetence AndenKomp = new AndenKompetence
                    {
                        Kompetencenavn = Convert.ToString(dr["kompetencenavn"]),
                        Kompetenceid = Convert.ToInt32(dr["kompetenceid"]),
                        ErfaringID = Convert.ToInt32(dr["erfaring"]),
                        Erfaringnavn = Convert.ToString(dr["erfaringnavn"]),
                        IndividID = Convert.ToChar(dr["individ_id"]),
                        Uddybning = Convert.ToString(dr["uddybning"])
                    };
                    AndenKompList.Add(AndenKomp);
                }
            }
            return AndenKompList;
        }
    }

    class Uddannelse
    {
        public String UddannelseNavn { get; set; }
        public Int32 UddannelsesID { get; set; }
        public String Uddybning { get; set; }
        public Int32 IndividID { get; set; }

        public static List<Uddannelse> HentUddannelser()
        {
            List<Uddannelse> UddannelseList = new List<Uddannelse>();
            LAKAServicesSoapClient client = new LAKAServicesSoapClient();
            string sql;
            OracleConnection con = new OracleConnection(client.GetDataSource(ConfigurationManager.AppSettings["DataSource"].ToString()));
            con.Open();
            using (con)
            {
                sql = "select mu.uddannelsesnavn, mu.uddannelsesid, mu.uddybning, mu.individ_id from MV_JOBMATCH_MEDLEMMER_UDD mu where rownum <= 10";
                OracleCommand cmd = new OracleCommand(sql, con);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Uddannelse Udd = new Uddannelse
                    {
                        UddannelseNavn = Convert.ToString(dr["uddannelsesnavn"]),
                        UddannelsesID = Convert.ToInt32(dr["uddannelsesid"]),
                        Uddybning = Convert.ToString(dr["uddybning"]),
                        IndividID = Convert.ToInt32(dr["individ_id"])
                    };
                    UddannelseList.Add(Udd);
                }
            }
            return UddannelseList;
        }
    }
}
