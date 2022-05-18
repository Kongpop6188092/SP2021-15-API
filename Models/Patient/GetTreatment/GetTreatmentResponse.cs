using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUDENT_API.Models.Patient.GetTreatment
{
    public class GetTreatmentResponse
    {
        public string Dentist { get; set; }
        public string ToothNo { get; set; }
        public string ToothSide { get; set; }
        public string Diagnosis { get; set; }
        public string Choice { get; set; }
        public string CreatedDate { get; set; }
    }
}
