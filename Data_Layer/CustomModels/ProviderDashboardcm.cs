using Data_Layer.DataModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Layer.CustomModels
{
    public class ProviderDashboardcm
    {

        public List<PatientData> patientDatas { get; set; }

      

        public AdminTransferRequestModalBox? AdminTransferRequestModalBoxx { get; set; }

        public int RequestStatus { get; set; }

        public int? Requestid { get; set; }

        public int? AdminId { get; set; }
        public List<Admincm> admins { get; set; }

        public class PatientData
        {
           

            public int? Regionid { get; set; }
          

            public string physician { get; set; }

            public short? Calltype { get; set; }

            public BitArray? IsFinalized { get; set; }

            public string Firstname { get; set; } = null!;

            public string? Strmonth { get; set; }

            public int? Intyear { get; set; }

            public int? Intdate { get; set; }

            public string? FirstnameRequestor { get; set; }

            public DateTime Createddate { get; set; }

            [Required(ErrorMessage = "Phone Number Required!")]
            [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
                       ErrorMessage = "Entered phone format is not valid.")]
            public string? Phonenumber { get; set; }

            public string? requestClientPhonenumber { get; set; }

            public string Lastname { get; set; } = null!;

            public string? LastnameRequestor { get; set; }

            public string? Street { get; set; }

            public string? Address { get; set; }

            public string? Notes { get; set; }

            public int Requesttypeid { get; set; }

            public string? Email { get; set; }

            public int Count { get; set; }

            public short Status { get; set; }

            public int Requestid { get; set; }

            public string Name { get; set; } = null!;

            public int Casetagid { get; set; }
        }

        public class Admincm
        {
            public int Adminid { get; set; }

            public string Firstname { get; set; }
        }


        public class AdminTransferRequestModalBox
        {
            public int RequestId { get; set; }

            [Required(ErrorMessage = "Select the region")]
            public string region { get; set; }

            [Required(ErrorMessage = "Description is Required")]
            public string? AssignAdditionalNotes { get; set; }

            public int physician { get; set; }

        }
    }
}
