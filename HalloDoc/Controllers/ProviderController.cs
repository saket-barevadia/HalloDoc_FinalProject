using Microsoft.AspNetCore.Mvc;
using Business_Logic.Interface;
using Data_Layer.DataContext;
using Data_Layer.DataModels;
using Data_Layer.CustomModels;
using static HelloDocMvc.Repository.Repositories.AuthManager;
using System.Text;
using Rotativa.AspNetCore;

namespace HalloDoc.Controllers
{
    public class ProviderController : Controller
    {
        private readonly IProviderDashboard _providerDashboard;
        private readonly IAdminDashboard _dashboard;
        private readonly IviewCase _viewCase;
        private readonly IviewNotes _viewNotes;
        private readonly IProviderMenu _providerMenu;
        private readonly IGeneralService _generalService;
        private readonly ApplicationDbContext _context;
        private readonly IAdminDashboard _adminDash;

        public ProviderController(IProviderDashboard providerDashboard,IAdminDashboard dashboard,IviewCase viewCase, IviewNotes viewNotes, IProviderMenu providerMenu,ApplicationDbContext context, IGeneralService generalService, IAdminDashboard adminDashboard) { 
          _providerDashboard = providerDashboard;
            _dashboard = dashboard;
            _viewCase = viewCase;
            _viewNotes = viewNotes;
            _providerMenu = providerMenu;
            _context = context;
            _generalService = generalService;
            _adminDash=adminDashboard;
        }


        [CustomAuthorize("Provider")]
        public IActionResult ProviderDashboardMain()
        {
            @ViewBag.Admin = 3;
            var roleMain = HttpContext.Session.GetInt32("roleId");
            List<string> roleMenu = _dashboard.GetListOfRoleMenu((int)roleMain);
            ViewBag.Menu = roleMenu;
            var Phyid = HttpContext.Session.GetInt32("physicianId");
            var patientDetails = _providerDashboard.GetPatientDetails(0,0, Convert.ToInt32(Phyid),0);
            return View(patientDetails);
        }

        public IActionResult ProviderDashboardShared()
        {
            var Phyid = HttpContext.Session.GetInt32("physicianId");
            var patientDetails = _providerDashboard.GetPatientDetails(0, 0, Convert.ToInt32(Phyid), 0);
            return PartialView("ProviderShared/ProviderDashboardShared",patientDetails);
        }

        public IActionResult ProviderTable(int status,int reqTypeId,int flag)
        
        {
            var Phyid = HttpContext.Session.GetInt32("physicianId");
            if (flag != null)
            {
                var patientDetails = _providerDashboard.GetPatientDetails(status, reqTypeId, Convert.ToInt32(Phyid),flag);
                return PartialView("ProviderShared/ProviderTable", patientDetails);
            }
            else
            {
                var patientDetails = _providerDashboard.GetPatientDetails(status, reqTypeId, Convert.ToInt32(Phyid),0);
                return PartialView("ProviderShared/ProviderTable", patientDetails);
            }         
            
        }

        public IActionResult TransferRequest(int requestId)
        {
            ProviderDashboardcm providerDashboardcm = new ProviderDashboardcm
            {
                Requestid = requestId,
            };

            return PartialView("ProviderShared/TransferRequest",providerDashboardcm);
        }

        [HttpPost]
        public IActionResult TransferReqPost(ProviderDashboardcm cm)
        {
            var Phyid = HttpContext.Session.GetInt32("physicianId");
            _providerDashboard.PostTransferRequest(cm, Convert.ToInt32(Phyid));
            return Ok();
        }

        public IActionResult CreateRequest()
        {
            @ViewBag.Admin = 3;     
            var regions = new AdminCreateReq()
            {
                regions = _dashboard.getRegions(),
            };
            return PartialView("ProviderShared/CreateRequest",regions);
        }

        [HttpPost]
        public IActionResult createRequest(AdminCreateReq cm)
        {
            string email = HttpContext.Session.GetString("email");
            _dashboard.adminCreateReq(cm, email);          
            return RedirectToAction("ProviderDashboardMain");
        }

        public IActionResult SendLink()
        {
            return PartialView("ProviderShared/SendLink");
        }

        [HttpPost]
        public IActionResult SendLinkPost(SendAgreement cm)
        {
            _dashboard.sendLink(cm);
            return Ok();
        }

        public IActionResult ViewCase(int id)
        {
            @ViewBag.Admin = 3;
           
            var data = _viewCase.viewCase(id, 0);
            if (data == null)
            {
                return NotFound();
            }
            else
            {
                return PartialView("ProviderShared/ViewCase", data);
            }
            
        }

        public IActionResult ViewNotes(int id)
        {
            @ViewBag.Admin = 3;
            
            var data = _viewNotes.addNote(id, 5);
            if (data != null)
            {
                return PartialView("ProviderShared/ViewNotes",data);
            }

            return PartialView("ProviderShared/ViewNotes");
        }

        [HttpPost]
        public IActionResult ViewNotesPost(Requestnote cm)
        {
            _viewNotes.viewNote(cm,cm.Requestid);

            return Ok();

        }

        public IActionResult Accept(int id)
        {
            return PartialView("ProviderShared/Accept");
        }

        [HttpPost]
        public IActionResult AcceptRequest(int Requestid)
        {
            var Phyid = HttpContext.Session.GetInt32("physicianId");
            if (Requestid != null)
            {
                _providerDashboard.AcceptRequest(Requestid, Convert.ToInt32(Phyid));
                return Ok();
            }
            return Json(new { Error = "Returned in else" });
        }

        public IActionResult SendAgreement(int Requestid)
        {
            var data = _dashboard.SendAgreement(Requestid);
            return PartialView("ProviderShared/SendAgreement",data);
        }

        [HttpPost]
        public IActionResult SendAgreementPost(SendAgreement cm)
        {
            _dashboard.sendEmail(cm);         
            return Ok();
        }

        public IActionResult ViewUploads(int reqId)
        {
            @ViewBag.Admin = 3;
            var data = _dashboard.viewUploads(reqId, 2);
            if (data == null)
            {
                return NotFound();
            }
            else
            {
                return PartialView("ProviderShared/ViewUploads", data);
            }       
        }

        [HttpPost]
        public IActionResult ViewUploads(ViewUploads cm)
        {
            _dashboard.postUploads(cm);
           
            return Ok();
        }

        [HttpPost]
        public IActionResult SendEmail(ViewUploads cm)
        {
            _dashboard.sendMail(cm.Requestid);
            return Ok();
        }

        [HttpPost]
        public IActionResult Delete(int id, int reqId)
        {
            _dashboard.deleteUploads(id);
            return Ok();
        }


        public IActionResult Orders(int reqId)
        {
            @ViewBag.Admin = 3;
            var data = _dashboard.getProfessions(reqId);
            return PartialView("ProviderShared/Orders",data);
        }

        public IActionResult GetBusiness(int id)
        {
            var business = _dashboard.getVendor(id);

            return Json(business);
        }

        public IActionResult GetVendorData(int vendorID)
        {
            HttpContext.Session.SetInt32("vendorId", vendorID);
            var vendorDetails = _dashboard.getVendorDetails(vendorID);
            return Json(vendorDetails);
        }

        [HttpPost]
       public IActionResult OrdersPost(Orders cm)
        {
            var id = Convert.ToInt32(HttpContext.Session.GetInt32("vendorId"));

            _dashboard.postOrderDetails(cm, id);        
            return Ok();
        }

        public IActionResult EditProviderr(int PhyId)
        {
            @ViewBag.Admin = 3;        
            var physicianData = _providerMenu.providerProfile(Convert.ToInt32(PhyId), 0);
            return PartialView("ProviderShared/ProviderProfile",physicianData);
        }

        [HttpPost]
        public IActionResult EditProvider(ProviderMenucm.ProviderProfile cm)
        {
            _providerMenu.editProvider(cm);
            return Ok();
        }

        [HttpPost]
        public IActionResult Administrator(ProviderMenucm.ProviderProfile cm, List<int> physicianRegions)
        {
            _providerMenu.editphysicianInfo(cm, physicianRegions);
            return Ok();
        }
        
        [HttpPost]
        public IActionResult Mailing(ProviderMenucm.ProviderProfile cm)
        {
            _providerMenu.editMailingForm(cm);
            return Ok();
        }
          
        [HttpPost]
        public IActionResult ProviderInfo(ProviderMenucm.ProviderProfile cm)
        {
            _providerMenu.editproviderInfo(cm);
            return Ok();
        } 
        
        [HttpPost]
        public IActionResult Boarding(ProviderMenucm.ProviderProfile cm)
        {
            _providerMenu.EditOnBoardingData(cm);
            return Ok();
        } 
        
        [HttpPost]
        public IActionResult DeleteAccount(ProviderMenucm.ProviderProfile cm)
        {
            _providerMenu.removePhysician(cm);
            return Ok();
        }

        [HttpPost]
        public void ResetPass(ProviderMenucm.ProviderProfile cm)
        {
            _providerMenu.resetPassword(cm);
        }

        public IActionResult Housecall(int requestid)
        {
            AdminDashboardcm adminDashboardcm = new AdminDashboardcm() { 
             encounter=null,
             Requestid=requestid,
            };

            return PartialView("ProviderShared/EncounterActive", adminDashboardcm);
        }

        public IActionResult EncounterCare(int requestid)
        {
            AdminDashboardcm adminDashboardCm = new AdminDashboardcm()
            {
                encounter = null,
                Requestid = requestid,
            };
            return PartialView("ProviderShared/EncounterActive", adminDashboardCm);
        }

        [HttpPost]
        public IActionResult PostEncounterCare(AdminDashboardcm cm)
        {
            if (cm.Requestid != 0)
            {
                _providerDashboard.PostEncounterCare(cm.encounter, cm.Requestid);
                return Ok();
            }

            return Json(new { Error = "Returned in else" });
        }
        public IActionResult Encounterr(int reqId)
        {
            @ViewBag.Admin = 3;         
            var encounterData = _dashboard.encounterFormData(reqId);
            return PartialView("ProviderShared/Encounter",encounterData);
        }

        [HttpPost]
        public IActionResult Encounter(Encounter cm)
        {
            _dashboard.encounterFormPost(cm);
            
            return Ok();
        }

        [HttpPost]
        public IActionResult PostHouseCall(int requestId)
        {
            _providerDashboard.ConcludeReq(requestId);

            return Ok();
        }

        [HttpPost]
        public IActionResult Finalize(int requestId)
        {
            _providerDashboard.FinalizeForm(requestId);

            return Ok();
        }

        public IActionResult GeneratePDF(int requestid)
        {
            AdminDashboardcm model = new AdminDashboardcm();
            model.encounter = _dashboard.encounterFormData(requestid);

            if (model == null)
            {
                TempData["error"] = "Something Went Wrong!";
                return NotFound();
            }

            DateTime currentDateTime = DateTime.Now;
            string fileName = $"encounter_{currentDateTime.ToString("ddMMyyyy_HHmmss")}.pdf";
            return new ViewAsPdf("ProviderShared/EncounterDownload", model)
            {
                FileName = fileName
            };
        }

        public IActionResult FinalizeEncounter()
        {
            return PartialView("ProviderShared/EncounterActive");
        }

        public IActionResult RequestToAdimin()
        {
            return PartialView("ProviderShared/EncounterActive");
        }

        [HttpPost]
        public IActionResult PostReqToAdmin(AdminDashboardcm cm)
        {
            string email = HttpContext.Session.GetString("email");
            _providerDashboard.ReqToAdmin(cm, email);
            return Ok();
        }

        public IActionResult ConcludecCare(int reqId)
        {
            @ViewBag.Admin = 3;
            var data = _dashboard.viewUploads(reqId, 3);
            return PartialView("ProviderShared/ConcludeCare",data);
        }

        [HttpPost]
        public IActionResult PostNote(ViewUploads cm)
        {
            _providerDashboard.AddNote(cm);
            return RedirectToAction("ProviderDashboardMain");
        }
 
        //scheduling
        public IActionResult SchedulingShared()
        {
            @ViewBag.Admin = 3;
            var PhyId = HttpContext.Session.GetInt32("physicianId");
            var schedulingCm = _dashboard.GetRegions(Convert.ToInt32(PhyId));
            return PartialView("ProviderShared/SchedulingShared",schedulingCm);
        }

        public IActionResult CreateNewShift()
        {
            var PhyId = HttpContext.Session.GetInt32("physicianId");
            var schedulingCm = _dashboard.GetRegions(Convert.ToInt32(PhyId));
            return PartialView("ProviderShared/CreateNewShift",schedulingCm);
        }

        [HttpPost]
        public IActionResult CreateShiftPost(SchedulingCm cm)
        {
            string email = HttpContext.Session.GetString("email");
            if (_dashboard.createShift(cm.ScheduleModel, email))
            {
                return Ok(true);              
            }
            else
            {
                return Ok(false);        
            }
        }

        [HttpPost]
        public IActionResult Loadshift(string datestring, string sundaystring, string saturdaystring, string shifttype, int regionid)
        {
            DateTime date = DateTime.Parse(datestring);
            DateTime sunday = DateTime.Parse(sundaystring);
            DateTime saturday = DateTime.Parse(saturdaystring);

            switch (shifttype)
            {
                case "month":
                    MonthShiftModal monthShift = new MonthShiftModal();

                    var totalDays = DateTime.DaysInMonth(date.Year, date.Month);
                    var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                    var startDayIndex = (int)firstDayOfMonth.DayOfWeek;

                    var dayceiling = (int)Math.Ceiling((float)(totalDays + startDayIndex) / 7);

                    monthShift.daysLoop = (int)dayceiling * 7;
                    monthShift.daysInMonth = totalDays;
                    monthShift.firstDayOfMonth = firstDayOfMonth;
                    monthShift.startDayIndex = startDayIndex;
                    monthShift.shiftDetailsmodals = _dashboard.ShiftDetailsmodal(date, sunday, saturday, "month");

                    return PartialView("ProviderShared/MonthWise", monthShift);

                case "week":
                    WeekShiftModal weekShift = new WeekShiftModal();

                    weekShift.Physicians = _dashboard.GetPhysicians(regionid);
                    weekShift.shiftDetailsmodals = _dashboard.ShiftDetailsmodal(date, sunday, saturday, "week");

                    List<int> dlist = new List<int>();

                    for (var i = 0; i < 7; i++)
                    {
                        var date12 = sunday.AddDays(i);
                        dlist.Add(date12.Day);
                    }

                    weekShift.datelist = dlist.ToList();
                    weekShift.dayNames = new string[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

                    return PartialView("WeekWiseShift", weekShift);

                case "day":
                    DayShiftModal dayShift = new DayShiftModal();
                    dayShift.Physicians = _dashboard.GetPhysicians(regionid);
                    dayShift.shiftDetailsmodals = _dashboard.ShiftDetailsmodal(date, sunday, saturday, "day");

                    return PartialView("DayWiseShift", dayShift);

                default:
                    return PartialView();
            }
        }

        public IActionResult OpenScheduledModal(ViewShiftModal viewShiftModal)
        {
            HttpContext.Session.SetInt32("shiftdetailsid", viewShiftModal.shiftdetailsid);
            switch (viewShiftModal.actionType)
            {
                case "shiftdetails":
                    ShiftDetailsmodal shift = _dashboard.GetShift(viewShiftModal.shiftdetailsid);
                    return PartialView("ProviderShared/ViewShift", shift);

                case "moremonthshifts":
                    DateTime date = DateTime.Parse(viewShiftModal.datestring);
                    ShiftDetailsmodal ScheduleModel = new ShiftDetailsmodal();
                    var list = ScheduleModel.ViewAllList = _dashboard.ShiftDetailsmodal(date, DateTime.Now, DateTime.Now, "month").Where(i => i.Shiftdate.Day == viewShiftModal.columnDate.Day).ToList();
                    ViewBag.TotalShift = list.Count();
                    return PartialView("MoreShift", ScheduleModel);

                default:

                    return PartialView();
            }
        }

        public IActionResult OpenScheduledModalWeek(string sundaystring, string saturdaystring, string datestring, DateTime shiftdate, int physicianid)
        {
            DateTime sunday = DateTime.Parse(sundaystring);
            DateTime saturday = DateTime.Parse(saturdaystring);
            DateTime date1 = DateTime.Parse(datestring);
            ShiftDetailsmodal ScheduleModel = new ShiftDetailsmodal();
            var list = ScheduleModel.ViewAllList = _dashboard.ShiftDetailsmodal(date1, sunday, saturday, "week").Where(i => i.Shiftdate.Day == shiftdate.Day && i.Physicianid == physicianid).ToList();
            ViewBag.TotalShift = list.Count();
            return PartialView("MoreShift", ScheduleModel);
        }

        public ActionResult GetRegion(int selectedregion)
        {
            var data = _dashboard.GetRegionvalue(selectedregion);
            return Json(data);
        }

        public IActionResult ReturnShift(int status, int shiftdetailid)
        {
            string email = HttpContext.Session.GetString("email");

            int Aspid = _context.Aspnetusers.FirstOrDefault(x => x.Email == email).Id;
            _dashboard.SetReturnShift(status, shiftdetailid, Aspid);
            return Ok();
        }

        public IActionResult EditShiftDetails(ShiftDetailsmodal shiftDetailsmodal)
        {
            string email = HttpContext.Session.GetString("email");

            int Aspid = _context.Aspnetusers.FirstOrDefault(x => x.Email == email).Id;
            if (_dashboard.SetEditShift(shiftDetailsmodal, Aspid))
            {
                return Ok(true);
            }
            return Ok(false);
        }   

        public IActionResult DeleteShift(int shiftdetailid)
        {
            string email = HttpContext.Session.GetString("email");

            int Aspid = _context.Aspnetusers.FirstOrDefault(x => x.Email == email).Id;
            _dashboard.SetDeleteShift(shiftdetailid, Aspid);
            return Ok();
        }

        public IActionResult InvoicingShared()
        {
            var Phyid = HttpContext.Session.GetInt32("physicianId");

            Invoicingcm invoicingcm = new Invoicingcm() {
                dates = _providerDashboard.GetDates(),
                PhysicianId=Convert.ToInt32(Phyid),
            };
         
            return PartialView("ProviderShared/InvoicingShared",invoicingcm);
        }

        public IActionResult GetInvoicingDataonChangeOfDate(string selectedValue, int PhysicianId)
        {
           

            var AdminID = HttpContext.Session.GetInt32("AdminId");
            string[] dateRange = selectedValue.Split('*');
            DateOnly startDate = DateOnly.Parse(dateRange[0]);
            DateOnly endDate = DateOnly.Parse(dateRange[1]);
            Invoicingcm model = _generalService.GetInvoicingDataonChangeOfDate(startDate, endDate, PhysicianId,Convert.ToInt32(AdminID));
            return PartialView("ProviderShared/InvoicingPartialView", model);
        }

        public IActionResult GetUploadedDataonChangeOfDate(string selectedValue, int PhysicianId, int pageNumber, int pagesize)
        {
            string[] dateRange = selectedValue.Split('*');
            DateOnly startDate = DateOnly.Parse(dateRange[0]);
            DateOnly endDate = DateOnly.Parse(dateRange[1]);
            Invoicingcm model = _generalService.GetUploadedDataonChangeOfDate(startDate, endDate, PhysicianId, pageNumber, pagesize);
            return PartialView("ProviderShared/TimeSheetReiembursementPartialView", model);
        }

        public IActionResult BiWeeklyTimesheet(string selectedValue, int PhysicianId)
        {
            int? AdminID = HttpContext.Session.GetInt32("AdminId");
            
            string[] dateRange = selectedValue.Split('*');
            DateOnly startDate = DateOnly.Parse(dateRange[0]);
            DateOnly endDate = DateOnly.Parse(dateRange[1]);
            Invoicingcm model = _generalService.getDataOfTimesheet(startDate, endDate, PhysicianId, AdminID);
            return PartialView("ProviderShared/BiWeeklyTimesheet", model);
        }

        [HttpPost]
        public IActionResult HandleSubmitTimeSheet(Invoicingcm model, string command)
        {
            //if (command == "Aproove")
            //{
            //    return AprooveTimeSheet(model);
            //}
            //else
            //{
                return SubmitTimeSheet(model);
            //}
        }

        [HttpPost]
        public IActionResult SubmitTimeSheet(Invoicingcm model)
        {
            int? AdminID = HttpContext.Session.GetInt32("AdminId");
            _generalService.SubmitTimeSheet(model, model.PhysicianId);
            //TempData["successrequest"] = "TimeSheet Saved Succesfully";
            //if (AdminID == null)
            //{
            //    return RedirectToAction("Invoicing", "Provider");
            //}
            //else
            //{
            //    return RedirectToAction("Invoicing", "Admin");
            //}
            return Ok();
        }

        public IActionResult DeleteBill(int id, DateOnly startDate, DateOnly endDate)
        {
            _generalService.DeleteBill(id);           
            return Ok();
        }

        [HttpPost]
        public IActionResult FinalizeTimeSheet(int id)
        {
            _generalService.FinalizeTimeSheet(id);

            return Ok();
        }

        public IActionResult LogoutSession()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("jwt");
            return RedirectToAction("patientLogin", "Home");
        }

        public IActionResult ChatWithAdmin(int providerId, int adminId, int requestId, int flag, int roleId)
        {
           var chats=_adminDash.GetChats(providerId, adminId, requestId, flag, roleId);
            return PartialView("ProviderShared/ChatShared",chats);
        }

        //[HttpPost]
        //public void PostChats(Chatcm cm)
        //{
        //    _adminDash.SaveChats(cm);
        //}

    }
}