using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nursing.Core.Models;

public class SimpleUser
{
    public required string Username { get; set; }
    public Guid GroupId { get; set; }
}
