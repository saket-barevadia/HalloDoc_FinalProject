using System;
using Data_Layer.CustomModels;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Layer.DataModels;
using Business_Logic.LogicRepositories;
using System.Collections.Specialized;

namespace Business_Logic.Interface
{
    public interface IAdminDashboard
    {
        public byte[] GenerateExcelFile(List<AdminDashboardcm> adminData);
        public List<AdminDashboardcm> data(int reqTypeId, int regionID);

        public void blockCase(int requestId, string reasonNote);

        public Assigncase assigncases(int requestId);

        public List<Physician> getPhysicianName(int physicianId);

        public void assignCasePost(int reqId, int regionId, int physicianId, string description);

        public ViewUploads viewUploads(int reqId, int flag);

        public void postUploads(ViewUploads cm);

       public void deleteUploads(int Id);

        public void sendMail(int reqID);

        public Orders getProfessions(int id);

        public List<Profession> getVendor(int id);

        public Orders getVendorDetails(int id);

        public bool postOrderDetails(Orders cm, int id);


        public List<TransferRequest> transferRequests(int requestId);

        public void transferReqPost(int reqId, int regionId, int physicianId, string description, string email);

        public bool clearCasePost(int id);

        public SendAgreement SendAgreement(int id);

        public void sendEmail(SendAgreement cm);

        public CloseCase CloseCase(int reqid);

        public void closeCasePost(CloseCase cm);

        public void close(int reqid);

        public AdminProfilecm adminData(string email,int flag);

        public void EditadminProfile(AdminProfilecm cm, List<int> adminRegions);

        public void resetPass(AdminProfilecm cm);

        public void editAdminAddress(AdminProfilecm cm);

        public Encounter encounterFormData(int reqid);

        public void encounterFormPost(Encounter cm);

        public void sendLink(SendAgreement cm);

        public void adminCreateReq(AdminCreateReq cm, string email);

        public List<Regioncm> getRegions();


        //scheduling

        public List<Physician> GetPhysicians(int regionid);

        public SchedulingCm GetRegions(int phyId);

        public List<ShiftDetailsmodal> ShiftDetailsmodal(DateTime date, DateTime sunday, DateTime saturday, string type);

        public bool createShift(ScheduleModel scheduleModel, string email);

        ShiftDetailsmodal GetShift(int shiftdetailsid);

        public List<Physician> GetRegionvalue(int selectedregion);

        public void SetReturnShift(int status, int shiftdetailid, int Aspid);

        public bool SetEditShift(ShiftDetailsmodal shiftDetailsmodal, int Aspid);

        public void SetDeleteShift(int shiftdetailid, int Aspid);

        public SchedulingCm GetShiftReview(int regionId, int callId);

        public void ApproveSelectedShift(int[] shiftDetailsId, int Aspid);

        public void DeleteShiftReview(int[] shiftDetailsId, int Aspid);

        public OnCallModal GetOnCallDetails(int regionId);



        //Records
        public patientRecordscm patientRecords(patientRecordscm cm);

        public void deletePermanently(int requestId);

        public patientRecordscm getPatientReqData(patientRecordscm cm);

        public patientRecordscm GetPatientData(int userId);

        public patientRecordscm getBlockHistory(patientRecordscm cm);

        public void unblockPatient(int reqId);

        public patientRecordscm emailLogs(patientRecordscm cm);
        public patientRecordscm smsLogs(patientRecordscm cm);





        public List<string> GetListOfRoleMenu(int roleId);
        public bool checkreq(int requestid);




    }
}
