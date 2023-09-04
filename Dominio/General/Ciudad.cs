namespace Api.Dominio.General
{
    public class Ciudad
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int DepartamentoId { get; set; }
        public int PaisId { get; set; }
        public string Codigo { get; set; } = string.Empty;

        public Departamento? Departamento { get; set; }
        public Pais? Pais { get; set; }
    }
}
