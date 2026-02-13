using CapaEntidad;
using Microsoft.EntityFrameworkCore;

namespace CapaDatos
{
    public class BDContexto : DbContext
    {
        // 1. Constructor vacío (necesario para migraciones o diseño)
        public BDContexto() { }

        // 2. Constructor que recibe opciones (aquí le pasaremos la cadena desde afuera)
        public BDContexto(DbContextOptions<BDContexto> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Pelicula> Peliculas { get; set; }
        public DbSet<Sala> Salas { get; set; }
        public DbSet<Funcion> Funciones { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

    }
}