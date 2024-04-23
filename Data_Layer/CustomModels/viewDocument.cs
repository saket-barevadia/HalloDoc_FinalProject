using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Layer.CustomModels
{
    public class viewDocument
    {
        public int Requestid { get; set; }

        [Required(ErrorMessage = "Please Upload a File")]
        public IFormFile Upload { get; set; }

        public string? Firstname { get; set; }


        public List<viewDoc> viewDocs { get; set; }




        public class viewDoc
        {
            public string? Firstname { get; set; }

            public int Requestid { get; set; }

            public DateTime Createddate { get; set; }


            public string? Filename { get; set; }


           
        }

        


    }
}
