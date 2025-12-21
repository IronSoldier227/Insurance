namespace Interfaces.DTO
{
    public class UserCreateDto
    {
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool IsClient { get; set; }

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string MiddleName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        // Client-specific
        public string Passport { get; set; } = string.Empty;
        public string DriverLicense { get; set; } = string.Empty;
        public int DrivingExperience { get; set; }
    }
}
