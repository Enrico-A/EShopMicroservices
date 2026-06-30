namespace Catalog.API.Products.GetProductById
{
    public record GetProductByIdQuery(Guid id) : IQuery<GetProductByIdResult>;
    public record GetProductByIdResult(Product Product);

    internal class GetProductByIdQueryHandler(CatalogRepository repository)
        : IQueryHandler<GetProductByIdQuery, GetProductByIdResult>
    {
        public async Task<GetProductByIdResult> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await repository.GetProductByIdAsync(request.id, cancellationToken);
            if (product is null)
            {
                throw new ProductNotFoundException($"Product with id {request.id} not found");
            }
            return new GetProductByIdResult(product);
        }
    }
}
