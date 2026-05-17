using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriEval.API.Models.Entities;

namespace NutriEval.API.Data;

public class NutriEvalDbContext : DbContext
{
    public NutriEvalDbContext(DbContextOptions<NutriEvalDbContext> options) : base(options) { }

    public DbSet<Entrenador> Entrenadores => Set<Entrenador>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Evaluacion> Evaluaciones => Set<Evaluacion>();
    public DbSet<MedidaCorporal> MedidasCorporales => Set<MedidaCorporal>();
    public DbSet<FotoProgreso> FotosProgreso => Set<FotoProgreso>();
    public DbSet<Sesion> Sesiones => Set<Sesion>();
    public DbSet<Rutina> Rutinas => Set<Rutina>();
    public DbSet<Ejercicio> Ejercicios => Set<Ejercicio>();
    public DbSet<PlanNutricional> PlanesNutricionales => Set<PlanNutricional>();
    public DbSet<Checkin> Checkins => Set<Checkin>();
    public DbSet<Pago> Pagos => Set<Pago>();
    public DbSet<SuscripcionSaas> SuscripcionesSaas => Set<SuscripcionSaas>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new EntrenadorConfiguration());
        modelBuilder.ApplyConfiguration(new ClienteConfiguration());
        modelBuilder.ApplyConfiguration(new EvaluacionConfiguration());
        modelBuilder.ApplyConfiguration(new MedidaCorporalConfiguration());
        modelBuilder.ApplyConfiguration(new FotoProgresoConfiguration());
        modelBuilder.ApplyConfiguration(new SesionConfiguration());
        modelBuilder.ApplyConfiguration(new RutinaConfiguration());
        modelBuilder.ApplyConfiguration(new EjercicioConfiguration());
        modelBuilder.ApplyConfiguration(new PlanNutricionalConfiguration());
        modelBuilder.ApplyConfiguration(new CheckinConfiguration());
        modelBuilder.ApplyConfiguration(new PagoConfiguration());
        modelBuilder.ApplyConfiguration(new SuscripcionSaasConfiguration());
    }

    // -------------------------------------------------------------------------
    // Entrenadores
    // -------------------------------------------------------------------------
    private sealed class EntrenadorConfiguration : IEntityTypeConfiguration<Entrenador>
    {
        public void Configure(EntityTypeBuilder<Entrenador> entity)
        {
            entity.ToTable("entrenadores");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.Nombre)
                .HasColumnName("nombre")
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Email)
                .HasColumnName("email")
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(e => e.PasswordHash)
                .HasColumnName("password_hash")
                .IsRequired();

            entity.Property(e => e.Plan)
                .HasColumnName("plan")
                .HasMaxLength(20)
                .HasDefaultValue("trial");

            entity.Property(e => e.TrialEndsAt)
                .HasColumnName("trial_ends_at");

            entity.Property(e => e.RedesSociales)
                .HasColumnName("redes_sociales")
                .HasColumnType("jsonb")
                .HasDefaultValueSql("'{}'::jsonb");

            entity.Property(e => e.Activo)
                .HasColumnName("activo")
                .HasDefaultValue(true);

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("NOW()");

            entity.HasIndex(e => e.Email)
                .IsUnique();
        }
    }

    // -------------------------------------------------------------------------
    // Clientes
    // -------------------------------------------------------------------------
    private sealed class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> entity)
        {
            entity.ToTable("clientes");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.TenantId)
                .HasColumnName("tenant_id")
                .IsRequired();

            entity.Property(e => e.Nombre)
                .HasColumnName("nombre")
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Email)
                .HasColumnName("email")
                .HasMaxLength(150);

            entity.Property(e => e.PasswordHash)
                .HasColumnName("password_hash");

            entity.Property(e => e.FechaNacimiento)
                .HasColumnName("fecha_nacimiento");

            entity.Property(e => e.Sexo)
                .HasColumnName("sexo")
                .HasMaxLength(10);

            entity.Property(e => e.PesoInicial)
                .HasColumnName("peso_inicial")
                .HasPrecision(5, 2);

            entity.Property(e => e.Estatura)
                .HasColumnName("estatura")
                .HasPrecision(5, 2);

            entity.Property(e => e.Objetivo)
                .HasColumnName("objetivo")
                .HasMaxLength(30);

            entity.Property(e => e.Nivel)
                .HasColumnName("nivel")
                .HasMaxLength(20);

            entity.Property(e => e.Telefono)
                .HasColumnName("telefono")
                .HasMaxLength(20);

            entity.Property(e => e.ContactoEmergencia)
                .HasColumnName("contacto_emergencia")
                .HasColumnType("jsonb")
                .HasDefaultValueSql("'{}'::jsonb");

            entity.Property(e => e.Salud)
                .HasColumnName("salud")
                .HasColumnType("jsonb")
                .HasDefaultValueSql("'{}'::jsonb");

            entity.Property(e => e.Habitos)
                .HasColumnName("habitos")
                .HasColumnType("jsonb")
                .HasDefaultValueSql("'{}'::jsonb");

            entity.Property(e => e.ParqCompletado)
                .HasColumnName("parq_completado")
                .HasDefaultValue(false);

            entity.Property(e => e.ParqDatos)
                .HasColumnName("parq_datos")
                .HasColumnType("jsonb")
                .HasDefaultValueSql("'{}'::jsonb");

            entity.Property(e => e.ParqFecha)
                .HasColumnName("parq_fecha");

            entity.Property(e => e.ConsentimientoAceptado)
                .HasColumnName("consentimiento_aceptado")
                .HasDefaultValue(false);

            entity.Property(e => e.ConsentimientoFecha)
                .HasColumnName("consentimiento_fecha");

            entity.Property(e => e.FotosIniciales)
                .HasColumnName("fotos_iniciales")
                .HasColumnType("jsonb")
                .HasDefaultValueSql("'[]'::jsonb");

            entity.Property(e => e.Activo)
                .HasColumnName("activo")
                .HasDefaultValue(true);

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("NOW()");

            entity.HasIndex(e => e.TenantId)
                .HasDatabaseName("idx_clientes_tenant");

            entity.HasOne(c => c.Entrenador)
                .WithMany(e => e.Clientes)
                .HasForeignKey(c => c.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    // -------------------------------------------------------------------------
    // Evaluaciones
    // -------------------------------------------------------------------------
    private sealed class EvaluacionConfiguration : IEntityTypeConfiguration<Evaluacion>
    {
        public void Configure(EntityTypeBuilder<Evaluacion> entity)
        {
            entity.ToTable("evaluaciones");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.TenantId)
                .HasColumnName("tenant_id")
                .IsRequired();

            entity.Property(e => e.ClienteId)
                .HasColumnName("cliente_id")
                .IsRequired();

            entity.Property(e => e.Tipo)
                .HasColumnName("tipo")
                .IsRequired()
                .HasMaxLength(30);

            entity.Property(e => e.DatosEntrada)
                .HasColumnName("datos_entrada")
                .HasColumnType("jsonb")
                .IsRequired();

            entity.Property(e => e.Resultados)
                .HasColumnName("resultados")
                .HasColumnType("jsonb")
                .IsRequired();

            entity.Property(e => e.Fecha)
                .HasColumnName("fecha")
                .IsRequired()
                .HasDefaultValueSql("CURRENT_DATE");

            entity.Property(e => e.Notas)
                .HasColumnName("notas");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("NOW()");

            entity.HasIndex(e => e.ClienteId)
                .HasDatabaseName("idx_evaluaciones_cliente");

            entity.HasIndex(e => e.TenantId)
                .HasDatabaseName("idx_evaluaciones_tenant");

            entity.HasOne(e => e.Cliente)
                .WithMany(c => c.Evaluaciones)
                .HasForeignKey(e => e.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    // -------------------------------------------------------------------------
    // Medidas Corporales
    // -------------------------------------------------------------------------
    private sealed class MedidaCorporalConfiguration : IEntityTypeConfiguration<MedidaCorporal>
    {
        public void Configure(EntityTypeBuilder<MedidaCorporal> entity)
        {
            entity.ToTable("medidas_corporales");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.TenantId)
                .HasColumnName("tenant_id")
                .IsRequired();

            entity.Property(e => e.ClienteId)
                .HasColumnName("cliente_id")
                .IsRequired();

            entity.Property(e => e.Peso)
                .HasColumnName("peso")
                .HasPrecision(5, 2);

            entity.Property(e => e.PorcentajeGrasa)
                .HasColumnName("porcentaje_grasa")
                .HasPrecision(4, 2);

            entity.Property(e => e.MasaMuscular)
                .HasColumnName("masa_muscular")
                .HasPrecision(5, 2);

            entity.Property(e => e.Imc)
                .HasColumnName("imc")
                .HasPrecision(4, 2);

            entity.Property(e => e.Cintura)
                .HasColumnName("cintura")
                .HasPrecision(5, 2);

            entity.Property(e => e.Cadera)
                .HasColumnName("cadera")
                .HasPrecision(5, 2);

            entity.Property(e => e.Pecho)
                .HasColumnName("pecho")
                .HasPrecision(5, 2);

            entity.Property(e => e.BrazoDerecho)
                .HasColumnName("brazo_derecho")
                .HasPrecision(5, 2);

            entity.Property(e => e.BrazoIzquierdo)
                .HasColumnName("brazo_izquierdo")
                .HasPrecision(5, 2);

            entity.Property(e => e.PiernaDerecha)
                .HasColumnName("pierna_derecha")
                .HasPrecision(5, 2);

            entity.Property(e => e.PiernaIzquierda)
                .HasColumnName("pierna_izquierda")
                .HasPrecision(5, 2);

            entity.Property(e => e.Fecha)
                .HasColumnName("fecha")
                .IsRequired()
                .HasDefaultValueSql("CURRENT_DATE");

            entity.Property(e => e.Notas)
                .HasColumnName("notas");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("NOW()");

            entity.HasIndex(e => e.ClienteId)
                .HasDatabaseName("idx_medidas_cliente");

            entity.HasOne(m => m.Cliente)
                .WithMany(c => c.Medidas)
                .HasForeignKey(m => m.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    // -------------------------------------------------------------------------
    // Fotos de Progreso
    // -------------------------------------------------------------------------
    private sealed class FotoProgresoConfiguration : IEntityTypeConfiguration<FotoProgreso>
    {
        public void Configure(EntityTypeBuilder<FotoProgreso> entity)
        {
            entity.ToTable("fotos_progreso");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.TenantId)
                .HasColumnName("tenant_id")
                .IsRequired();

            entity.Property(e => e.ClienteId)
                .HasColumnName("cliente_id")
                .IsRequired();

            entity.Property(e => e.UrlCloudinary)
                .HasColumnName("url_cloudinary")
                .IsRequired();

            entity.Property(e => e.PublicId)
                .HasColumnName("public_id")
                .IsRequired();

            entity.Property(e => e.Tipo)
                .HasColumnName("tipo")
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.Fecha)
                .HasColumnName("fecha")
                .IsRequired()
                .HasDefaultValueSql("CURRENT_DATE");

            entity.Property(e => e.MesReferencia)
                .HasColumnName("mes_referencia")
                .HasMaxLength(7);

            entity.Property(e => e.SubidoPor)
                .HasColumnName("subido_por")
                .HasMaxLength(20)
                .HasDefaultValue("entrenador");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("NOW()");

            entity.HasIndex(e => e.ClienteId)
                .HasDatabaseName("idx_fotos_cliente");

            entity.HasOne(f => f.Cliente)
                .WithMany(c => c.Fotos)
                .HasForeignKey(f => f.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    // -------------------------------------------------------------------------
    // Sesiones
    // -------------------------------------------------------------------------
    private sealed class SesionConfiguration : IEntityTypeConfiguration<Sesion>
    {
        public void Configure(EntityTypeBuilder<Sesion> entity)
        {
            entity.ToTable("sesiones");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.TenantId)
                .HasColumnName("tenant_id")
                .IsRequired();

            entity.Property(e => e.ClienteId)
                .HasColumnName("cliente_id")
                .IsRequired();

            entity.Property(e => e.FechaHora)
                .HasColumnName("fecha_hora")
                .IsRequired();

            entity.Property(e => e.DuracionMin)
                .HasColumnName("duracion_min")
                .HasDefaultValue(60);

            entity.Property(e => e.Estado)
                .HasColumnName("estado")
                .HasMaxLength(20)
                .HasDefaultValue("programada");

            entity.Property(e => e.Notas)
                .HasColumnName("notas");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("NOW()");

            entity.HasIndex(e => e.ClienteId)
                .HasDatabaseName("idx_sesiones_cliente");

            entity.HasIndex(e => e.FechaHora)
                .HasDatabaseName("idx_sesiones_fecha");

            entity.HasOne(s => s.Cliente)
                .WithMany(c => c.Sesiones)
                .HasForeignKey(s => s.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    // -------------------------------------------------------------------------
    // Rutinas (v2.0 — estructura lista)
    // -------------------------------------------------------------------------
    private sealed class RutinaConfiguration : IEntityTypeConfiguration<Rutina>
    {
        public void Configure(EntityTypeBuilder<Rutina> entity)
        {
            entity.ToTable("rutinas");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.TenantId)
                .HasColumnName("tenant_id")
                .IsRequired();

            entity.Property(e => e.ClienteId)
                .HasColumnName("cliente_id")
                .IsRequired();

            entity.Property(e => e.Nombre)
                .HasColumnName("nombre")
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Descripcion)
                .HasColumnName("descripcion");

            entity.Property(e => e.Dias)
                .HasColumnName("dias")
                .HasColumnType("jsonb")
                .HasDefaultValueSql("'[]'::jsonb");

            entity.Property(e => e.Activa)
                .HasColumnName("activa")
                .HasDefaultValue(true);

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("NOW()");

            entity.HasOne(r => r.Cliente)
                .WithMany(c => c.Rutinas)
                .HasForeignKey(r => r.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    // -------------------------------------------------------------------------
    // Ejercicios (v2.0 — estructura lista)
    // -------------------------------------------------------------------------
    private sealed class EjercicioConfiguration : IEntityTypeConfiguration<Ejercicio>
    {
        public void Configure(EntityTypeBuilder<Ejercicio> entity)
        {
            entity.ToTable("ejercicios");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.TenantId)
                .HasColumnName("tenant_id");

            entity.Property(e => e.Nombre)
                .HasColumnName("nombre")
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.GrupoMuscular)
                .HasColumnName("grupo_muscular")
                .HasMaxLength(50);

            entity.Property(e => e.Instrucciones)
                .HasColumnName("instrucciones");

            entity.Property(e => e.UrlVideo)
                .HasColumnName("url_video");

            entity.Property(e => e.ErroresComunes)
                .HasColumnName("errores_comunes");

            entity.Property(e => e.EsGlobal)
                .HasColumnName("es_global")
                .HasDefaultValue(false);

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("NOW()");
        }
    }

    // -------------------------------------------------------------------------
    // Planes Nutricionales (v2.0 — estructura lista)
    // -------------------------------------------------------------------------
    private sealed class PlanNutricionalConfiguration : IEntityTypeConfiguration<PlanNutricional>
    {
        public void Configure(EntityTypeBuilder<PlanNutricional> entity)
        {
            entity.ToTable("planes_nutricionales");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.TenantId)
                .HasColumnName("tenant_id")
                .IsRequired();

            entity.Property(e => e.ClienteId)
                .HasColumnName("cliente_id")
                .IsRequired();

            entity.Property(e => e.Calorias)
                .HasColumnName("calorias")
                .HasPrecision(6, 2);

            entity.Property(e => e.Proteinas)
                .HasColumnName("proteinas")
                .HasPrecision(5, 2);

            entity.Property(e => e.Carbohidratos)
                .HasColumnName("carbohidratos")
                .HasPrecision(5, 2);

            entity.Property(e => e.Grasas)
                .HasColumnName("grasas")
                .HasPrecision(5, 2);

            entity.Property(e => e.Agua)
                .HasColumnName("agua")
                .HasPrecision(4, 2);

            entity.Property(e => e.Comidas)
                .HasColumnName("comidas")
                .HasColumnType("jsonb")
                .HasDefaultValueSql("'[]'::jsonb");

            entity.Property(e => e.Activo)
                .HasColumnName("activo")
                .HasDefaultValue(true);

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("NOW()");

            entity.HasOne(p => p.Cliente)
                .WithMany(c => c.Planes)
                .HasForeignKey(p => p.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    // -------------------------------------------------------------------------
    // Check-ins (v2.0 — estructura lista)
    // -------------------------------------------------------------------------
    private sealed class CheckinConfiguration : IEntityTypeConfiguration<Checkin>
    {
        public void Configure(EntityTypeBuilder<Checkin> entity)
        {
            entity.ToTable("checkins", t =>
            {
                t.HasCheckConstraint("CK_checkins_energia",
                    "energia IS NULL OR energia BETWEEN 1 AND 10");
                t.HasCheckConstraint("CK_checkins_hambre",
                    "hambre IS NULL OR hambre BETWEEN 1 AND 10");
                t.HasCheckConstraint("CK_checkins_adherencia",
                    "adherencia IS NULL OR adherencia BETWEEN 1 AND 10");
                t.HasCheckConstraint("CK_checkins_sueno",
                    "sueno IS NULL OR sueno BETWEEN 1 AND 10");
                t.HasCheckConstraint("CK_checkins_estres",
                    "estres IS NULL OR estres BETWEEN 1 AND 10");
            });

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.TenantId)
                .HasColumnName("tenant_id")
                .IsRequired();

            entity.Property(e => e.ClienteId)
                .HasColumnName("cliente_id")
                .IsRequired();

            entity.Property(e => e.Semana)
                .HasColumnName("semana")
                .IsRequired();

            entity.Property(e => e.Energia).HasColumnName("energia");
            entity.Property(e => e.Hambre).HasColumnName("hambre");
            entity.Property(e => e.Adherencia).HasColumnName("adherencia");
            entity.Property(e => e.Sueno).HasColumnName("sueno");
            entity.Property(e => e.Estres).HasColumnName("estres");

            entity.Property(e => e.PesoSemana)
                .HasColumnName("peso_semana")
                .HasPrecision(5, 2);

            entity.Property(e => e.NotasCliente)
                .HasColumnName("notas_cliente");

            entity.Property(e => e.FeedbackEntrenador)
                .HasColumnName("feedback_entrenador");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("NOW()");

            entity.HasOne(c => c.Cliente)
                .WithMany(cl => cl.Checkins)
                .HasForeignKey(c => c.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    // -------------------------------------------------------------------------
    // Pagos (v3.0 — estructura lista)
    // -------------------------------------------------------------------------
    private sealed class PagoConfiguration : IEntityTypeConfiguration<Pago>
    {
        public void Configure(EntityTypeBuilder<Pago> entity)
        {
            entity.ToTable("pagos");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.TenantId)
                .HasColumnName("tenant_id")
                .IsRequired();

            entity.Property(e => e.ClienteId)
                .HasColumnName("cliente_id")
                .IsRequired();

            entity.Property(e => e.Monto)
                .HasColumnName("monto")
                .IsRequired()
                .HasPrecision(10, 2);

            entity.Property(e => e.Moneda)
                .HasColumnName("moneda")
                .HasMaxLength(3)
                .HasDefaultValue("COP");

            entity.Property(e => e.Estado)
                .HasColumnName("estado")
                .HasMaxLength(20)
                .HasDefaultValue("pendiente");

            entity.Property(e => e.Proveedor)
                .HasColumnName("proveedor")
                .HasMaxLength(20);

            entity.Property(e => e.ReferenciaExterna)
                .HasColumnName("referencia_externa");

            entity.Property(e => e.VenceEn)
                .HasColumnName("vence_en");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("NOW()");

            entity.HasOne(p => p.Cliente)
                .WithMany(c => c.Pagos)
                .HasForeignKey(p => p.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    // -------------------------------------------------------------------------
    // Suscripciones SaaS (v4.0 — estructura lista)
    // -------------------------------------------------------------------------
    private sealed class SuscripcionSaasConfiguration : IEntityTypeConfiguration<SuscripcionSaas>
    {
        public void Configure(EntityTypeBuilder<SuscripcionSaas> entity)
        {
            entity.ToTable("suscripciones_saas");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.EntrenadorId)
                .HasColumnName("entrenador_id")
                .IsRequired();

            entity.Property(e => e.Plan)
                .HasColumnName("plan")
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.Estado)
                .HasColumnName("estado")
                .HasMaxLength(20)
                .HasDefaultValue("activa");

            entity.Property(e => e.TrialEndsAt)
                .HasColumnName("trial_ends_at");

            entity.Property(e => e.NextBilling)
                .HasColumnName("next_billing");

            entity.Property(e => e.StripeCustomerId)
                .HasColumnName("stripe_customer_id");

            entity.Property(e => e.StripeSubscriptionId)
                .HasColumnName("stripe_subscription_id");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("NOW()");

            entity.HasOne(s => s.Entrenador)
                .WithMany(e => e.Suscripciones)
                .HasForeignKey(s => s.EntrenadorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
