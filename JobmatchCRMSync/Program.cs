using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobmatchCRMSync.LAKAServices;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System.Configuration;
using Microsoft.Xrm.Sdk.Query;
using JobmatchCRMSync.ModulusData;
using System.Collections.Specialized;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace JobmatchCRMSync
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Forbinder til CRM
            LAKAServicesSoapClient client = new LAKAServicesSoapClient();
            Uri urlcrm = new Uri(client.CRMOrgService(ConfigurationManager.AppSettings["OrgService"].ToString()));
            ClientCredentials credentials = new ClientCredentials();
            credentials.Windows.ClientCredential.UserName = client.CRMOrgUsername();
            credentials.Windows.ClientCredential.Password = client.CRMOrgPassword();
            OrganizationServiceProxy serviceproxy = new OrganizationServiceProxy(urlcrm, null, credentials, null);
            IOrganizationService service;
            service = (IOrganizationService)serviceproxy;
            #endregion

            List<Kompetence> KompList = new List<Kompetence>();
            KompList = ModulusData.Kompetence.HentKompetencer();


            NameValueCollection ErfaringsTyper = new NameValueCollection()
            {
                { "1", "808260000" },
                { "2", "808260001" },
                { "3", "808260002" },
                { "4", "808260003" },
                { "5", "808260004" }
            };

            foreach (var komp in KompList)
            {
                //Vi finder det medlem som kompetencen er vedrørende
                QueryByAttribute q_contact = new QueryByAttribute("contact");
                q_contact.Attributes.Add("ak_modulusid"); //??
                q_contact.Values.Add(komp.IndividID);
                Entity _contact = service.RetrieveMultiple(q_contact).Entities.FirstOrDefault();
                EntityReference ContactRef = new EntityReference("contact", _contact.Id); //TODO - relevant?

                // Find korrekte fag udfra ID
                QueryByAttribute qFag = new QueryByAttribute("lak_fag") //TODO tjek korrekte navne på attributter og entiteter
                {
                    ColumnSet = new ColumnSet(new string[] { "lak_name" })
                };
                qFag.Attributes.Add("lak_modulusid");
                qFag.Values.Add(komp.FagID);
                Entity EntFag = service.RetrieveMultiple(qFag).Entities.FirstOrDefault();
                EntityReference FagRef = new EntityReference("lak_fag", EntFag.Id);

                OptionSetValue osErfaring = new OptionSetValue(Convert.ToInt32(ErfaringsTyper[komp.ErfaringID]));

                //Findes kompetencen i forvejen?
                QueryByAttribute q_readKompetencer = new QueryByAttribute("lak_fagkompetence")
                {
                    ColumnSet = new ColumnSet(new string[] { "lak_fagkompetenceid" })
                };
                q_readKompetencer.Attributes.Add("lak_fag");
                q_readKompetencer.Values.Add(FagRef.Id);
                q_readKompetencer.Attributes.Add("lak_medlem");//????
                q_readKompetencer.Values.Add(ContactRef.Id);

                Entity Kompetence = new Entity("lak_fagkompetence");//TODO
                // Har det pågældende medlem allerede denne kompetence registreret?
                if (service.RetrieveMultiple(q_readKompetencer).Entities.Count() > 0)
                {
                    Console.WriteLine("Kompetencen findes i forvejen.");
                    // Vi laver en update
                    Kompetence.Attributes["lak_fagerfaring"] = osErfaring;//????
                }
                else
                {
                    Console.WriteLine("Vi opretter ny kompetence");
                    //Ny kompetence
                    //Entity Kompetence = new Entity("lak_fagkompetence");//TODO
                    Kompetence.Attributes["lak_medlem"] = ContactRef;
                    Kompetence.Attributes["lak_fag"] = FagRef;//????
                    Kompetence.Attributes["lak_fagerfaring"] = osErfaring;//????
                    Guid tempID = service.Create(Kompetence);
                }
                //Andre kompetencer

            }
        }
    }
}
