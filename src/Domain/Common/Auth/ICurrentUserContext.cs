namespace Domain.Common.Auth;

public interface ICurrentUserContext
{
    int UserId { get; }
    string? Email { get; }
    //public string? IpAddress { get; set; }
}

public class CurrentUserContext : ICurrentUserContext
{
    public int UserId { get; set; }
    public string? Email { get; set; }
    //public string? IpAddress { get; set; }
}