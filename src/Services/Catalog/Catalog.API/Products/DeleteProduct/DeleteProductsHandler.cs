namespace Catalog.API.Products.DeleteProduct
{
    public record DeleteProductCommand(Guid id) : ICommand<DeleteProductResult>;
    public record DeleteProductResult(bool Success);

    public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
    {
        public DeleteProductCommandValidator()
        {
            RuleFor(command => command.id).NotEmpty().WithMessage("Product ID is required.");
        }
    }

    internal class DeleteProductsHandler(CatalogRepository repository) 
        : ICommandHandler<DeleteProductCommand, DeleteProductResult>
    {
        public async Task<DeleteProductResult> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
        {
            await repository.DeleteProductAsync(command.id, cancellationToken);
            return new DeleteProductResult(true);
        }
    }
}
