
namespace BiblioTech2.Api.Constants
{
    public static class MensajesAutor
    {
        public const string Listado = "Autores consultados correctamente.";
        public const string Encontrado = "Autor encontrado.";
        public const string Creado = "Autor creado correctamente.";
        public const string Actualizado = "Autor actualizado correctamente.";
        public const string Eliminado = "Autor eliminado correctamente.";

        public static string NoExiste(int id) => $"El autor {id} no existe.";
        public static string TieneLibros(int cantidad) =>
            $"No se puede eliminar el autor porque tiene {cantidad} libro(s) asociado(s).";





    }
}
