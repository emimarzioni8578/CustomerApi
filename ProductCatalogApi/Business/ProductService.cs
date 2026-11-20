using ProductCatalogApi.Entities;
using ProductCatalogApi.DataAccess.Repositories;
using FluentValidation;
using ProductCatalogApi.Business.Results;

namespace ProductCatalogApi.Business
{
    public interface IProductService
    {
        Task<IEnumerable<Producto>> GetAllProductsAsync();
        Task<Result<Producto>> GetProductByIdAsync(int id);
        Task<Result<Producto>> AddProductAsync(Producto productRequest);
        Task<Result<Producto>> UpdateProductAsync(int id, Producto productRequest);
        Task<Result<Producto>> DeleteProductAsync(int id);
    }
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IValidator<Producto> _productValidator;

        public ProductService(IProductRepository productRepository, IValidator<Producto> productValidator)
        {
            _productRepository = productRepository;
            _productValidator = productValidator;
        }
        public async Task<IEnumerable<Producto>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllProductsAsync();
        }
        public async Task<Result<Producto>> GetProductByIdAsync(int id)
        {
            var producto = await _productRepository.GetProductByIdAsync(id);

            if(producto is null)
            {
                return Result<Producto>.Failure(Error.NotFound($"Product with ID {id} not found."));
            }

            return Result<Producto>.Success(producto);
        }
        public async Task<Result<Producto>> AddProductAsync(Producto productRequest)
        {
            var validationResult = await _productValidator.ValidateAsync(productRequest);
            if (!validationResult.IsValid)
            {
                return Result<Producto>.Failure(Error.Validation("Validation failed"));
            }
            await _productRepository.AddProductAsync(productRequest);
            return Result<Producto>.Success(productRequest);
        }

        public async Task<Result<Producto>> UpdateProductAsync(int id, Producto productRequest)
        {
            var existingProduct = await _productRepository.GetProductByIdAsync(id);
            if (existingProduct is null)
            {
                return Result<Producto>.Failure(Error.NotFound($"Product with ID {id} not found."));
            }

            var validationResult = await _productValidator.ValidateAsync(productRequest);

            if (!validationResult.IsValid)
            {
                return Result<Producto>.Failure(Error.Validation("Validation failed"));
            }

            existingProduct.Codigo = productRequest.Codigo;
            existingProduct.Nombre = productRequest.Nombre;
            existingProduct.Precio = productRequest.Precio;
            existingProduct.Stock = productRequest.Stock;
            existingProduct.CategoriaId = productRequest.CategoriaId;
            await _productRepository.UpdateProductAsync(existingProduct);
            return Result<Producto>.Success(existingProduct);
        }
        public async Task<Result<Producto>> DeleteProductAsync(int id)
        {
            var existingProduct = await _productRepository.GetProductByIdAsync(id);
            if (existingProduct is null)
            {
                return Result<Producto>.Failure(Error.NotFound($"Product with ID {id} not found."));
            }
            await _productRepository.DeleteProductAsync(id);
            return Result<Producto>.Success(existingProduct);
        }
    }
}
