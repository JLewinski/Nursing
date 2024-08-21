using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nursing.Models;

public class Settings
{
    public TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(210);
    public ThemeSetting Theme { get; set; } = ThemeSetting.Auto;
}

public enum ThemeSetting
{
    Dark,
    Light,
    Auto
}