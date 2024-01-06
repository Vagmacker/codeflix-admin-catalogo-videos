namespace FC.Codeflix.Catalog.Domain.Validation;

public abstract class ValidationHandler
{
    public abstract List<Error> GetErrors();

    public abstract ValidationHandler Append(Error anError);

    public abstract ValidationHandler Append(ValidationHandler aHandler);

    public bool HasErrors()
        => GetErrors().Count > 0;

    public Error? FirstError()
        => GetErrors().Count > 0 ? GetErrors()[0] : null;
}