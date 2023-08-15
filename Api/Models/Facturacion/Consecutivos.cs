using Api.Models.General;

namespace Api.Models.Facturacion
{
    public class Consecutivos
    {
        public int Id { get; set; }
        public decimal Consecutivo { get; set; }
        public int TipoMovimientoId { get; set; }
        public string Descripcion { get; set; } = string.Empty;

        public TipoMovimiento? TipoMovimiento { get; set; }
    }
}
