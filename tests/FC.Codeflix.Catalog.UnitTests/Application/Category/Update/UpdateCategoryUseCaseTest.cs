using Moq;
using FluentAssertions;
using FC.Codeflix.Catalog.Application;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.UnitTests.Fixtures;
using FC.Codeflix.Catalog.Application.Category.Update;
using FC.Codeflix.Catalog.Application.Exceptions;
using CategoryEntity = FC.Codeflix.Catalog.Domain.Category.Category;
using CategoryRepository = FC.Codeflix.Catalog.Domain.Category.ICategoryRepository;

namespace FC.Codeflix.Catalog.UnitTests.Application.Category.Update;

[Collection(nameof(CategoryFixture))]
public class UpdateCategoryUseCaseTest(CategoryFixture fixture)
{
    [Fact(DisplayName = nameof(GivenAValidCommand_WhenCallsUpdateCategory_ShouldReturnACategory))]
    [Trait("Application", "UpdateCategoryUseCaseTest")]
    public async void GivenAValidCommand_WhenCallsUpdateCategory_ShouldReturnACategory()
    {
        var aCategory = CategoryEntity.NewCategory("Film", "Some description", true);
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var repositoryMock = new Mock<CategoryRepository>();

        var expectedId = aCategory.Id;
        const bool expectedIsActive = true;
        var expectedName = fixture.Name();
        var expectedDescription = fixture.Description();

        repositoryMock.Setup(x => x.Get(
            expectedId,
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(aCategory);

        var aCommand = UpdateCategoryCommand.With(
            expectedId,
            expectedName,
            expectedDescription,
            expectedIsActive
        );

        var useCase = new UpdateCategoryUseCase(repositoryMock.Object, unitOfWorkMock.Object);

        // When
        var output = await useCase.Handle(aCommand, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Name.Should().Be(expectedName);
        output.IsActive.Should().Be(expectedIsActive);
        output.Description.Should().Be(expectedDescription);

        repositoryMock.Verify(
            repository => repository.Get(
                expectedId,
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        repositoryMock.Verify(
            repository => repository.Update(
                It.IsAny<CategoryEntity>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        unitOfWorkMock.Verify(
            uow => uow.Commit(
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    [Theory(DisplayName = nameof(GivenAnInvalidName_WhenCallsUpdateCategory_ShouldReturnNotificationException))]
    [Trait("Application", "UpdateCategoryUseCaseTest")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async void GivenAnInvalidName_WhenCallsUpdateCategory_ShouldReturnNotificationException(
        string? anInvalidName)
    {
        // Given
        var aCategory = CategoryEntity.NewCategory("Film", "Some description", true);

        var expectedId = aCategory.Id;
        const bool expectedIsActive = true;
        var expectedDescription = fixture.Description();

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var repositoryMock = new Mock<CategoryRepository>();

        repositoryMock.Setup(x => x.Get(
            expectedId,
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(aCategory);

        var aCommand = UpdateCategoryCommand.With(expectedId, anInvalidName!, expectedDescription, expectedIsActive);

        var useCase = new UpdateCategoryUseCase(repositoryMock.Object, unitOfWorkMock.Object);

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

    [Fact(DisplayName = nameof(GivenACommandWithInvalidId_WhenCallsUpdateCategory_ShouldReturnNotFoundException))]
    [Trait("Application", "UpdateCategoryUseCaseTest")]
    public async void GivenACommandWithInvalidId_WhenCallsUpdateCategory_ShouldReturnNotFoundException()
    {
        // Given
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var repositoryMock = new Mock<CategoryRepository>();

        var expectedId = Guid.NewGuid();
        const bool expectedIsActive = true;
        var expectedName = fixture.Name();
        var expectedDescription = fixture.Description();

        var expectedErrorMessage = $"Category '{expectedId}' not found";

        repositoryMock.Setup(x => x.Get(
            expectedId,
            It.IsAny<CancellationToken>()
        )).ThrowsAsync(
            new NotFoundException(expectedErrorMessage)
        );

        var useCase = new UpdateCategoryUseCase(repositoryMock.Object, unitOfWorkMock.Object);

        var aCommand = UpdateCategoryCommand.With(expectedId, expectedName!, expectedDescription, expectedIsActive);

        // When
        var task = async () => await useCase.Handle(aCommand, CancellationToken.None);

        // Then
        await task.Should().ThrowAsync<NotFoundException>();

        repositoryMock.Verify(
            repository => repository.Get(
                expectedId,
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }
}