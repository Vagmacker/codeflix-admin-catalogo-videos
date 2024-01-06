using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Validation;
using FC.Codeflix.Catalog.Domain.Validation.Handler;

namespace FC.Codeflix.Catalog.Domain.Category;

public class Category : AggregateRoot
{
    public string Name { get; private set; }
    public bool IsActive { get; private set; }
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    // Entity Framework needs
    private Category() {}
    
    private Category(
        string aName,
        string? aDescription,
        bool isActive,
        DateTime aCreatedAt,
        DateTime aUpdatedAt,
        DateTime? aDeletedAt
    ) : base()
    {
        Name = aName;
        IsActive = isActive;
        CreatedAt = aCreatedAt;
        UpdatedAt = aUpdatedAt;
        DeletedAt = aDeletedAt;
        Description = aDescription;

        SelfValidate();
    }

    public static Category NewCategory(string aName, string? aDescription, bool isActive)
    {
        var now = DateTime.Now;
        DateTime? deletedAt = isActive ? null : now;
        return new Category(aName, aDescription, isActive, now, now, deletedAt);
    }

    public void Activate()
    {
        IsActive = true;
        DeletedAt = default;
        UpdatedAt = DateTime.Now;

        SelfValidate();
    }

    public void Deactivate()
    {
        DeletedAt ??= DateTime.Now;

        IsActive = false;
        UpdatedAt = DateTime.Now;

        SelfValidate();
    }

    public void Update(string aName, string? aDescription, bool isActive)
    {
        if(isActive)
        {
            Activate();
        }
        else
        {
            Deactivate();
        }

        Name = aName;
        Description = aDescription ?? Description;
        UpdatedAt = DateTime.Now;

        SelfValidate();
    }

    private void SelfValidate()
    {
        var notification = Notification.Create();
        Validate(notification);

        if (notification.HasErrors())
            throw new NotificationException("Failed to create a Category", notification);
    }

    public override void Validate(ValidationHandler handler)
        => new CategoryValidator(this, handler).Validate();
}