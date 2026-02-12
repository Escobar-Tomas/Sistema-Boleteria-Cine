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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Solo usamos la configuración por defecto si NO se ha configurado ya.
            // Esto permite que las migraciones sigan funcionando si usas la consola.
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=DESKTOP-FGSHB0N\\SQLEXPRESS;Database=CineDB_Final;Integrated Security=True;TrustServerCertificate=True;");
            }
        }
    }
}