CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260410204547_AddCalendarioSanitario') THEN
    CREATE TABLE "Clientes" (
        "Id" uuid NOT NULL,
        "Nombre" character varying(200) NOT NULL,
        "Ruc" character varying(20) NOT NULL,
        "Direccion" character varying(500),
        "Telefono" character varying(50),
        "IsActive" boolean NOT NULL,
        CONSTRAINT "PK_Clientes" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260410204547_AddCalendarioSanitario') THEN
    CREATE TABLE "Galpones" (
        "Id" uuid NOT NULL,
        "Nombre" character varying(100) NOT NULL,
        "Capacidad" integer NOT NULL,
        "Ubicacion" character varying(255) NOT NULL,
        "IsActive" boolean NOT NULL,
        CONSTRAINT "PK_Galpones" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260410204547_AddCalendarioSanitario') THEN
    CREATE TABLE "Lotes" (
        "Id" uuid NOT NULL,
        "FechaIngreso" timestamp with time zone NOT NULL,
        "CantidadInicial" integer NOT NULL,
        "CantidadActual" integer NOT NULL,
        "MortalidadAcumulada" integer NOT NULL DEFAULT 0,
        "PollosVendidos" integer NOT NULL DEFAULT 0,
        "Estado" character varying(20) NOT NULL,
        "CostoUnitarioPollito" numeric(18,2) NOT NULL,
        "IsActive" boolean NOT NULL,
        CONSTRAINT "PK_Lotes" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260410204547_AddCalendarioSanitario') THEN
    CREATE TABLE "Productos" (
        "Id" uuid NOT NULL,
        "Nombre" character varying(150) NOT NULL,
        "Tipo" character varying(30) NOT NULL,
        "UnidadMedida" character varying(20) NOT NULL,
        "IsActive" boolean NOT NULL,
        CONSTRAINT "PK_Productos" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260410204547_AddCalendarioSanitario') THEN
    CREATE TABLE "Usuarios" (
        "Id" uuid NOT NULL,
        "FirebaseUid" character varying(128) NOT NULL,
        "Nombre" character varying(150) NOT NULL,
        "Rol" character varying(50) NOT NULL,
        "IsActive" boolean NOT NULL,
        CONSTRAINT "PK_Usuarios" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260410204547_AddCalendarioSanitario') THEN
    CREATE TABLE "CalendarioSanitario" (
        "Id" uuid NOT NULL,
        "LoteId" uuid NOT NULL,
        "DiaDeAplicacion" integer NOT NULL,
        "DescripcionTratamiento" character varying(250) NOT NULL,
        "Estado" character varying(20) NOT NULL,
        "IsActive" boolean NOT NULL,
        CONSTRAINT "PK_CalendarioSanitario" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_CalendarioSanitario_Lotes_LoteId" FOREIGN KEY ("LoteId") REFERENCES "Lotes" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260410204547_AddCalendarioSanitario') THEN
    CREATE TABLE "GastosOperativos" (
        "Id" uuid NOT NULL,
        "GalponId" uuid NOT NULL,
        "LoteId" uuid,
        "Descripcion" character varying(250) NOT NULL,
        "Fecha" timestamp with time zone NOT NULL,
        "Monto" numeric(18,2) NOT NULL,
        "IsActive" boolean NOT NULL,
        CONSTRAINT "PK_GastosOperativos" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_GastosOperativos_Galpones_GalponId" FOREIGN KEY ("GalponId") REFERENCES "Galpones" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_GastosOperativos_Lotes_LoteId" FOREIGN KEY ("LoteId") REFERENCES "Lotes" ("Id") ON DELETE SET NULL
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260410204547_AddCalendarioSanitario') THEN
    CREATE TABLE "Mortalidades" (
        "Id" uuid NOT NULL,
        "LoteId" uuid NOT NULL,
        "Fecha" timestamp with time zone NOT NULL,
        "CantidadBajas" integer NOT NULL,
        "IsActive" boolean NOT NULL,
        CONSTRAINT "PK_Mortalidades" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Mortalidades_Lotes_LoteId" FOREIGN KEY ("LoteId") REFERENCES "Lotes" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260410204547_AddCalendarioSanitario') THEN
    CREATE TABLE "Ventas" (
        "Id" uuid NOT NULL,
        "LoteId" uuid NOT NULL,
        "ClienteId" uuid NOT NULL,
        "Fecha" timestamp with time zone NOT NULL,
        "CantidadPollos" integer NOT NULL,
        "PrecioUnitario" numeric(18,2) NOT NULL,
        "TotalVenta" numeric(18,2) NOT NULL,
        "IsActive" boolean NOT NULL,
        CONSTRAINT "PK_Ventas" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Ventas_Clientes_ClienteId" FOREIGN KEY ("ClienteId") REFERENCES "Clientes" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_Ventas_Lotes_LoteId" FOREIGN KEY ("LoteId") REFERENCES "Lotes" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260410204547_AddCalendarioSanitario') THEN
    CREATE TABLE "MovimientosInventario" (
        "Id" uuid NOT NULL,
        "ProductoId" uuid NOT NULL,
        "LoteId" uuid,
        "Cantidad" numeric(18,2) NOT NULL,
        "Tipo" character varying(20) NOT NULL,
        "Fecha" timestamp with time zone NOT NULL,
        "IsActive" boolean NOT NULL,
        CONSTRAINT "PK_MovimientosInventario" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_MovimientosInventario_Lotes_LoteId" FOREIGN KEY ("LoteId") REFERENCES "Lotes" ("Id") ON DELETE SET NULL,
        CONSTRAINT "FK_MovimientosInventario_Productos_ProductoId" FOREIGN KEY ("ProductoId") REFERENCES "Productos" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260410204547_AddCalendarioSanitario') THEN
    CREATE INDEX "IX_CalendarioSanitario_LoteId" ON "CalendarioSanitario" ("LoteId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260410204547_AddCalendarioSanitario') THEN
    CREATE INDEX "IX_GastosOperativos_GalponId" ON "GastosOperativos" ("GalponId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260410204547_AddCalendarioSanitario') THEN
    CREATE INDEX "IX_GastosOperativos_LoteId" ON "GastosOperativos" ("LoteId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260410204547_AddCalendarioSanitario') THEN
    CREATE INDEX "IX_Mortalidades_LoteId" ON "Mortalidades" ("LoteId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260410204547_AddCalendarioSanitario') THEN
    CREATE INDEX "IX_MovimientosInventario_LoteId" ON "MovimientosInventario" ("LoteId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260410204547_AddCalendarioSanitario') THEN
    CREATE INDEX "IX_MovimientosInventario_ProductoId" ON "MovimientosInventario" ("ProductoId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260410204547_AddCalendarioSanitario') THEN
    CREATE UNIQUE INDEX "IX_Usuarios_FirebaseUid" ON "Usuarios" ("FirebaseUid");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260410204547_AddCalendarioSanitario') THEN
    CREATE INDEX "IX_Ventas_ClienteId" ON "Ventas" ("ClienteId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260410204547_AddCalendarioSanitario') THEN
    CREATE INDEX "IX_Ventas_LoteId" ON "Ventas" ("LoteId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260410204547_AddCalendarioSanitario') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260410204547_AddCalendarioSanitario', '10.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411001128_AddPerfilUsuario') THEN
    ALTER TABLE "Usuarios" ALTER COLUMN "Nombre" TYPE character varying(100);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411001128_AddPerfilUsuario') THEN
    ALTER TABLE "Usuarios" ADD "Apellidos" character varying(100) NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411001128_AddPerfilUsuario') THEN
    ALTER TABLE "Usuarios" ADD "Direccion" character varying(200) NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411001128_AddPerfilUsuario') THEN
    ALTER TABLE "Usuarios" ADD "FechaNacimiento" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411001128_AddPerfilUsuario') THEN
    ALTER TABLE "Usuarios" ADD "Profesion" character varying(100) NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411001128_AddPerfilUsuario') THEN
    ALTER TABLE "Mortalidades" ADD "Causa" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411001128_AddPerfilUsuario') THEN
    ALTER TABLE "GastosOperativos" ADD "TipoGasto" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411001128_AddPerfilUsuario') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260411001128_AddPerfilUsuario', '10.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411004209_AddEmailToUsuario') THEN
    ALTER TABLE "Usuarios" ADD "Email" character varying(150) NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411004209_AddEmailToUsuario') THEN
    CREATE UNIQUE INDEX "IX_Usuarios_Email" ON "Usuarios" ("Email");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411004209_AddEmailToUsuario') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260411004209_AddEmailToUsuario', '10.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411023802_RefactorRolesEnum') THEN
    ALTER TABLE "Usuarios" ALTER COLUMN "Rol" TYPE integer USING (CASE WHEN "Rol" = 'Admin' THEN 2 WHEN "Rol" = 'SubAdmin' THEN 1 ELSE 0 END);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411023802_RefactorRolesEnum') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260411023802_RefactorRolesEnum', '10.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411024932_AddPesajeLote') THEN
    CREATE TABLE "PesajesLote" (
        "Id" uuid NOT NULL,
        "LoteId" uuid NOT NULL,
        "Fecha" timestamp with time zone NOT NULL,
        "PesoPromedioGramos" numeric NOT NULL,
        "CantidadMuestreada" integer NOT NULL,
        "IsActive" boolean NOT NULL,
        CONSTRAINT "PK_PesajesLote" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_PesajesLote_Lotes_LoteId" FOREIGN KEY ("LoteId") REFERENCES "Lotes" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411024932_AddPesajeLote') THEN
    CREATE INDEX "IX_PesajesLote_LoteId" ON "PesajesLote" ("LoteId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411024932_AddPesajeLote') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260411024932_AddPesajeLote', '10.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411161111_UpdateVentaToWeightBased') THEN
    ALTER TABLE "Ventas" RENAME COLUMN "PrecioUnitario" TO "PrecioPorKilo";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411161111_UpdateVentaToWeightBased') THEN
    ALTER TABLE "Ventas" ADD "PesoTotalVendido" numeric(18,2) NOT NULL DEFAULT 0.0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411161111_UpdateVentaToWeightBased') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260411161111_UpdateVentaToWeightBased', '10.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411182705_AddJustificacionToMovimiento') THEN
    ALTER TABLE "MovimientosInventario" ADD "Justificacion" text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411182705_AddJustificacionToMovimiento') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260411182705_AddJustificacionToMovimiento', '10.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411190649_AddAuditoriaUsuarios') THEN
    ALTER TABLE "Ventas" ADD "UsuarioId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411190649_AddAuditoriaUsuarios') THEN
    ALTER TABLE "PesajesLote" ADD "UsuarioId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411190649_AddAuditoriaUsuarios') THEN
    ALTER TABLE "MovimientosInventario" ADD "UsuarioId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411190649_AddAuditoriaUsuarios') THEN
    ALTER TABLE "Mortalidades" ADD "UsuarioId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411190649_AddAuditoriaUsuarios') THEN
    ALTER TABLE "GastosOperativos" ADD "UsuarioId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411190649_AddAuditoriaUsuarios') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260411190649_AddAuditoriaUsuarios', '10.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "Ventas" ADD "FechaCreacion" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "Ventas" ADD "FechaModificacion" timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "Ventas" ADD "UsuarioCreacionId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "Ventas" ADD "UsuarioModificacionId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "Usuarios" ADD "FechaCreacion" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "Usuarios" ADD "FechaModificacion" timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "Usuarios" ADD "UsuarioCreacionId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "Usuarios" ADD "UsuarioModificacionId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "Productos" ADD "FechaCreacion" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "Productos" ADD "FechaModificacion" timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "Productos" ADD "UsuarioCreacionId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "Productos" ADD "UsuarioModificacionId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "PesajesLote" ADD "FechaCreacion" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "PesajesLote" ADD "FechaModificacion" timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "PesajesLote" ADD "UsuarioCreacionId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "PesajesLote" ADD "UsuarioModificacionId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "MovimientosInventario" ADD "FechaCreacion" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "MovimientosInventario" ADD "FechaModificacion" timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "MovimientosInventario" ADD "UsuarioCreacionId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "MovimientosInventario" ADD "UsuarioModificacionId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "Mortalidades" ADD "FechaCreacion" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "Mortalidades" ADD "FechaModificacion" timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "Mortalidades" ADD "UsuarioCreacionId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "Mortalidades" ADD "UsuarioModificacionId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "Lotes" ADD "FechaCreacion" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "Lotes" ADD "FechaModificacion" timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "Lotes" ADD "UsuarioCreacionId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "Lotes" ADD "UsuarioModificacionId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "GastosOperativos" ADD "FechaCreacion" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "GastosOperativos" ADD "FechaModificacion" timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "GastosOperativos" ADD "UsuarioCreacionId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "GastosOperativos" ADD "UsuarioModificacionId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "Galpones" ADD "FechaCreacion" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "Galpones" ADD "FechaModificacion" timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "Galpones" ADD "UsuarioCreacionId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "Galpones" ADD "UsuarioModificacionId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "Clientes" ADD "FechaCreacion" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "Clientes" ADD "FechaModificacion" timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "Clientes" ADD "UsuarioCreacionId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "Clientes" ADD "UsuarioModificacionId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "CalendarioSanitario" ADD "FechaCreacion" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "CalendarioSanitario" ADD "FechaModificacion" timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "CalendarioSanitario" ADD "UsuarioCreacionId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    ALTER TABLE "CalendarioSanitario" ADD "UsuarioModificacionId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411201320_AddAutomaticAuditing') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260411201320_AddAutomaticAuditing', '10.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411205454_AddIntegridadContableYPagos') THEN
    ALTER TABLE "Ventas" ADD "EstadoPago" integer NOT NULL DEFAULT 1;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411205454_AddIntegridadContableYPagos') THEN
    ALTER TABLE "Lotes" ADD "CostoTotalFinal" numeric(18,2);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411205454_AddIntegridadContableYPagos') THEN
    ALTER TABLE "Lotes" ADD "FCRFinal" numeric(18,2);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411205454_AddIntegridadContableYPagos') THEN
    ALTER TABLE "Lotes" ADD "PorcentajeMortalidadFinal" numeric(18,2);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411205454_AddIntegridadContableYPagos') THEN
    ALTER TABLE "Lotes" ADD "UtilidadNetaFinal" numeric(18,2);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411205454_AddIntegridadContableYPagos') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260411205454_AddIntegridadContableYPagos', '10.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411205634_AddPerformanceIndexes') THEN
    CREATE INDEX "IX_Ventas_Fecha" ON "Ventas" ("Fecha");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411205634_AddPerformanceIndexes') THEN
    CREATE INDEX "IX_MovimientosInventario_Fecha" ON "MovimientosInventario" ("Fecha");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411205634_AddPerformanceIndexes') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260411205634_AddPerformanceIndexes', '10.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411214552_AddCatalogosDinamicos') THEN
    CREATE TABLE "CategoriasProductos" (
        "Id" uuid NOT NULL,
        "Nombre" text NOT NULL,
        "Descripcion" text,
        "IsActive" boolean NOT NULL,
        "FechaCreacion" timestamp with time zone NOT NULL,
        "UsuarioCreacionId" uuid,
        "FechaModificacion" timestamp with time zone,
        "UsuarioModificacionId" uuid,
        CONSTRAINT "PK_CategoriasProductos" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411214552_AddCatalogosDinamicos') THEN
    CREATE TABLE "UnidadesMedida" (
        "Id" uuid NOT NULL,
        "Nombre" text NOT NULL,
        "Abreviatura" text NOT NULL,
        "IsActive" boolean NOT NULL,
        "FechaCreacion" timestamp with time zone NOT NULL,
        "UsuarioCreacionId" uuid,
        "FechaModificacion" timestamp with time zone,
        "UsuarioModificacionId" uuid,
        CONSTRAINT "PK_UnidadesMedida" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411214552_AddCatalogosDinamicos') THEN
    ALTER TABLE "Productos" ADD "CategoriaProductoId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411214552_AddCatalogosDinamicos') THEN
    ALTER TABLE "Productos" ADD "PesoUnitarioKg" numeric(18,4) NOT NULL DEFAULT 1.0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411214552_AddCatalogosDinamicos') THEN
    ALTER TABLE "Productos" ADD "UnidadMedidaId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411214552_AddCatalogosDinamicos') THEN

                    INSERT INTO "CategoriasProductos" ("Id", "Nombre", "IsActive", "FechaCreacion") VALUES 
                    ('e29a46c0-4af3-4044-856c-19c07872529d', 'Alimento', true, now()),
                    ('e06c7c17-f8f9-4792-b52b-8ca3947da665', 'Medicamento', true, now()),
                    ('01557845-2a39-4847-a85b-948979aa5589', 'Insumo', true, now()),
                    ('38757172-92e7-446d-a699-888e811e7c55', 'Otro', true, now());

                    INSERT INTO "UnidadesMedida" ("Id", "Nombre", "Abreviatura", "IsActive", "FechaCreacion") VALUES 
                    ('4d7d4ae6-48f7-406a-85bd-656b4b06ce5c', 'Kilogramo', 'Kg', true, now()),
                    ('afd71ddb-15e8-40ea-b0f7-d2687b45e142', 'Unidad', 'Und', true, now()),
                    ('ab910ad2-45b5-4804-8e5f-deb5e73bd07e', 'Litro', 'L', true, now()),
                    ('742c15ff-10c6-4656-b72f-57a274bab267', 'Saco', 'Sc', true, now());
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411214552_AddCatalogosDinamicos') THEN

                    UPDATE "Productos" SET "CategoriaProductoId" = 'e29a46c0-4af3-4044-856c-19c07872529d' WHERE "Tipo" = 'Alimento';
                    UPDATE "Productos" SET "CategoriaProductoId" = 'e06c7c17-f8f9-4792-b52b-8ca3947da665' WHERE "Tipo" = 'Medicamento';
                    UPDATE "Productos" SET "CategoriaProductoId" = '01557845-2a39-4847-a85b-948979aa5589' WHERE "Tipo" = 'Insumo';
                    UPDATE "Productos" SET "CategoriaProductoId" = '38757172-92e7-446d-a699-888e811e7c55' WHERE "CategoriaProductoId" IS NULL;
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411214552_AddCatalogosDinamicos') THEN

                    UPDATE "Productos" SET "UnidadMedidaId" = '4d7d4ae6-48f7-406a-85bd-656b4b06ce5c', "PesoUnitarioKg" = 1.0 WHERE "UnidadMedida" = 'Kg';
                    UPDATE "Productos" SET "UnidadMedidaId" = 'afd71ddb-15e8-40ea-b0f7-d2687b45e142', "PesoUnitarioKg" = 1.0 WHERE "UnidadMedida" = 'Unidad';
                    UPDATE "Productos" SET "UnidadMedidaId" = 'ab910ad2-45b5-4804-8e5f-deb5e73bd07e', "PesoUnitarioKg" = 1.0 WHERE "UnidadMedida" = 'Litro';
                    UPDATE "Productos" SET "UnidadMedidaId" = '742c15ff-10c6-4656-b72f-57a274bab267', "PesoUnitarioKg" = 40.0 WHERE "UnidadMedida" = 'Saco';
                    UPDATE "Productos" SET "UnidadMedidaId" = 'afd71ddb-15e8-40ea-b0f7-d2687b45e142' WHERE "UnidadMedidaId" IS NULL;
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411214552_AddCatalogosDinamicos') THEN
    ALTER TABLE "Productos" DROP COLUMN "Tipo";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411214552_AddCatalogosDinamicos') THEN
    ALTER TABLE "Productos" DROP COLUMN "UnidadMedida";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411214552_AddCatalogosDinamicos') THEN
    CREATE INDEX "IX_Productos_CategoriaProductoId" ON "Productos" ("CategoriaProductoId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411214552_AddCatalogosDinamicos') THEN
    CREATE INDEX "IX_Productos_UnidadMedidaId" ON "Productos" ("UnidadMedidaId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411214552_AddCatalogosDinamicos') THEN
    ALTER TABLE "Productos" ADD CONSTRAINT "FK_Productos_CategoriasProductos_CategoriaProductoId" FOREIGN KEY ("CategoriaProductoId") REFERENCES "CategoriasProductos" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411214552_AddCatalogosDinamicos') THEN
    ALTER TABLE "Productos" ADD CONSTRAINT "FK_Productos_UnidadesMedida_UnidadMedidaId" FOREIGN KEY ("UnidadMedidaId") REFERENCES "UnidadesMedida" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411214552_AddCatalogosDinamicos') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260411214552_AddCatalogosDinamicos', '10.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411230854_AddGalponIdToLote') THEN
    ALTER TABLE "Lotes" ADD "GalponId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411230854_AddGalponIdToLote') THEN

                    DO $$
                    DECLARE
                        default_galpon_id uuid;
                    BEGIN
                        IF NOT EXISTS (SELECT 1 FROM "Galpones") THEN
                            default_galpon_id := gen_random_uuid();
                            INSERT INTO "Galpones" ("Id", "Nombre", "Capacidad", "Ubicacion", "IsActive", "FechaCreacion")
                            VALUES (default_galpon_id, 'Galpón por Defecto', 1000, 'Ubicación Inicial', true, now());
                        ELSE
                            SELECT "Id" INTO default_galpon_id FROM "Galpones" LIMIT 1;
                        END IF;

                        -- 3. Actualizar lotes existentes para que apunten al galpón encontrado/creado
                        UPDATE "Lotes" SET "GalponId" = default_galpon_id WHERE "GalponId" IS NULL;
                    END $$;
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411230854_AddGalponIdToLote') THEN
    CREATE INDEX "IX_Lotes_GalponId" ON "Lotes" ("GalponId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411230854_AddGalponIdToLote') THEN
    ALTER TABLE "Lotes" ADD CONSTRAINT "FK_Lotes_Galpones_GalponId" FOREIGN KEY ("GalponId") REFERENCES "Galpones" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411230854_AddGalponIdToLote') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260411230854_AddGalponIdToLote', '10.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411231251_AddAuditoriaLogs') THEN
    CREATE TABLE "AuditoriaLogs" (
        "Id" uuid NOT NULL,
        "UsuarioId" uuid NOT NULL,
        "Accion" character varying(50) NOT NULL,
        "Entidad" character varying(100) NOT NULL,
        "EntidadId" uuid NOT NULL,
        "Fecha" timestamp with time zone NOT NULL,
        "DetallesJSON" jsonb NOT NULL,
        "IsActive" boolean NOT NULL,
        "FechaCreacion" timestamp with time zone NOT NULL,
        "UsuarioCreacionId" uuid,
        "FechaModificacion" timestamp with time zone,
        "UsuarioModificacionId" uuid,
        CONSTRAINT "PK_AuditoriaLogs" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411231251_AddAuditoriaLogs') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260411231251_AddAuditoriaLogs', '10.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411235541_AddPagosCuentasPorCobrar') THEN
    ALTER TABLE "Ventas" ALTER COLUMN "EstadoPago" SET DEFAULT 2;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411235541_AddPagosCuentasPorCobrar') THEN
    CREATE TABLE "PagosVentas" (
        "Id" uuid NOT NULL,
        "VentaId" uuid NOT NULL,
        "FechaPago" timestamp with time zone NOT NULL,
        "MetodoPago" integer NOT NULL,
        "UsuarioId" uuid NOT NULL,
        "Monto" numeric(18,2) NOT NULL,
        "IsActive" boolean NOT NULL,
        "FechaCreacion" timestamp with time zone NOT NULL,
        "UsuarioCreacionId" uuid,
        "FechaModificacion" timestamp with time zone,
        "UsuarioModificacionId" uuid,
        CONSTRAINT "PK_PagosVentas" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_PagosVentas_Ventas_VentaId" FOREIGN KEY ("VentaId") REFERENCES "Ventas" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411235541_AddPagosCuentasPorCobrar') THEN
    CREATE INDEX "IX_PagosVentas_FechaPago" ON "PagosVentas" ("FechaPago");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411235541_AddPagosCuentasPorCobrar') THEN
    CREATE INDEX "IX_PagosVentas_VentaId" ON "PagosVentas" ("VentaId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260411235541_AddPagosCuentasPorCobrar') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260411235541_AddPagosCuentasPorCobrar', '10.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260412000025_AddPlantillasSanitarias') THEN
    ALTER TABLE "CalendarioSanitario" ADD "ProductoIdRecomendado" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260412000025_AddPlantillasSanitarias') THEN
    CREATE TABLE "PlantillasSanitarias" (
        "Id" uuid NOT NULL,
        "Nombre" character varying(100) NOT NULL,
        "Descripcion" character varying(500),
        "IsActive" boolean NOT NULL,
        "FechaCreacion" timestamp with time zone NOT NULL,
        "UsuarioCreacionId" uuid,
        "FechaModificacion" timestamp with time zone,
        "UsuarioModificacionId" uuid,
        CONSTRAINT "PK_PlantillasSanitarias" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260412000025_AddPlantillasSanitarias') THEN
    CREATE TABLE "ActividadesPlantillas" (
        "Id" uuid NOT NULL,
        "PlantillaId" uuid NOT NULL,
        "TipoActividad" integer NOT NULL,
        "DiaDeAplicacion" integer NOT NULL,
        "Descripcion" character varying(200) NOT NULL,
        "ProductoIdRecomendado" uuid,
        "IsActive" boolean NOT NULL,
        "FechaCreacion" timestamp with time zone NOT NULL,
        "UsuarioCreacionId" uuid,
        "FechaModificacion" timestamp with time zone,
        "UsuarioModificacionId" uuid,
        CONSTRAINT "PK_ActividadesPlantillas" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_ActividadesPlantillas_PlantillasSanitarias_PlantillaId" FOREIGN KEY ("PlantillaId") REFERENCES "PlantillasSanitarias" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_ActividadesPlantillas_Productos_ProductoIdRecomendado" FOREIGN KEY ("ProductoIdRecomendado") REFERENCES "Productos" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260412000025_AddPlantillasSanitarias') THEN
    CREATE INDEX "IX_CalendarioSanitario_ProductoIdRecomendado" ON "CalendarioSanitario" ("ProductoIdRecomendado");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260412000025_AddPlantillasSanitarias') THEN
    CREATE INDEX "IX_ActividadesPlantillas_PlantillaId" ON "ActividadesPlantillas" ("PlantillaId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260412000025_AddPlantillasSanitarias') THEN
    CREATE INDEX "IX_ActividadesPlantillas_ProductoIdRecomendado" ON "ActividadesPlantillas" ("ProductoIdRecomendado");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260412000025_AddPlantillasSanitarias') THEN
    ALTER TABLE "CalendarioSanitario" ADD CONSTRAINT "FK_CalendarioSanitario_Productos_ProductoIdRecomendado" FOREIGN KEY ("ProductoIdRecomendado") REFERENCES "Productos" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260412000025_AddPlantillasSanitarias') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260412000025_AddPlantillasSanitarias', '10.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260412000627_AddJustificacionCancelacion') THEN
    ALTER TABLE "Lotes" ADD "JustificacionCancelacion" text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260412000627_AddJustificacionCancelacion') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260412000627_AddJustificacionCancelacion', '10.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260412024051_AddCalendarioSanitarioFields') THEN
    ALTER TABLE "Productos" ADD "UmbralMinimo" numeric(18,2) NOT NULL DEFAULT 0.0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260412024051_AddCalendarioSanitarioFields') THEN
    ALTER TABLE "MovimientosInventario" ADD "CostoTotal" numeric(18,2);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260412024051_AddCalendarioSanitarioFields') THEN
    ALTER TABLE "MovimientosInventario" ADD "Proveedor" character varying(200);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260412024051_AddCalendarioSanitarioFields') THEN
    ALTER TABLE "CalendarioSanitario" ADD "EsManual" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260412024051_AddCalendarioSanitarioFields') THEN
    ALTER TABLE "CalendarioSanitario" ADD "Justificacion" character varying(500);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260412024051_AddCalendarioSanitarioFields') THEN
    ALTER TABLE "CalendarioSanitario" ADD "Tipo" character varying(30) NOT NULL DEFAULT 'Otros';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260412024051_AddCalendarioSanitarioFields') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260412024051_AddCalendarioSanitarioFields', '10.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260412035059_AddProveedoresYCompras') THEN
    ALTER TABLE "MovimientosInventario" ADD "CompraId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260412035059_AddProveedoresYCompras') THEN
    CREATE TABLE "Proveedores" (
        "Id" uuid NOT NULL,
        "RazonSocial" character varying(200) NOT NULL,
        "NitRuc" character varying(20) NOT NULL,
        "Telefono" character varying(20),
        "Email" character varying(100),
        "Direccion" character varying(300),
        "IsActive" boolean NOT NULL,
        "FechaCreacion" timestamp with time zone NOT NULL,
        "UsuarioCreacionId" uuid,
        "FechaModificacion" timestamp with time zone,
        "UsuarioModificacionId" uuid,
        CONSTRAINT "PK_Proveedores" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260412035059_AddProveedoresYCompras') THEN
    CREATE TABLE "ComprasInventario" (
        "Id" uuid NOT NULL,
        "ProveedorId" uuid NOT NULL,
        "Fecha" timestamp with time zone NOT NULL,
        "EstadoPago" integer NOT NULL DEFAULT 2,
        "UsuarioIdRegistro" uuid NOT NULL,
        "Nota" text,
        "TotalCompra" numeric(18,2) NOT NULL,
        "TotalPagado" numeric(18,2) NOT NULL,
        "IsActive" boolean NOT NULL,
        "FechaCreacion" timestamp with time zone NOT NULL,
        "UsuarioCreacionId" uuid,
        "FechaModificacion" timestamp with time zone,
        "UsuarioModificacionId" uuid,
        CONSTRAINT "PK_ComprasInventario" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_ComprasInventario_Proveedores_ProveedorId" FOREIGN KEY ("ProveedorId") REFERENCES "Proveedores" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260412035059_AddProveedoresYCompras') THEN
    CREATE INDEX "IX_MovimientosInventario_CompraId" ON "MovimientosInventario" ("CompraId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260412035059_AddProveedoresYCompras') THEN
    CREATE INDEX "IX_ComprasInventario_Fecha" ON "ComprasInventario" ("Fecha");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260412035059_AddProveedoresYCompras') THEN
    CREATE INDEX "IX_ComprasInventario_ProveedorId" ON "ComprasInventario" ("ProveedorId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260412035059_AddProveedoresYCompras') THEN
    CREATE UNIQUE INDEX "IX_Proveedores_NitRuc" ON "Proveedores" ("NitRuc");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260412035059_AddProveedoresYCompras') THEN
    ALTER TABLE "MovimientosInventario" ADD CONSTRAINT "FK_MovimientosInventario_ComprasInventario_CompraId" FOREIGN KEY ("CompraId") REFERENCES "ComprasInventario" ("Id") ON DELETE SET NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260412035059_AddProveedoresYCompras') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260412035059_AddProveedoresYCompras', '10.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260412035655_AddCostoPPP') THEN
    ALTER TABLE "Productos" ADD "CostoUnitarioActual" numeric(18,4) NOT NULL DEFAULT 0.0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260412035655_AddCostoPPP') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260412035655_AddCostoPPP', '10.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260412041811_AddPagoCompra') THEN
    CREATE TABLE "PagosCompras" (
        "Id" uuid NOT NULL,
        "CompraId" uuid NOT NULL,
        "FechaPago" timestamp with time zone NOT NULL,
        "MetodoPago" integer NOT NULL,
        "UsuarioId" uuid NOT NULL,
        "Monto" numeric(18,2) NOT NULL,
        "IsActive" boolean NOT NULL,
        "FechaCreacion" timestamp with time zone NOT NULL,
        "UsuarioCreacionId" uuid,
        "FechaModificacion" timestamp with time zone,
        "UsuarioModificacionId" uuid,
        CONSTRAINT "PK_PagosCompras" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_PagosCompras_ComprasInventario_CompraId" FOREIGN KEY ("CompraId") REFERENCES "ComprasInventario" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260412041811_AddPagoCompra') THEN
    CREATE INDEX "IX_PagosCompras_CompraId" ON "PagosCompras" ("CompraId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260412041811_AddPagoCompra') THEN
    CREATE INDEX "IX_PagosCompras_FechaPago" ON "PagosCompras" ("FechaPago");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260412041811_AddPagoCompra') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260412041811_AddPagoCompra', '10.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260413000450_AddRegistroBienestarAndFixUsuario') THEN
    ALTER TABLE "Usuarios" ADD "CodigoVinculacion" character varying(10);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260413000450_AddRegistroBienestarAndFixUsuario') THEN
    ALTER TABLE "Usuarios" ADD "FechaExpiracionCodigo" timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260413000450_AddRegistroBienestarAndFixUsuario') THEN
    ALTER TABLE "Usuarios" ADD "Telefono" character varying(20);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260413000450_AddRegistroBienestarAndFixUsuario') THEN
    ALTER TABLE "Usuarios" ADD "WhatsAppNumero" character varying(20);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260413000450_AddRegistroBienestarAndFixUsuario') THEN
    ALTER TABLE "ComprasInventario" ADD "FechaVencimiento" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260413000450_AddRegistroBienestarAndFixUsuario') THEN
    ALTER TABLE "AuditoriaLogs" ADD "Detalles" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260413000450_AddRegistroBienestarAndFixUsuario') THEN
    ALTER TABLE "AuditoriaLogs" ADD "EntidadNombre" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260413000450_AddRegistroBienestarAndFixUsuario') THEN
    ALTER TABLE "AuditoriaLogs" ADD "UsuarioNombre" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260413000450_AddRegistroBienestarAndFixUsuario') THEN
    CREATE TABLE "ConfiguracionSistema" (
        "Id" uuid NOT NULL,
        "NombreEmpresa" character varying(200) NOT NULL,
        "Nit" character varying(50) NOT NULL,
        "Telefono" text,
        "Email" text,
        "Direccion" text,
        "MonedaPorDefecto" character varying(10) NOT NULL DEFAULT 'USD',
        "LogoUrl" text,
        "IsActive" boolean NOT NULL,
        "FechaCreacion" timestamp with time zone NOT NULL,
        "UsuarioCreacionId" uuid,
        "FechaModificacion" timestamp with time zone,
        "UsuarioModificacionId" uuid,
        CONSTRAINT "PK_ConfiguracionSistema" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260413000450_AddRegistroBienestarAndFixUsuario') THEN
    CREATE TABLE "Conversaciones" (
        "Id" uuid NOT NULL,
        "UsuarioId" uuid NOT NULL,
        "FechaInicio" timestamp with time zone NOT NULL,
        "Estado" character varying(20) NOT NULL,
        "ResumenActual" text,
        "UltimoIndiceMensajeResumido" integer NOT NULL,
        "IsActive" boolean NOT NULL,
        "FechaCreacion" timestamp with time zone NOT NULL,
        "UsuarioCreacionId" uuid,
        "FechaModificacion" timestamp with time zone,
        "UsuarioModificacionId" uuid,
        CONSTRAINT "PK_Conversaciones" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260413000450_AddRegistroBienestarAndFixUsuario') THEN
    CREATE TABLE "IntencionesPendientes" (
        "Id" uuid NOT NULL,
        "ConversacionId" uuid NOT NULL,
        "PluginNombre" text NOT NULL,
        "FuncionNombre" text NOT NULL,
        "ParametrosJson" text NOT NULL,
        "Procesada" boolean NOT NULL,
        "IsActive" boolean NOT NULL,
        "FechaCreacion" timestamp with time zone NOT NULL,
        "UsuarioCreacionId" uuid,
        "FechaModificacion" timestamp with time zone,
        "UsuarioModificacionId" uuid,
        CONSTRAINT "PK_IntencionesPendientes" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260413000450_AddRegistroBienestarAndFixUsuario') THEN
    CREATE TABLE "OrdenesCompra" (
        "Id" uuid NOT NULL,
        "ProveedorId" uuid NOT NULL,
        "Fecha" timestamp with time zone NOT NULL,
        "Estado" character varying(20) NOT NULL,
        "Nota" character varying(500),
        "UsuarioIdRegistro" uuid NOT NULL,
        "TotalOrden" numeric(18,2) NOT NULL,
        "IsActive" boolean NOT NULL,
        "FechaCreacion" timestamp with time zone NOT NULL,
        "UsuarioCreacionId" uuid,
        "FechaModificacion" timestamp with time zone,
        "UsuarioModificacionId" uuid,
        CONSTRAINT "PK_OrdenesCompra" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_OrdenesCompra_Proveedores_ProveedorId" FOREIGN KEY ("ProveedorId") REFERENCES "Proveedores" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260413000450_AddRegistroBienestarAndFixUsuario') THEN
    CREATE TABLE "RegistroBienestar" (
        "Id" uuid NOT NULL,
        "LoteId" uuid NOT NULL,
        "Fecha" timestamp with time zone NOT NULL,
        "Temperatura" numeric,
        "Humedad" numeric,
        "ConsumoAgua" numeric,
        "Observaciones" text,
        "UsuarioId" uuid NOT NULL,
        "IsActive" boolean NOT NULL,
        "FechaCreacion" timestamp with time zone NOT NULL,
        "UsuarioCreacionId" uuid,
        "FechaModificacion" timestamp with time zone,
        "UsuarioModificacionId" uuid,
        CONSTRAINT "PK_RegistroBienestar" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260413000450_AddRegistroBienestarAndFixUsuario') THEN
    CREATE TABLE "MensajesChat" (
        "Id" uuid NOT NULL,
        "ConversacionId" uuid NOT NULL,
        "Rol" character varying(20) NOT NULL,
        "Contenido" text NOT NULL,
        "Fecha" timestamp with time zone NOT NULL,
        "IsActive" boolean NOT NULL,
        "FechaCreacion" timestamp with time zone NOT NULL,
        "UsuarioCreacionId" uuid,
        "FechaModificacion" timestamp with time zone,
        "UsuarioModificacionId" uuid,
        CONSTRAINT "PK_MensajesChat" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_MensajesChat_Conversaciones_ConversacionId" FOREIGN KEY ("ConversacionId") REFERENCES "Conversaciones" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260413000450_AddRegistroBienestarAndFixUsuario') THEN
    CREATE TABLE "OrdenesCompraItems" (
        "Id" uuid NOT NULL,
        "OrdenCompraId" uuid NOT NULL,
        "ProductoId" uuid NOT NULL,
        "Cantidad" numeric(18,2) NOT NULL,
        "PrecioUnitario" numeric(18,2) NOT NULL,
        "TotalItem" numeric(18,2) NOT NULL,
        "IsActive" boolean NOT NULL,
        "FechaCreacion" timestamp with time zone NOT NULL,
        "UsuarioCreacionId" uuid,
        "FechaModificacion" timestamp with time zone,
        "UsuarioModificacionId" uuid,
        CONSTRAINT "PK_OrdenesCompraItems" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_OrdenesCompraItems_OrdenesCompra_OrdenCompraId" FOREIGN KEY ("OrdenCompraId") REFERENCES "OrdenesCompra" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_OrdenesCompraItems_Productos_ProductoId" FOREIGN KEY ("ProductoId") REFERENCES "Productos" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260413000450_AddRegistroBienestarAndFixUsuario') THEN
    CREATE UNIQUE INDEX "IX_Usuarios_Telefono" ON "Usuarios" ("Telefono") WHERE "Telefono" IS NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260413000450_AddRegistroBienestarAndFixUsuario') THEN
    CREATE INDEX "IX_MensajesChat_ConversacionId" ON "MensajesChat" ("ConversacionId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260413000450_AddRegistroBienestarAndFixUsuario') THEN
    CREATE INDEX "IX_OrdenesCompra_ProveedorId" ON "OrdenesCompra" ("ProveedorId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260413000450_AddRegistroBienestarAndFixUsuario') THEN
    CREATE INDEX "IX_OrdenesCompraItems_OrdenCompraId" ON "OrdenesCompraItems" ("OrdenCompraId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260413000450_AddRegistroBienestarAndFixUsuario') THEN
    CREATE INDEX "IX_OrdenesCompraItems_ProductoId" ON "OrdenesCompraItems" ("ProductoId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260413000450_AddRegistroBienestarAndFixUsuario') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260413000450_AddRegistroBienestarAndFixUsuario', '10.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260413003405_AddTituloToConversacion') THEN
    ALTER TABLE "Conversaciones" ADD "Titulo" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260413003405_AddTituloToConversacion') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260413003405_AddTituloToConversacion', '10.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260415001543_AddNombreToLote') THEN
    ALTER TABLE "Lotes" ADD "Nombre" character varying(100) NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260415001543_AddNombreToLote') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260415001543_AddNombreToLote', '10.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260415160634_SyncSchema') THEN
    ALTER TABLE "Productos" ADD "StockActualKg" numeric(18,4) NOT NULL DEFAULT 0.0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260415160634_SyncSchema') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260415160634_SyncSchema', '10.0.5');
    END IF;
END $EF$;
COMMIT;

