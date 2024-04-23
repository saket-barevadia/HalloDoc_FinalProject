using System;
using Business_Logic.Interface;
using Data_Layer.DataContext;
using Data_Layer.CustomModels;
using Data_Layer.DataModels;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Layer.DataContext;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using System.Net;

namespace Business_Logic.LogicRepositories
{
    public class reqSomeOneElse : IsomeoneElse
    {
        private readonly ApplicationDbContext _context;

        public reqSomeOneElse(ApplicationDbContext context)
        {
            _context = context;
        }

        public void someElse(familyFriendReqcm cm, string emaill)
        {
            var result = _context.Users.Where(x => x.Email == cm.Emailclient).FirstOrDefault();

            var data = _context.Aspnetusers.FirstOrDefault(x => x.Email == cm.Emailclient);

            

            if (data==null)
            {
                var aspnetuser = new Aspnetuser()
                {
                    Email = cm.Emailclient,
                    Phonenumber = cm.Phoneclient,
                    Username = cm.FirstNameclient + cm.LastNameclient,
                };

                _context.Aspnetusers.Add(aspnetuser);
                _context.SaveChanges();


                string smtpServer = "outlook.office365.com";
                int port = 587; // Port number for SMTP (e.g., 587 for Gmail)
                string senderEmail = "tatva.dotnet.saketbarevadia@outlook.com";
                string password = "S@ket6898";
                string recipientEmail = cm.Emailclient;
                string subject = "Create Your Account Using Link Given Below";
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








                if (result == null)
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
                    _context.Users.Add(userTb);
                    _context.SaveChanges();

                  
                }

                var netuserroleDATA = new Aspnetuserrole()
                {
                    Userid = aspnetuser.Id,
                    Roleid = 1,
                };
                _context.Aspnetuserroles.Add(netuserroleDATA);
                _context.SaveChanges();

                var user = _context.Users.FirstOrDefault(x => x.Email == emaill);   

                Request _request = new Request();

                _request.Requesttypeid = 1;
                _request.Userid = _context.Users.FirstOrDefault(x=>x.Email==cm.Emailclient).Userid;
                //_request.Status = 2;
                _request.Firstname = user.Firstname;
                _request.Lastname = user.Lastname;
                _request.Phonenumber = user.Mobile;
                _request.Email = user.Email;
                _request.Createddate = DateTime.Now;
                _request.Relationname = cm.Relationname;
                _request.Confirmationnumber = cm.FirstNameclient.Substring(0, 2) + DateTime.Now.ToString().Substring(0, 19).Replace(" ", "");

                _context.Requests.Add(_request);
                _context.SaveChanges();





                Requestclient _requestclient = new Requestclient();
                _requestclient.Requestid = _request.Requestid;
                _requestclient.Firstname = cm.FirstNameclient;
                _requestclient.Lastname = cm.LastNameclient;
                _requestclient.Street = cm.Street;
                _requestclient.City = cm.City;
                _requestclient.State = _context.Regions.FirstOrDefault(x => x.Regionid == Convert.ToInt16(cm.regionId)).Name;
                _requestclient.Zipcode = cm.Zipcode;
                _requestclient.Email = cm.Emailclient;
                _requestclient.Phonenumber = cm.Phoneclient;
                _requestclient.Strmonth = cm.Strmonth.Substring(5, 2);
                _requestclient.Intdate = Convert.ToInt16(cm.Strmonth.Substring(8, 2));
                _requestclient.Intyear = Convert.ToInt16(cm.Strmonth.Substring(0, 4));
                _requestclient.Notes = cm.Symptons;
                _requestclient.Regionid = Convert.ToInt16(cm.regionId);

                _context.Requestclients.Add(_requestclient);
                _context.SaveChanges();



                if (cm.Upload != null)
                {

                    string filename = cm.Upload.FileName;
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Documents", filename);
                    IFormFile file = cm.Upload;

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    // Request? req = _db.Requests.FirstOrDefault(i => i.Email == obj.Email);
                    // int ReqId = req.Requestid;

                    var data3 = new Requestwisefile()
                    {
                        Requestid = _requestclient.Requestid,
                        Filename = filename,
                    };

                    _context.Requestwisefiles.Add(data3);
                    _context.SaveChanges();
                }
            }

            if (data != null)
            {

                if (result != null)
                {
                    Request newReq = new Request()
                    {
                        Requesttypeid = 1,
                        Userid = result.Userid,
                        Firstname = result.Firstname,
                        Lastname = result.Lastname,
                        Phonenumber = result.Mobile,
                        Email = result.Email,
                        Status = 2,
                        Createddate = DateTime.Now,
                        Relationname = cm.Relationname,
                        Confirmationnumber = cm.FirstNameclient.Substring(0, 2) + DateTime.Now.ToString().Substring(0, 19).Replace(" ", ""),

                };

                    _context.Add(newReq);
                    _context.SaveChanges();
                }

                Requestclient requestclient = new Requestclient()
                {
                    Requestid = _context.Requests.FirstOrDefault(x => x.Email == emaill).Requestid,
                    Firstname = cm.FirstNameclient,
                    Lastname = cm.LastNameclient,
                    Phonenumber = cm.Phoneclient,
                    Email = cm.Emailclient,
                    Street = cm.Street,
                    City = cm.City,
                    State = _context.Regions.FirstOrDefault(x => x.Regionid == Convert.ToInt16(cm.regionId)).Name,
                    Zipcode = cm.Zipcode,
                    Strmonth = cm.Strmonth.Substring(5, 2),
                    Intdate = Convert.ToInt16(cm.Strmonth.Substring(8, 2)),
                    Intyear = Convert.ToInt16(cm.Strmonth.Substring(0, 4)),
                    Regionid = Convert.ToInt16(cm.regionId),
            };

                _context.Add(requestclient);
                _context.SaveChanges();


                if (cm.Upload != null)
                {

                    string filename = cm.Upload.FileName;
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Documents", filename);
                    IFormFile file = cm.Upload;

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    // Request? req = _db.Requests.FirstOrDefault(i => i.Email == obj.Email);
                    // int ReqId = req.Requestid;

                    var data3 = new Requestwisefile()
                    {
                        Requestid = requestclient.Requestid,
                        Filename = filename,
                    };

                    _context.Requestwisefiles.Add(data3);
                    _context.SaveChanges();

                }

            }

        }


    }
}
