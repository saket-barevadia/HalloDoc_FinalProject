using System;
using Data_Layer.DataContext;
using Data_Layer.DataModels;
using Business_Logic.Interface;
using Data_Layer.CustomModels;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;

namespace Business_Logic.LogicRepositories
{
    public class patientDashRepo : IpatientDashboard
    {

        private readonly ApplicationDbContext _context;

        public patientDashRepo(ApplicationDbContext context)
        {
            _context = context;
        }


        public ReviewAgreement review(int id)
        {

            var request=_context.Requestclients.FirstOrDefault(x=>x.Requestid == id);   

            ReviewAgreement agreement = new ReviewAgreement() { 
              Requestid = id,
              Firstname = request.Firstname,
              Lastname = request.Lastname,
              fullName = request.Firstname + " " + request.Lastname,
            };

            return agreement;

        }

        public bool iAgree(ReviewAgreement cm)
        {
            var request=_context.Requests.FirstOrDefault(x=>x.Requestid == cm.Requestid);

            

            if (request != null)
            {
                if (request.Status == 2)
                {
                    request.Status = 4;

                    //_context.Requests.Update(request);  
                    _context.SaveChanges();

                    Requeststatuslog requestStatuslog = new Requeststatuslog();

                    requestStatuslog.Requestid = cm.Requestid;
                    requestStatuslog.Createddate = DateTime.Now;
                    requestStatuslog.Status = 4;

                    _context.Requeststatuslogs.Add(requestStatuslog);
                    _context.SaveChanges();

                    return true;
                }

                return false;
               
            }

            return false;

            
                
            

        }


        public bool cancelAgree(ReviewAgreement cm)
        {
            var request = _context.Requests.FirstOrDefault(x => x.Requestid == cm.Requestid);


            if (request != null)
            {

                if (request.Status == 2)
                {
                    request.Status = 7;

                    _context.Requests.Update(request);
                    _context.SaveChanges();


                    Requeststatuslog requestStatuslog = new Requeststatuslog();

                    requestStatuslog.Requestid = cm.Requestid;
                    requestStatuslog.Createddate = DateTime.Now;
                    requestStatuslog.Status = 3;
                    requestStatuslog.Notes = cm.Notes;

                    _context.Requeststatuslogs.Add(requestStatuslog);
                    _context.SaveChanges();

                    return true;
                }
                return false;


            }
            return false;
        }
               
    


    public patientDashboard createAccount(int id, int flag)
        {
            var aspnetUser = _context.Aspnetusers.FirstOrDefault(x => x.Id == id);

            if (aspnetUser != null)
            {

                createAccount account = new createAccount()
                {
                    Email = aspnetUser.Email,
                    Id = aspnetUser.Id,
                    flag = flag,
                };

                patientDashboard patientDashboard = new patientDashboard() { 
                  create=account,
                };


                return patientDashboard;

            }

            else
            {
                return null;
            }
        }


        //POST
        public void account(createAccount cm)
        {
            var aspnetuser=_context.Aspnetusers.FirstOrDefault(x=>x.Id == cm.Id);

            if (aspnetuser != null)
            {

                aspnetuser.Email = cm.Email;
                aspnetuser.Passwordhash = cm.Passwordhash;
                aspnetuser.Modifieddate=DateTime.Now;
                



                //Aspnetuser aspnetuser1 = new Aspnetuser()
                //{
                //    Email = cm.Email,
                //    Passwordhash = cm.Passwordhash,
                //    Modifieddate=DateTime.Now,
                //    Createddate=aspnetuser.Createddate
                //};

                _context.Aspnetusers.Update(aspnetuser);
                _context.SaveChanges();
                
            }

            if(aspnetuser==null)
            {
                Aspnetuser aspnetuser1 = new Aspnetuser()
                {
                    Passwordhash = cm.Passwordhash,
                    Email= cm.Email,
                    Createddate=DateTime.Now,
                };

                _context.Aspnetusers.Add(aspnetuser1);

                _context.SaveChanges();
            }
        }





        public void resetPassword(patientLogincm cm)
        {
            var aspnetuser = _context.Aspnetusers.FirstOrDefault(x => x.Email == cm.Email);

            if (aspnetuser != null)
            {

                string smtpServer = "outlook.office365.com";
                int port = 587; // Port number for SMTP (e.g., 587 for Gmail)
                string senderEmail = "tatva.dotnet.saketbarevadia@outlook.com";
                string password = "S@ket6898";
                string recipientEmail = cm.Email;
                string subject = "Click the link below to Reset Your Password";
                string body = "http://localhost:5001/Home/createAccount?aspnetId=" + aspnetuser.Id +"&flag="+1;


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
        }
    }
}
