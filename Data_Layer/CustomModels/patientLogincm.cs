﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Layer.CustomModels
{
    public class patientLogincm
    {
        [Required(ErrorMessage = "Please Enter Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please Enter Password")]

        public string Password { get; set; }
        public int? id { get; set; }
        public int? AdminId { get; set; }
        public int? Physicianid { get; set; }
        public int roleId { get; set; }

        public string? Role { get; set; }
        public string? Username { get; set; }
    }
}


