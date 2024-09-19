using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<NursingUser> _userManager;
    private readonly SignInManager<NursingUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly PostgresContext _context;

    public AccountController(IConfiguration configuration, UserManager<NursingUser> userManager, SignInManager<NursingUser> signInManager, PostgresContext context)
    {
        _configuration = configuration;
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    [HttpPost("register")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var username = model.Username;
        var password = model.Password;

        var user = new NursingUser
        {
            UserName = username,
            Email = username,
            GroupId = Guid.NewGuid(),
            RefreshTokens = []
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        if (model.IsAdmin)
        {
            await _userManager.AddToRoleAsync(user, "Admin");
        }
        return Ok();
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
            var refreshToken = RefreshToken.Generate(user, GetClientIp());
            user.RefreshTokens ??= [];
            user.RefreshTokens.Add(refreshToken);
            _context.Update(user);
            await _context.SaveChangesAsync();

            await _signInManager.SignInAsync(user, rememberMe);

            return Ok(new NursingSigninResult { AuthToken = token, RefreshToken = refreshToken.Token, IsAdmin = await _userManager.IsInRoleAsync(user, "Admin") });
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

    [HttpGet("getUsers")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [ProducesResponseType<List<SimpleUser>>(200)]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _context.Users
            .Select(x => new SimpleUser
            {
                Username = x.UserName ?? x.Email ?? x.Id.ToString(),
                GroupId = x.GroupId
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }

    [HttpDelete("delete/{username}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> DeleteAccount(string username)
    {
        if (User.Identity?.Name != username && !User.IsInRole("Admin"))
        {
            return Unauthorized("You do not have permission to delete this user.");
        }

        var user = await _userManager.FindByEmailAsync(username);
        if (user == null)
        {
            return BadRequest("Invalid User");
        }

        var usernames = await _context.Users
            .Where(x => x.GroupId == user.GroupId && x.Id != user.Id)
            .Select(x => x.UserName)
            .ToListAsync();

        if (usernames.Count == 0)
        {
            await _context.Feedings.Where(x => x.GroupId == user.GroupId).ExecuteDeleteAsync();
            await _userManager.DeleteAsync(user);
            return Ok($"{username} and all data belonging to {username} were deleted");
        }

        await _userManager.DeleteAsync(user);

        if (User.Identity!.Name == username)
        {
            await _signInManager.SignOutAsync();
        }
        return Ok($"{username} was deleted. Data still in database attached to the following users: {string.Join(", ", usernames)}");
    }

    [HttpPost("changePassword")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
    {
        var user = await _userManager.FindByEmailAsync(User.Identity!.Name!);

        var result = await _userManager.ChangePasswordAsync(user!, model.CurrentPassword, model.NewPassword);
        return result.Succeeded ? Ok() : BadRequest(result.Errors);
    }

    [HttpGet("IsAdmin")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType<bool>(200)]
    public IActionResult IsAdmin()
    {
        return Ok(User.IsInRole("Admin"));
    }

    [HttpPost("refreshToken")]
    [ProducesResponseType<NursingSigninResult>(200)]
    public async Task<IActionResult> RefreshUserToken([FromBody] string clientToken)
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

        var clientIp = GetClientIp();
        var newRefreshToken = RefreshToken.Generate(user, clientIp);

        refreshToken.Revoked = DateTime.UtcNow;
        refreshToken.RevokedByIp = clientIp;
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

    private string GetToken(string username, IEnumerable<Claim> claims, bool rememberMe)
    {
        var tokenManagement = _configuration.GetSection("Token").Get<TokenManagement>() ?? throw new ArgumentException("Token management is missing");
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
