using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nursing.Core.Models;

public class SignInResult
{
    public required string AuthToken { get; set; }
    public required string RefreshToken { get; set; }
    public bool IsAdmin { get; init; }
}
