# CLAUDE-BACKEND.md — Fuente de verdad del backend NutriEval.API

> **Última actualización:** 2026-05-24 (regla solapamiento sesiones)  
> **Estado:** Revisado contra el código real. Toda la información aquí es fiel al código fuente.

---

## 1. Stack técnico

| Capa | Tecnología |
|------|-----------|
| Framework | ASP.NET Core 8 (minimal + controllers) |
| ORM | Entity Framework Core 8 (PostgreSQL / Npgsql) |
| Auth | JWT Bearer — access token (24 h) + refresh token (7 días) |
| Validación | FluentValidation (validators en el mismo archivo del DTO) |
| Contraseñas entrenador | `IPasswordHasher<Entrenador>` (ASP.NET Identity hasher) |
| Contraseñas cliente | BCrypt.Net |
| Imágenes | Cloudinary |
| Respuesta estándar | `ApiResponse<T>` wrapper en **todas** las respuestas |
| Multi-tenant | `TenantMiddleware` inyecta `TenantId` desde claim `sub` del JWT |

---

## 2. Envelope de respuesta estándar

Todas las respuestas usan `ApiResponse<T>`:

```json
{
  "success": true | false,
  "data": <T> | null,
  "message": "string",
  "errors": ["string"]
}
```

En los ejemplos de abajo, `data` es el objeto documentado. Los errores de validación devuelven `success: false` con el array `errors` poblado.

---

## 3. Entidades del dominio (campos exactos)

### Entrenador
| Campo | Tipo | Default |
|-------|------|---------|
| Id | Guid | — |
| Nombre | string | — |
| Email | string | — |
| PasswordHash | string | — |
| Plan | string | `"trial"` |
| TrialEndsAt | DateOnly? | null |
| RedesSociales | string (JSON) | `"{}"` |
| Activo | bool | `true` |
| CreatedAt | DateTimeOffset | UtcNow |

### Cliente
| Campo | Tipo | Default |
|-------|------|---------|
| Id | Guid | — |
| TenantId | Guid | — |
| Nombre | string | — |
| Email | string? | null |
| PasswordHash | string? | null |
| FechaNacimiento | DateOnly? | null |
| Sexo | string? | null |
| PesoInicial | decimal? | null |
| Estatura | decimal? | null |
| Objetivo | string? | null |
| Nivel | string? | null |
| Telefono | string? | null |
| ContactoEmergencia | string (JSON) | `"{}"` |
| Salud | string (JSON) | `"{}"` |
| Habitos | string (JSON) | `"{}"` |
| ParqCompletado | bool | `false` |
| ParqDatos | string (JSON) | `"{}"` |
| ParqFecha | DateTimeOffset? | null |
| ConsentimientoAceptado | bool | `false` |
| ConsentimientoFecha | DateTimeOffset? | null |
| FotosIniciales | string (JSON array) | `"[]"` |
| Activo | bool | `true` |
| CreatedAt | DateTimeOffset | UtcNow |

### Evaluacion
| Campo | Tipo | Default |
|-------|------|---------|
| Id | Guid | — |
| TenantId | Guid | — |
| ClienteId | Guid | — |
| Tipo | string | — (`"nutricional"` \| `"fisica"`) |
| DatosEntrada | string (JSON) | `"{}"` |
| Resultados | string (JSON) | `"{}"` |
| Fecha | DateOnly | hoy UTC |
| Notas | string? | null |
| CreatedAt | DateTimeOffset | UtcNow |

### MedidaCorporal
| Campo | Tipo |
|-------|------|
| Id | Guid |
| TenantId | Guid |
| ClienteId | Guid |
| Peso | decimal? |
| PorcentajeGrasa | decimal? |
| MasaMuscular | decimal? |
| Imc | decimal? |
| Cintura | decimal? |
| Cadera | decimal? |
| Pecho | decimal? |
| BrazoDerecho | decimal? |
| BrazoIzquierdo | decimal? |
| PiernaDerecha | decimal? |
| PiernaIzquierda | decimal? |
| Fecha | DateOnly (hoy UTC) |
| Notas | string? |
| CreatedAt | DateTimeOffset |

### FotoProgreso
| Campo | Tipo | Default |
|-------|------|---------|
| Id | Guid | — |
| TenantId | Guid | — |
| ClienteId | Guid | — |
| UrlCloudinary | string | — |
| PublicId | string | — |
| Tipo | string | — (`"frontal"` \| `"lateral_der"` \| `"lateral_izq"` \| `"espalda"`) |
| Fecha | DateOnly | hoy UTC |
| MesReferencia | string? | null (formato `"YYYY-MM"`) |
| SubidoPor | string | `"entrenador"` |
| CreatedAt | DateTimeOffset | UtcNow |

### Sesion
| Campo | Tipo | Default |
|-------|------|---------|
| Id | Guid | — |
| TenantId | Guid | — |
| ClienteId | Guid | — |
| FechaHora | DateTimeOffset | — |
| DuracionMin | int | `60` |
| Estado | string | `"programada"` |
| Notas | string? | null |
| CreatedAt | DateTimeOffset | UtcNow |

Estados válidos de sesión: `programada`, `completada`, `cancelada`, `no_asistio`

### Rutina (entidad sin controller aún)
| Campo | Tipo | Default |
|-------|------|---------|
| Id | Guid | — |
| TenantId | Guid | — |
| ClienteId | Guid | — |
| Nombre | string | — |
| Descripcion | string? | null |
| Dias | string (JSON array) | `"[]"` |
| Activa | bool | `true` |
| CreatedAt | DateTimeOffset | UtcNow |

### Ejercicio (entidad sin controller aún)
| Campo | Tipo | Default |
|-------|------|---------|
| Id | Guid | — |
| TenantId | Guid? | null (null = global) |
| Nombre | string | — |
| GrupoMuscular | string? | null |
| Instrucciones | string? | null |
| UrlVideo | string? | null |
| ErroresComunes | string? | null |
| EsGlobal | bool | `false` |
| CreatedAt | DateTimeOffset | UtcNow |

### PlanNutricional (entidad sin controller aún)
| Campo | Tipo | Default |
|-------|------|---------|
| Id | Guid | — |
| TenantId | Guid | — |
| ClienteId | Guid | — |
| Calorias | decimal? | null |
| Proteinas | decimal? | null |
| Carbohidratos | decimal? | null |
| Grasas | decimal? | null |
| Agua | decimal? | null |
| Comidas | string (JSON array) | `"[]"` |
| Activo | bool | `true` |
| CreatedAt | DateTimeOffset | UtcNow |

### Checkin (entidad sin controller aún)
| Campo | Tipo |
|-------|------|
| Id | Guid |
| TenantId | Guid |
| ClienteId | Guid |
| Semana | DateOnly |
| Energia | int? (1-10) |
| Hambre | int? (1-10) |
| Adherencia | int? (1-10) |
| Sueno | int? (1-10) |
| Estres | int? (1-10) |
| PesoSemana | decimal? |
| NotasCliente | string? |
| FeedbackEntrenador | string? |
| CreatedAt | DateTimeOffset |

### Pago (entidad sin controller aún)
| Campo | Tipo | Default |
|-------|------|---------|
| Id | Guid | — |
| TenantId | Guid | — |
| ClienteId | Guid | — |
| Monto | decimal | — |
| Moneda | string | `"COP"` |
| Estado | string | `"pendiente"` |
| Proveedor | string? | null |
| ReferenciaExterna | string? | null |
| VenceEn | DateOnly? | null |
| CreatedAt | DateTimeOffset | UtcNow |

### SuscripcionSaas (entidad sin controller aún)
| Campo | Tipo | Default |
|-------|------|---------|
| Id | Guid | — |
| EntrenadorId | Guid | — |
| Plan | string | — |
| Estado | string | `"activa"` |
| TrialEndsAt | DateOnly? | null |
| NextBilling | DateOnly? | null |
| StripeCustomerId | string? | null |
| StripeSubscriptionId | string? | null |
| CreatedAt | DateTimeOffset | UtcNow |

---

## 4. DTOs (exactos, tal como están en el código)

### Auth DTOs

#### `RegisterEntrenadorDto` (request)
```json
{
  "nombre": "string (required, max 100)",
  "email": "string (required, email válido, max 150)",
  "password": "string (required, min 8 chars, ≥1 mayúscula, ≥1 número)"
}
```

#### `LoginDto` (request)
```json
{
  "email": "string (required)",
  "password": "string (required)"
}
```

#### `RefreshTokenRequestDto` (request)
```json
{
  "refreshToken": "string (required)"
}
```

#### `AuthResponseDto` (response de register, login, refresh)
```json
{
  "accessToken": "string",
  "refreshToken": "string",
  "tipo": "string (\"entrenador\" | \"cliente\")",
  "usuario": {
    "id": "guid",
    "tenantId": "guid",
    "nombre": "string",
    "email": "string",
    "plan": "string | null"
  }
}
```

> **Nota:** Para clientes, `usuario.plan` siempre es `null`.  
> Para entrenadores, `usuario.plan` = `"trial"` (u otro valor de plan).

#### `MeDto` (response de GET /auth/me)
```json
{
  "id": "guid",
  "tenantId": "guid",
  "nombre": "string",
  "email": "string",
  "tipo": "string (\"entrenador\" | \"cliente\")",
  "plan": "string | null"
}
```

---

### Clientes DTOs

#### `CreateClienteDto` (request)
```json
{
  "nombre": "string (required, max 100)",
  "email": "string? (email válido, max 150)",
  "fechaNacimiento": "DateOnly? (YYYY-MM-DD)",
  "sexo": "string? (\"Masculino\" | \"Femenino\" | \"Otro\")",
  "pesoInicial": "decimal? (> 0)",
  "estatura": "decimal? (> 0)",
  "objetivo": "string? (\"bajar_grasa\" | \"ganar_musculo\" | \"recomposicion\" | \"rendimiento\")",
  "nivel": "string? (\"principiante\" | \"intermedio\" | \"avanzado\")",
  "telefono": "string?",
  "contactoEmergencia": "object?",
  "salud": "object?",
  "habitos": "object?",
  "passwordTemporal": "string? (min 6 chars)"
}
```

> **⚠️ Discrepancia:** `CreateClienteValidator` acepta `Sexo` como `"Masculino"` / `"Femenino"` / `"Otro"` (con mayúscula).  
> `UpdateClienteValidator` acepta `sexo` como `"masculino"` / `"femenino"` (minúscula, sin `"Otro"`). **Inconsistencia real en el código.**

#### `UpdateClienteDto` (request) — todos los campos son opcionales
```json
{
  "nombre": "string?",
  "email": "string?",
  "fechaNacimiento": "DateOnly?",
  "sexo": "string? (\"masculino\" | \"femenino\")",
  "pesoInicial": "decimal? (> 0)",
  "estatura": "decimal? (> 0)",
  "objetivo": "string?",
  "nivel": "string?",
  "telefono": "string?",
  "contactoEmergencia": "object?",
  "salud": "object?",
  "habitos": "object?",
  "activo": "bool?"
}
```

#### `ClienteListItemDto` (response de GET /clientes)
```json
{
  "id": "guid",
  "nombre": "string",
  "email": "string | null",
  "objetivo": "string | null",
  "nivel": "string | null",
  "pesoInicial": "decimal | null",
  "estatura": "decimal | null",
  "activo": "bool",
  "createdAt": "DateTimeOffset",

  // Campos calculados de onboarding (calculados en la misma query SQL)
  "tieneFotos": "bool",        // true si el cliente tiene ≥1 FotoProgreso
  "tieneEvaluaciones": "bool", // true si el cliente tiene ≥1 Evaluacion
  "tieneSesiones": "bool",     // true si el cliente tiene ≥1 Sesion
  "tieneSalud": "bool",        // true si Salud ≠ "{}" y ≠ null
  "tieneHabitos": "bool"       // true si Habitos ≠ "{}" y ≠ null
}
```

#### `ClienteDetalleDto` (response de GET /clientes/{id}, POST /clientes, PUT /clientes/{id})
```json
{
  "id": "guid",
  "tenantId": "guid",
  "nombre": "string",
  "email": "string | null",
  "fechaNacimiento": "DateOnly | null",
  "sexo": "string | null",
  "pesoInicial": "decimal | null",
  "estatura": "decimal | null",
  "objetivo": "string | null",
  "nivel": "string | null",
  "telefono": "string | null",
  "contactoEmergencia": "object | null",
  "salud": "object | null",
  "habitos": "object | null",
  "parqCompletado": "bool",
  "parqDatos": "object | null",
  "parqFecha": "DateTimeOffset | null",
  "consentimientoAceptado": "bool",
  "consentimientoFecha": "DateTimeOffset | null",
  "fotosIniciales": "object | null",
  "activo": "bool",
  "createdAt": "DateTimeOffset"
}
```

> **Nota de serialización:** Los campos JSON (`contactoEmergencia`, `salud`, `habitos`, `parqDatos`, `fotosIniciales`) se deserializan desde la BD y devuelven `null` si están vacíos (`"{}"` o `"[]"`).

---

### Evaluaciones DTOs

#### `CreateEvaluacionDto` (request)
```json
{
  "tipo": "string (required: \"nutricional\" | \"fisica\")",
  "datosEntrada": "object (required)",
  "resultados": "object (required)",
  "fecha": "DateOnly? (default: hoy UTC)",
  "notas": "string? (max 2000)"
}
```

#### `EvaluacionListItemDto` (response de GET lista)
```json
{
  "id": "guid",
  "clienteId": "guid",
  "tipo": "string",
  "fecha": "DateOnly",
  "notas": "string | null",
  "createdAt": "DateTimeOffset"
}
```

#### `EvaluacionDetalleDto` (response de GET detalle y POST)
```json
{
  "id": "guid",
  "tenantId": "guid",
  "clienteId": "guid",
  "tipo": "string",
  "datosEntrada": "object | null",
  "resultados": "object | null",
  "fecha": "DateOnly",
  "notas": "string | null",
  "createdAt": "DateTimeOffset"
}
```

---

### Medidas DTOs

#### `CreateMedidaDto` (request) — al menos un campo numérico requerido
```json
{
  "peso": "decimal? (> 0)",
  "porcentajeGrasa": "decimal? (0-100)",
  "masaMuscular": "decimal? (> 0)",
  "imc": "decimal? (> 0)",
  "cintura": "decimal? (> 0)",
  "cadera": "decimal? (> 0)",
  "pecho": "decimal? (> 0)",
  "brazoDerecho": "decimal? (> 0)",
  "brazoIzquierdo": "decimal? (> 0)",
  "piernaDerecha": "decimal? (> 0)",
  "piernaIzquierda": "decimal? (> 0)",
  "fecha": "DateOnly? (default: hoy UTC)",
  "notas": "string? (max 2000)"
}
```

#### `MedidaDto` (response de GET lista y POST)
```json
{
  "id": "guid",
  "tenantId": "guid",
  "clienteId": "guid",
  "peso": "decimal | null",
  "porcentajeGrasa": "decimal | null",
  "masaMuscular": "decimal | null",
  "imc": "decimal | null",
  "cintura": "decimal | null",
  "cadera": "decimal | null",
  "pecho": "decimal | null",
  "brazoDerecho": "decimal | null",
  "brazoIzquierdo": "decimal | null",
  "piernaDerecha": "decimal | null",
  "piernaIzquierda": "decimal | null",
  "fecha": "DateOnly",
  "notas": "string | null",
  "createdAt": "DateTimeOffset"
}
```

---

### Fotos DTOs

#### `UploadFotoDto` (request — multipart/form-data)
```
archivo:       File (required, max 10 MB, extensión: .jpg/.jpeg/.png/.webp)
tipo:          string (required: "frontal" | "lateral_der" | "lateral_izq" | "espalda")
fecha:         DateOnly? (YYYY-MM-DD)
mesReferencia: string? (formato "YYYY-MM")
```

#### `FotoDto` (response de GET lista y POST)
```json
{
  "id": "guid",
  "tenantId": "guid",
  "clienteId": "guid",
  "urlCloudinary": "string",
  "publicId": "string",
  "tipo": "string",
  "fecha": "DateOnly",
  "mesReferencia": "string | null",
  "subidoPor": "string",
  "createdAt": "DateTimeOffset"
}
```

---

### Sesiones DTOs

#### `CreateSesionDto` (request)
```json
{
  "clienteId": "guid (required)",
  "fechaHora": "DateTimeOffset (required)",
  "duracionMin": "int (default 60, 1-480)",
  "notas": "string? (max 2000)"
}
```

#### `UpdateSesionDto` (request) — todos opcionales
```json
{
  "fechaHora": "DateTimeOffset?",
  "duracionMin": "int? (1-480)",
  "estado": "string? (\"programada\" | \"completada\" | \"cancelada\" | \"no_asistio\")",
  "notas": "string? (max 2000)"
}
```

#### `SesionDto` (response de GET lista, POST y PUT)
```json
{
  "id": "guid",
  "tenantId": "guid",
  "clienteId": "guid",
  "clienteNombre": "string",
  "fechaHora": "DateTimeOffset",
  "duracionMin": "int",
  "estado": "string",
  "notas": "string | null",
  "createdAt": "DateTimeOffset"
}
```

---

### Configuración DTOs

#### `UpdateRedesDto` (request) — todos opcionales
```json
{
  "instagram": "string? (max 100)",
  "facebook": "string? (max 100)",
  "tiktok": "string? (max 100)",
  "web": "string? (max 200, URL válida)"
}
```

#### `ConfiguracionDto` (response de GET y PUT redes)
```json
{
  "id": "guid",
  "nombre": "string",
  "email": "string",
  "plan": "string",
  "trialEndsAt": "DateOnly | null",
  "redesSociales": "object | null",
  "createdAt": "DateTimeOffset"
}
```

---

## 5. Endpoints implementados — Request y Response reales

> **Autenticación:** Todos los endpoints excepto `POST /api/auth/register-entrenador`, `POST /api/auth/login` y `POST /api/auth/refresh` requieren `Authorization: Bearer <accessToken>`.
>
> **Autorización:** Todos los endpoints (salvo auth) llaman a `RequireEntrenador()` — lanzan 401 si el token es de cliente.

---

### 🔐 Auth — `/api/auth`

#### `POST /api/auth/register-entrenador`
- **Auth:** ❌ Pública
- **Request:** `RegisterEntrenadorDto`
- **Response 201:**
```json
{
  "success": true,
  "data": {
    "accessToken": "string (JWT 24h)",
    "refreshToken": "string (JWT 7d)",
    "tipo": "entrenador",
    "usuario": {
      "id": "guid",
      "tenantId": "guid",
      "nombre": "string",
      "email": "string",
      "plan": "trial"
    }
  },
  "message": "Entrenador registrado exitosamente.",
  "errors": []
}
```

---

#### `POST /api/auth/login`
- **Auth:** ❌ Pública
- **Request:** `LoginDto`
- **Response 200:** igual estructura que register. `tipo` puede ser `"entrenador"` o `"cliente"`. Para clientes, `plan` es `null`.
- **Errores:** 401 si credenciales inválidas.

---

#### `POST /api/auth/refresh`
- **Auth:** ❌ Pública (solo requiere refresh token válido)
- **Request:** `RefreshTokenRequestDto`
- **Response 200:** igual estructura que login.
- **Errores:** 401 si refresh token inválido/expirado.

---

#### `POST /api/auth/logout`
- **Auth:** ✅ Bearer
- **Request:** (sin body)
- **Response 200:**
```json
{
  "success": true,
  "data": null,
  "message": "Sesión cerrada. Elimina el token en el cliente.",
  "errors": []
}
```
> **Nota:** El logout es stateless — el servidor no invalida tokens. El cliente debe eliminar el token local.

---

#### `GET /api/auth/me`
- **Auth:** ✅ Bearer
- **Response 200:**
```json
{
  "success": true,
  "data": {
    "id": "guid",
    "tenantId": "guid",
    "nombre": "string",
    "email": "string",
    "tipo": "entrenador | cliente",
    "plan": "string | null"
  },
  "message": "Operación exitosa",
  "errors": []
}
```

---

### 👥 Clientes — `/api/clientes`

#### `GET /api/clientes`
- **Auth:** ✅ Bearer (solo entrenador)
- **Response 200:**
```json
{
  "success": true,
  "data": [
    {
      "id": "guid",
      "nombre": "string",
      "email": "string | null",
      "objetivo": "string | null",
      "nivel": "string | null",
      "pesoInicial": "decimal | null",
      "estatura": "decimal | null",
      "activo": "bool",
      "createdAt": "DateTimeOffset",
      "tieneFotos": "bool",
      "tieneEvaluaciones": "bool",
      "tieneSesiones": "bool",
      "tieneSalud": "bool",
      "tieneHabitos": "bool"
    }
  ],
  "message": "Operación exitosa",
  "errors": []
}
```

---

#### `POST /api/clientes`
- **Auth:** ✅ Bearer (solo entrenador)
- **Request:** `CreateClienteDto`
- **Response 201:**
```json
{
  "success": true,
  "data": { /* ClienteDetalleDto completo */ },
  "message": "Cliente creado exitosamente.",
  "errors": []
}
```

---

#### `GET /api/clientes/{id}`
- **Auth:** ✅ Bearer (solo entrenador)
- **Response 200:**
```json
{
  "success": true,
  "data": {
    "id": "guid",
    "tenantId": "guid",
    "nombre": "string",
    "email": "string | null",
    "fechaNacimiento": "DateOnly | null",
    "sexo": "string | null",
    "pesoInicial": "decimal | null",
    "estatura": "decimal | null",
    "objetivo": "string | null",
    "nivel": "string | null",
    "telefono": "string | null",
    "contactoEmergencia": "object | null",
    "salud": "object | null",
    "habitos": "object | null",
    "parqCompletado": false,
    "parqDatos": "object | null",
    "parqFecha": "DateTimeOffset | null",
    "consentimientoAceptado": false,
    "consentimientoFecha": "DateTimeOffset | null",
    "fotosIniciales": "object | null",
    "activo": true,
    "createdAt": "DateTimeOffset"
  },
  "message": "Operación exitosa",
  "errors": []
}
```
- **Errores:** 404 si cliente no existe o no pertenece al tenant.

---

#### `PUT /api/clientes/{id}`
- **Auth:** ✅ Bearer (solo entrenador)
- **Request:** `UpdateClienteDto` (todos los campos opcionales)
- **Response 200:** `ClienteDetalleDto` completo
- **Errores:** 404 si no existe; 400 si email duplicado.

---

#### `DELETE /api/clientes/{id}`
- **Auth:** ✅ Bearer (solo entrenador)
- **Comportamiento:** Soft delete (`activo = false`)
- **Response 200:**
```json
{
  "success": true,
  "data": null,
  "message": "Cliente desactivado.",
  "errors": []
}
```

---

#### `POST /api/clientes/{id}/invitar`
- **Auth:** ✅ Bearer (solo entrenador)
- **Response 202:**
```json
{
  "success": true,
  "data": null,
  "message": "Invitación pendiente. El sistema de emails estará disponible en v2.0.",
  "errors": []
}
```
> **Estado:** Placeholder — no envía emails realmente. Valida que el cliente tenga email.

---

### 📊 Evaluaciones — `/api/clientes/{clienteId}/evaluaciones`

#### `GET /api/clientes/{clienteId}/evaluaciones`
- **Auth:** ✅ Bearer (solo entrenador)
- **Response 200:**
```json
{
  "success": true,
  "data": [
    {
      "id": "guid",
      "clienteId": "guid",
      "tipo": "string",
      "fecha": "DateOnly",
      "notas": "string | null",
      "createdAt": "DateTimeOffset"
    }
  ],
  "message": "Operación exitosa",
  "errors": []
}
```

---

#### `POST /api/clientes/{clienteId}/evaluaciones`
- **Auth:** ✅ Bearer (solo entrenador)
- **Request:** `CreateEvaluacionDto`
- **Response 201:**
```json
{
  "success": true,
  "data": {
    "id": "guid",
    "tenantId": "guid",
    "clienteId": "guid",
    "tipo": "string",
    "datosEntrada": "object | null",
    "resultados": "object | null",
    "fecha": "DateOnly",
    "notas": "string | null",
    "createdAt": "DateTimeOffset"
  },
  "message": "Evaluación creada exitosamente.",
  "errors": []
}
```

---

#### `GET /api/clientes/{clienteId}/evaluaciones/{evalId}`
- **Auth:** ✅ Bearer (solo entrenador)
- **Response 200:** `EvaluacionDetalleDto` completo (mismo que POST)

---

#### `DELETE /api/clientes/{clienteId}/evaluaciones/{evalId}`
- **Auth:** ✅ Bearer (solo entrenador)
- **Response 200:**
```json
{
  "success": true,
  "data": null,
  "message": "Evaluación eliminada.",
  "errors": []
}
```

---

### 📏 Medidas — `/api/clientes/{clienteId}/medidas`

#### `GET /api/clientes/{clienteId}/medidas`
- **Auth:** ✅ Bearer (solo entrenador)
- **Response 200:**
```json
{
  "success": true,
  "data": [ /* array de MedidaDto */ ],
  "message": "Operación exitosa",
  "errors": []
}
```

---

#### `POST /api/clientes/{clienteId}/medidas`
- **Auth:** ✅ Bearer (solo entrenador)
- **Request:** `CreateMedidaDto` (al menos un campo numérico)
- **Response 201:** `MedidaDto` completo

---

#### `DELETE /api/clientes/{clienteId}/medidas/{mid}`
- **Auth:** ✅ Bearer (solo entrenador)
- **Response 200:**
```json
{
  "success": true,
  "data": null,
  "message": "Medida eliminada.",
  "errors": []
}
```

---

### 📸 Fotos — `/api/clientes/{clienteId}/fotos`

#### `GET /api/clientes/{clienteId}/fotos`
- **Auth:** ✅ Bearer (solo entrenador)
- **Response 200:**
```json
{
  "success": true,
  "data": [ /* array de FotoDto */ ],
  "message": "Operación exitosa",
  "errors": []
}
```

---

#### `POST /api/clientes/{clienteId}/fotos`
- **Auth:** ✅ Bearer (solo entrenador)
- **Content-Type:** `multipart/form-data`
- **Límite:** 10 MB
- **Request:** `UploadFotoDto` (form fields)
- **Response 201:** `FotoDto` completo

---

#### `DELETE /api/clientes/{clienteId}/fotos/{fid}`
- **Auth:** ✅ Bearer (solo entrenador)
- **Response 200:**
```json
{
  "success": true,
  "data": null,
  "message": "Foto eliminada.",
  "errors": []
}
```

---

### 📅 Sesiones — `/api/sesiones`

#### `GET /api/sesiones`
- **Auth:** ✅ Bearer (solo entrenador)
- **Descripción:** Lista TODAS las sesiones del tenant
- **Response 200:**
```json
{
  "success": true,
  "data": [ /* array de SesionDto */ ],
  "message": "Operación exitosa",
  "errors": []
}
```

---

#### `GET /api/clientes/{clienteId}/sesiones`
- **Auth:** ✅ Bearer (solo entrenador)
- **Descripción:** Lista sesiones de un cliente específico
- **Response 200:** igual al anterior

---

#### `POST /api/sesiones`
- **Auth:** ✅ Bearer (solo entrenador)
- **Request:** `CreateSesionDto`
- **Response 201:**
```json
{
  "success": true,
  "data": {
    "id": "guid",
    "tenantId": "guid",
    "clienteId": "guid",
    "clienteNombre": "string",
    "fechaHora": "DateTimeOffset",
    "duracionMin": 60,
    "estado": "programada",
    "notas": "string | null",
    "createdAt": "DateTimeOffset"
  },
  "message": "Sesión creada exitosamente.",
  "errors": []
}
```
- **Regla de negocio — solapamiento de horario:**  
  Antes de insertar se verifica si el entrenador (`tenantId` del JWT) ya tiene **otra sesión con `estado = "programada"`** cuya `fechaHora` coincide exactamente con la solicitada. Si hay conflicto:
  - **Response 409:**
  ```json
  {
    "success": false,
    "data": null,
    "message": "Ya tienes una sesión programada a esa hora.",
    "errors": ["SESION_SOLAPADA"]
  }
  ```
  > **Nota:** La comparación es exacta (`==`). Dos sesiones con un minuto de diferencia **no** se consideran solapadas. Sólo sesiones en estado `"programada"` participan del chequeo; las `"canceladas"`, `"completadas"` o `"no_asistio"` no bloquean la agenda.

---

#### `PUT /api/sesiones/{id}`
- **Auth:** ✅ Bearer (solo entrenador)
- **Request:** `UpdateSesionDto` (todos opcionales)
- **Response 200:** `SesionDto` completo actualizado

---

#### `DELETE /api/sesiones/{id}`
- **Auth:** ✅ Bearer (solo entrenador)
- **Response 200:**
```json
{
  "success": true,
  "data": null,
  "message": "Sesión cancelada.",
  "errors": []
}
```

---

### ⚙️ Configuración — `/api/configuracion`

#### `GET /api/configuracion`
- **Auth:** ✅ Bearer (solo entrenador)
- **Response 200:**
```json
{
  "success": true,
  "data": {
    "id": "guid",
    "nombre": "string",
    "email": "string",
    "plan": "string",
    "trialEndsAt": "DateOnly | null",
    "redesSociales": "object | null",
    "createdAt": "DateTimeOffset"
  },
  "message": "Operación exitosa",
  "errors": []
}
```

---

#### `PUT /api/configuracion/redes`
- **Auth:** ✅ Bearer (solo entrenador)
- **Request:** `UpdateRedesDto`
- **Response 200:** `ConfiguracionDto` completo actualizado

---

## 6. Tabla resumen de endpoints

| Método | Ruta | Auth | Request DTO | Response DTO |
|--------|------|------|-------------|--------------|
| POST | `/api/auth/register-entrenador` | — | `RegisterEntrenadorDto` | `AuthResponseDto` |
| POST | `/api/auth/login` | — | `LoginDto` | `AuthResponseDto` |
| POST | `/api/auth/refresh` | — | `RefreshTokenRequestDto` | `AuthResponseDto` |
| POST | `/api/auth/logout` | ✅ | — | `null` |
| GET | `/api/auth/me` | ✅ | — | `MeDto` |
| GET | `/api/clientes` | ✅ | — | `ClienteListItemDto[]` |
| POST | `/api/clientes` | ✅ | `CreateClienteDto` | `ClienteDetalleDto` |
| GET | `/api/clientes/{id}` | ✅ | — | `ClienteDetalleDto` |
| PUT | `/api/clientes/{id}` | ✅ | `UpdateClienteDto` | `ClienteDetalleDto` |
| DELETE | `/api/clientes/{id}` | ✅ | — | `null` |
| POST | `/api/clientes/{id}/invitar` | ✅ | — | `null` (202) |
| GET | `/api/clientes/{cid}/evaluaciones` | ✅ | — | `EvaluacionListItemDto[]` |
| POST | `/api/clientes/{cid}/evaluaciones` | ✅ | `CreateEvaluacionDto` | `EvaluacionDetalleDto` |
| GET | `/api/clientes/{cid}/evaluaciones/{eid}` | ✅ | — | `EvaluacionDetalleDto` |
| DELETE | `/api/clientes/{cid}/evaluaciones/{eid}` | ✅ | — | `null` |
| GET | `/api/clientes/{cid}/medidas` | ✅ | — | `MedidaDto[]` |
| POST | `/api/clientes/{cid}/medidas` | ✅ | `CreateMedidaDto` | `MedidaDto` |
| DELETE | `/api/clientes/{cid}/medidas/{mid}` | ✅ | — | `null` |
| GET | `/api/clientes/{cid}/fotos` | ✅ | — | `FotoDto[]` |
| POST | `/api/clientes/{cid}/fotos` | ✅ | `UploadFotoDto` (form) | `FotoDto` |
| DELETE | `/api/clientes/{cid}/fotos/{fid}` | ✅ | — | `null` |
| GET | `/api/sesiones` | ✅ | — | `SesionDto[]` |
| POST | `/api/sesiones` | ✅ | `CreateSesionDto` | `SesionDto` |
| PUT | `/api/sesiones/{id}` | ✅ | `UpdateSesionDto` | `SesionDto` |
| DELETE | `/api/sesiones/{id}` | ✅ | — | `null` |
| GET | `/api/clientes/{cid}/sesiones` | ✅ | — | `SesionDto[]` |
| GET | `/api/configuracion` | ✅ | — | `ConfiguracionDto` |
| PUT | `/api/configuracion/redes` | ✅ | `UpdateRedesDto` | `ConfiguracionDto` |

**Total: 26 endpoints implementados**

---

## 7. Discrepancias y notas importantes

### ⚠️ Inconsistencias reales en el código

1. **Validación de `Sexo` inconsistente:**
   - `CreateClienteValidator`: acepta `"Masculino"` / `"Femenino"` / `"Otro"` (primera letra mayúscula, incluye "Otro")
   - `UpdateClienteValidator`: acepta `"masculino"` / `"femenino"` (minúscula, sin "Otro")
   - **Impacto:** Un cliente creado con `sexo: "Masculino"` no puede actualizarse con el mismo valor.

2. **DELETE de sesión dice "cancelada" pero borra el registro:**
   - El mensaje dice `"Sesión cancelada."` pero el comportamiento real depende de `SesionService.DeleteAsync()` — revisar si es hard o soft delete.

3. **Logout es stateless:**
   - No invalida el token en el servidor. El refresh token sigue siendo válido. El cliente debe borrar ambos tokens localmente.

4. **POST /clientes/{id}/invitar es un stub:**
   - Valida que el cliente tenga email, pero no envía ninguna comunicación. Responde 202 Accepted.

### 📅 Reglas de negocio de Sesiones

| Regla | Endpoint | HTTP | Código de error |
|-------|----------|------|-----------------|
| Entrenador no puede tener dos sesiones `programada` a la misma `fechaHora` exacta | `POST /api/sesiones` | 409 | `SESION_SOLAPADA` |

---

### 📋 Entidades sin controller implementado
Las siguientes entidades existen en la BD pero no tienen endpoints aún:
- `Rutina` + `Ejercicio`
- `PlanNutricional`
- `Checkin`
- `Pago`
- `SuscripcionSaas`

### 🔑 Estructura del JWT
Claims en el access token:
- `sub` = `tenantId` (EntrenadorId para entrenadores, TenantId del cliente)
- `user_id` = id del usuario
- `tipo` = `"entrenador"` | `"cliente"`
- `email` (ClaimTypes.Email)
- `nombre`
- `jti` = UUID único del token

El refresh token agrega: `token_type: "refresh"`

---

## 8. Comportamiento de errores

| Excepción | HTTP Status | Manejada por |
|-----------|-------------|--------------|
| `ArgumentException` | 400 Bad Request | `ExceptionMiddleware` |
| `ConflictException` | 409 Conflict | `ExceptionMiddleware` |
| `KeyNotFoundException` | 404 Not Found | `ExceptionMiddleware` |
| `UnauthorizedAccessException` | 401 Unauthorized | `ExceptionMiddleware` |
| FluentValidation `ValidationException` | 400 Bad Request | `ExceptionMiddleware` |
| Cualquier otra | 500 Internal Server Error | `ExceptionMiddleware` |

> **`ConflictException`** (en `Exceptions/ConflictException.cs`) lleva una propiedad `Codes: IEnumerable<string>` con claves legibles por máquina (e.g. `["SESION_SOLAPADA"]`). El middleware la serializa en el campo `errors` del envelope estándar.

Los errores de validación de FluentValidation retornan:
```json
{
  "success": false,
  "data": null,
  "message": "Errores de validación.",
  "errors": ["mensaje 1", "mensaje 2"]
}
```
