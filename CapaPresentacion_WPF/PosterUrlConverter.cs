using System;
using System.Globalization;
using System.Windows.Data;

namespace CapaPresentacion_WPF.Convertidores
{
    // IValueConverter es la herramienta de WPF para transformar datos justo antes de mostrarlos
    public class PosterUrlConverter : IValueConverter
    {
        private const string BaseUrl = "https://image.tmdb.org/t/p/w500";
        private const string PlaceholderUrl = "https://via.placeholder.com/300x450?text=Sin+Imagen";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string url = value as string;

            // 1. Si es nulo o vacío, devolver imagen por defecto
            if (string.IsNullOrEmpty(url))
            {
                return PlaceholderUrl;
            }

            // 2. Si ya es una URL completa (http...), devolverla tal cual
            if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                return url;
            }

            // 3. Si es relativa, concatenar con el dominio de TMDB
            // El TrimStart('/') asegura que no dupliquemos la barra
            return $"{BaseUrl}/{url.TrimStart('/')}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // No necesitamos convertir de Imagen a URL para guardar
        }
    }
}