# Análise e Requisitos: Opamenu Gestor Avançado

Este documento detalha o estado atual do projeto `opamenu-gestor` e as necessidades de implementação para torná-lo um gestor completo para deliverys e restaurantes, integrando o controle de acesso por módulos e operações já existente na API.

## Estado Atual do Projeto

O projeto utiliza Flutter com Riverpod para gerenciamento de estado e GoRouter para navegação. A arquitetura segue princípios de Clean Architecture.

### Funcionalidades Implementadas
- **Autenticação**: Login básico com armazenamento de tokens JWT.
- **Dashboard**: Visualização de métricas de vendas e pedidos recentes.
- **PDV (POS)**: Busca de produtos, carrinho de compras e checkout básico.
- **Gestão de Mesas**: Listagem, criação, edição e geração de QR Code para mesas.
- **Pedidos**: Listagem geral de pedidos com visualização de detalhes.

### Lacunas Identificadas (Placeholders)
- **Gestão de Produtos**: Atualmente é uma página de placeholder. Necessita de CRUD completo.
- **Gestão de Usuários**: Atualmente é uma página de placeholder. Necessita de gestão de colaboradores e perfis de acesso.
- **Configurações**: Atualmente é uma página de placeholder. Necessita de configurações do restaurante (horários, taxas, etc).
- **Notificações e Mensagens**: Funcionalidades planejadas mas não implementadas.

---

## Requisitos de Implementação

### 1. Sistema de Controle de Acesso (ACL)
Integrar o sistema de permissões baseado na `opamenu-api`.

- [ ] **Mapeamento de Permissões**: Criar modelos para representar `Modulos` e `Operações` retornados pela API.
- [ ] **Persistência de Perfil**: Armazenar as permissões do usuário logado localmente (Secure Storage).
- [ ] **Guarda de Rotas**: Implementar um `PermissionGuard` no `GoRouter` para impedir acesso a módulos não autorizados.
- [ ] **Componentes Condicionais**: Criar um widget `PermissionGate` para ocultar botões ou seções baseadas em operações específicas (ex: ocultar o botão "Excluir" se o usuário não tiver permissão `DELETE`).

### 2. Módulo de Gestão de Catálogo (Produtos)
Transformar o placeholder em um módulo funcional.

- [ ] **CRUD de Categorias**: Criação e organização de categorias de produtos.
- [ ] **CRUD de Adicionais**: Gestão de grupos de adicionais e itens extras.
- [ ] **CRUD de Produtos**: Cadastro detalhado com fotos, preços e variações.

### 3. Módulo de Gestão de Usuários e Perfis
Integrar com as novas rotas da API para permitir que o gestor administre sua equipe.

- [ ] **Listagem de Colaboradores**: Visualização e busca de funcionários.
- [ ] **Atribuição de Perfis**: Definir quais cargos/perfis cada colaborador possui.
- [ ] **Gestão de Perfis (Roles)**: (Opcional para o Gestor) Se permitido, gerenciar as permissões de cada perfil.

### 4. Fluxo de Delivery e Pedidos Avançado
Melhorar o gerenciamento de pedidos para suportar múltiplos tipos.

- [ ] **Identificação de Tipo de Pedido**: Diferenciar claramente Delivery, Retirada e Mesa.
- [ ] **Gestão de Status**: Implementar transições de status (Aguardando -> Em Produção -> Saiu para Entrega -> Entregue).
- [ ] **Endereços de Entrega**: Visualização de mapas ou integração com apps de entrega para pedidos de Delivery.

### 5. Configurações do Estabelecimento
- [ ] **Dados do Restaurante**: Nome, Logo, Endereço e Contatos.
- [ ] **Horários de Funcionamento**: Gestão de turnos e dias de abertura.
- [ ] **Taxas e Regras**: Configuração de taxas de entrega e tempos estimados.

---

## Próximos Passos Sugeridos

1. **Prioridade 1**: Implementar a lógica de permissões no `AuthNotifier` e `AppRouter`.
2. **Prioridade 2**: Desenvolver o CRUD de Produtos para viabilizar o uso completo do PDV.
3. **Prioridade 3**: Implementar o fluxo completo de estados de pedidos (KDS simplification).
