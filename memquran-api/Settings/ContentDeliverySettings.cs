using QuranApi.Models;

namespace QuranApi.Settings;

public class ContentDeliverySettings
{
    public static string SectionName => "ContentDeliverySettings";
    
    public ContentDeliveryType Type { get; set; }
    public CachingSettings CachingSettings { get; set; }
}