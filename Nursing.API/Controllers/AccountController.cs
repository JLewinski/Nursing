﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Nursing.API.Models;
using Nursing.API.Services;
using Nursing.Core.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Nursing.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<NursingUser> _userManager;
    private readonly SignInManager<NursingUser> _signInManager;
    private readonly IConfiguration _configuration;

    public AccountController(IConfiguration configuration, UserManager<NursingUser> userManager, SignInManager<NursingUser> signInManager)
    {
        _configuration = configuration;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
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
            return Ok();
        }
        else
        {
            return BadRequest(result.Errors);
        }
    }

    [HttpPost("login")]
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

            await _signInManager.SignInAsync(user, rememberMe);

            return Ok(token);
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
            expires: rememberMe ? DateTime.MaxValue : DateTime.Now.AddHours(tokenManagement.Expiration),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }
}
