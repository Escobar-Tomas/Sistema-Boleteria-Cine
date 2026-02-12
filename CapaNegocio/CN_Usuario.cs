using System.Linq;
using CapaDatos;
using CapaEntidad;
using CapaNegocio.Interfaces;

namespace CapaNegocio
{
    public class CN_Usuario : ICN_Usuario
    {
        private readonly BDContexto _db;

        // Inyectamos el contexto de base de datos
        public CN_Usuario(BDContexto db)
        {
            _db = db;
        }

        public Usuario Login(string correo, string clave)
        {
            string claveEncriptada = CN_Recursos.ConvertirSha256(clave);

            return _db.Usuarios
                .FirstOrDefault(u => u.Correo == correo &&
                                     u.Clave == claveEncriptada &&
                                     u.Estado == true);
        }

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

        public List<Usuario> Listar()
        {
            // Traemos todos los usuarios
            return _db.Usuarios.ToList();
        }

        public string Editar(Usuario obj, string claveTextoPlano = "")
        {
            try
            {
                var usuarioDb = _db.Usuarios.Find(obj.Id);
                if (usuarioDb == null) return "Usuario no encontrado.";

                // Actualizamos todos los datos básicos
                usuarioDb.NombreCompleto = obj.NombreCompleto;
                usuarioDb.Correo = obj.Correo;
                usuarioDb.Rol = obj.Rol;
                usuarioDb.Estado = obj.Estado;

                // Solo actualizamos la clave si el administrador escribió una nueva
                if (!string.IsNullOrWhiteSpace(claveTextoPlano))
                {
                    usuarioDb.Clave = CN_Recursos.ConvertirSha256(claveTextoPlano);
                }

                _db.SaveChanges();
                return "Usuario actualizado correctamente.";
            }
            catch (System.Exception ex) { return "Error: " + ex.Message; }
        }

        public void Eliminar(int id)
        {
            var usuarioDb = _db.Usuarios.Find(id);
            if (usuarioDb != null)
            {
                // En usuarios es mejor hacer un "Borrado Lógico" (darlo de baja)
                // para no perder el historial de qué tickets vendió ese usuario.
                usuarioDb.Estado = false;
                _db.SaveChanges();
            }
        }
    }
}