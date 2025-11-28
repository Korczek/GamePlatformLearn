using Auth.Data;
using Auth.DTOs;
using Auth.Models;
using Auth.Services.Hasher;
using Auth.Services.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(AuthDbContext context, IJwtTokenGenerator jwtTokenGenerator) : ControllerBase
{

    [HttpGet]
    public IActionResult LogIn([FromBody] LoginRequest user)
    {
        // those values can be empty 
        var email = user.Email ?? string.Empty;
        var username = user.Username ?? string.Empty;

        if (string.IsNullOrWhiteSpace(user.Password))
            return BadRequest("You need to enter a password");
        
        var storedUser = context.Users.FirstOrDefault(x => x.Email == email || x.Username == username);
        if (storedUser == null) return NotFound("There is no user with this email or username");

        return PasswordHasher.Verify(user.Password, storedUser.HashedPassword, storedUser.Salt)
            ? Ok(jwtTokenGenerator.Generate(storedUser)) // create and return token 
            : Unauthorized("Wrong Password");
    }

    [HttpPost]
    public IActionResult Create([FromBody] RegisterRequest user)
    {
        if (string.IsNullOrWhiteSpace(user.Password) || string.IsNullOrWhiteSpace(user.RepeatPassword) || string.IsNullOrWhiteSpace(user.Email))
            return BadRequest("Wrong data provided");
        
        if (user.Password != user.RepeatPassword)
            return BadRequest("Passwords do not match");
        
        if (context.Users.Any(x => x.Email == user.Email))
            return BadRequest("There already exists a user with this email");
        
        // hash all stuff here and prepare new user 
        PasswordHasher.Create(user.Password, out byte[] hash, out byte[] salt);

        var newUser = new User
        {
            Email = user.Email,
            HashedPassword = hash,
            Salt = salt
        };

        context.Users.Add(newUser);
        context.SaveChanges();
        
        return Ok();
    }
    
    [Authorize]
    [HttpPost("me/SwitchPassword")]
    public IActionResult ChangePassword([FromBody] LoginRequest user)
    {
        return Empty;
    }
}