﻿namespace Api.Dominio.Seguridad
{
    public class Roles
    {
        public int Id { get; set; }
        public string Rol { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int ModuloId { get; set; }

        public Modulos? Modulo { get; set; }
        public List<Acciones>? Acciones { get; set; }
    }
}
