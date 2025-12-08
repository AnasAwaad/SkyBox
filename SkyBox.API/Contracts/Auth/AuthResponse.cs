namespace SkyBox.API.Contracts.Auth;

public record AuthResponse(
    string Id,
    string Email,
    string FullName,
    string Token,
    int ExpiresIn
);
