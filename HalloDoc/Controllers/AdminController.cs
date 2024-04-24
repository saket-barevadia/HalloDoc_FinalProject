using Microsoft.AspNetCore.Mvc;
using Business_Logic.Interface;
using Data_Layer.DataContext;
using Data_Layer.DataModels;
using Data_Layer.CustomModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Cryptography.X509Certificates;
using static HelloDocMvc.Repository.Repositories.AuthManager;
using System.Text;
using static Data_Layer.CustomModels.ProviderMenucm;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Components.Forms;
using System.Drawing;

namespace HalloDoc.Controllers
{
    [CustomAuthorize("Admin")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IAdminDashboard _dashboard;
        private readonly IviewCase _viewCase;
        private readonly IviewNotes _viewNotes;
        private readonly ApplicationDbContext _context;
        private readonly IcancelCase _cancelCase;
        private readonly IProviderMenu _providerMenu;


        public AdminController(ILogger<AdminController> logger,IAdminDashboard dashboard, IviewCase viewCase, IviewNotes viewNotes, ApplicationDbContext context, IcancelCase cancelCase, IProviderMenu providerMenu)
        {
            _dashboard = dashboard;
            _viewCase = viewCase;
            _viewNotes = viewNotes;
            _context = context;
            _cancelCase = cancelCase;
            _providerMenu = providerMenu;
            _logger = logger;
        }


        [CustomAuthorize("Admin", "Dashboard")]

        public IActionResult dashboard(int reqTypeId, int regionID)
        {
            ViewBag.Admin = 2;
            var roleMain = HttpContext.Session.GetInt32("roleId");
            List<string> roleMenu = _dashboard.GetListOfRoleMenu((int)roleMain);
            ViewBag.Menu = roleMenu;
        
            var query = _dashboard.data(reqTypeId,regionID);
            int count = query.Count();

            HttpContext.Session.SetInt32("count", count);
            return View(query);
        }


        public IActionResult viewCase(int id, int flag)
         {
            
                @ViewBag.Admin = 2;
                var roleMain = HttpContext.Session.GetInt32("roleId");
                List<string> roleMenu = _dashboard.GetListOfRoleMenu((int)roleMain);
                ViewBag.Menu = roleMenu;

                if (!_dashboard.checkreq(id))
                {
                    @ViewBag.Admin = 2;
                    return View("invalid");
                };

                var data = _viewCase.viewCase(id, flag);
            if (data == null)
            {
                return NotFound();
            }
            else
            {
                return View(data);
            }
               
            
            

        }


        [HttpPost]
        public IActionResult viewCase(viewCaseCm cm)
        {
            _viewCase.viewCaseUpdate(cm);
            return RedirectToAction("viewCase", new { id = cm.Requestid });
        }


        public IActionResult viewNotes(int id,int flag)
        {
            @ViewBag.Admin = 2;
            var roleMain = HttpContext.Session.GetInt32("roleId");
            List<string> roleMenu = _dashboard.GetListOfRoleMenu((int)roleMain);
            ViewBag.Menu = roleMenu;

            if (!_dashboard.checkreq(id))
            {
                @ViewBag.Admin = 2;
                return View("invalid");
            };

            var data = _viewNotes.addNote(id,flag);
            if (data != null)
            {
                return View(data);
            }

            return View();
        }


        [HttpPost]
        public IActionResult viewNotes(Requestnote cm, int id,int flag)
        {
            _viewNotes.viewNote(cm, id);

           

            TempData["success"] = "Data Updated Successfully";
            return RedirectToAction("viewNotes", new { id = id ,flag=flag });

        }



        public IActionResult cancelCase()
        {
            var data = _cancelCase.caseTag();

            return Json(data.ToList());

        }

        [HttpPost]
        public IActionResult cancelCase(int id, int reason, string note)
        {
            _cancelCase.cancelCase(id, reason, note);
            TempData["success"] = "Case Cancelled Successfully";
            return View();

        }


        [HttpPost]
        public void blockCase(int requestId, string reasonNote)
        {
            _dashboard.blockCase(requestId, reasonNote);
            TempData["success"] = "Patient Blocked Successfully";

        }


        public IActionResult assignCase(int requestId)
        {
            HttpContext.Session.SetInt32("reqId", requestId);
            var data = _dashboard.assigncases(requestId);
            return PartialView(data);
        }


        public IActionResult getPhysician(int id)
        {
            var data = _dashboard.getPhysicianName(id);

            return Json(data);
        }


        [HttpPost]
        public void assignCase(int regionId, int physicianId, string description)
        {
            try
            {
                int reqId = Convert.ToInt32(HttpContext.Session.GetInt32("reqId"));

                


                _dashboard.assignCasePost(reqId, regionId, physicianId, description);
                TempData["success"] = "Case Assigned Successfully";
            }
            catch
            {
                TempData["error"] = "Error Ocurred"; ;
            }
            

        }



        public IActionResult viewUploads(int reqId,int flag)
        {
            
                @ViewBag.Admin = 2;
                var roleMain = HttpContext.Session.GetInt32("roleId");
                List<string> roleMenu = _dashboard.GetListOfRoleMenu((int)roleMain);
                ViewBag.Menu = roleMenu;
                if (!_dashboard.checkreq(reqId))
                {
                    @ViewBag.Admin = 2;
                    return View("invalid");
                };

                var data = _dashboard.viewUploads(reqId, flag);
            if (data == null)
            {
                return NotFound();
            }
            else
            {
                return View(data);
            }
         
        }


        [HttpPost]
        public IActionResult viewUploads(ViewUploads cm)
        {
            _dashboard.postUploads(cm);
            TempData["success"] = "File Uploaded Successfully";
            return RedirectToAction("viewUploads", new { reqId = cm.Requestid,flag=cm.flag});
        }


        public IActionResult delete(int id, int reqId,int flag)
        {
            _dashboard.deleteUploads(id);
            return RedirectToAction("viewUploads", new { reqId = reqId,flag=flag });
        }


        public IActionResult orders(int id)
        {
            @ViewBag.Admin = 2;
            var roleMain = HttpContext.Session.GetInt32("roleId");
            List<string> roleMenu = _dashboard.GetListOfRoleMenu((int)roleMain);
            ViewBag.Menu = roleMenu;

            if (!_dashboard.checkreq(id))
            {
                @ViewBag.Admin = 2;
                return View("invalid");
            };

            var data = _dashboard.getProfessions(id);
            return View(data);
        }

        public IActionResult getBusiness(int id)
        {
            var business = _dashboard.getVendor(id);

            return Json(business);
        }

        public IActionResult getVendorData(int vendorID)
        {
            HttpContext.Session.SetInt32("vendorId", vendorID);
            var vendorDetails = _dashboard.getVendorDetails(vendorID);
            return Json(vendorDetails);
        }


        [HttpPost]
        public IActionResult orders(Orders cm)
        {
            var id = Convert.ToInt32(HttpContext.Session.GetInt32("vendorId"));

            var ans=_dashboard.postOrderDetails(cm, id);
            if (ans == true)
            {
                TempData["success"] = "Order Sent Successfully";
                return RedirectToAction("orders", (new { id = cm.Requestid }));
            }
            else
            {
                TempData["error"] = "Something Went Wrong";
                return NotFound();
            }
           
        }


        public IActionResult transferRequest(int id)
        {
            HttpContext.Session.SetInt32("reqIdd", id);
            var regions = _dashboard.transferRequests(id);
            return PartialView(regions);
        }

        [HttpPost]
        public void transferRequest(int regionId, int physicianId, string description)
        {
            try
            {
                int reqId = Convert.ToInt32(HttpContext.Session.GetInt32("reqIdd"));
                string email = HttpContext.Session.GetString("email");

                _dashboard.transferReqPost(reqId, regionId, physicianId, description, email);
                TempData["success"] = "Request Transfered Successfully";
            }
            catch
            {
                TempData["error"] = "Something Went Wrong";
            }
        }


        [HttpPost]
        public IActionResult sendEmail(ViewUploads cm)
        {
            _dashboard.sendMail(cm.Requestid);
            TempData["success"] = "Email Sent Successfully";
            return RedirectToAction("viewUploads", new { reqId = cm.Requestid,flag=cm.flag });
        }


        public IActionResult clearcase(int id)
        {
            HttpContext.Session.SetInt32("id", id);
            return View();
        }

        [HttpPost]
        public IActionResult clearcase()
        {
            
                int id = Convert.ToInt32(HttpContext.Session.GetInt32("id"));
               var check=_dashboard.clearCasePost(id);
            if (check == true)
            {
                TempData["success"] = "Case Cleared Successfully";
                return RedirectToAction("dashboard");
            }
            else
            {
                TempData["error"] = "Something Went Wrong";
                return NotFound();
            }
            
        }



        public IActionResult sendAgreement(int Requestid)
        {

            var data = _dashboard.SendAgreement(Requestid);

            if (data == null)
            {
                return NotFound();
            }
            else
            {
                return View(data);
            }
           
            
        }


        [HttpPost]
        public IActionResult sendAgreement(SendAgreement cm)
        {
            try
            {
                _dashboard.sendEmail(cm);
                TempData["success"] = "Agreement Sent Successfully";
                return RedirectToAction("dashboard");
            }
            catch
            {
                TempData["error"] = "Something Went Wrong";
                return NotFound();
            }
        }


        public IActionResult closeCase(int requestid)
        {
           
                @ViewBag.Admin = 2;
                var roleMain = HttpContext.Session.GetInt32("roleId");
                List<string> roleMenu = _dashboard.GetListOfRoleMenu((int)roleMain);
                ViewBag.Menu = roleMenu;

                if (!_dashboard.checkreq(requestid))
                {
                    @ViewBag.Admin = 2;
                    return View("invalid");
                };

                var patientDetails = _dashboard.CloseCase(requestid);
            if(patientDetails == null)
            {
                return NotFound();
            }
            else
            {
                return View(patientDetails);
            }
                
           
        }


        [HttpPost]
        public IActionResult closeCase(CloseCase cm)
        {
            _dashboard.closeCasePost(cm);
            TempData["success"] = "Data Updated Successfully";
            return RedirectToAction("closeCase", new { requestid = cm.Requestid });
        }

        [HttpPost]
        public void close(int reqId)
        {
            _dashboard.close(reqId);
        }


        [CustomAuthorize("Admin", "My Profile")]
        public IActionResult adminMyProfile(int flag,int aspnetuserId)
        {
            @ViewBag.Admin = 2;
            var roleMain = HttpContext.Session.GetInt32("roleId");
            List<string> roleMenu = _dashboard.GetListOfRoleMenu((int)roleMain);
            ViewBag.Menu = roleMenu;
            if (flag == 1)
            {
                string email = HttpContext.Session.GetString("email");
                var admionDetails = _dashboard.adminData(email,flag);
                return View(admionDetails);
            }
            if (flag == 2)
            {
                string email = _context.Aspnetusers.FirstOrDefault(x => x.Id == aspnetuserId).Email;
                var admionDetails = _dashboard.adminData(email,flag);
                return View(admionDetails);
            }
            return null;
           
        }


        [HttpPost]
        public IActionResult adminMyProfile(AdminProfilecm cm,List<int> adminRegions)
        {
            if (cm.Flag == 1)
            {
                _dashboard.resetPass(cm);
            }
            else if (cm.Flag == 2)
            {
                _dashboard.EditadminProfile(cm, adminRegions);
                HttpContext.Session.SetString("fname", cm.Firstname);

            }
            else if (cm.Flag == 3)
            {
                _dashboard.editAdminAddress(cm);
            }

            TempData["success"] = "Profile Updated Successfully";

            return RedirectToAction("adminMyProfile", new { flag =cm.back, aspnetuserId =cm.AspnetUserId});
        }


        public IActionResult encounter(int reqId)
        {
            try
            {
                @ViewBag.Admin = 2;
                var roleMain = HttpContext.Session.GetInt32("roleId");
                List<string> roleMenu = _dashboard.GetListOfRoleMenu((int)roleMain);
                ViewBag.Menu = roleMenu;

                if (!_dashboard.checkreq(reqId))
                {
                    @ViewBag.Admin = 2;
                    return View("invalid");
                };
                var encounterData = _dashboard.encounterFormData(reqId);
                return View(encounterData);
            }
            catch
            {
                return NotFound();
            }
        }


        [HttpPost]
        public IActionResult encounter(Encounter cm)
        {

            if (cm == null)
            {
                TempData["error"] = "Something Went Wrong";
                return NotFound();
            }
            else
            {
                _dashboard.encounterFormPost(cm);
                TempData["success"] = "Data Updated Successfully";
                return RedirectToAction("encounter", new { reqId = cm.Requestid });
            }
           
        }


        public IActionResult sendLink()
        {
            return View();
        }

        [HttpPost]
        public IActionResult sendLink(SendAgreement cm)
        {
            try
            {
                _dashboard.sendLink(cm);
                TempData["success"] = "Email Sent Successfully";
                return RedirectToAction("dashboard");
            }
            catch
            {
                TempData["error"] = "Something Went Wrong";
                return NotFound();
            }
        }


        public IActionResult createRequest()
        {
            
                @ViewBag.Admin = 2;
                var roleMain = HttpContext.Session.GetInt32("roleId");            
                List<string> roleMenu = _dashboard.GetListOfRoleMenu((int)roleMain);
                ViewBag.Menu = roleMenu;
                var regions = new AdminCreateReq()
                {
                    regions = _dashboard.getRegions(),                 
                };


                return View(regions);
            }
           

              
                  


        [HttpPost]
        public IActionResult createRequest(AdminCreateReq cm)
        {
            
                string email = HttpContext.Session.GetString("email");
                _dashboard.adminCreateReq(cm, email);
                TempData["success"] = "Request Created Successfully";
                return RedirectToAction("createRequest");
           
        }

      


        public IActionResult Export(string GridHtml)
        {
            return File(Encoding.ASCII.GetBytes(GridHtml), "application/vnd.ms-excel", "ExportData.xls");
        }

        public IActionResult ExportAll()
        {
            int typeId = 0;
            int[] status = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var exportAll = _dashboard.GenerateExcelFile(_dashboard.data(0,0));
            return File(exportAll, "application/vnd.ms-excel", "ExportAll.xls");
        }

        public IActionResult logoutSession()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("jwt");
            return RedirectToAction("patientLogin", "Home");
        }

        public IActionResult getRegionss()
        {
            var regions = _dashboard.getRegions();
            return Json(regions);
        }

        public IActionResult newDash(int reqTypeId, int regionID)
        {
            var query = _dashboard.data(reqTypeId,regionID);
            int count = query.Count();

            HttpContext.Session.SetInt32("count", count);
            return PartialView(query);
        }

         public IActionResult pending(int reqTypeId, int regionID)
        {
            var query = _dashboard.data(reqTypeId,regionID);
            int count = query.Count();

            HttpContext.Session.SetInt32("count", count);
            return PartialView(query);
        }

         public IActionResult Active(int reqTypeId, int regionID)
        {
            var query = _dashboard.data(reqTypeId,regionID);
            int count = query.Count();

            HttpContext.Session.SetInt32("count", count);
            return PartialView(query);
        }

         public IActionResult conclude(int reqTypeId, int regionID)
        {
            var query = _dashboard.data(reqTypeId,regionID);
            int count = query.Count();

            HttpContext.Session.SetInt32("count", count);
            return PartialView(query);
        }

          public IActionResult toClose(int reqTypeId, int regionID)
        {
            var query = _dashboard.data(reqTypeId,regionID);
            int count = query.Count();

            HttpContext.Session.SetInt32("count", count);
            return PartialView(query);
        }

          public IActionResult unpaid(int reqTypeId, int regionID)
        {
            var query = _dashboard.data(reqTypeId,regionID);
            int count = query.Count();

            HttpContext.Session.SetInt32("count", count);
            return PartialView(query);
        }




        // Providers

        [CustomAuthorize("Admin", "Providers")]
        public IActionResult providerMenu(int regionId)
        {
            @ViewBag.Admin = 2;
            var roleMain = HttpContext.Session.GetInt32("roleId");
            List<string> roleMenu = _dashboard.GetListOfRoleMenu((int)roleMain);
            ViewBag.Menu = roleMenu;
            var providerData = _providerMenu.providerDetails(regionId);
            return View(providerData);
        }

        public IActionResult providerMenuShared(int regionId)
        {
            var providerData = _providerMenu.providerDetails(regionId);
            return View(providerData);
        }

        public IActionResult editProvider(int phyId,int flag)
        {
            @ViewBag.Admin = 2;
            var roleMain = HttpContext.Session.GetInt32("roleId");
            List<string> roleMenu = _dashboard.GetListOfRoleMenu((int)roleMain);
            ViewBag.Menu = roleMenu;

            var physicianData=_providerMenu.providerProfile(phyId,flag);

            return View(physicianData);
        }

        [HttpPost]
        public IActionResult editProvider(ProviderMenucm.ProviderProfile cm, List<int> physicianRegions, int back)
        {
            if(cm.Flag==1)
            {
                _providerMenu.editProvider(cm);
                TempData["success"] = "Data Updated Successfully";
            }
            else if(cm.Flag==2)
            {
                _providerMenu.editphysicianInfo(cm,physicianRegions);
                TempData["success"] = "Data Updated Successfully";
            } 
            else if(cm.Flag==3)
            {
                _providerMenu.editMailingForm(cm);
                TempData["success"] = "Data Updated Successfully";
            }
            else if(cm.Flag==4)
            {
                _providerMenu.editproviderInfo(cm);
                TempData["success"] = "Data Updated Successfully";
            }
            else if(cm.Flag==5)
            {
                 _providerMenu.EditOnBoardingData(cm);
                TempData["success"] = "File Uploaded Successfully";
            } 
            else if(cm.Flag==6)
            {
                 _providerMenu.removePhysician(cm); 
                TempData["success"] = "Account Deleted Successfully";
                return RedirectToAction("providerMenu");
            }
            
            return RedirectToAction("editProvider", new { phyId = cm.Physicianid,flag=back });

        }


        [HttpPost]

        public void resetPass(ProviderMenucm.ProviderProfile cm)
        {
            _providerMenu.resetPassword(cm);  
        }


        [HttpPost]
        public void contactProvider(ProviderMenucm cm)
        {
            try
            {
                string emaill = HttpContext.Session.GetString("email");
                _providerMenu.sendMail(cm, emaill);
               // return View();
        }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while downloading files.");
                //return StatusCode(405, "An error occurred while downloading files.");
            }


        }


        [HttpPost]
        public IActionResult stopNotification(int phyId)
        {
            try
            {
                var notification = _providerMenu.stopNotification(phyId);

                return Json(notification);
            }
            catch
            {
                return NotFound();
            }
           

        }


        public IActionResult createProviderAcc(int flag,int filterRole)
        {
          
                @ViewBag.Admin = 2;
                var roleMain = HttpContext.Session.GetInt32("roleId");
                List<string> roleMenu = _dashboard.GetListOfRoleMenu((int)roleMain);
                ViewBag.Menu = roleMenu;
                var data = _providerMenu.getRegions(flag, filterRole);

                return View(data);
            
            

        }

        [HttpPost]
        public IActionResult createProviderAcc(ProviderProfile cm, List<int> physicianRegions, int back)
        {
            var exist=_providerMenu.createProviderAcc(cm,physicianRegions);
            if (exist == true)
            {
                TempData["error"] = "Account Already Exist";
            }
            if(exist==false)
            {
                TempData["success"] = "Account Created Successfully";
            }
            if (back == 1)
            {
                return RedirectToAction("providerMenu");
            }
            if(back == 2)
            {
                return RedirectToAction("userAccess");
            }
            return null;
            
        }




        //Account Access

        [CustomAuthorize("Admin", "Account Access")]
        public IActionResult accountAccess()
        {
            @ViewBag.Admin = 2;
            var roleMain = HttpContext.Session.GetInt32("roleId");
            List<string> roleMenu = _dashboard.GetListOfRoleMenu((int)roleMain);
            ViewBag.Menu = roleMenu;
            var accountData=_providerMenu.accountAccessData();
            
            return View(accountData);
        }


        public IActionResult createAccess()
        {
            @ViewBag.Admin = 2;
            var roleMain = HttpContext.Session.GetInt32("roleId");
            List<string> roleMenu = _dashboard.GetListOfRoleMenu((int)roleMain);
            ViewBag.Menu = roleMenu;
            var createAccData = _providerMenu.createAccount();
            return View(createAccData);
        }


     
        public IActionResult filterRoles(int accountTypeId)
        {
            var menu = _providerMenu.getMenu(accountTypeId);

            var htmlcontent = "";

            foreach (var obj in menu.Menus)  
            {
                htmlcontent += $"<div class='form-check form-check-inline px-2 mx-3'><input class='form-check-input d2class' name='AccountMenu' type='checkbox' id='{obj.Menuid}' value='{obj.Menuid}'/><label class='form-check-label' for='{obj.Menuid}'>{obj.Name}</label></div>";
            }           

            return Content(htmlcontent);
        }

        [HttpPost]
        public IActionResult createAccess(ProviderMenucm cm, List<int> AccountMenu)
        {
            _providerMenu.accountAccessPost(cm, AccountMenu);
            TempData["success"] = "Access Created Successfully";
            return RedirectToAction("accountAccess");
        }


        public IActionResult editAccountAccess(int accTypeId, int roleId)
        {
            var data=_providerMenu.getEditAccAccess(accTypeId, roleId);
            @ViewBag.Admin = 2;

            var roleMain = HttpContext.Session.GetInt32("roleId");
            List<string> roleMenu = _dashboard.GetListOfRoleMenu((int)roleMain);
            ViewBag.Menu = roleMenu;
            return View(data);
        }

        public IActionResult filterEditRoles(int accountTypeId,int roleId)
        {
            var menu = _providerMenu.GetAccountMenu(accountTypeId, roleId); 

            var htmlcontent = "";

            foreach (var obj in menu.accountMenu)
            {
                htmlcontent += $"<div class='form-check form-check-inline px-2 mx-3'><input class='form-check-input d2class' {(obj.ExistsInTable ? "checked" : "")} name='AccountMenu' type='checkbox' id='{obj.menuid}' value='{obj.menuid}'/><label class='form-check-label' for='{obj.menuid}'>{obj.name}</label></div>";
            }

            return Content(htmlcontent);
        }


        [HttpPost]
        public IActionResult editAccountAccess(ProviderMenucm cm, List<int> AccountMenu)
        {
            _providerMenu.editAccAccessPost(cm, AccountMenu);
            TempData["success"] = "Access Edited Successfully";
            return RedirectToAction("editAccountAccess", new { accTypeId =cm.CreateAccountAccess.AccounttypeId, roleId =cm.CreateAccountAccess.Roleid});
        }


        public IActionResult deleteAccount(int roleId)
        {
            _providerMenu.DeleteAccountAccess(roleId);
            TempData["success"] = "Account Deleted Successfully";
            return RedirectToAction("accountAccess");
        }


        //User Access
        [CustomAuthorize("Admin", "User Access")]
        public IActionResult userAccess(int acccountTypeId)
        {
          var userAccessData=  _providerMenu.userAccess(acccountTypeId);
            @ViewBag.Admin = 2;
            var roleMain = HttpContext.Session.GetInt32("roleId");
            List<string> roleMenu = _dashboard.GetListOfRoleMenu((int)roleMain);
            ViewBag.Menu = roleMenu;
            return View(userAccessData);
        }

            public IActionResult userAccessShared(int acccountTypeId)
        {
          var userAccessData=  _providerMenu.userAccess(acccountTypeId);
            return View(userAccessData);
        }


        public IActionResult createAdminAccount()
        {
            var data = _providerMenu.getRegions(0,0);
            @ViewBag.Admin = 2;
            var roleMain = HttpContext.Session.GetInt32("roleId");
            List<string> roleMenu = _dashboard.GetListOfRoleMenu((int)roleMain);
            ViewBag.Menu = roleMenu;
            return View(data);
        }


        [HttpPost]
        public IActionResult createAdminAccount(ProviderProfile cm, List<int> physicianRegions)
        {
           var exist= _providerMenu.createAdminAccount(cm, physicianRegions);
            if (exist == true)
            {
                TempData["error"] = "Account Already Exist";
            }
            if (exist == false)
            {
                TempData["success"] = "Account Created Successfully";
            }
            return RedirectToAction("createAdminAccount");
        }



        //Provider Location

        [CustomAuthorize("Admin", "Provider Location")]
        public IActionResult ProviderLocation()
        {
            @ViewBag.Admin = 2;
            var roleMain = HttpContext.Session.GetInt32("roleId");
            List<string> roleMenu = _dashboard.GetListOfRoleMenu((int)roleMain);
            ViewBag.Menu = roleMenu;
            return View();
        }

        public IActionResult GetLocation()
        {
            List<Physicianlocation> getLocation = _providerMenu.GetPhysicianlocations();
            return Ok(getLocation);
        }




        //Scheduling

        [CustomAuthorize("Admin", "Scheduling")]
        public IActionResult scheduling()
        {
            @ViewBag.Admin = 2;
            var roleMain = HttpContext.Session.GetInt32("roleId");
            List<string> roleMenu = _dashboard.GetListOfRoleMenu((int)roleMain);
            ViewBag.Menu = roleMenu;
            var schedulingCm = _dashboard.GetRegions(0);
            return View(schedulingCm);
        }


        public IActionResult CreateNewShift()
        {
            var schedulingCm = _dashboard.GetRegions(0);
            return PartialView(schedulingCm);
        }

        [HttpPost]
        public IActionResult CreateNewShift(SchedulingCm cm)
        {
            string email = HttpContext.Session.GetString("email");
            if (_dashboard.createShift(cm.ScheduleModel, email))
            {
                TempData["success"] = "New Shift Created Successfully";
            }
            else
            {
                TempData["error"] = "Error Ocurred in Creating New Shift";
            }
            
            return RedirectToAction("scheduling");

        }


        [HttpPost]
        public IActionResult loadshift(string datestring, string sundaystring, string saturdaystring, string shifttype, int regionid)
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

                    return PartialView("MonthWiseShift", monthShift);

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
                    return PartialView("ViewShift", shift);

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

            int Aspid =_context.Aspnetusers.FirstOrDefault(x=>x.Email== email).Id;
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

        public IActionResult deleteShift(int shiftdetailid)
        {
            string email = HttpContext.Session.GetString("email");

            int Aspid = _context.Aspnetusers.FirstOrDefault(x => x.Email == email).Id;
            _dashboard.SetDeleteShift(shiftdetailid, Aspid);
            return Ok();
        }


        public IActionResult ShiftReview(int regionId, int callId)
        {
            @ViewBag.Admin = 2;
            var roleMain = HttpContext.Session.GetInt32("roleId");
            List<string> roleMenu = _dashboard.GetListOfRoleMenu((int)roleMain);
            ViewBag.Menu = roleMenu;
            var schedulingCm=_dashboard.GetShiftReview(regionId, callId);

            return View(schedulingCm);
        }
        
        public IActionResult ShiftReviewShared(int regionId, int callId)
        {          
            var schedulingCm=_dashboard.GetShiftReview(regionId, callId);

            return View(schedulingCm);
        }

        [HttpPost]
        public IActionResult ApproveShift(int[] shiftDetailsId)
        {
            string email = HttpContext.Session.GetString("email");

            int Aspid = _context.Aspnetusers.FirstOrDefault(x => x.Email == email).Id;

            _dashboard.ApproveSelectedShift(shiftDetailsId, Aspid);

            return Ok();
        }


        [HttpPost]
        public IActionResult DeleteSelectedShift(int[] shiftDetailsId)
        {
            string email = HttpContext.Session.GetString("email");

            int Aspid = _context.Aspnetusers.FirstOrDefault(x => x.Email == email).Id;

            _dashboard.DeleteShiftReview(shiftDetailsId, Aspid);

            return Ok();
        }


        public IActionResult MdsOnCall(int regionid)
        {
            @ViewBag.Admin = 2;
            var roleMain = HttpContext.Session.GetInt32("roleId");
            List<string> roleMenu = _dashboard.GetListOfRoleMenu((int)roleMain);
            ViewBag.Menu = roleMenu;
            var MdsCallModal = _dashboard.GetOnCallDetails(regionid);
            return View(MdsCallModal);
        }


        public IActionResult MdsOnCallShared(int regionid)
        {
            var MdsCallModal = _dashboard.GetOnCallDetails(regionid);
            return View(MdsCallModal);
        }

        //public IActionResult FilterAssignCase(int regionid)
        //{

        //    var physicians = _dashboard.GetPhysicians(regionid);



        //    return Json(physicians);
        //}




        //Vendors

        [CustomAuthorize("Admin", "Partners")]
        public IActionResult vendors(Vendor cm)
        {
            @ViewBag.Admin = 2;
            var roleMain = HttpContext.Session.GetInt32("roleId");
            List<string> roleMenu = _dashboard.GetListOfRoleMenu((int)roleMain);
            ViewBag.Menu = roleMenu;
            var vendorData = _providerMenu.GetVendors(0,cm);
            return View(vendorData);
        }

        public IActionResult vendorShared(int professionId, Vendor cm)
        {
            var vendorData = _providerMenu.GetVendors(professionId,cm);
            return View(vendorData);
        }

        public IActionResult addBusiness()
        {
            @ViewBag.Admin = 2;
            var roleMain = HttpContext.Session.GetInt32("roleId");
            List<string> roleMenu = _dashboard.GetListOfRoleMenu((int)roleMain);
            ViewBag.Menu = roleMenu;
            var profession = _providerMenu.getHealthProfessionals();
            return View(profession);
        }

        [HttpPost]
        public IActionResult addBusiness(Vendor cm)
        {
            _providerMenu.addBusiness(cm);
            TempData["success"] = "Business Added Successfully";
            return RedirectToAction("vendors");
        }


        [HttpPost]

        public IActionResult deleteBusiness(int vendorId)
        {
            _providerMenu.deleteBusiness(vendorId);
            TempData["success"] = "Business Deleted Successfully";
            return RedirectToAction("vendors");
        }


        public IActionResult editBusiness(int vendorId)
        {
            @ViewBag.Admin = 2;
            var roleMain = HttpContext.Session.GetInt32("roleId");
            List<string> roleMenu = _dashboard.GetListOfRoleMenu((int)roleMain);
            ViewBag.Menu = roleMenu;
            var vendorDetails=_providerMenu.getVendorDetails(vendorId);
            return View(vendorDetails);
        }


        [HttpPost]
        public IActionResult editBusiness(Vendor cm)
        {
            var editBusiness=_providerMenu.editBusiness(cm);

            if (editBusiness == true)
            {
                TempData["success"] = "Business Details Edited Successfully";
            }
            if(editBusiness==false)
            {
                TempData["error"] = "Error Occurred";
            }

            return RedirectToAction("editBusiness", new { vendorId = cm.Vendorid });
        }





        //Records

        [CustomAuthorize("Admin", "Records")]
        public IActionResult PatientHistory()
        {
            @ViewBag.Admin = 2;
            var roleMain = HttpContext.Session.GetInt32("roleId");
            List<string> roleMenu = _dashboard.GetListOfRoleMenu((int)roleMain);
            ViewBag.Menu = roleMenu;
            var patientDetails = _dashboard.patientRecords(null);
            return View(patientDetails);
        }

        [CustomAuthorize("Admin", "Records")]
        public IActionResult patientRecord(int userId)
        {
            @ViewBag.Admin = 2;
            var roleMain = HttpContext.Session.GetInt32("roleId");
            List<string> roleMenu = _dashboard.GetListOfRoleMenu((int)roleMain);
            ViewBag.Menu = roleMenu;
            var patientInfo=_dashboard.GetPatientData(userId);
            return View(patientInfo);
        }

        public IActionResult patientHistoryShared(patientRecordscm cm)
        {
            var patientDetails = _dashboard.patientRecords(cm);
            return View(patientDetails);
        }

        [CustomAuthorize("Admin", "Records")]
        public IActionResult searchRecords(patientRecordscm cm)
        {
            var patientReqData = _dashboard.getPatientReqData(null);
            @ViewBag.Admin = 2;
            var roleMain = HttpContext.Session.GetInt32("roleId");
            List<string> roleMenu = _dashboard.GetListOfRoleMenu((int)roleMain);
            ViewBag.Menu = roleMenu;
            return View(patientReqData);
        }

        public IActionResult searchRecordsShared(patientRecordscm cm)
        {
            var patientReqData = _dashboard.getPatientReqData(cm);
            return View(patientReqData);
        }


        [HttpPost]
        public IActionResult deletePermanently(int requestId)
        {
            _dashboard.deletePermanently(requestId);
            TempData["success"] = "Patient Request Deleted Successfully";

            return RedirectToAction("searchRecords");
        }


        [CustomAuthorize("Admin", "Records")]
        public IActionResult blockHistory(patientRecordscm cm)
        {

            @ViewBag.Admin = 2;
            var roleMain = HttpContext.Session.GetInt32("roleId");
            List<string> roleMenu = _dashboard.GetListOfRoleMenu((int)roleMain);
            ViewBag.Menu = roleMenu;
            var blockReqData = _dashboard.getBlockHistory(cm);
            return View(blockReqData);
        }

        public IActionResult blockHistoryShared(patientRecordscm cm)
        {
            var blockReqData = _dashboard.getBlockHistory(cm);
            return View(blockReqData);
        }


        [HttpPost]
        public IActionResult unblockPatient(int reqId)
        {
            _dashboard.unblockPatient(reqId);
            TempData["success"] = "Patient Unblocked Successfully";

            return RedirectToAction("blockHistory");
        }


        [CustomAuthorize("Admin", "Records")]
        public IActionResult emailLogs(patientRecordscm cm)
        {
            @ViewBag.Admin = 2;
            var roleMain = HttpContext.Session.GetInt32("roleId");
            List<string> roleMenu = _dashboard.GetListOfRoleMenu((int)roleMain);
            ViewBag.Menu = roleMenu;
            var emailLogs = _dashboard.emailLogs(cm);
            return View(emailLogs);
        }

        public IActionResult emailLogsShared(patientRecordscm cm)
        {
            var emailLogs = _dashboard.emailLogs(cm);
            return View(emailLogs);
        }


        [CustomAuthorize("Admin", "Records")]
        public IActionResult smsLogs(patientRecordscm cm)
        {
            @ViewBag.Admin = 2;
            var roleMain = HttpContext.Session.GetInt32("roleId");
            List<string> roleMenu = _dashboard.GetListOfRoleMenu((int)roleMain);
            ViewBag.Menu = roleMenu;
            var smsLogs = _dashboard.smsLogs(cm);
            return View(smsLogs);
        }
        public IActionResult smsLogsShared(patientRecordscm cm)
        {        
            var smsLogs = _dashboard.smsLogs(cm);
            return View(smsLogs);
        }

    }
}