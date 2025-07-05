using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using Topica.Messages;

namespace MemQuran.Api.Extensions;

public static class ActivityExtensions
{
    public static Activity? GetActivityByTraceId(string activitySourceName, string activityStartName, Dictionary<string, string>? sourceProperties, BaseMessage baseMessage, Dictionary<string, string> tags)
    {
        tags.TryAdd("MessageId", baseMessage.Id.ToString());
        tags.TryAdd("MessageConversationId", baseMessage.ConversationId.ToString());
        tags.TryAdd("MessageEventId", baseMessage.EventId.ToString());
        tags.TryAdd("MessageEventName", baseMessage.EventName ?? string.Empty);
        tags.TryAdd("MessageRaisingComponent", baseMessage.RaisingComponent ?? string.Empty);
        tags.TryAdd("MessageSourceIp", baseMessage.SourceIp ?? string.Empty);
        tags.TryAdd("MessageTimeStampUtc", baseMessage.TimeStampUtc.ToString("O"));
        tags.TryAdd("MessageVersion", baseMessage.Version ?? string.Empty);
        tags.TryAdd("MessageGroupId", baseMessage.MessageGroupId ?? string.Empty);
        tags.TryAdd("MessageProperties", string.Join("; ", sourceProperties?.Select(x => $"{x.Key}:{x.Value}") ?? []));
        
        var parentContext = Propagators.DefaultTextMapPropagator.Extract(default, sourceProperties ?? new Dictionary<string, string>(), (headers, key) => headers.TryGetValue(key, out var value) ? [value] : []);
        
        Baggage.Current = parentContext.Baggage;
        
        var activityTagsCollection = new ActivityTagsCollection();
        foreach (var tag in tags)
        {
            activityTagsCollection.Add(tag.Key, tag.Value);
        }
        
        // Need to .AddSource(name) in .WithTracing in OpenTelemetry configuration for ActivitySource not to be NULL
        return new ActivitySource(activitySourceName).StartActivity(activityStartName, ActivityKind.Consumer, parentContext.ActivityContext, activityTagsCollection)?.SetParentId(parentContext.ActivityContext.TraceId.ToString());
    }
}