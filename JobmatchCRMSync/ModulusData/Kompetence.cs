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

        public static List<Kompetence> HentKompetencer()
        {
            List<Kompetence> KompList = new List<Kompetence>();
            LAKAServicesSoapClient client = new LAKAServicesSoapClient();
            OracleConnection con = new OracleConnection(client.GetDataSource(ConfigurationManager.AppSettings["DataSource"].ToString()));
            using (con)
            {
                string sql;
                con.Open();
                sql = "select individ_id, erfaring, linjefagid, linjefag_jn From v_jobmatch_CRM_SYNC where rownum<1000";
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
}
