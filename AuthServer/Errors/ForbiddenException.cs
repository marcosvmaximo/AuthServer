namespace AuthServer.Errors;

public class ForbiddenException : Exception
{
    public ForbiddenException(string message = "Forbbiden") : base(message)
    {
        
    }
}