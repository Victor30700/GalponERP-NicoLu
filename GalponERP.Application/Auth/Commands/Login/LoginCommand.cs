using MediatR;

namespace GalponERP.Application.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;

public record LoginResponse(string IdToken, string RefreshToken, string Email, int ExpiresIn);
