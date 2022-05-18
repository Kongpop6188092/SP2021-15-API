using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUDENT_API.Models.Patient.GetMedicine
{
    public class GetMedicineResponse
    {
        public string Doctor { get; set; }
        public int Medicine { get; set; }
        public int Amount { get; set; }
        public string CreatedDate { get; set; }
    }
}
