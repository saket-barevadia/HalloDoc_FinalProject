using System;
using Data_Layer.CustomModels;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Business_Logic.Interface
{
    public interface IviewCase
    {
        public viewCaseCm viewCase(int id, int flag);

        public void viewCaseUpdate(viewCaseCm cm);
    }
}
