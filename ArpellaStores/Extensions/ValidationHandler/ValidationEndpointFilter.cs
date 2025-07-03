using System.ComponentModel.DataAnnotations;

namespace ArpellaStores.Extensions;

public class ValidationEndpointFilter<T> : IEndpointFilter where T : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var model = context.Arguments.OfType<T>().FirstOrDefault();
        if (model == null) return Results.BadRequest($"Missing payload of type {typeof(T).Name}");

        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model);

        if (!Validator.TryValidateObject(model, validationContext, validationResults, true))
        {
            var errorDict = new Dictionary<string, List<string>>();

            foreach (var result in validationResults)
            {
                foreach (var member in result.MemberNames)
                {
                    if (!errorDict.ContainsKey(member))
                        errorDict[member] = new List<string>();

                    errorDict[member].Add(result.ErrorMessage);
                }
            }

            return Results.BadRequest(new
            {
                Message = "Validation failed",
                Errors = errorDict
            });
        }
        return await next(context);
    }
}
