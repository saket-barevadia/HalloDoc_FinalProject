using Business_Logic.Interface;
using Data_Layer.CustomModels;
using Data_Layer.DataContext;
using Data_Layer.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.LogicRepositories
{
    public class BusinessReqRepo : IBusinessReq
    {
       private readonly ApplicationDbContext _db;

        public BusinessReqRepo (ApplicationDbContext db)
        {
            _db = db;
        }

        public void businessReq(businessReqcm cm)
        {
            

            var data = _db.Aspnetusers.FirstOrDefault(x => x.Email == cm.Emailclient);

            var user=_db.Users.FirstOrDefault(x=>x.Email== cm.Emailclient);


            if (data == null)
            {
                var aspnetuser = new Aspnetuser()
                {
                    Email = cm.Emailclient,
                    Phonenumber = cm.Phoneclient,
                    Username =cm.FirstNameclient + cm.LastNameclient,
                };

                _db.Aspnetusers.Add(aspnetuser);
                _db.SaveChanges();


                string smtpServer = "outlook.office365.com";
                int port = 587; // Port number for SMTP (e.g., 587 for Gmail)
                string senderEmail = "tatva.dotnet.saketbarevadia@outlook.com";
                string password = "S@ket6898";
                string recipientEmail = cm.Emailclient;
                string subject = "Click the link given below to create your account";
                string body = "http://localhost:5001/Home/createAccount?aspnetId=" + aspnetuser.Id;


                using (SmtpClient client = new SmtpClient(smtpServer, port))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(senderEmail, password);

                    using (MailMessage message = new MailMessage(senderEmail, recipientEmail, subject, body))
                    {

                        client.Send(message);
                    }
                }








                if (user == null)
                {
                    var userTb = new User();
                    userTb.Aspnetuserid = aspnetuser.Id;
                    userTb.Createdby = aspnetuser.Id;
                    userTb.Createddate = DateTime.Now;
                    userTb.Firstname = cm.FirstNameclient;
                    userTb.Lastname = cm.LastNameclient;
                    userTb.Email = cm.Emailclient;
                    userTb.Mobile = cm.Phoneclient;
                    userTb.Street = cm.Street;
                    userTb.City = cm.City;
                    userTb.State = cm.State;
                    userTb.Zipcode = cm.Zipcode;
                    userTb.Strmonth = cm.Strmonth.Substring(5, 2);
                    userTb.Intdate = Convert.ToInt16(cm.Strmonth.Substring(8, 2));
                    userTb.Intyear = Convert.ToInt16(cm.Strmonth.Substring(0, 4));
                    _db.Users.Add(userTb);
                    _db.SaveChanges();

                   
                }

                var netuserroleDATA = new Aspnetuserrole()
                {
                    Userid = aspnetuser.Id,
                    Roleid = 1,
                };
                _db.Aspnetuserroles.Add(netuserroleDATA);
                _db.SaveChanges();

                Request _request = new Request();

                _request.Requesttypeid = 4;
                _request.Userid = _db.Users.FirstOrDefault(x => x.Email == cm.Emailclient).Userid;              
                _request.Firstname = cm.firstnamebusiness;
                _request.Lastname = cm.lastnamebusiness;
                _request.Phonenumber = cm.mobilebusiness;
                _request.Email = cm.emailbusiness;
                _request.Createddate = DateTime.Now;
                
                _request.Confirmationnumber = cm.FirstNameclient.Substring(0, 2) + DateTime.Now.ToString().Substring(0, 19).Replace(" ", "");

                _db.Requests.Add(_request);
                _db.SaveChanges();




                Requestclient _requestclient = new Requestclient();

                _requestclient.Requestid = _request.Requestid;
                _requestclient.Firstname = cm.FirstNameclient;
                _requestclient.Lastname = cm.LastNameclient;
                _requestclient.Street = cm.Street;
                _requestclient.City = cm.City;
                _requestclient.State = _db.Regions.FirstOrDefault(x => x.Regionid == Convert.ToInt16(cm.regionId)).Name;
                _requestclient.Zipcode = cm.Zipcode;
                _requestclient.Email = cm.Emailclient;
                _requestclient.Phonenumber = cm.Phoneclient;
                _requestclient.Strmonth = cm.Strmonth.Substring(5, 2);
                _requestclient.Intdate = Convert.ToInt16(cm.Strmonth.Substring(8, 2));
                _requestclient.Intyear = Convert.ToInt16(cm.Strmonth.Substring(0, 4));
                _requestclient.Notes = cm.Symptons;
                _requestclient.Regionid = Convert.ToInt16(cm.regionId);

                _db.Requestclients.Add(_requestclient);
                _db.SaveChanges();



                Business _business = new Business();
                _business.Name = cm.firstnamebusiness + " " + cm.lastnamebusiness;
                _business.City = cm.City;
                _business.Regionid = cm.regionId;
                _business.Zipcode = cm.Zipcode;
                _business.Createddate = DateTime.Now;
                _db.Businesses.Add(_business);
                _db.SaveChanges();

            }


            if (data != null)
            {

                if (user == null)
                {
                    var userTb = new User();
                    userTb.Aspnetuserid = data.Id;
                    userTb.Createdby = data.Id;
                    userTb.Createddate = DateTime.Now;
                    userTb.Firstname = cm.FirstNameclient;
                    userTb.Lastname = cm.LastNameclient;
                    userTb.Email = cm.Emailclient;
                    userTb.Mobile = cm.Phoneclient;
                    userTb.Street = cm.Street;
                    userTb.City = cm.City;
                    userTb.State = cm.State;
                    userTb.Zipcode = cm.Zipcode;
                    userTb.Strmonth = cm.Strmonth.Substring(5, 2);
                    userTb.Intdate = Convert.ToInt16(cm.Strmonth.Substring(8, 2));
                    userTb.Intyear = Convert.ToInt16(cm.Strmonth.Substring(0, 4));
                    _db.Users.Add(userTb);
                    _db.SaveChanges();
                }

                Request _request = new Request();
               
                _request.Requesttypeid = 4;
                _request.Userid = _db.Users.FirstOrDefault(x => x.Email == cm.Emailclient).Userid;
                _request.Firstname = cm.firstnamebusiness;
                _request.Lastname = cm.lastnamebusiness;
                _request.Phonenumber = cm.mobilebusiness;
                _request.Email = cm.emailbusiness;
                _request.Createddate = DateTime.Now;
                _request.Confirmationnumber = cm.FirstNameclient.Substring(0, 2) + DateTime.Now.ToString().Substring(0, 19).Replace(" ", "");

                _db.Requests.Add(_request);
                _db.SaveChanges();




                Requestclient _requestclient = new Requestclient();

                _requestclient.Requestid = _request.Requestid;
                _requestclient.Firstname = cm.FirstNameclient;
                _requestclient.Lastname = cm.LastNameclient;
                _requestclient.Street = cm.Street;
                _requestclient.City = cm.City;
                _requestclient.State = _db.Regions.FirstOrDefault(x => x.Regionid == Convert.ToInt16(cm.regionId)).Name;
                _requestclient.Zipcode = cm.Zipcode;
                _requestclient.Email = cm.Emailclient;
                _requestclient.Phonenumber = cm.Phoneclient;
                _requestclient.Strmonth = cm.Strmonth.Substring(5, 2);
                _requestclient.Intdate = Convert.ToInt16(cm.Strmonth.Substring(8, 2));
                _requestclient.Intyear = Convert.ToInt16(cm.Strmonth.Substring(0, 4));
                _requestclient.Notes = cm.Symptons;
                _requestclient.Regionid = Convert.ToInt16(cm.regionId);

                _db.Requestclients.Add(_requestclient);
                _db.SaveChanges();



                Business _business = new Business();
                _business.Name = cm.firstnamebusiness + " " + cm.lastnamebusiness;
                _business.City = cm.City;
                _business.Regionid = cm.regionId;
                _business.Zipcode = cm.Zipcode;
                _business.Createddate = DateTime.Now;
                _db.Businesses.Add(_business);
                _db.SaveChanges();


               


            }

        }
    }
        }
 