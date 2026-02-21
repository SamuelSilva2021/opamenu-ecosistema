# 💰 Sistema de Controle de Caixa (PDV) OpaMenu

O sistema de Controle de Caixa do OpaMenu permite uma gestão financeira precisa e auditável de cada terminal de venda, vinculando turnos de trabalho a usuários específicos e registrando cada movimentação de entrada e saída.

## 🚀 Fluxo de Trabalho do Usuário

### 1. Abertura do Turno
Para iniciar as operações no PDV/Balcão, o operador deve abrir o caixa.
- **Ação**: Clique em "Abrir Caixa" no menu **Fluxo de Caixa**.
- **Fundo de Troco**: Informe o valor em dinheiro disponível na gaveta no momento do início (ex: R$ 50,00).
- **Resultado**: O sistema altera o status para **Aberto** e registra a primeira movimentação de "Abertura".

### 2. Registro de Vendas (Automático)
Toda venda realizada através do **PDV** ou **Balcão** que tenha um método de pagamento informado é registrada automaticamente no caixa.
- **Entrada**: Pagamentos em dinheiro aumentam o "Saldo Esperado".
- **Auditoria**: Cada venda gera um vínculo direto entre o pedido e a movimentação de caixa.

### 3. Movimentações Manuais (Sangria e Suprimento)
Durante o turno, podem ocorrer entradas ou saídas extras de dinheiro.
- **Suprimento (Entrada)**: Adição de dinheiro no caixa (ex: reforço de troco).
- **Sangria (Saída)**: Retirada de dinheiro da gaveta (ex: pagamento de fornecedor ou retirada de excesso para cofre).
- **Como fazer**: No menu **Fluxo de Caixa**, clique em **Movimentar**, escolha o tipo e informe o motivo.

### 4. Fechamento do Caixa
Ao final do expediente ou troca de turno, o operador deve encerrar a sessão.
- **Conferência**: O sistema exibe o **Saldo Esperado** (Abertura + Vendas + Suprimentos - Sangrias).
- **Contagem Real**: O operador deve contar o dinheiro físico na gaveta e informar no campo "Saldo Final".
- **Diferença**: O sistema calcula automaticamente se há **Sobra** ou **Quebra** de caixa, facilitando o fechamento financeiro.

---

## 🛠️ Detalhes Técnicos

### Arquitetura de Dados
- **CashShift (Turno)**: Entidade mestre que controla o estado (`Open`/`Closed`) e os saldos acumulados.
- **CashMovement (Movimentação)**: Registros atômicos de cada transação vinculados a um turno.

### Localização no Código
- **Backend API**: `OpaMenu.Web/UserEntry/CashRegister/CashRegisterController.cs`
- **Lógica de Negócio**: `OpaMenu.Application/Services/Opamenu/CashRegisterService.cs`
- **Frontend (React)**: `src/features/cash-register/`

### Segurança e Multitenancy
- **Segmentação por Usuário**: Cada usuário possui seu próprio turno ativo, evitando conflitos em terminais compartilhados.
- **Tenant Isolation**: O uso do `ICurrentUserService` garante que as movimentações nunca vazem entre diferentes restaurantes (tenants).
- **Permissões**: O acesso é controlado pelo módulo `PDV`, garantindo que apenas operadores autorizados gerenciem o fluxo financeiro.

---

## 📊 Dashboard de Controle
O gestor pode acompanhar em tempo real:
- **Fundo de Troco**: Valor inicial do dia.
- **Entradas Totais**: Soma de vendas em dinheiro e suprimentos.
- **Saídas Totais**: Soma de sangrias e estornos.
- **Saldo Atual**: Valor que deve estar fisicamente presente na gaveta de dinheiro.
