using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Logic.Interface;
using Data_Layer.DataContext;
using Data_Layer.DataContext;
using Data_Layer.CustomModels;
using Data_Layer.DataModels;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;
using LinqKit;
using Hangfire.Dashboard;
using System.Collections;

namespace Business_Logic.LogicRepositories
{
    public class GeneralService : IGeneralService
    {
 
        private readonly IGenericRepository<WeeklyTimeSheet> _weeklyTimeSheetRepo;
        private readonly IGenericRepository<WeeklyTimeSheetDetail> _weeklyTimeSheetDetailRepo;
        private readonly IGenericRepository<PayRate> _payRateRepo;
        private readonly IGenericRepository<Shiftdetail> _shiftDetailrepo;
        private readonly ApplicationDbContext _context;

        public GeneralService(IGenericRepository<WeeklyTimeSheet> weeklyTimeSheetRepo, IGenericRepository<WeeklyTimeSheetDetail> weeklyTimeSheetDetailRepo, IGenericRepository<PayRate> payRateRepo,ApplicationDbContext context)
        {
            _weeklyTimeSheetRepo = weeklyTimeSheetRepo;
            _weeklyTimeSheetDetailRepo = weeklyTimeSheetDetailRepo;
            _payRateRepo = payRateRepo;
            _context = context;
        }

        public Invoicingcm getDataOfTimesheet(DateOnly startDate, DateOnly endDate, int? PhysicianId, int? AdminID)
        {
            Invoicingcm model = new Invoicingcm();
            model.startDate = startDate;
            model.endDate = endDate;
            model.differentDays = endDate.Day - startDate.Day;
            WeeklyTimeSheet weeklyTimeSheet = _weeklyTimeSheetRepo.GetFirstOrDefault(u => u.ProviderId == PhysicianId && u.StartDate == startDate && u.EndDate == endDate);
            Expression<Func<WeeklyTimeSheetDetail, bool>> whereClauseSyntax1 = PredicateBuilder.New<WeeklyTimeSheetDetail>();

            if (weeklyTimeSheet != null)
            {
                PayRate payRate = _payRateRepo.GetFirstOrDefault(u => u.PhysicianId == weeklyTimeSheet.ProviderId);
                whereClauseSyntax1 = x => x.TimeSheet!.ProviderId == PhysicianId && x.TimeSheet.StartDate == startDate && x.TimeSheet.EndDate == endDate;
                model.TimeSheetId = weeklyTimeSheet.TimeSheetId;
                var ExistingTimeshet = _weeklyTimeSheetDetailRepo.SelectWhereOrderBy(x => new Timesheet
                {
                    Date = x.Date,
                    NumberOfHouseCall = x.HouseCall ?? 0,
                    NumberOfPhoneConsults = x.PhoneConsult ?? 0,
                    Weekend = x.IsWeekendHoliday ?? false,
                    TotalHours = x.TotalHours ?? 0,
                    OnCallhours = x.OnCallHours ?? 0,
                    Amount = x.Amount ?? 0,
                    Items = x.Item,
                    BillName = x.Bill,
                    WeeklyTimesheetDeatilsId = x.TimeSheetDetailId,
                }, whereClauseSyntax1, x => x.Date);
                List<Timesheet> list = new List<Timesheet>();
                foreach (Timesheet item in ExistingTimeshet)
                {
                    model.shiftTotal += (item.TotalHours * payRate.Shift) ?? 0;
                    model.weekendTotal += item.Weekend == true ? (1 * payRate.NightShiftWeekend) ?? 0 : 0;
                    model.HouseCallTotal += (item.NumberOfHouseCall * payRate.HouseCall) ?? 0;
                    model.phoneconsultTotal += (item.NumberOfPhoneConsults * payRate.PhoneConsult) ?? 0;
                    list.Add(item);
                }
                model.timesheets = list;
                model.shiftPayrate = payRate.Shift ?? 0;
                model.weekendPayrate = payRate.NightShiftWeekend ?? 0;
                model.HouseCallPayrate = payRate.HouseCall ?? 0;
                model.phoneConsultPayrate = payRate.PhoneConsult ?? 0;
                model.GrandTotal = model.shiftTotal + model.weekendTotal + model.HouseCallTotal + model.phoneconsultTotal;
            }
            else
            {
                DateOnly currentDate = startDate;
                while (currentDate <= endDate)
                {
                    model.timesheets.Add(new Timesheet
                    {
                        Date = currentDate,

                    });
                    currentDate = currentDate.AddDays(1);
                }
            }
            model.startDate = startDate;
            model.endDate = endDate;
            model.PhysicianId = PhysicianId ?? 0;
            model.IsFinalized = weeklyTimeSheet == null ? false : true;
            model.isAdminSide = AdminID == 0 ? false : true;
            return model;

        }
        public void SubmitTimeSheet(Invoicingcm model, int? PhysicianId)
         {
            WeeklyTimeSheet existingWeekltTimesheet = _weeklyTimeSheetRepo.GetFirstOrDefault(u => u.ProviderId == PhysicianId && u.StartDate == model.startDate && u.EndDate == model.endDate);
            if (existingWeekltTimesheet == null)
            {
                WeeklyTimeSheet weeklyTimeSheet = new WeeklyTimeSheet();
                weeklyTimeSheet.StartDate = model.startDate;
                weeklyTimeSheet.EndDate = model.endDate;
                weeklyTimeSheet.Status = 1;
                weeklyTimeSheet.CreatedDate = DateTime.Now;
                weeklyTimeSheet.ProviderId = PhysicianId ?? 0;
                _weeklyTimeSheetRepo.Add(weeklyTimeSheet);


                BitArray deletedBit = new BitArray(new[] { false });

                foreach (var item in model.timesheets)
                {
                    
                    WeeklyTimeSheetDetail detail = new WeeklyTimeSheetDetail();
                    detail.Date = item.Date;
                    detail.NumberOfShifts = _context.Shiftdetails.Count(u => u.Shift.Physicianid == PhysicianId && DateOnly.FromDateTime(u.Shiftdate) == item.Date && u.Isdeleted == deletedBit); 
                    detail.TotalHours = item.TotalHours;
                    detail.IsWeekendHoliday = item.Weekend;
                    detail.HouseCall = item.NumberOfHouseCall;
                    detail.PhoneConsult = item.NumberOfPhoneConsults;
                    detail.OnCallHours = item.OnCallhours;
                    detail.TimeSheetId = weeklyTimeSheet.TimeSheetId;
                    if (item.Bill != null)
                    {
                        IFormFile newFile = item.Bill;
                        detail.Bill = newFile.FileName;
                        var filePath = Path.Combine("wwwroot", "Documents", "ProviderBills", PhysicianId + "-" + item.Date + "-" + Path.GetFileName(newFile.FileName));
                        using (FileStream stream = System.IO.File.Create(filePath))
                        {
                            newFile.CopyTo(stream);
                        }
                    }
                    detail.Item = item.Items;
                    detail.Amount = item.Amount;
                    _weeklyTimeSheetDetailRepo.Add(detail);
                }
            }
            else
            {
                var exsitingTimeSheetDetail = _weeklyTimeSheetDetailRepo.GetList(u => u.TimeSheetId == existingWeekltTimesheet.TimeSheetId && u.Date >= model.startDate && u.Date <= model.endDate);
                List<WeeklyTimeSheetDetail> list = new List<WeeklyTimeSheetDetail>();

                for (int i = 0; i < model.timesheets.Count; i++)
                {
                    var currentDate = model.timesheets[i].Date;
                    WeeklyTimeSheetDetail weeklyTimeSheetDetail = exsitingTimeSheetDetail.FirstOrDefault(detail => detail.Date == currentDate)!;
                    if (weeklyTimeSheetDetail != null)
                    {
                        weeklyTimeSheetDetail.Date = model.timesheets[i].Date;
                        weeklyTimeSheetDetail.HouseCall = model.timesheets[i].NumberOfHouseCall;
                        weeklyTimeSheetDetail.PhoneConsult = model.timesheets[i].NumberOfPhoneConsults;
                        weeklyTimeSheetDetail.Item = model.timesheets[i].Items ?? null;
                        weeklyTimeSheetDetail.Amount = model.timesheets[i].Amount;
                        weeklyTimeSheetDetail.OnCallHours = model.timesheets[i].OnCallhours;
                        weeklyTimeSheetDetail.TotalHours = model.timesheets[i].TotalHours;
                        weeklyTimeSheetDetail.IsWeekendHoliday = model.timesheets[i].Weekend;
                        if (model.timesheets[i].Bill != null && model.timesheets[i].Bill!.Length > 0)
                        {
                            IFormFile newFile = model.timesheets[i].Bill!;
                            weeklyTimeSheetDetail.Bill = newFile.FileName;
                            var filePath = Path.Combine("wwwroot", "Documents", "ProviderBills", PhysicianId + "-" + model.timesheets[i].Date + "-" + Path.GetFileName(newFile.FileName));
                            FileStream stream = null;
                            try
                            {
                                stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                                newFile.CopyToAsync(stream)
;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"An error occurred: {ex.Message}");
                            }
                        }
                        list.Add(weeklyTimeSheetDetail);
                    }
                }
                foreach (WeeklyTimeSheetDetail item in list)
                {
                    _weeklyTimeSheetDetailRepo.Update(item);
                }
            }

        }
        public Invoicingcm GetInvoicingDataonChangeOfDate(DateOnly startDate, DateOnly endDate, int? PhysicianId, int? AdminID)
        {
            Invoicingcm model = new Invoicingcm();
            WeeklyTimeSheet weeklyTimeSheet = _weeklyTimeSheetRepo.GetFirstOrDefault(u => u.ProviderId == PhysicianId && (u.StartDate == startDate && u.EndDate == endDate));
            if (weeklyTimeSheet != null)
            {
                var TimehseetsData = _weeklyTimeSheetDetailRepo.SelectWhereOrderBy(x => new Timesheet
                {
                    Date = x.Date,
                    NumberofShift = x.NumberOfShifts ?? 0,
                    NightShiftWeekend = x.IsWeekendHoliday == true ? 1 : 0,
                    NumberOfHouseCall = (x.IsWeekendHoliday == false ? x.HouseCall : 0) ?? 0,
                    HousecallNightsWeekend = (x.IsWeekendHoliday == true ? x.HouseCall : 0) ?? 0,
                    NumberOfPhoneConsults = (x.IsWeekendHoliday == false ? x.PhoneConsult : 0) ?? 0,
                    phoneConsultNightsWeekend = (x.IsWeekendHoliday == true ? x.PhoneConsult : 0) ?? 0,
                    BatchTesting = x.BatchTesting ?? 0
                }, x => x.TimeSheetId == weeklyTimeSheet.TimeSheetId, x => x.Date);
                List<Timesheet> list = new List<Timesheet>();
                foreach (Timesheet item in TimehseetsData)
                {
                    list.Add(item);
                }
                model.timesheets = list;
                model.PhysicianId = PhysicianId ?? 0;
                model.IsFinalized = weeklyTimeSheet.IsFinalized == true ? true : false;
                model.startDate = startDate;
                model.endDate = endDate;
                model.Status = weeklyTimeSheet.Status == 1 ? "Pending" : "Aprooved";
            }
            else
            {
                model.timesheets = new List<Timesheet>();
            }
            model.isAdminSide = AdminID == null ? false : true;
            return model;
        }
        public Invoicingcm GetUploadedDataonChangeOfDate(DateOnly startDate, DateOnly endDate, int? PhysicianId, int pageNumber, int pagesize)
        {
            WeeklyTimeSheet weeklyTimeSheet = _weeklyTimeSheetRepo.GetFirstOrDefault(u => u.ProviderId == PhysicianId && (u.StartDate == startDate && u.EndDate == endDate));
            Invoicingcm model = new Invoicingcm();
            Expression<Func<WeeklyTimeSheetDetail, bool>> whereClauseSyntax = PredicateBuilder.New<WeeklyTimeSheetDetail>();
            whereClauseSyntax = x => x.Bill != null && x.TimeSheetId == weeklyTimeSheet.TimeSheetId;
            if (weeklyTimeSheet != null)
            {
                var UploadedItems = _weeklyTimeSheetDetailRepo.GetAllWithPagination(x => new Timesheet
                {
                    Date = x.Date,
                    Items = x.Item ?? "-",
                    Amount = x.Amount ?? 0,
                    BillName = x.Bill ?? "-",
                }, whereClauseSyntax, pageNumber, pagesize, x => x.Date, true);
                List<Timesheet> list = new List<Timesheet>();
                foreach (Timesheet item in UploadedItems)
                {
                    list.Add(item);
                }
                model.timesheets = list;

                model.pager = new Data_Layer.CustomModels.Pager
                {
                    TotalItems = _weeklyTimeSheetDetailRepo.GetTotalcount(whereClauseSyntax),
                    CurrentPage = pageNumber,
                    ItemsPerPage = pagesize
                };
                model.SkipCount = (pageNumber - 1) * pagesize;
                model.CurrentPage = pageNumber;
                model.TotalPages = (int)Math.Ceiling((decimal)model.pager.TotalItems / pagesize);
                model.IsFinalized = weeklyTimeSheet.IsFinalized == true ? true : false;
            }
            model.PhysicianId = PhysicianId ?? 0;
            return model;
        }
        public void FinalizeTimeSheet(int id)
        {
            WeeklyTimeSheet weeklyTimeSheet = _weeklyTimeSheetRepo.GetFirstOrDefault(u => u.TimeSheetId == id);
            if (weeklyTimeSheet != null)
            {
                weeklyTimeSheet.IsFinalized = true;
                _weeklyTimeSheetRepo.Update(weeklyTimeSheet);
            }
        }
        public void DeleteBill(int id)
        {
            WeeklyTimeSheetDetail weeklyTimeSheetDetail = _weeklyTimeSheetDetailRepo.GetFirstOrDefault(u => u.TimeSheetDetailId == id);
            weeklyTimeSheetDetail.Bill = null;
            _weeklyTimeSheetDetailRepo.Update(weeklyTimeSheetDetail);
        }

        public List<PhysicianViewModel> GetPhysiciansForInvoicing()
        {         
            var physicians = from r in _context.Physicians
                             where r.Isdeleted == null
                             select (new PhysicianViewModel()
                             {
                                 PhysicianId = r.Physicianid,
                                 PhysicianName = r.Firstname + " " + r.Lastname,
                             });

            List<PhysicianViewModel> PhysicianList = new List<PhysicianViewModel>();
            foreach (PhysicianViewModel item in physicians)
            {
                PhysicianList.Add(item);
            }
            return PhysicianList;
        }

        public string CheckInvoicingAproove(string selectedValue, int PhysicianId)
        {
            string[] dateRange = selectedValue.Split('*');
            DateOnly startDate = DateOnly.Parse(dateRange[0]);
            DateOnly endDate = DateOnly.Parse(dateRange[1]);
            string result = "";
            WeeklyTimeSheet weeklyTimeSheet = _weeklyTimeSheetRepo.GetFirstOrDefault(u => u.ProviderId == PhysicianId && u.StartDate == startDate && u.EndDate == endDate);
            if (weeklyTimeSheet != null)
            {
                if (weeklyTimeSheet.IsFinalized != true && weeklyTimeSheet.Status == 1)
                {
                    result = "NotFinalized-NotAprooved";
                }
                else if (weeklyTimeSheet.IsFinalized == true && weeklyTimeSheet.Status == 1)
                {
                    result = "Finalized-NotAprooved";
                }
                else if (weeklyTimeSheet.IsFinalized == true && weeklyTimeSheet.Status == 2)
                {
                    result = "Finalized-Aprooved";
                }
            }
            else
            {
                result = "False";
            }
            return result;
        }
        public Invoicingcm GetApprovedViewData(string selectedValue, int PhysicianId)
        {
            string[] dateRange = selectedValue.Split('*');
            DateOnly startDate = DateOnly.Parse(dateRange[0]);
            DateOnly endDate = DateOnly.Parse(dateRange[1]);
            WeeklyTimeSheet weeklyTimeSheet = _weeklyTimeSheetRepo.GetFirstOrDefault(u => u.ProviderId == PhysicianId && u.StartDate == startDate && u.EndDate == endDate);

            Invoicingcm model = new Invoicingcm();
            if (weeklyTimeSheet != null)
            {
                model.startDate = weeklyTimeSheet.StartDate;
                model.endDate = weeklyTimeSheet.EndDate;
                model.Status = weeklyTimeSheet.Status == 1 ? "Pending" : "Aprooved";
                model.TimeSheetId = weeklyTimeSheet.TimeSheetId;
                model.IsFinalized = weeklyTimeSheet.IsFinalized == true ? true : false;
            }
            return model;
        }

        public void AprooveTimeSheet(Invoicingcm model, int? AdminID)
        {
            WeeklyTimeSheet weeklyTimeSheet = _weeklyTimeSheetRepo.GetFirstOrDefault(u => u.ProviderId == model.PhysicianId && u.StartDate == model.startDate && u.EndDate == model.endDate);
            if (weeklyTimeSheet != null)
            {
                weeklyTimeSheet.AdminId = AdminID;
                weeklyTimeSheet.Status = 2;
                weeklyTimeSheet.Bonusamount = model.BonusAmount;
                weeklyTimeSheet.AdminNote = model.AdminNotes;
                _weeklyTimeSheetRepo.Update(weeklyTimeSheet);
            }
        }

    }
}
