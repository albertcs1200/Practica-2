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

URL en Render.com: _[Por definir]_

(El despliegue está configurado para Web Services en Render usando variables de entorno).
