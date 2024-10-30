using Microsoft.EntityFrameworkCore; // Importa el espacio de nombres para utilizar Entity Framework Core

public class MiBD : DbContext // Definición de la clase MiBD que hereda de DbContext
{
    // Constructor de la clase que recibe opciones de configuración para el DbContext
    public MiBD(DbContextOptions<MiBD> options) : base(options) { }

    public DbSet<Imagen> Imagen { get; set; } // Propiedad que representa la tabla de imágenes en la base de datos

    // Método que se llama para configurar el modelo de datos
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // Llama al método base para realizar configuraciones adicionales si es necesario
    }
}
