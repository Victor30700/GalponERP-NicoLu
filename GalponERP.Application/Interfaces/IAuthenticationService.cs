namespace GalponERP.Application.Interfaces;

public interface IAuthenticationService
{
    Task<string?> GetUserIdAsync();
    Task<string?> GetUserEmailAsync();
    bool IsAuthenticated();
}
