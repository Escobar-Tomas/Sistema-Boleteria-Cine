using System.Windows;
using Microsoft.Extensions.Configuration; // Necesitas instalar este paquete

namespace CapaPresentacion_WPF
{
    public partial class App : Application
    {
        // Propiedad estática para acceder a la cadena desde toda la app
        public static string CadenaConexion { get; private set; }
        public static string TmdbApiKey { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 1. Construir la configuración
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<App>(); // Busca secretos asociados a esta asamblea

            var configuration = builder.Build();

            // 2. Leer y guardar las configuraciones
            CadenaConexion = configuration.GetConnectionString("CadenaSQL");
            TmdbApiKey = configuration["TmdbApiKey"];

            // Validación básica
            if (string.IsNullOrEmpty(CadenaConexion))
            {
                MessageBox.Show("No se encontró la cadena de conexión en los secretos de usuario.");
                Shutdown();
            }
        }
    }
}