using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoDatos.Repositorio
{
    public interface IRepositorio<T>
    {
        /// <summary>
        /// Ejecuta una consulta SQL de forma asincrónica y devuelve el resultado como una colección de tipo T.
        /// </summary>
        /// <typeparam name="T">El tipo de datos esperado en el resultado.</typeparam>
        /// <param name="sql">La consulta SQL que se ejecutará.</param>
        /// <param name="parameters">Los parámetros opcionales que se pueden pasar a la consulta.</param>
        /// <returns>Una tarea que representa la operación asincrónica y que devuelve una colección de tipo T que contiene el resultado de la consulta.</returns>
        Task<IEnumerable<T>> ExecuteQueryAsync<T>(string sql, object? parameters = null);

        /// <summary>
        /// Ejecuta una consulta SQL de forma asincrónica y devuelve un único valor escalar de tipo T.
        /// </summary>
        /// <typeparam name="T">El tipo de datos esperado para el valor escalar.</typeparam>
        /// <param name="sql">La consulta SQL que se ejecutará.</param>
        /// <param name="parameters">Los parámetros opcionales que se pueden pasar a la consulta.</param>
        /// <returns>Una tarea que representa la operación asincrónica y que devuelve un valor escalar de tipo T que contiene el resultado de la consulta.</returns>
        Task<T> ExecuteScalarAsync<T>(string sql, object? parameters = null);

        /// <summary>
        /// Ejecuta un comando SQL de forma asincrónica (por ejemplo, INSERT, UPDATE, DELETE) y devuelve el número de filas afectadas.
        /// </summary>
        /// <param name="sql">El comando SQL que se ejecutará.</param>
        /// <param name="parameters">Los parámetros opcionales que se pueden pasar al comando.</param>
        /// <returns>Una tarea que representa la operación asincrónica y que devuelve el número de filas afectadas por el comando SQL.</returns>
        Task<int> ExecuteAsync(string sql, object? parameters = null);

        /// <summary>
        /// Ejecuta un procedimiento almacenado SQL de forma asincrónica y devuelve el resultado como una colección de tipo T.
        /// </summary>
        /// <typeparam name="T">El tipo de datos esperado en el resultado.</typeparam>
        /// <param name="storedProcedureName">El nombre o código del procedimiento almacenado SQL que se ejecutará.</param>
        /// <param name="parameters">Los parámetros opcionales que se pueden pasar al procedimiento almacenado.</param>
        /// <returns>Una tarea que representa la operación asincrónica y que devuelve una colección de tipo T que contiene el resultado del procedimiento almacenado.</returns>
        Task<IEnumerable<T>> ExecuteStoredProcedureAsync<T>(string storedProcedureName, object parameters = null);
    }
}
