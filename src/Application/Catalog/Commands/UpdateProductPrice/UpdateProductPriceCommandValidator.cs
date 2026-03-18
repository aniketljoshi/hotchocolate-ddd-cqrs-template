using FluentValidation;

namespace HotChocolateDddCqrsTemplate.Application.Catalog.Commands.UpdateProductPrice;

public sealed class UpdateProductPriceCommandValidator : AbstractValidator<UpdateProductPriceCommand>
{
    public UpdateProductPriceCommandValidator()
    {
        RuleFor(command => command.ProductId)
            .NotEqual(Guid.Empty);

        RuleFor(command => command.Price)
            .GreaterThan(0);

        RuleFor(command => command.Currency)
            .NotEmpty()
            .Length(3);
    }
}
