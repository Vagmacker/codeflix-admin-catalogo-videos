using Moq;
using FluentAssertions;
using FC.Codeflix.Catalog.Domain.Category;
using FC.Codeflix.Catalog.Domain.Pagination;
using FC.Codeflix.Catalog.UnitTests.Fixtures;
using FC.Codeflix.Catalog.Application.Category.Retrieve.List;
using CategoryEntity = FC.Codeflix.Catalog.Domain.Category.Category;

namespace FC.Codeflix.Catalog.UnitTests.Application.Category.Retrieve.List;

[Collection(nameof(CategoryFixture))]
public class ListCategoriesUseCaseTest(CategoryFixture fixture)
{
    [Fact(DisplayName = nameof(GivenAValidQuery_WhenCallsListCategories_ThenShouldReturnCategories))]
    [Trait("Application", "ListCategoriesUseCaseTest")]
    public async void GivenAValidQuery_WhenCallsListCategories_ThenShouldReturnCategories()
    {
        // Given
        var repositoryMock = new Mock<ICategoryRepository>();
        var categories = new List<CategoryEntity>()
        {
            CategoryEntity.NewCategory("Movies", "Some description", true),
            CategoryEntity.NewCategory("Series", "Some description", true)
        };

        const int expectedPage = 0;
        const int expectedPerPage = 10;
        const string expectedTerms = "";
        const string expectedSort = "createdAt";

        var aCommand = new ListCategoriesCommand(
            expectedPage,
            expectedPerPage,
            expectedTerms,
            expectedSort
        );

        var expectedPagination =
            new Page<CategoryEntity>(expectedPage, expectedPerPage, categories.Count, categories);

        var expectedResult = expectedPagination.Map(ListCategoriesOutput.From);

        repositoryMock.Setup(x => x.GetAll(
            It.Is<SearchQuery>(query => query.Page == aCommand.Page
                                        && query.PerPage == aCommand.PerPage
                                        && query.Terms == aCommand.Terms
                                        && query.Sort == aCommand.Sort
                                        && query.Direction == aCommand.Direction
            ),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(expectedPagination);

        var useCase = new ListCategoriesUseCase(repositoryMock.Object);

        // When
        var output = await useCase.Handle(aCommand, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Meta.Total.Should().Be(categories.Count);
        output.Meta.PerPage.Should().Be(expectedPerPage);
        output.Meta.CurrentPage.Should().Be(expectedPage);
        output.Data.Should().Contain(expectedResult.Data);
        output.Data.Should().HaveCount(expectedResult.Data.Count);

        repositoryMock.Verify(x => x.GetAll(
            It.Is<SearchQuery>(query => query.Page == aCommand.Page
                                        && query.PerPage == aCommand.PerPage
                                        && query.Terms == aCommand.Terms
                                        && query.Sort == aCommand.Sort
                                        && query.Direction == aCommand.Direction
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact(DisplayName = nameof(GivenAValidQuery_WhenHasNoResults_ThenShouldReturnEmptyCategories))]
    [Trait("Application", "ListCategoriesUseCaseTest")]
    public async void GivenAValidQuery_WhenHasNoResults_ThenShouldReturnEmptyCategories()
    {
        // Given
        var repositoryMock = new Mock<ICategoryRepository>();
        var categories = new List<CategoryEntity>().AsReadOnly();

        const int expectedPage = 0;
        const int expectedPerPage = 10;
        const string expectedTerms = "";
        const string expectedSort = "createdAt";

        var aCommand = new ListCategoriesCommand(
            expectedPage,
            expectedPerPage,
            expectedTerms,
            expectedSort
        );
        
        var expectedPagination =
            new Page<CategoryEntity>(expectedPage, expectedPerPage, categories.Count, categories);
        
        repositoryMock.Setup(x => x.GetAll(
            It.Is<SearchQuery>(query => query.Page == aCommand.Page
                                        && query.PerPage == aCommand.PerPage
                                        && query.Terms == aCommand.Terms
                                        && query.Sort == aCommand.Sort
                                        && query.Direction == aCommand.Direction
            ),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(expectedPagination);
        
        var useCase = new ListCategoriesUseCase(repositoryMock.Object);

        // When
        var output = await useCase.Handle(aCommand, CancellationToken.None);
        
        // Then
        output.Should().NotBeNull();
        output.Data.Should().BeEmpty();
        output.Meta.Total.Should().Be(categories.Count);
        output.Meta.PerPage.Should().Be(expectedPerPage);
        output.Meta.CurrentPage.Should().Be(expectedPage);

        repositoryMock.Verify(x => x.GetAll(
            It.Is<SearchQuery>(query => query.Page == aCommand.Page
                                        && query.PerPage == aCommand.PerPage
                                        && query.Terms == aCommand.Terms
                                        && query.Sort == aCommand.Sort
                                        && query.Direction == aCommand.Direction
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }
}