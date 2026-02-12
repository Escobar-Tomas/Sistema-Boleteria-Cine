using System.Collections.Generic;
using System.Threading.Tasks;
using CapaNegocio.ModelosAPI;

namespace CapaNegocio.Servicios
{
    public interface ITmdbService
    {
        // Solo definimos las firmas de los métodos que interactúan con la API
        Task<List<PeliculaBusqueda>> BuscarPeliculasAsync(string nombre);
        Task<DetallePelicula> ObtenerDetallePeliculaAsync(int idTmdb);
    }
}