namespace MUDENT_API.Models.User.Patient.SaveTreatment
{
    public class SaveTreatmentRequest
    {
        public string? Username { get; set; }
        public string? Patient { get; set; }
        public string? ToothNo { get; set; }
        public string? ToothSide { get; set; }
        public string? Diagnosis { get; set; }
        public string? Choice { get; set; }
    }
}
