using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUDENT_API.Models.Doctor.GetAppointment
{
    public class GetAppointmentResponse
    {
        public string Username { get; set; }
        public List<string> DateTime { get; set; }
        public string? Patient { get; set; }
    }
}
