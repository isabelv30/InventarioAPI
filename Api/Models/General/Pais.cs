namespace Api.Models.General
{
    public class Pais
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string CodigoIso2 { get; set; } = string.Empty;
        public string CodigoIso3 { get; set; } = string.Empty;
    }
}
