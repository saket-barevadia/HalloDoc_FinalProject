using System;
using Data_Layer.CustomModels;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.Interface
{
    public interface IsubmitInfoMe
    {
        public submitReqMe getInfo(string email);

        public void submitMe(submitReqMe obj);
    }
}
