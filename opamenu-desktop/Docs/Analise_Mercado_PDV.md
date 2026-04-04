# Análise de Mercado: Sistemas de PDV para Restaurantes

Esta documentação compila as melhores práticas de UI/UX, telas e funcionalidades baseadas nos sistemas de gestão de restaurantes líderes de mercado (como Square, Toast, Lightspeed, e no Brasil: Consumer, Saipos, e KCMS). O objetivo é guiar o desenvolvimento do **OpaMenu Desktop**.

---

## 1. Padrões de Interface (UI / UX)

Os sistemas de PDV modernos são projetados para **velocidade** e **baixa curva de aprendizado**, visto que a rotatividade de funcionários (garçons/caixas) pode ser alta.

* **Design Touch-First:** Botões grandes, áreas de clique espaçadas e suporte a gestos de arrastar (swipe), otimizados para monitores touch screen.
* **Layout em 3 Colunas (Balcão/Fast Food):**
  * *Esquerda:* Categorias (Filtros rápidos).
  * *Centro:* Grid de Produtos com imagens ou cores sólidas + texto legível.
  * *Direita:* Resumo do Pedido (Carrinho), totalizador e botão gigante de "Cobrar / Enviar para Cozinha".
* **Esquema de Cores (Feedback Visual):**
  * Cores vibrantes para ações de sucesso (Verde para "Pagamento Aprovado" ou "Mesa Livre").
  * Cores de alerta para atenção (Amarelo para "Mesa aguardando pedido há X minutos", Vermelho para "Mesa Ocupada" ou "Cancelamento").
* **Modo Escuro (Dark Mode):** Muito utilizado para não cansar a visão do caixa em ambientes de salão que costumam ter pouca iluminação.

---

## 2. Estrutura de Telas Essenciais

### 2.1. Tela de Login e Abertura de Caixa
* **Acesso Rápido:** Login via PIN (senha numérica de 4 a 6 dígitos) ou crachá (RFID/Código de barras), sem necessidade de digitar e-mail complexo a todo momento.
* **Abertura de Turno:** Tela que exige a inserção do "Fundo de Caixa" (troco inicial) antes de começar a operar.

### 2.2. Frente de Caixa (Balcão / Fast Food)
* **Objetivo:** Tirar o pedido e cobrar o mais rápido possível.
* **Funcionalidades:**
  * Catálogo de acesso rápido.
  * Personalização de produto (Adicionais, Observações como "Sem cebola").
  * Identificação do cliente (CPF na nota, nome para chamar no balcão).
  * Atalhos de pagamento rápido (Ex: Botão "Dinheiro Exato", "Cartão de Crédito", "Pix").

### 2.3. Gestão de Mesas e Comandas (Salão)
* **Objetivo:** Visão panorâmica do restaurante.
* **Funcionalidades:**
  * **Mapa do Salão (Floor Plan):** Representação visual das mesas.
  * **Status por Tempo:** Mesas mudam de cor se os clientes estão há muito tempo sem pedir, ou aguardando a conta.
  * **Ações de Mesa:** Transferir itens de uma mesa para outra, juntar mesas, dividir a conta (Split Check) por item ou por valor igual.

### 2.4. Tela de Checkout / Pagamento
* **Objetivo:** Finalizar a venda com flexibilidade.
* **Funcionalidades:**
  * Pagamento Múltiplo (Ex: Conta de R$ 100 -> R$ 40 no PIX, R$ 60 no Cartão).
  * Cálculo de Gorjeta / Taxa de Serviço (10%).
  * Emissão de Cupom Fiscal (NFC-e / SAT).

### 2.5. Integração com Cozinha (KDS ou Impressão)
* Envio automático do pedido segmentado por praça (Ex: Bebidas vão para a impressora do bar, pratos vão para a tela/impressora da cozinha).

### 2.6. Retaguarda do Caixa (Fechamento e Sangria)
* **Sangria:** Retirada de dinheiro do caixa por excesso de notas.
* **Suprimento:** Entrada de troco extra.
* **Fechamento Cego:** O operador informa quanto tem na gaveta sem saber quanto o sistema registrou, para o gerente auditar depois (evita fraudes).

## 3. Backoffice / Gestão do Restaurante (ERP)

A maioria dos sistemas de mercado separa a operação do "chão de loja" (PDV) da gestão do negócio (Backoffice). Enquanto o PDV foca em vender rápido, o Backoffice foca em configurar e analisar. No entanto, é muito comum que ambos coexistam na mesma aplicação desktop (acessados via senha de gerente) ou que o PDV seja um app e a gestão seja web.

### 3.1. Cadastros de Cardápio (Categorias e Produtos)
* **Gestão de Categorias:** Ordenação (qual aparece primeiro no PDV), definição se a categoria está visível apenas no salão, no delivery, ou em ambos.
* **Cadastro de Produtos:**
  * Dados básicos: Nome, Descrição, Preço, Foto.
  * **Ficha Técnica (Receita):** Composição do prato (ex: 1 pão, 150g carne, 2 fatias de queijo) para dar baixa automática no estoque.
  * **Grade de Adicionais (Complementos):** Regras complexas de venda. Exemplo: "Escolha o Ponto da Carne" (Obrigatório, máx 1), "Adicionais Extras" (Opcional, máx 5, cobra valor extra).
  * **Integração Fiscal:** Configuração de NCM, CFOP, CEST e impostos.

### 3.2. Controle de Estoque
* **Movimentação:** Entrada de notas fiscais de fornecedores, ajustes manuais (perda, validade, consumo interno).
* **Alerta de Estoque Mínimo:** Avisar quando o produto está acabando. O PDV deve ser capaz de travar a venda (ou alertar) se o estoque acabar durante a operação.

### 3.3. Controle de Usuários e Permissões (Segurança)
* **Perfis de Acesso (Roles):**
  * *Caixa:* Só pode vender, não pode cancelar item sem senha do gerente.
  * *Garçom:* Pode lançar na mesa, mas não pode fechar a conta ou dar desconto.
  * *Gerente:* Pode aplicar descontos, cancelar pedidos, realizar sangrias e acessar relatórios.
* **Auditoria (Log de Ações):** O sistema registra quem fez o quê e que horas (ex: "Cancelamento de R$50 feito pelo Gerente João às 14:30").

### 3.4. Relatórios e Dashboard
* Vendas por Período, Curva ABC (Produtos mais vendidos x mais lucrativos), Ticket Médio, Fechamento de Caixa detalhado.

---

## 4. Próximos Passos sugeridos para o OpaMenu Desktop

Considerando a base que já construímos (Login + Estrutura do PDV com categorias e carrinho):

1. **Aprimoramento da Tela de PDV (Balcão):**
   * Adicionar suporte a **Adicionais e Observações** no momento de adicionar o item ao carrinho.
   * Criar a **Tela de Checkout** modal (forma de pagamento, troco, emissão).
2. **Desenvolvimento da Tela de Mesas/Comandas:**
   * Criar a visualização em Grid/Mapa das mesas do salão.
   * Fluxo de abrir uma mesa -> adicionar itens -> fechar conta.
3. **Fluxo de Caixa:**
   * Tela de Abertura/Fechamento de Caixa, Sangria e Suprimento.

---
*Documento gerado como referência de arquitetura de produto para o ecossistema OpaMenu.*