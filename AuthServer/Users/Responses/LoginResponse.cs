namespace AuthServer.Users.Responses;

public class LoginResponse
{
    public string Token { get; set; }
    public UserResponse User { get; set; }
}