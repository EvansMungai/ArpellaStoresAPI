using System.ComponentModel.DataAnnotations;

namespace ArpellaStores.Extensions;

public static class BulkUploadValidator<T> where T : class
{
    public static Dictionary<string, List<string>> Validate(T model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model);

        var errorDict = new Dictionary<string, List<string>>();

        if (!Validator.TryValidateObject(model, validationContext, validationResults, true))
        {
            foreach (var result in validationResults)
            {
                foreach (var member in result.MemberNames)
                {
                    if (!errorDict.ContainsKey(member))
                        errorDict[member] = new List<string>();

                    errorDict[member].Add(result.ErrorMessage);
                }
            }
        }

        return errorDict;
    }
}
