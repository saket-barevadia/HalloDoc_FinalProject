using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Layer.CustomModels
{
    public class viewNotescm
    {
        public int Requestid { get; set; }
        public int? userId { get; set; }
        public int? flag { get; set; }

        public short Status { get; set; }
        public string? cancellationNotes { get; set; }
       
        public string? transfernotes { get; set; }


        [Required(ErrorMessage = "Please Enter Your Note")]
        public string? Adminnotes { get; set; }

        [Required(ErrorMessage = "Please Enter Your Note")]
        public string? Physiciannotes { get; set; }


    }
}
