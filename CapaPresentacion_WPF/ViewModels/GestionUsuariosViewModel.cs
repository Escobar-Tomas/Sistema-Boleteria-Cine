using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CapaNegocio.Interfaces;
using CapaEntidad;
using System.Collections.ObjectModel;
using System.Windows;

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
        [ObservableProperty] private string correoForm;
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

        partial void OnUsuarioSeleccionadoChanged(Usuario value)
        {
            if (value != null)
            {
                NombreCompletoForm = value.NombreCompleto;
                CorreoForm = value.Correo;
                RolForm = value.Rol;
                ClaveForm = ""; // Se deja vacía por seguridad
            }
        }

        [RelayCommand]
        public void Guardar()
        {
            string mensaje = "";

            if (UsuarioSeleccionado == null) // NUEVO
            {
                if (string.IsNullOrWhiteSpace(ClaveForm))
                {
                    MessageBox.Show("La clave es obligatoria para un usuario nuevo.");
                    return;
                }

                var nuevoUsuario = new Usuario
                {
                    NombreCompleto = NombreCompletoForm,
                    Correo = CorreoForm,
                    Rol = RolForm,
                    Estado = true
                };
                mensaje = _negocioUsuario.Registrar(nuevoUsuario, ClaveForm);
            }
            else // EDICIÓN
            {
                UsuarioSeleccionado.NombreCompleto = NombreCompletoForm;
                UsuarioSeleccionado.Correo = CorreoForm;
                UsuarioSeleccionado.Rol = RolForm;

                mensaje = _negocioUsuario.Editar(UsuarioSeleccionado, ClaveForm);
            }

            MessageBox.Show(mensaje);

            if (mensaje.Contains("correctamente") || mensaje.Contains("registrado"))
            {
                LimpiarFormulario();
                CargarUsuarios();
            }
        }

        [RelayCommand]
        public void Eliminar(Usuario usuario)
        {
            if (usuario == null) return;

            if (MessageBox.Show($"¿Dar de baja a {usuario.NombreCompleto}?", "Confirmar", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
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
            CorreoForm = "";
            ClaveForm = "";
            RolForm = "Empleado";
        }
    }
}