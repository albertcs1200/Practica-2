# Portal de Universidad

Este es el proyecto de evaluación técnica para gestionar cursos, estudiantes y matrículas.
Fue desarrollado utilizando ASP.NET Core MVC con Entity Framework Core (SQLite) e Identity.

## Ejecución Local

Para correr el proyecto localmente, asegúrate de tener el SDK de .NET 10 instalado.

1. Restaura los paquetes:
   ```bash
   dotnet restore
   ```

2. Ejecuta las migraciones. Al iniciar la aplicación, las migraciones pendientes y la siembra de datos (Seeding) se aplican de manera automática. No obstante, si prefieres correrlas manualmente:
   ```bash
   dotnet tool restore
   dotnet dotnet-ef database update
   ```

3. Ejecuta la aplicación:
   ```bash
   dotnet run
   ```

La aplicación estará disponible en `https://localhost:5001` (o el puerto que se indique en la consola).

### Datos Semilla
- **Usuario Coordinador**: `admin@universidad.edu` | Contraseña: `Password123!`
- Existen 3 cursos disponibles por defecto.

## Variables de Entorno

Si deseas desplegar la aplicación o usar una base de datos externa, configura la siguiente variable:

- `ConnectionStrings__DefaultConnection`: Cadena de conexión para la base de datos (por defecto se utiliza SQLite con `DataSource=app.db;Cache=Shared`).
- (Opcional) Variables de Redis para sesiones si se implementa más adelante.

## Despliegue en Render.com

Para desplegar esta aplicación como **Web Service** en Render.com, sigue estos pasos:

1. Crea un nuevo "Web Service" y conéctalo a tu repositorio de GitHub.
2. Selecciona **Docker** como el entorno de ejecución (Runtime). Render usará automáticamente el `Dockerfile` en la raíz.
3. En la sección **Environment Variables**, configura las siguientes variables mínimas obligatorias:
   - `ASPNETCORE_ENVIRONMENT`: `Production`
   - `ASPNETCORE_URLS`: `http://0.0.0.0:${PORT}`
   - `ConnectionStrings__DefaultConnection`: La cadena de conexión a SQLite (`Data Source=app.db`) o a tu base de datos en producción (ej. PostgreSQL si cambias de provider).
   - `Redis__ConnectionString`: *(Opcional)* URL de conexión a un servidor Redis gestionado para habilitar la caché distribuida y manejo de sesión (ej. `redis-xxxxx.c11.us-east-1-2.ec2.cloud.redislabs.com:12345,password=xxxxxx`).

### Verificación Online
Una vez desplegado en Render, deberías poder realizar y verificar las siguientes funciones:
- **Login**: Iniciar sesión con el usuario Coordinador (`admin@universidad.edu` / `Password123!`).
- **Catálogo e Inscripción**: Visualizar cursos activos y matricularte (requiere crear una cuenta de estudiante e iniciar sesión).
- **Panel Coordinador**: Accesible solo si tu cuenta posee el rol de coordinador (`/Coordinador`). Permite CRUD de cursos y gestión de estados en las matrículas.
