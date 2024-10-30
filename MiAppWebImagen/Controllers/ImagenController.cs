using MiAppWebImagen.Models; // Importa el espacio de nombres donde se encuentran los modelos
using Microsoft.AspNetCore.Mvc; // Importa el espacio de nombres para MVC
using Microsoft.EntityFrameworkCore; // Importa el espacio de nombres para Entity Framework Core
using System; // Importa el espacio de nombres para tipos básicos
using System.IO; // Importa el espacio de nombres para operaciones de archivos
using System.Threading.Tasks; // Importa el espacio de nombres para tareas asíncronas
using Microsoft.AspNetCore.Http; // Importa el espacio de nombres para manejar HTTP

public class ImagenController : Controller // Define la clase ImagenController que hereda de Controller
{
    private readonly MiBD _context; // Declara una variable privada de tipo MiBD para interactuar con la base de datos

    // Constructor que inicializa el contexto de la base de datos
    public ImagenController(MiBD context)
    {
        _context = context; // Asigna el contexto recibido a la variable privada
    }

    // INDEX: Listar Imágenes
    public async Task<IActionResult> Index() // Acción asíncrona que retorna una lista de imágenes
    {
        var listaImagenes = await _context.Imagen.ToListAsync(); // Obtiene la lista de imágenes de la base de datos
        return View(listaImagenes); // Retorna la vista con la lista de imágenes
    }

    // CREATE: Vista para Crear Imagen
    [HttpGet] // Atributo que indica que esta acción responde a una solicitud GET
    public IActionResult Crear() // Acción que muestra la vista para crear una nueva imagen
    {
        // Obtenemos la lista de imágenes en la carpeta "uploads" para mostrar en la vista
        var rutaCarpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagenes"); // Ruta de la carpeta donde se almacenan las imágenes
        var archivos = Directory.Exists(rutaCarpeta) // Verifica si la carpeta existe
            ? Directory.GetFiles(rutaCarpeta).Select(Path.GetFileName).ToList() // Obtiene los nombres de los archivos en la carpeta
            : new List<string>(); // Si no existe, crea una lista vacía

        ViewBag.ListaArchivos = archivos.Cast<string>().ToList(); // Pasa la lista de archivos a la vista

        return View(); // Retorna la vista para crear una imagen
    }

    [HttpPost] // Atributo que indica que esta acción responde a una solicitud POST
    public async Task<IActionResult> Crear(IFormFile archivoImagen, [FromForm] string nombre) // Acción para crear una nueva imagen
    {
        if (archivoImagen != null && archivoImagen.Length > 0) // Verifica que se haya seleccionado un archivo
        {
            var rutaCarpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagenes"); // Ruta de la carpeta para guardar la imagen
            var nombreArchivo = Guid.NewGuid() + Path.GetExtension(archivoImagen.FileName); // Genera un nombre único para el archivo
            var rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo); // Ruta completa donde se guardará el archivo

            // Guarda el archivo en el servidor
            using (var stream = new FileStream(rutaCompleta, FileMode.Create)) // Crea un nuevo FileStream para guardar el archivo
            {
                await archivoImagen.CopyToAsync(stream); // Copia el archivo subido al FileStream
            }

            // Guarda la ruta relativa en la base de datos
            var rutaRelativa = Path.Combine("imagenes", nombreArchivo); // Crea la ruta relativa para almacenar en la base de datos
            var nuevaEntidad = new Imagen // Crea una nueva instancia de Imagen
            {
                Nombre = nombre, // Asigna el nombre recibido
                RutaImagen = rutaRelativa // Asigna la ruta de la imagen
            };

            _context.Imagen.Add(nuevaEntidad); // Agrega la nueva imagen al contexto
            await _context.SaveChangesAsync(); // Guarda los cambios en la base de datos

            return RedirectToAction("Index"); // Redirige a la acción Index
        }

        ModelState.AddModelError(string.Empty, "No se ha seleccionado ninguna imagen"); // Agrega un error al modelo si no se seleccionó una imagen
        return View(); // Retorna la vista si hay un error
    }

    // GET: Detalles de la Imagen
    [HttpGet] // Atributo que indica que esta acción responde a una solicitud GET
    public async Task<IActionResult> Detalles(int id) // Acción para mostrar los detalles de una imagen específica
    {
        // Busca la imagen en la base de datos por su ID
        var imagen = await _context.Imagen.FindAsync(id); // Busca la imagen por su ID

        // Verifica si la imagen existe
        if (imagen == null) // Si no se encuentra la imagen
        {
            return NotFound(); // Retorna un error 404
        }

        // Retorna la vista Detalles con la entidad de imagen
        return View(imagen); // Retorna la vista con los detalles de la imagen
    }

    // EDIT: Vista para Editar Imagen
    [HttpGet] // Atributo que indica que esta acción responde a una solicitud GET
    public async Task<IActionResult> Editar(int id) // Acción para mostrar la vista de edición de una imagen
    {
        var entidad = await _context.Imagen.FindAsync(id); // Busca la imagen en la base de datos por su ID
        if (entidad == null) // Verifica si la entidad existe
        {
            return NotFound(); // Retorna un error 404 si no existe
        }

        return View(entidad); // Retorna la vista de edición con la entidad encontrada
    }

    // UPDATE: Actualizar Imagen
    [HttpPost] // Atributo que indica que esta acción responde a una solicitud POST
    public async Task<IActionResult> Actualizar(int id, IFormFile nuevaImagen, [FromForm] string nombre) // Acción para actualizar una imagen existente
    {
        var entidad = await _context.Imagen.FindAsync(id); // Busca la imagen en la base de datos por su ID
        if (entidad == null) // Verifica si la entidad existe
        {
            return NotFound(); // Retorna un error 404 si no existe
        }

        entidad.Nombre = nombre; // Actualiza el nombre de la imagen

        if (nuevaImagen != null && nuevaImagen.Length > 0) // Verifica si se subió una nueva imagen
        {
            var rutaImagenAnterior = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", entidad.RutaImagen); // Ruta de la imagen anterior
            if (System.IO.File.Exists(rutaImagenAnterior)) // Verifica si la imagen anterior existe
            {
                System.IO.File.Delete(rutaImagenAnterior); // Elimina la imagen anterior del servidor
            }

            var rutaCarpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagenes"); // Ruta de la carpeta para guardar la nueva imagen
            var nombreArchivo = Guid.NewGuid() + Path.GetExtension(nuevaImagen.FileName); // Genera un nombre único para el nuevo archivo
            var rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo); // Ruta completa donde se guardará la nueva imagen

            using (var stream = new FileStream(rutaCompleta, FileMode.Create)) // Crea un nuevo FileStream para guardar la nueva imagen
            {
                await nuevaImagen.CopyToAsync(stream); // Copia el archivo subido al FileStream
            }

            entidad.RutaImagen = Path.Combine("imagenes", nombreArchivo); // Actualiza la ruta de la imagen en la entidad
        }

        _context.Imagen.Update(entidad); // Actualiza la entidad en el contexto
        await _context.SaveChangesAsync(); // Guarda los cambios en la base de datos

        return RedirectToAction("Index"); // Redirige a la acción Index
    }

    // DELETE: Eliminar Imagen
    [HttpPost] // Atributo que indica que esta acción responde a una solicitud POST
    public async Task<IActionResult> Eliminar(int id) // Acción para eliminar una imagen
    {
        var entidad = await _context.Imagen.FindAsync(id); // Busca la imagen en la base de datos por su ID
        if (entidad == null) // Verifica si la entidad existe
        {
            return NotFound(); // Retorna un error 404 si no existe
        }

        var rutaCompleta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", entidad.RutaImagen); // Ruta completa de la imagen a eliminar
        if (System.IO.File.Exists(rutaCompleta)) // Verifica si la imagen existe
        {
            System.IO.File.Delete(rutaCompleta); // Elimina la imagen del servidor
        }

        _context.Imagen.Remove(entidad); // Elimina la entidad del contexto
        await _context.SaveChangesAsync(); // Guarda los cambios en la base de datos

        return RedirectToAction("Index"); // Redirige a la acción Index
    }
}
