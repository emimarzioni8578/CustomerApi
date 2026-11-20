using ProductCatalogApi.Entities;
using Microsoft.EntityFrameworkCore;
namespace ProductCatalogApi.DataAccess.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Producto>> GetAllProductsAsync();
        Task<Producto?> GetProductByIdAsync(int id);
        Task AddProductAsync(Producto product);
        Task UpdateProductAsync(Producto product);
        Task DeleteProductAsync(int id);
    }

    public class ProductRepository : IProductRepository
    {
        private readonly CatalogoDbContext _context;
        public ProductRepository(CatalogoDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Producto>> GetAllProductsAsync()
        {
            return await _context.Productos.ToListAsync();
        }
        public async Task<Producto?> GetProductByIdAsync(int id)
        {
            return await _context.Productos.FindAsync(id);
        }
        public async Task AddProductAsync(Producto product)
        {
            _context.Productos.Add(product);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateProductAsync(Producto product)
        {
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Productos.FindAsync(id);
            if (product != null)
            {
                _context.Productos.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}
