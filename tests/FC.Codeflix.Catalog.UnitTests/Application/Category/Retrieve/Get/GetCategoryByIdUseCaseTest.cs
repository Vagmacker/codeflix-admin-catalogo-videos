using Moq;
using FluentAssertions;
using FluentValidation;
using FC.Codeflix.Catalog.Domain.Category;
using FC.Codeflix.Catalog.UnitTests.Fixtures;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.Category.Retrieve.Get;

namespace FC.Codeflix.Catalog.UnitTests.Application.Category.Retrieve.Get;

[Collection(nameof(CategoryFixture))]
public class GetCategoryByIdUseCaseTest(CategoryFixture fixture)
{
    [Fact(DisplayName = nameof(GivenAValidId_WhenCallGetCategory_ShouldReturnACategory))]
    [Trait("Application", "GetCategoryByIdUseCaseTest")]
    public async void GivenAValidId_WhenCallGetCategory_ShouldReturnACategory()
    {
        // Given
        var category = fixture.Movies();
        var repositoryMock = new Mock<ICategoryRepository>();
        
        var expectedId = category.Id;

        repositoryMock.Setup(it => it.Get(
            expectedId, 
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(category);

        var useCase = new GetCategoryByIdUseCase(repositoryMock.Object);

        // When
        var output = await useCase.Handle(new GetCategoryCommand(expectedId), CancellationToken.None);
        
        // Then
        output.Should().NotBeNull();
        output.IsActive.Should().BeTrue();
        output.Name.Should().Be(category.Name);
        output.Id.Should().Be(expectedId.ToString());
        output.CreatedAt.Should().Be(category.CreatedAt);
        output.Description.Should().Be(category.Description);

        repositoryMock.Verify(
            repository => repository.Get(
                expectedId,
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    [Fact(DisplayName = nameof(GivenAnInvalidId_WhenCallGetCategoryAndDoesNotExists_ShouldReturnNotFoundException))]
    [Trait("Application", "GetCategoryByIdUseCaseTest")]
    public async void GivenAnInvalidId_WhenCallGetCategoryAndDoesNotExists_ShouldReturnNotFoundException()
    {
        // Given
        var expectedId = Guid.NewGuid();
        var repositoryMock = new Mock<ICategoryRepository>();

        var expectedErrorMessage = $"Category '{expectedId}' not found";
        
        repositoryMock.Setup(x => x.Get(
            expectedId,
            It.IsAny<CancellationToken>()
        )).ThrowsAsync(
            new NotFoundException(expectedErrorMessage)
        );
        
        var aCommand = new GetCategoryCommand(expectedId);
        var useCase = new GetCategoryByIdUseCase(repositoryMock.Object);

        // When
        var task = async () => await useCase.Handle(aCommand, CancellationToken.None);

        // Then
        await task.Should().ThrowAsync<NotFoundException>();
        repositoryMock.Verify(x => x.Get(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact(DisplayName = nameof(GivenAnInvalidEmptyId_WhenGetCategory_ShouldReturnError))]
    [Trait("Application", "GetCategoryByIdUseCaseTest")]
    public void GivenAnInvalidEmptyId_WhenGetCategory_ShouldReturnError()
    {
        ValidatorOptions.Global.LanguageManager.Enabled = false;

        // Given
        var aValidator = new GetCategoryCommandValidator();
        var aCommand = new GetCategoryCommand(Guid.Empty);

        const string expectedErrorMessage = "'Id' must not be empty.";
        
        // When
        var aValidationResult = aValidator.Validate(aCommand);

        // Then
        aValidationResult.Should().NotBeNull();
        aValidationResult.IsValid.Should().BeFalse();
        aValidationResult.Errors.Should().HaveCount(1);
        aValidationResult.Errors[0].ErrorMessage.Should().Be(expectedErrorMessage);
    }
}