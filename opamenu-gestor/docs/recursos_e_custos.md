# Análise de Custos e Recursos: Delivery e Notificações

Este documento detalha os custos estimados (financeiros e de infraestrutura) para a implementação das funcionalidades de "Delivery Avançado" e "Notificações em Tempo Real" no projeto `opamenu-gestor`.

## 1. Delivery Avançado (Mapas e Logística)

Para funcionalidades como exibir mapas, autocompletar endereços, calcular rotas e taxas de entrega baseadas em distância.

### Opção A: Google Maps Platform (Recomendada)
É a solução mais robusta e fácil de integrar com Flutter.
*   **Modelo de Preço**: Pay-as-you-go com **$200 USD de crédito mensal gratuito**.
*   **APIs Necessárias**:
    *   *Maps SDK for Android/iOS / Maps JavaScript API*: Para exibir o mapa visualmente.
    *   *Geocoding API*: Converter endereço em coordenadas (Lat/Long).
    *   *Distance Matrix API*: Calcular distância e tempo entre restaurante e cliente (essencial para taxas).
    *   *Places API*: Autocomplete de endereços.
*   **Custo Estimado**:
    *   **Até ~28.000 carregamentos de mapa/mês**: Gratuito (coberto pelos $200).
    *   **Até ~40.000 cálculos de rota/mês**: Gratuito.
    *   **Excedente**: ~$5-7 USD por 1.000 requisições.
    *   *Veredito*: Para a maioria dos restaurantes individuais, o custo será **ZERO** (coberto pelo crédito).

### Opção B: Mapbox
Alternativa forte com limites gratuitos maiores, mas integração ligeiramente mais complexa.
*   **Mapas Web**: 50.000 cargas gratuitas/mês.
*   **Geocoding**: 100.000 requisições gratuitas/mês.
*   *Veredito*: Melhor se o volume de acessos for muito alto desde o início.

### Opção C: OpenStreetMap (Gratuito)
*   **Custo**: Zero licença.
*   **Infraestrutura**: Pode exigir hospedagem de servidor de tiles ou uso de APIs de terceiros lentas.
*   *Veredito*: Não recomendado para operação comercial crítica devido à complexidade de manutenção e qualidade visual inferior.

---

## 2. Notificações e Tempo Real

Para alertar o gestor/cozinha instantaneamente quando um pedido chega (sem precisar recarregar a página).

### Opção A: Firebase (Google) - Recomendada
Padrão de mercado para Apps Mobile e Web.
*   **Cloud Messaging (FCM)**:
    *   **Custo**: **Gratuito e Ilimitado**.
    *   Uso: Enviar notificações Push para o celular/tablet do gestor.
*   **Firestore (Banco de Dados em Tempo Real)**:
    *   **Custo**: Plano Spark (Gratuito).
        *   50.000 leituras/dia.
        *   20.000 gravações/dia.
    *   **Excedente**: Pay-as-you-go (barato para texto).
    *   *Veredito*: Custo **ZERO** para operações de pequeno/médio porte.

### Opção B: WebSockets Próprios (SignalR / Socket.io)
Se já tivermos um backend .NET rodando (como a `opamenu-api`).
*   **Custo Financeiro**: Zero (já incluso na hospedagem do servidor).
*   **Custo Técnico**: Alto. Exige manter conexões persistentes, gerenciar reconexões, escalar servidor se houver muitos clientes.
*   *Veredito*: Válido se quiser total independência do Google, mas aumenta o tempo de desenvolvimento.

### Opção C: Serviços Pagos (Pusher, Ably)
*   **Custo**: Planos iniciais gratuitos (sandbox), depois saltam para ~$29-49/mês.
*   *Veredito*: Desnecessário dado que o Firebase atende bem gratuitamente.

---

## 3. Custos de Desenvolvimento (Esforço)

Além das ferramentas, há o "custo" de horas de desenvolvimento para integrar essas features.

| Feature | Complexidade | Estimativa de Esforço |
| :--- | :--- | :--- |
| **Integração Firebase (FCM)** | Baixa | 1-2 dias (Configuração + Front/Back) |
| **KDS em Tempo Real (Sync)** | Média | 3-5 dias (Mudança de arquitetura para Streams) |
| **Mapas e Taxas (Google Maps)** | Alta | 5-8 dias (UI de Mapa, Lógica de Taxas, API) |
| **Gestão de Entregadores** | Média | 3-4 dias (CRUD, Associação a Pedidos) |

---

## Resumo da Recomendação

Para iniciar com **Custo Zero** e alta qualidade:

1.  **Mapas**: Usar **Google Maps Platform** (aproveitando os $200 de crédito).
2.  **Notificações**: Usar **Firebase** (FCM para Push + Firestore para sincronização de tela).

Essa stack permite escalar até milhares de pedidos mensais sem pagar nada além da hospedagem atual, começando a pagar apenas quando o negócio já estiver faturando alto.
