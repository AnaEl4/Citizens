# Citizens API

API desarrollada en .NET para la gestión de ciudadanos, implementando operaciones CRUD, almacenamiento en CSV y consumo de una API externa.

---

## 🚀 Tecnologías utilizadas

* .NET Web API
* C#
* CSV como almacenamiento
* Serilog (logging)
* API externa: https://api.restful-api.dev/objects

---

## 📌 Funcionalidades

* Crear ciudadano
* Obtener todos los ciudadanos
* Obtener ciudadano por CI
* Actualizar ciudadano (solo FirstName y LastName)
* Eliminar ciudadano
* Asignación automática de:

  * BloodGroup (aleatorio)
  * PersonalAsset (API externa con fallback)

---

## ⚙️ Configuración

Archivo `appsettings.Development.json`:

```json
"Data": {
  "Location": "C:/ruta/del/archivo.csv"
},
"ExternalServices": {
  "ObjectsApi": {
    "BaseUrl": "https://api.restful-api.dev/objects"
  }
},
"Serilog": {
    "Using":  [ "Serilog.Sinks.Console", "Serilog.Sinks.File" ],
    "MinimumLevel": "Debug",
    "WriteTo": [
      { 
        "Name": "File", 
        "Args": { 
          "path": "log.log", 
          "rollingInterval": "Day"
        } 
      },
      {
        "Name": "Console"
      }
    ]
}

```

---

## ▶️ Ejecución

```bash
dotnet run
```

Swagger disponible en:

```
https://localhost:9070/swagger
```

---

## 📥 Ejemplo de request (POST)

```json
{
  "ci": 123456,
  "firstName": "Ana",
  "lastName": "Lopez"
}
```

---

## 📤 Ejemplo de respuesta

```json
{
  "ci": 123456,
  "firstName": "Ana",
  "lastName": "Lopez",
  "bloodGroup": "B+",
  "personalAsset": "Google Pixel 6 Pro"
}
```

---

## ⚠️ Manejo de errores

Si la API externa falla:

* Se asigna `"Objeto inválido"` como `PersonalAsset`
* La aplicación continúa funcionando sin interrupciones

---

## 🔔 Logging

Se implementó Serilog para:

* Registrar creación, actualización y eliminación de ciudadanos
* Registrar errores de la API externa
* Guardar logs en archivo y consola

---

# 📝 12 Factor App Principles

## 1. Codebase

Se utiliza un solo repositorio de Git para todo el proyecto, manteniendo un único codebase versionado.

---

## 2. Dependencies

Las dependencias están declaradas explícitamente en el archivo de configuración del proyecto (.csproj), el cual actúa como manifiesto de la aplicación.

Se utilizan paquetes NuGet como:

- Serilog (logging)

Esto asegura que no existan dependencias implícitas y que el entorno pueda reproducirse fácilmente.

---

## 3. Config

La configuración está separada del código usando `appsettings.Development.json`.

Incluye:

* Ruta del archivo CSV
* URL de la API externa
* Configuración de logging

---

## 4. Backing Services

La aplicación utiliza una API externa (Objects API) para obtener el PersonalAsset de cada ciudadano.

Este servicio es tratado como un recurso independiente porque:

- Se accede mediante HTTP
- Su configuración se encuentra fuera del código (appsettings.Development.json)
- Puede ser reemplazado fácilmente sin modificar la lógica interna
- La aplicación no depende de su disponibilidad, ya que implementa un fallback ("Objeto inválido")

Esto cumple con el principio de desacoplamiento de servicios externos.

---

## 5. Build, Release, Run

La aplicación sigue el principio de separación entre build, release y run:

- **Build:** Se realiza mediante `dotnet build`, compilando el código fuente en artefactos ejecutables.

- **Release:** Se gestiona mediante Git, donde cada cambio es versionado y puede representar una versión específica del sistema.

- **Run:** La aplicación se ejecuta con `dotnet run`, exponiendo la API en un puerto HTTP.

Esto permite separar la construcción, versionado y ejecución del sistema.

---

## 6. Processes

La aplicación es stateless, ya que no depende del estado en memoria.

Los datos persistentes se almacenan en un archivo CSV.

---

## 7. Port Binding

La API se expone a través de un puerto HTTPS:

```
https://localhost:9070
```

---

## 8. Concurrency

Actualmente no se implementa concurrencia.

Sin embargo, la aplicación podría escalar en el futuro mediante:

* Ejecución de múltiples instancias del servicio
* Uso de contenedores (Docker)
* Balanceadores de carga

Esto permitiría manejar múltiples solicitudes simultáneamente.

---

## 9. Disposability

La aplicación inicia rápidamente y maneja errores sin detenerse.

Ejemplo:

* Si falla la API externa, se usa un valor por defecto.

---

## 10. Dev / Prod Parity

El entorno de desarrollo y producción son similares ya que:

* Se utiliza la misma configuración base
* No hay dependencias ocultas

---

## 11. Logs

Los logs se manejan como flujos de eventos usando Serilog.

Ejemplos:

* INFO: Citizen created
* WARNING: Citizen not found
* ERROR: External API failure

---

## 12. Admin Processes

En este proyecto no se implementan procesos administrativos independientes debido a su tamaño y simplicidad.

Sin embargo, se identifican posibles tareas administrativas como:

- Limpieza o edición manual del archivo CSV
- Mantenimiento de datos
- Reprocesamiento de información

Estas tareas podrían implementarse en el futuro como scripts independientes o herramientas externas.

Además, el uso de logging (Serilog) permite monitorear el sistema, lo cual facilita tareas administrativas indirectamente.

---

## 👤 Autor

* Ana Elena La Ruta Rosas
