namespace Interfaces.DTO
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Login { get; set; } = null!;
        public bool IsClient { get; set; }

        // optional password hash for authentication (byte array)
        public byte[]? PasswordHash { get; set; }
    }
}
