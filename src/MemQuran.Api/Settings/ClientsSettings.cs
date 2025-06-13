namespace MemQuran.Api.Settings;

public class ClientsSettings
{
    public static string SectionName => "Clients";
    public required ClientSettings JsDelivrService { get; init; }
}

public class ClientSettings
{
    public bool Enabled { get; set; }
    public string BaseUrl { get; set; }
    public string Version { get; set; }
    public TimeSpan DefaultTimeout { get; set; }
    public int IncrementalBackoffFactorSeconds { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string ApiKey { get; set; }
    public string TokenEndpointUrl { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string Scope { get; set; }
    public string GrantType { get; set; }
    public string RefreshToken { get; set; }
    public string AccessToken { get; set; }
    public string TokenType { get; set; }
    public string RefreshTokenEndpointUrl { get; set; }
    public string RefreshTokenClientId { get; set; }
    public string RefreshTokenClientSecret { get; set; }
    public string RefreshTokenGrantType { get; set; }
    public string RefreshTokenScope { get; set; }
    public string RefreshTokenTokenType { get; set; }
}