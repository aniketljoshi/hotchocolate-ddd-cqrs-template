using FluentValidation;

namespace HotChocolateDddCqrsTemplate.Application.Catalog.Commands.CreateProduct;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(command => command.Price)
            .GreaterThan(0);

        RuleFor(command => command.Currency)
            .NotEmpty()
            .Length(3);

        RuleFor(command => command.Sku)
            .NotEmpty()
            .Length(3, 32)
            .Matches("^[A-Za-z0-9-]+$");

        RuleFor(command => command.CategoryId)
            .NotEqual(Guid.Empty);
    }
}
