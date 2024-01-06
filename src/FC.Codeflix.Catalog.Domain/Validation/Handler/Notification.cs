namespace FC.Codeflix.Catalog.Domain.Validation.Handler;

public class Notification : ValidationHandler
{
    private readonly List<Error> _errors;

    private Notification(List<Error> anErrors)
        => _errors = anErrors;

    public static Notification Create()
    {
        return new Notification(new List<Error>());
    }

    public static Notification Create(Error anError)
    {
        return (Notification)new Notification(new List<Error>()).Append(anError);
    }

    public override List<Error> GetErrors()
        => _errors;

    public override ValidationHandler Append(Error anError)
    {
        _errors.Add(anError);
        return this;
    }

    public override ValidationHandler Append(ValidationHandler aHandler)
    {
        _errors.AddRange(aHandler.GetErrors());
        return this;
    }
}