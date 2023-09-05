using Api.Dominio.General;

namespace Api.Dominio.Inventario
{
    public class Articulos
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public decimal Stock { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal PrecioCompra { get; set; }
        public decimal PrecioVenta { get; set; }
        public int CategoriaId { get; set; }
        public int UnidadMedidaId { get; set; }
        public int EstadoId { get; set; }

        //public Categorias? Categoria { get; set; }
        //public UnidadesMedida? UnidadesMedida { get; set; }
        //public Estados? Estado { get; set; }
    }
}
