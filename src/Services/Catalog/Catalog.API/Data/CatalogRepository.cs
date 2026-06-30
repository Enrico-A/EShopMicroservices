using Catalog.API.Models;
using NpgsqlTypes;

namespace Catalog.API.Data
{
    public class CatalogRepository
    {
        private readonly NpgsqlDataSource _dataSource;

        public CatalogRepository(NpgsqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        /// <summary>
        /// Gets a paginated list of products asynchronously.
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IReadOnlyList<Product>> GetProductsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            const string sql = "" +
                "SELECT id, name, category, description, imagefile, price " +
                "FROM products " +
                "ORDER BY name " +
                "OFFSET @offset " +
                "LIMIT @limit";
            await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("offset", (pageNumber - 1) * pageSize);
            command.Parameters.AddWithValue("limit", pageSize);
            return await ReadProductsAsync(command, cancellationToken);
        }

        /// <summary>
        /// Gets a product by its ID asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Product?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            const string sql = "" +
                "SELECT id, name, category, description, imagefile, price " +
                "FROM products " +
                "WHERE id = @id;";

            await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("id", id);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            return await reader.ReadAsync(cancellationToken) ? MapProduct(reader) : null;
        }

        /// <summary>
        /// Get products by category asynchronously.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IReadOnlyList<Product>> GetProductsByCategoryAsync(string category, CancellationToken cancellationToken)
        {
            const string sql = "" +
                "SELECT id, name, category, description, imagefile, price " +
                "FROM products " +
                "WHERE @category = ANY(category) " +
                "ORDER BY name;";

            await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("category", category);

            return await ReadProductsAsync(command, cancellationToken);
        }

        /// <summary>
        /// Creates a new product asynchronously and returns the generated product ID.
        /// </summary>
        /// <param name="product"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Guid> CreateProductAsync(Product product, CancellationToken cancellationToken)
        {
            const string sql = "" +
                "INSERT INTO products (id, name, category, description, imagefile, price)" +
                "VALUES (@id, @name, @category, @description, @imageFile, @price);";

            product.Id = product.Id == Guid.Empty ? Guid.NewGuid() : product.Id;

            await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
            await using var command = new NpgsqlCommand(sql, connection);
            AddProductParameters(command, product);

            await command.ExecuteNonQueryAsync(cancellationToken);

            return product.Id;
        }

        /// <summary>
        /// Updates an existing product asynchronously and returns a boolean indicating whether the update was successful.
        /// </summary>
        /// <param name="product"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> UpdateProductAsync(Product product, CancellationToken cancellationToken)
        {
            const string sql = "" +
                "UPDATE products " +
                "SET name = @name, category = @category, description = @description, imagefile = @imageFile, price = @price " +
                "WHERE id = @id;";

            await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
            await using var command = new NpgsqlCommand(sql, connection);
            AddProductParameters(command, product);

            var affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);
            return affectedRows > 0;
        }

        /// <summary>
        /// Deletes a product by its ID asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task DeleteProductAsync(Guid id, CancellationToken cancellationToken)
        {
            const string sql = "" +
                "DELETE FROM products " +
                "WHERE id = @id;";

            await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("id", id);

            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        /// <summary>
        /// Reads products from the database using the provided command and returns a list of Product objects.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<IReadOnlyList<Product>> ReadProductsAsync(NpgsqlCommand command, CancellationToken cancellationToken)
        {
            var products = new List<Product>();

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                products.Add(MapProduct(reader));
            }

            return products;
        }

        /// <summary>
        /// Maps a data reader to a Product object.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static Product MapProduct(NpgsqlDataReader reader)
        {
            return new Product
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Category = reader.GetFieldValue<string[]>(reader.GetOrdinal("category")).ToList(),
                Description = reader.GetString(reader.GetOrdinal("description")),
                ImageFile = reader.GetString(reader.GetOrdinal("imagefile")),
                Price = reader.GetDecimal(reader.GetOrdinal("price"))
            };
        }

        /// <summary>
        /// Adds parameters for a Product object to the provided NpgsqlCommand.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="product"></param>
        private static void AddProductParameters(NpgsqlCommand command, Product product)
        {
            command.Parameters.AddWithValue("id", product.Id);
            command.Parameters.AddWithValue("name", product.Name);
            command.Parameters.Add("category", NpgsqlDbType.Array | NpgsqlDbType.Text).Value = product.Category.ToArray();
            command.Parameters.AddWithValue("description", product.Description);
            command.Parameters.AddWithValue("imageFile", product.ImageFile);
            command.Parameters.AddWithValue("price", product.Price);
        }
    }
}
