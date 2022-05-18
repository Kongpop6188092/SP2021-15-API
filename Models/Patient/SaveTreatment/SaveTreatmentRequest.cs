using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUDENT_API.Models.Patient.SaveTreatment
{
    public class SaveTreatmentRequest
    {
        public string? Dentist { get; set; }
        public string? CID { get; set; }
        public string? ToothNo { get; set; }
        public string? ToothSide { get; set; }
        public string? Diagnosis { get; set; }
        public string? Choice { get; set; }
    }
}
