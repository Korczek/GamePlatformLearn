using Auth.Models;
using Microsoft.AspNetCore.Identity.Data;

namespace Auth.Services;

public class AuthService : IAuthService
{
    public User ValidateUser(LoginRequest request)
    {
        // err: add db and check in it for user 
        throw new NotImplementedException();
    }
}