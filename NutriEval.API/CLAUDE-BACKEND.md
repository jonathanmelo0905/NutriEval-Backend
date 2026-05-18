# CLAUDE.md — NutriEval (Backend)

---

## 0. Instrucciones para la IA (leer siempre primero)

### Comportamiento general
- Antes de escribir código, confirma qué archivo(s) vas a tocar y espera aprobación si son más de 3
- Si una tarea es ambigua, haz una sola pregunta de clarificación antes de proceder
- Al terminar una tarea, indica qué cambió y si hay algo que deba actualizarse en este archivo

### Fase actual: **Fase 5 — Backend v1.0**
- Implementar API REST con ASP.NET Core, PostgreSQL, JWT y Cloudinary
- No implementar funcionalidades de v2.0 ni posteriores aunque parezcan convenientes
- Al completar un ítem, marcarlo como `[x]` en la sección 6

### Contexto crítico: multi-tenant desde el inicio
- **TODAS** las tablas tienen `tenant_id` (uuid)
- **TODOS** los queries filtran por `tenant_id` del JWT
- Un entrenador NUNCA puede ver datos de otro entrenador
- El `tenant_id` se extrae del JWT en cada request — nunca viene del body

### Reglas de código (permanentes)
- Arquitectura en capas: Controllers → Services → Repositories → DbContext
- Controllers: solo reciben, validan y responden — sin lógica de negocio
- Services: toda la lógica de negocio aquí
- Repositories: todo acceso a datos aquí — nunca queries en controllers
- DTOs para todas las entradas y salidas — nunca exponer entidades directamente
- Validación con FluentValidation o DataAnnotations en los DTOs
- Respuestas estandarizadas: `{ success, data, message, errors }`
- Manejo de errores con middleware global de excepciones
- Logs con `ILogger` en servicios — nunca `Console.WriteLine`
- Variables de entorno para todas las credenciales — nunca hardcodeadas
- Commits: Conventional Commits → `feat(modulo): descripción`

---

## 1. Descripción General

**Nombre:** NutriEval API
**Propósito:** Backend REST para la plataforma SaaS NutriEval. Gestiona autenticación multi-tenant, clientes, evaluaciones físicas, medidas corporales, fotos de progreso, rutinas, nutrición, agenda y pagos.
**Repositorio backend:** (por crear en Fase 5)
**Versión actual:** 1.0.0

---

## 2. Stack Tecnológico

| Tecnología | Versión | Uso |
|---|---|---|
| ASP.NET Core Web API | .NET 8 | Framework principal |
| Entity Framework Core | 8.x | ORM |
| PostgreSQL | Latest | Base de datos principal |
| ASP.NET Identity | 8.x | Gestión de usuarios y roles |
| JWT Bearer | Latest | Autenticación stateless |
| Cloudinary SDK | Latest | Almacenamiento fotos de progreso |
| FluentValidation | Latest | Validación de DTOs |
| AutoMapper | Latest | Mapeo entidades ↔ DTOs |
| Swagger / Scalar | Latest | Documentación API |

### Infraestructura
| Servicio | Uso |
|---|---|
| Railway | Hosting backend + PostgreSQL |
| Cloudinary | Almacenamiento imágenes (cloud: dnj3zphoj) |

### Variables de entorno requeridas
```env
DATABASE_URL=postgresql://...
JWT_SECRET=...
JWT_ISSUER=nutrieval-api
JWT_AUDIENCE=nutrieval-app
JWT_EXPIRES_HOURS=24
CLOUDINARY_CLOUD_NAME=dnj3zphoj
CLOUDINARY_API_KEY=...
CLOUDINARY_API_SECRET=...
FRONTEND_URL=https://nutrieval-app.web.app
```

---

## 3. Arquitectura

### Estructura de carpetas

```
NutriEval.API/
├── Controllers/
│   ├── AuthController.cs
│   ├── ClientesController.cs
│   ├── EvaluacionesController.cs
│   ├── MedidasController.cs
│   ├── FotosController.cs
│   ├── SesionesController.cs
│   └── ConfiguracionController.cs
├── Services/
│   ├── Interfaces/
│   │   ├── IAuthService.cs
│   │   ├── IClienteService.cs
│   │   ├── IEvaluacionService.cs
│   │   ├── IMedidasService.cs
│   │   ├── IFotosService.cs
│   │   ├── ISesionService.cs
│   │   └── ICloudinaryService.cs
│   ├── AuthService.cs
│   ├── ClienteService.cs
│   ├── EvaluacionService.cs
│   ├── MedidasService.cs
│   ├── FotosService.cs
│   ├── SesionService.cs
│   └── CloudinaryService.cs
├── Repositories/
│   ├── Interfaces/
│   │   ├── IClienteRepository.cs
│   │   ├── IEvaluacionRepository.cs
│   │   ├── IMedidasRepository.cs
│   │   ├── IFotosRepository.cs
│   │   └── ISesionRepository.cs
│   ├── ClienteRepository.cs
│   ├── EvaluacionRepository.cs
│   ├── MedidasRepository.cs
│   ├── FotosRepository.cs
│   └── SesionRepository.cs
├── Models/
│   ├── Entities/
│   │   ├── Entrenador.cs
│   │   ├── Cliente.cs
│   │   ├── Evaluacion.cs
│   │   ├── MedidaCorporal.cs
│   │   ├── FotoProgreso.cs
│   │   ├── Rutina.cs
│   │   ├── Ejercicio.cs
│   │   ├── PlanNutricional.cs
│   │   ├── Sesion.cs
│   │   ├── Checkin.cs
│   │   ├── Pago.cs
│   │   └── SuscripcionSaas.cs
│   └── DTOs/
│       ├── Auth/
│       ├── Clientes/
│       ├── Evaluaciones/
│       ├── Medidas/
│       ├── Fotos/
│       └── Sesiones/
├── Data/
│   └── NutriEvalDbContext.cs
├── Middleware/
│   ├── ExceptionMiddleware.cs
│   └── TenantMiddleware.cs
├── Extensions/
│   └── ServiceExtensions.cs
├── appsettings.json
├── appsettings.Development.json
└── Program.cs
```

### Patrón de respuesta estándar
```json
{
  "success": true,
  "data": { },
  "message": "Operación exitosa",
  "errors": []
}
```

---

## 4. Base de Datos — Esquema Completo

### Regla fundamental
**Todas las tablas tienen `tenant_id`**. Todos los queries incluyen `WHERE tenant_id = @tenantId`. El `tenant_id` del entrenador ES su `id` — no es un campo separado en la tabla `Entrenadores`.

### Tablas

#### Entrenadores (usuarios con rol entrenador)
```sql
CREATE TABLE entrenadores (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  nombre VARCHAR(100) NOT NULL,
  email VARCHAR(150) UNIQUE NOT NULL,
  password_hash TEXT NOT NULL,
  plan VARCHAR(20) DEFAULT 'trial',        -- trial, basic, pro, enterprise
  trial_ends_at DATE,
  redes_sociales JSONB DEFAULT '{}',       -- { instagram, facebook, tiktok, web }
  activo BOOLEAN DEFAULT true,
  created_at TIMESTAMPTZ DEFAULT NOW()
);
```

#### Clientes
```sql
CREATE TABLE clientes (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  tenant_id UUID NOT NULL REFERENCES entrenadores(id),
  nombre VARCHAR(100) NOT NULL,
  email VARCHAR(150),
  password_hash TEXT,                       -- null hasta que active su cuenta
  fecha_nacimiento DATE,
  sexo VARCHAR(10),                         -- masculino, femenino
  peso_inicial DECIMAL(5,2),
  estatura DECIMAL(5,2),
  objetivo VARCHAR(30),                     -- bajar_grasa, ganar_musculo, recomposicion, rendimiento
  nivel VARCHAR(20),                        -- principiante, intermedio, avanzado
  telefono VARCHAR(20),
  contacto_emergencia JSONB DEFAULT '{}',   -- { nombre, telefono, relacion }
  salud JSONB DEFAULT '{}',                 -- { lesiones, enfermedades, medicamentos, cirugias, restricciones }
  habitos JSONB DEFAULT '{}',              -- { sueno, estres, agua, pasos_diarios }
  parq_completado BOOLEAN DEFAULT false,
  parq_datos JSONB DEFAULT '{}',            -- respuestas PAR-Q
  parq_fecha TIMESTAMPTZ,
  consentimiento_aceptado BOOLEAN DEFAULT false,
  consentimiento_fecha TIMESTAMPTZ,
  fotos_iniciales JSONB DEFAULT '[]',       -- urls cloudinary
  activo BOOLEAN DEFAULT true,
  created_at TIMESTAMPTZ DEFAULT NOW()
);
CREATE INDEX idx_clientes_tenant ON clientes(tenant_id);
```

#### Evaluaciones (nutricionales y físicas)
```sql
CREATE TABLE evaluaciones (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  tenant_id UUID NOT NULL,
  cliente_id UUID NOT NULL REFERENCES clientes(id),
  tipo VARCHAR(30) NOT NULL,                -- nutricional, fisica
  datos_entrada JSONB NOT NULL,             -- inputs del formulario
  resultados JSONB NOT NULL,                -- outputs calculados
  fecha DATE NOT NULL DEFAULT CURRENT_DATE,
  notas TEXT,
  created_at TIMESTAMPTZ DEFAULT NOW()
);
CREATE INDEX idx_evaluaciones_cliente ON evaluaciones(cliente_id);
CREATE INDEX idx_evaluaciones_tenant ON evaluaciones(tenant_id);
```

#### Medidas Corporales (registro histórico)
```sql
CREATE TABLE medidas_corporales (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  tenant_id UUID NOT NULL,
  cliente_id UUID NOT NULL REFERENCES clientes(id),
  peso DECIMAL(5,2),
  porcentaje_grasa DECIMAL(4,2),
  masa_muscular DECIMAL(5,2),
  imc DECIMAL(4,2),
  cintura DECIMAL(5,2),
  cadera DECIMAL(5,2),
  pecho DECIMAL(5,2),
  brazo_derecho DECIMAL(5,2),
  brazo_izquierdo DECIMAL(5,2),
  pierna_derecha DECIMAL(5,2),
  pierna_izquierda DECIMAL(5,2),
  fecha DATE NOT NULL DEFAULT CURRENT_DATE,
  notas TEXT,
  created_at TIMESTAMPTZ DEFAULT NOW()
);
CREATE INDEX idx_medidas_cliente ON medidas_corporales(cliente_id);
```

#### Fotos de Progreso
```sql
CREATE TABLE fotos_progreso (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  tenant_id UUID NOT NULL,
  cliente_id UUID NOT NULL REFERENCES clientes(id),
  url_cloudinary TEXT NOT NULL,
  public_id TEXT NOT NULL,                  -- para eliminar de Cloudinary
  tipo VARCHAR(20) NOT NULL,                -- frontal, lateral_der, lateral_izq, espalda
  fecha DATE NOT NULL DEFAULT CURRENT_DATE,
  mes_referencia VARCHAR(7),                -- formato: 2025-01
  subido_por VARCHAR(20) DEFAULT 'entrenador', -- entrenador, cliente
  created_at TIMESTAMPTZ DEFAULT NOW()
);
CREATE INDEX idx_fotos_cliente ON fotos_progreso(cliente_id);
```

#### Sesiones (agenda)
```sql
CREATE TABLE sesiones (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  tenant_id UUID NOT NULL,
  cliente_id UUID NOT NULL REFERENCES clientes(id),
  fecha_hora TIMESTAMPTZ NOT NULL,
  duracion_min INT DEFAULT 60,
  estado VARCHAR(20) DEFAULT 'programada',  -- programada, completada, cancelada, no_asistio
  notas TEXT,
  created_at TIMESTAMPTZ DEFAULT NOW()
);
CREATE INDEX idx_sesiones_cliente ON sesiones(cliente_id);
CREATE INDEX idx_sesiones_fecha ON sesiones(fecha_hora);
```

#### Rutinas (v2.0 — estructura lista)
```sql
CREATE TABLE rutinas (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  tenant_id UUID NOT NULL,
  cliente_id UUID NOT NULL REFERENCES clientes(id),
  nombre VARCHAR(100) NOT NULL,
  descripcion TEXT,
  dias JSONB DEFAULT '[]',                  -- array de días con ejercicios
  activa BOOLEAN DEFAULT true,
  created_at TIMESTAMPTZ DEFAULT NOW()
);
```

#### Ejercicios (v2.0 — estructura lista)
```sql
CREATE TABLE ejercicios (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  tenant_id UUID,                           -- null = ejercicio global NutriEval
  nombre VARCHAR(100) NOT NULL,
  grupo_muscular VARCHAR(50),
  instrucciones TEXT,
  url_video TEXT,
  errores_comunes TEXT,
  es_global BOOLEAN DEFAULT false,
  created_at TIMESTAMPTZ DEFAULT NOW()
);
```

#### Planes Nutricionales (v2.0 — estructura lista)
```sql
CREATE TABLE planes_nutricionales (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  tenant_id UUID NOT NULL,
  cliente_id UUID NOT NULL REFERENCES clientes(id),
  calorias DECIMAL(6,2),
  proteinas DECIMAL(5,2),
  carbohidratos DECIMAL(5,2),
  grasas DECIMAL(5,2),
  agua DECIMAL(4,2),
  comidas JSONB DEFAULT '[]',               -- array de comidas del día
  activo BOOLEAN DEFAULT true,
  created_at TIMESTAMPTZ DEFAULT NOW()
);
```

#### Check-ins Semanales (v2.0 — estructura lista)
```sql
CREATE TABLE checkins (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  tenant_id UUID NOT NULL,
  cliente_id UUID NOT NULL REFERENCES clientes(id),
  semana DATE NOT NULL,                     -- lunes de la semana
  energia INT CHECK (energia BETWEEN 1 AND 10),
  hambre INT CHECK (hambre BETWEEN 1 AND 10),
  adherencia INT CHECK (adherencia BETWEEN 1 AND 10),
  sueno INT CHECK (sueno BETWEEN 1 AND 10),
  estres INT CHECK (estres BETWEEN 1 AND 10),
  peso_semana DECIMAL(5,2),
  notas_cliente TEXT,
  feedback_entrenador TEXT,
  created_at TIMESTAMPTZ DEFAULT NOW()
);
```

#### Pagos de Clientes (v3.0 — estructura lista)
```sql
CREATE TABLE pagos (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  tenant_id UUID NOT NULL,
  cliente_id UUID NOT NULL REFERENCES clientes(id),
  monto DECIMAL(10,2) NOT NULL,
  moneda VARCHAR(3) DEFAULT 'COP',
  estado VARCHAR(20) DEFAULT 'pendiente',   -- pendiente, pagado, vencido, cancelado
  proveedor VARCHAR(20),                    -- stripe, mercadopago, efectivo
  referencia_externa TEXT,
  vence_en DATE,
  created_at TIMESTAMPTZ DEFAULT NOW()
);
```

#### Suscripciones SaaS (v4.0 — estructura lista)
```sql
CREATE TABLE suscripciones_saas (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  entrenador_id UUID NOT NULL REFERENCES entrenadores(id),
  plan VARCHAR(20) NOT NULL,                -- trial, basic, pro, enterprise
  estado VARCHAR(20) DEFAULT 'activa',      -- activa, vencida, cancelada
  trial_ends_at DATE,
  next_billing DATE,
  stripe_customer_id TEXT,
  stripe_subscription_id TEXT,
  created_at TIMESTAMPTZ DEFAULT NOW()
);
```

---

## 5. Endpoints API v1.0

### Auth
```
POST /api/auth/register-entrenador    → registro entrenador
POST /api/auth/login                  → login (entrenador + cliente)
POST /api/auth/refresh                → renovar JWT
POST /api/auth/logout                 → invalidar token
GET  /api/auth/me                     → perfil del usuario actual
```

### Clientes
```
GET    /api/clientes                  → lista clientes del entrenador
POST   /api/clientes                  → crear cliente
GET    /api/clientes/:id              → detalle cliente
PUT    /api/clientes/:id              → actualizar cliente
DELETE /api/clientes/:id              → desactivar cliente
POST   /api/clientes/:id/invitar      → enviar invitación por email
```

### Evaluaciones
```
GET    /api/clientes/:id/evaluaciones          → historial
POST   /api/clientes/:id/evaluaciones          → crear evaluación
GET    /api/clientes/:id/evaluaciones/:evalId  → detalle
DELETE /api/clientes/:id/evaluaciones/:evalId  → eliminar
```

### Medidas Corporales
```
GET    /api/clientes/:id/medidas       → historial medidas
POST   /api/clientes/:id/medidas       → registrar medidas
DELETE /api/clientes/:id/medidas/:mid  → eliminar registro
```

### Fotos de Progreso
```
GET    /api/clientes/:id/fotos         → fotos del cliente
POST   /api/clientes/:id/fotos         → subir foto (multipart)
DELETE /api/clientes/:id/fotos/:fid    → eliminar foto (Cloudinary + BD)
```

### Sesiones
```
GET    /api/sesiones                   → agenda del entrenador
POST   /api/sesiones                   → crear sesión
PUT    /api/sesiones/:id               → actualizar sesión
DELETE /api/sesiones/:id               → cancelar sesión
GET    /api/clientes/:id/sesiones      → sesiones de un cliente
```

### Configuración
```
GET    /api/configuracion              → config del entrenador
PUT    /api/configuracion/redes        → actualizar redes sociales
```

---

## 6. Roadmap Backend

### ✅ Fase 5 — v1.0 (completada)
- [x] Crear proyecto ASP.NET Core Web API (.NET 8)
- [x] Configurar Entity Framework Core + PostgreSQL
- [x] Crear todas las entidades del esquema
- [x] Configurar ASP.NET Identity + JWT
- [x] Implementar middleware de tenant
- [x] Implementar middleware de excepciones
- [x] Endpoints Auth completos
- [x] Endpoints Clientes completos
- [x] Endpoints Evaluaciones completos
- [x] Endpoints Medidas completos
- [x] Endpoints Fotos + integración Cloudinary
- [x] Endpoints Sesiones completos
- [x] Endpoints Configuración completos
- [x] Configurar CORS para frontend Angular
- [x] Configurar Swagger / Scalar (habilitado en todos los entornos)
- [x] Deploy en Railway con variables de entorno (Dockerfile + railway.toml + PORT + DATABASE_URL → Npgsql + auto-migrate)
- [x] Configurar PostgreSQL en Railway
- [x] `/update-claude-md` al terminar

### ⏳ v2.0
- [ ] Endpoints Rutinas + Ejercicios
- [ ] Endpoints Planes Nutricionales
- [ ] Endpoints Check-ins
- [ ] Integración Open Food Facts API
- [ ] Sistema de invitaciones por email (SendGrid)
- [ ] Notificaciones push (Firebase Cloud Messaging)

### ⏳ v3.0
- [ ] Endpoints Pagos
- [ ] Integración Stripe
- [ ] Integración Mercado Pago
- [ ] Alertas automáticas de vencimiento
- [ ] Jobs programados (Hangfire o similar)
- [ ] Análisis automático de estancamiento

### ⏳ v4.0
- [ ] Endpoints Suscripciones SaaS
- [ ] Sistema de registro público de entrenadores
- [ ] Panel superadmin
- [ ] White-label (subdominio por entrenador)
- [ ] Integración WhatsApp Business API

---

## 7. Decisiones Abiertas

- [ ] Estrategia de refresh tokens (en BD o stateless con blacklist)
- [ ] Sistema de emails (SendGrid vs Resend vs Mailgun)
- [ ] Jobs programados (Hangfire vs Quartz.NET)
- [ ] Estrategia de backups PostgreSQL en Railway
