using Data_Layer.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Layer.CustomModels
{
    public class patientProfile
    {
     
        public int Userid { get; set; }


        public int? Aspnetuserid { get; set; }

       
 
        public string Firstname { get; set; } = null!;

 
  
        public string? Lastname { get; set; }

 

        public string Email { get; set; } = null!;



        [Required(ErrorMessage = "Phone Number Required!")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
                   ErrorMessage = "Entered phone format is not valid.")]
        public string? Mobile { get; set; }


        public BitArray? Ismobile { get; set; }

        public string? Street { get; set; }

 
        public string? City { get; set; }

    
        public string? State { get; set; }


        public int? Regionid { get; set; }

   
        public string? Zipcode { get; set; }

  
        public string? Strmonth { get; set; }

 
        public int? Intyear { get; set; }


        public int? Intdate { get; set; }

        public string? Date { get; set; }



        public int Createdby { get; set; }


        public DateTime Createddate { get; set; }


        public int? Modifiedby { get; set; }


        public DateTime? Modifieddate { get; set; }

  
        public short? Status { get; set; }


        public BitArray? Isdeleted { get; set; }

        public string? Ip { get; set; }


        public BitArray? Isrequestwithemail { get; set; }


        public virtual Aspnetuser? Aspnetuser { get; set; }


        public virtual Aspnetuser CreatedbyNavigation { get; set; } = null!;

        
        public virtual Aspnetuser? ModifiedbyNavigation { get; set; }


        public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
    }
}
