using HotChocolateDddCqrsTemplate.Application.Catalog.DTOs;
using HotChocolateDddCqrsTemplate.Domain.Catalog.Repositories;
using MediatR;

namespace HotChocolateDddCqrsTemplate.Application.Catalog.Queries.ListCategories;

public sealed class ListCategoriesQueryHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<ListCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    public async Task<IReadOnlyList<CategoryDto>> Handle(ListCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await categoryRepository.ListAsync(cancellationToken);
        return categories.Select(category => category.ToDto()).ToList();
    }
}
