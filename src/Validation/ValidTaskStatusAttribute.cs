using System.ComponentModel.DataAnnotations;
using TaskManagerApi.Constants;

namespace TaskManagerApi.Validation;

public class ValidTaskStatusAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string status && TaskStatuses.IsValid(status))
            return ValidationResult.Success;

        var allowed = string.Join(", ", TaskStatuses.AllowedValues);
        return new ValidationResult($"Status deve ser um dos valores: {allowed}.");
    }
}
