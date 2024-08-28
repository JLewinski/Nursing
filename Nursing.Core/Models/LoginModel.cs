namespace Nursing.Core.Models;

public class RegisterModel
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}
public class LoginModel : RegisterModel
{
    public bool RememberMe { get; set; }
}
