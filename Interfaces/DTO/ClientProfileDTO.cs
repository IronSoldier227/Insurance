namespace Interfaces.DTO
{
    public class ClientProfileDto
    {
        public int Id { get; set; }
        public string Passport { get; set; } = null!;
        public string DriverLicense { get; set; } = null!;
        public int DrivingExperience { get; set; }
    }
}