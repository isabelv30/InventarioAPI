namespace Api.Dominio.Inventario
{
    public class Articulos
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public decimal Stock { get; set; }
        public int CategoriaId { get; set; }
        public ArticulosCategoria? ArticuloCategoria { get; set; }

    }
}
