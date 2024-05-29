using Api.Dominio.Inventario;
using Api.Dominio.Seguridad;

namespace Dominio.Facturacion
{
    public class Registros
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int ResponsableId { get; set; }
        public int ClienteId { get; set; }
        public string TipoRegistro { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string Comentario { get; set; } = string.Empty;

        public Personas? Responsable { get; set; }
        public Personas? Cliente { get; set; }
        public List<RegistrosDetalles>? RegistroDetalle { get; set; }
        public Pagos? Pago { get; set; }
    }

    public class RegistrosDetalles
    {
        public int Id { get; set; }
        public int RegistroId { get; set; }
        public int ArticuloId { get; set; }
        public int Cantidad { get; set; }
        public Articulos? Articulo { get; set; }
    }
}
