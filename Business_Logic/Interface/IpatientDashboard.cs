using System;
using Data_Layer.CustomModels;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.Interface
{
    public interface IpatientDashboard
    {
        public ReviewAgreement review(int id);

        public bool iAgree(ReviewAgreement cm);

        public bool cancelAgree(ReviewAgreement cm);

        public patientDashboard createAccount(int id, int flag);

        public void account(createAccount cm);

        public void resetPassword(patientLogincm cm);
    }
}
