using CapaEntidad;
using CapaNegocio.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace CapaPresentacion_WPF.ViewModels
{
    public partial class GestionSalasViewModel : ObservableObject
    {
        private readonly ICN_Sala _servicioSala;

        // 1. DECLARACIÓN DE PROPIEDADES
        // El atributo [ObservableProperty] generará automáticamente la propiedad "ListaSalas"
        [ObservableProperty]
        private ObservableCollection<Sala> _listaSalas;

        [ObservableProperty]
        private Sala _salaSeleccionada;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CapacidadCalculada))]
        private int _filas;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CapacidadCalculada))]
        private int _columnas;

        // Propiedad calculada para la interfaz
        public int CapacidadCalculada => Filas * Columnas;

        // 2. CONSTRUCTOR
        public GestionSalasViewModel(ICN_Sala servicioSala)
        {
            _servicioSala = servicioSala;

            // Inicializamos la colección para que no sea nula antes de cargar
            ListaSalas = new ObservableCollection<Sala>();

            CargarSalas();
            Limpiar();
        }

        // 3. MÉTODOS DE LÓGICA
        private void CargarSalas()
        {
            try
            {
                // Obtenemos la lista desde la capa de negocio
                var lista = _servicioSala.Listar();
                ListaSalas = new ObservableCollection<Sala>(lista);
            }
            catch (Exception)
            {
                // Manejo de error silencioso o log
            }
        }

        // Este método se dispara automáticamente gracias a CommunityToolkit cuando cambia la selección
        partial void OnSalaSeleccionadaChanged(Sala value)
        {
            if (value != null)
            {
                Filas = value.Filas;
                Columnas = value.Columnas;
            }
            else
            {
                Filas = 0;
                Columnas = 0;
            }
        }

        // 4. COMANDOS
        [RelayCommand]
        private void Guardar()
        {
            if (SalaSeleccionada == null) return;

            // Sincronizamos los valores del formulario al objeto
            SalaSeleccionada.Filas = Filas;
            SalaSeleccionada.Columnas = Columnas;
            SalaSeleccionada.Capacidad = CapacidadCalculada;

            string mensaje = _servicioSala.Guardar(SalaSeleccionada);

            if (mensaje.Contains("Éxito") || mensaje.Contains("exitosa"))
            {
                CargarSalas();
                Limpiar();
            }
            else
            {
                // Aquí atrapamos los errores como "Ya existe una sala..." o validaciones de tamaño
                MessageBox.Show(mensaje, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Limpiar()
        {
            SalaSeleccionada = new Sala
            {
                Nombre = string.Empty,
                Estado = true,
                Filas = 0,
                Columnas = 0,
                Capacidad = 0
            };
            Filas = 0;
            Columnas = 0;
        }
    }
}