using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUDENT_API.Models.Core
{
    public class Validate
    {
        public bool isMissing { get; set; }
        public bool isInvalid { get; set; }
        public string missingError { get; set; }
        public string invalidError { get; set; }

        public Validate()
        {
            isMissing = false;
            isInvalid = false;
            missingError = "Missing parameter: ";
            invalidError = "Invalid input: ";
        }
    }
}
