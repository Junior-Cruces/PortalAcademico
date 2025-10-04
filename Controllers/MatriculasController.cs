using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Data;
using PortalAcademico.Models;

[Authorize]
public class MatriculasController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public MatriculasController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    [ActionName("Inscribir")]
    public async Task<IActionResult> InscribirGet(int cursoId)
    {
        var usuario = await _userManager.GetUserAsync(User);
        if (usuario == null) return Challenge();

        var curso = await _context.Cursos.FindAsync(cursoId);
        if (curso == null || !curso.Activo)
            return NotFound();

        return View("ConfirmarInscripcion", curso);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Inscribir")]
    public async Task<IActionResult> InscribirPost(int cursoId)
    {
        var usuario = await _userManager.GetUserAsync(User);
        if (usuario == null) return Challenge();

        var curso = await _context.Cursos.FindAsync(cursoId);
        if (curso == null || !curso.Activo)
            return NotFound();

        // Validación: ya matriculado
        bool yaMatriculado = await _context.Matriculas
            .AnyAsync(m => m.UsuarioId == usuario.Id && m.CursoId == cursoId);
        if (yaMatriculado)
        {
            TempData["Error"] = "Ya estás inscrito en este curso.";
            return RedirectToAction("Index", "Cursos");
        }

        // Validación: cupo máximo
        int matriculasActuales = await _context.Matriculas
            .CountAsync(m => m.CursoId == cursoId && (m.Estado == "Pendiente" || m.Estado == "Confirmada"));
        if (matriculasActuales >= curso.CupoMaximo)
        {
            TempData["Error"] = "El curso ha alcanzado su cupo máximo.";
            return RedirectToAction("Index", "Cursos");
        }

        // Validación: choque de horarios
        var cursosMatriculados = await _context.Matriculas
            .Where(m => m.UsuarioId == usuario.Id && (m.Estado == "Pendiente" || m.Estado == "Confirmada"))
            .Select(m => m.Curso)
            .ToListAsync();

        foreach (var c in cursosMatriculados)
        {
            if (curso.HorarioInicio < c.HorarioFin && curso.HorarioFin > c.HorarioInicio)
            {
                TempData["Error"] = $"Horario en conflicto con el curso '{c.Nombre}'.";
                return RedirectToAction("Index", "Cursos");
            }
        }

        // Crear matrícula
        var matricula = new Matricula
        {
            CursoId = cursoId,
            UsuarioId = usuario.Id,
            FechaRegistro = DateTime.Now,
            Estado = "Pendiente"
        };

        _context.Matriculas.Add(matricula);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Te has inscrito en '{curso.Nombre}'. Estado: Pendiente.";
        return RedirectToAction("Index", "Cursos");
    }
}