# Mejoras propuestas — Proyecto-PrograWeb-Team1

Lista de cambios **pequeños** detectados al revisar el repositorio. No agregan
funcionalidad nueva: corrigen bugs, limpian código muerto y mejoran consistencia
y seguridad. Están ordenados por prioridad. Pídeme cada uno por su número y lo
aplico.

---

## 🔴 Bugs (corregir primero)

### 1. Audience del token usa el Issuer en vez del Audience
`Services/AuthService.cs` (método `GenerateToken`), la línea del `audience`:

```csharp
audience: _configuration["Jwt:Issuer"], // <- usa Issuer
```

Debería ser `_configuration["Jwt:Audience"]`. Hoy "funciona" solo porque en
`appsettings.json` el Issuer y el Audience tienen el mismo valor, pero es un bug
latente: si algún día se diferencian, la validación del token fallará.

### 2. `Id` sin inicializar en los modelos
`Models/Denuncia.cs` y `Models/Experimento.cs` declaran:

```csharp
public string Id { get; set; }   // genera warning de nullable
```

A diferencia del resto de propiedades, `Id` no tiene `= string.Empty;`. Con
`<Nullable>enable</Nullable>` esto produce warning de compilación. Igualar al
patrón del resto.

---

## 🟠 Seguridad

### 3. Clave JWT y credenciales hardcodeadas en `appsettings.json`
`appsettings.json` tiene la clave secreta en texto plano y versionada en git:

```json
"Jwt": { "Key": "esta-es-una-clave-secreta-larga-para-jwt-2024", ... }
```

Moverla a *user-secrets* / variable de entorno y dejar el valor vacío (o
placeholder) en el archivo versionado. Cualquiera con acceso al repo puede
firmar tokens válidos.

### 4. Validar que `Jwt:Key` exista (quitar el `!`)
En `Program.cs` y `AuthService.cs` se usa `_configuration["Jwt:Key"]!`. Si la
clave falta, la app crashea con `NullReferenceException` poco clara. Reemplazar
por una comprobación explícita que lance un error entendible al arrancar.

### 5. Mensaje de login que permite enumeración de usuarios
`AuthService.Login` lanza `"No existe ningun usuario con esa credencial"` cuando
el correo no existe y `"Password incorrecto"` cuando la contraseña falla. Eso
revela qué correos están registrados. Unificar a un único mensaje genérico tipo
`"Credenciales inválidas"`.

### 6. Hash de contraseña sin salt (SHA256 plano)
`HashPasword` usa SHA256 directo, vulnerable a rainbow tables. Mejora pequeña:
usar el `PasswordHasher` integrado de ASP.NET Core o agregar un salt por
usuario. (Cambio acotado, sin librerías nuevas.)

---

## 🟡 Código muerto y limpieza

### 7. Eliminar el modelo `Experimento.cs`
`Models/Experimento.cs` no se usa en ningún lado: fue reemplazado por
`Denuncia.cs`. Además tiene typos (`Tittle`, `fracasp`). Borrarlo.

### 8. Manejo de excepciones demasiado genérico
Los controllers (`AuthController`, `DenunciaController`) atrapan
`catch (Exception)` y devuelven siempre `400 BadRequest`. Un fallo real de
Firestore se reporta como error del cliente. Diferenciar errores de validación
(400) de errores internos (500).

### 9. Typo en el nombre del método `HashPasword`
`AuthService.HashPasword` → `HashPassword`. Renombrar (afecta 2 llamadas en el
mismo archivo).

---

## 🟢 Consistencia y calidad

### 10. Inconsistencia de nombre en `.gitignore` vs. código
`.gitignore` ignora `Config/Firebase-Credentials.json` (F mayúscula) pero
`FirebaseService.cs` lee `firebase-credentials.json` (minúscula). En Windows no
falla, pero en Linux/CI sí. Unificar el nombre.

### 11. Validación de DTOs con DataAnnotations
`RegisterDto`, `LoginDto` y `CrearDenunciaDto` no validan nada. Con
`[ApiController]` activo, basta agregar `[Required]`, `[EmailAddress]`,
`[MinLength]` para que .NET rechace datos inválidos automáticamente (400) sin
escribir código extra.

### 12. `project-id` y nombre de colección hardcodeados
`FirebaseService.cs` tiene `"proyecto-847"` fijo en código. Moverlo a
`appsettings.json` (`Firebase:ProjectId`) para no recompilar al cambiar de
entorno.

### 13. Falta un `README.md` del proyecto
No hay README. Agregar uno con: descripción, requisitos (.NET 10, credenciales
Firebase), cómo correr (`dotnet run`), y los endpoints disponibles.

### 14. Indentación/formato en `GenerateToken`
El bloque de `GenerateToken` en `AuthService.cs` tiene indentación irregular
(sangrías crecientes). Reformatear para legibilidad (cambio cosmético).

---

### Cómo usar esta lista
Dime, por ejemplo, **"hacé el 1 y el 2"** y aplico esos cambios. Recomiendo
empezar por los bugs (1–2), luego seguridad (3–6) y por último la limpieza.
