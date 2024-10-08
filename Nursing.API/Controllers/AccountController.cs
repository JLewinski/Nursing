﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Nursing.API.Models;
using Nursing.API.Services;
using Nursing.Core.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using NursingSigninResult = Nursing.Core.Models.SignInResult;

namespace Nursing.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<NursingUser> _userManager;
    private readonly SignInManager<NursingUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly SqlContext _context;

    public AccountController(IConfiguration configuration, UserManager<NursingUser> userManager, SignInManager<NursingUser> signInManager, SqlContext context)
    {
        _configuration = configuration;
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var initialAdmin = _configuration.GetSection("InitialAdmin").GetValue<string>("User");
        if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Admin"))
        {
            if (model.Username != initialAdmin)
            {
                return Unauthorized("User is not " + initialAdmin);
            }
            if (await _userManager.FindByNameAsync(initialAdmin) != null)
            {
                return BadRequest("Initial Admin already exists");
            }
        }

        var username = model.Username;
        var password = model.Password;

        var user = new NursingUser
        {
            UserName = username,
            Email = username,
            GroupId = Guid.NewGuid()
        };

        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            if (model.IsAdmin || username == initialAdmin)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }
            return Ok();
        }
        else
        {
            return BadRequest(result.Errors);
        }
    }

    [HttpPost("login")]
    [ProducesResponseType<NursingSigninResult>(200)]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var username = model.Username;
        var password = model.Password;
        var rememberMe = model.RememberMe;

        var user = await _userManager.FindByEmailAsync(username);

        if (user == null)
        {
            return BadRequest("Invalid Password");
        }

        user = await _context.Users.FindAsync(user.Id);

        if (user == null)
        {
            return BadRequest("Invalid Password");
        }

        if (await _userManager.IsLockedOutAsync(user))
        {
            return BadRequest("User is locked out");
        }

        if (await _userManager.CheckPasswordAsync(user, password))
        {
            await _userManager.ResetAccessFailedCountAsync(user);

            var claims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = GetToken(username, claims, rememberMe);
            var refreshToken = GenerateRefreshToken();
            user.RefreshTokens ??= [];
            user.RefreshTokens.Add(refreshToken);
            _context.Update(user);
            await _context.SaveChangesAsync();

            await _signInManager.SignInAsync(user, rememberMe);

            return Ok(new NursingSigninResult { AuthToken = token, RefreshToken = refreshToken.Token });
        }
        else
        {
            var result = await _signInManager.PasswordSignInAsync(user, password, true, true);
            if (result.IsLockedOut)
            {
                return BadRequest("User is locked out");
            }
            else
            {
                return BadRequest("Invalid Password");
            }
        }
    }

    [HttpDelete("delete")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAccount(string username)
    {
        var user = await _userManager.FindByEmailAsync(username);
        if (user == null)
        {
            return BadRequest("Invalid User");
        }

        await _userManager.DeleteAsync(user);

        return Ok();
    }

    [HttpGet("IsAdmin")]
    [Authorize]
    [ProducesResponseType<bool>(200)]
    public IActionResult IsAdmin()
    {
        return Ok(User.IsInRole("Admin"));
    }

    [HttpPost("refreshToken")]
    [ProducesResponseType<NursingSigninResult>(200)]
    public async Task<IActionResult> RefreshToken([FromBody] string clientToken)
    {
        var user = await _context.Users
            .Where(u => u.RefreshTokens.Any(t => t.Token == clientToken))
            .Include(u => u.RefreshTokens)
            .SingleOrDefaultAsync();

        if (user == null || user.UserName == null)
        {
            return BadRequest("Invalid Token");
        }

        var refreshToken = user.RefreshTokens.SingleOrDefault(t => t.Token == clientToken);
        if (refreshToken == null || refreshToken.Revoked != null)
        {
            return BadRequest("Invalid Token");
        }

        if (refreshToken.IsExpired)
        {
            return BadRequest("Token Expired");
        }


        var newRefreshToken = GenerateRefreshToken();

        refreshToken.Revoked = DateTime.UtcNow;
        refreshToken.RevokedByIp = GetClientIp();
        refreshToken.ReplacedByToken = newRefreshToken.Token;

        user.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync();

        var claims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        var token = GetToken(user.UserName, claims, false);
        await _signInManager.SignInAsync(user, false);

        return Ok(new NursingSigninResult { AuthToken = token, RefreshToken = newRefreshToken.Token });
    }

    private string GetClientIp()
    {
        var ip = Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (string.IsNullOrEmpty(ip))
        {
            ip = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
        }
        return ip ?? "NA";
    }

    private RefreshToken GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return new RefreshToken
        {
            Token = Convert.ToBase64String(randomNumber),
            Expires = DateTime.UtcNow.AddDays(7),
            Created = DateTime.UtcNow,
            CreatedByIp = GetClientIp()
        };
    }

    private string GetToken(string username, IEnumerable<Claim> claims, bool rememberMe)
    {
        var tokenManagement = _configuration.GetSection("Token").Get<TokenManagement>();

        if (tokenManagement == null)
        {
            throw new ArgumentException("Token management is missing");
        }

        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenManagement.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claimsList = claims.Append(new Claim(ClaimTypes.Name, username)).ToList();

        var jwtToken = new JwtSecurityToken(
            tokenManagement.Issuer,
            tokenManagement.Audience,
            claimsList,
            expires: rememberMe ? DateTime.MaxValue : DateTime.Now.AddMinutes(tokenManagement.Expiration),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }

    
}
