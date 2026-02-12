# Análise e Requisitos: Opamenu Gestor Avançado

Este documento detalha o estado atual do projeto `opamenu-gestor` e as necessidades de implementação para torná-lo um gestor completo para deliverys e restaurantes, integrando o controle de acesso por módulos e operações já existente na API.

## Estado Atual do Projeto

O projeto utiliza Flutter com Riverpod para gerenciamento de estado e GoRouter para navegação. A arquitetura segue princípios de Clean Architecture.

### Funcionalidades Implementadas

- **Autenticação**: Login básico com armazenamento de tokens JWT e persistência segura.
- **Controle de Acesso (ACL)**: 
    - Proteção de rotas via `AppRouter` baseada em módulos.
    - Componentes condicionais `PermissionGate` na UI.
- **Dashboard**: Visualização de métricas de vendas e pedidos recentes.
- **PDV (POS)**: Busca de produtos, carrinho de compras e checkout.
- **Gestão de Mesas**: Listagem, criação, edição e geração de QR Code para mesas.
- **Gestão de Produtos (Catálogo)**: 
    - CRUD completo de Produtos (com imagens).
    - CRUD de Categorias.
    - CRUD de Adicionais e Grupos de Adicionais.
- **Gestão de Usuários**: 
    - Listagem de colaboradores.
    - Cadastro e edição com atribuição de Perfis (Roles).
- **KDS (Cozinha)**: 
    - Visualização Kanban de pedidos.
    - Gestão de status (Pendente -> Em Preparo -> Pronto).
    - Cronômetros de tempo de preparo.
- **Configurações**: 
    - Dados do Restaurante.
    - Configuração de Impressoras de rede (Cozinha/Balcão).

### Lacunas Identificadas (Placeholders/Futuro)

- **Delivery Avançado**: Integração com mapas, gestão de motoboys e rastreamento.
- **Notificações e Mensagens**: Funcionalidades planejadas mas não implementadas.
- **Relatórios Avançados**: Exportação de dados e gráficos mais detalhados.

---

## Requisitos de Implementação Pendentes

### 1. Fluxo de Delivery e Pedidos Avançado
Melhorar o gerenciamento de pedidos para suportar a complexidade logística do delivery.

- [x] **Identificação de Tipo de Pedido**: Diferenciar visualmente e logicamente Delivery, Retirada e Mesa no PDV e KDS.
- [ ] **Gestão de Entregadores**: Cadastro ou associação de entregadores aos pedidos "Saiu para Entrega".
- [x] **Endereços de Entrega**: Visualização de mapas ou integração com API de geolocalização para cálculo de taxas automática. (Parcial: Endereço estruturado implementado sem mapas)
- [ ] **Integração com Apps**: (Futuro) Webhook para receber pedidos de iFood/Rappi.

### 2. Notificações e Mensagens
Sistema para alertar o gestor sobre eventos importantes.

- [ ] **Centro de Notificações**: Tela para visualizar histórico de alertas.
- [x] **Alertas em Tempo Real**: Som ou Popup quando chega novo pedido (via WebSocket ou Polling otimizado).
- [ ] **Mensagens Internas**: Comunicação simples entre cozinha e balcão (opcional).

### 3. Qualidade e Testes
Garantir a estabilidade do sistema antes da escala.

- [ ] **Testes Unitários**: Aumentar cobertura nos Providers e Repositories.
- [ ] **Testes de Widget**: Validar fluxos críticos como Checkout e Fechamento de Pedido.
- [ ] **Tratamento de Erros Offline**: Melhorar comportamento quando sem internet (Sync posterior).

---

## Próximos Passos Sugeridos

1.  **Prioridade 1**: Refinar o fluxo de **Delivery** (Endereços, Taxas Dinâmicas).
2.  **Prioridade 2**: Implementar sistema de **Notificações em Tempo Real** para novos pedidos.
3.  **Prioridade 3**: Escrever testes automatizados para os fluxos críticos (PDV e KDS).

---

## Insights para Implementação Futura (Backlog)

### 1. Dashboard Financeiro (Caixa)
- [ ] Implementar aba específica para **Controle de Caixa**.
- [ ] Exibir dados consolidados por período (dia, semana, mês).
- [ ] Visualização de entradas, saídas e saldo.

### 2. Regras de Negócio
- [ ] **Bloqueio de Pedidos**: Impedir abertura de novos pedidos quando o status da loja for "Fechado".

### 3. Controle de Estoque Avançado
Funcionalidade opcional para clientes que desejam gestão detalhada.

- [ ] **Configuração de Estoque**: Flag no cadastro da empresa/configurações para ativar/desativar o módulo.
    - *Caso desativado*: Segue o padrão atual de cadastro de produtos simples.
    - *Caso ativado*: Habilita as funções abaixo.
- [ ] **Integração Produção e Vendas**: Baixa automática de estoque ao realizar vendas.
- [ ] **Ficha Técnica**: Possibilidade de "montar" produtos compostos por itens de estoque (ex: Hambúrguer = 1 Pão + 1 Carne + 1 Queijo).
