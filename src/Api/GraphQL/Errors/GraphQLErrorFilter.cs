using FluentValidation;
using HotChocolate;

namespace HotChocolateDddCqrsTemplate.Api.GraphQL.Errors;

public sealed class GraphQLErrorFilter : IErrorFilter
{
    public IError OnError(IError error)
    {
        if (error.Exception is not ValidationException validationException)
        {
            return error;
        }

        return error
            .WithMessage("Request validation failed.")
            .WithCode("VALIDATION_ERROR")
            .SetExtension(
                "validationErrors",
                validationException.Errors
                    .Select(failure => new
                    {
                        failure.PropertyName,
                        failure.ErrorMessage
                    })
                    .ToList());
    }
}
