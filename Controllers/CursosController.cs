using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Data;
using PortalAcademico.Models;

namespace PortalAcademico.Controllers;

public class CursosController : Controller
{
    private readonly ApplicationDbContext _context;

    public CursosController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Cursos
    public async Task<IActionResult> Index(string nombre, int? creditosMin, int? creditosMax)
    {
        var query = _context.Cursos.Where(c => c.Activo).AsQueryable();

        // Filtro por nombre
        if (!string.IsNullOrWhiteSpace(nombre))
        {
            query = query.Where(c => c.Nombre.Contains(nombre));
        }

        // Filtro por créditos mínimo
        if (creditosMin.HasValue && creditosMin > 0)
        {
            query = query.Where(c => c.Creditos >= creditosMin.Value);
        }

        // Filtro por créditos máximo
        if (creditosMax.HasValue && creditosMax > 0)
        {
            query = query.Where(c => c.Creditos <= creditosMax.Value);
        }

        var cursos = await query.ToListAsync();
        return View(cursos);
    }

    // GET: Cursos/Detalle/5
    public async Task<IActionResult> Detalle(int? id)
    {
        if (id == null) return NotFound();

        var curso = await _context.Cursos.FindAsync(id);
        if (curso == null || !curso.Activo) return NotFound();

        return View(curso);
    }
}