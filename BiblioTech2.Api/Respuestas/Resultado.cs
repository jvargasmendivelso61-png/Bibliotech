namespace BiblioTech2.Api.Respuestas
{
    /// <summary>Resultado de una operación SIN datos (actualizar, eliminar...).</summary>
    public class Resultado
    {
        public bool Exito { get; }
        public String Mensaje { get; }
        public TipoResultado Tipo { get; }

        protected Resultado(bool exito, String mensaje, TipoResultado tipo)
        {
            Exito = exito;
            Mensaje = mensaje;
            Tipo = tipo;
        }

        public static Resultado ok(String mensaje) => new(true, mensaje, TipoResultado.Exito);
        public static Resultado NoEncontrado(String mensaje) => new(false, mensaje, TipoResultado.NoEncontrado);
        public static Resultado ReglaNegocio(String mensaje) => new(false, mensaje, TipoResultado.ReglaNegocio);
        public static Resultado Validacion(String mensaje) => new(false, mensaje, TipoResultado.Validacion);
        public static Resultado NoAutorizado(String mensaje) => new(false, mensaje, TipoResultado.NoAutorizado);
        public static Resultado Prohibido(String mensaje) => new(false, mensaje, TipoResultado.Prohibido);
    }

    /// <summary>Resultado de una operación CON datos (listar, obtener, crear...).</summary>
    public class Resultado<T> : Resultado
    {
        public T? Datos { get; }

        private Resultado(bool exito, String mensaje, TipoResultado tipo, T? datos)
            : base(exito, mensaje, tipo)
        {
            Datos = datos;
        }

        public static Resultado<T> ok(T datos, String mensaje) => new(true, mensaje, TipoResultado.Exito, datos);
        public static new Resultado<T> NoEncontrado(String mensaje) => new(false, mensaje, TipoResultado.NoEncontrado, default);
        public static new Resultado<T> ReglaNegocio(String mensaje) => new(false, mensaje, TipoResultado.ReglaNegocio, default);
        public static new Resultado<T> Validacion(String mensaje) => new(false, mensaje, TipoResultado.Validacion, default);
        public static new Resultado<T> NoAutorizado(String mensaje) => new(false, mensaje, TipoResultado.NoAutorizado, default);
        public static new Resultado<T> Prohibido(String mensaje) => new(false, mensaje, TipoResultado.Prohibido, default);
    }
}
