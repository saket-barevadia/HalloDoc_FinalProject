using Business_Logic.Interface;
using Data_Layer.CustomModels;
using Data_Layer.DataContext;
using Data_Layer.DataModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.LogicRepositories
{
    public class ILogicRepositories : ILogin
    {
        private readonly ApplicationDbContext _db;

        public ILogicRepositories(ApplicationDbContext db)
        {
            _db = db;
        }
        public patientLogincm login(patientLogincm obj)
        {

            var exist = _db.Aspnetusers.Where(x => x.Email == obj.Email && x.Passwordhash == obj.Password);

            if (exist != null)
            {

                var checkRoleId = _db.Admins.FirstOrDefault(r => r.Email == obj.Email);

                var roleIdMain = 0;

                if (checkRoleId != null)
                {
                    roleIdMain = _db.Admins.Where(r => r.Email == obj.Email).Select(x => (int)x.Roleid).First();
                }

                var physician = _db.Physicians.FirstOrDefault(x => x.Aspnetuserid == _db.Aspnetusers.FirstOrDefault(x => x.Email == obj.Email).Id);

                if (physician != null)
                {
                    var phyId = physician.Physicianid;
                    var user1 = _db.Aspnetusers.Where(x => x.Email == obj.Email && x.Passwordhash == obj.Password).Select(r => new patientLogincm()
                    {
                        Email = r.Email,
                        Password = r.Passwordhash,
                        //Role = _db.Aspnetroles.Where(y =>y.Id == _db.Aspnetuserroles.Where(x =>x.Userid ==_db.Users.FirstOrDefault(x=>x.Aspnetuserid==r.Id).Userid).Select(x => x.Roleid).First()).Select(y => y.Name).First(),
                        Role = _db.Aspnetroles.Where(y => y.Id == _db.Aspnetuserroles.Where(x => x.Userid == r.Id).Select(x => x.Roleid).First()).Select(y => y.Name).First(),
                        id = r.Id,
                        Username = r.Username,
                        roleId = roleIdMain,
                        Physicianid = phyId,
                    }).ToList().FirstOrDefault();

                    return user1;
                }




                var user = _db.Aspnetusers.Where(x => x.Email == obj.Email && x.Passwordhash == obj.Password).Select(r => new patientLogincm()
                {
                    Email = r.Email,
                    Password = r.Passwordhash,
                    //Role = _db.Aspnetroles.Where(y =>y.Id == _db.Aspnetuserroles.Where(x =>x.Userid ==_db.Users.FirstOrDefault(x=>x.Aspnetuserid==r.Id).Userid).Select(x => x.Roleid).First()).Select(y => y.Name).First(),
                    Role = _db.Aspnetroles.Where(y => y.Id == _db.Aspnetuserroles.Where(x => x.Userid == r.Id).Select(x => x.Roleid).First()).Select(y => y.Name).First(),
                    id = r.Id,
                    Username = r.Username,
                    roleId = roleIdMain,

                }).ToList().FirstOrDefault();

                return user;


            }
            else
            {
                return null;
            }


        }

    }
}



