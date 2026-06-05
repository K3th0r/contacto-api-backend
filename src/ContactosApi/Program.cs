using ContactosApi.Middleware;
using ContactosApi.Repositories;
using ContactosApi.Services;

var builder = WebApplication.CreateBuilder(args);


//exponer endpoints en controller
builder.Services.AddControllers();


//dependencias usando interfaces -- desacoplo
builder.Services.AddSingleton<IContactoRepository, InMemoryContactoRepository>();
builder.Services.AddScoped<IContactoService, ContactoService>();


//declara uso de servicios de swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


//captura de errores nos controlados mediante middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();


//habilita swagger
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();


//implementado para pruebas de integracion con    WebApplicationFactory
public partial class Program
{
}