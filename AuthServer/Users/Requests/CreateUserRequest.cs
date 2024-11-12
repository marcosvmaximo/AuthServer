using System.ComponentModel.DataAnnotations;

namespace AuthServer.Users.Requests;

public class CreateUserRequest
{
    public CreateUserRequest(string name, string email, string password)
    {
        Name = name;
        Email = email;
        Password = password;
    }
    
    public CreateUserRequest(){}

    [Required]
    public string Name { get; set; }
    
    [EmailAddress]
    [Required]
    public string Email { get; set; }
    
    [Required]
    public string Password { get; set; }
}