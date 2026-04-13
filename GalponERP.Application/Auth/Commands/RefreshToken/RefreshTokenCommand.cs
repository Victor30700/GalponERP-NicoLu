using MediatR;

namespace GalponERP.Application.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<RefreshTokenResponse>;

public record RefreshTokenResponse(string IdToken, string RefreshToken, string Email, int ExpiresIn);
