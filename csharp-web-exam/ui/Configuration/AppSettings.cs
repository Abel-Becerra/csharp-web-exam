using System.Configuration;

namespace csharp_web_exam.Configuration
{
    /// <summary>
    /// Clase para gestionar la configuración de la aplicación
    /// Permite acceder a los settings de manera tipada y centralizada
    /// </summary>
    public static class AppSettings
    {
        /// <summary>
        /// URL base de la API
        /// </summary>
        public static string ApiBaseUrl => ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "https://localhost:5001/api";

        /// <summary>
        /// Obtiene un valor de configuración por su clave
        /// </summary>
        /// <param name="key">Clave del setting</param>
        /// <param name="defaultValue">Valor por defecto si no existe</param>
        /// <returns>Valor del setting o el valor por defecto</returns>
        public static string GetSetting(string key, string defaultValue = null)
        {
            return ConfigurationManager.AppSettings[key] ?? defaultValue;
        }

        /// <summary>
        /// Obtiene un valor de configuración como entero
        /// </summary>
        /// <param name="key">Clave del setting</param>
        /// <param name="defaultValue">Valor por defecto si no existe o no es válido</param>
        /// <returns>Valor del setting como entero o el valor por defecto</returns>
        public static int GetSettingAsInt(string key, int defaultValue = 0)
        {
            var value = ConfigurationManager.AppSettings[key];
            return int.TryParse(value, out int result) ? result : defaultValue;
        }

        /// <summary>
        /// Obtiene un valor de configuración como booleano
        /// </summary>
        /// <param name="key">Clave del setting</param>
        /// <param name="defaultValue">Valor por defecto si no existe o no es válido</param>
        /// <returns>Valor del setting como booleano o el valor por defecto</returns>
        public static bool GetSettingAsBool(string key, bool defaultValue = false)
        {
            var value = ConfigurationManager.AppSettings[key];
            return bool.TryParse(value, out bool result) ? result : defaultValue;
        }
    }
}
