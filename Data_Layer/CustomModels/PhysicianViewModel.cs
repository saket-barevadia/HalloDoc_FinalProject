using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Layer.CustomModels
{
    public class PhysicianViewModel
    {
        public int PhysicianId { get; set; }

        public string? PhysicianName { get; set; }

        public string? role { get; set; }

        public string? OnCallStatus { get; set; }

        public int region { get; set; }

        public bool IsNotificatonStopped { get; set; }

        public string? Email { get; set; }
    }
}
