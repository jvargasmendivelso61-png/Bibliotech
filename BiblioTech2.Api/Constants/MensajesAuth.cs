namespace BiblioTech2.Api.Constants
{
    public static class MensajesAuth
    {
        public const string LoginExitoso = "Inicio de sesión correcto.";
        // Mismo mensaje si falla el correo o la contraseña: no damos pistas a un atacante.
        public const string CredencialesInvalidas = "Correo o contraseña incorrectos.";

        public const string Registrado = "Usuario registrado correctamente.";
        public const string PrimerAdmin = "Primer usuario registrado como Admin. Desde ahora solo un Admin puede registrar usuarios.";
        public const string Perfil = "Usuario autenticado.";

        public const string NoAutenticado = "Debes iniciar sesión para realizar esta acción.";
        public const string SinPermiso = "No tienes permiso para realizar esta acción.";
        public const string SoloAdmin = "Solo un Admin puede registrar usuarios.";

        public static string EmailDuplicado(string email) => $"Ya existe un usuario con el correo {email}.";
        public static string NoExiste(int id) => $"El usuario {id} no existe.";
    }
}
