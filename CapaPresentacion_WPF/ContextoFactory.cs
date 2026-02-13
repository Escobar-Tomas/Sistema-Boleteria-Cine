using CapaDatos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace CapaPresentacion_WPF
{
    // IDesignTimeDbContextFactory le dice a EF Core: "Si no sabes cómo crear el contexto, usa esta clase"
    public class ContextoFactory : IDesignTimeDbContextFactory<BDContexto>
    {
        public BDContexto CreateDbContext(string[] args)
        {
            // Construimos la configuración EXACTAMENTE igual que en App.xaml.cs
            // Esto permite leer appsettings.json y, lo más importante, los User Secrets.
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddUserSecrets<App>() // <--- AQUÍ está la clave: Leemos los secretos asociados a la clase App
                .Build();

            // Recuperamos la cadena de conexión segura
            var connectionString = configuration.GetConnectionString("CadenaSQL");

            // Configuramos las opciones del contexto para usar SQL Server con esa cadena
            var optionsBuilder = new DbContextOptionsBuilder<BDContexto>();
            optionsBuilder.UseSqlServer(connectionString);

            // Retornamos el contexto listo para que EF Core genere las migraciones
            return new BDContexto(optionsBuilder.Options);
        }
    }
}