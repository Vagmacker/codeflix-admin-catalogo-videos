using Moq;
using FluentAssertions;
using FC.Codeflix.Catalog.Application;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.UnitTests.Fixtures;
using FC.Codeflix.Catalog.Application.Category.Create;
using CategoryEntity = FC.Codeflix.Catalog.Domain.Category.Category;
using CategoryRepository = FC.Codeflix.Catalog.Domain.Category.ICategoryRepository;

namespace FC.Codeflix.Catalog.UnitTests.Application.Category.Create;

[Collection(nameof(CategoryFixture))]
public class CreateCategoryUseCaseTest(CategoryFixture fixture)
{
    [Fact(DisplayName = nameof(GivenAValidCommand_WhenCallsCreateCategory_ShouldReturnIt))]
    [Trait("Application", "CreateCategoryUseCaseTest")]
    public async void GivenAValidCommand_WhenCallsCreateCategory_ShouldReturnIt()
    {
        // Given
        const bool expectedIsActive = true;
        var expectedName = fixture.Name();
        var expectedDescription = fixture.Description();

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var repositoryMock = new Mock<CategoryRepository>();

        var aCommand = CreateCategoryCommand.With(expectedName, expectedDescription, expectedIsActive);

        var useCase = new CreateCategoryUseCase(repositoryMock.Object, unitOfWorkMock.Object);

        // When
        var output = await useCase.Handle(aCommand, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.IsActive.Should().BeTrue();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(expectedName);
        output.Description.Should().Be(expectedDescription);
        output.CreatedAt.Should().NotBeSameDateAs(default);

        repositoryMock.Verify(
            repository => repository.Insert(
                It.IsAny<CategoryEntity>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        unitOfWorkMock.Verify(
            uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory(DisplayName = nameof(GivenAnInvalidName_WhenCallsCreateCategory_ShouldReturnNotificationException))]
    [Trait("Application", "CreateCategoryUseCaseTest")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async void GivenAnInvalidName_WhenCallsCreateCategory_ShouldReturnNotificationException(string? anInvalidName)
    {
        // Given
        const bool expectedIsActive = true;
        var expectedDescription = fixture.Description();

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var repositoryMock = new Mock<CategoryRepository>();

        var aCommand = CreateCategoryCommand.With(anInvalidName!, expectedDescription, expectedIsActive);

        var useCase = new CreateCategoryUseCase(repositoryMock.Object, unitOfWorkMock.Object);

        // When
        var action = () => useCase.Handle(aCommand, CancellationToken.None);

        const string expectedErrorMessage = "Name should not be null or empty";

        // Then
        var exception = await action.Should()
            .ThrowAsync<NotificationException>()
            .WithMessage("Failed to create a Category");

        exception.Which.GetErrors()[0].Message.Should().Be(expectedErrorMessage);

        repositoryMock.Verify(
            repository => repository.Insert(
                It.IsAny<CategoryEntity>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );

        unitOfWorkMock.Verify(
            uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact(DisplayName = nameof(GivenAnInvalidNameLengthLessThan3_WhenCallsCreateCategory_ShouldReturnNotificationException))]
    [Trait("Application", "CreateCategoryUseCaseTest")]
    public async void GivenAnInvalidNameLengthLessThan3_WhenCallsCreateCategory_ShouldReturnNotificationException()
    {
        // Given
        const string expectedName = "Fi ";
        const bool expectedIsActive = true;
        var expectedDescription = fixture.Description();

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var repositoryMock = new Mock<CategoryRepository>();

        var aCommand = CreateCategoryCommand.With(expectedName, expectedDescription, expectedIsActive);

        var useCase = new CreateCategoryUseCase(repositoryMock.Object, unitOfWorkMock.Object);

        // When
        var action = () => useCase.Handle(aCommand, CancellationToken.None);

        const string expectedErrorMessage = "Name should be between 3 and 255 characters";

        // Then
        var exception = await action.Should()
            .ThrowAsync<NotificationException>()
            .WithMessage("Failed to create a Category");

        exception.Which.GetErrors()[0].Message.Should().Be(expectedErrorMessage);

        repositoryMock.Verify(
            repository => repository.Insert(
                It.IsAny<CategoryEntity>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );

        unitOfWorkMock.Verify(
            uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact(DisplayName = nameof(GivenAnInvalidNameLengthMoreThan255_WhenCallsCreateCategory_ShouldReturnNotificationException))]
    [Trait("Application", "CreateCategoryUseCaseTest")]
    public async void GivenAnInvalidNameLengthMoreThan255_WhenCallsCreateCategory_ShouldReturnNotificationException()
    {
        // Given
        const bool expectedIsActive = true;
        var expectedDescription = fixture.Description();
        var expectedName = string.Join(null, Enumerable.Range(1, 256).Select(_ => "a").ToArray());

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var repositoryMock = new Mock<CategoryRepository>();

        var aCommand = CreateCategoryCommand.With(expectedName, expectedDescription, expectedIsActive);

        var useCase = new CreateCategoryUseCase(repositoryMock.Object, unitOfWorkMock.Object);

        // When
        var action = () => useCase.Handle(aCommand, CancellationToken.None);

        const string expectedErrorMessage = "Name should be between 3 and 255 characters";

        // Then
        var exception = await action.Should()
            .ThrowAsync<NotificationException>()
            .WithMessage("Failed to create a Category");

        exception.Which.GetErrors()[0].Message.Should().Be(expectedErrorMessage);

        repositoryMock.Verify(
            repository => repository.Insert(
                It.IsAny<CategoryEntity>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );

        unitOfWorkMock.Verify(
            uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }
}