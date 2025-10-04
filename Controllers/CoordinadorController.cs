using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using PortalAcademico.Data;
using PortalAcademico.Models;

[Authorize(Roles = "Coordinador")]
public class CoordinadorController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IDistributedCache _cache;

    public CoordinadorController(ApplicationDbContext context, IDistributedCache cache)
    {
        _context = context;
        _cache = cache;
    }

    // GET: /Coordinador
    public async Task<IActionResult> Index()
    {
        return View(await _context.Cursos.ToListAsync());
    }

    // GET: /Coordinador/Matriculas/5
    public async Task<IActionResult> Matriculas(int id)
    {
        var curso = await _context.Cursos.FindAsync(id);
        if (curso == null) return NotFound();

        var matriculas = await _context.Matriculas
            .Where(m => m.CursoId == id)
            .Include(m => m.Usuario)
            .ToListAsync();

        ViewBag.Curso = curso;
        return View(matriculas);
    }

    // POST: /Coordinador/Confirmar
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirmar(int matriculaId)
    {
        var matricula = await _context.Matriculas.FindAsync(matriculaId);
        if (matricula == null) return NotFound();

        matricula.Estado = "Confirmada";
        _context.Update(matricula);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Matrícula confirmada.";
        return RedirectToAction("Matriculas", new { id = matricula.CursoId });
    }

    // POST: /Coordinador/Cancelar
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int matriculaId)
    {
        var matricula = await _context.Matriculas.FindAsync(matriculaId);
        if (matricula == null) return NotFound();

        matricula.Estado = "Cancelada";
        _context.Update(matricula);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Matrícula cancelada.";
        return RedirectToAction("Matriculas", new { id = matricula.CursoId });
    }

    // GET: /Coordinador/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Coordinador/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Codigo,Nombre,Creditos,CupoMaximo,HorarioInicio,HorarioFin,Activo")] Curso curso)
    {
        if (ModelState.IsValid)
        {
            _context.Add(curso);
            await _context.SaveChangesAsync();
            await _cache.RemoveAsync("CursosActivos"); // Invalidar caché (Pregunta 4)
            TempData["Success"] = "Curso creado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        return View(curso);
    }

    // GET: /Coordinador/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var curso = await _context.Cursos.FindAsync(id);
        if (curso == null) return NotFound();
        return View(curso);
    }

    // POST: /Coordinador/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Codigo,Nombre,Creditos,CupoMaximo,HorarioInicio,HorarioFin,Activo")] Curso curso)
    {
        if (id != curso.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(curso);
                await _context.SaveChangesAsync();
                await _cache.RemoveAsync("CursosActivos"); // Invalidar caché
                TempData["Success"] = "Curso actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CursoExists(curso.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        return View(curso);
    }

    private bool CursoExists(int id)
    {
        return _context.Cursos.Any(e => e.Id == id);
    }
}