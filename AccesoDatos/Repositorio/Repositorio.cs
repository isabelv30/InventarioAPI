using AccesoDatos.Conexion;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoDatos.Repositorio
{
    public class Repositorio<T> : IRepositorio<T> where T : class
    {
        private readonly ICadenaConexion _connectionString;

        public Repositorio(ICadenaConexion connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Ejecuta una consulta SQL que devuelve un conjunto de resultados y los mapea a una colección de objetos de tipo T.
        /// </summary>
        /// <typeparam name="T">El tipo de objeto al que se deben mapear los resultados de la consulta.</typeparam>
        /// <param name="sql">La consulta SQL que se va a ejecutar.</param>
        /// <param name="parameters">Los parámetros de la consulta SQL (opcional).</param>
        /// <returns>Una colección de objetos de tipo T que representan los resultados de la consulta.</returns>
        public async Task<IEnumerable<T>> ExecuteQueryAsync<T>(string sql, object? parameters = null)
        {
            // Establece una conexión a la base de datos utilizando la cadena de conexión proporcionada.
            using (var connection = await _connectionString.CrearCadenaAsync())
            {
                // Ejecuta la consulta SQL de forma asincrónica y mapea los resultados a objetos de tipo T.
                return await connection.QueryAsync<T>(sql, parameters);
            }
        }

        /// <summary>
        /// Ejecuta una consulta SQL que devuelve un valor escalar y lo devuelve de manera asincrónica.
        /// </summary>
        /// <typeparam name="T">El tipo de valor escalar que se espera como resultado.</typeparam>
        /// <param name="sql">La consulta SQL que se va a ejecutar.</param>
        /// <param name="parameters">Los parámetros de la consulta SQL (opcional).</param>
        /// <returns>El valor escalar resultante de la consulta.</returns>
        public async Task<T> ExecuteScalarAsync<T>(string sql, object? parameters = null)
        {
            // Establece una conexión a la base de datos utilizando la cadena de conexión proporcionada.
            using (var connection = await _connectionString.CrearCadenaAsync())
            {
                // Ejecuta la consulta SQL de forma asincrónica y devuelve un valor escalar del tipo especificado.
                return await connection.ExecuteScalarAsync<T>(sql, parameters);
            }
        }

        /// <summary>
        /// Ejecuta una consulta SQL que no devuelve un conjunto de resultados y devuelve el número de filas afectadas de manera asincrónica.
        /// </summary>
        /// <param name="sql">La consulta SQL que se va a ejecutar.</param>
        /// <param name="parameters">Los parámetros de la consulta SQL (opcional).</param>
        /// <returns>El número de filas afectadas por la consulta.</returns>
        public async Task<int> ExecuteAsync(string sql, object? parameters = null)
        {
            // Establece una conexión a la base de datos utilizando la cadena de conexión proporcionada.
            using (var connection = await _connectionString.CrearCadenaAsync())
            {
                // Ejecuta la consulta SQL de forma asincrónica y devuelve el número de filas afectadas.
                return await connection.ExecuteAsync(sql, parameters);
            }
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado.
        /// </summary>
        /// <typeparam name="T">El tipo de objeto al que se deben mapear los resultados de la consulta.</typeparam>
        /// <param name="storedProcedureName">El nombre del procedimiento almacenado.</param>
        /// <param name="parameters">Los parámetros de la consulta SQL (opcional).</param>
        /// <returns>Una colección de objetos de tipo T que representan los resultados de la consulta.</returns>
        public async Task<IEnumerable<T>> ExecuteStoredProcedureAsync<T>(string storedProcedureName, object parameters = null)
        {
            // Establece una conexión a la base de datos utilizando la cadena de conexión proporcionada.
            using (var connection = await _connectionString.CrearCadenaAsync())
            {

                // Ejecuta el procedimiento almacenado utilizando Dapper.
                var result = await connection.QueryAsync<T>(
                    storedProcedureName,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result;
            }
        }
    }
}
