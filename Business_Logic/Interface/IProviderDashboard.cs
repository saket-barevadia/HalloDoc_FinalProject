using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Layer.DataModels;
using Data_Layer.CustomModels;

namespace Business_Logic.Interface
{
    public interface IProviderDashboard
    {
        public ProviderDashboardcm GetPatientDetails(int status, int reqTypeId, int phyid, int flag);

        public void PostTransferRequest(ProviderDashboardcm cm, int phyId);

        public void AcceptRequest(int requestid, int physicianid);

        public void PostEncounterCare(Encounter encounterFormData, int requestid);

        public void ConcludeReq(int requestId);

        public void FinalizeForm(int requestId);

        public void AddNote(ViewUploads cm);

        public void ReqToAdmin(AdminDashboardcm cm,string email);

        public List<DateViewModel> GetDates();

    }
}
