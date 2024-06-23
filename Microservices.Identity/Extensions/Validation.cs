using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Microservices.Identity.Extensions;

public static class Validation
{
    [RequiresUnreferencedCode("Validation logic might require unreferenced code.")]
    public static List<ValidationResult> Validate<T>(T model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model);
        Validator.TryValidateObject(model, context, results, true);
        return results;
    }
}
