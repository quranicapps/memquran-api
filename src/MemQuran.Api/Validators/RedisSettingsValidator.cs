using FluentValidation;
using MemQuran.Api.Settings;

namespace MemQuran.Api.Validators;

public class RedisSettingsValidator : AbstractValidator<RedisSettings>
{
    public RedisSettingsValidator()
    {
        RuleFor(x => x.ConnectionString).NotNull().NotEmpty().WithMessage("Redis connection string must be provided.");
    }
}