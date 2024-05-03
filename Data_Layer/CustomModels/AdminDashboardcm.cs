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
using static Data_Layer.CustomModels.ProviderMenucm;

namespace Data_Layer.CustomModels
{
    public class  AdminDashboardcm
    {

        // Models inside custom model ---------------------
        public Assigncase assigncase { get; set; }

        public TransferRequest transferReq { get; set; }

        public ViewUploads viewUploads { get; set; }

        public Orders orders { get; set; }

        public SendAgreement sendAgreements {  get; set; } 

        public CloseCase closeCase { get; set; }

        public AdminProfilecm adminProfile { get; set; }

        public Encounter encounter { get; set; }

        public AdminCreateReq adminCreateReq { get; set; }

        public Regioncm regions { get; set; }

        public GetPayRate GetPayRate { get; set; }


        // -------------------------------------------

        public int? Regionid { get; set; }
        public int? PhysicianId { get; set; }

        public short? Calltype { get; set; }
        public string physician { get; set; }


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



    public class Assigncase {

        [Required(ErrorMessage = "Please Select Region")]
        public string? Name { get; set; } 
        public List<Region> Regions { get; set; }

    }

    public class ViewUploads
    {
        public int Requestid { get; set; }
        public int? userId { get; set; }
        public int flag { get; set; }

        public BitArray? IsFinalized { get; set; }

        public string? fullName { get; set; }

        public string? Confirmationnumber { get; set; }

        [Required(ErrorMessage = "Please Upload File")]
        public IFormFile Upload { get; set; }

        public List<Documentss> documents { get; set; }

        public BitArray? Isdeleted { get; set; }

        [Required(ErrorMessage = "Enter Your Note")]
        public string? providerNote { get; set; }
    }

    public class Documentss
    {
         public string Filename { get; set; }

         public DateTime Createddate { get; set; }

        public int Requestwisefileid { get; set; }
       
    }


    public class Orders
    {

        public int Requestid { get; set; }

        public List<Profession> professions { get; set; }

        [Required(ErrorMessage = "Please Enter Faxnumber")]
        public string Faxnumber { get; set; } = null!;

        [Required(ErrorMessage = "Please Enter Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please Enter Business Contact")]
        public string? Businesscontact { get; set; }

        [Required(ErrorMessage = "Please Enter Prescription")]
        public string? Prescription { get; set; }

        [Required(ErrorMessage = "Please Select Number of Refill")]
        public int? Noofrefill { get; set; }


    }

    public class Profession
    {
        public string Professionname { get; set; }

        public int Vendorid { get; set; }
      
        public string Vendorname { get; set; } 

        public int Healthprofessionalid { get; set; }
    }


    public class TransferRequest {
        public string Name { get; set; } = null!;

        public int Regionid { get; set; }
    }


    public class SendAgreement
    {
        public int Requestid { get; set; }

        [Required(ErrorMessage = "Please Enter Receiver's FirstName")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Please Enter Receiver's LastName")]
        public string? Lastname { get; set; }

        public int Requesttypeid { get; set; }

        [Required(ErrorMessage = "Phone Number Required!")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
                   ErrorMessage = "Entered phone format is not valid.")]
        public string? Phonenumber { get; set; }

        [Required(ErrorMessage = "Please Enter Receiver's Email")]
        public string? Email { get; set; }
    }




    public class CloseCase
    {
        public int Requestid { get; set; }

        public string Firstname { get; set; } = null!;

        public string Lastname { get; set; } = null!;

        public string? fullName { get; set; }

        public string? Confirmationnumber { get; set; }

        [Required(ErrorMessage = "Phone Number Required!")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
                   ErrorMessage = "Entered phone format is not valid.")]
        public string? Phonenumber { get; set; }

        [Required(ErrorMessage = "Please Enter Your Email")]
        public string? Email { get; set; }

        public string? Strmonth { get; set; }

        public string? Date { get; set; }


        public int? Intyear { get; set; }


        public int? Intdate { get; set; }

        public List<Documentss> documents { get; set; }

    }

    public class AdminProfilecm
    {
        public List<AdminregionTable> adminregionTables { get; set; }

        public List<Regions> Region { get; set; }
        public int Adminid { get; set; } 
        
        public int? Regionid { get; set; }

        public int AspnetUserId { get; set; }

        [Required(ErrorMessage = "Please Enter Your UserName")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Please Enter Password")]
        public string? Password { get; set; }

        public short Status { get; set; }

        [Required(ErrorMessage = "Please Enter Your FirstName")]
        public string? Firstname { get; set; }

        [Required(ErrorMessage = "Please Enter Your LastName")]
        public string? Lastname { get; set; }

        [Required(ErrorMessage = "Phone Number Required!")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
                   ErrorMessage = "Entered phone format is not valid.")]
        public string? Mobile { get; set; }

        [Required(ErrorMessage = "Phone Number Required!")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
                   ErrorMessage = "Entered phone format is not valid.")]
        public string? Altphone { get; set; }

        [Required(ErrorMessage = "Please Enter Your Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please Confirm Your Email")]
        public string? ConfirmEmail { get; set; }

        [Required(ErrorMessage = "Please Enter Your First Address")]
        public string? Address1 { get; set; }

        [Required(ErrorMessage = "Please Enter Your Second Address")]
        public string? Address2 { get; set; }

        [Required(ErrorMessage = "Please Enter Your City")]
        public string? City { get; set; }

        [Required(ErrorMessage = "Please Enter Your Zipcode")]
        public string? Zip { get; set; }

        [Required(ErrorMessage = "Please Enter Your State")]
        public string? State { get; set; }

        public int Flag { get; set; }

        public int back { get; set; }


    }

    public class AdminregionTable
    {
        public int Adminid { get; set; }

        public int Regionid { get; set; }

        public string Name { get; set; }

        public bool ExistsInTable { get; set; }
    }

    public class Encounter
    {
        [Required(ErrorMessage = "Please Select atleast one option")]
        public short Option { get; set; }

        [Required(ErrorMessage = "Please Enter Your FirstName")]
        public string? Firstname { get; set; }

        [Required(ErrorMessage = "Please Enter Your LastName")]
        public string? Lastname { get; set; }

        [Required(ErrorMessage = "Please Enter Your Location")]
        public string? Location { get; set; }
       
        public string? Strmonth { get; set; }

        [Required(ErrorMessage = "Please Enter Your BirthDate")]
        public string? DateofBirth { get; set; }

        [Required(ErrorMessage = "Please Enter Your Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Phone Number Required!")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
                   ErrorMessage = "Entered phone format is not valid.")]
        public string? Mobile { get; set; }


        public int? Intyear { get; set; }


        public int? Intdate { get; set; }

        public int Id { get; set; }

        public int Requestid { get; set; }

        public BitArray? IsFinalized { get; set; }

        [Required(ErrorMessage = "Please Enter Your HistoryIllness")]
        public string? HistoryIllness { get; set; }

        [Required(ErrorMessage = "Please Enter Your MedicalHistory")]
        public string? MedicalHistory { get; set; }

        public DateTime? Date { get; set; }

        [Required(ErrorMessage = "Please Enter Your Medications")]
        public string? Medications { get; set; }

        [Required(ErrorMessage = "Please Enter Your Allergies")]
        public string? Allergies { get; set; }

        [Required(ErrorMessage = "Please Enter Temp")]
        public decimal? Temp { get; set; }
        
        [Required(ErrorMessage = "Please Enter HR")]
        public decimal? Hr { get; set; }

        [Required(ErrorMessage = "Please Enter RR")]
        public decimal? Rr { get; set; }

        [Required(ErrorMessage = "Please Enter BPS")]
        public int? BpS { get; set; }

        [Required(ErrorMessage = "Please Enter BPD")]
        public int? BpD { get; set; }

        [Required(ErrorMessage = "Please Enter O2")]
        public decimal? O2 { get; set; }

        [Required(ErrorMessage = "Please Enter Pain")]
        public string? Pain { get; set; }

        [Required(ErrorMessage = "Please Enter Heent")]
        public string? Heent { get; set; }

        [Required(ErrorMessage = "Please Enter CV")]
        public string? Cv { get; set; }

        [Required(ErrorMessage = "Please Enter Chest")]
        public string? Chest { get; set; }

        [Required(ErrorMessage = "Please Enter ABD")]
        public string? Abd { get; set; }

        [Required(ErrorMessage = "Please Enter EXTR")]
        public string? Extr { get; set; }

        [Required(ErrorMessage = "Please Enter Skin")]
        public string? Skin { get; set; }

        [Required(ErrorMessage = "Please Enter Neuro")]
        public string? Neuro { get; set; }

        [Required(ErrorMessage = "Please Enter Other")]
        public string? Other { get; set; }

        [Required(ErrorMessage = "Please Enter Diagnosis")]
        public string? Diagnosis { get; set; }

        [Required(ErrorMessage = "Please Enter Treatment Plan")]
        public string? TreatmentPlan { get; set; }

        [Required(ErrorMessage = "Please Enter MedicationDispensed")]
        public string? MedicationDispensed { get; set; }

        [Required(ErrorMessage = "Please Enter Procedures")]
        public string? Procedures { get; set; }

        [Required(ErrorMessage = "Please Enter FollowUp")]
        public string? FollowUp { get; set; }



    }


    public class AdminCreateReq
    {
        public List<Regioncm> regions { get; set; }

        [Required(ErrorMessage = "Please Enter Patient's FirstName")]
        public string? Firstname { get; set; }

        [Required(ErrorMessage = "Please Enter Patient's LastName")]
        public string? Lastname { get; set; }

        [Required(ErrorMessage = "Please Enter Patient's BirthDate")]
        public string? Strmonth { get; set; }

        [Required(ErrorMessage = "Please Enter Patient's Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Phone Number Required!")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
                   ErrorMessage = "Entered phone format is not valid.")]
        public string? Mobile { get; set; }


        public int? Intyear { get; set; }


        public int? Intdate { get; set; }

        public int? flag { get; set; }

        [Required(ErrorMessage = "Please Enter Patient's Street")]
        public string Street { get; set; }


        [Required(ErrorMessage = "Please Enter Patient's City")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please Enter Patient's State")]
        public string State { get; set; }

        [Required(ErrorMessage = "Please Enter Patient's Zipcode")]
        public string Zipcode { get; set; }

        [Required(ErrorMessage = "Please Enter Patient's Room")]
        public string Room { get; set; }

        [Required(ErrorMessage = "Please Enter Note")]
        public string Notes { get; set; }
    }


    public class Regioncm {
        public int Regionid { get; set; }

        public string? Region { get; set; }
    }

    public class GetPayRate
    {
        public int PhysicianId { get; set; }
        public int? NightShift_Weekend { get; set; }
        public int? Shift { get; set; }
        public int? HouseCalls_Nights_Weekend { get; set; }
        public int? PhoneConsult { get; set; }
        public int? PhoneConsults_Nights_Weekend { get; set; }
        public int? BatchTesting { get; set; }
        public int? HouseCalls { get; set; }
        public string? AspId { get; set; }
        public int callid { get; set; }
    }




}
