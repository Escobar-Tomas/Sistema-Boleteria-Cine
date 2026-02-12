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
            if (string.IsNullOrWhiteSpace(obj.Nombre))
                return "El nombre de la sala es obligatorio.";

            if (obj.Filas <= 0 || obj.Columnas <= 0)
                return "Las filas y columnas deben ser mayores a 0.";

            // Cálculo automático para integridad de datos
            obj.Capacidad = obj.Filas * obj.Columnas;

            try
            {
                if (obj.Id == 0)
                {
                    _db.Salas.Add(obj);
                }
                else
                {
                    var salaDb = _db.Salas.Find(obj.Id);
                    if (salaDb != null)
                    {
                        salaDb.Nombre = obj.Nombre;
                        salaDb.Filas = obj.Filas;
                        salaDb.Columnas = obj.Columnas;
                        salaDb.Capacidad = obj.Capacidad;
                        salaDb.Estado = obj.Estado;
                    }
                }
                _db.SaveChanges();
                return "Operación exitosa";
            }
            catch (Exception ex)
            {
                return "Error al guardar: " + ex.Message;
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