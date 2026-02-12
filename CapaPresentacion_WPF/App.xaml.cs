using System;
using System.IO;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using CapaDatos;
using CapaNegocio;
using CapaNegocio.Interfaces;
using CapaNegocio.Servicios;
using CapaPresentacion_WPF.ViewModels;

namespace CapaPresentacion_WPF
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public static IConfiguration Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // CONSTRUIR LA CONFIGURACIÓN (Leer appsettings.json y User Secrets)
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddUserSecrets<App>(); // ¡Aquí recuperamos los secretos de usuario!

            Configuration = builder.Build();

            // CONFIGURAR LOS SERVICIOS
            var services = new ServiceCollection();

            services.AddDbContext<BDContexto>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("CadenaSQL")));

            // Registrar Servicios Externos obteniendo la API Key desde la Configuración
            services.AddSingleton<ITmdbService>(provider =>
            {
                string apiKey = Configuration["TmdbApiKey"];
                return new TmdbService(apiKey);
            });

            // Registrar Capa de Negocio
            services.AddTransient<ICN_Pelicula, CN_Pelicula>();
            services.AddTransient<ICN_Ticket, CN_Ticket>();
            services.AddTransient<ICN_Funcion, CN_Funcion>();
            services.AddTransient<ICN_Sala, CN_Sala>();
            services.AddTransient<ICN_Usuario, CN_Usuario>();


            // D. Registrar ViewModels
            services.AddTransient<BuscadorPeliculasViewModel>();
            services.AddTransient<GestionFuncionesViewModel>();
            services.AddTransient<GestionSalasViewModel>();
            services.AddTransient<VentasViewModel>();
            services.AddTransient<GestionUsuariosViewModel>();

            // CONSTRUIR EL CONTENEDOR
            ServiceProvider = services.BuildServiceProvider();
        }
    }
}