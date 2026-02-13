# 🏆 Sistema de Fidelidade OpaMenu v2

O sistema de fidelidade do OpaMenu é uma engine de regras flexível que permite aos restaurantes gerenciar múltiplos programas de recompensas simultâneos, com segmentação por itens e tipos de acúmulo dinâmicos.

## 🛠️ Arquitetura Técnica

### Modelagem de Dados (Backend)
- **Engine de Regras**: Processa múltiplos programas ativos. Um único pedido pode pontuar em diferentes regras independentes.
- **Enums Compartilhados**: Localizados em `opamenu-commons`, definem os tipos de acúmulo e recompensas.
    - `ELoyaltyProgramType`: PointsPerValue (0), OrderCount (1), ItemCount (2).
    - `ELoyaltyRewardType`: DiscountPercentage (0), DiscountValue (1), FreeProduct (2).

### Implementação Frontend (`opamenu-painel`)
- **Segurança**: Baseada no hook `usePermission`. O botão de criação e ações de edição/exclusão seguem permissões granulares (`LOYALTY:CREATE`, `LOYALTY:UPDATE`, etc).
- **Sincronização de Estado**: Utiliza `@tanstack/react-query` para cache e invalidação automática de listas após mutações.
- **Robustez de Formulário**:
    - **Key-Remounting**: O `LoyaltyForm` utiliza `key={editingProgram?.id}` para forçar um remount limpo, garantindo que o estado interno do `react-hook-form` seja zerado entre diferentes programas.
    - **Defensive Casting**: Conversão explícita de tipos numéricos do backend para garantir compatibilidade com componentes `Select` e `Input`.

## ✨ UI/UX Patterns (Design System)

### Visualização em Cards
- **Hierarquia Clara**: Uso de ícones dinâmicos (`Award`, `Calendar`) e badges coloridos para indicar status (Ativo/Inativo).
- **Progressão**: Barras de progresso visuais para representar a meta de resgate.
- **Micro-interações**: Hover effects e botões de ação rápidos (Editar/Excluir) que aparecem contextualmente.

### Formulário de Gestão
- **Dialog Flexível**: Janelas de edição widen (1024px) para acomodar layouts complexos sem claustrofobia.
- **Alinhamento Cirúrgico**: Design em grid sistemático (`gap-6`) com alinhamento perfeito de inputs em ambos os eixos, independente das descrições ou estados dinâmicos.
- **Interações Modernas**: Substituição de alertas nativos por `AlertDialog` estilizados.

## 🧠 Smart Features

### Descrição Automática Inteligente
- **Agrupamento de Itens**: Ao selecionar produtos para o tipo `ItemCount`, o sistema extrai o "nome base" (ex: "Pizza" de "Pizza Calabresa").
- **Deduplicação**: Se múltiplos sabores da mesma categoria forem selecionados, a descrição lista apenas o grupo principal, mantendo o texto conciso e legível para o cliente final.
- **Texto Natural**: Gera listas gramaticalmente corretas usando vírgulas e conjunções ("e").

---

## 🚀 Como Utilizar
1. **Configurar Programa**: Defina o nome e o tipo (Valor, Pedido ou Item).
2. **Estabelecer Meta**: Defina quantos pontos/itens são necessários para o prêmio.
3. **Escolher Recompensa**: Selecione se o prêmio é desconto ou produto grátis.
4. **Filtros (Opcional)**: Se for um programa de itens, selecione os produtos ou categorias participantes.
5. **Monitorar**: Acompanhe o status "Em vigor" diretamente na listagem principal.
