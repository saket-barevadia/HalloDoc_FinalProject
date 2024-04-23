using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Data_Layer.CustomModels
{
    public class businessReqcm
    {
        [Key]
        [Column("Userid")]
        public string Userid { get; set; }

        [Required(ErrorMessage ="Please Enter Your FirstName")]
        public string firstnamebusiness { get; set; }

        [Required(ErrorMessage = "Please Enter Your LastName")]
        public string lastnamebusiness { get; set; }

        [Required(ErrorMessage = "Please Enter Your Email")]
        public string emailbusiness { get; set; }




        [Required(ErrorMessage = "Phone Number Required!")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
                   ErrorMessage = "Entered phone format is not valid.")]
        public string mobilebusiness { get; set; }


       // [Required(ErrorMessage = "Please Enter Symptoms")]
        public String? Symptons { get; set; }

        [Required(ErrorMessage = "Please Enter Patient's FirstName")]
        public string FirstNameclient { get; set; }

        [Required(ErrorMessage = "Please Enter Patient's LastName")]
        public string LastNameclient { get; set; }

        [Required(ErrorMessage = "Please Enter Patient's BirthDate")]
        public string? Strmonth { get; set; }

        public int? Intyear { get; set; }

        public int? Intdate { get; set; }

        [Required(ErrorMessage = "Please Enter Patient's Email")]
        public string Emailclient { get; set; }



        [Required(ErrorMessage = "Phone Number Required!")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
                   ErrorMessage = "Entered phone format is not valid.")]
        public string Phoneclient { get; set; }

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

       // [Required(ErrorMessage = "Please Upload Patient's Documents")]
        public IFormFile? Upload { get; set; }


        [Required(ErrorMessage = "Please Select Your State")]
        public int? regionId { get; set; }

    }
}
