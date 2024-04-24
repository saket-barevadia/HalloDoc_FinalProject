using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.DataModels;
using DataLayer.CustomModels;

namespace BusinessLayer.Interface
{
    public interface IRecords
    {
        public Recordscm GetRecords();

        public Recordscm GetBookDetails(int BorrowerId);

        public bool SaveBookForm(Recordscm cm);

        public bool DeleteBookRecord(int BorrowerId);

        
    }
}
