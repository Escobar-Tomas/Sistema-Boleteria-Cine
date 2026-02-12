using CapaEntidad;

namespace CapaNegocio.Interfaces
{
    public interface ICN_Usuario
    {
        Usuario Login(string correo, string clave);
        string Registrar(Usuario obj, string claveTextoPlano);
        List<Usuario> Listar();
        string Editar(Usuario obj, string claveTextoPlano = "");
        void Eliminar(int id);
    }
}