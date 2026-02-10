using System.Linq;
using CapaDatos;
using CapaEntidad;
using Microsoft.EntityFrameworkCore; // Necesario para .FirstOrDefault

namespace CapaNegocio
{
    public class CN_Usuario
    {
        private readonly BDContexto _db;

        // CAMBIO: Constructor recibe la cadena
        public CN_Usuario(string cadenaConexion)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BDContexto>();
            optionsBuilder.UseSqlServer(cadenaConexion);

            _db = new BDContexto(optionsBuilder.Options);
        }

        public Usuario Login(string correo, string clave)
        {
            // 1. Encriptamos la clave que escribió el usuario para compararla
            string claveEncriptada = CN_Recursos.ConvertirSha256(clave);

            // 2. Buscamos en la BD
            return _db.Usuarios
                .FirstOrDefault(u => u.Correo == correo &&
                                     u.Clave == claveEncriptada &&
                                     u.Estado == true);
        }

        // Método auxiliar para crear el primer usuario (Admin)
        public string Registrar(Usuario obj, string claveTextoPlano)
        {
            try
            {
                obj.Clave = CN_Recursos.ConvertirSha256(claveTextoPlano);
                _db.Usuarios.Add(obj);
                _db.SaveChanges();
                return "Usuario registrado";
            }
            catch (System.Exception ex) { return ex.Message; }
        }
    }
}