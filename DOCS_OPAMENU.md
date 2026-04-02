# Opamenu - Sistema de Gestão para Restaurantes e Delivery

## Visão Geral

Este projeto descreve a arquitetura de um **ecossistema completo para restaurantes**, incluindo:

- PDV / Caixa
- Atendimento de mesas
- Comandas individuais por cliente
- Delivery
- Retirada
- Gestão de cozinha
- Aplicativo para garçons
- Integrações externas
- Gestão de estoque
- Relatórios e BI

O objetivo é atender **restaurantes de pequeno a grande porte**, podendo evoluir para um **SaaS multi-restaurante**.

---

# 1. Arquitetura do Ecossistema
ECOSSISTEMA RESTAURANTE

├── Painel Administrativo (Web)
│
├── PDV / Caixa
│
├── Atendimento Mesa / Comandas
│
├── Sistema de Pedidos
│ ├── Local (mesa)
│ ├── Balcão
│ └── Delivery
│
├── Aplicativo Garçom
│
├── Aplicativo Cliente (opcional)
│
├── Gestão de Cozinha (KDS)
│
├── Gestão Financeira
│
├── Gestão de Estoque
│
├── Integrações
│ ├── iFood
│ ├── Rappi
│ ├── Uber Eats
│ └── WhatsApp
│
└── Relatórios e BI
---

# 2. Módulo PDV (Caixa)

## Funções principais

- Abertura de caixa
- Fechamento de caixa
- Sangria
- Suprimento
- Controle de operadores
- Controle de formas de pagamento
- Emissão de comprovante

## Fluxo do Caixa
Abrir Caixa
↓
Registrar pedidos
↓
Receber pagamento
↓
Emitir comprovante
↓
Fechar caixa

## Formas de Pagamento

- Dinheiro
- Cartão de crédito
- Cartão de débito
- PIX
- Voucher
- Pagamento dividido

### Exemplo
Total: R$120

Pagamento:
R$60 PIX
R$60 Cartão
---

# 3. Sistema de Mesas e Comandas

Permite **várias comandas dentro da mesma mesa**.

## Estrutura
Salão
├─ Mesa 1
│ ├─ Comanda João
│ ├─ Comanda Maria
│ └─ Comanda Pedro
│
├─ Mesa 2
│ └─ Comanda única
│
└─ Mesa 3
├─ Comanda 1
├─ Comanda 2
├─ Comanda 3

## Funcionalidades

- Abrir mesa
- Criar múltiplas comandas
- Transferir itens entre comandas
- Juntar comandas
- Separar contas
- Fechar mesa

## Exemplo de uso
Mesa 10 aberta

Clientes:
João
Maria
Pedro

Sistema cria:

Comanda João
Comanda Maria
Comanda Pedro


Pedidos:
* Comanda João
Chopp
Picanha

* Comanda Maria
Caipirinha
Batata

* Comanda Pedro
Hambúrguer
Refrigerante
---

# 4. Sistema de Pedidos

Tipos de pedidos:
1 — Mesa
2 — Balcão
3 — Delivery
4 — Retirada


## Estrutura do Pedido
Pedido
├─ Itens
├─ Observações
├─ Status
├─ Forma de pagamento
└─ Origem


## Status do Pedido
Recebido
Em preparo
Pronto
Saiu para entrega
Finalizado
---

# 5. Sistema de Cozinha (KDS)

KDS = **Kitchen Display System**

Tela para exibir pedidos na cozinha.

### Exemplo
PEDIDO #104

Mesa 10 - Comanda João

1x Picanha
Obs: mal passada

1x Chopp


### Status
Novo
Preparando
Pronto


### Setores de preparo
Bar
Cozinha
Pizzaria
Hamburgueria


Cada setor recebe apenas seus itens.

---

# 6. Aplicativo do Garçom

Aplicativo para celular ou tablet.

## Funcionalidades

- Abrir mesa
- Criar comanda
- Fazer pedidos
- Enviar pedidos para cozinha
- Ver status dos pedidos
- Fechar conta

## Fluxo
Garçom seleciona mesa
↓
Cria comanda
↓
Adiciona pedidos
↓
Envia para cozinha

---

# 7. Sistema de Delivery

## Cadastro de Cliente
Cliente
Telefone
Endereço
Complemento


## Fluxo de pedido
Pedido recebido
↓
Confirma pagamento
↓
Cozinha prepara
↓
Entrega sai
↓
Pedido finalizado


## Gestão de Entregadores
Entregador 1
Entregador 2
Entregador 3


---

# 8. Integração com WhatsApp

Possibilidade de automatizar pedidos via WhatsApp.

Exemplo:

Cliente envia:
Quero 2 hambúrguer

Bot responde:
1 - Ver cardápio
2 - Fazer pedido
3 - Acompanhar pedido

Pedido entra automaticamente no sistema.

---

# 9. Gestão de Estoque

Controle automático de estoque.

### Exemplo
Produto: Hambúrguer
Estoque: 50

Venda de 1 unidade
↓

Estoque: 49

## Ficha Técnica

Permite baixar ingredientes automaticamente.

Exemplo:
X-Burger

1 pão
1 carne
1 queijo


Ao vender 1 X-Burger, o sistema baixa os ingredientes.

---

# 10. Relatórios

## Vendas
Vendas do dia
Vendas por garçom
Vendas por produto
Vendas por mesa


## Financeiro
Total do caixa
PIX
Cartão
Dinheiro


## Produtos
Produto mais vendido
Produto menos vendido


---

# 11. Multi-Restaurante (SaaS)

Sistema projetado para atender **múltiplos restaurantes**.

## Estrutura
Plataforma

├── Restaurante A
├── Restaurante B
├── Restaurante C


Cada restaurante possui:
Usuários
Mesas
Cardápio
Pedidos
Clientes
Relatórios