namespace AuthServer.Users.Requests;

public class UpdateUserRequest
{
    public UpdateUserRequest()
    {
        
    }

    public UpdateUserRequest(string name)
    {
        Name = name;
    }
    
    public string Name { get; set; }
}