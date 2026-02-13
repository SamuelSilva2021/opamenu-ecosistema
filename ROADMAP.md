# Roadmap MVP: OpaMenu Delivery Otimizado

Este roadmap foca em transformar o OpaMenu em um competidor de elite, priorizando a experiência de delivery e automação para o lojista.

---

## 🚀 Fase 1: Refinamento do Core & UX (Semana 1-2)
*Objetivo: Garantir que a fundação seja rápida, sem erros e visualmente impecável.*

- [ ] **Otimização de Performance:** Implementar Lazy Loading e compressão de imagens via Cloudinary (já integrado no .NET).
- [ ] **UX de Checkout "One-Page":** Reduzir campos desnecessários e focar no autopreenchimento de endereço via CEP.
- [x] **Feedback em Tempo Real:** Implementado notificações instantâneas via SignalR no Painel do Lojista (Novo Pedido e Atualização de Status).
- [ ] **Validação de Pagamento Pix:** Automatizar a verificação do recebimento via Webhook do provedor de pagamento.

## 🤖 Fase 2: Automação WhatsApp Engine (Semana 3-4)
*Objetivo: Reduzir a carga de trabalho do lojista e capturar o tráfego das redes sociais.*

- [ ] **Bot de Boas-Vindas:** Atendimento automático que envia o link do cardápio assim que o cliente chama.
- [ ] **Consulta de Status Automática:** Permitir que o cliente digite "Status" no WhatsApp e receba a etapa atual do pedido (Cozinha/Saiu para Entrega).
- [ ] **Impressão Automática:** Integração com o App Gestor (Flutter) para imprimir o pedido na cozinha assim que for confirmado.
- [ ] **Anotação Assistida:** Bot que entende "Quero um X-Bacon e uma Coca" e pré-monta o carrinho para o cliente.

## 📦 Fase 3: Inteligência de Delivery & Expansão (Semana 5-6)
*Objetivo: Dominar a logística e oferecer dados estratégicos ao lojista.*

- [ ] **Cálculo Dinâmico de Frete:** Integração com Google Maps para cobrar por distância real, não apenas por bairro.
- [ ] **Painel de Desempenho (Lojista):** Dashboard no App Gestor com faturamento por período e itens mais vendidos.
- [ ] **Gestão de Entregadores:** Módulo no Gestor para associar pedidos a entregadores específicos e rastrear entregas.
- [x] **Programa de Fidelidade v2:** Engine flexível com suporte a itens, categorias e múltiplos programas ativos. [Ver Documentação](DOCS_LOYALTY_V2.md)
- [ ] **Promoções Direcionadas:** Sistema para enviar cupons via WhatsApp para clientes que não compram há mais de 15 dias.

---

## 🎯 Definição de Sucesso para o MVP
1.  **Tempo de Pedido:** Cliente deve conseguir finalizar o pedido em menos de 60 segundos.
2.  **Automação:** Pelo menos 70% das dúvidas de status devem ser resolvidas pelo bot do WhatsApp.
3.  **Gestão:** O lojista deve conseguir operar todo o delivery apenas pelo App em Flutter (celular ou tablet).
