using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nursing.Models;

public class Settings
{
    public TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(210);
}