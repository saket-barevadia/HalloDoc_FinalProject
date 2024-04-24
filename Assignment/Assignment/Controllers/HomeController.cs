using Assignment.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BusinessLayer.Interface;
using DataLayer.CustomModels;

namespace Assignment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRecords _records;

        public HomeController(ILogger<HomeController> logger, IRecords records)
        {
            _logger = logger;
            _records = records;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Records()
        {
            var recordsList=_records.GetRecords();
            return View(recordsList);
        }

        public IActionResult AddBookShared(int borrowerId)
        {
            var bookDetails=_records.GetBookDetails(borrowerId);
            return PartialView(bookDetails);
        }

     

        [HttpPost]
        public IActionResult BookFormPost(Recordscm recordscm)
        {
            bool isExist=_records.SaveBookForm(recordscm);

            if (isExist)
            {
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }
           
        }

        [HttpPost]
        public IActionResult DeleteRecord(int borrowerId)
        {
            bool isDelete=_records.DeleteBookRecord(borrowerId);

            if (isDelete)
            {
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}