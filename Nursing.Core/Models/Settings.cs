using System.ComponentModel.DataAnnotations;

namespace Nursing.Models;

public class Settings
{
    [Key]
    public int Id { get; set; } = 1;
    public TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(210);
    public ThemeSetting Theme { get; set; } = ThemeSetting.Auto;
}

public enum ThemeSetting
{
    Dark,
    Light,
    Auto
}