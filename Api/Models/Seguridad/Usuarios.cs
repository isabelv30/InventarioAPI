namespace Api.Models.Seguridad
{
    public class Usuarios
    {
        public int Id { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public int PersonaId { get; set; }
        public bool CambioClave { get; set; }

        public Personas? Persona { get; set; }
    }
}
