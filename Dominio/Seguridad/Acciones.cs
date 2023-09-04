namespace Api.Dominio.Seguridad
{
    public class Acciones
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;

        public Modulos? Modulo { get; set; }
        public List<Roles>? Rol { get; set; }
    }
}
