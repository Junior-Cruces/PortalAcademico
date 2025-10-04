using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed; // ← ESTA LÍNEA
using PortalAcademico.Data;
using PortalAcademico.Models;

public class CursosController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IDistributedCache _cache;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CursosController(
        ApplicationDbContext context,
        IDistributedCache cache,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _cache = cache;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IActionResult> Index(string nombre, int? creditosMin, int? creditosMax)
    {
        // Obtener cursos desde caché o base de datos
        const string cacheKey = "CursosActivos";
        var cursosJson = await _cache.GetStringAsync(cacheKey);

        List<Curso> cursos;
        if (cursosJson == null)
        {
            cursos = await _context.Cursos.Where(c => c.Activo).ToListAsync();
            await _cache.SetStringAsync(cacheKey, System.Text.Json.JsonSerializer.Serialize(cursos), 
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60) });
        }
        else
        {
            cursos = System.Text.Json.JsonSerializer.Deserialize<List<Curso>>(cursosJson)!;
        }

        // Aplicar filtros en memoria (después de caché)
        if (!string.IsNullOrWhiteSpace(nombre))
            cursos = cursos.Where(c => c.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase)).ToList();

        if (creditosMin.HasValue)
            cursos = cursos.Where(c => c.Creditos >= creditosMin.Value).ToList();

        if (creditosMax.HasValue)
            cursos = cursos.Where(c => c.Creditos <= creditosMax.Value).ToList();

        return View(cursos);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var curso = await _context.Cursos.FirstOrDefaultAsync(m => m.Id == id);
        if (curso == null) return NotFound();

        // Guardar en sesión el último curso visitado
        var session = _httpContextAccessor.HttpContext!.Session;
        session.SetString("UltimoCursoNombre", curso.Nombre);
        session.SetInt32("UltimoCursoId", curso.Id);

        return View(curso);
    }
}