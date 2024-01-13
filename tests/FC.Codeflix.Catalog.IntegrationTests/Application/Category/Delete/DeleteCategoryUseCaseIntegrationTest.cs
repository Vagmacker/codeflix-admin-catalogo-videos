using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.IntegrationTests.Fixtures;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.Codeflix.Catalog.Application.Category.Delete;
using CategoryEntity = FC.Codeflix.Catalog.Domain.Category.Category;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.Category.Delete;

[Collection(nameof(CategoryFixture))]
public class DeleteCategoryUseCaseIntegrationTest(CategoryFixture fixture)
{
    [Fact(DisplayName = nameof(GivenAValidCategory_WhenCallsDeleteCategory_ShouldDeleteIt))]
    [Trait("Integration/Application", "DeleteCategoryUseCase")]
    public async Task GivenAValidCategory_WhenCallsDeleteCategory_ShouldDeleteIt()
    {
        // Given
        var dbContext = fixture.CreateDbContext();
        var unitOfWork = new UnitOfWork(dbContext);
        var categoryRepository = new CategoryRepository(dbContext);

        var actionCategory = CategoryEntity.NewCategory("Action", "Some description", true);
        var horrorCategory = CategoryEntity.NewCategory("Horror", "Some description", true);

        await dbContext.AddAsync(horrorCategory);
        var tracking = await dbContext.AddAsync(actionCategory);
        await dbContext.SaveChangesAsync();
        tracking.State = EntityState.Detached;

        (await dbContext.Categories.ToListAsync()).Should().HaveCount(2);

        var aCommand = new DeleteCategoryCommand(actionCategory.Id);
        var useCase = new DeleteCategoryUseCase(categoryRepository, unitOfWork);

        // When
        await useCase.Handle(aCommand, CancellationToken.None);

        // Then
        var assertDbContext = fixture.CreateDbContext(true);
        var categoryDeleted = await assertDbContext.Categories.FindAsync(actionCategory.Id);

        categoryDeleted.Should().BeNull();

        (await assertDbContext.Categories.ToListAsync()).Should().HaveCount(1);
    }
}