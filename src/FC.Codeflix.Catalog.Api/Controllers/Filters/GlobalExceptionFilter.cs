using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Validation;
using FC.Codeflix.Catalog.Application.Exceptions;

namespace FC.Codeflix.Catalog.Api.Controllers.Filters;

public class GlobalExceptionFilter(IHostEnvironment env) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        var details = new ProblemDetails();

        if (env.IsDevelopment())
            details.Extensions.Add("StackTrace", exception.StackTrace);

        switch (exception)
        {
            case DomainException domainException:
            {
                var apiError = ApiError.From(domainException);

                details.Title = apiError.Message;
                details.Status = StatusCodes.Status422UnprocessableEntity;
                details.Extensions = new Dictionary<string, object?>
                {
                    {
                        "errors", new[] { apiError.Errors }
                    }
                };
                break;
            }
            case NotFoundException notFoundException:
                details.Title = notFoundException.Message;
                details.Status = StatusCodes.Status404NotFound;
                break;
        }

        context.HttpContext.Response.StatusCode = (int)details.Status!;
        context.Result = new ObjectResult(details);
        context.ExceptionHandled = true;
    }

    private record ApiError(string Message, IReadOnlyList<Error> Errors)
    {
        public static ApiError From(DomainException exception)
            => new(exception.Message, exception.GetErrors().AsReadOnly());
    }
}