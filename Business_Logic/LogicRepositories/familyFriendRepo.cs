using Data_Layer.CustomModels;
using Business_Logic.Interface;
using Data_Layer.DataContext;
using Data_Layer.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using System.Net;

namespace Business_Logic.LogicRepositories
{
    public class familyFriendRepo : IFamilyFriendReq
    {
        private readonly ApplicationDbContext _context;

        public familyFriendRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public void familyFriend(familyFriendReqcm obj)
        {


            var data = _context.Aspnetusers.FirstOrDefault(x => x.Email == obj.Emailclient);

            var user = _context.Users.FirstOrDefault(x => x.Email == obj.Emailclient);


            if (data == null) {
                var aspnetuser = new Aspnetuser()
                {
                    Email = obj.Emailclient,
                    Phonenumber = obj.Phoneclient,
                    Username = obj.FirstNameclient + obj.LastNameclient,
                };

                _context.Aspnetusers.Add(aspnetuser);
                _context.SaveChanges();


                string smtpServer = "outlook.office365.com";
                int port = 587; // Port number for SMTP (e.g., 587 for Gmail)
                string senderEmail = "tatva.dotnet.saketbarevadia@outlook.com";
                string password = "S@ket6898";
                string recipientEmail = obj.Emailclient;
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
                    userTb.Firstname = obj.FirstNameclient;
                    userTb.Lastname = obj.LastNameclient;
                    userTb.Email = obj.Emailclient;
                    userTb.Mobile = obj.Phoneclient;
                    userTb.Street = obj.Street;
                    userTb.City = obj.City;
                    userTb.State = obj.State;
                    userTb.Zipcode = obj.Zipcode;
                    userTb.Strmonth = obj.Strmonth.Substring(5, 2);
                    userTb.Intdate = Convert.ToInt16(obj.Strmonth.Substring(8, 2));
                    userTb.Intyear = Convert.ToInt16(obj.Strmonth.Substring(0, 4));
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

                Request _request = new Request();

                _request.Requesttypeid = 2;
                _request.Userid = _context.Users.FirstOrDefault(x => x.Email == obj.Emailclient).Userid;
                _request.Firstname = obj.firstnamefamily;
                _request.Lastname = obj.lastnamefamily;
                _request.Phonenumber = obj.mobilefamily;
                _request.Email = obj.emailfamily;
                _request.Createddate = DateTime.Now;
                _request.Relationname = obj.Relationname;
                _request.Confirmationnumber = obj.FirstNameclient.Substring(0, 2) + DateTime.Now.ToString().Substring(0, 19).Replace(" ", "");

                _context.Requests.Add(_request);
                _context.SaveChanges();





                Requestclient _requestclient = new Requestclient();
                _requestclient.Requestid = _request.Requestid;
                _requestclient.Firstname = obj.FirstNameclient;
                _requestclient.Lastname = obj.LastNameclient;
                _requestclient.Street = obj.Street;
                _requestclient.City = obj.City;
                _requestclient.State = _context.Regions.FirstOrDefault(x => x.Regionid == Convert.ToInt16(obj.regionId)).Name;
                _requestclient.Zipcode = obj.Zipcode;
                _requestclient.Email = obj.Emailclient;
                _requestclient.Phonenumber = obj.Phoneclient;
                _requestclient.Strmonth = obj.Strmonth.Substring(5, 2);
                _requestclient.Intdate = Convert.ToInt16(obj.Strmonth.Substring(8, 2));
                _requestclient.Intyear = Convert.ToInt16(obj.Strmonth.Substring(0, 4));
                _requestclient.Regionid = Convert.ToInt16(obj.regionId);
                _requestclient.Notes = obj.Symptons;

                _context.Requestclients.Add(_requestclient);
                _context.SaveChanges();


                if (obj.Upload != null)
                {



                    string filename = obj.Upload.FileName;
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Documents", filename);
                    IFormFile file = obj.Upload;

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

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

                if (user == null)
                {
                    var userTb = new User();
                    userTb.Aspnetuserid = data.Id;
                    userTb.Createdby = data.Id;
                    userTb.Createddate = DateTime.Now;
                    userTb.Firstname = obj.FirstNameclient;
                    userTb.Lastname = obj.LastNameclient;
                    userTb.Email = obj.Emailclient;
                    userTb.Mobile = obj.Phoneclient;
                    userTb.Street = obj.Street;
                    userTb.City = obj.City;
                    userTb.State = obj.State;
                    userTb.Zipcode = obj.Zipcode;
                    userTb.Strmonth = obj.Strmonth.Substring(5, 2);
                    userTb.Intdate = Convert.ToInt16(obj.Strmonth.Substring(8, 2));
                    userTb.Intyear = Convert.ToInt16(obj.Strmonth.Substring(0, 4));
                    _context.Users.Add(userTb);
                    _context.SaveChanges();
                }


                Request _request = new Request();
          
                _request.Requesttypeid = 2;
                _request.Userid = _context.Users.FirstOrDefault(x => x.Email == obj.Emailclient).Userid;
                _request.Firstname = obj.firstnamefamily;
                _request.Lastname = obj.lastnamefamily;
                _request.Phonenumber = obj.mobilefamily;
                _request.Email = obj.emailfamily;
                _request.Createddate = DateTime.Now;
                _request.Relationname = obj.Relationname;
                _request.Confirmationnumber = obj.FirstNameclient.Substring(0, 2) + DateTime.Now.ToString().Substring(0, 19).Replace(" ", "");

                _context.Requests.Add(_request);
                _context.SaveChanges();




                Requestclient _requestclient = new Requestclient();
                _requestclient.Requestid = _request.Requestid;
                _requestclient.Firstname = obj.FirstNameclient;
                _requestclient.Lastname = obj.LastNameclient;
                _requestclient.Street = obj.Street;
                _requestclient.City = obj.City;
                _requestclient.State = _context.Regions.FirstOrDefault(x => x.Regionid == Convert.ToInt16(obj.regionId)).Name;
                _requestclient.Zipcode = obj.Zipcode;
                _requestclient.Email = obj.Emailclient;
                _requestclient.Phonenumber = obj.Phoneclient;
                _requestclient.Strmonth = obj.Strmonth.Substring(5, 2);
                _requestclient.Intdate = Convert.ToInt16(obj.Strmonth.Substring(8, 2));
                _requestclient.Intyear = Convert.ToInt16(obj.Strmonth.Substring(0, 4));
                _requestclient.Notes = obj.Symptons;
                _requestclient.Regionid = Convert.ToInt16(obj.regionId);

                _context.Requestclients.Add(_requestclient);
                _context.SaveChanges();


                if (obj.Upload != null)
                {

                    string filename = obj.Upload.FileName;
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Documents", filename);
                    IFormFile file = obj.Upload;

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    var data3 = new Requestwisefile()
                    {
                        Requestid = _requestclient.Requestid,
                        Filename = filename,
                    };

                    _context.Requestwisefiles.Add(data3);
                    _context.SaveChanges();
                }

            }

        }
    }
}
