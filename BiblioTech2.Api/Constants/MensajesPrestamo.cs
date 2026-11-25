namespace BiblioTech2.Api.Constants
{
    public static class MensajesPrestamo
    {
        public const string Listado = "Préstamos consultados correctamente.";
        public const string Encontrado = "Préstamo encontrado.";
        public const string Vencidos = "Préstamos vencidos consultados correctamente.";
        public const string Creado = "Préstamo registrado correctamente.";

        public static string NoExiste(int id) => $"El préstamo {id} no existe.";
        public static string LibroNoExiste(int id) => $"El libro {id} no existe.";
        public static string SocioNoExiste(int id) => $"El socio {id} no existe.";
        public static string EstadoInvalido(string estado) =>
            $"El estado '{estado}' no es válido. Usa: Activo, Devuelto o Vencido.";

        // Reglas de negocio
        public static string SinDisponibles(string titulo) =>                                          // R1
            $"No hay ejemplares disponibles de \"{titulo}\".";
        public static string MaximoPrestamos(int maximo) =>                                             // R2
            $"El socio ya tiene {maximo} préstamos activos, que es el máximo permitido.";
        public static string TieneVencidos(int cantidad) =>                                             // R3
            $"El socio tiene {cantidad} préstamo(s) vencido(s). Debe devolverlos antes de pedir otro.";
        public const string SocioInactivo = "El socio está inactivo y no puede pedir préstamos.";       // R6
        public const string YaDevuelto = "Este préstamo ya fue devuelto.";                              // R5

        public static string Devuelto(int diasRetraso) => diasRetraso > 0
            ? $"Préstamo devuelto con {diasRetraso} día(s) de retraso."
            : "Préstamo devuelto a tiempo.";
    }
}
