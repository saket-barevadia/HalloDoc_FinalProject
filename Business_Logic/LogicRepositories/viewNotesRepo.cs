using System;
using Business_Logic.Interface;
using Data_Layer.DataContext;
using Data_Layer.CustomModels;
using Data_Layer.DataModels;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Layer.CustomModels;
using System.Security.Cryptography.X509Certificates;

namespace Business_Logic.LogicRepositories
{
    public class viewNotesRepo : IviewNotes
    {
        private readonly ApplicationDbContext _context;

        public viewNotesRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        //POST
        public void viewNote(Requestnote cm,int id)
        {
           var addNote=_context.Requestnotes.FirstOrDefault(x=>x.Requestid== id);

            if (cm.Adminnotes != null)
            {
                addNote.Adminnotes = cm.Adminnotes;
                addNote.Modifieddate = DateTime.Now;

                _context.Requestnotes.Update(addNote);
                _context.SaveChanges();
            }
            if (cm.Physiciannotes != null)
            {
                addNote.Physiciannotes = cm.Physiciannotes;
                addNote.Modifieddate = DateTime.Now;

                _context.Requestnotes.Update(addNote);
                _context.SaveChanges();
            }

           
                                   
        }


        public viewNotescm addNote(int id, int flag)
        {

            var reqId = _context.Requestnotes.FirstOrDefault(x => x.Requestid == id);
            var transferNote = _context.Requeststatuslogs.OrderBy(i => i.Requeststatuslogid).LastOrDefault(i => i.Requestid == id);


            var user = _context.Requests.FirstOrDefault(x => x.Requestid == id).Userid;


            var status = _context.Requests.FirstOrDefault(x => x.Requestid == id).Status;
            var cancelData = _context.Requeststatuslogs.OrderBy(i => i.Requeststatuslogid).LastOrDefault(i => i.Requestid == id && (i.Status == 3 || i.Status == 7));

            if (reqId != null)
            {
                viewNotescm viewNotes = new viewNotescm()
                {
                    Requestid = id,
                    Status = status,
                    Adminnotes = reqId.Adminnotes,
                    Physiciannotes = reqId.Physiciannotes,
                    userId = user,

                    transfernotes = transferNote == null ? "" : transferNote.Notes,
                    cancellationNotes = cancelData == null ? "" : cancelData.Notes,
                    flag = flag,
                };

                return viewNotes;

            }
            else
            {
                Requestnote requestnote = new Requestnote()
                {
                    Requestid = _context.Requests.FirstOrDefault(x => x.Requestid == id).Requestid,
                    Createdby = _context.Users.FirstOrDefault(x => x.Userid == user).Createdby,
                    Createddate = DateTime.Now,

                };

                _context.Requestnotes.Add(requestnote);
                _context.SaveChanges();

                viewNotescm viewNotes = new viewNotescm()
                {
                    Requestid = id,
                    Status = status,
                    transfernotes = transferNote == null ? "" : transferNote.Notes,
                    cancellationNotes = cancelData == null ? "" : cancelData.Notes,
                    flag = flag,
                    userId = user,
                };

                return viewNotes;
            
        

            }
          
            }
        
    }
}
