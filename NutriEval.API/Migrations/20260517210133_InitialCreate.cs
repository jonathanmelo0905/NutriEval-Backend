using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriEval.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ejercicios",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    grupo_muscular = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    instrucciones = table.Column<string>(type: "text", nullable: true),
                    url_video = table.Column<string>(type: "text", nullable: true),
                    errores_comunes = table.Column<string>(type: "text", nullable: true),
                    es_global = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ejercicios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "entrenadores",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    plan = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "trial"),
                    trial_ends_at = table.Column<DateOnly>(type: "date", nullable: true),
                    redes_sociales = table.Column<string>(type: "jsonb", nullable: false, defaultValueSql: "'{}'::jsonb"),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entrenadores", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "clientes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    fecha_nacimiento = table.Column<DateOnly>(type: "date", nullable: true),
                    sexo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    peso_inicial = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    estatura = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    objetivo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    nivel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    contacto_emergencia = table.Column<string>(type: "jsonb", nullable: false, defaultValueSql: "'{}'::jsonb"),
                    salud = table.Column<string>(type: "jsonb", nullable: false, defaultValueSql: "'{}'::jsonb"),
                    habitos = table.Column<string>(type: "jsonb", nullable: false, defaultValueSql: "'{}'::jsonb"),
                    parq_completado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    parq_datos = table.Column<string>(type: "jsonb", nullable: false, defaultValueSql: "'{}'::jsonb"),
                    parq_fecha = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    consentimiento_aceptado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    consentimiento_fecha = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    fotos_iniciales = table.Column<string>(type: "jsonb", nullable: false, defaultValueSql: "'[]'::jsonb"),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clientes", x => x.id);
                    table.ForeignKey(
                        name: "FK_clientes_entrenadores_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "entrenadores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "suscripciones_saas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    entrenador_id = table.Column<Guid>(type: "uuid", nullable: false),
                    plan = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "activa"),
                    trial_ends_at = table.Column<DateOnly>(type: "date", nullable: true),
                    next_billing = table.Column<DateOnly>(type: "date", nullable: true),
                    stripe_customer_id = table.Column<string>(type: "text", nullable: true),
                    stripe_subscription_id = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_suscripciones_saas", x => x.id);
                    table.ForeignKey(
                        name: "FK_suscripciones_saas_entrenadores_entrenador_id",
                        column: x => x.entrenador_id,
                        principalTable: "entrenadores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "checkins",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cliente_id = table.Column<Guid>(type: "uuid", nullable: false),
                    semana = table.Column<DateOnly>(type: "date", nullable: false),
                    energia = table.Column<int>(type: "integer", nullable: true),
                    hambre = table.Column<int>(type: "integer", nullable: true),
                    adherencia = table.Column<int>(type: "integer", nullable: true),
                    sueno = table.Column<int>(type: "integer", nullable: true),
                    estres = table.Column<int>(type: "integer", nullable: true),
                    peso_semana = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    notas_cliente = table.Column<string>(type: "text", nullable: true),
                    feedback_entrenador = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checkins", x => x.id);
                    table.CheckConstraint("CK_checkins_adherencia", "adherencia IS NULL OR adherencia BETWEEN 1 AND 10");
                    table.CheckConstraint("CK_checkins_energia", "energia IS NULL OR energia BETWEEN 1 AND 10");
                    table.CheckConstraint("CK_checkins_estres", "estres IS NULL OR estres BETWEEN 1 AND 10");
                    table.CheckConstraint("CK_checkins_hambre", "hambre IS NULL OR hambre BETWEEN 1 AND 10");
                    table.CheckConstraint("CK_checkins_sueno", "sueno IS NULL OR sueno BETWEEN 1 AND 10");
                    table.ForeignKey(
                        name: "FK_checkins_clientes_cliente_id",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "evaluaciones",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cliente_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    datos_entrada = table.Column<string>(type: "jsonb", nullable: false),
                    resultados = table.Column<string>(type: "jsonb", nullable: false),
                    fecha = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "CURRENT_DATE"),
                    notas = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_evaluaciones", x => x.id);
                    table.ForeignKey(
                        name: "FK_evaluaciones_clientes_cliente_id",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "fotos_progreso",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cliente_id = table.Column<Guid>(type: "uuid", nullable: false),
                    url_cloudinary = table.Column<string>(type: "text", nullable: false),
                    public_id = table.Column<string>(type: "text", nullable: false),
                    tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    fecha = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "CURRENT_DATE"),
                    mes_referencia = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    subido_por = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "entrenador"),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fotos_progreso", x => x.id);
                    table.ForeignKey(
                        name: "FK_fotos_progreso_clientes_cliente_id",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "medidas_corporales",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cliente_id = table.Column<Guid>(type: "uuid", nullable: false),
                    peso = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    porcentaje_grasa = table.Column<decimal>(type: "numeric(4,2)", precision: 4, scale: 2, nullable: true),
                    masa_muscular = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    imc = table.Column<decimal>(type: "numeric(4,2)", precision: 4, scale: 2, nullable: true),
                    cintura = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    cadera = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    pecho = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    brazo_derecho = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    brazo_izquierdo = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    pierna_derecha = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    pierna_izquierda = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    fecha = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "CURRENT_DATE"),
                    notas = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_medidas_corporales", x => x.id);
                    table.ForeignKey(
                        name: "FK_medidas_corporales_clientes_cliente_id",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pagos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cliente_id = table.Column<Guid>(type: "uuid", nullable: false),
                    monto = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    moneda = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "COP"),
                    estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "pendiente"),
                    proveedor = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    referencia_externa = table.Column<string>(type: "text", nullable: true),
                    vence_en = table.Column<DateOnly>(type: "date", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pagos", x => x.id);
                    table.ForeignKey(
                        name: "FK_pagos_clientes_cliente_id",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "planes_nutricionales",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cliente_id = table.Column<Guid>(type: "uuid", nullable: false),
                    calorias = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: true),
                    proteinas = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    carbohidratos = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    grasas = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    agua = table.Column<decimal>(type: "numeric(4,2)", precision: 4, scale: 2, nullable: true),
                    comidas = table.Column<string>(type: "jsonb", nullable: false, defaultValueSql: "'[]'::jsonb"),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_planes_nutricionales", x => x.id);
                    table.ForeignKey(
                        name: "FK_planes_nutricionales_clientes_cliente_id",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "rutinas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cliente_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    dias = table.Column<string>(type: "jsonb", nullable: false, defaultValueSql: "'[]'::jsonb"),
                    activa = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rutinas", x => x.id);
                    table.ForeignKey(
                        name: "FK_rutinas_clientes_cliente_id",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "sesiones",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cliente_id = table.Column<Guid>(type: "uuid", nullable: false),
                    fecha_hora = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    duracion_min = table.Column<int>(type: "integer", nullable: false, defaultValue: 60),
                    estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "programada"),
                    notas = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sesiones", x => x.id);
                    table.ForeignKey(
                        name: "FK_sesiones_clientes_cliente_id",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_checkins_cliente_id",
                table: "checkins",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "idx_clientes_tenant",
                table: "clientes",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_entrenadores_email",
                table: "entrenadores",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_evaluaciones_cliente",
                table: "evaluaciones",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "idx_evaluaciones_tenant",
                table: "evaluaciones",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "idx_fotos_cliente",
                table: "fotos_progreso",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "idx_medidas_cliente",
                table: "medidas_corporales",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "IX_pagos_cliente_id",
                table: "pagos",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "IX_planes_nutricionales_cliente_id",
                table: "planes_nutricionales",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "IX_rutinas_cliente_id",
                table: "rutinas",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "idx_sesiones_cliente",
                table: "sesiones",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "idx_sesiones_fecha",
                table: "sesiones",
                column: "fecha_hora");

            migrationBuilder.CreateIndex(
                name: "IX_suscripciones_saas_entrenador_id",
                table: "suscripciones_saas",
                column: "entrenador_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "checkins");

            migrationBuilder.DropTable(
                name: "ejercicios");

            migrationBuilder.DropTable(
                name: "evaluaciones");

            migrationBuilder.DropTable(
                name: "fotos_progreso");

            migrationBuilder.DropTable(
                name: "medidas_corporales");

            migrationBuilder.DropTable(
                name: "pagos");

            migrationBuilder.DropTable(
                name: "planes_nutricionales");

            migrationBuilder.DropTable(
                name: "rutinas");

            migrationBuilder.DropTable(
                name: "sesiones");

            migrationBuilder.DropTable(
                name: "suscripciones_saas");

            migrationBuilder.DropTable(
                name: "clientes");

            migrationBuilder.DropTable(
                name: "entrenadores");
        }
    }
}
