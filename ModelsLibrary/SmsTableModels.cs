using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModelsLibrary
{
    // Authentication Table
    public class Authentication
    {
        [Key]
        [JsonIgnore]
        public int? AutoId { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string ServiceId { get; set; }

        [JsonIgnore]
        public List<ServiceRoles>? serviceRoles { get; set; }

        [JsonIgnore]
        public int? Status { get; set; } = 1;

        [JsonIgnore]
        public DateTime? EntryDate { get; set; } = DateTime.UtcNow;
    }


    // Service Roles Table
    public class ServiceRoles
    {
        [Key]
        [JsonIgnore]
        public int AutoId { get; set; }
        public string UserId { get; set; }
        public string Service_role_id { get; set; }
        public string Status { get; set; } = "Y";
    }


    // Service Roles Table
    public class AuthsResponse
    {
        public int StatusCode { get; set; }
        public string StatusMsg { get; set; }
    }


    // Service Type Table
    public class ServiceType
    {
        [Key]
        public int AutoId { get; set; }
        public string ServiceId { get; set; }
        public string SmsType { get; set; }
        public long PriorityId { get; set; }
    }

    // SMS Transaction Table
    public class SmsTranData
    {
        [Key]
        public int AutoId { get; set; }
        public string RefId { get; set; }
        public DateTime TranDate { get; set; } = DateTime.UtcNow;
        public string CsmsId { get; set; }
        public string ServiceId { get; set; }
        public string MobileNo { get; set; }
        public string Message { get; set; }
        public int Status { get; set; } = 1;
        public string StatusMsg { get; set; }
        public string SmsType { get; set; }
        public string MnoCode { get; set; }
        public string MnoPrefix { get; set; }
        public long PriorityId { get; set; }
        public DateTime EntryDate { get; set; } = DateTime.UtcNow;
    }


    //--response --
    public class SmsTranResponse
    {
        public int Status { get; set; }
        public string CsmsId { get; set; }
        public string RefId { get; set; }
        public string Msisdn { get; set; }
    }

    // SMS Type Table
    public class SMSType
    {
        [Key]
        public int AutoId { get; set; }
        public string SmsType { get; set; }
        public string SmsTypeDesc { get; set; }
    }

    // MNO List Table
    public class MNOInfo
    {
        [Key]
        public int AutoId { get; set; }
        public string MnoCode { get; set; }
        public string MnoPrefix { get; set; }
        public string MnoDesc { get; set; }
        public int Status { get; set; } = 1;
        public DateTime EntryDate { get; set; } = DateTime.UtcNow;
    }

    // MNO Authentication Table
    public class MNOAuthentication
    {
        [Key]
        public int AutoId { get; set; }
        public string MnoCode { get; set; }
        public string UserIdKey { get; set; }
        public string PassKey { get; set; }
        public string MnoUrl { get; set; }
        public int Status { get; set; } = 1;
        public DateTime EntryDate { get; set; } = DateTime.UtcNow;
    }

    // Customer MNO Whitelist/Blacklist Table
    public class CustomerMNOWhiteBlacklist
    {
        [Key]
        public int AutoId { get; set; }
        public string MobileNo { get; set; }
        public string MnoCode { get; set; }
        public int Status { get; set; } = 1;
        public DateTime EntryDate { get; set; } = DateTime.UtcNow;
    }

    // SMS Summary Table
    public class SMSSummary
    {
        [Key]
        public int AutoId { get; set; }
        public string MnoCode { get; set; }
        public string MonthYear { get; set; }
        public long RecordNo { get; set; }
    }

    

}
