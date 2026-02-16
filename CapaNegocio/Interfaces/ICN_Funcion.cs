using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CapaNegocio.Interfaces
{
    public interface ICN_Funcion
    {
        // Obtiene todas las funciones de una fecha específica (Ideal para la grilla diaria)
        Task<List<Funcion>> ListarPorFechaAsync(DateTime fecha);

        // Intenta crear la función. Retorna true/false y el mensaje explicativo.
        Task<(bool Exito, string Mensaje)> CrearFuncionAsync(Funcion funcion);

        // Baja lógica (cambiar estado a false)
        Task<bool> EliminarFuncionAsync(int id);

        // Mantenemos el listar general por si acaso, pero asíncrono
        Task<List<Funcion>> ListarTodoAsync();
    }
}