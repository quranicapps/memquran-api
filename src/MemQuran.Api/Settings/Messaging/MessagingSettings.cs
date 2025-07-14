namespace MemQuran.Api.Settings.Messaging;

public class MessagingSettings
{
    public static string SectionName => nameof(MessagingSettings);

    public bool AwsEnabled { get; set; }
}