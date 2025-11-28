using System.Text;
using Auth.Data;
using Auth.Services;
using Auth.Services.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Auth;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // database: (local, this project is for learning purposes only)
        var dbString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<AuthDbContext>(options => options.UseSqlServer(dbString));
        
        // those data should be accessed from some key vault or aws secrets
        var jwt = builder.Configuration.GetSection("JwtSettings");
        
        builder.Services.Configure<JwtSettings>(jwt);
        builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        
        // Add authorisation for editing or deleting user data's
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt["Issuer"],

                    ValidateAudience = true,
                    ValidAudience = jwt["Audience"],

                    ValidateLifetime = true,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Secret"]!))
                };
            });
        
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();

        var app = builder.Build();

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.MapGet("/Auth", () => "Hello in Auth.Api");

        app.Run();
    }
}