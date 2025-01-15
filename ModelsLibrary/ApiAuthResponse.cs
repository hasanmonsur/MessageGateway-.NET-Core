using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModelsLibrary
{
    public class ApiAuthResponse
    {
            public string UserId { get; set; }
            public string ServiceId { get; set; }
            public string service_role_id { get; set; }
        
    }
}
