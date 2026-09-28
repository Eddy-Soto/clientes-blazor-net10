# WebEmpresa — Blazor .NET 10 conectado a la API de clientes

Frontend para la tarea de administración de clientes. Usa InteractiveServer, HttpClient inyectado, modales Bootstrap y validación con DataAnnotations y clases Bootstrap.

## API real
Repositorio: https://github.com/Eddy-Soto/api-clientes-dotnet10-mysql
Contrato revisado en el commit `96bbdf8af52c28de08a1906d7a02387220815976`.

Esta versión reemplaza el contrato propuesto de la primera entrega. No usa correo: utiliza CUI, NIT, nombres, apellidos, dirección, teléfono y fecha de nacimiento. La API JSON de ejemplo se retiró de la solución.

La API original sigue siendo un proyecto separado: no se ha modificado ni publicado ningún cambio en su repositorio. El frontend no contiene MySQL, EF Core ni cadenas de conexión a la base de datos.

## Ejecutar

Requisito: SDK .NET 10.

1. Inicia tu API original con MySQL configurado. El perfil `http` del repositorio usa **http://localhost:5092**:
   ```powershell
   dotnet run --project Clientes.Api --launch-profile http
   ```
   Este comando se ejecuta desde el repositorio de la API, después de configurar su conexión y esquema como en tu tarea anterior. No configures la contraseña de MySQL en Blazor.

2. Desde esta carpeta del frontend:
   ```powershell
   dotnet run --project Clientes.Web --launch-profile http
   ```

3. Abre **http://localhost:5080/clientes**. La página Home tiene un enlace al módulo.

El frontend consulta `http://localhost:5092/api/clientes` por defecto, según `Clientes.Web/appsettings.json`.

### Si usas Iniciar-MySQL.ps1 del repositorio original
Ese script fuerza la API al puerto **5080**, aunque launchSettings.json indique 5092. No ejecutes Blazor en el mismo puerto. Puedes iniciar el frontend así:
```powershell
./Iniciar-Blazor.ps1 -ApiUrl http://localhost:5080/ -WebUrl http://localhost:5180
```
Abre **http://localhost:5180/clientes**.

También puedes cambiar `Api:BaseUrl` en appsettings.json, o usar `Api__BaseUrl` como variable de entorno. La URL base debe terminar en / y no incluir api/clientes.

## Archivos para copiar o estudiar
- `Clientes.Web/Program.cs`: registra InteractiveServer y HttpClient.
- `Clientes.Web/Components/Pages/Cliente.razor`: página completa con GET/POST/PUT/DELETE, búsqueda, paginación y modales.
- `Clientes.Contracts/Cliente.cs`: DTO del frontend; mapea explícitamente los nombres JSON.
- `Clientes.Web/Components/App.razor`: carga CSS y JavaScript de Bootstrap y clientes.js.
- `Clientes.Web/wwwroot/clientes.js`: abre y cierra bootstrap.Modal.
- `Clientes.Web/Components/Layout/MainLayout.razor` y `wwwroot/app.css`: menú lateral y aspecto basado en la foto.

Copiar solamente Program.cs y Cliente.razor a un proyecto diferente requiere también el DTO, su referencia de proyecto, Bootstrap y clientes.js. En esta solución ya se incluyen todos los archivos.

## Contrato HTTP exacto

| Operación | Solicitud | Respuesta |
|---|---|---|
| Listar | GET /api/clientes | 200 con arreglo de clientes |
| Consultar uno | GET /api/clientes/{id} | 200 o 404 |
| Crear | POST /api/clientes | 201 con cliente y Location |
| Actualizar | PUT /api/clientes/{id} | 204 |
| Eliminar | DELETE /api/clientes/{id} | 204 |

Cuerpo de POST/PUT:
```json
{
  "cui": "9000000000001",
  "nit": "1234567-8",
  "nombres": "Cliente",
  "apellidos": "Ejemplo",
  "direccion": "Dirección de prueba",
  "telefono": "55551234",
  "fecha_Nacimiento": "1990-01-06"
}
```
Estos son datos ficticios de documentación. GET y POST devuelven además `id_cliente`.
El identificador se usa internamente para PUT/DELETE y no aparece como columna ni como campo editable.

## Validaciones y comportamiento
- CUI obligatorio de 13 dígitos; NIT obligatorio de hasta 20 caracteres.
- Nombres y apellidos obligatorios, máximo 100 caracteres cada uno.
- Dirección obligatoria, máximo 250; teléfono obligatorio, máximo 25 y validación Phone.
- Fecha obligatoria, sin fechas futuras. Se envía yyyy-MM-dd y se muestra dd/MM/yyyy.
- La API vuelve a validar y puede devolver 400. Un CUI repetido devuelve 409 y se muestra un mensaje claro.
- El formulario usa una copia al editar. Cancelar no modifica la fila.
- Modal nativa de Bootstrap; dos columnas en pantallas amplias, una en pantallas pequeñas.
- Buscar filtra por CUI, NIT, nombres, apellidos, dirección y teléfono, ignorando mayúsculas y acentos.
- Paginación de cinco registros; GET devuelve el arreglo completo, por lo que la paginación y búsqueda se hacen en Blazor.
- Después de guardar se consulta GET, se limpia el filtro y se muestra la página que contiene el registro guardado.
- Después de eliminar se consulta GET y se ajusta la página cuando es necesario.
- Se bloquean operaciones duplicadas mientras se espera una respuesta y se muestran errores de conexión.
- El total representa todos los clientes; con una búsqueda se muestra también el número de coincidencias.

## Verificación realizada
```powershell
dotnet build ClientesApp.slnx
dotnet run --project tests/Clientes.ContractChecks
```
- Compilación correcta.
- 12 verificaciones automáticas de serialización y validación contra copias de los DTO originales del commit indicado.
- En navegador: GET, alta, edición, cancelación de eliminación, DELETE, refresco, paginación, búsqueda y error 409.
- Revisión de escritorio a 1440 px y de pantalla estrecha.
- Las pruebas de navegador utilizaron un servidor HTTP temporal con datos ficticios y el contrato original. Ese servidor no forma parte de la entrega ni se activa automáticamente.
- Integración local: el usuario confirmó el funcionamiento con MySQL y mostró un alta exitosa desde Blazor. Las pruebas automáticas de contrato son independientes de la base de datos.
- Advertencia de compilación NU1900: el entorno no pudo consultar la auditoría de vulnerabilidades de NuGet; no impidió compilar.

Las capturas entregadas muestran datos ficticios de esa prueba, no registros recuperados de MySQL.

## Aspectos importantes
Ocultar id_cliente no reemplaza autenticación ni autorización. La API original revisada no incorpora esas funciones. Los puertos HTTP documentados son para desarrollo local.
InteractiveServer llama a la API desde el servidor Blazor, por lo que no hace falta habilitar CORS para estas llamadas.

Si este entorno tiene claves ASP.NET/DPAPI inaccesibles, `Iniciar-Blazor.ps1 -ClavesLocalesDePrueba` usa una carpeta local separada que está excluida de Git. Es solo para desarrollo; esas claves no se cifran automáticamente en disco. No las publiques. En un equipo sin ese problema, usa los comandos normales.

## Repositorios
- Frontend Blazor: https://github.com/Eddy-Soto/clientes-blazor-net10
- API .NET 10 y MySQL: https://github.com/Eddy-Soto/api-clientes-dotnet10-mysql

Para entregar la tarea, utiliza el enlace del frontend. La API se ejecuta por separado siguiendo las instrucciones anteriores.

## Referencias
- API: https://github.com/Eddy-Soto/api-clientes-dotnet10-mysql
- HttpClient en Blazor: https://learn.microsoft.com/en-us/aspnet/core/blazor/call-web-api?view=aspnetcore-10.0
- Validación: https://learn.microsoft.com/en-us/aspnet/core/blazor/forms/validation?view=aspnetcore-10.0


