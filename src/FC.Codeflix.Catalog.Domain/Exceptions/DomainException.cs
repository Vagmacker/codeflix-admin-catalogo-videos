using FC.Codeflix.Catalog.Domain.Validation;

namespace FC.Codeflix.Catalog.Domain.Exceptions;

public class DomainException(string message, List<Error> errors) : Exception(message)
{
    public static DomainException With(Error anError)
        => new(anError.Message, new List<Error>());

    public static DomainException With(List<Error> anErrors)
        => new("", anErrors);

    public List<Error> GetErrors()
        => errors;
} 