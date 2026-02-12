using System.Collections.Generic;
using CapaEntidad;

namespace CapaNegocio.Interfaces
{
    public interface ICN_Funcion
    {
        List<Funcion> Listar();
        string Guardar(Funcion obj);
        void Eliminar(int id);
    }
}