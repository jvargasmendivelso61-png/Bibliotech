namespace BiblioTech2.Api.Respuestas
{
    public class RespuestaApi<T>
    {
        public bool Exito {  get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public T? Datos { get; set; }
        public Dictionary<String, String[]>? Errores { get; set; }


        public static RespuestaApi<T> Desde(Resultado<T> resultado) => new()
        {
            Exito = resultado.Exito,
            Mensaje = resultado.Mensaje,
            Datos = resultado.Datos


        };

        public static RespuestaApi<T> Desde(Resultado resultado) => new()
        {
            Exito = resultado.Exito,
            Mensaje = resultado.Mensaje

        }; 





    }
}
