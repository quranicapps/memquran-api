namespace MemQuran.Api.Settings;

public class IdentityProviderSettings
{
    public static string SectionName => nameof(IdentityProviderSettings);
    
    public string Authority { get; set; } = null!;
    public string Audience { get; set; } = null!;
}