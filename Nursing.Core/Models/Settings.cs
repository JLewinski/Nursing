using System.ComponentModel.DataAnnotations;

namespace Nursing.Models;

public class Settings
{
    [Key]
    public int Id { get; set; } = 1;
    public TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(210);
    public ThemeSetting Theme { get; set; } = ThemeSetting.Auto;
    public DateTime LastSync { get; set; } = DateTime.MinValue;
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public bool IsAdmin { get; set; } = false;
}

public enum ThemeSetting
{
    Dark,
    Light,
    Auto
}