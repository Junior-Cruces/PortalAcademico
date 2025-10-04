# Portal Académico — Examen Parcial Programación I 2025

Sistema de gestión de cursos y matrículas para una universidad, desarrollado en ASP.NET Core MVC (.NET 8) con Identity, EF Core, Redis y desplegado en Render.

🔗 **Repositorio**: https://github.com/Junior-Cruces/PortalAcademico  
🌐 **URL en Render**: https://portalacademico.onrender.com 

---

## 🚀 Funcionalidades implementadas

- ✅ **Pregunta 1**: Modelos `Curso` y `Matricula` con validaciones (créditos > 0, horario válido, unicidad de matrícula, cupo máximo).
- ✅ **Pregunta 2**: Catálogo de cursos con filtros (nombre, créditos, horario) y vista detalle.
- ✅ **Pregunta 3**: Inscripción con validaciones server-side (cupo, choque de horarios, autenticación).
- ✅ **Pregunta 4**: 
  - Sesión con Redis: enlace “Volver al curso {Nombre}” en el layout.
  - Caché con Redis: catálogo almacenado 60 segundos, invalidado al editar cursos.
- ✅ **Pregunta 5**: Panel de Coordinador con rol, CRUD de cursos y gestión de matrículas (confirmar/cancelar).
- ✅ **Pregunta 6**: Despliegue en Render con variables de entorno.

---

## 🛠️ Ejecución local

### Requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- (Opcional) Redis local o cuenta en [Redis Labs](https://app.redislabs.com/)

### Pasos para ejecutar
1. Clonar el repositorio:
   ```bash
   git clone https://github.com/Junior-Cruces/PortalAcademico.git
   cd PortalAcademico
   2: restaurar
   dotnet restore
   3:aplicar migraciones 
   dotnet ef database update
   4: confirurar redits 
   dotnet user-secrets set "Redis__ConnectionString" "tu-cadena-de-redis"
   5: ejecutar 
   dotnet run

### Credenciales de prueba
   
Coordinador:
Email: coordinador@universidad.edu
Contraseña: Password123!
Estudiante: Regístrate con cualquier correo.
