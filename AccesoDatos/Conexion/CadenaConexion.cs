﻿using MySql.Data.MySqlClient;
using System.Data;

namespace AccesoDatos.Conexion
{
    public class CadenaConexion : ICadenaConexion
    {
        private readonly string connectionString;

        /// <summary>
        /// Inicializa una nueva instancia de la clase DataConnection.
        /// </summary>
        /// <param name="connectionString">Cadena de conexión utilizada para establecer la conexión a la base de datos.</param>
        public CadenaConexion(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("La cadena de conexión no puede ser nula o vacía.", nameof(connectionString));
            }
            this.connectionString = connectionString;
        }

        /// <summary>
        /// Crea y abre una conexión a la base de datos de forma asíncrona.
        /// </summary>
        /// <returns>Una tarea que representa la conexión a la base de datos.</returns>
        public async Task<IDbConnection> CrearCadenaAsync()
        {
            var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}
