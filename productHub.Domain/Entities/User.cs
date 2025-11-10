namespace productHub.Domain.Entities;

public class User
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Username { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }
    public string Role { get; private set; }

    
    protected User() { }

    public User(string username, string email, string password, string role)
    {
        Username = username;
        Email = email;
        Password = password;
        Role = role;
    }
}