using System;
using Data_Layer.DataContext;
using Business_Logic.Interface;
using Data_Layer.CustomModels;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Data_Layer.DataModels;

namespace Business_Logic.LogicRepositories
{
    public class cancelCaseRepo : IcancelCase
    {
        private readonly ApplicationDbContext _context;

        public cancelCaseRepo(ApplicationDbContext context)
        {
            _context = context;
        }



        //To POST the data :--
        public void cancelCase(int id, int reason, string note)
        {
            var request = _context.Requests.FirstOrDefault(x => x.Requestid == id);
            var casetag = _context.Casetags.FirstOrDefault(x => x.Casetagid == reason).Name;

            if (request != null)
            {
                request.Status = 3;
                request.Casetag= casetag;

                _context.Requests.Update(request);
                _context.SaveChanges();


                Requeststatuslog requeststatuslog = new Requeststatuslog()
                {
                    Requestid = request.Requestid,
                    Status = request.Status,
                    Notes = note,
                    Createddate = DateTime.Now,
                };

                _context.Requeststatuslogs.Add(requeststatuslog);
                _context.SaveChanges();
            }


            }




        // To GET the data :--

        public List<AdminDashboardcm> caseTag()
        {
            var query = from r in _context.Casetags
                       select (new AdminDashboardcm()
                       {
                           Name = r.Name,
                           Casetagid= r.Casetagid,
                       });

            var data=query.ToList();

            return data;
        }
    }
}
