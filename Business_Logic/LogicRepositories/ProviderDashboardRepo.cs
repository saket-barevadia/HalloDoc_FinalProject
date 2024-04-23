using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Layer.DataContext;
using Data_Layer.DataModels;
using Data_Layer.CustomModels;
using Business_Logic.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using System.Net;
using static Data_Layer.CustomModels.ProviderDashboardcm;

namespace Business_Logic.LogicRepositories
{
    public class ProviderDashboardRepo : IProviderDashboard
    {

        private readonly ApplicationDbContext _context;

        public ProviderDashboardRepo(ApplicationDbContext context)
        {
            _context= context;
        }

        public ProviderDashboardcm GetPatientDetails(int status, int reqTypeId, int Phyid, int flag)
        {
            var query = from r in _context.Requests
                        join rw in _context.Requestclients
                        on r.Requestid equals rw.Requestid
                        where r.Isdeleted == null && r.Physicianid==Phyid

                        select (new PatientData
                        {
                            FirstnameRequestor = r.Firstname,
                            Firstname = rw.Firstname,
                            Createddate = r.Createddate,
                            Phonenumber = r.Phonenumber,
                            Street = rw.Street,
                            Address = rw.City + " " + rw.State,
                            Strmonth = rw.Strmonth,
                            Intdate = rw.Intdate,
                            Intyear = rw.Intyear,
                            Requesttypeid = r.Requesttypeid,
                            Email = rw.Email,
                            requestClientPhonenumber = rw.Phonenumber,
                            Lastname = rw.Lastname,
                            LastnameRequestor = r.Lastname,
                            Notes = rw.Notes,
                            Status = r.Status,
                            Requestid = rw.Requestid,
                            Regionid = rw.Regionid,
                            physician = _context.Physicians.FirstOrDefault(x => x.Physicianid == r.Physicianid).Firstname,
                            Calltype=r.Calltype,
                            IsFinalized=_context.EncounterForms.FirstOrDefault(x=>x.Requestid==r.Requestid).IsFinalized,
                        });

            if (flag!=0)
            {
                query = query.Where(r => r.Status == 4 || r.Status==5);
            }

            if(flag==0)
            {
                if (status > 0)
                {
                    query = query.Where(r => r.Status == status);
                }
            }

           

            if (reqTypeId > 0)
            {
                query = query.Where(r => r.Requesttypeid == reqTypeId);
            }

            

          


            var patientData =query.ToList();

            ProviderDashboardcm patientDetails = new ProviderDashboardcm() { 
             patientDatas=patientData,
             RequestStatus= status,
            };


            return patientDetails;


        }



        public void PostTransferRequest(ProviderDashboardcm cm, int PhyId)
        {

            var transferdata = _context.Requests.FirstOrDefault(i => i.Requestid == cm.Requestid);


            var transferstatuslog = new Requeststatuslog
            {
                Requestid = Convert.ToInt32(cm.Requestid),
                Notes = cm.AdminTransferRequestModalBoxx.AssignAdditionalNotes,
                Status = 1,
                Transtoadmin = new BitArray(1, true),
                Physicianid=PhyId,
                Createddate = DateTime.Now,
            };

            transferdata.Physicianid = null;
            transferdata.Status = 1;

            _context.Requeststatuslogs.Add(transferstatuslog);
            _context.SaveChanges();
        }



        public void AcceptRequest(int Requestid, int physicianid)
        {
            var request = _context.Requests.FirstOrDefault(i => i.Requestid == Requestid);

            if (request != null)
            {
                request.Status = 2;
                request.Modifieddate = DateTime.Now;

                _context.SaveChanges();

                var statuslog = new Requeststatuslog
                {
                    Requestid = request.Requestid,
                    Status = 2,
                    Physicianid = physicianid,
                    Createddate = DateTime.Now,
                    Notes="Case accepted by Provider"
                };

                _context.Requeststatuslogs.Add(statuslog);
                _context.SaveChanges();
            }
        }


        public void PostEncounterCare(Encounter encounterFormData, int Requestid)
        {
            if (Requestid != null && encounterFormData.Option != null)
            {
                var request = _context.Requests.FirstOrDefault(i => i.Requestid == Requestid);

                if (encounterFormData.Option == 1)
                {
                    request.Calltype = encounterFormData.Option;
                    request.Status = 5;
                    _context.SaveChanges();


                    var statuslog = new Requeststatuslog
                    {
                        Requestid = Requestid,
                        Status = 5,
                        Createddate = DateTime.Now,
                    };

                    _context.Add(statuslog);
                    _context.SaveChanges();
                }
                else
                {
                    request.Calltype = encounterFormData.Option;
                    request.Status = 6;
                    _context.SaveChanges();


                    var statuslog = new Requeststatuslog
                    {
                        Requestid = Requestid,
                        Status = 6,
                        Createddate = DateTime.Now,
                        Notes="Request is Concluded"
                    };

                    _context.Add(statuslog);
                    _context.SaveChanges();
                }

            }
        }


        public void concludeReq(int requestId)
        {
            var request=_context.Requests.FirstOrDefault(x=>x.Requestid== requestId);

            if (request != null)
            {
                request.Status = 6;
                _context.SaveChanges();

                var statuslog = new Requeststatuslog
                {
                    Requestid = requestId,
                    Status = 6,
                    Createddate = DateTime.Now,
                    Notes = "Request is Concluded"
                };

                _context.Add(statuslog);
                _context.SaveChanges();
            }


        }


        public void finalizeForm(int requestId)
        {
           
            var encounterForm=_context.EncounterForms.FirstOrDefault(x=>x.Requestid== requestId);

            if (encounterForm != null)
            {
                encounterForm.IsFinalized = new BitArray(1, true);

                _context.SaveChanges();
            }
            
        }



        public void addNote(ViewUploads cm)
        {
            var request=_context.Requests.FirstOrDefault(x=>x.Requestid== cm.Requestid);    

            if(request != null)
            {
                request.Status = 8;
                request.Completedbyphysician=new BitArray(1, true);

                _context.SaveChanges();


                var statuslog = new Requeststatuslog
                {
                    Requestid = cm.Requestid,
                    Status = 8,
                    Createddate = DateTime.Now,
                    Notes=cm.providerNote,
                };

                _context.Add(statuslog);
                _context.SaveChanges();
            }
        }



        public void reqToAdmin(AdminDashboardcm cm, string email)
        {
            var emaill = _context.Admins.ToList();

            var provider=_context.Physicians.FirstOrDefault(x=>x.Email== email);

            var pass = _context.Aspnetusers.FirstOrDefault(x => x.Id == provider.Aspnetuserid).Passwordhash;

            foreach (var item in emaill)
            {
                try
                {
                    string smtpServer = "outlook.office365.com";
                    int port = 587; // Port number for SMTP (e.g., 587 for Gmail)
                    string senderEmail = "tatva.dotnet.saketbarevadia@outlook.com";
                    string password = "S@ket6898";
                    string recipientEmail = item.Email;
                    string subject = "Request to edit profile";
                    string body = cm.Notes;


                    using (SmtpClient client = new SmtpClient(smtpServer, port))
                    {
                        client.EnableSsl = true;
                        client.Credentials = new NetworkCredential(senderEmail, password);

                        using (MailMessage message = new MailMessage(senderEmail, recipientEmail, subject, body))
                        {

                            client.Send(message);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }


            }

            
        }
    }
}
