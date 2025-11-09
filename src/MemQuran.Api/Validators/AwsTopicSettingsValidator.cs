using FluentValidation;
using MemQuran.Api.Settings;

namespace MemQuran.Api.Validators;

public class AwsTopicSettingsValidator : AbstractValidator<AwsTopicSettings>
{
    public AwsTopicSettingsValidator()
    {
        RuleFor(x => x.WebUpdateTopicSettings)
            .NotNull().DependentRules(() =>
            {
                RuleFor(x => x.WebUpdateTopicSettings.WorkerName).NotNull().NotEmpty();
                RuleFor(x => x.WebUpdateTopicSettings.Source).NotNull().NotEmpty();
            });
    }
}