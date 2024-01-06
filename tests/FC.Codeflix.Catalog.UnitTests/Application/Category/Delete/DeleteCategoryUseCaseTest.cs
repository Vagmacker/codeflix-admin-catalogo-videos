using Moq;
using FluentAssertions;
using FC.Codeflix.Catalog.Application;
using FC.Codeflix.Catalog.Domain.Category;
using FC.Codeflix.Catalog.UnitTests.Fixtures;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.Category.Delete;

namespace FC.Codeflix.Catalog.UnitTests.Application.Category.Delete;

[Collection(nameof(CategoryFixture))]
public class DeleteCategoryUseCaseTest(CategoryFixture fixture)
{
    [Fact(DisplayName = nameof(GivenAValidId_WhenCallsDeleteCategory_ShouldDeleteIt))]
    [Trait("Application", "DeleteCategoryUseCaseTest")]
    public async void GivenAValidId_WhenCallsDeleteCategory_ShouldDeleteIt()
    {
        // Given
        var aCategory = fixture.Movies();
        var expectedId = aCategory.Id;

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var repositoryMock = new Mock<ICategoryRepository>();

        repositoryMock.Setup(x => x.Get(
            expectedId,
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(aCategory);

        var aCommand = new DeleteCategoryCommand(expectedId);
        var useCase = new DeleteCategoryUseCase(repositoryMock.Object, unitOfWorkMock.Object);

        // When
        await useCase.Handle(aCommand, CancellationToken.None);

        // Then
        repositoryMock.Verify(
            repository => repository.Get(
                expectedId,
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        repositoryMock.Verify(
            repository => repository.Delete(
                aCategory,
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        unitOfWorkMock.Verify(uow => uow.Commit(
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    [Fact(DisplayName =
        nameof(GivenAValidId_WhenCallsDeleteCategoryAndDoesNotExistCategory_ShouldReturnNotFoundException))]
    [Trait("Application", "DeleteCategoryUseCaseTest")]
    public async void GivenAValidId_WhenCallsDeleteCategoryAndDoesNotExistCategory_ShouldReturnNotFoundException()
    {
        // Given
        var expectedId = Guid.NewGuid();

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var repositoryMock = new Mock<ICategoryRepository>();

        repositoryMock.Setup(x => x.Get(
            expectedId,
            It.IsAny<CancellationToken>()
        )).ThrowsAsync(
            new NotFoundException($"Category '{expectedId}' not found")
        );

        var aCommand = new DeleteCategoryCommand(expectedId);
        var useCase = new DeleteCategoryUseCase(repositoryMock.Object, unitOfWorkMock.Object);

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