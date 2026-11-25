namespace BiblioTech2.Api.Constants
{
    public static class MensajesLibro
    {
        public const string Listado = "Libros consultados correctamente.";
        public const string Encontrado = "Libro encontrado.";
        public const string Creado = "Libro creado correctamente.";
        public const string Actualizado = "Libro actualizado correctamente.";
        public const string Eliminado = "Libro eliminado correctamente.";

        public static string NoExiste(int id) => $"El libro {id} no existe.";
        public static string IsbnDuplicado(string isbn) => $"Ya existe un libro con el ISBN {isbn}.";
        public static string AutoresNoExisten(IEnumerable<int> ids) =>
            $"Los siguientes autores no existen: {string.Join(", ", ids)}.";
        public static string TotalesMenoresQuePrestados(int prestados) =>
            $"No se pueden dejar menos ejemplares que los {prestados} que están prestados ahora.";
        public static string TienePrestamos(int cantidad) =>
            $"No se puede eliminar el libro porque tiene {cantidad} préstamo(s) registrado(s).";
    }
}
