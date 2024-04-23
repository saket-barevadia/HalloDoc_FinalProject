using System;
using Business_Logic.Interface;
using Data_Layer.DataContext;
using Data_Layer.DataModels;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Data_Layer.CustomModels.ProviderMenucm;

namespace Business_Logic.LogicRepositories
{
    public class patientProfileRepo : IpatientProfile
    {
        private readonly ApplicationDbContext _db;

        public patientProfileRepo(ApplicationDbContext db)
        {
            _db = db;
        }

        public void update(User cm)
        {
            var user=_db.Users.FirstOrDefault(a=>a.Email == cm.Email);
            

            if (user != null)
            {
                user.Aspnetuserid=_db.Aspnetusers.FirstOrDefault(a=> a.Email == cm.Email).Id;
                user.Firstname = cm.Firstname;
                user.Lastname = cm.Lastname;
                user.Mobile = cm.Mobile;
                user.Email = cm.Email;
                user.Street = cm.Street;
                user.City = cm.City;
                user.State = cm.State;
                user.Zipcode = cm.Zipcode;
                

                _db.Users.Update(user);
                _db.SaveChanges();
            }
        }



        
    }
}
