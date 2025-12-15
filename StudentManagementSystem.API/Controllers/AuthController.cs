using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using StudentManagementSystem.Application.DTOs;
 
namespace StudentManagementSystem.API.Controllers;
 
[ApiController]
[Route("api/[controller]")]
public class AuthController(IConfiguration config) : ControllerBase
{
    private readonly IConfiguration _config = config;
    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest req)
    {
        var hardUser = "admin";
        var hardPass = "P@ssw0rd!";
 
        if (req.Username != hardUser || req.Password != hardPass)
            return Unauthorized(new { message = "Invalid credentials" });
 
        IConfigurationSection jwt = _config.GetSection("Jwt");
        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(jwt["Key"]));
        SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);
 
        Claim[] claims =
        [
            new Claim(ClaimTypes.Name, req.Username),
            new Claim(ClaimTypes.Role, "Administrator")
        ];
 
        JwtSecurityToken token = new(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(jwt["ExpireMinutes"] ?? "60")),
            signingCredentials: creds
        );
 
        string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
 
        return Ok(new { token = tokenString, expires = token.ValidTo });
    }
}
 
 
 
 