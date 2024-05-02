using Business_Logic.Interface;
using Data_Layer.DataContext;
using Data_Layer.DataModels;
using Data_Layer.CustomModels;
using HalloDoc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using static HelloDocMvc.Repository.Repositories.AuthManager;

namespace HalloDoc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;      
        private readonly ILogin _login;
        private readonly IPatientReq _patient;
        private readonly IConciergeReq _concierge;
        private readonly IFamilyFriendReq _familyFriend;
        private readonly IBusinessReq _business;
        private readonly ApplicationDbContext _context;
        private readonly IsubmitInfoMe _submitInfoMe;
        private readonly IpatientProfile _profile;
        private readonly IsomeoneElse _someelse;
        private readonly IConfiguration _configuration;
        private readonly IjwtService _jwtService;
        private readonly IpatientDashboard _dash;
        private readonly IAdminDashboard _dashboard;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [ActivatorUtilitiesConstructor]
        public HomeController(ILogin login, IPatientReq patient, ApplicationDbContext context, IConciergeReq concierge, IFamilyFriendReq familyFriendReq, IBusinessReq businessReq, IsubmitInfoMe isubmitInfoMe, IpatientProfile profile, IsomeoneElse someelse, IConfiguration configuration, IjwtService jwtService, IpatientDashboard dash, IAdminDashboard dashboard)
        {
            _login = login;
            _patient = patient;
            _context = context;
            _concierge = concierge;
            _familyFriend = familyFriendReq;
            _business = businessReq;
            _submitInfoMe = isubmitInfoMe;
            _profile = profile;
            _someelse = someelse;
            _configuration = configuration;
            _jwtService = jwtService;
            _dash = dash;
            _dashboard = dashboard;
        }

        public IActionResult Index()
        {
            @ViewBag.Admin = 1;
            return View();
        }

        public IActionResult patientLogin()
        {
            @ViewBag.Admin = 1;
            return View();
        }

    [AllowAnonymous]
    [HttpPost]
    public IActionResult patientLogin(patientLogincm logincm)
    {
            var check = _context.Aspnetusers.FirstOrDefault(x => x.Email == logincm.Email);

            if (check != null) { 

                int id = _context.Aspnetusers.FirstOrDefault(x => x.Email == logincm.Email).Id;
           
                var firstname = _context.Aspnetusers.FirstOrDefault(x => x.Email == logincm.Email).Username;

                HttpContext.Session.SetString("fname", firstname);
                HttpContext.Session.SetString("email", logincm.Email);
                HttpContext.Session.SetInt32("reqId", id);

                var user = _login.login(logincm);

                if (user != null)
                {
                    HttpContext.Session.SetInt32("roleId", user.roleId);
                    HttpContext.Session.SetInt32("physicianId", Convert.ToInt32(user.Physicianid));
                    if (user.AdminId != 0)
                    {
                        HttpContext.Session.SetInt32("AdminId", Convert.ToInt32(user.AdminId));
                    }

                    IActionResult response = Unauthorized();

                    if (user != null)
                    {
                        var jwtToken = _jwtService.GenerateJwtToken(user);
                        Response.Cookies.Append("jwt", jwtToken);
                        HttpContext.Session.SetString("user", user.Role);
                        TempData["name"] = user.Username;

                        if (user.Role == "User")
                        {
                            TempData["success"] = "Login Successfully!";
                            return RedirectToAction("patientDashboard", new { emaill = logincm.Email });
                        }
                        if (user.Role == "Admin")
                        {
                            TempData["success"] = "Login Successfully!";
                            return RedirectToAction("dashboard", "Admin");
                        }
                        if (user.Role == "Provider")
                        {
                            TempData["success"] = "Login Successfully!";
                            return RedirectToAction("ProviderDashboardMain", "Provider");
                        }
                    }
                    else
                    {
                        ViewBag.AuthFailedMessege = "Please Enter Correct Username And Password!";
                    }
                    return View();
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return NotFound();
            }
        }

        public IActionResult logout()
        {
            @ViewBag.Admin = 1;
            HttpContext.Session.Clear();
            Response.Cookies.Delete("jwt");

            return RedirectToAction("patientLogin");
        }

        public IActionResult forgotPass(string msg)
        {
            @ViewBag.Admin = 1;
            if (msg != null)
            {
                TempData["success"] = msg;
            }
            return View();
        }

        [HttpPost]
        public IActionResult resetPass(patientLogincm cm)
        {
           _dash.resetPassword(cm);
            
            return RedirectToAction("forgotPass", new {msg= "Email Sent To Your EmailId" });
        }

        public IActionResult submitReq()
        {
            @ViewBag.Admin = 1;
            return View();
        }

        public IActionResult patientReq()
        {
            @ViewBag.Admin = 1;        
            return View();
        }

        [HttpPost]
        public IActionResult patientReq(PatientRequestCm obj)
        {
            _patient.patientReq(obj);
            return RedirectToAction("submitReq");


        }

        public IActionResult checkEmailAvailibility(string email) //action
        {
            
            if (email != null)
            {
                Aspnetuser? exist = _context.Aspnetusers.FirstOrDefault(u => u.Email == email);
                if (exist != null)
                {
                    return Json(new { code = 401 });
                }
            }
            return Json(new { code = 402 });
        }

        public IActionResult familyFriendReq()
        {
            @ViewBag.Admin = 1;
            return View();
        }

        [HttpPost]
        public IActionResult familyFriendReq(familyFriendReqcm obj)
        {
            _familyFriend.familyFriend(obj);
            return RedirectToAction("submitReq");
        }

        public IActionResult concierge()
        {
            @ViewBag.Admin = 1;
            return View();
        }
  
        [HttpPost]
        public IActionResult concierge(ConciergeRequestCm cm)
        {
            _concierge.conciergeReq(cm);
            return RedirectToAction("submitReq");
        }

        public IActionResult business()
        {
            @ViewBag.Admin = 1;
            return View();
        }

        [HttpPost]
        public IActionResult business(businessReqcm cm)
        {
            _business.businessReq(cm);
            return RedirectToAction("submitReq");
        }
        
        public IActionResult patientSite()
        {
            @ViewBag.Admin = 1;
            return View();
        }

        [CustomAuthorize("User")]
        public IActionResult patientDashboard(string emaill)
        {
            @ViewBag.Admin = 1;
            var uid = _context.Users.Where(r => r.Email == emaill).Select(x => x.Userid).First();
            var request = _context.Requests.Where(r => r.Userid == uid).AsNoTracking();
            var RequestList = request.Select(r => new patientDashboard()
            {
                Createddate = r.Createddate,
                Status = r.Status,
                doc_Count = r.Requestwisefiles.Where(r=>r.Isdeleted==null).Select(f => f.Requestid).Count(),
                Requestid = r.Requestid,
                Email = emaill,

            }).ToList();

            return View(RequestList);
        }

        public IActionResult AccessDenied()
        {
            @ViewBag.Admin = 1;
            return View();
        }

        public IActionResult submitInfoMe()
        {
            @ViewBag.Admin = 1;
            string email = HttpContext.Session.GetString("email");
            var mailId = _submitInfoMe.getInfo(email);
            return View(mailId);
        }

        [HttpPost]
        public IActionResult submitInfoMe(submitReqMe obj)
        {
            _submitInfoMe.submitMe(obj);
            return RedirectToAction("patientDashboard", new {emaill=obj.Email});
        }

        public IActionResult submitReqSomeElse()
        {
            @ViewBag.Admin = 1;
            return View();
        }

        [CustomAuthorize("User")]
        [HttpPost]
        public IActionResult submitReqSomeElse(familyFriendReqcm cm)
        {
            string emaill = HttpContext.Session.GetString("email");
            _someelse.someElse(cm, emaill);
            return RedirectToAction("patientDashboard", new {emaill=emaill});
        }

        [CustomAuthorize("User")]
        public IActionResult viewDocuments(int data)
        {
            @ViewBag.Admin = 1;
            var query = from rw in _context.Requestwisefiles
                        join r in _context.Requestclients on rw.Requestid equals r.Requestid
                        where r.Requestid == data && rw.Isdeleted==null
                        select new { rw.Createddate, rw.Filename, r.Requestid, r.Firstname, r.Lastname };

            var RequestList = query.Select(r => new viewDocument.viewDoc()
            {
                Createddate = r.Createddate,
                Requestid = r.Requestid,
                Firstname = r.Firstname,
                Filename = r.Filename
            }).ToList();

            var Request = new viewDocument()
            {
                Requestid = data,
                viewDocs = RequestList,
                Firstname=_context.Requestclients.FirstOrDefault(x=>x.Requestid==data).Firstname,
            };


            return View(Request);
        }

        [CustomAuthorize("User")]
        [HttpPost]
        public IActionResult viewDocuments(viewDocument obj)
        {
            string filename = obj.Upload.FileName;
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Documents", filename);
            IFormFile file = obj.Upload;

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            var data3 = new Requestwisefile()
            {
                Requestid = obj.Requestid,
                Filename = filename,
            };

            _context.Requestwisefiles.Add(data3);
            _context.SaveChanges();

            return RedirectToAction("viewDocuments", new {data=obj.Requestid});
        }

        public IActionResult patientProfile()
        {
            @ViewBag.Admin = 1;
            string email = HttpContext.Session.GetString("email");
            var result = _context.Users.Where(u => u.Email == email).FirstOrDefault();


            return View(result);
        }

        [HttpPost]
        public IActionResult patientProfile(User cm)
        {
            _profile.update(cm);
            return RedirectToAction("PatientDashboard",new {emaill=cm.Email});
        }
      
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult reviewAgreement(int reqId)
        {
            @ViewBag.Admin = 1;
            var id= _dash.review(reqId);
            return View(id);
        }

        [HttpPost]
        public IActionResult agree(ReviewAgreement cm)
        {
            var isAgreed=_dash.iAgree(cm);
            if (isAgreed)
            {
                TempData["success"] = "Agreement Signed Successfully!";
                return RedirectToAction("patientLogin");
            }
            return RedirectToAction("statusError");
        }

        public IActionResult statusError()
        {
            return View();
        }

        [HttpPost]
        public IActionResult cancelAgreement(ReviewAgreement cm)
        {
           var cancelled= _dash.cancelAgree(cm);

            if (cancelled)
            {
                TempData["success"] = "Agreement Declined Successfully!";
                return RedirectToAction("patientLogin");
            }
            return RedirectToAction("statusError");
        }

        public IActionResult createAccount(int aspnetId, int flag)
        {
            var data=_dash.createAccount(aspnetId, flag);

            return View(data);
        }

        [HttpPost]
        public IActionResult createAccount(patientDashboard cm) {
          _dash.account(cm.create);

            return RedirectToAction("patientLogin");
        }

    }
}