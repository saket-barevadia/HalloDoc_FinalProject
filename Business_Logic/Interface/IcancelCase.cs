using System;
using Data_Layer.CustomModels;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.Interface
{
     public interface IcancelCase
    {
        public void cancelCase(int id, int reason, string note);

        public List<AdminDashboardcm> caseTag();
    }
}
