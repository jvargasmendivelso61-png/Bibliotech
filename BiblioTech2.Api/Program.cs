using Scalar.AspNetCore;
using BiblioTech2.Api.Configuracion;
using BiblioTech2.Api.Data;
using BiblioTech2.Api.Middleware;
using BiblioTech2.Api.Respuestas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ---------- 1. SERVICIOS ----------

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(opciones =>
    {
        // Cuando fallan las validaciones del DTO ([Required], [StringLength]...)
        // respondemos con el mismo formato RespuestaApi en lugar del ProblemDetails por defecto.
        opciones.InvalidModelStateResponseFactory = contexto =>
        {
            var errores = contexto.ModelState
                .Where(campo => campo.Value!.Errors.Count > 0)
                .ToDictionary(
                    campo => campo.Key,
                    campo => campo.Value!.Errors.Select(e => e.ErrorMessage).ToArray());

            return new BadRequestObjectResult(new RespuestaApi<object>
            {
                Exito = false,
                Mensaje = "Los datos enviados no son válidos.",
                Errores = errores
            });
        };
    });

builder.Services.AddDbContext<BibliotecaContext>(opciones =>
    opciones.UseSqlServer(builder.Configuration.GetConnectionString("BiblioTechConnection")));

// Todo esto está en Configuracion/ServiciosExtensions.cs
builder.Services.AddServiciosBiblioteca();
builder.Services.AddCorsWeb(builder.Configuration);
builder.Services.AddAutenticacionJwt(builder.Configuration);
builder.Services.AddOpenApiConJwt();

var app = builder.Build();

// ---------- 2. PIPELINE (el orden importa) ----------

app.UseCors(ServiciosExtensions.PoliticaCors);  // antes del middleware: así hasta los errores llevan el permiso CORS
app.UseMiddleware<ErrorHandlingMiddleware>();   // red de seguridad: envuelve a todo lo demás

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthentication();   // 1° ¿quién eres? (lee y valida el token)
app.UseAuthorization();    // 2° ¿puedes hacer esto? ([Authorize])
app.MapControllers();

app.Run();
