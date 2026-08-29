using Microsoft.AspNetCore.Authentication.OAuth.Claims;

public static class taskHandler{

    public static status create()
    {
        return null 
    }
    public static status delete()
    {
        
    }
    public static status edit()
    {
        
    }
    public static status get()
    {
        
    }
}

public class taskDTO
{
    public string? taskName{ get; set; }
    public string? description{get;set;}
    public bool done { get; set; }
    public DateTime creationDate { get; set; }
}

public class status
{
    public bool good { get; set; }
    public string? msg { get; set; }=null;
}