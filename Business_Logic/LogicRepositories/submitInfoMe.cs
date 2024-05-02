using System;
using Business_Logic.Interface;
using Data_Layer.CustomModels;
using Data_Layer.DataContext;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Layer.DataModels;
using Microsoft.AspNetCore.Http;

namespace Business_Logic.LogicRepositories
{
    public class submitInfoMe : IsubmitInfoMe
    {

        private readonly ApplicationDbContext _db;

        public submitInfoMe(ApplicationDbContext db)
        {
            _db = db;
        }

        public submitReqMe getInfo(string email)
        {
            submitReqMe submitReqMe = new submitReqMe() { 
             Email = email,
            };

            return submitReqMe;

        }


        public void submitMe(submitReqMe obj)
        {
            Request request = new Request();
            Requestclient requestclient = new Requestclient();
            Aspnetuser aspnetuser = new Aspnetuser();

            var data = _db.Aspnetusers.FirstOrDefault(x => x.Email == obj.Email);
            var user = _db.Users.FirstOrDefault(x => x.Email == obj.Email);


            if (data != null)
            {
                if (user == null)
                {
                    User user1 = new User();

                    user1.Aspnetuserid = data.Id;
                    user1.Firstname = obj.FirstName;
                    user1.Lastname = obj.LastName;
                    user1.Email = obj.Email;
                    user1.Mobile = obj.Phone;
                    user1.Street = obj.Street;
                    user1.City = obj.City;
                    user1.State = obj.State;
                    user1.Zipcode = obj.Zipcode;
                    user1.Createdby = data.Id;
                    user1.Createddate = DateTime.Now;
                    user1.Strmonth = obj.Strmonth.Substring(5, 2);
                    user1.Intdate = Convert.ToInt16(obj.Strmonth.Substring(8, 2));
                    user1.Intyear = Convert.ToInt16(obj.Strmonth.Substring(0, 4));

                    _db.Users.Add(user1);
                    _db.SaveChanges();
                }

                request.Requesttypeid = 1;
                request.Userid = _db.Users.FirstOrDefault(x => x.Email == obj.Email).Userid;
                request.Firstname = obj.FirstName;
                request.Lastname = obj.LastName;
                request.Phonenumber = obj.Phone;
                request.Email = obj.Email;
                request.Status = 1;
                request.Createddate = DateTime.Now;
                request.Confirmationnumber = obj.FirstName.Substring(0, 2) + DateTime.Now.ToString().Substring(0, 19).Replace(" ", "");
                _db.Requests.Add(request);
                _db.SaveChanges();



                requestclient.Requestid = request.Requestid;
                requestclient.Phonenumber = obj.Phone;
                requestclient.Firstname = obj.FirstName;
                requestclient.Lastname = obj.LastName;
                requestclient.Email = obj.Email;
                requestclient.Street = obj.Street;
                requestclient.City = obj.City;
                requestclient.State = _db.Regions.FirstOrDefault(x => x.Regionid == Convert.ToInt16(obj.regionId)).Name;
                requestclient.Zipcode = obj.Zipcode;
                requestclient.Strmonth = obj.Strmonth.Substring(5, 2);
                requestclient.Intdate = Convert.ToInt16(obj.Strmonth.Substring(8, 2));
                requestclient.Intyear = Convert.ToInt16(obj.Strmonth.Substring(0, 4));
                requestclient.Notes = obj.Symptons;
                requestclient.Regionid = Convert.ToInt16(obj.regionId);
                _db.Requestclients.Add(requestclient);
                _db.SaveChanges();


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
                        Requestid = requestclient.Requestid,
                        Filename = filename,
                    };

                    _db.Requestwisefiles.Add(data3);
                    _db.SaveChanges();
                }
            }
        }
    }
}