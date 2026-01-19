-- Script de Atualização para Consistência do Controle de Acesso
-- Execute este script para garantir que os valores no banco de dados
-- estejam alinhados com as constantes definidas na aplicação (C#).

-- 1. Padronizar Operações
-- Atualiza SELECT para READ
UPDATE operation 
SET value = 'READ', name = 'Read' 
WHERE value = 'SELECT';

-- Atualiza INSERT para CREATE
UPDATE operation 
SET value = 'CREATE', name = 'Create' 
WHERE value = 'INSERT';

-- 2. Padronizar Chaves de Módulos
-- Remove o sufixo _PERMISSION de todas as chaves de módulo (ex: DASHBOARD_PERMISSION -> DASHBOARD)
UPDATE module 
SET key = REPLACE(key, '_PERMISSION', '') 
WHERE key LIKE '%_PERMISSION';

-- 3. Verificação (Opcional)
-- SELECT * FROM operation;
-- SELECT * FROM module;
