using System.Collections.Generic;
using System.Threading.Tasks; // Necesario para Task
using CapaEntidad;
using CapaNegocio.ModelosAPI;

namespace CapaNegocio.Interfaces
{
    public interface ICN_Pelicula
    {
        // Todos los métodos ahora devuelven una Tarea (Task)
        Task<List<PeliculaBusqueda>> BuscarPeliculasEnTMDBAsync(string nombre);
        Task<string> GuardarPeliculaDesdeApiAsync(int idTmdb);
        Task<List<Pelicula>> ListarAsync();
        Task EditarAsync(Pelicula pelicula);
        Task EliminarAsync(int id);
    }
}