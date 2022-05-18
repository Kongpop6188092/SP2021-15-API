using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUDENT_API.Models.User.Patient.Patients
{
    public class PatientsResponse
    {
        public string CID { get; set; }
        public string Name { get; set; }
        public string DrugAllergy { get; set; }
    }
}
