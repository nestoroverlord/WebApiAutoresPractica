using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entidades;
using WebApiAutores.Filtros;
using WebApiAutores.Servicios;

namespace WebApiAutores.Controllers
{
    [ApiController] //Realiza validaciones automáticas respecto a la data recibida en nuestro controlador
    [Route("api/autores")] // Definimos la ruta de para nuestra api, es un endpoint nuestra ruta base -- También se puede así "api/[controller]"
    //[Authorize]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IServicio _servicio;
        private readonly ServicioTransient _servicioTransient;
        private readonly ServicioScoped _servicioScoped;
        private readonly ServicioSingleton _servicioSingleton;
        private readonly ILogger<AutoresController> _logger;

        //Inyección de dependencias
        public AutoresController(ApplicationDbContext context,
                                 IServicio servicio,
                                 ServicioTransient servicioTransient,
                                 ServicioScoped servicioScoped,
                                 ServicioSingleton servicioSingleton,
                                 ILogger<AutoresController> logger)
        {
            this.context = context;
            this._servicio = servicio;
            this._servicioTransient = servicioTransient;
            this._servicioScoped = servicioScoped;
            this._servicioSingleton = servicioSingleton;
            this._logger = logger;
        }

        [HttpGet("GUID")]
        //[ResponseCache(Duration = 10)]
        [ServiceFilter(typeof(MiFiltroDeAccion))]
        public ActionResult ObtenerGuids()
        {
            return Ok(new { 
                AutoresControllerTransient = _servicioTransient.guid,
                ServicioA_Transient = _servicio.ObtenerTransient(),
                AutoresControllerScoped = _servicioScoped.guid,
                ServicioA_Scoped = _servicio.ObtenerScoped(),
                AutoresControllerSingleton = _servicioSingleton.guid,
                ServicioA_Singleton = _servicio.ObtenerSingleton()
            });
        }

        [HttpGet] //=> api/autores
        [HttpGet("listado")] //=> api/autores/listado
        [HttpGet("/listado")] //=> /listado
        //[ResponseCache(Duration = 10)]
        [ServiceFilter(typeof(MiFiltroDeAccion))]
        public async Task<ActionResult<List<Autor>>> Get()
        {
            _logger.LogInformation("Estamos obteniendo el listado de autores");
            _logger.LogWarning("Esto es una prueba de warning");
            _servicio.RealizarTarea();
            return await context.Autores.Include(x => x.Libros).ToListAsync();
        }

        [HttpGet("primerAutor")]
        public async Task<ActionResult<Autor>> GetPrimerAutor()
        {
            return await context.Autores.FirstOrDefaultAsync();
        }

        [HttpGet("priAutFromHeader")]
        public async Task<ActionResult<Autor>> GetPrimerAutor([FromHeader] int myvalue, [FromQuery] string valors)
        {
            return await context.Autores.FirstOrDefaultAsync();
        }

        //[HttpGet("{id:int}/{param2=persona}")] // {param2?}
        //public async Task<ActionResult<Autor>> Get(int id, string param2)
        //{
        //    var autor = await context.Autores.FirstOrDefaultAsync(x => x.Id == id);

        //    if (autor == null)
        //    {
        //        return NotFound();
        //    }
        //    return autor;
        //}

        //Por id o por nombre
        [HttpGet("{id:int}/{nombre=defecto}")]
        public async Task<ActionResult<Autor>> BuscarAutor(int id, string nombre)
        {
            var code = await context.Autores.FirstOrDefaultAsync(x => x.Id == id);
            var name = await context.Autores.FirstOrDefaultAsync(x => x.Nombre.Contains(nombre));

            if (code == null)
            {
                if (name == null)
                {
                    return NotFound();
                }
                else
                {
                    return name;
                }
            }
            return code;
        }

        [HttpGet("{nombre}")]
        public async Task<ActionResult<Autor>> GetName(string nombre)
        {
            var autor = await context.Autores.FirstOrDefaultAsync(x => x.Nombre.Contains(nombre));

            if (autor == null)
            {
                return NotFound();
            }
            return autor;
        }

        [HttpGet("{routename}")]
        public async Task<ActionResult<Autor>> GetFromRoute([FromRoute]string nombre)
        {
            var autor = await context.Autores.FirstOrDefaultAsync(x => x.Nombre.Contains(nombre));

            if (autor == null)
            {
                return NotFound();
            }
            return autor;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody]Autor autor)
        {

            var existeAutor = await context.Autores.AnyAsync(x => x.Nombre == autor.Nombre);

            if (existeAutor)
            {
                return BadRequest($"Ya existe un autor con el nombre {autor.Nombre}");
            }

            context.Add(autor);
            await context.SaveChangesAsync(); // guardar cambios de manera asíncrona
            return Ok();
        }

        [HttpPut ("{id:int}")] // pa actualizar api/autores/1
        public async Task<ActionResult> Put(Autor autor, int id)
        {
            if (autor.Id != id)
            {
                return BadRequest("El id del autor no coincide con el id de la URL");
            }

            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            context.Update(autor);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("/updateAutor/{Autor}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> ActualizarAutor([FromBody] Autor obj)
        {
            var existe = await context.Autores.AnyAsync(a => a.Id == obj.Id && a.Nombre == obj.Nombre);

            if (!existe) 
            {
                return NotFound();
            }

            context.Update(obj);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("/updateAutorV2")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> ActualizarAutor([FromHeader] int Id, Autor obj)
        {
            var existe = await context.Autores.AnyAsync(a => a.Id == Id);

            if (!existe)
            {
                return NotFound();
            }

            context.Update(obj);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]// pa borrar api/autores/1
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Autor() { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
