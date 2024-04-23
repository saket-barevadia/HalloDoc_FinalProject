using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Data_Layer.CustomModels
{
    public class PatientRequestCm
    {
        [Key]
        [Column("Userid")]
        public string Userid { get; set; }

        //[Required(ErrorMessage = "Please Enter Symptoms")]
        public String? symptoms { get; set; }

        [Required(ErrorMessage ="Please Enter Your FirstName")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please Enter Your LastName")]
        public string LastName { get; set; }


        [Required(ErrorMessage = "Please Enter Your Email")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Phone Number Required!")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
                   ErrorMessage = "Entered phone format is not valid.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Please Enter Your Street")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Please Enter Your City")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please Enter Your State")]
        public string State { get; set; }

        [Required(ErrorMessage = "Please Enter Your Zipcode")]
        public string Zipcode { get; set; }

        [Required(ErrorMessage = "Please Enter Your Room")]
        public string Room { get; set; }

        //[Required(ErrorMessage = "Please Upload Your Documents")]
        public IFormFile? Uploadd { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string PasswordHash { get; set; }


        [Required(ErrorMessage = "Confirm Password is required")]
        [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Compare("PasswordHash")]
        public string ConfirmPasswordHash { get; set; }

        [Required(ErrorMessage = "Please Enter Your BirthDate")]
        public string? Strmonth { get; set; }

        
        public int? Intyear { get; set; }

        
        public int? Intdate { get; set; }

        [Required(ErrorMessage = "Please Select Your State")]
        public int? regionId { get; set; }


    }
    
}

