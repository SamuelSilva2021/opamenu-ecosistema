# 🏆 Sistema de Fidelidade OpaMenu v2

O sistema de fidelidade do OpaMenu foi evoluído de um modelo simples de pontos para uma engine de regras flexível que suporta múltiplos tipos de campanhas simultâneas por restaurante (tenant).

## 1. Tipos de Programas Suportados
O lojista pode configurar três modelos distintos:

- **Pontos por Valor Gasto (`PointsPerValue`)**: O modelo clássico onde cada R$ 1,00 gasto equivale a X pontos.
- **Quantidade de Pedidos (`OrderCount`)**: Baseado no número de compras realizadas. Ex: "Ganhe 1 ponto a cada pedido. Ao atingir 10 pedidos, ganhe uma recompensa."
- **Quantidade de Itens Especificos (`ItemCount`)**: Baseado em categorias ou produtos específicos. Ex: "Ganhe 1 ponto a cada Açaí comprado. Ao atingir 10 unidades, o 11º é grátis."

## 2. Estrutura de Recompensas
Cada programa pode definir sua própria recompensa:
- **Desconto Percentual**: X% de desconto no próximo pedido.
- **Desconto Fixo**: R$ X de desconto no próximo pedido.
- **Produto Grátis**: Um item específico sem custo.

## 3. Lógica de Acúmulo Multi-Programa
Diferente da versão 1.0, o sistema agora permite que **múltiplos programas** estejam ativos ao mesmo tempo.
- Quando um pedido é finalizado, a engine filtra todos os programas ativos do restaurante.
- Cada regra é aplicada de forma independente.
- Um único pedido pode gerar pontos em diferentes programas (ex: pontos por valor total e crédito na cartela de pizzas).

## 4. Estrutura de Dados (Backend)
- `loyalty_programs`: Armazena a configuração das regras, metas e recompensas.
- `loyalty_program_filters`: Define quais produtos ou categorias pertencem a um programa (essencial para o tipo `ItemCount`).
- `loyalty_transactions`: Registro individual de cada crédito/débito de pontos/contagem.
- `customer_loyalty_balances`: Saldo consolidado do cliente por restaurante.

## 5. Fluxo de Integração (Roadmap Frontend)
Para o painel do lojista (`opamenu-painel`), o fluxo seguirá:
1. **Configuração**: Tela para criar/editar programas com seleção de tipo.
2. **Filtros**: Se o tipo for "Por Item", abrir seletor de categorias/produtos.
3. **Monitoramento**: Dashboard para ver quantos clientes estão próximos de atingir recompensas em cada campanha.
