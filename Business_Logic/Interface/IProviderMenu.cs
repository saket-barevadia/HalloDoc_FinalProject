using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Layer.CustomModels;
using Data_Layer.DataModels;
using static Data_Layer.CustomModels.ProviderMenucm;

namespace Business_Logic.Interface
{
    public interface IProviderMenu
    {
        public ProviderMenucm providerDetails(int regionId);

        public void sendMail(ProviderMenucm cm, string adminEmail);

        public ProviderMenucm stopNotification(int phyId);

        public ProviderProfile providerProfile(int phyId,int flag);

        public void editProvider(ProviderProfile cm);

        public void editphysicianInfo(ProviderProfile cm, List<int> physicianRegions);

        public void resetPassword(ProviderProfile cm);


        public void removePhysician(ProviderProfile cm);

        public void editMailingForm(ProviderProfile cm);

        public void editproviderInfo(ProviderProfile cm);

        public void EditOnBoardingData(ProviderProfile providerProfileCm);

        public ProviderProfile getRegions(int flag, int filterRole);


        public bool createProviderAcc(ProviderProfile cm, List<int> physicianRegions);



        // Account Access

        public ProviderMenucm accountAccessData();

        public ProviderMenucm createAccount();

        public ProviderMenucm getMenu(int accountTypeId);

        public void accountAccessPost(ProviderMenucm cm, List<int> AccountMenu);

        public ProviderMenucm getEditAccAccess(int accTypeId, int roleId);

        public ProviderMenucm GetAccountMenu(int accounttype, int roleid);

        public void editAccAccessPost(ProviderMenucm cm, List<int> AccountMenu);

        public void DeleteAccountAccess(int roleid);

        public ProviderMenucm userAccess(int accountTypeId);

        public bool createAdminAccount(ProviderProfile cm, List<int> physicianRegions);


        //Provider Location
        public List<Physicianlocation> GetPhysicianlocations();


        //vendors/partners
        public Vendor GetVendors( int professionId, Vendor cm);

        public Vendor getHealthProfessionals();

        public void addBusiness(Vendor cm);

        public void deleteBusiness(int vendorId);

        public Vendor getVendorDetails(int vendorId);

        public bool editBusiness(Vendor cm);
    }
}
