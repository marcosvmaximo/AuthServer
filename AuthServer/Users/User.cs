using System.ComponentModel.DataAnnotations;
using AuthServer.Roles;
using AuthServer.Users.Requests;

namespace AuthServer.Users;

public class User
{
    public User(string name, string email, string password)
    {
        Name = name;
        Email = email;
        Password = password;
    }

    public User(CreateUserRequest request)
    {
        Name = request.Name;
        Email = request.Email;
        Password = request.Password;
    }

    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    public string Password { get; set; }
    
    public List<Role> Roles { get; set; }
    public bool IsAdmin { get; set; }
}