namespace Dominio.Facturacion
{
    public class Pagos
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Monto { get; set; }
        public int RegistroId { get; set; }
        public string MedioPagoId { get; set; } = string.Empty;
    }
}
