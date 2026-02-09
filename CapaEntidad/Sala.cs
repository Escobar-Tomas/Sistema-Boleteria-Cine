using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    internal class Sala
    {
        private int IdSala { get; set; }
        private string Nombre { get; set; }
        private int Capacidad { get; set; }

        public Sala(int _IdSala, string _Nombre, int _Capacidad)
        {
            IdSala = _IdSala;
            Nombre = _Nombre;
            Capacidad = _Capacidad;
        }
    }
}
