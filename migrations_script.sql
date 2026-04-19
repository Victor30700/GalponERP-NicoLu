CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;
CREATE TABLE "Clientes" (
    "Id" uuid NOT NULL,
    "Nombre" character varying(200) NOT NULL,
    "Ruc" character varying(20) NOT NULL,
    "Direccion" character varying(500),
    "Telefono" character varying(50),
    "IsActive" boolean NOT NULL,
    CONSTRAINT "PK_Clientes" PRIMARY KEY ("Id")
);

CREATE TABLE "Galpones" (
    "Id" uuid NOT NULL,
    "Nombre" character varying(100) NOT NULL,
    "Capacidad" integer NOT NULL,
    "Ubicacion" character varying(255) NOT NULL,
    "IsActive" boolean NOT NULL,
    CONSTRAINT "PK_Galpones" PRIMARY KEY ("Id")
);

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

CREATE TABLE "Productos" (
    "Id" uuid NOT NULL,
    "Nombre" character varying(150) NOT NULL,
    "Tipo" character varying(30) NOT NULL,
    "UnidadMedida" character varying(20) NOT NULL,
    "IsActive" boolean NOT NULL,
    CONSTRAINT "PK_Productos" PRIMARY KEY ("Id")
);

CREATE TABLE "Usuarios" (
    "Id" uuid NOT NULL,
    "FirebaseUid" character varying(128) NOT NULL,
    "Nombre" character varying(150) NOT NULL,
    "Rol" character varying(50) NOT NULL,
    "IsActive" boolean NOT NULL,
    CONSTRAINT "PK_Usuarios" PRIMARY KEY ("Id")
);

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

CREATE TABLE "Mortalidades" (
    "Id" uuid NOT NULL,
    "LoteId" uuid NOT NULL,
    "Fecha" timestamp with time zone NOT NULL,
    "CantidadBajas" integer NOT NULL,
    "IsActive" boolean NOT NULL,
    CONSTRAINT "PK_Mortalidades" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Mortalidades_Lotes_LoteId" FOREIGN KEY ("LoteId") REFERENCES "Lotes" ("Id") ON DELETE RESTRICT
);

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

CREATE INDEX "IX_CalendarioSanitario_LoteId" ON "CalendarioSanitario" ("LoteId");

CREATE INDEX "IX_GastosOperativos_GalponId" ON "GastosOperativos" ("GalponId");

CREATE INDEX "IX_GastosOperativos_LoteId" ON "GastosOperativos" ("LoteId");

CREATE INDEX "IX_Mortalidades_LoteId" ON "Mortalidades" ("LoteId");

CREATE INDEX "IX_MovimientosInventario_LoteId" ON "MovimientosInventario" ("LoteId");

CREATE INDEX "IX_MovimientosInventario_ProductoId" ON "MovimientosInventario" ("ProductoId");

CREATE UNIQUE INDEX "IX_Usuarios_FirebaseUid" ON "Usuarios" ("FirebaseUid");

CREATE INDEX "IX_Ventas_ClienteId" ON "Ventas" ("ClienteId");

CREATE INDEX "IX_Ventas_LoteId" ON "Ventas" ("LoteId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260410204547_AddCalendarioSanitario', '10.0.5');

COMMIT;

START TRANSACTION;
ALTER TABLE "Usuarios" ALTER COLUMN "Nombre" TYPE character varying(100);

ALTER TABLE "Usuarios" ADD "Apellidos" character varying(100) NOT NULL DEFAULT '';

ALTER TABLE "Usuarios" ADD "Direccion" character varying(200) NOT NULL DEFAULT '';

ALTER TABLE "Usuarios" ADD "FechaNacimiento" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';

ALTER TABLE "Usuarios" ADD "Profesion" character varying(100) NOT NULL DEFAULT '';

ALTER TABLE "Mortalidades" ADD "Causa" text NOT NULL DEFAULT '';

ALTER TABLE "GastosOperativos" ADD "TipoGasto" text NOT NULL DEFAULT '';

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260411001128_AddPerfilUsuario', '10.0.5');

COMMIT;

START TRANSACTION;
ALTER TABLE "Usuarios" ADD "Email" character varying(150) NOT NULL DEFAULT '';

CREATE UNIQUE INDEX "IX_Usuarios_Email" ON "Usuarios" ("Email");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260411004209_AddEmailToUsuario', '10.0.5');

COMMIT;

START TRANSACTION;
ALTER TABLE "Usuarios" ALTER COLUMN "Rol" TYPE integer USING (CASE WHEN "Rol" = 'Admin' THEN 2 WHEN "Rol" = 'SubAdmin' THEN 1 ELSE 0 END);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260411023802_RefactorRolesEnum', '10.0.5');

COMMIT;

START TRANSACTION;
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

CREATE INDEX "IX_PesajesLote_LoteId" ON "PesajesLote" ("LoteId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260411024932_AddPesajeLote', '10.0.5');

COMMIT;

START TRANSACTION;
ALTER TABLE "Ventas" RENAME COLUMN "PrecioUnitario" TO "PrecioPorKilo";

ALTER TABLE "Ventas" ADD "PesoTotalVendido" numeric(18,2) NOT NULL DEFAULT 0.0;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260411161111_UpdateVentaToWeightBased', '10.0.5');

COMMIT;

START TRANSACTION;
ALTER TABLE "MovimientosInventario" ADD "Justificacion" text;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260411182705_AddJustificacionToMovimiento', '10.0.5');

COMMIT;

START TRANSACTION;
ALTER TABLE "Ventas" ADD "UsuarioId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';

ALTER TABLE "PesajesLote" ADD "UsuarioId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';

ALTER TABLE "MovimientosInventario" ADD "UsuarioId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';

ALTER TABLE "Mortalidades" ADD "UsuarioId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';

ALTER TABLE "GastosOperativos" ADD "UsuarioId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260411190649_AddAuditoriaUsuarios', '10.0.5');

COMMIT;

START TRANSACTION;
ALTER TABLE "Ventas" ADD "FechaCreacion" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';

ALTER TABLE "Ventas" ADD "FechaModificacion" timestamp with time zone;

ALTER TABLE "Ventas" ADD "UsuarioCreacionId" uuid;

ALTER TABLE "Ventas" ADD "UsuarioModificacionId" uuid;

ALTER TABLE "Usuarios" ADD "FechaCreacion" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';

ALTER TABLE "Usuarios" ADD "FechaModificacion" timestamp with time zone;

ALTER TABLE "Usuarios" ADD "UsuarioCreacionId" uuid;

ALTER TABLE "Usuarios" ADD "UsuarioModificacionId" uuid;

ALTER TABLE "Productos" ADD "FechaCreacion" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';

ALTER TABLE "Productos" ADD "FechaModificacion" timestamp with time zone;

ALTER TABLE "Productos" ADD "UsuarioCreacionId" uuid;

ALTER TABLE "Productos" ADD "UsuarioModificacionId" uuid;

ALTER TABLE "PesajesLote" ADD "FechaCreacion" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';

ALTER TABLE "PesajesLote" ADD "FechaModificacion" timestamp with time zone;

ALTER TABLE "PesajesLote" ADD "UsuarioCreacionId" uuid;

ALTER TABLE "PesajesLote" ADD "UsuarioModificacionId" uuid;

ALTER TABLE "MovimientosInventario" ADD "FechaCreacion" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';

ALTER TABLE "MovimientosInventario" ADD "FechaModificacion" timestamp with time zone;

ALTER TABLE "MovimientosInventario" ADD "UsuarioCreacionId" uuid;

ALTER TABLE "MovimientosInventario" ADD "UsuarioModificacionId" uuid;

ALTER TABLE "Mortalidades" ADD "FechaCreacion" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';

ALTER TABLE "Mortalidades" ADD "FechaModificacion" timestamp with time zone;

ALTER TABLE "Mortalidades" ADD "UsuarioCreacionId" uuid;

ALTER TABLE "Mortalidades" ADD "UsuarioModificacionId" uuid;

ALTER TABLE "Lotes" ADD "FechaCreacion" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';

ALTER TABLE "Lotes" ADD "FechaModificacion" timestamp with time zone;

ALTER TABLE "Lotes" ADD "UsuarioCreacionId" uuid;

ALTER TABLE "Lotes" ADD "UsuarioModificacionId" uuid;

ALTER TABLE "GastosOperativos" ADD "FechaCreacion" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';

ALTER TABLE "GastosOperativos" ADD "FechaModificacion" timestamp with time zone;

ALTER TABLE "GastosOperativos" ADD "UsuarioCreacionId" uuid;

ALTER TABLE "GastosOperativos" ADD "UsuarioModificacionId" uuid;

ALTER TABLE "Galpones" ADD "FechaCreacion" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';

ALTER TABLE "Galpones" ADD "FechaModificacion" timestamp with time zone;

ALTER TABLE "Galpones" ADD "UsuarioCreacionId" uuid;

ALTER TABLE "Galpones" ADD "UsuarioModificacionId" uuid;

ALTER TABLE "Clientes" ADD "FechaCreacion" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';

ALTER TABLE "Clientes" ADD "FechaModificacion" timestamp with time zone;

ALTER TABLE "Clientes" ADD "UsuarioCreacionId" uuid;

ALTER TABLE "Clientes" ADD "UsuarioModificacionId" uuid;

ALTER TABLE "CalendarioSanitario" ADD "FechaCreacion" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';

ALTER TABLE "CalendarioSanitario" ADD "FechaModificacion" timestamp with time zone;

ALTER TABLE "CalendarioSanitario" ADD "UsuarioCreacionId" uuid;

ALTER TABLE "CalendarioSanitario" ADD "UsuarioModificacionId" uuid;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260411201320_AddAutomaticAuditing', '10.0.5');

COMMIT;

START TRANSACTION;
ALTER TABLE "Ventas" ADD "EstadoPago" integer NOT NULL DEFAULT 1;

ALTER TABLE "Lotes" ADD "CostoTotalFinal" numeric(18,2);

ALTER TABLE "Lotes" ADD "FCRFinal" numeric(18,2);

ALTER TABLE "Lotes" ADD "PorcentajeMortalidadFinal" numeric(18,2);

ALTER TABLE "Lotes" ADD "UtilidadNetaFinal" numeric(18,2);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260411205454_AddIntegridadContableYPagos', '10.0.5');

COMMIT;

START TRANSACTION;
CREATE INDEX "IX_Ventas_Fecha" ON "Ventas" ("Fecha");

CREATE INDEX "IX_MovimientosInventario_Fecha" ON "MovimientosInventario" ("Fecha");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260411205634_AddPerformanceIndexes', '10.0.5');

COMMIT;

START TRANSACTION;
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

ALTER TABLE "Productos" ADD "CategoriaProductoId" uuid;

ALTER TABLE "Productos" ADD "PesoUnitarioKg" numeric(18,4) NOT NULL DEFAULT 1.0;

ALTER TABLE "Productos" ADD "UnidadMedidaId" uuid;


                INSERT INTO "CategoriasProductos" ("Id", "Nombre", "IsActive", "FechaCreacion") VALUES 
                ('0c19aed0-4e96-4116-8439-b103eea9d2f1', 'Alimento', true, now()),
                ('9bb45574-1045-4fc3-9eaf-bf5723e5276f', 'Medicamento', true, now()),
                ('0604f076-987a-49b5-8395-b05c7bb2518a', 'Insumo', true, now()),
                ('844c1f54-4d92-49cb-8a92-cdfa1c312069', 'Otro', true, now());

                INSERT INTO "UnidadesMedida" ("Id", "Nombre", "Abreviatura", "IsActive", "FechaCreacion") VALUES 
                ('39f0f4cc-7e58-4430-8e08-269d8291b9c2', 'Kilogramo', 'Kg', true, now()),
                ('5de2d4db-3453-4018-9a5e-fbcbf3e14ac2', 'Unidad', 'Und', true, now()),
                ('8e5d45c8-b248-48ab-a5cb-49213a0e55aa', 'Litro', 'L', true, now()),
                ('ce494f67-ab61-4977-a768-5d1e2283de3f', 'Saco', 'Sc', true, now());
            


                UPDATE "Productos" SET "CategoriaProductoId" = '0c19aed0-4e96-4116-8439-b103eea9d2f1' WHERE "Tipo" = 'Alimento';
                UPDATE "Productos" SET "CategoriaProductoId" = '9bb45574-1045-4fc3-9eaf-bf5723e5276f' WHERE "Tipo" = 'Medicamento';
                UPDATE "Productos" SET "CategoriaProductoId" = '0604f076-987a-49b5-8395-b05c7bb2518a' WHERE "Tipo" = 'Insumo';
                UPDATE "Productos" SET "CategoriaProductoId" = '844c1f54-4d92-49cb-8a92-cdfa1c312069' WHERE "CategoriaProductoId" IS NULL;
            


                UPDATE "Productos" SET "UnidadMedidaId" = '39f0f4cc-7e58-4430-8e08-269d8291b9c2', "PesoUnitarioKg" = 1.0 WHERE "UnidadMedida" = 'Kg';
                UPDATE "Productos" SET "UnidadMedidaId" = '5de2d4db-3453-4018-9a5e-fbcbf3e14ac2', "PesoUnitarioKg" = 1.0 WHERE "UnidadMedida" = 'Unidad';
                UPDATE "Productos" SET "UnidadMedidaId" = '8e5d45c8-b248-48ab-a5cb-49213a0e55aa', "PesoUnitarioKg" = 1.0 WHERE "UnidadMedida" = 'Litro';
                UPDATE "Productos" SET "UnidadMedidaId" = 'ce494f67-ab61-4977-a768-5d1e2283de3f', "PesoUnitarioKg" = 40.0 WHERE "UnidadMedida" = 'Saco';
                UPDATE "Productos" SET "UnidadMedidaId" = '5de2d4db-3453-4018-9a5e-fbcbf3e14ac2' WHERE "UnidadMedidaId" IS NULL;
            

ALTER TABLE "Productos" DROP COLUMN "Tipo";

ALTER TABLE "Productos" DROP COLUMN "UnidadMedida";

CREATE INDEX "IX_Productos_CategoriaProductoId" ON "Productos" ("CategoriaProductoId");

CREATE INDEX "IX_Productos_UnidadMedidaId" ON "Productos" ("UnidadMedidaId");

ALTER TABLE "Productos" ADD CONSTRAINT "FK_Productos_CategoriasProductos_CategoriaProductoId" FOREIGN KEY ("CategoriaProductoId") REFERENCES "CategoriasProductos" ("Id") ON DELETE RESTRICT;

ALTER TABLE "Productos" ADD CONSTRAINT "FK_Productos_UnidadesMedida_UnidadMedidaId" FOREIGN KEY ("UnidadMedidaId") REFERENCES "UnidadesMedida" ("Id") ON DELETE RESTRICT;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260411214552_AddCatalogosDinamicos', '10.0.5');

COMMIT;

START TRANSACTION;
ALTER TABLE "Lotes" ADD "GalponId" uuid;


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
            

CREATE INDEX "IX_Lotes_GalponId" ON "Lotes" ("GalponId");

ALTER TABLE "Lotes" ADD CONSTRAINT "FK_Lotes_Galpones_GalponId" FOREIGN KEY ("GalponId") REFERENCES "Galpones" ("Id") ON DELETE RESTRICT;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260411230854_AddGalponIdToLote', '10.0.5');

COMMIT;

START TRANSACTION;
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

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260411231251_AddAuditoriaLogs', '10.0.5');

COMMIT;

START TRANSACTION;
ALTER TABLE "Ventas" ALTER COLUMN "EstadoPago" SET DEFAULT 2;

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

CREATE INDEX "IX_PagosVentas_FechaPago" ON "PagosVentas" ("FechaPago");

CREATE INDEX "IX_PagosVentas_VentaId" ON "PagosVentas" ("VentaId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260411235541_AddPagosCuentasPorCobrar', '10.0.5');

COMMIT;

START TRANSACTION;
ALTER TABLE "CalendarioSanitario" ADD "ProductoIdRecomendado" uuid;

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

CREATE INDEX "IX_CalendarioSanitario_ProductoIdRecomendado" ON "CalendarioSanitario" ("ProductoIdRecomendado");

CREATE INDEX "IX_ActividadesPlantillas_PlantillaId" ON "ActividadesPlantillas" ("PlantillaId");

CREATE INDEX "IX_ActividadesPlantillas_ProductoIdRecomendado" ON "ActividadesPlantillas" ("ProductoIdRecomendado");

ALTER TABLE "CalendarioSanitario" ADD CONSTRAINT "FK_CalendarioSanitario_Productos_ProductoIdRecomendado" FOREIGN KEY ("ProductoIdRecomendado") REFERENCES "Productos" ("Id") ON DELETE RESTRICT;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260412000025_AddPlantillasSanitarias', '10.0.5');

COMMIT;

START TRANSACTION;
ALTER TABLE "Lotes" ADD "JustificacionCancelacion" text;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260412000627_AddJustificacionCancelacion', '10.0.5');

COMMIT;

START TRANSACTION;
ALTER TABLE "Productos" ADD "UmbralMinimo" numeric(18,2) NOT NULL DEFAULT 0.0;

ALTER TABLE "MovimientosInventario" ADD "CostoTotal" numeric(18,2);

ALTER TABLE "MovimientosInventario" ADD "Proveedor" character varying(200);

ALTER TABLE "CalendarioSanitario" ADD "EsManual" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "CalendarioSanitario" ADD "Justificacion" character varying(500);

ALTER TABLE "CalendarioSanitario" ADD "Tipo" character varying(30) NOT NULL DEFAULT 'Otros';

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260412024051_AddCalendarioSanitarioFields', '10.0.5');

COMMIT;

START TRANSACTION;
ALTER TABLE "MovimientosInventario" ADD "CompraId" uuid;

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

CREATE INDEX "IX_MovimientosInventario_CompraId" ON "MovimientosInventario" ("CompraId");

CREATE INDEX "IX_ComprasInventario_Fecha" ON "ComprasInventario" ("Fecha");

CREATE INDEX "IX_ComprasInventario_ProveedorId" ON "ComprasInventario" ("ProveedorId");

CREATE UNIQUE INDEX "IX_Proveedores_NitRuc" ON "Proveedores" ("NitRuc");

ALTER TABLE "MovimientosInventario" ADD CONSTRAINT "FK_MovimientosInventario_ComprasInventario_CompraId" FOREIGN KEY ("CompraId") REFERENCES "ComprasInventario" ("Id") ON DELETE SET NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260412035059_AddProveedoresYCompras', '10.0.5');

COMMIT;

START TRANSACTION;
ALTER TABLE "Productos" ADD "CostoUnitarioActual" numeric(18,4) NOT NULL DEFAULT 0.0;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260412035655_AddCostoPPP', '10.0.5');

COMMIT;

START TRANSACTION;
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

CREATE INDEX "IX_PagosCompras_CompraId" ON "PagosCompras" ("CompraId");

CREATE INDEX "IX_PagosCompras_FechaPago" ON "PagosCompras" ("FechaPago");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260412041811_AddPagoCompra', '10.0.5');

COMMIT;

START TRANSACTION;
ALTER TABLE "Usuarios" ADD "CodigoVinculacion" character varying(10);

ALTER TABLE "Usuarios" ADD "FechaExpiracionCodigo" timestamp with time zone;

ALTER TABLE "Usuarios" ADD "Telefono" character varying(20);

ALTER TABLE "Usuarios" ADD "WhatsAppNumero" character varying(20);

ALTER TABLE "ComprasInventario" ADD "FechaVencimiento" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';

ALTER TABLE "AuditoriaLogs" ADD "Detalles" text NOT NULL DEFAULT '';

ALTER TABLE "AuditoriaLogs" ADD "EntidadNombre" text NOT NULL DEFAULT '';

ALTER TABLE "AuditoriaLogs" ADD "UsuarioNombre" text NOT NULL DEFAULT '';

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

CREATE UNIQUE INDEX "IX_Usuarios_Telefono" ON "Usuarios" ("Telefono") WHERE "Telefono" IS NOT NULL;

CREATE INDEX "IX_MensajesChat_ConversacionId" ON "MensajesChat" ("ConversacionId");

CREATE INDEX "IX_OrdenesCompra_ProveedorId" ON "OrdenesCompra" ("ProveedorId");

CREATE INDEX "IX_OrdenesCompraItems_OrdenCompraId" ON "OrdenesCompraItems" ("OrdenCompraId");

CREATE INDEX "IX_OrdenesCompraItems_ProductoId" ON "OrdenesCompraItems" ("ProductoId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260413000450_AddRegistroBienestarAndFixUsuario', '10.0.5');

COMMIT;

START TRANSACTION;
ALTER TABLE "Conversaciones" ADD "Titulo" text NOT NULL DEFAULT '';

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260413003405_AddTituloToConversacion', '10.0.5');

COMMIT;

START TRANSACTION;
ALTER TABLE "Lotes" ADD "Nombre" character varying(100) NOT NULL DEFAULT '';

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260415001543_AddNombreToLote', '10.0.5');

COMMIT;

START TRANSACTION;

                DO $$ 
                BEGIN 
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='Productos' AND column_name='PesoUnitarioKg') THEN
                        ALTER TABLE "Productos" ADD COLUMN "PesoUnitarioKg" numeric(18,4) NOT NULL DEFAULT 1.0;
                    END IF;
                END $$;
            

ALTER TABLE "Productos" ADD "StockActualKg" numeric(18,4) NOT NULL DEFAULT 0.0;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260415161429_EnsureMissingColumns', '10.0.5');

COMMIT;

START TRANSACTION;
ALTER TABLE "Productos" ADD "StockActual" numeric(18,4) NOT NULL DEFAULT 0.0;

ALTER TABLE "CalendarioSanitario" ADD "CantidadRecomendada" numeric(18,4) NOT NULL DEFAULT 0.0;

ALTER TABLE "ActividadesPlantillas" ADD "CantidadRecomendada" numeric(18,4) NOT NULL DEFAULT 0.0;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260415181814_AddStockActualAndRecommendedQuantity', '10.0.5');

COMMIT;

START TRANSACTION;
ALTER TABLE "UnidadesMedida" ADD "Tipo" integer NOT NULL DEFAULT 0;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260415192126_AddTipoUnidadToUnidadMedida', '10.0.5');

COMMIT;

START TRANSACTION;
ALTER TABLE "Productos" ALTER COLUMN "StockActualKg" TYPE numeric(18,6);

ALTER TABLE "Productos" ALTER COLUMN "StockActual" TYPE numeric(18,6);

ALTER TABLE "Productos" ALTER COLUMN "PesoUnitarioKg" TYPE numeric(18,6);

ALTER TABLE "MovimientosInventario" ALTER COLUMN "Cantidad" TYPE numeric(18,6);

ALTER TABLE "MovimientosInventario" ADD "PesoUnitarioHistorico" numeric(18,6) NOT NULL DEFAULT 0.0;


                UPDATE "MovimientosInventario"
                SET "PesoUnitarioHistorico" = p."PesoUnitarioKg"
                FROM "Productos" p
                WHERE "MovimientosInventario"."ProductoId" = p."Id"
            


                UPDATE "UnidadesMedida" SET "Tipo" = 0 WHERE "Abreviatura" IN ('Kg', 'kg', 'Kilos');
                UPDATE "UnidadesMedida" SET "Tipo" = 2 WHERE "Abreviatura" IN ('Und', 'un', 'Unidad', 'Pza');
                UPDATE "UnidadesMedida" SET "Tipo" = 1 WHERE "Abreviatura" IN ('Lt', 'lt', 'Litro');
            

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260415200002_AddPesoHistoricoToMovimientos', '10.0.5');

COMMIT;

START TRANSACTION;
ALTER TABLE "Usuarios" ADD "Active" integer NOT NULL DEFAULT 1;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260417010921_AddActiveToUsuario', '10.0.5');

COMMIT;

START TRANSACTION;
ALTER TABLE "RegistroBienestar" ADD "CloroPpm" numeric;

ALTER TABLE "RegistroBienestar" ADD "Ph" numeric;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260419013730_AddPhAndCloroPpmToRegistroBienestar', '10.0.5');

COMMIT;

