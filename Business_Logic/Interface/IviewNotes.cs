using System;
using Data_Layer.CustomModels;
using Data_Layer.DataModels;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.Interface
{
    public interface IviewNotes
    {
        public void viewNote(Requestnote cm,int id);

        public viewNotescm addNote(int id, int flag);
    }
}
