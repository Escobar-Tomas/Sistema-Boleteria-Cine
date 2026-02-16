using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CapaNegocio.Interfaces;
using CapaEntidad;
using CapaNegocio.ModelosAPI;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System;

namespace CapaPresentacion_WPF.ViewModels
{
    public partial class BuscadorPeliculasViewModel : ObservableObject
    {
        private readonly ICN_Pelicula _negocioPelicula;

        // Colecciones
        public ObservableCollection<PeliculaBusqueda> ResultadosBusqueda { get; set; } = new ObservableCollection<PeliculaBusqueda>();
        public ObservableCollection<Pelicula> PeliculasLocales { get; set; } = new ObservableCollection<Pelicula>();

        // Propiedades para Búsqueda
        [ObservableProperty] private string textoBusqueda;
        [ObservableProperty] private bool estaCargando;

        // Propiedad para Edición (La película seleccionada en la lista local)
        [ObservableProperty] private Pelicula peliculaSeleccionada;

        public BuscadorPeliculasViewModel(ICN_Pelicula negocioPelicula)
        {
            _negocioPelicula = negocioPelicula;
        }

        // --- COMANDOS ---

        [RelayCommand]
        public async Task CargarInicio()
        {
            await CargarPeliculasLocales();
        }

        private async Task CargarPeliculasLocales()
        {
            try
            {
                EstaCargando = true;
                var lista = await _negocioPelicula.ListarAsync();
                PeliculasLocales.Clear();
                foreach (var p in lista) PeliculasLocales.Add(p);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar: {ex.Message}");
            }
            finally { EstaCargando = false; }
        }

        // Búsqueda en API (Tu código existente)
        [RelayCommand]
        public async Task Buscar()
        {
            if (string.IsNullOrWhiteSpace(TextoBusqueda)) return;
            EstaCargando = true;
            try
            {
                var resultados = await _negocioPelicula.BuscarPeliculasEnTMDBAsync(TextoBusqueda);
                ResultadosBusqueda.Clear();
                foreach (var r in resultados) ResultadosBusqueda.Add(r);
                if (resultados.Count == 0) MessageBox.Show("No se encontraron resultados.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error TMDB: {ex.Message}");
            }
            finally { EstaCargando = false; }
        }

        // Importar desde API (Tu código existente)
        [RelayCommand]
        public async Task AgregarPelicula(PeliculaBusqueda peliculaApi)
        {
            if (peliculaApi == null) return;
            EstaCargando = true;
            try
            {
                string msj = await _negocioPelicula.GuardarPeliculaDesdeApiAsync(peliculaApi.Id);
                MessageBox.Show(msj);
                await CargarPeliculasLocales(); // Refrescar lista local
            }
            catch (Exception ex) { MessageBox.Show($"Error CRÍTICO:\n{ex.Message}\n\nDetalle:{ex.InnerException?.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
            finally { EstaCargando = false; }
        }

        // --- NUEVOS COMANDOS DE GESTIÓN ---

        [RelayCommand]
        public async Task GuardarEdicion()
        {
            if (PeliculaSeleccionada == null) return;

            try
            {
                await _negocioPelicula.EditarAsync(PeliculaSeleccionada);
                MessageBox.Show("Película actualizada correctamente.");
                await CargarPeliculasLocales();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al editar: {ex.Message}");
            }
        }

        [RelayCommand]
        public async Task EliminarPelicula(Pelicula pelicula)
        {
            if (pelicula == null) return;

            var result = MessageBox.Show($"¿Estás seguro de eliminar '{pelicula.Titulo}'?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;

            try
            {
                await _negocioPelicula.EliminarAsync(pelicula.Id); // Asumiendo que Id es la PK
                await CargarPeliculasLocales();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar: {ex.Message}");
            }
        }


        [RelayCommand]
        public void SeleccionarPelicula(Pelicula pelicula)
        {
            if (pelicula == null) return;

            // Al asignar esto, gracias al Binding del Panel Lateral, 
            // los campos de texto se llenarán automáticamente.
            PeliculaSeleccionada = pelicula;
        }
    }
}