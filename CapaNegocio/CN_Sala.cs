using CapaDatos;
using CapaEntidad;
using CapaNegocio.Interfaces;
using Microsoft.EntityFrameworkCore;
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
            // 1. Validaciones básicas de entrada
            if (obj.Filas <= 0 || obj.Columnas <= 0)
            {
                return "Las filas y columnas deben ser mayores a 0.";
            }

            try
            {
                // =======================================================
                // CASO 1: CREAR NUEVA SALA (Id == 0)
                // =======================================================
                if (obj.Id == 0)
                {
                    // Validar nombre duplicado
                    if (_db.Salas.Any(s => s.Nombre == obj.Nombre))
                    {
                        return "Ya existe una sala con ese nombre.";
                    }

                    // Calcular capacidad automáticamente
                    obj.Capacidad = obj.Filas * obj.Columnas;

                    _db.Salas.Add(obj);
                    _db.SaveChanges();

                    return "Creación exitosa"; // Mensaje que espera tu ViewModel
                }

                // =======================================================
                // CASO 2: EDITAR SALA EXISTENTE (Id != 0)
                // =======================================================
                else
                {
                    // Buscamos la sala y sus relaciones (Funciones)
                    var salaDb = _db.Salas
                                    .Include(s => s.Funciones) // <--- CLAVE: Traer historial
                                    .FirstOrDefault(s => s.Id == obj.Id);

                    if (salaDb == null)
                    {
                        return "La sala no existe.";
                    }

                    // Validar nombre duplicado (excluyendo la actual)
                    if (_db.Salas.Any(s => s.Nombre == obj.Nombre && s.Id != obj.Id))
                    {
                        return "Ya existe otra sala con ese nombre.";
                    }

                    // ---------------------------------------------------
                    // VALIDACIÓN DE INTEGRIDAD (Corrección solicitada)
                    // ---------------------------------------------------

                    // Detectamos si cambió el tamaño físico
                    bool cambioDimensiones = salaDb.Filas != obj.Filas ||
                                             salaDb.Columnas != obj.Columnas;

                    // Detectamos si tiene funciones/historial
                    bool tieneUso = salaDb.Funciones != null &&
                                    salaDb.Funciones.Any();

                    if (cambioDimensiones && tieneUso)
                    {
                        // Retornamos el error como string, tal cual pide la interfaz
                        return "Error: No se puede modificar el tamaño de la sala porque ya tiene funciones asociadas.";
                    }
                    // ---------------------------------------------------

                    // Si pasa la validación, actualizamos los datos
                    salaDb.Nombre = obj.Nombre;
                    salaDb.Filas = obj.Filas;
                    salaDb.Columnas = obj.Columnas;
                    salaDb.Capacidad = obj.Filas * obj.Columnas; // Recalcular
                    salaDb.Estado = obj.Estado; // Tu propiedad es 'Estado', no 'Disponible'

                    _db.Update(salaDb);
                    _db.SaveChanges();

                    return "Modificación exitosa"; // Mensaje que espera tu ViewModel
                }
            }
            catch (Exception ex)
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