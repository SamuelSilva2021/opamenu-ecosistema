# Roadmap de Desenvolvimento: OpaMenu PDV (Desktop)

Este documento detalha o progresso atual do desenvolvimento da aplicação WPF (Frente de Caixa e Balcão) e define os próximos passos para obtermos um MVP (Minimum Viable Product) 100% funcional para restaurantes e lanchonetes. O foco principal é a **venda rápida no balcão** e o **funcionamento offline-first**.

---

## ✅ Fase 1: Fundação e Estrutura (Concluída)
*O que já temos pronto e rodando no projeto.*

- [x] **Arquitetura Base:** Projeto WPF em .NET 8 com padrão MVVM (`CommunityToolkit.Mvvm`).
- [x] **Injeção de Dependência:** Configuração do `Microsoft.Extensions.Hosting` para injeção de ViewModels, Services e HttpClient.
- [x] **Interface e UX:** Integração com `MaterialDesignThemes` e tipografia `Poppins` para consistência visual com os apps mobile.
- [x] **Banco de Dados Local:** Configuração inicial do Entity Framework Core + SQLite (`AppDbContext`), preparando o terreno para o Offline-First.
- [x] **Autenticação:**
  - Tela de Login UI/UX.
  - Consumo da API de Autenticação (`/api/auth/login`).
  - Persistência do token JWT em memória via `TokenStore`.
- [x] **Frente de Caixa (Balcão - Estrutura):**
  - Interface base do PDV com 3 colunas (Categorias, Produtos, Carrinho).
  - Consumo das APIs protegidas de catálogo (`/api/Categories` e `/api/products`) passando o token JWT.
  - Lógica funcional do Carrinho (Adicionar, Incrementar, Remover, Limpar, Totalizador).

---

## 🚀 Fase 2: Checkout e Finalização de Venda (Próximos Passos Imediatos)
*O foco agora é conseguir concluir uma venda no balcão e enviá-la para o banco de dados.*

- [ ] **1. Identificação do Cliente:**
  - Input para CPF na Nota e Nome do Cliente (para chamar o pedido no balcão).
- [ ] **2. Personalização do Produto:**
  - Ao clicar em um produto, se ele possuir "Adicionais" (ex: Ponto da carne, Borda recheada), abrir um modal para seleção.
  - Adicionar campo de "Observações" (ex: Sem cebola).
- [ ] **3. Tela/Modal de Checkout (Pagamento):**
  - Múltiplas formas de pagamento (Dinheiro, Pix, Cartão de Crédito/Débito).
  - Cálculo de Troco dinâmico quando a forma for Dinheiro.
  - Pagamento parcial (ex: Conta R$ 100 -> R$ 50 no PIX, R$ 50 no Cartão).
- [ ] **4. Geração do Pedido (SQLite):**
  - Salvar o pedido com status `PendingSync` na tabela `LocalOrder` do banco SQLite.

---

## 🔄 Fase 3: Gestão de Caixa e Operação Segura
*Regras e telas para que o dono do restaurante confie no sistema.*

- [ ] **1. Abertura de Caixa:**
  - Tela que obriga o usuário a informar o "Fundo de Caixa" (troco inicial) antes de realizar a primeira venda.
- [ ] **2. Movimentações de Caixa:**
  - **Sangria:** Retirada de dinheiro excessivo por segurança.
  - **Suprimento:** Entrada de mais troco.
- [ ] **3. Fechamento de Caixa:**
  - Tela de fechamento "cego" (o operador diz quanto tem em dinheiro/cartão sem ver o total do sistema).

---

## ☁️ Fase 4: Resiliência e Sincronização (Offline-First)
*Garantir que a operação não pare sem internet.*

- [ ] **1. Job de Sincronização:**
  - Desenvolver o `SyncBackgroundService` para ler os `LocalOrder` (Pedidos), `Caixa` e `Movimentacoes` que estão com status `PendingSync` e enviar para a `opamenu-api` quando a internet estiver ativa.
- [ ] **2. Cache de Catálogo:**
  - Salvar as categorias e produtos da nuvem no SQLite local durante o Login, para que o PDV abra mesmo sem internet.
- [ ] **3. Integração KDS / Impressão Térmica:**
  - Envio do pedido para as impressoras das praças (Cozinha, Bar, Copa).

---

## 🍽️ Fase 5: Gestão de Salão (Mesas e Comandas)
*Atendimento para clientes que consomem no local. O pedido não é pago na hora; ele fica aberto, recebendo itens até que o cliente solicite a conta.*

- [ ] **1. Visão de Mapa do Salão (Grid de Mesas):** 
  - Exibição de mesas com status de cores (Verde = Livre, Vermelho = Ocupada, Amarelo = Aguardando Conta).
- [ ] **2. Abertura de Mesa / Comanda:**
  - Consumir `IOrderService.CreateOrderDineInAsync` da API para abrir um pedido vinculado a uma mesa ou nome de cliente.
  - Suporte a múltiplas comandas na mesma mesa (ex: cada pessoa da mesa tem sua conta separada).
- [ ] **3. Lançamento Contínuo de Itens:**
  - O carrinho de uma mesa aberta não vai para a tela de Pagamento. O botão "COBRAR E ENVIAR" muda para "ENVIAR PARA COZINHA".
  - Consumir `IOrderService.AddItemsToOrderAsync` para ir adicionando os itens no pedido que já está aberto, sem cobrar.
- [ ] **4. Ações de Comanda:**
  - Transferir itens (ex: mover a cerveja da Mesa 1 para a Mesa 2).
  - Juntar Mesas.
- [ ] **5. Fechamento de Conta (Split Check):**
  - Consumir `IOrderService.CloseTableAccountAsync` (ou fluxo similar de pagamento) quando o cliente pedir a conta.
  - Permitir a divisão da conta (ex: dividir o total por 3 pessoas, ou cobrar a bebida em um cartão e o lanche no PIX).

---
*Roadmap focado exclusivamente na experiência Desktop do operador do PDV. O backoffice/gestão permanecerá no ambiente Web.*