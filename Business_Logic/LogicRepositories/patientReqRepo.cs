using Business_Logic.Interface;
using Data_Layer.DataContext;
using Data_Layer.DataModels;
using Data_Layer.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace Business_Logic.LogicRepositories
{
    public class patientReqRepo : IPatientReq
    {
        private readonly ApplicationDbContext _db;

        public patientReqRepo(ApplicationDbContext db)
        {
            _db = db;
        }

        public int UserExist(string email)
        {
            var exist = _db.Aspnetusers.FirstOrDefault(u => u.Email == email);
            if (exist == null)
            {
                return 402;
            }
            else
            {
                return 401;
            }

        }

        public void patientReq(PatientRequestCm obj)
        {
            Request request = new Request();
            Requestclient requestclient = new Requestclient();
            Aspnetuser aspnetuser = new Aspnetuser();

            var data = _db.Aspnetusers.FirstOrDefault(x => x.Email == obj.Email);

            var user=_db.Users.FirstOrDefault(x=>x.Email == obj.Email);
             
            var asp=_db.Aspnetusers.FirstOrDefault(x=>x.Email==obj.Email);

             

            if (data == null)
            {
                if (obj.PasswordHash == obj.ConfirmPasswordHash)
                {
                    aspnetuser.Username = obj.Email.Substring(0, obj.Email.IndexOf("@"));
                    aspnetuser.Email = obj.Email;
                    aspnetuser.Phonenumber = obj.Phone;
                    aspnetuser.Passwordhash = obj.PasswordHash;
                    aspnetuser.Createddate = DateTime.Now;
                    _db.Aspnetusers.Add(aspnetuser);
                    _db.SaveChanges();
                }

                if (user == null)
                {
                    var userTb = new User();
                    userTb.Aspnetuserid = aspnetuser.Id;
                    userTb.Createdby = aspnetuser.Id;
                    userTb.Createddate = DateTime.Now;
                    userTb.Firstname = obj.FirstName;
                    userTb.Lastname = obj.LastName;
                    userTb.Email = obj.Email;
                    userTb.Mobile = obj.Phone;
                    userTb.Street = obj.Street;
                    userTb.City = obj.City;
                    userTb.State = obj.State;
                    userTb.Zipcode = obj.Zipcode;
                    userTb.Strmonth = obj.Strmonth.Substring(5, 2);
                    userTb.Intdate = Convert.ToInt16(obj.Strmonth.Substring(8, 2));
                    userTb.Intyear = Convert.ToInt16(obj.Strmonth.Substring(0, 4));
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



                request.Requesttypeid = 1;
                request.Userid = _db.Users.FirstOrDefault(x => x.Email == obj.Email).Userid;
                request.Firstname = obj.FirstName;
                request.Lastname = obj.LastName;
                request.Phonenumber = obj.Phone;
                request.Email = obj.Email;
                request.Createddate = DateTime.Now;
                request.Confirmationnumber = obj.FirstName.Substring(0, 2) + DateTime.Now.ToString().Substring(0, 19).Replace(" ", "");

                _db.Requests.Add(request);
                _db.SaveChanges();



                requestclient.Requestid = request.Requestid;
                requestclient.Firstname = obj.FirstName;
                requestclient.Lastname = obj.LastName;
                requestclient.Email = obj.Email;
                requestclient.Street = obj.Street;
                requestclient.Phonenumber = obj.Phone;
                requestclient.City = obj.City;
                requestclient.State = _db.Regions.FirstOrDefault(x => x.Regionid == Convert.ToInt16(obj.regionId)).Name;
                requestclient.Zipcode = obj.Zipcode;
                requestclient.Strmonth = obj.Strmonth.Substring(5, 2);
                requestclient.Intdate = Convert.ToInt16(obj.Strmonth.Substring(8, 2));
                requestclient.Intyear = Convert.ToInt16(obj.Strmonth.Substring(0, 4));
                requestclient.Notes = obj.symptoms;
                requestclient.Regionid = Convert.ToInt16(obj.regionId);
                _db.Requestclients.Add(requestclient);
                _db.SaveChanges();


                if (obj.Uploadd != null)
                {
                    string filename = obj.Uploadd.FileName;
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Documents", filename);
                    IFormFile file = obj.Uploadd;

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    var data3 = new Requestwisefile()
                    {
                        Requestid = request.Requestid,
                        Filename = filename,
                    };

                    _db.Requestwisefiles.Add(data3);
                    _db.SaveChanges();
                }
            }

            if (data != null)
            {

                if (user == null)
                {
                    var userTb=new User();
                    userTb.Aspnetuserid = asp.Id;
                    userTb.Createdby = data.Id;
                    userTb.Createddate = DateTime.Now;
                    userTb.Firstname = obj.FirstName;
                    userTb.Lastname = obj.LastName;
                    userTb.Email = obj.Email;
                    userTb.Mobile = obj.Phone;
                    userTb.Street = obj.Street;
                    userTb.City = obj.City;
                    userTb.State = obj.State;
                    userTb.Zipcode = obj.Zipcode;
                    userTb.Strmonth=obj.Strmonth.Substring(5, 2);
                    userTb.Intdate= Convert.ToInt16(obj.Strmonth.Substring(8, 2));
                    userTb.Intyear = Convert.ToInt16(obj.Strmonth.Substring(0, 4));
                    _db.Users.Add(userTb);
                    _db.SaveChanges();
                }



  
                request.Requesttypeid = 1;
                request.Userid = _db.Users.FirstOrDefault(x => x.Email == obj.Email).Userid;
                request.Firstname = obj.FirstName;
                request.Lastname = obj.LastName;
                request.Phonenumber = obj.Phone;
                request.Email = obj.Email;
                request.Createddate = DateTime.Now;
                request.Confirmationnumber = obj.FirstName.Substring(0, 2) + DateTime.Now.ToString().Substring(0, 19).Replace(" ", "");

                _db.Requests.Add(request);
                _db.SaveChanges();


                
                requestclient.Requestid = request.Requestid;
                requestclient.Firstname = obj.FirstName;
                requestclient.Lastname = obj.LastName;
                requestclient.Email = obj.Email;
                requestclient.Street = obj.Street;
                requestclient.Phonenumber = obj.Phone;
                requestclient.City = obj.City;
                requestclient.State = _db.Regions.FirstOrDefault(x => x.Regionid == Convert.ToInt16(obj.regionId)).Name;
                requestclient.Zipcode = obj.Zipcode;
                requestclient.Strmonth= obj.Strmonth.Substring(5, 2);
                requestclient.Intdate= Convert.ToInt16(obj.Strmonth.Substring(8, 2));
                requestclient.Intyear = Convert.ToInt16(obj.Strmonth.Substring(0, 4));
                requestclient.Notes = obj.symptoms;
                requestclient.Regionid = Convert.ToInt16(obj.regionId);
                _db.Requestclients.Add(requestclient);
                _db.SaveChanges();

                if (obj.Uploadd != null)
                {
                    string filename = obj.Uploadd.FileName;
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Documents", filename);
                    IFormFile file = obj.Uploadd;

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    var data3 = new Requestwisefile()
                    {
                        Requestid = request.Requestid,
                        Filename = filename,
                    };

                    _db.Requestwisefiles.Add(data3);
                    _db.SaveChanges();

                }

            }



        }



        




    }
}
