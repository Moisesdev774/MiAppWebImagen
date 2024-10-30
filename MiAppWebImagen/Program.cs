using Microsoft.EntityFrameworkCore; // Importa el espacio de nombres para utilizar Entity Framework Core

var builder = WebApplication.CreateBuilder(args); // Crea un nuevo constructor de la aplicaci�n web con los argumentos de la l�nea de comandos

// Configura y agrega un contexto de base de datos para Entity Framework Core.
builder.Services.AddDbContext<MiBD>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Conn")) // Usa SQL Server con la cadena de conexi�n "Conn" del archivo de configuraci�n
);

// Agrega servicios al contenedor (para controladores y vistas).
builder.Services.AddControllersWithViews(); // Registra los servicios necesarios para la funcionalidad MVC

var app = builder.Build(); // Construye la aplicaci�n web

// Configura el pipeline de solicitudes HTTP.
if (!app.Environment.IsDevelopment()) // Verifica si no est� en un entorno de desarrollo
{
    app.UseExceptionHandler("/Home/Error"); // Maneja las excepciones redirigiendo a la acci�n de error en Home
    // El valor predeterminado de HSTS es de 30 d�as. Puede que desees cambiar esto para escenarios de producci�n, consulta https://aka.ms/aspnetcore-hsts.
    app.UseHsts(); // Activa HTTP Strict Transport Security (HSTS) para mejorar la seguridad
}

app.UseHttpsRedirection(); // Redirige todas las solicitudes HTTP a HTTPS
app.UseStaticFiles(); // Permite servir archivos est�ticos (como CSS, JS e im�genes)

app.UseRouting(); // Activa el enrutamiento para el manejo de solicitudes

app.UseAuthorization(); // Habilita la autorizaci�n de las solicitudes

// Configura la ruta predeterminada para los controladores
app.MapControllerRoute(
    name: "default", // Nombre de la ruta
    pattern: "{controller=Imagen}/{action=Index}/{id?}"); // Patr�n de la ruta: controlador "Imagen", acci�n "Index" y un par�metro opcional "id"

app.Run(); // Ejecuta la aplicaci�n web
