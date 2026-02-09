using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    internal class Pelicula
    {
        private int IdPelicula { get; set; }
        private string Titulo { get; set; }
        private string Genero { get; set; }
        private int Duracion { get; set; } // Duracion en minutos
        private string Director { get; set; }
        private string Clasificacion { get; set; }
        private DateTime FechaEstreno { get; set; }
        private string Descripcion { get; set; }

        public Pelicula(int _IdPelicula, string _Titulo, string _Genero, int _Duracion, string _Director, string _Clasificacion, DateTime _FechaEstreno, string _Descripcion)
        {
            IdPelicula = _IdPelicula;
            Titulo = _Titulo;
            Genero = _Genero;
            Duracion = _Duracion;
            Director = _Director;
            Clasificacion = _Clasificacion;
            FechaEstreno = _FechaEstreno;
            Descripcion = _Descripcion;
        }
    }
}
