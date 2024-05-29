namespace Api.Dominio.Seguridad
{
    public class Usuarios
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int RolId { get; set; }
        public int PersonaId { get; set; }

        public Roles? Rol {  get; set; }
        public Personas? Persona { get; set; }
    }
}
