using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.IntegrationTests.Fixtures;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.Codeflix.Catalog.Application.Category.Create;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.Category.Create;

[Collection(nameof(CategoryFixture))]
public class CreateCategoryUseCaseIntegrationTest(CategoryFixture fixture) : IDisposable
{
    [Fact(DisplayName = nameof(GivenAValidCommand_WhenCallsCreateCategory_ShouldReturnCategoryOutput))]
    [Trait("Integration/Application", "CreateCategoryUseCase")]
    public async void GivenAValidCommand_WhenCallsCreateCategory_ShouldReturnCategoryOutput()
    {
        // Given
        var dbContext = fixture.CreateDbContext();
        var unitOfWork = new UnitOfWork(dbContext);
        var categoryRepository = new CategoryRepository(dbContext);

        const string expectedName = "Action";
        const string expectedDescription = "Some description";

        var useCase = new CreateCategoryUseCase(categoryRepository, unitOfWork);
        var aCommand = CreateCategoryCommand.With(expectedName, expectedDescription);

        // When
        var output = await useCase.Handle(aCommand, CancellationToken.None);

        // When
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.IsActive.Should().BeTrue();
        output.Name.Should().Be(expectedName);
        output.CreatedAt.Should().NotBeSameDateAs(default);
        output.Description.Should().Be(expectedDescription);

        var actualCategory = await dbContext.Categories.FindAsync(output.Id);

        actualCategory.Should().NotBeNull();
        actualCategory!.Id.Should().NotBeEmpty();
        actualCategory.IsActive.Should().BeTrue();
        actualCategory.DeletedAt.Should().BeNull();
        actualCategory.Name.Should().Be(expectedName);
        actualCategory.CreatedAt.Should().NotBeSameDateAs(default);
        actualCategory.UpdatedAt.Should().NotBeSameDateAs(default);
        actualCategory.Description.Should().Be(expectedDescription);
    }

    [Fact(DisplayName = nameof(GivenAnInvalidName_WhenCallsCreateCategory_ShouldThrowsNotificationException))]
    [Trait("Integration/Application", "CreateCategoryUseCase")]
    public async void GivenAnInvalidName_WhenCallsCreateCategory_ShouldThrowsNotificationException()
    {
        // Given
        var dbContext = fixture.CreateDbContext();
        var unitOfWork = new UnitOfWork(dbContext);
        var categoryRepository = new CategoryRepository(dbContext);

        var useCase = new CreateCategoryUseCase(categoryRepository, unitOfWork);
        var aCommand = CreateCategoryCommand.With(null!, "Some description");

        // When
        var task = async () => await useCase.Handle(aCommand, CancellationToken.None);

        // Then
        const string expectedErrorMessage = "Name should not be null or empty";

        var exception = await task.Should().ThrowAsync<NotificationException>()
            .WithMessage("Failed to create a Category");

        exception.Which.GetErrors()[0].Message.Should().Be(expectedErrorMessage);

        dbContext.Categories
            .AsNoTracking()
            .ToList()
            .Should()
            .HaveCount(0);
    }

    [Fact(DisplayName = nameof(GivenAnInvalidDescription_WhenCallsCreateCategory_ShouldThrowsNotificationException))]
    [Trait("Integration/Application", "CreateCategoryUseCase")]
    public async void GivenAnInvalidDescription_WhenCallsCreateCategory_ShouldThrowsNotificationException()
    {
        // Given
        var dbContext = fixture.CreateDbContext();
        var unitOfWork = new UnitOfWork(dbContext);
        var categoryRepository = new CategoryRepository(dbContext);

        var useCase = new CreateCategoryUseCase(categoryRepository, unitOfWork);
        var aCommand = CreateCategoryCommand.With("Action", null!);

        // When
        var task = async () => await useCase.Handle(aCommand, CancellationToken.None);

        // Then
        const string expectedErrorMessage = "Description should not be null";

        var exception = await task.Should().ThrowAsync<NotificationException>()
            .WithMessage("Failed to create a Category");

        exception.Which.GetErrors()[0].Message.Should().Be(expectedErrorMessage);

        dbContext.Categories
            .AsNoTracking()
            .ToList()
            .Should()
            .HaveCount(0);
    }

    public void Dispose()
        => fixture.CreateDbContext().Database.EnsureDeleted();
}