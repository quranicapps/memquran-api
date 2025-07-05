using FluentValidation;
using MemQuran.Api.Settings.Messaging;

namespace MemQuran.Api.Validators;

public class AwsConsumerSettingsValidator : AbstractValidator<AwsConsumerSettings>
{
    public AwsConsumerSettingsValidator()
    {
        RuleFor(x => x.WebUpdateQueueSettings)
            .NotNull().DependentRules(() =>
            {
                RuleFor(x => x.WebUpdateQueueSettings.WorkerName).NotNull().NotEmpty();
                RuleFor(x => x.WebUpdateQueueSettings.Source).NotNull().NotEmpty();
            });
    }
}