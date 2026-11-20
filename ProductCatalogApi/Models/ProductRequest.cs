namespace ProductCatalogApi.Models
{
    public class ProductRequest
    {
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public int CategoriaId { get; set; }
    }
}
