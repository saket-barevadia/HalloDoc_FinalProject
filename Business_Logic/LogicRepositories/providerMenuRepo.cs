using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Logic.Interface;
using Data_Layer.DataContext;
using Data_Layer.DataModels;
using Data_Layer.CustomModels;
using static Data_Layer.CustomModels.ProviderMenucm;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Mail;
using System.Net;
using System.Collections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Security.Cryptography.Xml;
using System.Drawing;
using System.Data;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML.Messaging;

namespace Business_Logic.LogicRepositories
{
    public class providerMenuRepo : IProviderMenu
    {
        private readonly ApplicationDbContext _context;

        public providerMenuRepo(ApplicationDbContext context)
        {
            _context = context;
        }


        public ProviderMenucm providerDetails(int regionId)
        {
            var regions = from r in _context.Regions
                          select (new Regions()
                          {
                              region = r.Name,
                              Regionid = r.Regionid
                          });





            var physicianData = from r in _context.Physicians
                                where r.Isdeleted == null
                                select (new Provider()
                                {
                                    Physicianid = r.Physicianid,
                                    providerName = r.Firstname + " " + r.Lastname,
                                    Email = r.Email,
                                    Mobile = r.Mobile,
                                    Status = r.Status,

                                    Role = _context.Roles.FirstOrDefault(i => i.Roleid.ToString() == r.Roleid.ToString()).Name,
                                    Regionid = r.Regionid,

                                }) ;

            if (regionId >0)
            {
                physicianData = physicianData.Where(r => r.Regionid == regionId);
            }

            ProviderMenucm providerDetails = new ProviderMenucm()
            {
                Providers = physicianData.ToList(),
                Region = regions.ToList(),
            };

            return providerDetails;

        }



        //contact POST
        public void sendMail(ProviderMenucm cm,string adminEmail)
        {

            var physician = _context.Physicians.FirstOrDefault(x => x.Physicianid == cm.Physicianid);

            var admin = _context.Admins.FirstOrDefault(x => x.Email == adminEmail);

            if (cm.Radio == 1)
            {
                var accountSid = "ACac576656eeaa15707ed2ef4464209e55";
                var authToken = "e8f9007b647d13348dbae276bd226d75";
                var twilionumber = "+16562189333";

                var messageBody = $"Hello {physician.Firstname} {physician.Lastname},\n {cm.notes} \n\n\nRegards,\n{admin.Firstname},\n(HelloDoc Admin)";

                TwilioClient.Init(accountSid, authToken);

                var messagee = MessageResource.Create(
                from: new Twilio.Types.PhoneNumber(twilionumber),
                body: messageBody,
                to: new Twilio.Types.PhoneNumber("+91" + physician.Mobile)
                );


                Smslog smslog = new Smslog()
                {
                    Smstemplate = "Sender : " + adminEmail ,
                    Mobilenumber = physician.Mobile,
                    Confirmationnumber = _context.Physicians.FirstOrDefault(x => x.Physicianid == cm.Physicianid).Firstname.Substring(0, 2) + DateTime.Now.ToString().Substring(0, 19).Replace(" ", ""),
                    Roleid = 3,
                    Adminid = admin.Adminid,
                    Physicianid = physician.Physicianid,
                    Createdate = DateTime.Now,
                    Sentdate = DateTime.Now,
                    Issmssent = new BitArray(1, true),
                    Senttries = 1,

                };

                _context.Smslogs.Add(smslog);
                _context.SaveChanges();

            }

            if (cm.Radio == 2)
            {
                string smtpServer = "outlook.office365.com";
                int port = 587; // Port number for SMTP (e.g., 587 for Gmail)
                string senderEmail = "tatva.dotnet.saketbarevadia@outlook.com";
                string password = "S@ket6898";
                string recipientEmail = cm.Email;
                string subject = "Message from Admin";
                string body = cm.notes;
                using (SmtpClient client = new SmtpClient(smtpServer, port))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(senderEmail, password);

                    using (MailMessage message = new MailMessage(senderEmail, recipientEmail, subject, body))
                    {

                        client.Send(message);
                    }
                }


                Emaillog emaillog = new Emaillog()
                {
                    Subjectname = subject,
                    Emailid = cm.Email,
                    Roleid = 3,
                    Emailtemplate = "Sender : " + senderEmail + "Reciver :" + recipientEmail + "Subject : " + subject + "Message : " + body,
                    Isemailsent = new BitArray(1, true),
                    Physicianid = cm.Physicianid,
                    Createdate = DateTime.Now,
                    Sentdate = DateTime.Now,
                    Adminid = _context.Admins.FirstOrDefault(x => x.Email == adminEmail).Adminid,
                    Confirmationnumber = _context.Physicians.FirstOrDefault(x => x.Physicianid == cm.Physicianid).Firstname.Substring(0, 2) + DateTime.Now.ToString().Substring(0, 19).Replace(" ", ""),
                    Senttries = 1,
                };

                _context.Emaillogs.Add(emaillog);
                _context.SaveChanges();
            }

            if (cm.Radio == 3)
            {
                var accountSid = "ACac576656eeaa15707ed2ef4464209e55";
                var authToken = "e8f9007b647d13348dbae276bd226d75";
                var twilionumber = "+16562189333";

                var messageBody = $"Hello {physician.Firstname} {physician.Lastname},\n {cm.notes} \n\n\nRegards,\n{admin.Firstname},\n(HelloDoc Admin)";

                TwilioClient.Init(accountSid, authToken);

                var messagee = MessageResource.Create(
                from: new Twilio.Types.PhoneNumber(twilionumber),
                body: messageBody,
                to: new Twilio.Types.PhoneNumber("+91" + physician.Mobile)
                );


                Smslog smslog = new Smslog()
                {
                    Smstemplate = "Sender : " + adminEmail,
                    Mobilenumber = physician.Mobile,
                    Confirmationnumber = _context.Physicians.FirstOrDefault(x => x.Physicianid == cm.Physicianid).Firstname.Substring(0, 2) + DateTime.Now.ToString().Substring(0, 19).Replace(" ", ""),
                    Roleid = 3,
                    Adminid = admin.Adminid,
                    Physicianid = physician.Physicianid,
                    Createdate = DateTime.Now,
                    Sentdate = DateTime.Now,
                    Issmssent = new BitArray(1, true),
                    Senttries = 1,

                };

                _context.Smslogs.Add(smslog);
                _context.SaveChanges();


                string smtpServer = "outlook.office365.com";
                int port = 587; // Port number for SMTP (e.g., 587 for Gmail)
                string senderEmail = "tatva.dotnet.saketbarevadia@outlook.com";
                string password = "S@ket6898";
                string recipientEmail = cm.Email;
                string subject = "Message from Admin";
                string body = cm.notes;
                using (SmtpClient client = new SmtpClient(smtpServer, port))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(senderEmail, password);

                    using (MailMessage message = new MailMessage(senderEmail, recipientEmail, subject, body))
                    {

                        client.Send(message);
                    }
                }


                Emaillog emaillog = new Emaillog()
                {
                    Subjectname = subject,
                    Emailid = cm.Email,
                    Roleid = 3,
                    Emailtemplate = "Sender : " + senderEmail + "Reciver :" + recipientEmail + "Subject : " + subject + "Message : " + body,
                    Isemailsent = new BitArray(1, true),
                    Physicianid = cm.Physicianid,
                    Createdate = DateTime.Now,
                    Sentdate = DateTime.Now,
                    Adminid = _context.Admins.FirstOrDefault(x => x.Email == adminEmail).Adminid,
                    Confirmationnumber = _context.Physicians.FirstOrDefault(x => x.Physicianid == cm.Physicianid).Firstname.Substring(0, 2) + DateTime.Now.ToString().Substring(0, 19).Replace(" ", ""),
                    Senttries = 1,
                };

                _context.Emaillogs.Add(emaillog);
                _context.SaveChanges();
            }

               
            
           

        }




        public ProviderMenucm stopNotification(int phyId)
        {
            ProviderMenucm menucm = new ProviderMenucm();

            var phyNotification = _context.Physiciannotifications.Where(r => r.Physicianid == phyId).Select(r => r).First();

            var notification = new BitArray(1);
            notification[0] = false;

            if (phyNotification.Isnotificationstopped[0] == notification[0])
            {
                phyNotification.Isnotificationstopped = new BitArray(1);
                phyNotification.Isnotificationstopped[0] = true;
                _context.SaveChanges();

                menucm.indicate = true;
                return menucm;


            }
            else
            {
                phyNotification.Isnotificationstopped = new BitArray(1);
                phyNotification.Isnotificationstopped[0] = false;
                _context.SaveChanges();

                menucm.indicate = false;
                return menucm;
            }
        }



        public ProviderProfile providerProfile(int phyId, int flag)
        {
            var physician = _context.Physicians.FirstOrDefault(x => x.Physicianid == phyId);

            if (physician != null)
            {

                var regions = from r in _context.Regions
                              select (new Regions()
                              {
                                  region = r.Name,
                                  Regionid = r.Regionid
                              });

                var roles = from r in _context.Roles where r.Accounttype==3 select (new Roles() { role = r.Name, Roleid = r.Roleid });

                var region = _context.Regions.ToList();
                var phyRegion = _context.Physicianregions.ToList();

                var checkedRegion = region.Select(r1 => new PhysicianRegionTable
                {
                    Regionid = r1.Regionid,
                    Name = r1.Name,
                    ExistsInTable = phyRegion.Any(r2 => r2.Physicianid == phyId && r2.Regionid == r1.Regionid),
                }).ToList();


                ProviderProfile providerProfileData = new ProviderProfile()
                {
                    Physicianid = physician.Physicianid,
                    Username = _context.Aspnetusers.FirstOrDefault(x => x.Id == physician.Aspnetuserid).Username,
                    Firstname = physician.Firstname,
                    Lastname = physician.Lastname,
                    Email = physician.Email,
                    Mobile = physician.Mobile,
                    Medicallicense = physician.Medicallicense,
                    Npinumber = physician.Npinumber,
                    Syncemailaddress = physician.Syncemailaddress,
                    Address1 = physician.Address1,
                    Address2 = physician.Address2,
                    City = physician.City,
                    Zip = physician.Zip,
                    Altphone = physician.Altphone,
                    Businessname = physician.Businessname,
                    Businesswebsite = physician.Businesswebsite,
                    Adminnotes = physician.Adminnotes,
                    Region = regions.ToList(),
                    roles = roles.ToList(),
                    physicianRegionTables = checkedRegion,
                    Roleid = physician.Roleid,
                    Status = physician.Status,
                    Aspnetuserid = physician.Aspnetuserid,
                    Regionid = physician.Regionid,
                    PhotoValue = physician.Photo,
                    SignatureValue = physician.Signature,
                    Flag=flag,
                };


                return providerProfileData;
            }

            return null;


        }


        //POST
        public void editProvider(ProviderProfile cm)
        {
            var physician = _context.Physicians.FirstOrDefault(x => x.Physicianid == cm.Physicianid);

            var aspnetuser = _context.Aspnetusers.FirstOrDefault(x => x.Id == physician.Aspnetuserid);


            aspnetuser.Username = cm.Username;
            aspnetuser.Modifieddate = DateTime.Now;

            _context.Aspnetusers.Update(aspnetuser);
            _context.SaveChanges();



            physician.Status = cm.Status;
            physician.Roleid = cm.Roleid;
            physician.Modifieddate = DateTime.Now;

            _context.Physicians.Update(physician);
            _context.SaveChanges();


        }



        //POST
        public void editphysicianInfo(ProviderProfile cm, List<int> physicianRegions)
        {
            var physician = _context.Physicians.FirstOrDefault(x => x.Physicianid == cm.Physicianid);

            if (physician != null)
            {

                physician.Firstname = cm.Firstname;
                physician.Lastname = cm.Lastname;
                physician.Email = cm.Email;
                physician.Mobile = cm.Mobile;
                physician.Medicallicense = cm.Medicallicense;
                physician.Npinumber = cm.Npinumber;
                physician.Syncemailaddress = cm.Syncemailaddress;
                physician.Modifieddate = DateTime.Now;

                if (_context.Physicianregions.Any(x => x.Physicianid == physician.Physicianid))
                {
                    var physicianRegion = _context.Physicianregions.Where(x => x.Physicianid == physician.Physicianid).ToList();

                    _context.Physicianregions.RemoveRange(physicianRegion);
                    _context.SaveChanges();
                }

                foreach (int regionId in physicianRegions)
                {
                    var region = _context.Regions.FirstOrDefault(x => x.Regionid == regionId);

                    _context.Physicianregions.Add(new Physicianregion()
                    {
                        Physicianid = cm.Physicianid,
                        Regionid = region.Regionid,
                    });
                    _context.SaveChanges();
                }
                _context.SaveChanges();

            }
            

        }


        //Reset
        public void resetPassword(ProviderProfile cm)
        {
            var aspnetuser = _context.Aspnetusers.FirstOrDefault(x => x.Id == cm.Aspnetuserid);

            aspnetuser.Passwordhash = cm.Passwordhash;
            aspnetuser.Modifieddate = DateTime.Now;

            _context.Aspnetusers.Update(aspnetuser);
            _context.SaveChanges();
        }



        public void editMailingForm(ProviderProfile cm)
        {
            var physician = _context.Physicians.FirstOrDefault(x => x.Physicianid == cm.Physicianid);

            if (physician != null)
            {

                physician.Address1 = cm.Address1;
                physician.Address2 = cm.Address2;
                physician.City = cm.City;
                physician.Regionid = cm.Regionid;
                physician.Zip = cm.Zip;
                physician.Altphone = cm.Altphone;
                physician.Modifieddate = DateTime.Now;

                _context.Physicians.Update(physician);
                _context.SaveChanges();

                var physicainLocation = _context.Physicianlocations.FirstOrDefault(x => x.Physicianid == physician.Physicianid);

                if(physicainLocation!= null)
                {
                    physicainLocation.Physicianid=physician.Physicianid;
                    physicainLocation.Physicianname = physician.Firstname;
                    physicainLocation.Longitude = cm.Longitude;
                    physicainLocation.Latitude= cm.Latitude;
                    physicainLocation.Address = physician.Address1;
                    physicainLocation.Createddate = physician.Createddate;

                    _context.SaveChanges();
                }
                


            }

        }



        //POST editproviderInfo
        public void editproviderInfo(ProviderProfile cm)
        {
            var physician = _context.Physicians.FirstOrDefault(x => x.Physicianid == cm.Physicianid);

            physician.Businessname = cm.Businessname;
            physician.Businesswebsite = cm.Businesswebsite;
            physician.Adminnotes = cm.Adminnotes;
            physician.Modifieddate = DateTime.Now;

            _context.Physicians.Update(physician);
            _context.SaveChanges();


            if (cm.Photo != null)
            {
                string filename = cm.Photo.FileName;
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Documents", filename);
                IFormFile file = cm.Photo;

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                physician.Photo = cm.Photo.FileName;

                _context.SaveChanges();
            }

            if (cm.Signature != null)
            {
                string filename = cm.Signature.FileName;
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Documents", filename);
                IFormFile file = cm.Signature;

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                physician.Signature = cm.Signature.FileName;
                _context.SaveChanges();
            }



        }


        public void removePhysician(ProviderProfile cm)
        {
            var physician=_context.Physicians.FirstOrDefault(x=>x.Physicianid== cm.Physicianid);

            if(physician != null)
            {
                physician.Isdeleted = new BitArray(1, true);

                _context.SaveChanges();
            }
        }




        public void EditOnBoardingData(ProviderProfile providerProfileCm)
        {
            var physicianData = _context.Physicians.FirstOrDefault(x => x.Aspnetuserid == providerProfileCm.Aspnetuserid);

            string directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Documents", physicianData.Physicianid.ToString());

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (providerProfileCm.ContractorAgreement != null)
            {
                string path = Path.Combine(directory, "Independent_Contractor" + Path.GetExtension(providerProfileCm.ContractorAgreement.FileName));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    providerProfileCm.ContractorAgreement.CopyTo(fileStream);
                }

                physicianData.Isagreementdoc = new BitArray(1, true);
            }

            if (providerProfileCm.BackgroundCheck != null)
            {
                string path = Path.Combine(directory, "Background" + Path.GetExtension(providerProfileCm.BackgroundCheck.FileName));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    providerProfileCm.BackgroundCheck.CopyTo(fileStream);
                }

                physicianData.Isagreementdoc = new BitArray(1, true);
            }

            if (providerProfileCm.HIPAA != null)
            {
                string path = Path.Combine(directory, "HIPAA" + Path.GetExtension(providerProfileCm.HIPAA.FileName));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    providerProfileCm.HIPAA.CopyTo(fileStream);
                }

                physicianData.Istrainingdoc = new BitArray(1, true);
            }

            if (providerProfileCm.NonDisclosure != null)
            {
                string path = Path.Combine(directory, "Non_Disclosure" + Path.GetExtension(providerProfileCm.NonDisclosure.FileName));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    providerProfileCm.NonDisclosure.CopyTo(fileStream);
                }

                physicianData.Isnondisclosuredoc = new BitArray(1, true);
            }

            if (providerProfileCm.LicenseDocument != null)
            {
                string path = Path.Combine(directory, "Licence" + Path.GetExtension(providerProfileCm.LicenseDocument.FileName));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    providerProfileCm.LicenseDocument.CopyTo(fileStream);
                }

                physicianData.Islicensedoc = new BitArray(1, true);
            }

            _context.SaveChanges();

        }



        public ProviderProfile getRegions(int flag, int filterRole)
        {
            var regionss = from r in _context.Regions
                           select (new Regions()
                           {
                               region = r.Name,
                               Regionid = r.Regionid
                           });

            if (filterRole == 1)
            {
                var roless1 = from r in _context.Roles where r.Accounttype==3 select (new Roles() { role = r.Name, Roleid = r.Roleid });

                ProviderProfile regions1 = new ProviderProfile()
                {
                    Region = regionss.ToList(),
                    roles = roless1.ToList(),
                    Flag = flag
                };

                return regions1;
            }
           
            var roless = from r in _context.Roles where r.Accounttype == 2 select (new Roles() { role = r.Name, Roleid = r.Roleid });

            ProviderProfile regions = new ProviderProfile() { 
             Region=regionss.ToList(),
             roles= roless.ToList(),
             Flag=flag
            };

            return regions;

        }



        // Create Provider Account POST
        public bool createProviderAcc(ProviderProfile cm, List<int> physicianRegions)
        {
            var aspnetuser=_context.Aspnetusers.FirstOrDefault(x=>x.Email==cm.Email);

            if(aspnetuser==null)
            {

                var aspUser = new Aspnetuser()
                {
                  Username = cm.Username,
                  Email = cm.Email,
                  Passwordhash = cm.Passwordhash,
                  Phonenumber=cm.Mobile,
                  Createddate=DateTime.Now,                 
                };

                _context.Aspnetusers.Add(aspUser);
                _context.SaveChanges();

                var netuserroleDATA = new Aspnetuserrole()
                {
                    Userid = aspUser.Id,
                    Roleid = 3,
                };
                _context.Aspnetuserroles.Add(netuserroleDATA);
                _context.SaveChanges();

                var physician = new Physician()
                {
                    Aspnetuserid = aspUser.Id,
                    Firstname = cm.Firstname,
                    Lastname = cm.Lastname,
                    Email = cm.Email,
                    Mobile = cm.Mobile,
                    Medicallicense = cm.Medicallicense,
                    Adminnotes = cm.Adminnotes,
                    Address1 = cm.Address1,
                    Address2 = cm.Address2,
                    City = cm.City,
                    Regionid = cm.Regionid,
                    Zip = cm.Zip,
                    Altphone = cm.Altphone,
                    Createdby = null,
                    Createddate = DateTime.Now,
                    Status = 1,
                    Businessname = cm.Businessname,
                    Businesswebsite = cm.Businesswebsite,
                    Roleid=cm.Roleid,
                };

                _context.Physicians.Add(physician);
                _context.SaveChanges();


                var physiciannotification = new Physiciannotification() { 
                 Physicianid=physician.Physicianid,
                    Isnotificationstopped =new BitArray(1),
                };

                _context.Physiciannotifications.Add(physiciannotification);
                _context.SaveChanges();


                foreach (int regionId in physicianRegions)
                {
                    var region = _context.Regions.FirstOrDefault(x => x.Regionid == regionId);

                    _context.Physicianregions.Add(new Physicianregion
                    {
                        Physicianid = physician.Physicianid,
                        Regionid = region.Regionid,
                    });
                    _context.SaveChanges();
                }

                var physicainLocation = new Physicianlocation()
                {
                    Physicianid = physician.Physicianid,
                    Physicianname = physician.Firstname,
                    Longitude = cm.Longitude,
                    Latitude = cm.Latitude,
                    Address = physician.Address1,
                    Createddate = DateTime.Now,
                };

                _context.Physicianlocations.Add(physicainLocation);
                _context.SaveChanges();

            



                AddProviderDocuments(physician.Physicianid, cm.Photo, cm.ContractorAgreement, cm.BackgroundCheck, cm.HIPAA, cm.NonDisclosure);
                return false;
            }

            return true;
        }



        public void AddProviderDocuments(int Physicianid, IFormFile Photo, IFormFile ContractorAgreement, IFormFile BackgroundCheck, IFormFile HIPAA, IFormFile NonDisclosure)
        {
            var physicianData = _context.Physicians.FirstOrDefault(x => x.Physicianid == Physicianid);

            string directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Documents", Physicianid.ToString());

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (Photo != null)
            {
                string path = Path.Combine(directory, "Profile" + Path.GetExtension(Photo.FileName));
                //string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "content", Photo.FileName);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    Photo.CopyTo(fileStream);
                }

                physicianData.Photo = Photo.FileName;
            }


            if (ContractorAgreement != null)
            {
                string path = Path.Combine(directory, "Independent_Contractor" + Path.GetExtension(ContractorAgreement.FileName));

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    ContractorAgreement.CopyTo(fileStream);
                }

                physicianData.Isagreementdoc = new BitArray(1, true);
            }

            if (BackgroundCheck != null)
            {
                string path = Path.Combine(directory, "Background" + Path.GetExtension(BackgroundCheck.FileName));

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    BackgroundCheck.CopyTo(fileStream);
                }

                physicianData.Isagreementdoc = new BitArray(1, true);
            }

            if (HIPAA != null)
            {
                string path = Path.Combine(directory, "HIPAA" + Path.GetExtension(HIPAA.FileName));

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    HIPAA.CopyTo(fileStream);
                }

                physicianData.Istrainingdoc = new BitArray(1, true);
            }

            if (NonDisclosure != null)
            {
                string path = Path.Combine(directory, "Non_Disclosure" + Path.GetExtension(NonDisclosure.FileName));

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    NonDisclosure.CopyTo(fileStream);
                }

                physicianData.Isnondisclosuredoc = new BitArray(1, true);
            }

            _context.SaveChanges();
        }






        //Account Access
        public ProviderMenucm accountAccessData()
        {
            BitArray deletedBit = new BitArray(new[] { false });
            var accountData = from r in _context.Roles where r.Isdeleted==deletedBit select new AccountAccess()
            {
                Roleid=r.Roleid,
                Name=r.Name,
                Accounttype=_context.Aspnetroles.FirstOrDefault(x=>x.Id==r.Accounttype).Name,
                AccounttypeId=r.Accounttype,
            };

            var data = new ProviderMenucm()
            {
                accountAccess = accountData.ToList(),
            };

            return data;
            
        }


        public ProviderMenucm createAccount()
        {
            var aspnetroles = from r in _context.Aspnetroles select r;

            var menus = from r in _context.Menus select r;

            var createAcc = new ProviderMenucm() { 
             Aspnetroles=aspnetroles.Where(x=>x.Id!=1).ToList(),
             Menus=menus.ToList(),
            };

            return createAcc;

        }

        public ProviderMenucm getMenu(int accountTypeId)
        {
            var menus = from r in _context.Menus where r.Accounttype==accountTypeId select r;

            var menu = new ProviderMenucm()
            {
                Menus = menus.ToList(),
            };

            return menu;
        }


        //POST
        public void accountAccessPost(ProviderMenucm cm, List<int> AccountMenu)
        {
            var role = new Role()
            {
                Name = cm.CreateAccountAccess.Name,
                Accounttype = cm.CreateAccountAccess.AccounttypeId,
                Createddate = DateTime.Now,
                Isdeleted = new BitArray(1, false)
            };

            _context.Roles.Add(role);
            _context.SaveChanges();

            foreach(var menuId in AccountMenu)
            {
                var roleMenu = new Rolemenu()
                {
                    Roleid = role.Roleid,
                    Menuid = menuId,
                };

                _context.Rolemenus.Add(roleMenu);
                _context.SaveChanges();
            }
           
        }



        public ProviderMenucm getEditAccAccess(int accTypeId, int roleId)
        {
            var role=_context.Roles.FirstOrDefault(x=>x.Roleid==roleId);

           


            if (role!=null)
            {
                var accountAccess = new AccountAccess()
                {
                    Roleid = role.Roleid,
                    Name = role.Name,
                    AccounttypeId = role.Accounttype
                };

                var aspnetroles=from r in _context.Aspnetroles select r;


                var menu = _context.Menus.Where(r => r.Accounttype == accTypeId).ToList();


                var rolemenu = _context.Rolemenus.ToList();

                var checkedMenu = menu.Select(r1 => new AccountMenu
                {
                    menuid = r1.Menuid,
                    name = r1.Name,
                    ExistsInTable = rolemenu.Any(r2 => r2.Roleid == roleId && r2.Menuid == r1.Menuid),

                }).ToList();

                var data = new ProviderMenucm()
                {
                    CreateAccountAccess = accountAccess,
                    Aspnetroles = aspnetroles.ToList(),
                    accountMenu= checkedMenu,

                };
                return data;

            }

            return null;
        }



        public ProviderMenucm GetAccountMenu(int accounttype, int roleid)
        {

            var menu = _context.Menus.Where(r => r.Accounttype == accounttype).ToList();


            var rolemenu = _context.Rolemenus.ToList();

            var checkedMenu = menu.Select(r1 => new AccountMenu
            {
                menuid = r1.Menuid,
                name = r1.Name,
                ExistsInTable = rolemenu.Any(r2 => r2.Roleid == roleid && r2.Menuid == r1.Menuid),

            }).ToList();

            var data = new ProviderMenucm()
            {
                accountMenu = checkedMenu,
            };

            return data;

        }



        //Edit Access Account POST
        public void editAccAccessPost(ProviderMenucm cm, List<int> AccountMenu)
        {
            var role = _context.Roles.FirstOrDefault(x => x.Roleid == cm.CreateAccountAccess.Roleid);
            if (role != null)
            {

                role.Name =cm.CreateAccountAccess.Name;
                role.Accounttype = cm.CreateAccountAccess.AccounttypeId;
                //role.Createdby = 1;
                role.Modifieddate = DateTime.Now;

                _context.SaveChanges();

                var rolemenu = _context.Rolemenus.Where(i => i.Roleid == cm.CreateAccountAccess.Roleid).ToList();
                if (rolemenu != null)
                {
                    _context.Rolemenus.RemoveRange(rolemenu);
                }

                if (AccountMenu != null)
                {
                    foreach (int menuid in AccountMenu)
                    {
                        _context.Rolemenus.Add(new Rolemenu
                        {
                            Roleid = role.Roleid,
                            Menuid = menuid,
                        });
                    }
                    _context.SaveChanges();

                }
            }
        }


        public void DeleteAccountAccess(int roleid)
        {
            var role = _context.Roles.FirstOrDefault(x => x.Roleid == roleid);
            if (role != null)
            {
                role.Isdeleted = new BitArray(1, true);
                _context.SaveChanges();
            }


            var rolemenu = _context.Rolemenus.Where(i => i.Roleid == roleid).ToList();
            if (rolemenu != null)
            {
                _context.Rolemenus.RemoveRange(rolemenu);
            }
        }




        //User Access
        public ProviderMenucm userAccess(int accountTypeId)
        {
            var Admindata = from r in _context.Admins where r.Isdeleted==null select r;
           
            var Providerdata =from r in _context.Physicians where r.Isdeleted==null select r;

            var Adrequest = _context.Requests.Where(i => i.Status != 10 || i.Status != 11).Count();
            

            var role=from r in _context.Aspnetroles where r.Id!=1 select r;

            if (accountTypeId == 3)
            {
                var physician = Providerdata.Select(r => new UserAccess()
                {
                    aspnetid = (int)r.Aspnetuserid,
                    PhysicianId=r.Physicianid,
                    Accounttype = "Provider",
                    accountname = r.Firstname + ", " + r.Lastname,
                    phone = r.Mobile,
                    status = (int)r.Status,
                    openrequest = _context.Requests.Where(i => i.Physicianid == r.Physicianid && (i.Status == 1 || i.Status == 2 || i.Status == 4 || i.Status == 5 || i.Status == 6)).Count(),
                    roleid = 3,
                }).ToList();

                var physicianData = new ProviderMenucm()
                {
                    Aspnetroles = role.ToList(),
                    Useraccess = physician,
                    Flag=1,
                };

                return physicianData;
            }

            if (accountTypeId == 2)
            {
                var admin = Admindata.Select(r => new UserAccess()
                {

                    aspnetid = r.Aspnetuserid,
                    Accounttype = "Admin",
                    accountname = r.Firstname + ", " + r.Lastname,
                    phone = r.Mobile,
                    status = (int)r.Status,
                    openrequest = Adrequest,
                    roleid = 1,
                }).ToList();

                var adminData = new ProviderMenucm()
                {
                    Aspnetroles = role.ToList(),
                    Useraccess = admin,
                    Flag = 2,
                };

                return adminData;
            }

            var Addata = Admindata.Select(r => new UserAccess()
            {

                aspnetid = r.Aspnetuserid,
                Accounttype = "Admin",
                accountname = r.Firstname + ", " + r.Lastname,
                phone = r.Mobile,
                status = (int)r.Status,
                openrequest = Adrequest,
                roleid = 1,
            }).ToList();

            var phdata = Providerdata.Select(r => new UserAccess()
            {
                aspnetid = (int)r.Aspnetuserid,
                PhysicianId = r.Physicianid,
                Accounttype = "Provider",
                accountname = r.Firstname + ", " + r.Lastname,
                phone = r.Mobile,
                status = (int)r.Status,
                openrequest = _context.Requests.Where(i => i.Physicianid == r.Physicianid && (i.Status == 1 || i.Status == 2 || i.Status == 4 || i.Status == 5 || i.Status == 6)).Count(),
                roleid = 3,
            }).ToList();

            var combineddata = Addata.Concat(phdata).ToList();

            var userAccessData = new ProviderMenucm()
            {
                Aspnetroles = role.ToList(),
                Useraccess = combineddata,
                Flag = 0,
            };

            return userAccessData;

           
        }



        //Create Admin Account POST
        public bool createAdminAccount(ProviderProfile cm, List<int> physicianRegions)
        {
            var aspnetuser = _context.Aspnetusers.FirstOrDefault(x => x.Email == cm.Email);

            

                if (aspnetuser == null)
                {

                if (cm.Email == cm.ConfirmEmail)
                {

                    var aspUser = new Aspnetuser()
                    {
                        Username = cm.Username,
                        Email = cm.Email,
                        Passwordhash = cm.Passwordhash,
                        Phonenumber = cm.Mobile,
                        Createddate = DateTime.Now,
                    };

                    _context.Aspnetusers.Add(aspUser);
                    _context.SaveChanges();

                    var netuserroleDATA = new Aspnetuserrole()
                    {
                        Userid = aspUser.Id,
                        Roleid = 2,
                    };
                    _context.Aspnetuserroles.Add(netuserroleDATA);
                    _context.SaveChanges();

                    var admin = new Admin()
                    {
                        Aspnetuserid = aspUser.Id,
                        Firstname = cm.Firstname,
                        Lastname = cm.Lastname,
                        Email = cm.Email,
                        Address1 = cm.Address1,
                        Address2 = cm.Address2,
                        Mobile = cm.Mobile,
                        Altphone = cm.Altphone,
                        City = cm.City,
                        Zip = cm.Zip,
                        Createddate = DateTime.Now,
                        Regionid = cm.Regionid,
                        Status = 1,
                        Roleid = cm.Roleid,
                    };

                    _context.Admins.Add(admin);
                    _context.SaveChanges();

                    foreach (int regionId in physicianRegions)
                    {
                        var region = _context.Regions.FirstOrDefault(x => x.Regionid == regionId);

                        _context.Adminregions.Add(new Adminregion
                        {
                            Adminid = admin.Adminid,
                            Regionid = region.Regionid,
                        });
                        _context.SaveChanges();
                    }
                    return false;
                }            
                   
                }
            

            return true;

            }



        //Provider Location
        public List<Physicianlocation> GetPhysicianlocations()
        {
            var phyLocation = _context.Physicianlocations.ToList();
            return phyLocation;
        }


        //vendors
        public Vendor GetVendors(int professionId, Vendor cm)
        {
            var query = from r in _context.Healthprofessionals
                        join rw in _context.Healthprofessionaltypes 
                        on r.Profession equals rw.Healthprofessionalid 
                        where r.Isdeleted==null
                        select (new Vendors() {
                          Vendorid=r.Vendorid,
                          Professionname=rw.Professionname,
                          Vendorname=r.Vendorname,
                          Email=r.Email,
                          Faxnumber=r.Faxnumber,
                          Phonenumber=r.Phonenumber,
                          Businesscontact=r.Businesscontact,
                          Healthprofessionalid=rw.Healthprofessionalid,
                         
                        });

            if (professionId > 0)
            {
                query=query.Where(r=>r.Healthprofessionalid==professionId);
            }

            if(cm!=null)
            {
                if (cm.Vendorname != null)
                {
                    query = query.Where(r => r.Vendorname.Contains(cm.Vendorname)).Select(r => r);
                }
               
            }
   
            var vendorData=query.ToList();

            var vendor = new Vendor()
            {
                vendors = vendorData,
                healthprofessionaltypes=_context.Healthprofessionaltypes.ToList(),
            };

            return vendor;

        }


        public Vendor getHealthProfessionals()
        {

            var professions = new Vendor()
            {
                healthprofessionaltypes = _context.Healthprofessionaltypes.ToList(),
                regions=_context.Regions.ToList(),
            };

            return professions;
        }


        //add business POST
        public void addBusiness(Vendor cm)
        {
            Healthprofessional healthprofessional = new Healthprofessional()
            {
                Vendorname = cm.Vendorname,
                Profession = cm.Healthprofessionalid,
                Faxnumber = cm.Faxnumber,
                Address = cm.Street,
                City = cm.City,
                State = _context.Regions.FirstOrDefault(x=>x.Regionid==cm.RegionId).Name,
                Zip = cm.Zip,
                Phonenumber = cm.Phonenumber,
                Email = cm.Email,
                Businesscontact = cm.Businesscontact,
                Createddate = DateTime.Now,
                Regionid=cm.RegionId,
            };

            _context.Healthprofessionals.Add(healthprofessional);
            _context.SaveChanges(); 
        }


        //delete business POST
        public void deleteBusiness(int vendorId)
        {
            var vendors=_context.Healthprofessionals.FirstOrDefault(x=>x.Vendorid==vendorId);

            if (vendors != null)
            {
                vendors.Isdeleted = new BitArray(1, true);
                vendors.Modifieddate = DateTime.Now;

                _context.SaveChanges();
            }
        }



        //edit business GET
        public Vendor getVendorDetails(int vendorId)
        {
            var vendor=_context.Healthprofessionals.FirstOrDefault(x=>x.Vendorid==vendorId);

            if(vendor != null)
            {
                var details = new Vendor()
                {
                    Vendorid = vendorId,
                    Vendorname = vendor.Vendorname,
                    Profession = vendor.Profession,
                    Faxnumber = vendor.Faxnumber,
                    Email = vendor.Email,
                    Phonenumber = vendor.Phonenumber,
                    Businesscontact = vendor.Businesscontact,
                    Street = vendor.Address,
                    City = vendor.City,
                    State = vendor.State,                 
                    RegionId = vendor.Regionid,
                    Zip = vendor.Zip,
                    regions=_context.Regions.ToList(),
                    healthprofessionaltypes=_context.Healthprofessionaltypes.ToList(),
                    Healthprofessionalid=vendor.Profession,
                    
                };

                return details;


            }

            return null;
        }


        ///Edit vendor POST
        public bool editBusiness(Vendor cm)
        {
            var vendor=_context.Healthprofessionals.FirstOrDefault(x=>x.Vendorid== cm.Vendorid);

            if(vendor != null )
            {
                vendor.Profession = cm.Healthprofessionalid;
                vendor.Faxnumber = cm.Faxnumber;
                vendor.Phonenumber = cm.Phonenumber;
                vendor.Businesscontact = cm.Businesscontact;
                vendor.Email = cm.Email;
                vendor.Address = cm.Street;
                vendor.City = cm.City;
                vendor.State = _context.Regions.FirstOrDefault(x => x.Regionid == cm.RegionId).Name;
                vendor.Zip= cm.Zip;
                vendor.Regionid = cm.RegionId;
                vendor.Vendorname = cm.Vendorname;
                vendor.Modifieddate = DateTime.Now;

                _context.SaveChanges();

                return true;
            }

            return false;
        }






        



    }

}

