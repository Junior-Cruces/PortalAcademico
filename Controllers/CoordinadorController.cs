using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed; // ← ESTA LÍNEA
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

    public async Task<IActionResult> Index()
    {
        return View(await _context.Cursos.ToListAsync());
    }

    // GET: Coordinador/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Coordinador/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Codigo,Nombre,Creditos,CupoMaximo,HorarioInicio,HorarioFin,Activo")] Curso curso)
    {
        if (ModelState.IsValid)
        {
            _context.Add(curso);
            await _context.SaveChangesAsync();
            await _cache.RemoveAsync("CursosActivos"); // Invalidar caché
            return RedirectToAction(nameof(Index));
        }
        return View(curso);
    }

    // GET: Coordinador/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var curso = await _context.Cursos.FindAsync(id);
        if (curso == null) return NotFound();
        return View(curso);
    }

    // POST: Coordinador/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Codigo,Nombre,Creditos,CupoMaximo,HorarioInicio,HorarioFin,Activo")] Curso curso)
    {
        if (id != curso.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(curso);
                await _context.SaveChangesAsync();
                await _cache.RemoveAsync("CursosActivos"); // Invalidar caché
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CursoExists(curso.Id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(curso);
    }

    private bool CursoExists(int id) => _context.Cursos.Any(e => e.Id == id);
}