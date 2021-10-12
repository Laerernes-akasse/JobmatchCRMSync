using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobmatchCRMSync.LAKAServices;
using Oracle.ManagedDataAccess.Client;

namespace JobmatchCRMSync.ModulusData
{
    class Kompetence
    {
        public Int32 IndividID { get; set; }
        public Int32 FagID { get; set; }
        public Boolean LinjefagJN { get; set; }
        public Int32 ErfaringID { get; set; }

        public static List<Kompetence> HentKompetencer()
        {
            List<Kompetence> KompList = new List<Kompetence>();
            LAKAServicesSoapClient client = new LAKAServicesSoapClient();
            OracleConnection con = new OracleConnection(client.GetDataSource("KUNDEDLFAPROD"));
            
            using (con)
            {
                string sql;
                con.Open();
                sql = "select * From v_jobmatch_CRM_SYNC";



            }

                
            return KompList;
        }
    }
}
