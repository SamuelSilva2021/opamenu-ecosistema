# Precificação do Sistema Opamenu

Este documento descreve a proposta de preços e pacotes para o ecossistema Opamenu - Sistema de Gestão para Restaurantes e Delivery, considerando os módulos atualmente implementados e a proposta de valor para atender restaurantes, lanchonetes e estabelecimentos que operam tanto no salão quanto em delivery/retirada.

## 📊 Pacotes e Preços Mensais

| Pacote | Valor Mensal | Desconto Anual* | Ideal Para |
|--------|--------------|-----------------|------------|
| **Básico** | R$ 249,90 | R$ 2.549,00 (15% off) | Estabelecimentos pequenos com foco principal no salão |
| **Standard** | R$ 349,90 | R$ 3.569,00 (15% off) | Restaurantes e lanchonetes que operam nos dois modelos (salão + delivery) |
| **Premium** | R$ 499,90 | R$ 5.099,00 (15% off) | Médios e grandes estabelecimentos com necessidade de controle avançado |

*Desconto anual aplicado ao pagar 12 meses antecipadamente

### 💰 Taxa de Implementação
- **Taxa única de setup/configuração inicial**: R$ 299,90
- Inclui: configuração inicial do cardápio, mapeamento de mesas, treinamento básico de 1 hora

---

## 📦 Detalhes dos Pacotes

### Pacote Básico - R$ 249,90/mês
**Para estabelecimentos que priorizam operações no salão com controle básico**

**Inclui:**
- PDV/Caixa completo com controle de formas de pagamento (dinheiro, cartão, PIX, voucher, pagamento dividido)
- Sistema de mesas e comandas (múltiplas comandas por mesa)
- Controle básico de estoque (cadastro de produtos, entradas/saidas manuais)
- Relatórios essenciais: vendas do dia, por produto, por forma de pagamento
- Acesso ao painel administrativo web
- Suporte para até 1 terminal PDV + 1 comanda digital
- Atualizações e suporte técnico básico

**Não inclui:**
- Cardápio digital para clientes
- Sistema de delivery/retirada
- Aplicativo para garçons
- Gestão de cozinha (KDS)
- Integrações externas
- Relatórios avançados

---

### Pacote Standard - R$ 349,90/mês
**Pacote recomendado para operação completa em salão e delivery**

**Inclui (tudo do Básico +):**
- Cardápio digital responsivo (opamenu-cardapio) para clientes acessarem via QR Code ou link
- Sistema completo de pedidos para delivery e retirada (tipos 3 e 4)
- Aplicativo para garçons/atendimento (funcionalidades completas)
- Gestão básica de cozinha (exibição de pedidos por setor)
- Integração com WhatsApp para automação de pedidos
- Relatórios avançados: vendas por garçom, por mesa, por período, produtividade
- Suporte para até 3 terminais PDV + 2 comandas digitais
- Backup automático e segurança básica

**Fluxo de delivery suportado:**
1. Cliente faz pedido via cardápio digital ou WhatsApp
2. Pedido entra no sistema como "Delivery" (tipo 3)
3. Cozinha recebe o pedido via KDS
4. Após preparo, status muda para "Pronto"
5. Entregador retira o pedido (status: "Saiu para entrega")
6. Confirmação de entrega finaliza o pedido

---

### Pacote Premium - R$ 499,90/mês
**Para estabelecimentos que necessitam de controle avançado e escalabilidade**

**Inclui (tudo do Standard +):**
- Gestão avançada de estoque com fichas técnicas e baixa automática de ingredientes
- Controle de validade e alertas de reposição
- Relatórios de BI personalizáveis e exportáveis
- Preparação para integrações com iFood/Rappi/Uber Eats (webhooks prontos)
- Suporte ilimitado a terminais PDV e comandas digitais
- Treinamento inicial incluso (2 horas online + materiais)
- Suporte técnico prioritário (resposta em até 2h úteis)
- Personalização de cores e logo no cardápio digital
- Funcionalidade de promoções e cupons de desconto

---

## 🍽️ Atendimento a Delivery e Salão (Restaurante/Lanchonete)

O sistema Opamenu foi projetado desde sua arquitetura para atender **simultaneamente** ambos os modelos de operação, conforme detalhado em DOCS_OPAMENU.md:

### 🏢 Para Operações de Salão/Restaurante:
- **PDV/Caixa** com controle financeiro completo (sangria, suprimento, fechamento de caixa)
- **Sistema de Mesas e Comandas** que permite múltiplas comandas por mesa (ideal para grupos e consumos individuais)
- **Aplicativo para Garçons** com fluxo: abrir mesa → criar comanda → fazer pedidos → enviar para cozinha → fechar conta
- **Gestão de Cozinha (KDS)** com setores de preparo (bar, cozinha, pizzaria, hamburgueria, etc.)
- **Controle de Estoque** com baixa automática ao consumir produtos (no pacote Premium)
- **Relatórios de Performance** por garçom, mesa, produto, período e horário de pico

### 🛵 Para Operações de Delivery/Retirada:
- **Sistema de Pedidos** com tipos específicos: 
  - Tipo 1: Mesa (salão)
  - Tipo 2: Balcão 
  - Tipo 3: Delivery
  - Tipo 4: Retirada
- **Cadastro de Cliente** com telefone, endereço completo e complemento para delivery
- **Fluxo de Pedido Delivery**: Recebido → Confirma pagamento → Cozinha prepara → Entrega sai → Finalizado
- **Cardápio Digital** acessível pelos clientes para fazer pedidos online (via QR Code na mesa ou link compartilhado)
- **Status Específico para Delivery**: Recebido → Em preparo → Pronto → Saiu para entrega → Finalizado
- **Integração Preparada** com plataformas externas (webhooks para iFood, Rappi, Uber Eats)

### 🔄 Integração entre Modelos:
O mesmo backend gerencia todos os tipos de pedido em uma única plataforma:
- **Fila Única de Cozinha**: Todos os pedidos (mesa, balcão, delivery, retirada) entram na mesma tela de cozinha, organizados por setor de preparo
- **Controle Unificado de Estoque**: Baixa automática de ingredientes independentemente da origem do pedido (salão ou delivery)
- **Relatórios Consolidados**: Visão completa de vendas por canal (salão vs delivery vs retirada)
- **Cardápio Sincronizado**: Mesmo cardápio e preços utilizados em todas as frentes de atendimento
- **Gestão Centralizada**: Um único painel para controlar todas as operações do estabelecimento

### 💡 Diferencial Competitivo:
Enquanto plataformas como iFood, Rappi e Uber Eats focam exclusivamente na **intermediação de delivery** (cobrando taxas por pedido), o Opamenu oferece:
- **Controle Total da Operação**: Gerencia salão, delivery, estoque, finanças e equipe em um único sistema
- **Zero Taxa por Pedido**: Modelo de assinatura mensal fixa, não cobramos por pedido realizado
- **Dados Próprios**: Você mantém 100% dos dados dos clientes e das operações
- **Flexibilidade Total**: Pode atender clientes no salão, por telefone, WhatsApp, cardápio digital ou plataformas externas simultaneamente
- **Redução de Custos**: Elimina a necessidade de múltiplos sistemas (um para PDV, outro para delivery, outro para estoque, etc.)

---

## 📈 Recomendações de Posicionamento de Mercado

1. **Para entrada no mercado**: Comece com o pacote Standard (R$ 349,90/mês) como oferta principal, pois atende à necessidade mais comum de estabelecimentos que operam nos dois modelos.

2. **Para prospecção**:
   - Ofereça o pacote Básico para estabelecimentos pequenos que estão começando ou têm foco quase exclusivo no salão
   - Apresente o Premium para clientes que já têm operação consolidada e buscam otimização e crescimento

3. **Estratégia de expansão futura**:
   - Mantenha os preços atuais por 6-12 meses para estabelecer base de clientes
   - Após implementar o módulo de logística de entrega (própria frota), introduza um plano "Delivery Plus" com acréscimo de R$ 150/mês
   - Considere um plano "Enterprise" para redes de franquias com preço negociável baseado no número de unidades

4. **Vendas complementares**:
   - Taxa de implementation: R$ 299,90 (único)
   - Treinamento adicional: R$ 199,00/sessão
   - Customização avançada: Orçamento sob demanda
   - Integrações específicas com APIs externas: R$ 499,00/integração

---

## ✅ Próximos Passos Sugeridos

1. Salvar este documento como `Precificação.md` na raiz do projeto
2. Criar uma página de preços no site institucional baseada neste conteúdo
3. Preparar proposta comercial padrão usando estes pacotes como base
4. Desenvolver materiais de venda que destaquem o diferencial de "controle total da operação" vs "apenas intermediação de delivery" dos concorrentes
5. Treinar a equipe de vendas nos pontos de diferenciação-chave destacados neste documento

---
*Documento criado em: Abril de 2026*
*Baseado na análise do ecossistema Opamenu conforme documentado em DOCS_OPAMENU.md e estrutura de código existente*