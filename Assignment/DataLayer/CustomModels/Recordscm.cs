using DataLayer.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.CustomModels
{
    public class Recordscm
    {
        public List<Records> records {  get; set; }

        public List<Cities> cities {  get; set; }


        public class Cities
        {
            public int? BorrowerId { get; set; }

            public string City { get; set; } = null!;

        }

        public class Records
        {
            public int Id { get; set; }

       
            public string BookName { get; set; } = null!;

    
            public string Author { get; set; } = null!;

     
            public int? BorrowerId { get; set; }

   
            public string BorrowerName { get; set; } = null!;

    
            public DateTime DateOfIssue { get; set; }

            public string City { get; set; } = null!;

            public string Genre { get; set; } = null!;

           
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Please Enter Book Name")]
        public string BookName { get; set; } = null!;

        [Required(ErrorMessage = "Please Enter Author")]
        public string Author { get; set; } = null!;


        public int? BorrowerId { get; set; }

        [Required(ErrorMessage = "Please Enter Borrower Name")]
        public string BorrowerName { get; set; } = null!;

        [Required(ErrorMessage = "Please Enter Date Of Issue")]
        public DateTime DateOfIssue { get; set; }

        [Required(ErrorMessage = "Please Enter City")]
        public string City { get; set; } = null!;

        [Required(ErrorMessage = "Please Select Genere")]
        public string Genre { get; set; } = null!;
    }
}
