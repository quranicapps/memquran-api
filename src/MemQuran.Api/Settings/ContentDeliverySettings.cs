using MemQuran.Core.Models;

namespace MemQuran.Api.Settings;

public class ContentDeliverySettings
{
    public static string SectionName => "ContentDeliverySettings";
    
    public ContentDeliveryType ContentDeliveryType { get; set; }
    public CachingSettings CachingSettings { get; set; }
}