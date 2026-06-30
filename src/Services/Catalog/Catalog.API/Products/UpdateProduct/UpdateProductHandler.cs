namespace Catalog.API.Products.UpdateProduct
{
    public record UpdateProductCommand(Guid id, string Name, List<string> Category, string Description, string ImageFile, decimal Price) 
        : ICommand<UpdateProductResult>;
    public record UpdateProductResult(bool Success);

    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(command => command.id).NotEmpty().WithMessage("Product ID is required.");
            RuleFor(command => command.Name).NotEmpty().Length(2, 150).WithMessage("Product name is required.");
            RuleFor(command => command.Category).NotEmpty().WithMessage("At least one category is required.");
            RuleFor(command => command.Description).NotEmpty().WithMessage("Product description is required.");
            RuleFor(command => command.ImageFile).NotEmpty().WithMessage("Image file path is required.");
            RuleFor(command => command.Price).GreaterThan(0).WithMessage("Price must be greater than zero.");
        }
    }

    internal class UpdateProductHandler(CatalogRepository repository) 
        : ICommandHandler<UpdateProductCommand, UpdateProductResult>
    {
        public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Id = command.id,
                Name = command.Name,
                Category = command.Category,
                Description = command.Description,
                ImageFile = command.ImageFile,
                Price = command.Price
            };

            var isUpdated = await repository.UpdateProductAsync(product, cancellationToken);
            if(!isUpdated) {
                throw new ProductNotFoundException($"Product with id {command.id} not found");
            }
            return new UpdateProductResult(true);
        }
    }
}
