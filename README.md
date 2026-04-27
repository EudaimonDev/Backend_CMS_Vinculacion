# Backend_CMS_Vinculacion
proyecto backend en .NET para el proyecto de CMS de vinculación con la sociedad realizado por los estudiantes de la universidad de guayaquil.
---------------------------------------------------------------------------------------------------------------------------------------------
## Requisitos previos

- Visual Studio 2022 o superior
- .NET 9 SDK
- SQL Server Express
- SQL Server Management Studio (SSMS)

---

## 1. Clonar el repositorio

```bash
git clone https://github.com/EudaimonDev/Backend_CMS_Vinculacion.git
cd Backend_CMS_Vinculacion
```

---

## 2. Configurar appsettings.json

Ubicación: `CMSVinculacion.Api/CMSVinculacion.Api/appsettings.json`

Reemplazar el valor de `defaultConnection` con el nombre exacto de tu instancia de SQL Server. Para verificarlo, abrir SSMS y copiar el nombre del servidor que aparece al conectarse.
(NO MODIFICAR EL KEY DE JWT)

```json
{
  "ConnectionStrings": {
    "defaultConnection": "Server=TU_SERVIDOR\\TU_INSTANCIA;Database=CMSVinculacion;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Jwt": {
    "Key": "CAMBIAR_POR_CLAVE_SECRETA_MIN_32_CHARS_AQUI",
    "Issuer": "CMSVinculacion.Api",
    "Audience": "CMSVinculacion.Client"
  },
  "CORSDomainClients": [
    "http://localhost:4200",
    "http://localhost:3000"
  ],
  "LlaveProtector": "CMSVinculacion_LlaveProtector_2026",
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

> Ejemplo de cadena de conexión: `Server=localhost\\SQLEXPRESS01;Database=CMSVinculacion;Trusted_Connection=True;TrustServerCertificate=True`

---

## 3. Crear la base de datos

Abrir la **Consola del Administrador de Paquetes** en Visual Studio:

**Herramientas → Administrador de paquetes NuGet → Consola del Administrador de paquetes**

Ejecutar:

```powershell
Update-Database -Project CMSVinculacion.Infrastructure -StartupProject CMSVinculacion.Api
```

Si aparece el error `Ya hay un objeto con el nombre 'X' en la base de datos`, ejecutar esto en SSMS y volver a correr el comando anterior:

```sql
USE CMSVinculacion;
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES ('20260414044014_MigracionInicial', '9.0.0');
```

---

## 4. Insertar datos iniciales

Abrir SSMS, conectarse al servidor y ejecutar:

```sql
USE CMSVinculacion;

-- Rol administrador
INSERT INTO SEG.Roles (RoleName, Description)
VALUES ('Admin', 'Administrador del sistema');

-- Usuario administrador (password: Admin123!)
INSERT INTO SEG.Users (Username, Email, PasswordHash, RoleId, IsActive, CreatedAt)
VALUES (
    'admin',
    'admin@cms.com',
    '$2a$11$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2uheWG/igi.',
    1, 1, GETUTCDATE()
);

-- Estados de artículo
INSERT INTO CON.ArticleStatus (StatusName) VALUES ('Draft');
INSERT INTO CON.ArticleStatus (StatusName) VALUES ('Published');

-- Categorías base
INSERT INTO CAT.Categories (Name, Slug, Description, IsPublicVisible, IsActive, CreatedAt)
VALUES
('Investigación', 'investigacion', 'Artículos de investigación', 1, 1, GETUTCDATE()),
('Cultura',       'cultura',       'Artículos de cultura',       1, 1, GETUTCDATE()),
('Tecnología',    'tecnologia',    'Artículos de tecnología',    1, 1, GETUTCDATE()),
('Eventos',       'eventos',       'Artículos de eventos',       1, 1, GETUTCDATE()),
('Proyectos',     'proyectos',     'Artículos de proyectos',     1, 1, GETUTCDATE());
```

---

## 5. Levantar el proyecto

Presionar **F5** en Visual Studio o hacer clic en el botón `https`.

El proyecto levanta en:
https://localhost:{puerto}/swagger

El puerto exacto aparece en la barra del navegador al levantar.
