namespace BiblioTech2.Api.Respuestas
{
    /// <summary>Qué pasó con la operación. ApiControllerBase lo traduce a un código HTTP.</summary>
    public enum TipoResultado
    {
        Exito,          // 200
        NoEncontrado,   // 404
        ReglaNegocio,   // 409
        Validacion,     // 400
        NoAutorizado,   // 401  (login fallido)
        Prohibido       // 403  (logueado, pero sin el rol necesario)
    }
}
