﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoDatos.Conexion
{
    public interface ICadenaConexion
    {
        /// <summary>
        /// Crea y devuelve una conexión a la base de datos de forma asíncrona.
        /// </summary>
        /// <returns>Una tarea que representa la conexión a la base de datos.</returns>
        Task<IDbConnection> CrearCadenaAsync();
    }
}
