using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Layer.DataModels;

namespace Data_Layer.CustomModels
{
    public class viewCaseCm
    {
        public int Requestid { get; set; }
        public int? PhysicianId { get; set; }
        public int? userId { get; set; }
        public int flag { get; set; }

        [Required(ErrorMessage = "Please Enter Patient's FirstName")]
        public string Firstname { get; set; } = null!;


        [Required(ErrorMessage = "Please Enter Patient's LastName")]
        public string? Lastname { get; set; }

        [Required(ErrorMessage = "Please Enter Patient's PhoneNumber")]
        public string? Phonenumber { get; set; }

        [Required(ErrorMessage = "Please Enter Patient's Address")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Please Enter Patient's Symptoms")]
        public string? Notes { get; set; }

        [Required(ErrorMessage = "Please Enter Patient's Email")]
        public string? Email { get; set; }

      
        public string? Strmonth { get; set; }

        [Required(ErrorMessage = "Please Enter Patient's BirthDate")]
        public string? Date { get; set; }


        public int? Intyear { get; set; }


        public int? Intdate { get; set; }

        public string? Street { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        [Required(ErrorMessage = "Please Enter Patient's Region")]
        public virtual Region? Region { get; set; }

        public int Requesttypeid { get; set; }

        public short Status { get; set; }


        public string? Confirmationnumber  { get; set; }

    }
}
