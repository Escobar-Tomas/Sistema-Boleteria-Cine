using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CapaNegocio.Interfaces;
using CapaEntidad;
using System.Collections.ObjectModel;
using System.Windows;
using System.Linq;

namespace CapaPresentacion_WPF.ViewModels
{
    public partial class GestionUsuariosViewModel : ObservableObject
    {
        private readonly ICN_Usuario _negocioUsuario;

        public ObservableCollection<Usuario> ListaUsuarios { get; set; } = new ObservableCollection<Usuario>();

        // Lista para el ComboBox de Roles
        public ObservableCollection<string> ListaRoles { get; set; } = new ObservableCollection<string> { "Administrador", "Empleado" };

        // Propiedades del formulario
        [ObservableProperty] private string nombreCompletoForm;
        [ObservableProperty] private string nombreUsuarioForm;
        [ObservableProperty] private string claveForm;
        [ObservableProperty] private string rolForm;
        [ObservableProperty] private Usuario usuarioSeleccionado;

        public GestionUsuariosViewModel(ICN_Usuario negocioUsuario)
        {
            _negocioUsuario = negocioUsuario;
            RolForm = "Empleado"; // Valor por defecto
            CargarUsuarios();
        }

        private void CargarUsuarios()
        {
            ListaUsuarios.Clear();
            var lista = _negocioUsuario.Listar();
            foreach (var u in lista) ListaUsuarios.Add(u);
        }

        // Actualizar mapeo al seleccionar un usuario de la lista
        partial void OnUsuarioSeleccionadoChanged(Usuario value)
        {
            if (value != null)
            {
                NombreCompletoForm = value.NombreCompleto;
                NombreUsuarioForm = value.NombreUsuario;
                RolForm = value.Rol;
                ClaveForm = ""; // Se deja vacía por seguridad
            }
        }

        [RelayCommand]
        public void Guardar()
        {
            string mensaje = "";

            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(NombreUsuarioForm) || string.IsNullOrWhiteSpace(NombreCompletoForm))
            {
                MessageBox.Show("El nombre de usuario y nombre completo son obligatorios.");
                return;
            }

            if (UsuarioSeleccionado == null) // NUEVO USUARIO
            {
                if (string.IsNullOrWhiteSpace(ClaveForm))
                {
                    MessageBox.Show("La clave es obligatoria para un usuario nuevo.");
                    return;
                }

                var nuevoUsuario = new Usuario
                {
                    NombreCompleto = NombreCompletoForm,
                    NombreUsuario = NombreUsuarioForm, // Usamos la propiedad nueva
                    Rol = RolForm,
                    Estado = true
                };

                // USAMOS TU MÉTODO REGISTRAR
                mensaje = _negocioUsuario.Registrar(nuevoUsuario, ClaveForm);
            }
            else // EDICIÓN DE USUARIO EXISTENTE
            {
                UsuarioSeleccionado.NombreCompleto = NombreCompletoForm;
                UsuarioSeleccionado.NombreUsuario = NombreUsuarioForm;
                UsuarioSeleccionado.Rol = RolForm;

                // USAMOS TU MÉTODO EDITAR
                // Pasamos ClaveForm. Si está vacía, tu lógica de negocio debería ignorarla.
                mensaje = _negocioUsuario.Editar(UsuarioSeleccionado, ClaveForm);
            }

            // Verificamos el mensaje de éxito (ajusta el texto según lo que retorne tu CN_Usuario)
            if (mensaje.ToLower().Contains("éxito") || mensaje.ToLower().Contains("correctamente") || mensaje.ToLower().Contains("registrado")) // Agregué "registrado" por si acaso
            {
                MessageBox.Show(mensaje, "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                LimpiarFormulario();
                CargarUsuarios();
            }
            else
            {
                MessageBox.Show(mensaje, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        public void Eliminar(Usuario usuario)
        {
            if (usuario == null) return;

            if (MessageBox.Show($"¿Dar de baja a {usuario.NombreCompleto}?", "Confirmar", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                // Asegúrate que ICN_Usuario tenga el método Eliminar definido
                _negocioUsuario.Eliminar(usuario.Id);
                CargarUsuarios();
                LimpiarFormulario();
            }
        }

        [RelayCommand]
        public void LimpiarFormulario()
        {
            UsuarioSeleccionado = null;
            NombreCompletoForm = "";
            NombreUsuarioForm = ""; // Antes: CorreoForm
            ClaveForm = "";
            RolForm = "Empleado";
        }
    }
}