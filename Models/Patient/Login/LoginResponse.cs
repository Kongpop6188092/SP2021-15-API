using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUDENT_API.Models.Patient.Login
{
    public class LoginResponse
    {
        public bool Status { get; set; }
        public Dictionary<string, string> Data { get; set; }

        public LoginResponse(bool status)
        {
            Status = status;
            Data = new Dictionary<string, string>();
        }
    }
}
