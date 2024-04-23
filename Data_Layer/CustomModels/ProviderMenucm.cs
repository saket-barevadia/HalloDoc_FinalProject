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
    public class ProviderMenucm
    {

        public string? Email { get; set; }

        [Required(ErrorMessage = "Please Enter Your Notes")]
        public string? notes { get; set; }

        public int? Physicianid { get; set; }

        public int? Radio { get; set; }



        public bool? indicate { get; set; }
        public List<Regions> Region { get; set; }

        public List<Provider> Providers { get; set; }

        public ProviderProfile provideProfile { get; set; }


        public class Regions {
            public int Regionid { get; set; }
        
            public string region { get; set; } 
        }

        public class Provider
        {
            public int? Regionid { get; set; }
            public int Physicianid { get; set; }
            public string providerName { get; set; }

            public string Email { get; set; }

            public string? Mobile { get; set; }

            public short? Status { get; set; }

            public string? Role { get; set; }

            public string callStatus { get; set; }


        }


        public class ProviderProfile {

            public decimal? Longitude { get; set; }
            public decimal? Latitude { get; set; }
            public int Flag { get; set; }

            public int filterRole { get; set; }

            public List<Regions> Region { get; set; }

            public List<Roles> roles { get; set; }

            public List<PhysicianRegionTable> physicianRegionTables { get; set; }

            public int Physicianid { get; set; }

            public int Adminid { get; set; }

            [Required(ErrorMessage = "Please Enter Your Password")]
            public string? Passwordhash { get; set; }

            public int? Aspnetuserid { get; set; }

            [Required(ErrorMessage = "Please Enter Your UserName")]
            public string? Username { get; set; }

            [Required(ErrorMessage = "Please Enter Your FirstName")]
            public string Firstname { get; set; } = null!;

            [Required(ErrorMessage = "Please Enter Your LastName")]
            public string? Lastname { get; set; }

            [Required]
            [EmailAddress(ErrorMessage = "Invalid email address.")]
            [RegularExpression(@"^[^@\s]+@[^@\s]+\.(com|net|org|gov)$", ErrorMessage = "Invalid pattern.")]
            public string Email { get; set; } = null!;

            [Required(ErrorMessage = "Please Confirm Your Email")]
            public string ConfirmEmail { get; set; } = null!;

            [Required(ErrorMessage = "Phone Number Required!")]
            [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
                     ErrorMessage = "Entered phone format is not valid.")]
            public string? Mobile { get; set; }

            [Required(ErrorMessage = "Please Enter MedicalLicense")]
            public string? Medicallicense { get; set; }

            [Required(ErrorMessage = "Please Upload Photo")]
            public IFormFile? Photo { get; set; }

            public string? PhotoValue { get; set; }

            [Required(ErrorMessage = "Please Upload Signature")]
            public IFormFile? Signature { get; set; }

            public string? SignatureValue { get; set; }

            [Required(ErrorMessage = "Please Enter Your Notes")]
            public string? Adminnotes { get; set; }

            [Required(ErrorMessage = "Please Enter Your Address1")]
            public string? Address1 { get; set; }

            [Required(ErrorMessage = "Please Enter Your Address2")]
            public string? Address2 { get; set; }

            [Required(ErrorMessage = "Please Enter Your City")]
            public string? City { get; set; }

            [Required(ErrorMessage = "Please Select Your State")]
            public int? Regionid { get; set; }

            [Required(ErrorMessage = "Please Enter Your ZipCode")]
            public string? Zip { get; set; }

            [Required(ErrorMessage = "Phone Number Required!")]
            [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
                      ErrorMessage = "Entered phone format is not valid.")]
            public string? Altphone { get; set; }

          
            public int? Createdby { get; set; }

            public DateTime Createddate { get; set; }

           
            public int? Modifiedby { get; set; }

           
            public DateTime? Modifieddate { get; set; }

           
            public short? Status { get; set; }

            [Required(ErrorMessage = "Please Enter Your BusinessName")]
            public string Businessname { get; set; } = null!;

            [Required(ErrorMessage = "Please Enter Your Business Website")]
            public string? Businesswebsite { get; set; }

           
            public BitArray? Isdeleted { get; set; }

            [Required(ErrorMessage = "Please Select Your Role")]
            public int? Roleid { get; set; }

            [Required(ErrorMessage = "Please Enter Your Npi Number")]
            public string? Npinumber { get; set; }

            [Required]
            [EmailAddress(ErrorMessage = "Invalid email address.")]
            [RegularExpression(@"^[^@\s]+@[^@\s]+\.(com|net|org|gov)$", ErrorMessage = "Invalid pattern.")]
            public string? Syncemailaddress { get; set; }

            public IFormFile? ContractorAgreement { get; set; }

            public bool IsContractorAgreement { get; set; }

            public IFormFile? BackgroundCheck { get; set; }

            public bool IsBackgroundCheck { get; set; }

            public IFormFile? HIPAA { get; set; }

            public bool IsHIPAA { get; set; }

            public IFormFile? NonDisclosure { get; set; }

            public bool IsNonDisclosure { get; set; }

            public IFormFile? LicenseDocument { get; set; }

            public bool IsLicenseDocument { get; set; }
        }

        public class Roles
        {
            public int Roleid { get; set; }

            public string role { get; set; } 
        }

        public class PhysicianRegionTable
        {
            public int PhysicianId { get; set; }

            public int Regionid { get; set; }

            public string Name { get; set; }

            public bool ExistsInTable { get; set; }
        }




        // Account Access
        public List<AccountAccess> accountAccess { get; set; }
        public List<Aspnetrole> Aspnetroles { get; set; }
        public AccountAccess CreateAccountAccess { get; set; }
        public List<Menu> Menus { get; set; }
        public List<AccountMenu> accountMenu { get; set; }

        public List<UserAccess> Useraccess { get; set; }

        public int Flag { get; set; }


        public class AccountAccess
        {
            
            public int Roleid { get; set; }


            [Required(ErrorMessage = "Please Enter Your RoleName")]
            public string Name { get; set; } = null!;

            public string Accounttype { get; set; }

            [Required(ErrorMessage = "Please Select Your Role")]
            public short AccounttypeId { get; set; }


        }

        public class AccountMenu
        {
            public int menuid { get; set; }

            public int roleid { get; set; }

            public string name { get; set; }

            public int accounttype { get; set; }

            public bool ExistsInTable { get; set; }

        }

        public class UserAccess
        {
            public int PhysicianId { get; set; }
            public int aspnetid { get; set; }
            public string Accounttype { get; set; }

            public string accountname { get; set; }

            public string phone { get; set; }

            public int status { get; set; }
            public int openrequest { get; set; }

            public int roleid { get; set; }



        }


        //Vendors

        public Vendor vendor { get; set; }

        public class Vendor
        {
            public List<Healthprofessionaltype> healthprofessionaltypes { get; set; }

            public List<Vendors> vendors { get; set; }


            public List<Region> regions { get; set; }

            [Required(ErrorMessage = "Please Select Your State")]
            public int? RegionId { get; set; }

            [Required(ErrorMessage = "Please Select Your Profession")]
            public int? Healthprofessionalid { get; set; }
            public string Professionname { get; set; }

            public int Vendorid { get; set; }

            [Required(ErrorMessage = "Please Enter Your BusinessName")]
            public string Vendorname { get; set; }

            public int? Profession { get; set; }

            [Required(ErrorMessage = "Please Enter Your Fax Number")]
            public string Faxnumber { get; set; }

            [Required(ErrorMessage = "Please Enter Your Phone Number")]
            public string? Phonenumber { get; set; }

            [Required]
            [EmailAddress(ErrorMessage = "Invalid email address.")]
            [RegularExpression(@"^[^@\s]+@[^@\s]+\.(com|net|org|gov)$", ErrorMessage = "Invalid pattern.")]
            public string? Email { get; set; }

            [Required(ErrorMessage = "Please Enter Your Business Contact")]
            public string? Businesscontact { get; set; }

            [Required(ErrorMessage = "Please Enter Your Street")]
            public string? Street { get; set; }

            [Required(ErrorMessage = "Please Enter Your City")]
            public string? City { get; set; }

            [Required(ErrorMessage = "Please Enter Your State")]
            public string? State { get; set; }

            [Required(ErrorMessage = "Please Enter Your Zip Code")]
            public string? Zip { get; set; }
        }
     
        public class Vendors
        {

            public int Healthprofessionalid { get; set; }
            public string Professionname { get; set; }

            public int Vendorid { get; set; }

            public string Vendorname { get; set; } 

            public int? Profession { get; set; }

            public string Faxnumber { get; set; }

            public string? Phonenumber { get; set; }

            public string? Email { get; set; }
            public string? Businesscontact { get; set; }

            public string? Street { get; set; }

            public string? City { get; set; }

            public string? State { get; set; }

            public string? Zip { get; set; }
        }



    }
}
