namespace ProductCatalogApi.Models
{
    public class ProductResponse
    {
        public int ProductoId { get; set; }

        public int CategoriaId { get; set; }

        public string Codigo { get; set; } = null!;

        public string Nombre { get; set; } = null!;

        public decimal Precio { get; set; }

        public int Stock { get; set; }

        public bool Activo { get; set; }

        public DateTime FechaAlta { get; set; }
    }
}
