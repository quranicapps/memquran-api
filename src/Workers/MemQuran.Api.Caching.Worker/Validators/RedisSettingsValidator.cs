using FluentValidation;
using MemQuran.Api.Caching.Worker.Settings;

namespace MemQuran.Api.Caching.Worker.Validators;

public class RedisSettingsValidator : AbstractValidator<RedisSettings>
{
    public RedisSettingsValidator()
    {
        RuleFor(x => x.ConnectionString).NotNull().NotEmpty().WithMessage("Redis connection string must be provided.");
    }
}