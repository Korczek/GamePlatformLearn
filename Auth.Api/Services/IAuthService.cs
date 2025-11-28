using Auth.Models;
using Microsoft.AspNetCore.Identity.Data;

namespace Auth.Services;

public interface IAuthService
{
    public User ValidateUser(LoginRequest request);
}