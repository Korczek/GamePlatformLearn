namespace Auth.Models;

public class User
{
    public Guid Id { get; set; }

    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = null!;

    public byte[] HashedPassword { get; set; } = null!;
    public byte[] Salt { get; set; } = null!;

    public string Role { get; set; } = "User";
}