using FC.Codeflix.Catalog.Domain.Validation.Handler;

namespace FC.Codeflix.Catalog.Domain.Exceptions;

public class NotificationException(string message, Notification notification)
    : DomainException(message, notification.GetErrors());