using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Data;
using PortalAcademico.Models;

public class CursosController : Controller
{
    private readonly ApplicationDbContext _context;

    public CursosController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string nombre, int? creditosMin, int? creditosMax)
    {
        var cursos = _context.Cursos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(nombre))
            cursos = cursos.Where(c => c.Nombre.Contains(nombre));

        if (creditosMin.HasValue)
            cursos = cursos.Where(c => c.Creditos >= creditosMin.Value);

        if (creditosMax.HasValue)
            cursos = cursos.Where(c => c.Creditos <= creditosMax.Value);

        cursos = cursos.Where(c => c.Activo);

        return View(await cursos.ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var curso = await _context.Cursos.FirstOrDefaultAsync(m => m.Id == id);
        if (curso == null) return NotFound();

        return View(curso);
    }
}