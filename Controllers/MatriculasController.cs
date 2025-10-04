using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Data;
using PortalAcademico.Models;
using System.Security.Claims;

namespace PortalAcademico.Controllers;

[Authorize]
public class MatriculasController : Controller
{
    private readonly ApplicationDbContext _context;

    public MatriculasController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Matriculas/Inscribir?cursoId=1
    public async Task<IActionResult> Inscribir(int cursoId)
    {
        var curso = await _context.Cursos.FindAsync(cursoId);
        if (curso == null || !curso.Activo)
        {
            TempData["Error"] = "El curso no está disponible.";
            return RedirectToAction("Index", "Cursos");
        }

        ViewBag.Curso = curso;
        return View();
    }

    // POST: /Matriculas/Inscribir
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> InscribirConfirmar(int cursoId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            TempData["Error"] = "Debes iniciar sesión para inscribirte.";
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }

        var curso = await _context.Cursos.FindAsync(cursoId);
        if (curso == null || !curso.Activo)
        {
            TempData["Error"] = "El curso no está disponible.";
            return RedirectToAction("Index", "Cursos");
        }

        // Validar cupo
        var matriculados = await _context.Matriculas
            .CountAsync(m => m.CursoId == cursoId && m.Estado == EstadoMatricula.Confirmada);
        if (matriculados >= curso.CupoMaximo)
        {
            TempData["Error"] = "Lo sentimos, el cupo del curso está lleno.";
            return RedirectToAction("Detalle", "Cursos", new { id = cursoId });
        }

        // Validar horario solapado
        var cursosMatriculados = await _context.Matriculas
            .Where(m => m.UsuarioId == userId && m.Estado == EstadoMatricula.Confirmada)
            .Select(m => m.Curso)
            .ToListAsync();

        foreach (var c in cursosMatriculados)
        {
            if (curso.HorarioInicio < c.HorarioFin && curso.HorarioFin > c.HorarioInicio)
            {
                TempData["Error"] = $"Horario solapado con el curso '{c.Nombre}'.";
                return RedirectToAction("Detalle", "Cursos", new { id = cursoId });
            }
        }

        // Crear matrícula en estado Pendiente
        var matricula = new Matricula
        {
            CursoId = cursoId,
            UsuarioId = userId,
            Estado = EstadoMatricula.Pendiente,
            FechaRegistro = DateTime.UtcNow
        };

        _context.Matriculas.Add(matricula);
        await _context.SaveChangesAsync();

        TempData["Success"] = "¡Inscripción realizada! Estado: Pendiente.";
        return RedirectToAction("Index", "Cursos");
    }
}