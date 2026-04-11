namespace GalponERP.Application.Interfaces;

public interface IAuthenticationService
{
    Task<string?> GetUserIdAsync();
    Task<string?> GetUserEmailAsync();
    bool IsAuthenticated();
    Task<string> CreateUserAsync(string email, string password, string displayName, IDictionary<string, object>? extraUserData = null);
    Task<string?> GetUidByEmailAsync(string email);
}
