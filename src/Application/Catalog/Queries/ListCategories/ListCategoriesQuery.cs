using HotChocolateDddCqrsTemplate.Application.Catalog.DTOs;
using HotChocolateDddCqrsTemplate.Application.Common.Interfaces;

namespace HotChocolateDddCqrsTemplate.Application.Catalog.Queries.ListCategories;

public sealed record ListCategoriesQuery : IQuery<IReadOnlyList<CategoryDto>>;
