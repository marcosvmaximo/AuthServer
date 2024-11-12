namespace AuthServer.Users.Requests;

public class LoginRequest
{
    public LoginRequest(){}

    public LoginRequest(string email, string password)
    {
        
    }
    
    public string Email { get; set; }
    public string Password { get; set; }
}