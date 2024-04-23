using System;
using System.Collections.Generic;
using System.Linq;
using Data_Layer.DataModels;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections;

namespace Data_Layer.CustomModels
{
    public class patientRecordscm
    {
        public List<Records> records {  get; set; }

       

        public List<patientDetails> patientDetail {  get; set; }
        public List<blockHistory> blockHistories {  get; set; }

        public List<EmailLogs> emails { get; set; }

        public short Status { get; set; }
        public string patientName { get; set; }
        public int RequestTypeid { get; set; }
        public string? Email { get; set; }
        public string? Createddate { get; set; }
        public string? Sentdate { get; set; }

        public string? Phonenumber { get; set; }

        public string physician { get; set; }

        public string Firstname { get; set; }

        public string? Lastname { get; set; }


        public class Records
        {
            public int Requestclientid { get; set; }

            public int Requestid { get; set; }
            public int Userid { get; set; }

            public string Firstname { get; set; } 

            public string? Lastname { get; set; }

           

            public string? Phonenumber { get; set; }

            public string? Address { get; set; }

            public string? Email { get; set; }
        }

        public class patientDetails
        {
            public int UserdId { get; set; }
            public int Requestclientid { get; set; }
            public int count { get; set; }
            public string? Confirmationnumber { get; set; }        
            public int Requestid { get; set; }
            public int RequestTypeid { get; set; }
            public int Physicianid { get; set; }
            public string patientName { get; set; }

            public string Requestor { get; set; }

            public string? Email { get; set; }

            public string? Phonenumber { get; set; }

            public string physician { get; set; }

            public DateTime Createddate { get; set; } 
            public DateTime? concludeDate { get; set; } 
            public DateTime closeCaseDate { get; set; }

            public string? PhysicianNote { get; set; }
            public string? AdminNote { get; set; }
            public string? CancelledNote { get; set; }

            public string? PatientNote { get; set; } 
            public string? Address { get; set; }

            public short Status { get; set; }

            public string? Zip { get; set; }

        }

        public class blockHistory
        {
            public int Blockrequestid { get; set; }

             public string patientName { get; set; }
    
            public string? Phonenumber { get; set; }

            public string? Email { get; set; }

            public BitArray? Isactive { get; set; }

            public string? Reason { get; set; }

            public int Requestid { get; set; }

            public string? Createddate { get; set; }

            public DateTime? Modifieddate { get; set; }
        }

        public List<Aspnetrole> aspnetroles { get; set; }

        public int? Roleid { get; set; }

        public class EmailLogs
        {

           

            public int? Adminid { get; set; }
            public int? Physicianid { get; set; }
            public int Emaillogid { get; set; }
            public int SmsLogid { get; set; }

            public string Emailid { get; set; } = null!;
            public string RoleName { get; set; }

            public string? Confirmationnumber { get; set; }
            public string? Mobile { get; set; }

            public int? Roleid { get; set; }

            public int? Requestid { get; set; }

            public string? Createdate { get; set; }
            public string? Sentdate { get; set; }
            public string? Recipient { get; set; }

            public int? Senttries { get; set; }

            public int? Action { get; set; }

            public BitArray? Isemailsent { get; set; }
        }
    }
}
