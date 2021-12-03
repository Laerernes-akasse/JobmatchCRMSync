using System;
using System.Collections.Generic;
using System.Linq;
using JobmatchCRMSync.LAKAServices;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System.Configuration;
using Microsoft.Xrm.Sdk.Query;
using JobmatchCRMSync.ModulusData;
using System.Collections.Specialized;

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
            
            List<Kompetence> FagKompList = new List<Kompetence>();
            List<AndenKompetence> AndreKompList = new List<AndenKompetence>();
            List<Uddannelse> UddannelseList = new List<Uddannelse>();
            FagKompList = ModulusData.Kompetence.HentFagKompetencer();
            AndreKompList = AndenKompetence.HentAndreKompetencer();
            UddannelseList = Uddannelse.HentUddannelser();

            NameValueCollection ErfaringsTyper = new NameValueCollection()
            {
                { "1", "808260000" },
                { "2", "808260001" },
                { "3", "808260002" },
                { "4", "808260003" },
                { "5", "808260004" }
            };

            //Fagkompetencer
            foreach (var FagKomp in FagKompList)
            {
                //Vi finder det medlem som kompetencen er vedrørende
                QueryByAttribute q_contact = new QueryByAttribute("contact");
                q_contact.Attributes.Add("ak_modulusid"); //??
                q_contact.Values.Add(FagKomp.IndividID);
                Entity _contact = service.RetrieveMultiple(q_contact).Entities.FirstOrDefault();
                EntityReference ContactRef = new EntityReference("contact", _contact.Id); //TODO - relevant?

                // Find korrekte fag udfra ID
                QueryByAttribute qFag = new QueryByAttribute("lak_fag") //TODO tjek korrekte navne på attributter og entiteter
                {
                    ColumnSet = new ColumnSet(new string[] { "lak_name" })
                };
                qFag.Attributes.Add("lak_modulusid");
                qFag.Values.Add(FagKomp.FagID);
                Entity EntFag = service.RetrieveMultiple(qFag).Entities.FirstOrDefault();
                EntityReference FagRef = new EntityReference("lak_fag", EntFag.Id);

                OptionSetValue osErfaring = new OptionSetValue(Convert.ToInt32(ErfaringsTyper[FagKomp.ErfaringID]));

                //Findes kompetencen i forvejen?
                QueryByAttribute q_readKompetencer = new QueryByAttribute("lak_fagkompetence")
                {
                    ColumnSet = new ColumnSet(new string[] { "lak_fagkompetenceid" })
                };
                q_readKompetencer.Attributes.Add("lak_fag");
                q_readKompetencer.Values.Add(FagRef.Id);
                q_readKompetencer.Attributes.Add("lak_medlem");
                q_readKompetencer.Values.Add(ContactRef.Id);

                Entity Kompetence = new Entity("lak_fagkompetence");
                // Har det pågældende medlem allerede denne kompetence registreret?
                if (service.RetrieveMultiple(q_readKompetencer).Entities.Count() > 0)
                {
                    Console.WriteLine("Kompetencen findes i forvejen.");
                    // Vi laver en update
                    Kompetence.Attributes["lak_fagerfaring"] = osErfaring;
                    // Hvad skal ellers opdateres?
                }
                else
                {
                    Console.WriteLine("Vi opretter ny kompetence");
                    //Ny kompetence
                    //Entity Kompetence = new Entity("lak_fagkompetence");//TODO
                    Kompetence.Attributes["lak_medlem"] = ContactRef;
                    Kompetence.Attributes["lak_fag"] = FagRef;
                    Kompetence.Attributes["lak_fagerfaring"] = osErfaring;
                    Kompetence.Attributes["linje_fag"] ="Ja";
                    Guid tempID = service.Create(Kompetence);
                }
                //Andre kompetencer
            }

            foreach (var AndreKomp in AndreKompList)
            {
                //Vi finder det medlem som kompetencen er vedrørende
                QueryByAttribute qContactAnden = new QueryByAttribute("contact");
                qContactAnden.Attributes.Add("ak_modulusid"); //??
                qContactAnden.Values.Add(AndreKomp.IndividID);
                Entity ContactAnden = service.RetrieveMultiple(qContactAnden).Entities.FirstOrDefault();
                EntityReference ContactAndenRef = new EntityReference("contact", ContactAnden.Id); //TODO - relevant?

                // Find korrekte fag udfra ID
                QueryByAttribute qAndenKompetence = new QueryByAttribute("lak_andenkompetence") //TODO tjek korrekte navne på attributter og entiteter
                {
                    ColumnSet = new ColumnSet(new string[] { "lak_name" })
                };
                qAndenKompetence.Attributes.Add("lak_modulusid");
                qAndenKompetence.Values.Add(AndreKomp.Kompetenceid);
                Entity EntAndenKompetence = service.RetrieveMultiple(qAndenKompetence).Entities.FirstOrDefault();
                EntityReference AndenKompetenceRef = new EntityReference("lak_andenkompetence", EntAndenKompetence.Id);

                //OptionSetValue osErfaring = new OptionSetValue(Convert.ToInt32(ErfaringsTyper[FagKomp.ErfaringID]));
            }

            //Uddannelse
            foreach (Uddannelse Uddannelser in UddannelseList)
            {
                //Vi finder det medlem som kompetencen er vedrørende
                QueryByAttribute qContactUdd = new QueryByAttribute("contact");
                qContactUdd.Attributes.Add("ak_modulusid"); //??
                qContactUdd.Values.Add(Uddannelser.IndividID);
                Entity ContactUdd = service.RetrieveMultiple(qContactUdd).Entities.FirstOrDefault();
                EntityReference ContactUddRef = new EntityReference("contact", ContactUdd.Id); //TODO - relevant?
                
                // Find uddannelsen udfra ID
                //QueryByAttribute qUddannelsesType = new QueryByAttribute("lak_uddannelses_type") //TODO tjek korrekte navne på attributter og entiteter
                //{
                //    ColumnSet = new ColumnSet(new string[] { "lak_name" })
                //};
                //qUddannelsesType.Attributes.Add("lak_modulusid");
                //qUddannelsesType.Values.Add(Uddannelser.UddannelsesID);
                //Entity EntUddannelsesType= service.RetrieveMultiple(qUddannelsesType).Entities.FirstOrDefault();

                // Har medlemmet denne uddannelse registreret i forvejen?
                QueryByAttribute qUddannelse = new QueryByAttribute("lak_uddannelse") //TODO tjek korrekte navne på attributter og entiteter
                {
                    ColumnSet = new ColumnSet(new string[] { "lak_name" })
                };
                qUddannelse.Attributes.Add("lak_modulusid");
                qUddannelse.Values.Add(Uddannelser.UddannelsesID);
                qUddannelse.Attributes.Add("lak_modulusid"); // Medlem
                qUddannelse.Values.Add(ContactUddRef.);
                Entity EntUddannelse = service.RetrieveMultiple(qUddannelse).Entities.FirstOrDefault();
                if (service.RetrieveMultiple(qUddannelse).Entities.Count() > 0)
                {
                    // Uddannelsen findes i forvejen
                    EntUddannelse.Attributes["Uddybning"] = Uddannelser.Uddybning;
                    //Entity EntUddannelse = service.RetrieveMultiple(qUddannelse).Entities.FirstOrDefault();
                }
                else
                {
                    Entity EntUdd = new Entity("lak_uddannelse");
                //    EntUdd.Attributes["lak_uddannelse"] = EntUddannelsesType;
                    EntUdd.Attributes["medlem"] = ContactUddRef;
                    EntUdd.Attributes["Uddybning"] = Uddannelser.Uddybning;
                }
            }
        }
    }
}
