using Auth.Models;

namespace Auth.Services.Tokens;

public interface IJwtTokenGenerator
{
    public string Generate(User user);
}