namespace AuthServer.Errors;

public class NotFoundException(string s) : Exception(s)
{
    
}