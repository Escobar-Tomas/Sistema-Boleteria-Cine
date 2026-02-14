using CapaEntidad;
using System.Collections.Generic; // Necesario para List<Usuario>

namespace CapaNegocio.Interfaces
{
    public interface ICN_Usuario
    {
        Usuario Login(string nombreUsuario, string clave);

        string Registrar(Usuario obj, string claveTextoPlano);

        List<Usuario> Listar();

        string Editar(Usuario obj, string claveTextoPlano = "");

        void Eliminar(int id);
    }
}