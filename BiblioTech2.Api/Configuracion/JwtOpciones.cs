namespace BiblioTech2.Api.Configuracion;

/// <summary>
/// Sección "Jwt" del appsettings. Se llena sola con builder.Services.Configure&lt;JwtOpciones&gt;(...).
/// La Clave NUNCA va en appsettings.json: en desarrollo está en appsettings.Development.json
/// y en Azure se pone como variable de configuración "Jwt__Clave".
/// </summary>
public class JwtOpciones
{
    public const string Seccion = "Jwt";

    public string Clave { get; set; } = string.Empty;        // mínimo 32 caracteres (HMAC-SHA256)
    public string Emisor { get; set; } = string.Empty;       // quién crea el token (la API)
    public string Audiencia { get; set; } = string.Empty;    // para quién es el token (la Web)
    public int MinutosExpiracion { get; set; } = 120;
}
