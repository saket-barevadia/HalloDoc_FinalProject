using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Layer.CustomModels
{
    public class Invoicingcm
    {
        public List<DateViewModel>? dates { get; set; }

        public DateOnly startDate { get; set; }

        public DateOnly endDate { get; set; }

        public int differentDays { get; set; }

        public List<Timesheet> timesheets { get; set; } = new List<Timesheet>();

        public List<PhysicianMain>? Physicians { get; set; }

        public int PhysicianId { get; set; }

        public int TimeSheetId { get; set; }

        public bool IsFinalized { get; set; }

        public Pager? pager { get; set; }

        public int SkipCount { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public bool isAdminSide { get; set; }

        public string? Status { get; set; }

        public string? AdminNotes { get; set; }

        public int? BonusAmount { get; set; }

        public int shiftPayrate { get; set; }

        public int weekendPayrate { get; set; }

        public int HouseCallPayrate { get; set; }

        public int phoneConsultPayrate { get; set; }

        public int shiftTotal { get; set; }

        public int weekendTotal { get; set; }

        public int HouseCallTotal { get; set; }

        public int phoneconsultTotal { get; set; }

        public int GrandTotal { get; set; }

    }
    public class Timesheet
    {
        public int TotalHours { get; set; } = 0;

        public bool Weekend { get; set; } = false;

        public int NumberOfHouseCall { get; set; } = 0;

        public int NumberOfPhoneConsults { get; set; } = 0;

        public DateOnly Date { get; set; }

        public int OnCallhours { get; set; } = 0;

        public string? Medicine { get; set; }

        public int Amount { get; set; }

        public IFormFile? Bill { get; set; }

        public string? Items { get; set; }

        public int NumberofShift { get; set; } = 0;

        public int NightShiftWeekend { get; set; } = 0;

        public int HousecallNightsWeekend { get; set; } = 0;

        public int phoneConsultNightsWeekend { get; set; } = 0;

        public int BatchTesting { get; set; } = 0;

        public string? BillName { get; set; }

        public int WeeklyTimesheetDeatilsId { get; set; }


    }

    public class DateViewModel
    {
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }

    public class PhysicianMain
    {
        public string PhyName { get; set; }
        public int PhyId { get; set; }
    }

    public class Pager
    {
        public int TotalItems { get; set; }

        public int CurrentPage { get; set; }

        public int ItemsPerPage { get; set; }

    }
}

