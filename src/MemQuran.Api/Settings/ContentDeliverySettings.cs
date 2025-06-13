using MemQuran.Api.Models;

namespace MemQuran.Api.Settings;

public class ContentDeliverySettings
{
    public static string SectionName => "ContentDeliverySettings";
    
    public ContentDeliveryType Type { get; set; }
    public CachingSettings CachingSettings { get; set; }
}