DO $$ 
BEGIN 
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='Productos' AND column_name='PesoUnitarioKg') THEN
        ALTER TABLE "Productos" ADD COLUMN "PesoUnitarioKg" numeric(18,4) NOT NULL DEFAULT 1.0;
    END IF;
END $$;
