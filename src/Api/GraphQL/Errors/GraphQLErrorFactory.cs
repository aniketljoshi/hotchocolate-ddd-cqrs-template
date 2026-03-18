using HotChocolate;
using HotChocolateDddCqrsTemplate.Api.GraphQL.Payloads;
using ErrorOrError = ErrorOr.Error;

namespace HotChocolateDddCqrsTemplate.Api.GraphQL.Errors;

public static class GraphQLErrorFactory
{
    public static GraphQLException ToGraphQLException(IReadOnlyList<ErrorOrError> errors)
    {
        return new GraphQLException(errors.Select(ToGraphQLError));
    }

    public static PayloadError ToPayloadError(ErrorOrError error)
    {
        return new PayloadError(error.Code, error.Description);
    }

    private static IError ToGraphQLError(ErrorOrError error)
    {
        return ErrorBuilder.New()
            .SetMessage(error.Description)
            .SetCode(error.Code)
            .SetExtension("errorType", error.Type.ToString())
            .Build();
    }
}
