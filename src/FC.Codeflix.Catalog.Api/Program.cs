using FC.Codeflix.Catalog.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDatabaseConnection(builder.Configuration)
    .AddUseCases()
    .AddAndConfigureControllers()
    .AddCors(p => p.AddPolicy("CORS", policy =>
        policy.WithOrigins("*").AllowAnyMethod().AllowAnyHeader())
    );

var app = builder.Build();

app.UseCors("CORS");
// app.UseHttpLogging();
app.MapControllers();
app.UseDocumentation();

app.Run();

public partial class Program
{
}