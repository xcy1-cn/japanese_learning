using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JapaneseLearningApi.Data;
using JapaneseLearningApi.Models;
using JapaneseLearningApi.Requests;
using JapaneseLearningApi.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace JapaneseLearningApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var admin = await _context.AdminUsers
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (admin == null)
        {
            // return Unauthorized("Invalid username or password.");
            return BadRequest(ApiResponse<string>.Fail(400, "Invalid username or password."));
        }

        var passwordHasher = new PasswordHasher<AdminUser>();

        var result = passwordHasher.VerifyHashedPassword(
            admin,
            admin.PasswordHash,
            request.Password
        );

        if (result == PasswordVerificationResult.Failed)
        {
            // return Unauthorized("Invalid username or password.");
            return BadRequest(ApiResponse<string>.Fail(400, "Invalid username or password."));
        }

        var token = GenerateJwtToken(admin);

        // return Ok(new
        // {
        //     token,
        //     username = admin.Username,
        //     role = admin.Role
        // });

        var response = new AuthResponse
        {
            Token = token,
            Username = admin.Username,
            Role = admin.Role
        };

        return Ok(ApiResponse<AuthResponse>.Success(response, "Article created successfully."));
    }

    private string GenerateJwtToken(AdminUser admin)
    {
        var jwtKey = _configuration["Jwt:Key"];
        var jwtIssuer = _configuration["Jwt:Issuer"];
        var jwtAudience = _configuration["Jwt:Audience"];
        var expireMinutes = int.Parse(_configuration["Jwt:ExpireMinutes"] ?? "120");

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
            new Claim(ClaimTypes.Name, admin.Username),
            new Claim(ClaimTypes.Role, admin.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(expireMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}