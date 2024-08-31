namespace Nursing.Core.Models;

public class AccountModel
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}
public class RegisterModel : AccountModel
{
    public required bool IsAdmin { get; set; }
}
public class LoginModel : AccountModel
{
    public bool RememberMe { get; set; }
}
