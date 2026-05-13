# Backend_CMS_Vinculacion
proyecto backend en .NET para el proyecto de CMS de vinculación con la sociedad realizado por los estudiantes de la universidad de guayaquil.
---------------------------------------------------------------------------------------------------------------------------------------------
🚀 Requisitos previos

.NET 9 SDK
SQL Server (o SQL Server Express)
Visual Studio 2022 o VS Code con extensión C#


⚙️ Configuración
1. Clonar el repositorio
bashgit clone https://github.com/EudaimonDev/Backend_CMS_Vinculacion.git
cd Backend_CMS_Vinculacion
git checkout feature/equipo4-backend
2. Configurar la base de datos
En CMSVinculacion.Api/appsettings.json actualiza la cadena de conexión:
json{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TU_SERVIDOR;Database=CMS_Vinculacion;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
3. Configurar JWT
En el mismo appsettings.json verifica la sección JWT:
json{
  "Jwt": {
    "Key": "TU_CLAVE_SECRETA_MINIMO_32_CARACTERES",
    "Issuer": "CMSVinculacion",
    "Audience": "CMSVinculacionClient"
  },
  "LlaveProtector": "TU_LLAVE_PROTECTOR"
}
4. Aplicar migraciones
bashcd CMSVinculacion.Api
dotnet ef database update
5. Levantar el proyecto
bashdotnet run
La API estará disponible en:

https://localhost:7218
Swagger UI: https://localhost:7218/swagger


🔑 Credenciales por defecto
Email:    admin@cms.com
Password: Admin123!

📁 Carpeta de uploads
Las imágenes subidas se guardan en CMSVinculacion.Api/uploads/. Esta carpeta se crea automáticamente al subir la primera imagen.
