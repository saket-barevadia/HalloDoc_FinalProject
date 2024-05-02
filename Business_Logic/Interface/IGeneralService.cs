using Data_Layer.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.Interface
{
    public interface IGeneralService
    {
        //public List<DateViewModel> GetDates();

        public Invoicingcm getDataOfTimesheet(DateOnly startDate, DateOnly endDate, int? PhysicianId, int? AdminID);

        public void SubmitTimeSheet(Invoicingcm model, int? PhysicianId);

        public Invoicingcm GetInvoicingDataonChangeOfDate(DateOnly startDate, DateOnly endDate, int? PhysicianId, int? AdminID);

        public Invoicingcm GetUploadedDataonChangeOfDate(DateOnly startDate, DateOnly endDate, int? PhysicianId, int pageNumber, int pagesize);

        public void FinalizeTimeSheet(int id);

        public void DeleteBill(int id);
    }
}
