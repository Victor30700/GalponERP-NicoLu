DO $$ 
DECLARE 
    t record;
BEGIN
    FOR t IN 
        SELECT table_name 
        FROM information_schema.tables 
        WHERE table_schema = 'public' 
        AND table_type = 'BASE TABLE'
    LOOP
        -- Verificar si la columna ya existe
        IF NOT EXISTS (
            SELECT 1 
            FROM information_schema.columns 
            WHERE table_name = t.table_name 
            AND column_name = 'Version'
        ) THEN
            EXECUTE format('ALTER TABLE %I ADD COLUMN "Version" bytea DEFAULT ''\x''::bytea', t.table_name);
        END IF;
    END LOOP;
END $$;
