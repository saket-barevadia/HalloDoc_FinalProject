using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Layer.CustomModels
{
    public class patientDashboard
    {

        public ReviewAgreement review { get; set; }

        public createAccount create {  get; set; }

        public DateTime Createddate { get; set; }


        public short Status { get; set; }

        public int doc_Count { get; set; }

        public int Requestid { get; set; }

        public string Email { get; set; } = null!;


        public string? Filename { get; set; }

      
    }
    public class ReviewAgreement
    {
        public int Requestid { get; set; }

        public short Status { get; set; }

        public string? Notes { get; set; }

        public string Firstname { get; set; } = null!;

        public string Lastname { get; set; } = null!;

        public string? fullName { get; set; }
    }

    public class createAccount {
        public int Id { get; set; }
        public int flag { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.(com|net|org|gov)$", ErrorMessage = "Invalid pattern.")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Password is required")]
        [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string? Passwordhash { get; set; }



        [Required(ErrorMessage = "Confirm Password is required")]
        [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Compare("Passwordhash")]
        public string? ConfirmPasswordhash { get; set; }
    }

}
