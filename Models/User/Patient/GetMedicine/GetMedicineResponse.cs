using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUDENT_API.Models.User.Patient.GetMedicine
{
    public class GetMedicineResponse
    {
        public string? ID { get; set; }
        public string? Username { get; set; }
        public string? Patient { get; set; }
        public string? Name { get; set; }
        public string? Amount { get; set; }
        public string? Detail { get; set; }
        public string? CreatedDate { get; set; }
    }
}
