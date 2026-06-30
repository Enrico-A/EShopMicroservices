
namespace Catalog.API.Products.CreateProduct
{
    /// <summary>
    /// Command to create a new product in the catalog.
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="Category"></param>
    /// <param name="Description"></param>
    /// <param name="ImageFile"></param>
    /// <param name="Price"></param>
    public record CreateProductCommand(string Name, List<string> Category, string Description, string ImageFile, decimal Price) 
        : ICommand<CreateProductResult>;

    /// <summary>
    /// Result of the CreateProductCommand, containing the ID of the newly created product.
    /// </summary>
    /// <param name="Id"></param>
    public record CreateProductResult(Guid Id);

    /// <summary>
    /// Handler for the CreateProductCommand, responsible for creating a new product in the catalog.
    /// </summary>
    /// <param name="repository"></param>
    internal class CreateProductHandler(CatalogRepository repository) 
        : ICommandHandler<CreateProductCommand, CreateProductResult>
    {
        /// <summary>
        /// Handles the creation of a new product based on the provided command.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            // Business logic to create a new product
            // 1 - Create product entity from command object
            var product = new Product
            {
                Name = command.Name,
                Category = command.Category,
                Description = command.Description,
                ImageFile = command.ImageFile,
                Price = command.Price
            };

            // 2 - Save to database
            var productId = await repository.CreateProductAsync(product, cancellationToken);

            // 3 - Return CreateProductResult result
            return new CreateProductResult(productId);
        }
    }
}
