using Data_Layer.CustomModels;
using Business_Logic.Interface;
using Data_Layer.DataContext;
using System.Collections;
using Data_Layer.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using System.Net;
using OfficeOpenXml;
using static Data_Layer.CustomModels.ProviderMenucm;

namespace Business_Logic.LogicRepositories
{
    public class AdminDashboardRepo : IAdminDashboard
    {
        private readonly ApplicationDbContext _db;
        public AdminDashboardRepo(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<AdminDashboardcm> data(int reqTypeId, int regionID)
        {
            var query = from r in _db.Requests
                        join rw in _db.Requestclients
                        on r.Requestid equals rw.Requestid
                        where r.Isdeleted == null
                        select (new AdminDashboardcm
                        {
                            FirstnameRequestor = r.Firstname,
                            Firstname = rw.Firstname,
                            Createddate = r.Createddate,
                            Phonenumber = r.Phonenumber,
                            Street = rw.Street,
                            Address = rw.City + " " + rw.State,
                            Strmonth = rw.Strmonth,
                            Intdate = rw.Intdate,
                            Intyear = rw.Intyear,
                            Requesttypeid = r.Requesttypeid,
                            Email = rw.Email,
                            requestClientPhonenumber = rw.Phonenumber,
                            Lastname = rw.Lastname,
                            LastnameRequestor = r.Lastname,
                            Notes = rw.Notes,
                            Status = r.Status,
                            Requestid = rw.Requestid,
                            Regionid = rw.Regionid,
                            physician = _db.Physicians.FirstOrDefault(x => x.Physicianid == r.Physicianid).Firstname,
                            PhysicianId = r.Physicianid,
                        });

            if (reqTypeId > 0)
            {
                query = query.Where(r => r.Requesttypeid == reqTypeId);
            }

            if (regionID != 0)
            {
                query = query.Where(rw => rw.Regionid == regionID);
            }

            var data = query.ToList();

            return (data);
        }

        // Block Patient
        public void blockCase(int requestId, string reasonNote)
        {
            var request = _db.Requests.FirstOrDefault(x => x.Requestid == requestId);

            if (request != null)
            {
                if (request.Isdeleted == null)
                {
                    request.Isdeleted = new BitArray(1);
                    request.Isdeleted[0] = true;
                    request.Status = 10;

                    _db.Requests.Update(request);
                    _db.SaveChanges();
                }

                Blockrequest blockrequest = new Blockrequest()
                {
                    Requestid = request.Requestid,
                    Phonenumber = request.Phonenumber,
                    Email = request.Email,
                    Createddate = DateTime.Now,
                    Reason = reasonNote,
                    Isactive = new BitArray(1, true),
                };

                _db.Blockrequests.Add(blockrequest);
                _db.SaveChanges();
            }
        }

        // Assign Case GET
        public Assigncase assigncases(int requestId)
        {
            var requestClient = _db.Requestclients.FirstOrDefault(x => x.Requestid == requestId);

            if (requestClient != null)
            {
                Assigncase assigncase = new Assigncase()
                {
                    Regions = _db.Regions.ToList()
                };

                return assigncase;
            }

            return null;
        }

        // Get Physician
        public List<Physician> getPhysicianName(int physicianId)
        {
            var query = from r in _db.Physicianregions
                        join rw in _db.Physicians on r.Physicianid equals rw.Physicianid
                        where r.Regionid == physicianId
                        select (new Physician()
                        {
                            Physicianid = rw.Physicianid,
                            Firstname = rw.Firstname,
                        });

            var data = query.ToList();

            return data;
        }

        // Assign Case POST
        public void assignCasePost(int reqId, int regionId, int physicianId, string description)
        {
            var request = _db.Requests.FirstOrDefault(x => x.Requestid == reqId);

            if (request != null)
            {
                request.Physicianid = physicianId;
                request.Status = 1;

                _db.Requests.Update(request);
                _db.SaveChanges();

                Requeststatuslog requeststatuslog = new Requeststatuslog()
                {
                    Requestid = request.Requestid,
                    Physicianid = physicianId,
                    Notes = description,
                    Createddate = DateTime.Now,
                };

                _db.Requeststatuslogs.Add(requeststatuslog);
                _db.SaveChanges();
            }
        }

        // GET View Uploads
        public ViewUploads viewUploads(int reqId, int flag)
        {
            var request = _db.Requests.FirstOrDefault(x => x.Requestid == reqId);

            if (request != null)
            {
                var query = from r in _db.Requestwisefiles
                            where r.Requestid == reqId && r.Isdeleted == null
                            select (new Documentss()
                            {
                                Filename = r.Filename,
                                Createddate = r.Createddate,
                                Requestwisefileid = r.Requestwisefileid,
                            });

                ViewUploads data = new ViewUploads()
                {
                    Requestid = request.Requestid,
                    Confirmationnumber = request.Confirmationnumber,
                    fullName = _db.Requestclients.FirstOrDefault(x => x.Requestid == reqId).Firstname + _db.Requestclients.FirstOrDefault(x => x.Requestid == reqId).Lastname,
                    documents = query.ToList(),
                    flag = flag,
                    userId = _db.Requests.FirstOrDefault(x => x.Requestid == request.Requestid).Userid,
                    IsFinalized = _db.EncounterForms.FirstOrDefault(x => x.Requestid == reqId) == null ? null : _db.EncounterForms.FirstOrDefault(x => x.Requestid == reqId).IsFinalized,
                };

                return data;
            }
            return null;
        }

        // POST view uploads
        public void postUploads(ViewUploads cm)
        {
            string filename = cm.Upload.FileName;
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Documents", filename);
            IFormFile file = cm.Upload;

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            Requestwisefile requestwisefile = new Requestwisefile()
            {
                Requestid = cm.Requestid,
                Filename = filename,
                Createddate = DateTime.Now,
            };

            _db.Requestwisefiles.Add(requestwisefile);
            _db.SaveChanges();

        }

        // Delete file in view uploads
        public void deleteUploads(int Id)
        {
            var check = _db.Requestwisefiles.FirstOrDefault(x => x.Requestwisefileid == Id);

            if (check != null)
            {
                if (check.Isdeleted == null)
                {
                    check.Isdeleted = new BitArray(1);
                    check.Isdeleted[0] = true;

                    _db.Requestwisefiles.Update(check);
                    _db.SaveChanges();
                }
            }
        }

        // Send Mail in view uploads
        public void sendMail(int reqID)
        {
            string smtpServer = "outlook.office365.com";
            int port = 587; // Port number for SMTP (e.g., 587 for Gmail)
            string senderEmail = "tatva.dotnet.saketbarevadia@outlook.com";
            string password = "S@ket6898";
            string recipientEmail = "saketpatel139@gmail.com";
            string subject = "Documents of Request";
            string body = "Find the files uploaded for your request below";
            using (SmtpClient client = new SmtpClient(smtpServer, port))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(senderEmail, password);

                using (MailMessage message = new MailMessage(senderEmail, recipientEmail, subject, body))
                {
                    string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Documents");
                    string[] files = Directory.GetFiles(directoryPath);
                    var request = from r in _db.Requestwisefiles where r.Requestid == reqID && r.Isdeleted == null select r.Filename;
                    foreach (string file in files)
                    {
                        foreach (var item in request)
                        {
                            if (item == file.Substring(directoryPath.Length + 1))
                            {
                                message.Attachments.Add(new Attachment(file));
                            }
                        }
                    }

                    client.Send(message);
                }
            }
        }

        //  Orders
        // get Profession
        public Orders getProfessions(int id)
        {
            var professions = from r in _db.Healthprofessionaltypes
                              select (new Profession()
                              {
                                  Healthprofessionalid = r.Healthprofessionalid,
                                  Professionname = r.Professionname,

                              });

            var ans = professions.ToList();

            Orders orders = new Orders()
            {
                Requestid = id,
                professions = ans,
            };

            return (orders);
        }

        // Get Vendor
        public List<Profession> getVendor(int id)
        {
            var data = from r in _db.Healthprofessionals
                       where r.Profession == id
                       select (new Profession()
                       {
                           Vendorname = r.Vendorname,
                           Vendorid = r.Vendorid,
                       });

            var vendor = data.ToList();

            return (vendor);
        }

        // Get vendor details
        public Orders getVendorDetails(int id)
        {

            var match = _db.Healthprofessionals.FirstOrDefault(x => x.Vendorid == id);


            if (match != null)
            {
                Orders details = new Orders()
                {
                    Email = match.Email,
                    Faxnumber = match.Faxnumber,
                    Businesscontact = match.Businesscontact,
                };

                return (details);
            }

            return null;
        }

        // Post order details
        public bool postOrderDetails(Orders cm, int id)
        {
            var vendorId = _db.Healthprofessionals.FirstOrDefault(x => x.Vendorid == id);

            if (vendorId != null)
            {
                vendorId.Faxnumber = cm.Faxnumber;
                vendorId.Businesscontact = cm.Businesscontact;
                vendorId.Email = cm.Email;

                _db.Healthprofessionals.Update(vendorId);
                _db.SaveChanges();

                Orderdetail orderdetail = new Orderdetail()
                {
                    Vendorid = vendorId.Vendorid,
                    Requestid = cm.Requestid,
                    Email = cm.Email,
                    Faxnumber = cm.Faxnumber,
                    Businesscontact = cm.Businesscontact,
                    Prescription = cm.Prescription,
                    Noofrefill = cm.Noofrefill,
                    Createddate = DateTime.Now,
                };

                _db.Orderdetails.Add(orderdetail);
                _db.SaveChanges();

                return true;
            }
            else
            {
                return false;
            }
        }

        // Transfer request
        public List<TransferRequest> transferRequests(int requestId)
        {
            var requestClient = _db.Requestclients.FirstOrDefault(x => x.Requestid == requestId);

            if (requestClient != null)
            {
                var query = from r in _db.Regions
                            select (new TransferRequest()
                            {
                                Name = r.Name,
                                Regionid = r.Regionid,
                            });

                var data = query.ToList();

                return data;
            }

            return null;
        }

        // Transfer request post
        public void transferReqPost(int reqId, int regionId, int physicianId, string description, string email)
        {
            var request = _db.Requests.FirstOrDefault(x => x.Requestid == reqId);

            var aspnetId = _db.Aspnetusers.FirstOrDefault(x => x.Email == email).Id;

            var adminID = _db.Admins.FirstOrDefault(x => x.Aspnetuserid == aspnetId).Adminid;

            if (request != null)
            {
                request.Physicianid = physicianId;

                _db.Requests.Update(request);
                _db.SaveChanges();

                Requeststatuslog requeststatuslog = new Requeststatuslog()
                {
                    Requestid = request.Requestid,
                    Transtophysicianid = physicianId,
                    Notes = description,
                    Createddate = DateTime.Now,
                    Adminid = adminID,
                    Status = _db.Requests.FirstOrDefault(x => x.Requestid == reqId).Status,
                };

                _db.Requeststatuslogs.Add(requeststatuslog);
                _db.SaveChanges();
            }
        }

        // Clear case POST
        public bool clearCasePost(int id)
        {
            var request = _db.Requests.FirstOrDefault(x => x.Requestid == id);

            if (request != null)
            {
                request.Status = 11;

                _db.Requests.Update(request);
                _db.SaveChanges();

                Requeststatuslog requeststatuslog = new Requeststatuslog()
                {
                    Requestid = id,
                    Status = request.Status,
                    Createddate = DateTime.Now,
                };

                _db.Requeststatuslogs.Add(requeststatuslog);
                _db.SaveChanges();
                return true;
            }

            return false;
        }

        // Send Agreement 
        public SendAgreement SendAgreement(int id)
        {
            var request = _db.Requests.FirstOrDefault(x => x.Requestid == id);

            if (request != null)
            {
                SendAgreement agreement = new SendAgreement()
                {
                    Requestid = request.Requestid,
                    Requesttypeid = request.Requesttypeid,
                    Phonenumber = _db.Requestclients.FirstOrDefault(x => x.Requestid == id).Phonenumber,
                    Email = _db.Requestclients.FirstOrDefault(x => x.Requestid == id).Email,
                };

                return (agreement);
            }

            return null;
        }

        // Send Agreement POST
        public void sendEmail(SendAgreement cm)
        {
            string smtpServer = "outlook.office365.com";
            int port = 587; // Port number for SMTP (e.g., 587 for Gmail)
            string senderEmail = "tatva.dotnet.saketbarevadia@outlook.com";
            string password = "S@ket6898";
            string recipientEmail = cm.Email;
            string subject = "Click the link given below to open the Agreement";
            string body = "http://localhost:5001/Home/reviewAgreement?reqId=" + cm.Requestid;
            using (SmtpClient client = new SmtpClient(smtpServer, port))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(senderEmail, password);

                using (MailMessage message = new MailMessage(senderEmail, recipientEmail, subject, body))
                {

                    client.Send(message);
                }
            }
        }

        // Close Case
        public CloseCase CloseCase(int reqid)
        {
            var request = _db.Requestclients.FirstOrDefault(x => x.Requestid == reqid);

            if (request != null)
            {
                var files = from r in _db.Requestwisefiles
                            where r.Requestid == reqid
                            select (new Documentss()
                            {
                                Filename = r.Filename,
                                Requestwisefileid = r.Requestid,
                                Createddate = r.Createddate,
                            });

                var filenames = files.ToList();

                CloseCase patientDetails = new CloseCase()
                {
                    Requestid = reqid,
                    Firstname = request.Firstname,
                    Lastname = request.Lastname,
                    fullName = request.Firstname + request.Lastname,
                    Confirmationnumber = _db.Requests.FirstOrDefault(x => x.Requestid == reqid).Confirmationnumber,
                    Email = request.Email,
                    Phonenumber = request.Phonenumber,
                    Strmonth = request.Strmonth,
                    Intdate = request.Intdate,
                    Intyear = request.Intyear,
                    Date = new DateTime((int)request.Intyear, Convert.ToInt16(request.Strmonth), (int)request.Intdate).ToString("yyyy-MM-dd"),
                    documents = filenames,
                };

                return patientDetails;
            }

            return null;
        }

        // Close Case POST
        public void closeCasePost(CloseCase cm)
        {
            var patient = _db.Requestclients.FirstOrDefault(x => x.Requestid == cm.Requestid);

            if (patient != null)
            {
                patient.Email = cm.Email;
                patient.Phonenumber = cm.Phonenumber;

                _db.Requestclients.Update(patient);
                _db.SaveChanges();
            }
        }

        // Close POST
        public void close(int reqid)
        {
            var request = _db.Requests.FirstOrDefault(x => x.Requestid == reqid);

            if (request != null)
            {
                request.Status = 9;
                request.Modifieddate = DateTime.Now;

                _db.Requests.Update(request);
                _db.SaveChanges();
            }

            Requeststatuslog requestStatusLog = new Requeststatuslog();
            requestStatusLog.Requestid = reqid;
            requestStatusLog.Createddate = DateTime.Now;
            requestStatusLog.Status = 9;

            _db.Requeststatuslogs.Add(requestStatusLog);
            _db.SaveChanges();
        }

        // Admin My Profile
        public AdminProfilecm adminData(string email, int flag)
        {
            var username = _db.Aspnetusers.FirstOrDefault(x => x.Email == email);

            var admin = _db.Admins.FirstOrDefault(x => x.Email == email);

            if (admin != null)
            {
                var region = _db.Regions.ToList();

                var phyRegion = _db.Adminregions.ToList();

                var adminId = admin.Adminid;

                var checkedRegion = region.Select(r1 => new AdminregionTable
                {
                    Regionid = r1.Regionid,
                    Name = r1.Name,
                    ExistsInTable = phyRegion.Any(r2 => r2.Adminid == adminId && r2.Regionid == r1.Regionid),
                }).ToList();

                var regionss = from r in _db.Regions
                               select (new Regions()
                               {
                                   region = r.Name,
                                   Regionid = r.Regionid
                               });

                if (flag == 1)
                {
                    AdminProfilecm adminData = new AdminProfilecm()
                    {
                        AspnetUserId = username.Id,
                        Email = admin.Email,
                        Username = username.Username,
                        Firstname = admin.Firstname,
                        Lastname = admin.Lastname,
                        Mobile = admin.Mobile,
                        Address1 = admin.Address1,
                        Address2 = admin.Address2,
                        City = admin.City,
                        Zip = admin.Zip,
                        Adminid = admin.Adminid,
                        back = 1,
                        adminregionTables = checkedRegion,
                        Altphone = admin.Altphone,
                        Region = regionss.ToList(),
                        Regionid = admin.Regionid,
                    };

                    var data = adminData;

                    return adminData;
                }

                if (flag == 2)
                {
                    AdminProfilecm adminData = new AdminProfilecm()
                    {
                        AspnetUserId = username.Id,
                        Email = admin.Email,
                        Username = username.Username,
                        Firstname = admin.Firstname,
                        Lastname = admin.Lastname,
                        Mobile = admin.Mobile,
                        Address1 = admin.Address1,
                        Address2 = admin.Address2,
                        City = admin.City,
                        Zip = admin.Zip,
                        Adminid = admin.Adminid,
                        back = 2,
                        adminregionTables = checkedRegion,
                        Altphone = admin.Altphone,
                        Region = regionss.ToList(),
                        Regionid = admin.Regionid,
                    };

                    var data = adminData;

                    return adminData;
                }
            }

            return null;
        }

        // Admin My Profile POST
        public void EditadminProfile(AdminProfilecm cm, List<int> adminRegions)
        {
            var admin = _db.Admins.FirstOrDefault(x => x.Adminid == cm.Adminid);
            var aspId = _db.Admins.FirstOrDefault(x => x.Adminid == cm.Adminid).Aspnetuserid;
            var aspnetuser = _db.Aspnetusers.FirstOrDefault(x => x.Id == aspId);

            if (admin != null)
            {
                if (cm.Email == cm.ConfirmEmail)
                {
                    admin.Firstname = cm.Firstname;
                    admin.Lastname = cm.Lastname;
                    admin.Email = cm.Email;
                    admin.Mobile = cm.Mobile;


                    _db.Admins.Update(admin);
                    _db.SaveChanges();

                    aspnetuser.Email = cm.Email;
                    aspnetuser.Username = cm.Firstname;

                    _db.Aspnetusers.Update(aspnetuser);
                    _db.SaveChanges();

                    if (_db.Adminregions.Any(x => x.Adminid == admin.Adminid))
                    {
                        var adminRegion = _db.Adminregions.Where(x => x.Adminid == admin.Adminid).ToList();

                        _db.Adminregions.RemoveRange(adminRegion);
                        _db.SaveChanges();
                    }

                    foreach (int regionId in adminRegions)
                    {
                        var region = _db.Regions.FirstOrDefault(x => x.Regionid == regionId);

                        _db.Adminregions.Add(new Adminregion()
                        {
                            Adminid = admin.Adminid,
                            Regionid = region.Regionid,
                        });
                        _db.SaveChanges();
                    }

                }
            }

        }

        // Admin My Profile reset password POST
        public void resetPass(AdminProfilecm cm)
        {
            var aspId = _db.Admins.FirstOrDefault(x => x.Adminid == cm.Adminid).Aspnetuserid;
            var aspnetuser = _db.Aspnetusers.FirstOrDefault(x => x.Id == aspId);

            if (aspnetuser != null)
            {
                aspnetuser.Passwordhash = cm.Password;

                _db.Aspnetusers.Update(aspnetuser);
                _db.SaveChanges();
            }
        }

        // Admin My Profile edit address POST
        public void editAdminAddress(AdminProfilecm cm)
        {
            var admin = _db.Admins.FirstOrDefault(x => x.Adminid == cm.Adminid);

            if (admin != null)
            {
                admin.Address1 = cm.Address1;
                admin.Address2 = cm.Address2;
                admin.City = cm.City;
                admin.Zip = cm.Zip;
                admin.Altphone = cm.Altphone;
                admin.Regionid = cm.Regionid;

                _db.Admins.Update(admin);
                _db.SaveChanges();
            }
        }

        // Encounter form GET
        public Encounter encounterFormData(int reqid)
        {
            var request = _db.Requestclients.FirstOrDefault(x => x.Requestid == reqid);
            var encounterr = _db.EncounterForms.FirstOrDefault(x => x.Requestid == reqid);

            if (request != null)
            {
                if (encounterr != null)
                {
                    Encounter encounter = new Encounter()
                    {
                        Requestid = reqid,
                        Firstname = request.Firstname,
                        Lastname = request.Lastname,
                        Location = request.Location,
                        Email = request.Email,
                        Strmonth = request.Strmonth,
                        Intdate = request.Intdate,
                        Intyear = request.Intyear,
                        DateofBirth = new DateTime((int)request.Intyear, Convert.ToInt16(request.Strmonth), (int)request.Intdate).ToString("yyyy-MM-dd"),
                        Mobile = request.Phonenumber,
                        HistoryIllness = encounterr.HistoryIllness,
                        MedicalHistory = encounterr.MedicalHistory,
                        Medications = encounterr.Medications,
                        Allergies = encounterr.Allergies,
                        Temp = encounterr.Temp,
                        Hr = encounterr.Hr,
                        Rr = encounterr.Rr,
                        BpS = encounterr.BpS,
                        BpD = encounterr.BpD,
                        O2 = encounterr.O2,
                        Pain = encounterr.Pain,
                        Heent = encounterr.Heent,
                        Cv = encounterr.Cv,
                        Chest = encounterr.Chest,
                        Abd = encounterr.Abd,
                        Extr = encounterr.Extr,
                        Skin = encounterr.Skin,
                        Neuro = encounterr.Neuro,
                        Other = encounterr.Other,
                        Diagnosis = encounterr.Diagnosis,
                        TreatmentPlan = encounterr.TreatmentPlan,
                        MedicationDispensed = encounterr.MedicationDispensed,
                        Procedures = encounterr.Procedures,
                        FollowUp = encounterr.FollowUp,
                        Date = encounterr.Date,
                        IsFinalized = encounterr.IsFinalized,
                    };
                    return encounter;
                }

                else
                {
                    Encounter encounter = new Encounter()
                    {
                        Requestid = reqid,
                        Firstname = request.Firstname,
                        Lastname = request.Lastname,
                        Location = request.Location,
                        Email = request.Email,
                        Strmonth = request.Strmonth,
                        Intdate = request.Intdate,
                        Intyear = request.Intyear,
                        DateofBirth = new DateTime((int)request.Intyear, Convert.ToInt16(request.Strmonth), (int)request.Intdate).ToString("yyyy-MM-dd"),
                        Mobile = request.Phonenumber,
                    };
                    return encounter;
                }
            }

            return null;
        }

        // Encounter POST
        public void encounterFormPost(Encounter cm)
        {

            var request = _db.Requestclients.FirstOrDefault(x => x.Requestid == cm.Requestid);
            var encounterr = _db.EncounterForms.FirstOrDefault(x => x.Requestid == cm.Requestid);

            if (request != null)
            {
                request.Firstname = cm.Firstname;
                request.Lastname = cm.Lastname;
                request.Location = cm.Location;
                request.Email = cm.Email;
                request.Phonenumber = cm.Mobile;
                request.Strmonth = cm.DateofBirth.Substring(5, 2);
                request.Intdate = Convert.ToInt16(cm.DateofBirth.Substring(8, 2));
                request.Intyear = Convert.ToInt16(cm.DateofBirth.Substring(0, 4));

                _db.Requestclients.Update(request);
                _db.SaveChanges();
            }

            if (encounterr == null)
            {
                EncounterForm form = new EncounterForm()
                {
                    Requestid = request.Requestid,
                    HistoryIllness = cm.HistoryIllness,
                    MedicalHistory = cm.MedicalHistory,
                    Medications = cm.Medications,
                    Allergies = cm.Allergies,
                    Temp = cm.Temp,
                    Hr = cm.Hr,
                    Rr = cm.Rr,
                    BpS = cm.BpS,
                    BpD = cm.BpD,
                    O2 = cm.O2,
                    Pain = cm.Pain,
                    Heent = cm.Heent,
                    Cv = cm.Cv,
                    Chest = cm.Chest,
                    Abd = cm.Abd,
                    Extr = cm.Extr,
                    Skin = cm.Skin,
                    Neuro = cm.Neuro,
                    Other = cm.Other,
                    Diagnosis = cm.Diagnosis,
                    TreatmentPlan = cm.TreatmentPlan,
                    MedicationDispensed = cm.MedicationDispensed,
                    Procedures = cm.Procedures,
                    FollowUp = cm.FollowUp,
                    Date = cm.Date
                };

                _db.EncounterForms.Add(form);
                _db.SaveChanges();
            }

            if (encounterr != null)
            {
                encounterr.Requestid = request.Requestid;
                encounterr.HistoryIllness = cm.HistoryIllness;
                encounterr.MedicalHistory = cm.MedicalHistory;
                encounterr.Medications = cm.Medications;
                encounterr.Allergies = cm.Allergies;
                encounterr.Temp = cm.Temp;
                encounterr.Hr = cm.Hr;
                encounterr.Rr = cm.Rr;
                encounterr.BpS = cm.BpS;
                encounterr.BpD = cm.BpD;
                encounterr.O2 = cm.O2;
                encounterr.Pain = cm.Pain;
                encounterr.Heent = cm.Heent;
                encounterr.Cv = cm.Cv;
                encounterr.Chest = cm.Chest;
                encounterr.Abd = cm.Abd;
                encounterr.Extr = cm.Extr;
                encounterr.Skin = cm.Skin;
                encounterr.Neuro = cm.Neuro;
                encounterr.Other = cm.Other;
                encounterr.Diagnosis = cm.Diagnosis;
                encounterr.TreatmentPlan = cm.TreatmentPlan;
                encounterr.MedicationDispensed = cm.MedicationDispensed;
                encounterr.Procedures = cm.Procedures;
                encounterr.FollowUp = cm.FollowUp;
                encounterr.Date = cm.Date;

                _db.EncounterForms.Update(encounterr);
                _db.SaveChanges();
            }
        }

        // Send Link
        public void sendLink(SendAgreement cm)
        {
            string smtpServer = "outlook.office365.com";
            int port = 587; // Port number for SMTP (e.g., 587 for Gmail)
            string senderEmail = "tatva.dotnet.saketbarevadia@outlook.com";
            string password = "S@ket6898";
            string recipientEmail = cm.Email;
            string subject = "Click the link given below to Submit a Request";
            string body = "http://localhost:5001/Home/submitReq";
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
                Roleid = 1,
                Emailtemplate = "Sender : " + senderEmail + "Reciver :" + recipientEmail + "Subject : " + subject + "Message : " + body,
                Isemailsent = new BitArray(1, true),
                Createdate = DateTime.Now,
                Sentdate = DateTime.Now,

                Senttries = 1,
            };

            _db.Emaillogs.Add(emaillog);
            _db.SaveChanges();
        }

        // Admin create request POST
        public void adminCreateReq(AdminCreateReq cm, string email)
        {

            var admin = _db.Admins.FirstOrDefault(x => x.Email == email);

            var physician = _db.Physicians.FirstOrDefault(x => x.Email == email);

            var user = _db.Users.FirstOrDefault(x => x.Email == cm.Email);

            var asp = _db.Aspnetusers.FirstOrDefault(x => x.Email == cm.Email);

            if (asp == null)
            {
                var aspnetuser = new Aspnetuser()
                {
                    Email = cm.Email,
                    Phonenumber = cm.Mobile,
                    Username = cm.Firstname + cm.Lastname,
                };

                _db.Aspnetusers.Add(aspnetuser);
                _db.SaveChanges();

                string smtpServer = "outlook.office365.com";
                int port = 587; // Port number for SMTP (e.g., 587 for Gmail)
                string senderEmail = "tatva.dotnet.saketbarevadia@outlook.com";
                string password = "S@ket6898";
                string recipientEmail = cm.Email;
                string subject = "Click the link given below to create your account";
                string body = "http://localhost:5001/Home/createAccount?aspnetId=" + aspnetuser.Id;

                using (SmtpClient client = new SmtpClient(smtpServer, port))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(senderEmail, password);

                    using (MailMessage message = new MailMessage(senderEmail, recipientEmail, subject, body))
                    {
                        client.Send(message);
                    }
                }

                if (user == null)
                {
                    var userTb = new User();
                    userTb.Aspnetuserid = aspnetuser.Id;
                    userTb.Createdby = aspnetuser.Id;
                    userTb.Createddate = DateTime.Now;
                    userTb.Firstname = cm.Firstname;
                    userTb.Lastname = cm.Lastname;
                    userTb.Email = cm.Email;
                    userTb.Mobile = cm.Mobile;
                    userTb.Street = cm.Street;
                    userTb.City = cm.City;
                    userTb.State = cm.State;
                    userTb.Zipcode = cm.Zipcode;
                    userTb.Strmonth = cm.Strmonth.Substring(5, 2);
                    userTb.Intdate = Convert.ToInt16(cm.Strmonth.Substring(8, 2));
                    userTb.Intyear = Convert.ToInt16(cm.Strmonth.Substring(0, 4));
                    _db.Users.Add(userTb);
                    _db.SaveChanges();
                }

                var netuserroleDATA = new Aspnetuserrole()
                {
                    Userid = aspnetuser.Id,
                    Roleid = 1,
                };
                _db.Aspnetuserroles.Add(netuserroleDATA);
                _db.SaveChanges();

                Request request = new Request();
                request.Firstname = admin == null ? physician.Firstname : admin.Firstname;
                request.Lastname = admin == null ? physician.Lastname : admin.Lastname;
                request.Email = admin == null ? physician.Email : admin.Email;
                request.Phonenumber = admin == null ? physician.Mobile : admin.Mobile;
                request.Createddate = DateTime.Now;
                request.Status = 1;
                request.Confirmationnumber = admin == null ? physician.Firstname.Substring(0, 2) + DateTime.Now.ToString().Substring(0, 19).Replace(" ", "") : admin.Firstname.Substring(0, 2) + DateTime.Now.ToString().Substring(0, 19).Replace(" ", "");
                request.Userid = _db.Users.FirstOrDefault(x => x.Email == cm.Email).Userid;


                _db.Requests.Add(request);
                _db.SaveChanges();

                Requestclient requestclient = new Requestclient()
                {
                    Firstname = cm.Firstname,
                    Lastname = cm.Lastname,
                    Email = cm.Email,
                    Phonenumber = cm.Mobile,
                    State = _db.Regions.FirstOrDefault(x => x.Regionid == Convert.ToInt16(cm.State)).Name,
                    Street = cm.Street,
                    City = cm.City,
                    Zipcode = cm.Zipcode,
                    Requestid = request.Requestid,
                    Strmonth = cm.Strmonth.Substring(5, 2),
                    Intdate = Convert.ToInt16(cm.Strmonth.Substring(8, 2)),
                    Intyear = Convert.ToInt16(cm.Strmonth.Substring(0, 4)),
                    Regionid = Convert.ToInt16(cm.State),
                };

                _db.Requestclients.Add(requestclient);
                _db.SaveChanges();

                var userIdd = _db.Users.FirstOrDefault(x => x.Email == cm.Email).Userid;

                var requestId = _db.Requests.FirstOrDefault(x => x.Userid == userIdd).Requestid;

                Emaillog emaillog = new Emaillog()
                {
                    Subjectname = subject,
                    Emailid = cm.Email,
                    Roleid = 1,
                    Emailtemplate = "Sender : " + senderEmail + "Reciver :" + recipientEmail + "Subject : " + subject + "Message : " + body,
                    Isemailsent = new BitArray(1, true),
                    Requestid = requestId,
                    Createdate = DateTime.Now,
                    Sentdate = DateTime.Now,
                    Confirmationnumber = _db.Requestclients.FirstOrDefault(x => x.Requestid == requestId).Firstname.Substring(0, 2) + DateTime.Now.ToString().Substring(0, 19).Replace(" ", ""),
                    Senttries = 1,
                };

                _db.Emaillogs.Add(emaillog);
                _db.SaveChanges();


                Requestnote requestnote = new Requestnote()
                {
                    Requestid = request.Requestid,
                    Adminnotes = admin == null ? null : cm.Notes,
                    Physiciannotes = physician == null ? null : cm.Notes,
                    Createddate = DateTime.Now,
                    Createdby = admin == null ? Convert.ToInt32(physician.Createdby) : Convert.ToInt32(admin.Createdby),
                };

                _db.Requestnotes.Add(requestnote);
                _db.SaveChanges();
            }

            if (asp != null)
            {
                if (user == null)
                {
                    var userTb = new User();
                    userTb.Aspnetuserid = asp.Id;
                    userTb.Createdby = asp.Id;
                    userTb.Createddate = DateTime.Now;
                    userTb.Firstname = cm.Firstname;
                    userTb.Lastname = cm.Lastname;
                    userTb.Email = cm.Email;
                    userTb.Mobile = cm.Mobile;
                    userTb.Street = cm.Street;
                    userTb.City = cm.City;
                    userTb.State = cm.State;
                    userTb.Zipcode = cm.Zipcode;
                    userTb.Strmonth = cm.Strmonth.Substring(5, 2);
                    userTb.Intdate = Convert.ToInt16(cm.Strmonth.Substring(8, 2));
                    userTb.Intyear = Convert.ToInt16(cm.Strmonth.Substring(0, 4));
                    _db.Users.Add(userTb);
                    _db.SaveChanges();
                }

                Request request = new Request();
                request.Firstname = admin == null ? physician.Firstname : admin.Firstname;
                request.Lastname = admin == null ? physician.Lastname : admin.Lastname;
                request.Email = admin == null ? physician.Email : admin.Email;
                request.Phonenumber = admin == null ? physician.Mobile : admin.Mobile;
                request.Createddate = DateTime.Now;
                request.Status = 1;
                request.Confirmationnumber = admin == null ? physician.Firstname.Substring(0, 2) + DateTime.Now.ToString().Substring(0, 19).Replace(" ", "") : admin.Firstname.Substring(0, 2) + DateTime.Now.ToString().Substring(0, 19).Replace(" ", "");
                request.Userid = _db.Users.FirstOrDefault(x => x.Email == cm.Email).Userid;

                _db.Requests.Add(request);
                _db.SaveChanges();

                Requestclient requestclient = new Requestclient()
                {
                    Firstname = cm.Firstname,
                    Lastname = cm.Lastname,
                    Email = cm.Email,
                    Phonenumber = cm.Mobile,
                    State = cm.State,
                    Street = cm.Street,
                    City = cm.City,
                    Zipcode = cm.Zipcode,
                    Requestid = request.Requestid,
                    Strmonth = cm.Strmonth.Substring(5, 2),
                    Intdate = Convert.ToInt16(cm.Strmonth.Substring(8, 2)),
                    Intyear = Convert.ToInt16(cm.Strmonth.Substring(0, 4)),
                };

                _db.Requestclients.Add(requestclient);
                _db.SaveChanges();

                Requestnote requestnote = new Requestnote()
                {
                    Requestid = request.Requestid,
                    Adminnotes = admin == null ? null : cm.Notes,
                    Physiciannotes = physician == null ? null : cm.Notes,
                    Createddate = DateTime.Now,
                    Createdby = admin == null ? Convert.ToInt32(physician.Createdby) : Convert.ToInt32(admin.Createdby),
                };

                _db.Requestnotes.Add(requestnote);
                _db.SaveChanges();
            }
        }

        // Get Regions
        public List<Regioncm> getRegions()
        {
            var query = from r in _db.Regions
                        select (new Regioncm()
                        {
                            Region = r.Name,
                            Regionid = r.Regionid
                        });

            var regions = query.ToList();

            return regions;
        }

        public byte[] GenerateExcelFile(List<AdminDashboardcm> adminData)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial; using (var excelPackage = new ExcelPackage())
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("Requests");

                worksheet.Cells[1, 1].Value = "Request ID";
                worksheet.Cells[1, 2].Value = "Name";
                worksheet.Cells[1, 3].Value = "DOB";
                worksheet.Cells[1, 4].Value = "Address";
                worksheet.Cells[1, 5].Value = "Requestor";
                worksheet.Cells[1, 6].Value = "Phone";
                worksheet.Cells[1, 7].Value = "Request Date";
                worksheet.Cells[1, 8].Value = "Request Type ID";
                worksheet.Cells[1, 9].Value = "Notes";
                worksheet.Cells[1, 10].Value = "Email";

                for (int i = 0; i < adminData.Count; i++)
                {
                    var rowData = adminData[i];
                    worksheet.Cells[i + 2, 1].Value = rowData.Requestid;
                    worksheet.Cells[i + 2, 2].Value = rowData.Firstname;
                    worksheet.Cells[i + 2, 3].Value = rowData.Strmonth;
                    worksheet.Cells[i + 2, 4].Value = rowData.Address;
                    worksheet.Cells[i + 2, 5].Value = rowData.FirstnameRequestor;
                    worksheet.Cells[i + 2, 6].Value = rowData.Phonenumber;
                    worksheet.Cells[i + 2, 7].Value = rowData.Createddate;
                    worksheet.Cells[i + 2, 8].Value = rowData.Requesttypeid;
                    worksheet.Cells[i + 2, 9].Value = rowData.Notes;
                    worksheet.Cells[i + 2, 10].Value = rowData.Email;
                }

                return excelPackage.GetAsByteArray();
            }
        }

        //scheduling

        public List<Physician> GetPhysicians(int regionid)
        {
            if (regionid == 0)
            {
                var physicians = _db.Physicians.ToList();
                return physicians;
            }
            else
            {
                var physicians1 = _db.Physicians.Where(i => i.Regionid == regionid).ToList();
                return physicians1;
            }

        }

        public SchedulingCm GetRegions(int phyId)
        {
            var regions = from r in _db.Regions select r;

            if (phyId > 0)
            {
                var phyreg = _db.Physicianregions.Where(i => i.Physicianid == phyId).Select(j => j.Region).ToList();

                var regio = new SchedulingCm()
                {
                    regions = phyreg,
                };
                return regio;

            }

            var regionsss = regions.ToList();

            var regionss = new SchedulingCm()
            {
                regions = regionsss,
            };
            return regionss;
        }

        //create shift POST
        public bool createShift(ScheduleModel scheduleModel, string email)
        {
            var Aspid = _db.Aspnetusers.FirstOrDefault(x => x.Email == email).Id;

            if (_db.Shifts.Where(x => x.Physicianid == scheduleModel.Physicianid).Count() > 1)
            {
                var shiftData = _db.Shifts.Where(i => i.Physicianid == scheduleModel.Physicianid).ToList();
                var shiftDetailData = new List<Shiftdetail>();

                foreach (var obj in shiftData)
                {
                    var details = _db.Shiftdetails.Where(x => x.Shiftid == obj.Shiftid).ToList();
                    shiftDetailData.AddRange(details);
                }

                foreach (var obj in shiftDetailData)
                {
                    var shiftDate = new DateTime(scheduleModel.Startdate.Year, scheduleModel.Startdate.Month, scheduleModel.Startdate.Day);

                    if (obj.Shiftdate.Date == shiftDate.Date)
                    {
                        if ((obj.Starttime <= scheduleModel.Starttime && obj.Endtime >= scheduleModel.Starttime) || (obj.Starttime <= scheduleModel.Endtime && obj.Endtime >= scheduleModel.Endtime) || (obj.Starttime >= scheduleModel.Starttime && obj.Endtime <= scheduleModel.Endtime))
                        {
                            return false;
                        }
                    }
                }
            }

            Shift shift = new Shift();
            shift.Physicianid = scheduleModel.Physicianid;
            shift.Repeatupto = scheduleModel.Repeatupto;
            shift.Startdate = scheduleModel.Startdate;
            shift.Createdby = Aspid;
            shift.Createddate = DateTime.Now;
            shift.Isrepeat = scheduleModel.Isrepeat == false ? new BitArray(1, false) : new BitArray(1, true);
            shift.Repeatupto = scheduleModel.Repeatupto;
            shift.Weekdays = scheduleModel.checkWeekday;
            _db.Shifts.Add(shift);
            _db.SaveChanges();

            Shiftdetail sd = new Shiftdetail();
            sd.Shiftid = shift.Shiftid;
            sd.Shiftdate = new DateTime(scheduleModel.Startdate.Year, scheduleModel.Startdate.Month, scheduleModel.Startdate.Day);
            sd.Starttime = scheduleModel.Starttime;
            sd.Endtime = scheduleModel.Endtime;
            sd.Regionid = scheduleModel.Regionid;
            sd.Status = 1;
            sd.Isdeleted = new BitArray(1, false);
            _db.Shiftdetails.Add(sd);
            _db.SaveChanges();

            Shiftdetailregion sr = new Shiftdetailregion();
            sr.Shiftdetailid = sd.Shiftdetailid;
            sr.Regionid = scheduleModel.Regionid;
            sr.Isdeleted = new BitArray(1, false);
            _db.Shiftdetailregions.Add(sr);
            _db.SaveChanges();

            if (scheduleModel.Isrepeat != false)
            {
                List<int> day = scheduleModel.checkWeekday.Split(',').Select(int.Parse).ToList();

                foreach (int d in day)
                {
                    DayOfWeek desiredDayOfWeek = (DayOfWeek)d;
                    DateTime today = DateTime.Today;
                    DateTime nextOccurrence = new DateTime(scheduleModel.Startdate.Year, scheduleModel.Startdate.Month, scheduleModel.Startdate.Day);
                    int occurrencesFound = 0;
                    while (occurrencesFound < scheduleModel.Repeatupto)
                    {
                        if (nextOccurrence.DayOfWeek == desiredDayOfWeek && (nextOccurrence.Day != scheduleModel.Startdate.Day))
                        {
                            Shiftdetail sdd = new Shiftdetail();
                            sdd.Shiftid = shift.Shiftid;
                            sdd.Shiftdate = nextOccurrence;
                            sdd.Starttime = scheduleModel.Starttime;
                            sdd.Endtime = scheduleModel.Endtime;
                            sdd.Regionid = scheduleModel.Regionid;
                            sdd.Status = 1;
                            sdd.Isdeleted = new BitArray(1, false);
                            _db.Shiftdetails.Add(sdd);
                            _db.SaveChanges();

                            Shiftdetailregion srr = new Shiftdetailregion();
                            srr.Shiftdetailid = sdd.Shiftdetailid;
                            srr.Regionid = scheduleModel.Regionid;
                            srr.Isdeleted = new BitArray(1, false);
                            _db.Shiftdetailregions.Add(srr);
                            _db.SaveChanges();
                            occurrencesFound++;
                        }
                        nextOccurrence = nextOccurrence.AddDays(1);
                    }
                }
            }

            return true;
        }

        public List<Physician> GetRegionvalue(int selectedregion)
        {
            var query = from r in _db.Physicianregions
                        join rw in _db.Physicians on r.Physicianid equals rw.Physicianid
                        where r.Regionid == selectedregion
                        select (new Physician()
                        {
                            Physicianid = rw.Physicianid,
                            Firstname = rw.Firstname,
                        });

            var data = query.ToList();

            return data;
        }

        public List<ShiftDetailsmodal> ShiftDetailsmodal(DateTime date, DateTime sunday, DateTime saturday, string type)
        {
            var shiftdetails = _db.Shiftdetails.Where(u => u.Shiftdate.Month == date.Month && u.Shiftdate.Year == date.Year);

            BitArray deletedBit = new BitArray(new[] { true });

            switch (type)
            {
                case "month":
                    shiftdetails = _db.Shiftdetails.Where(u => u.Shiftdate.Month == date.Month && u.Shiftdate.Year == date.Year && !u.Isdeleted.Equals(deletedBit));
                    break;

                case "week":
                    shiftdetails = _db.Shiftdetails.Where(u => u.Shiftdate >= sunday.Date && u.Shiftdate <= saturday && !u.Isdeleted.Equals(deletedBit));
                    break;

                case "day":
                    shiftdetails = _db.Shiftdetails.Where(u => u.Shiftdate.Month == date.Month && u.Shiftdate.Year == date.Year && u.Shiftdate.Day == date.Day && !u.Isdeleted.Equals(deletedBit));
                    break;
            }

            var list = shiftdetails.Select(s => new ShiftDetailsmodal
            {
                Shiftid = s.Shiftid,
                Shiftdetailid = s.Shiftdetailid,
                Shiftdate = s.Shiftdate,
                Startdate = s.Shift.Startdate,
                Starttime = s.Starttime,
                Endtime = s.Endtime,
                Physicianid = s.Shift.Physicianid,
                PhysicianName = s.Shift.Physician.Firstname,
                Status = s.Status,
                regionname = _db.Regions.FirstOrDefault(i => i.Regionid == s.Regionid).Name,
                Abberaviation = _db.Regions.FirstOrDefault(i => i.Regionid == s.Regionid).Abbreviation,
                Regionid = s.Regionid,
            }).ToList();

            return list;
        }

        public ShiftDetailsmodal GetShift(int shiftdetailsid)
        {
            var shiftdetails = _db.Shiftdetails.FirstOrDefault(s => s.Shiftdetailid == shiftdetailsid);
            var shifttable = _db.Shifts.FirstOrDefault(s => s.Shiftid == shiftdetails.Shiftid);
            var physicianlist = _db.Physicianregions.Where(p => p.Regionid == shiftdetails.Regionid).Select(s => s.Physicianid).ToList();

            ShiftDetailsmodal shift = new ShiftDetailsmodal
            {
                Shiftdetailid = shiftdetailsid,
                Shiftdate = shiftdetails.Shiftdate,
                Shiftid = shiftdetails.Shiftid,
                Starttime = shiftdetails.Starttime,
                Endtime = shiftdetails.Endtime,
                Regionid = shiftdetails.Regionid,
                Abberaviation = _db.Regions.FirstOrDefault(i => i.Regionid == shiftdetails.Regionid).Abbreviation,
                Status = shiftdetails.Status,
                regions = _db.Regions.ToList(),
                Physicians = _db.Physicians.Where(p => physicianlist.Contains(p.Physicianid)).ToList(),
                Physicianid = shifttable.Physicianid,
            };

            return shift;
        }

        public void SetReturnShift(int status, int shiftdetailid, int Aspid)
        {
            var shiftdetails = _db.Shiftdetails.FirstOrDefault(s => s.Shiftdetailid == shiftdetailid);
            if (status == 1)
            {
                shiftdetails.Status = 2;
                shiftdetails.Modifieddate = DateTime.Now;
                shiftdetails.Modifiedby = Aspid;
            }
            else
            {
                shiftdetails.Status = 1;
                shiftdetails.Modifieddate = DateTime.Now;
                shiftdetails.Modifiedby = Aspid;
            }
            _db.SaveChanges();
        }

        public SchedulingCm GetShiftReview(int regionId, int callId)
        {
            BitArray deletedBit = new BitArray(new[] { false });

            var shiftDetail = _db.Shiftdetails.Where(i => i.Isdeleted.Equals(deletedBit) && i.Status != 2);

            DateTime currentDate = DateTime.Now;

            if (regionId != 0)
            {
                shiftDetail = shiftDetail.Where(i => i.Regionid == regionId);
            }

            if (callId == 1)
            {
                shiftDetail = shiftDetail.Where(i => i.Shiftdate.Month == currentDate.Month);
            }

            var reviewList = shiftDetail.Select(x => new ShiftReview
            {
                shiftDetailId = x.Shiftdetailid,
                PhysicianName = _db.Physicians.FirstOrDefault(y => y.Physicianid == _db.Shifts.FirstOrDefault(z => z.Shiftid == x.Shiftid).Physicianid).Firstname + ", " + _db.Physicians.FirstOrDefault(y => y.Physicianid == _db.Shifts.FirstOrDefault(z => z.Shiftid == x.Shiftid).Physicianid).Lastname,
                ShiftDate = x.Shiftdate.ToString("MMM dd, yyyy"),
                ShiftTime = x.Starttime.ToString("hh:mm tt") + " - " + x.Endtime.ToString("hh:mm tt"),
                ShiftRegion = _db.Regions.FirstOrDefault(y => y.Regionid == x.Regionid).Name,
            }).ToList();

            var regions = _db.Regions.ToList();

            var list = new SchedulingCm()
            {
                ShiftReview = reviewList,
                regionId = regionId,
                callId = callId,
                regions = regions,
            };

            return list;
        }


        public bool SetEditShift(ShiftDetailsmodal shiftDetailsmodal, int Aspid)
        {
            var shiftdetails = _db.Shiftdetails.FirstOrDefault(s => s.Shiftdetailid == shiftDetailsmodal.Shiftdetailid);

            if (shiftdetails != null)
            {
                shiftdetails.Shiftdate = shiftDetailsmodal.Shiftdate;
                shiftdetails.Starttime = shiftDetailsmodal.Starttime;
                shiftdetails.Endtime = shiftDetailsmodal.Endtime;
                shiftdetails.Modifieddate = DateTime.Now;
                shiftdetails.Modifiedby = Aspid;
                _db.SaveChanges();
            }
            return true;
        }


        public void SetDeleteShift(int shiftdetailid, int Aspid)
        {
            var shiftdetails = _db.Shiftdetails.FirstOrDefault(s => s.Shiftdetailid == shiftdetailid);

            shiftdetails.Isdeleted = new BitArray(1, true);
            shiftdetails.Modifieddate = DateTime.Now;
            shiftdetails.Modifiedby = Aspid;

            _db.SaveChanges();
        }


        public void ApproveSelectedShift(int[] shiftDetailsId, int Aspid)
        {
            foreach (var shiftId in shiftDetailsId)
            {
                var shift = _db.Shiftdetails.FirstOrDefault(i => i.Shiftdetailid == shiftId);
                shift.Status = 2;
                shift.Modifieddate = DateTime.Now;
                shift.Modifiedby = Aspid;
            }
            _db.SaveChanges();
        }


        public void DeleteShiftReview(int[] shiftDetailsId, int Aspid)
        {
            foreach (var shiftId in shiftDetailsId)
            {
                var shift = _db.Shiftdetails.FirstOrDefault(i => i.Shiftdetailid == shiftId);

                shift.Isdeleted = new BitArray(1, true);
                shift.Modifieddate = DateTime.Now;
                shift.Modifiedby = Aspid;

            }
            _db.SaveChanges();
        }

        public OnCallModal GetOnCallDetails(int regionId)
        {
            var currentTime = new TimeOnly(DateTime.Now.Hour, DateTime.Now.Minute);
            BitArray deletedBit = new BitArray(new[] { false });

            var onDutyQuery = _db.Shiftdetails
                .Include(sd => sd.Shift.Physician)
                .Where(sd => (regionId == 0 || sd.Shift.Physician.Physicianregions.Any(pr => pr.Regionid == regionId)) &&
                             sd.Shiftdate.Date == DateTime.Today &&
                             currentTime >= sd.Starttime &&
                             currentTime <= sd.Endtime &&
                             sd.Isdeleted.Equals(deletedBit))
                .Select(sd => sd.Shift.Physician)
                .Distinct()
                .ToList();


            var offDutyQuery = _db.Physicians
                .Include(p => p.Physicianregions)
                .Where(p => (regionId == 0 || p.Physicianregions.Any(pr => pr.Regionid == regionId)) &&
                            !_db.Shiftdetails.Any(sd => sd.Shift.Physicianid == p.Physicianid &&
                                                               sd.Shiftdate.Date == DateTime.Today &&
                                                               currentTime >= sd.Starttime &&
                                                               currentTime <= sd.Endtime &&
                                                               sd.Isdeleted.Equals(deletedBit)) && p.Isdeleted == null)
                .ToList();

            var regions = _db.Regions.ToList();

            var onCallModal = new OnCallModal
            {
                OnCall = onDutyQuery,
                OffDuty = offDutyQuery,
                regions = regions,
            };

            return onCallModal;
        }

        //Records
        public patientRecordscm patientRecords(patientRecordscm cm)
        {
            var records = from r in _db.Users
                          select (new patientRecordscm.Records()
                          {
                              Userid = r.Userid,
                              Firstname = r.Firstname,
                              Lastname = r.Lastname,
                              Address = r.Street + "," + r.City + "," + r.State,
                              Email = r.Email,
                              Phonenumber = r.Mobile,
                          });

            if (cm != null)
            {
                if (cm.Firstname != null)
                {
                    records = records.Where(x => x.Firstname.Contains(cm.Firstname)).Select(r => r);
                }
                if (cm.Lastname != null)
                {
                    records = records.Where(x => x.Lastname.Contains(cm.Lastname)).Select(r => r);
                }
                if (cm.Email != null)
                {
                    records = records.Where(x => x.Email.Contains(cm.Email)).Select(r => r);
                }
                if (cm.Phonenumber != null)
                {
                    records = records.Where(x => x.Phonenumber.Contains(cm.Phonenumber)).Select(r => r);
                }
            }

            var pateintRecords = new patientRecordscm()
            {
                records = records.ToList(),
            };

            return pateintRecords;
        }

        public patientRecordscm getPatientReqData(patientRecordscm cm)
        {
            var query = from r in _db.Requests
                        join rw in _db.Requestclients
                        on r.Requestid equals rw.Requestid
                        where r.Isdeleted == null
                        select (new patientRecordscm.patientDetails()
                        {
                            Requestid = r.Requestid,
                            Requestclientid = rw.Requestclientid,
                            patientName = rw.Firstname,
                            Status = r.Status,
                            Email = rw.Email,
                            Phonenumber = rw.Phonenumber,
                            Zip = rw.Zipcode,
                            PatientNote = rw.Notes,
                            PhysicianNote = _db.Requestnotes.FirstOrDefault(x => x.Requestid == r.Requestid).Physiciannotes,
                            AdminNote = _db.Requestnotes.FirstOrDefault(x => x.Requestid == r.Requestid).Adminnotes,
                            physician = _db.Physicians.FirstOrDefault(x => x.Physicianid == r.Physicianid).Firstname,
                            RequestTypeid = r.Requesttypeid,
                            Address = rw.Address,
                            CancelledNote = _db.Requeststatuslogs.FirstOrDefault(x => x.Requestid == r.Requestid && x.Status == 3).Notes,
                        });

            if (cm != null)
            {
                if (cm.Status > 0)
                {
                    query = query.Where(x => x.Status == cm.Status);
                }
                if (cm.patientName != null)
                {
                    query = query.Where(x => x.patientName.Contains(cm.patientName)).Select(r => r);
                }
                if (cm.physician != null)
                {
                    query = query.Where(x => x.physician.Contains(cm.physician)).Select(r => r);
                }
                if (cm.Email != null)
                {
                    query = query.Where(x => x.Email.Contains(cm.Email)).Select(r => r);
                }
                if (cm.Phonenumber != null)
                {
                    query = query.Where(x => x.Phonenumber.Contains(cm.Phonenumber)).Select(r => r);
                }
                if (cm.RequestTypeid > 0)
                {
                    query = query.Where(x => x.RequestTypeid == cm.RequestTypeid);
                }
            }

            var queryy = query.ToList();

            var patientReqData = new patientRecordscm()
            {
                patientDetail = queryy,
            };

            return patientReqData;
        }

        public void deletePermanently(int requestId)
        {
            var request = _db.Requests.FirstOrDefault(x => x.Requestid == requestId);

            if (request != null)
            {
                request.Isdeleted = new BitArray(1, true);

                _db.SaveChanges();
            }
        }

        public patientRecordscm GetPatientData(int userId)
        {
            var patientInfo = from r in _db.Requests
                              join rw in _db.Requestclients
                              on r.Requestid equals rw.Requestid
                              where r.Userid == userId
                              select (new patientRecordscm.patientDetails()
                              {
                                  UserdId = userId,
                                  Requestid = r.Requestid,
                                  patientName = rw.Firstname + " " + rw.Lastname,
                                  Createddate = r.Createddate,
                                  Confirmationnumber = r.Confirmationnumber,
                                  physician = _db.Physicians.FirstOrDefault(x => x.Physicianid == r.Physicianid).Firstname,
                                  Status = r.Status,
                                  concludeDate = r.Status == 6 ? r.Modifieddate : null,
                                  count = _db.Requestwisefiles.Where(x => x.Requestid == r.Requestid && x.Isdeleted == null).Count(),
                              });

            var patientData = new patientRecordscm()
            {
                patientDetail = patientInfo.ToList(),
            };

            return patientData;
        }

        public patientRecordscm getBlockHistory(patientRecordscm cm)
        {
            var blockHistory = from r in _db.Blockrequests
                               where r.Isactive != null
                               select (new patientRecordscm.blockHistory()
                               {
                                   Blockrequestid = r.Blockrequestid,
                                   Phonenumber = r.Phonenumber,
                                   Email = r.Email,
                                   Createddate = r.Createddate.ToString(),
                                   Reason = r.Reason,
                                   Requestid = r.Requestid,
                                   patientName = _db.Requestclients.FirstOrDefault(x => x.Requestid == r.Requestid).Firstname + " " + _db.Requestclients.FirstOrDefault(x => x.Requestid == r.Requestid).Lastname,
                                   Isactive = r.Isactive,
                               });

            if (cm != null)
            {
                if (cm.Firstname != null)
                {
                    blockHistory = blockHistory.Where(x => x.patientName.Contains(cm.Firstname)).Select(r => r);
                }
                if (cm.Email != null)
                {
                    blockHistory = blockHistory.Where(x => x.Email.Contains(cm.Email)).Select(r => r);
                }
                if (cm.Phonenumber != null)
                {
                    blockHistory = blockHistory.Where(x => x.Phonenumber.Contains(cm.Phonenumber)).Select(r => r);
                }
                if (cm.Createddate != null)
                {
                    blockHistory = blockHistory.Where(x => x.Createddate.Substring(0, 10) == cm.Createddate).Select(r => r);
                }
            }

            var blockReqData = new patientRecordscm()
            {
                blockHistories = blockHistory.ToList(),
            };

            return blockReqData;
        }

        public void unblockPatient(int reqId)
        {
            var blockReq = _db.Blockrequests.FirstOrDefault(x => x.Requestid == reqId);

            var request = _db.Requests.FirstOrDefault(x => x.Requestid == reqId);

            if (blockReq != null)
            {
                blockReq.Isactive = null;

                request.Status = 1;
                request.Isdeleted = null;

                _db.SaveChanges();
            }
        }

        public patientRecordscm emailLogs(patientRecordscm cm)
        {

            var emailLogs = from r in _db.Emaillogs
                            select (new patientRecordscm.EmailLogs()
                            {
                                Emailid = r.Emailid,
                                Emaillogid = r.Emaillogid,
                                Createdate = r.Createdate.ToString(),
                                Senttries = r.Senttries,
                                Sentdate = r.Sentdate.ToString(),
                                Action = r.Action,
                                Isemailsent = r.Isemailsent,
                                Confirmationnumber = r.Confirmationnumber,
                                Recipient = _db.Requestclients.FirstOrDefault(x => x.Requestid == r.Requestid) == null ? _db.Physicians.FirstOrDefault(x => x.Physicianid == r.Physicianid).Firstname : _db.Requestclients.FirstOrDefault(x => x.Requestid == r.Requestid).Firstname,
                                RoleName = _db.Aspnetroles.FirstOrDefault(x => x.Id == r.Roleid).Name,
                                Roleid = r.Roleid,
                            });

            if (cm != null)
            {

                if (cm.patientName != null)
                {
                    emailLogs = emailLogs.Where(x => x.Recipient.Contains(cm.patientName)).Select(r => r);
                }
                if (cm.Email != null)
                {
                    emailLogs = emailLogs.Where(x => x.Emailid.Contains(cm.Email)).Select(r => r);
                }

                if (cm.Createddate != null)
                {
                    emailLogs = emailLogs.Where(x => x.Createdate.Substring(0, 10) == cm.Createddate).Select(r => r);
                }
                if (cm.Sentdate != null)
                {
                    emailLogs = emailLogs.Where(x => x.Sentdate.Substring(0, 10) == cm.Sentdate).Select(r => r);
                }
                if (cm.Roleid != null)
                {
                    emailLogs = emailLogs.Where(x => x.Roleid == cm.Roleid).Select(r => r);
                }
            }

            var emailLogsData = new patientRecordscm()
            {
                emails = emailLogs.ToList(),
                aspnetroles = _db.Aspnetroles.Select(r => r).ToList(),
            };

            return emailLogsData;
        }

        public patientRecordscm smsLogs(patientRecordscm cm)
        {
            var smsLogs = from r in _db.Smslogs
                          select (new patientRecordscm.EmailLogs()
                          {
                              Mobile = r.Mobilenumber,
                              SmsLogid = r.Smslogid,
                              Createdate = r.Createdate.ToString(),
                              Senttries = r.Senttries,
                              Sentdate = r.Sentdate.ToString(),
                              Action = r.Action,
                              Isemailsent = r.Issmssent,
                              Confirmationnumber = r.Confirmationnumber,
                              Recipient = _db.Requestclients.FirstOrDefault(x => x.Requestid == r.Requestid) == null ? _db.Physicians.FirstOrDefault(x => x.Physicianid == r.Physicianid).Firstname : _db.Requestclients.FirstOrDefault(x => x.Requestid == r.Requestid).Firstname,
                              RoleName = _db.Aspnetroles.FirstOrDefault(x => x.Id == r.Roleid).Name,
                              Roleid = r.Roleid,
                          });

            if (cm != null)
            {
                if (cm.patientName != null)
                {
                    smsLogs = smsLogs.Where(x => x.Recipient.Contains(cm.patientName)).Select(r => r);
                }
                if (cm.Phonenumber != null)
                {
                    smsLogs = smsLogs.Where(x => x.Mobile.Contains(cm.Phonenumber)).Select(r => r);
                }

                if (cm.Createddate != null)
                {
                    smsLogs = smsLogs.Where(x => x.Createdate.Substring(0, 10) == cm.Createddate).Select(r => r);
                }
                if (cm.Sentdate != null)
                {
                    smsLogs = smsLogs.Where(x => x.Sentdate.Substring(0, 10) == cm.Sentdate).Select(r => r);
                }
                if (cm.Roleid != null)
                {
                    smsLogs = smsLogs.Where(x => x.Roleid == cm.Roleid).Select(r => r);
                }
            }

            var smsLogsData = new patientRecordscm()
            {
                emails = smsLogs.ToList(),
                aspnetroles = _db.Aspnetroles.Select(r => r).ToList(),
            };

            return smsLogsData;
        }

        public List<string> GetListOfRoleMenu(int roleId)
        {
            List<Rolemenu> roleMenus = _db.Rolemenus.Where(u => u.Roleid == roleId).ToList();
            if (roleMenus.Count > 0)
            {
                List<string> menus = new List<string>();
                foreach (var item in roleMenus)
                {
                    menus.Add(_db.Menus.FirstOrDefault(u => u.Menuid == item.Menuid).Name);
                }
                return menus;
            }
            else
            {
                return new List<string>();
            }
        }

        public bool checkreq(int requestid)
        {
            var req = _db.Requests.FirstOrDefault(c => c.Requestid == requestid);

            if (req != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public GetPayRate GetPayRate(int physicianId, int callid)
        {
            var payrate = _db.PayRates.FirstOrDefault(i => i.PhysicianId == physicianId);
            var Aspid = _db.Physicians.FirstOrDefault(a => a.Physicianid == physicianId).Aspnetuserid;
            if (payrate == null)
            {
                var GetPayRate = new GetPayRate()
                {
                    PhysicianId = physicianId,
                    AspId = Convert.ToString(Aspid),
                    callid = callid,
                };
                return GetPayRate;
            }
            else
            {
                var GetPayRate = new GetPayRate()
                {
                    PhysicianId = physicianId,
                    AspId = Convert.ToString(Aspid),
                    callid = callid,
                    NightShift_Weekend = payrate.NightShiftWeekend != 0 ? payrate.NightShiftWeekend : default,
                    Shift = payrate.Shift != 0 ? payrate.Shift : default,
                    HouseCalls_Nights_Weekend = payrate.HouseCallNightWeekend != 0 ? payrate.HouseCallNightWeekend : default,
                    PhoneConsult = payrate.PhoneConsult != 0 ? payrate.PhoneConsult : default,
                    PhoneConsults_Nights_Weekend = payrate.PhoneConsultNightWeekend != 0 ? payrate.PhoneConsultNightWeekend : default,
                    BatchTesting = payrate.BatchTesting != 0 ? payrate.BatchTesting : default,
                    HouseCalls = payrate.HouseCall != 0 ? payrate.HouseCall : default
                };
                return GetPayRate;
            }
        }


        public bool SetPayRate(GetPayRate getPayRate)
        {
            var payrate = _db.PayRates.FirstOrDefault(i => i.PhysicianId == getPayRate.PhysicianId);
            if (payrate == null)
            {
                var payratedata = new PayRate
                {
                    PhysicianId = getPayRate.PhysicianId,
                    NightShiftWeekend = getPayRate.NightShift_Weekend,
                    Shift = getPayRate.Shift,
                    HouseCallNightWeekend = getPayRate.HouseCalls_Nights_Weekend,
                    PhoneConsult = getPayRate.PhoneConsult,
                    PhoneConsultNightWeekend = getPayRate.PhoneConsults_Nights_Weekend,
                    BatchTesting = getPayRate.BatchTesting,
                    HouseCall = getPayRate.HouseCalls,     
                    CreatedDate=DateTime.Now,
                };
                _db.PayRates.Add(payratedata);
                _db.SaveChanges();
                return true;
            }
            else
            {
                if (getPayRate.NightShift_Weekend != payrate.NightShiftWeekend || getPayRate.Shift != payrate.Shift || getPayRate.HouseCalls_Nights_Weekend != payrate.HouseCallNightWeekend ||
                getPayRate.PhoneConsult != payrate.PhoneConsult || getPayRate.PhoneConsults_Nights_Weekend != payrate.PhoneConsultNightWeekend || getPayRate.BatchTesting != payrate.BatchTesting ||
                getPayRate.HouseCalls != payrate.HouseCall)
                {
                    payrate.NightShiftWeekend = getPayRate.NightShift_Weekend;
                    payrate.Shift = getPayRate.Shift;
                    payrate.HouseCallNightWeekend = getPayRate.HouseCalls_Nights_Weekend;
                    payrate.PhoneConsult = getPayRate.PhoneConsult;
                    payrate.PhoneConsultNightWeekend = getPayRate.PhoneConsults_Nights_Weekend;
                    payrate.BatchTesting = getPayRate.BatchTesting;
                    payrate.HouseCall = getPayRate.HouseCalls;
                    payrate.CreatedDate = DateTime.Now;
                   
                    // payrate.ModifiedDate=DateTime.Now.Date;
                    _db.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public Chatcm GetChats(int providerId, int adminId, int requestId, int flag, int roleId)
        {

            if (flag == 1)
            {
                var chats = from r in _db.Chats
                            where r.RequestId == null && r.PhyscainId == providerId && r.AdminId == adminId && (r.SentBy==1 || r.SentBy==2)
                            select (new Chatcm()
                            {
                                RequestId = null,
                                ChatId = r.ChatId,
                                ProviderId = r.PhyscainId,
                                Message = r.Message,
                                AdminId = r.AdminId,
                                ChatDate = Convert.ToDateTime(r.SentDate),
                                SentBy = r.SentBy
                            });
                var chat = new Chatcm()
                {
                    RequestId = null,
                    ProviderId = providerId,
                    AdminId = adminId,
                    Flag=flag,              
                    RecieverName=roleId==1?_db.Physicians.FirstOrDefault(x=>x.Physicianid==providerId).Firstname : _db.Admins.FirstOrDefault(x=>x.Adminid==adminId).Firstname,
                    chats = chats.ToList(),
                };

                

                return chat;

            }
            if(flag == 2)
            {
                var chats = from r in _db.Chats
                            where r.RequestId == requestId && r.PhyscainId == null && r.AdminId == adminId && (r.SentBy == 1 || r.SentBy == 3)
                            select (new Chatcm()
                            {
                                RequestId = r.RequestId,
                                ChatId = r.ChatId,
                                ProviderId = null,
                                Message = r.Message,
                                AdminId = r.AdminId,
                                ChatDate = Convert.ToDateTime(r.SentDate),
                                SentBy = r.SentBy
                            });

                var chat = new Chatcm()
                {
                    RequestId = requestId,
                    ProviderId = null,
                    AdminId = adminId,
                    Flag = flag,
                    RecieverName = roleId == 1 ? _db.Requestclients.FirstOrDefault(x => x.Requestid == requestId).Firstname : _db.Admins.FirstOrDefault(x => x.Adminid == adminId).Firstname,
                    chats = chats.ToList(),
                };

                return chat;
            }
            if(flag == 3)
            {
                var chats = from r in _db.Chats
                            where r.RequestId == requestId && r.PhyscainId == providerId && r.AdminId == null && (r.SentBy == 2 || r.SentBy == 3)
                            select (new Chatcm()
                            {
                                RequestId = r.RequestId,
                                ChatId = r.ChatId,
                                ProviderId = r.PhyscainId,
                                Message = r.Message,
                                AdminId = null,
                                ChatDate = Convert.ToDateTime(r.SentDate),
                                SentBy = r.SentBy
                            });

                var chat = new Chatcm()
                {
                    RequestId = requestId,
                    ProviderId = providerId,
                    AdminId = null,
                    Flag = flag,
                    RecieverName = roleId == 3 ? _db.Physicians.FirstOrDefault(x => x.Physicianid == providerId).Firstname : _db.Requestclients.FirstOrDefault(x => x.Requestid == requestId).Firstname,
                    chats = chats.ToList(),
                };

                return chat;
            }
            return null;     
        }

        public void SaveChats(Chatcm cm)
        {
            if (cm != null)
            {
                if (cm.Flag == 1)
                {
                    Chat chat = new Chat()
                    {
                        RequestId = null,
                        AdminId = cm.AdminId,
                        PhyscainId = cm.ProviderId,
                        Message = cm.Message,
                        SentDate = DateTime.Now,
                        SentBy = cm.SentBy

                    };

                    _db.Add(chat);
                    _db.SaveChanges();
                }
                if (cm.Flag == 2)
                {
                    Chat chat = new Chat()
                    {
                        RequestId = cm.RequestId,
                        AdminId = cm.AdminId,
                        PhyscainId = null,
                        Message = cm.Message,
                        SentDate = DateTime.Now,
                        SentBy = cm.SentBy

                    };

                    _db.Add(chat);
                    _db.SaveChanges();
                }
                if (cm.Flag == 3)
                {
                    Chat chat = new Chat()
                    {
                        RequestId = cm.RequestId,
                        AdminId = null,
                        PhyscainId = cm.ProviderId,
                        Message = cm.Message,
                        SentDate = DateTime.Now,
                        SentBy = cm.SentBy

                    };

                    _db.Add(chat);
                    _db.SaveChanges();
                }
               
               
            }
        }

    }
}




