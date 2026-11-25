namespace BiblioTech2.Api.Constants
{
    public static class MensajesSocio
    {
        public const string Listado = "Socios consultados correctamente.";
        public const string Encontrado = "Socio encontrado.";
        public const string Creado = "Socio creado correctamente.";
        public const string Actualizado = "Socio actualizado correctamente.";
        public const string Activado = "Socio activado correctamente.";
        public const string Desactivado = "Socio desactivado correctamente.";

        public static string NoExiste(int id) => $"El socio {id} no existe.";
        public static string DocumentoDuplicado(string documento) => $"Ya existe un socio con el documento {documento}.";
        public static string EmailDuplicado(string email) => $"Ya existe un socio con el correo {email}.";
        public static string YaEstaba(bool activo) => $"El socio ya estaba {(activo ? "activo" : "inactivo")}.";
        public static string TienePrestamosActivos(int cantidad) =>
            $"No se puede desactivar el socio porque tiene {cantidad} préstamo(s) sin devolver.";
    }
}
