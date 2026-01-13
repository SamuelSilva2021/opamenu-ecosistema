-- Habilitar a extensão primeiro (fora do bloco DO)
CREATE EXTENSION IF NOT EXISTS pgcrypto;

DO LANGUAGE plpgsql $$
DECLARE
    v_now           TIMESTAMP := now() AT TIME ZONE 'utc';

    v_role_id       UUID;
    v_group_type_id UUID;
    v_group_id      UUID;
    v_user_id       UUID;

    v_password_hash TEXT;
BEGIN
    -- Gerar o hash dentro do bloco BEGIN
    v_password_hash := crypt('Abc@123', gen_salt('bf'));

    -- 1. Role SUPER_ADMIN
    SELECT id
    INTO v_role_id
    FROM public.role
    WHERE code = 'SUPER_ADMIN';

    IF v_role_id IS NULL THEN
        v_role_id := gen_random_uuid();

        INSERT INTO public.role (
            id, name, description, code, is_active, created_at
        )
        VALUES (
            v_role_id,
            'SuperAdmin',
            'Acesso total ao sistema',
            'SUPER_ADMIN',
            TRUE,
            v_now
        );
    END IF;

    -- 2. GroupType SYSTEM
    SELECT id
    INTO v_group_type_id
    FROM public.group_type
    WHERE code = 'SYSTEM'
    LIMIT 1;

    IF v_group_type_id IS NULL THEN
        v_group_type_id := gen_random_uuid();

        INSERT INTO public.group_type (
            id, name, description, code, is_active, created_at
        )
        VALUES (
            v_group_type_id,
            'System',
            'Grupos de Sistema',
            'SYSTEM',
            TRUE,
            v_now
        );
    END IF;

    -- 3. AccessGroup ADMIN_GROUP
    SELECT id
    INTO v_group_id
    FROM public.access_group
    WHERE code = 'ADMIN_GROUP';

    IF v_group_id IS NULL THEN
        v_group_id := gen_random_uuid();

        INSERT INTO public.access_group (
            id, name, description, code, is_active, created_at, group_type_id
        )
        VALUES (
            v_group_id,
            'Administradores',
            'Grupo de administradores globais',
            'ADMIN_GROUP',
            TRUE,
            v_now,
            v_group_type_id
        );
    END IF;

    -- 4. Vincular Role ao Grupo
    IF NOT EXISTS (
        SELECT 1
        FROM public.role_access_group
        WHERE access_group_id = v_group_id
          AND role_id = v_role_id
    ) THEN
        INSERT INTO public.role_access_group (
            id, access_group_id, role_id, is_active, created_at
        )
        VALUES (
            gen_random_uuid(),
            v_group_id,
            v_role_id,
            TRUE,
            v_now
        );
    END IF;

    -- 5. Usuário admin
    SELECT id
    INTO v_user_id
    FROM public.user_account
    WHERE email = 'admin@opamenu.com';

    IF v_user_id IS NULL THEN
        v_user_id := gen_random_uuid();

        INSERT INTO public.user_account (
            id, username, email, password_hash,
            first_name, last_name, status, created_at
        )
        VALUES (
            v_user_id,
            'admin',
            'admin@opamenu.com',
            v_password_hash,
            'Super',
            'Admin',
            'Ativo',
            v_now
        );
    END IF;

    -- 6. Vincular usuário ao grupo
    IF NOT EXISTS (
        SELECT 1
        FROM public.account_access_group
        WHERE user_account_id = v_user_id
          AND access_group_id = v_group_id
    ) THEN
        INSERT INTO public.account_access_group (
            id, user_account_id, access_group_id, is_active, created_at
        )
        VALUES (
            gen_random_uuid(),
            v_user_id,
            v_group_id,
            TRUE,
            v_now
        );
    END IF;

    RAISE NOTICE 'Setup concluído com sucesso!';
    RAISE NOTICE 'Email: admin@opamenu.com';
    RAISE NOTICE 'Senha: Abc@123';

END $$;