namespace Interfaces.DTO
{
    public class UserDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string MiddleName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public int Id { get; set; }
        public string Login { get; set; } = null!;
        public bool IsClient { get; set; }
        public byte[]? PasswordHash { get; set; }
    }
}
