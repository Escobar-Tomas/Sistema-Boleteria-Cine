using CapaDatos;
using CapaEntidad;
using CapaNegocio.Interfaces; 
using System;

namespace CapaNegocio
{
    public class CN_Sala : ICN_Sala
    {
        private readonly BDContexto _db;

        public CN_Sala(BDContexto db)
        {
            _db = db;
        }

        public List<Sala> Listar()
        {
            return _db.Salas.Where(s => s.Estado == true).ToList();
        }

        public string Guardar(Sala obj)
        {
            if (string.IsNullOrWhiteSpace(obj.Nombre)) return "El nombre es obligatorio.";
            if (obj.Capacidad <= 0) return "La capacidad debe ser mayor a 0.";

            try
            {
                if (obj.Id == 0)
                {
                    _db.Salas.Add(obj); // Nueva
                }
                else
                {
                    _db.Salas.Update(obj); // Editar
                }
                _db.SaveChanges();
                return "Sala guardada correctamente.";
            }
            catch (System.Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        public void Eliminar(int id)
        {
            var sala = _db.Salas.Find(id);
            if (sala != null)
            {
                sala.Estado = false; // Borrado lógico (mejor práctica que borrar el registro)
                _db.SaveChanges();
            }
        }
    }
}