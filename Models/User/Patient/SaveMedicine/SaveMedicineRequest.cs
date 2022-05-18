using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUDENT_API.Models.User.Patient.SaveMedicine
{
    public class SaveMedicineRequest
    {
        public string? Doctor { get; set; }
        public string? CID { get; set; }
        public string? Medicine { get; set; }
        public int? Amount { get; set; }
    }
}
