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
        public ProviderDashboardcm GetPatientDetails(int status, int reqTypeId, int Phyid, int flag);

        public void PostTransferRequest(ProviderDashboardcm cm, int PhyId);

        public void AcceptRequest(int Requestid, int physicianid);


        public void PostEncounterCare(Encounter encounterFormData, int Requestid);

        public void concludeReq(int requestId);

        public void finalizeForm(int requestId);

        public void addNote(ViewUploads cm);


        public void reqToAdmin(AdminDashboardcm cm,string email);

    }
}
