namespace BiblioTech2.Api.Constants
{
    /// <summary>
    /// Valores fijos de las reglas de negocio de préstamos.
    /// Si la biblioteca cambia una regla (ej: 4 préstamos), se cambia solo aquí.
    /// </summary>
    public static class ReglasPrestamo
    {
        public const string EstadoActivo = "Activo";
        public const string EstadoDevuelto = "Devuelto";

        public const int MaximoPrestamosActivos = 3;   // R2
        public const int PlazoDias = 15;               // R4
    }
}
