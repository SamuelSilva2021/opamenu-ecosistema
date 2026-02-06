import React, { useState, useEffect } from 'react';
import QRCode from 'qrcode';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { Copy, CheckCircle, Clock, RefreshCw, Loader2 } from 'lucide-react';
import { toast } from 'sonner';
import { orderService } from '@/services/order-service';
import { OrderStatus } from '@/types/api';

interface PixPaymentProps {
  orderId: string;
  amount: number;
  pixKey?: string;
  merchantName?: string;
  merchantCity?: string;
  qrCodePayload?: string;
  slug?: string;
  onPaymentConfirmed?: () => void;
  onCancel?: () => void;
}

const PixPayment: React.FC<PixPaymentProps> = ({
  orderId,
  amount,
  pixKey,
  merchantName = "OPAMENU",
  merchantCity = "BRASILIA",
  qrCodePayload,
  slug,
  onPaymentConfirmed,
  onCancel
}) => {
  const [qrCodeUrl, setQrCodeUrl] = useState<string>('');
  const [pixCode, setPixCode] = useState<string>('');
  const [copied, setCopied] = useState(false);
  const [timeLeft, setTimeLeft] = useState(900); // 15 minutos em segundos
  const [isExpired, setIsExpired] = useState(false);
  const [isPolling, setIsPolling] = useState(true);
  const [orderStatus, setOrderStatus] = useState<OrderStatus>(OrderStatus.Pending);

  // Função para gerar o código PIX (simplificado para demonstração)
  const generatePixCode = () => {
    // Se o payload já veio do backend, usa ele
    if (qrCodePayload) return qrCodePayload;

    const txId = `OPAMENU${orderId}`;

    // Código PIX simplificado (em produção, usar biblioteca oficial ou API do banco)
    const pixPayload = `00020126580014BR.GOV.BCB.PIX0136${pixKey}5204000053039865802BR5913${merchantName}6008${merchantCity}62070503***6304`;

    return pixPayload;
  };

  // Gerar QR Code
  useEffect(() => {
    const generateQRCode = async () => {
      // Se não tem payload do backend e não tem chave PIX, erro
      if (!qrCodePayload && !pixKey) {
        toast.error('Chave PIX não configurada para este estabelecimento');
        return;
      }

      try {
        const code = generatePixCode();
        setPixCode(code);

        const qrUrl = await QRCode.toDataURL(code, {
          width: 256,
          margin: 1,
          color: {
            dark: '#000000',
            light: '#FFFFFF'
          }
        });

        setQrCodeUrl(qrUrl);
      } catch (error) {
        console.error('Erro ao gerar QR Code:', error);
        toast.error('Erro ao gerar código PIX');
      }
    };

    generateQRCode();
  }, [orderId, amount, pixKey, qrCodePayload]);

  useEffect(() => {
    if (timeLeft <= 0) {
      setIsExpired(true);
      setIsPolling(false);
      return;
    }

    const timer = setInterval(() => {
      setTimeLeft(prev => prev - 1);
    }, 1000);

    return () => clearInterval(timer);
  }, [timeLeft]);

  // Polling de status do pedido
  useEffect(() => {
    if (!isPolling || !orderId || isExpired) return;

    const pollStatus = async () => {
      try {
        const status = await orderService.checkOrderStatus(orderId, slug);

        // Se o status mudou para Confirmed ou superior, o pagamento foi identificado
        if (status !== OrderStatus.Pending && status !== OrderStatus.Cancelled) {
          setOrderStatus(status);
          setIsPolling(false);
          toast.success('Pagamento confirmado com sucesso!');

          // Pequeno delay para o usuário ver a mensagem de sucesso
          setTimeout(() => {
            if (onPaymentConfirmed) onPaymentConfirmed();
          }, 2000);
        }
      } catch (error) {
        console.error('Erro ao verificar status do pagamento:', error);
      }
    };

    const pollInterval = setInterval(pollStatus, 5000); // Polling a cada 5 segundos

    return () => clearInterval(pollInterval);
  }, [isPolling, orderId, slug, isExpired, onPaymentConfirmed]);

  // Copiar código PIX
  const handleCopyCode = async () => {
    try {
      await navigator.clipboard.writeText(pixCode);
      setCopied(true);
      toast.success('Código PIX copiado!');

      setTimeout(() => {
        setCopied(false);
      }, 3000);
    } catch (error) {
      console.error('Erro ao copiar:', error);
      toast.error('Erro ao copiar código');
    }
  };

  // Formatar tempo restante
  const formatTime = (seconds: number) => {
    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = seconds % 60;
    return `${minutes.toString().padStart(2, '0')}:${remainingSeconds.toString().padStart(2, '0')}`;
  };

  // Gerar novo código
  const handleRefresh = () => {
    setTimeLeft(900);
    setIsExpired(false);
    const code = generatePixCode();
    setPixCode(code);

    QRCode.toDataURL(code, {
      width: 256,
      margin: 1,
      color: {
        dark: '#000000',
        light: '#FFFFFF'
      }
    }).then(setQrCodeUrl);
  };

  if (isExpired) {
    return (
      <Card className="w-full max-w-md mx-auto">
        <CardContent className="p-1 text-center">
          <div className="text-6xl mb-4">⏰</div>
          <h3 className="text-lg font-semibold mb-2">Código PIX Expirado</h3>
          <p className="text-muted-foreground mb-4">
            O código PIX expirou. Gere um novo código para continuar com o pagamento.
          </p>
          <div className="flex gap-2">
            <Button onClick={handleRefresh} className="flex-1">
              <RefreshCw className="h-4 w-4 mr-2" />
              Gerar Novo Código
            </Button>
            <Button variant="outline" onClick={onCancel} className="flex-1">
              Cancelar
            </Button>
          </div>
        </CardContent>
      </Card>
    );
  }

  return (
    <div className="w-full max-w-4xl mx-auto space-y-6 px-4 pb-8 md:px-6">
      {/* Header - Full Width */}
      <Card className="border-none shadow-sm">
        <CardHeader className="text-center pb-2">
          <CardTitle className="flex flex-col sm:flex-row items-center justify-center gap-2">
            <div className="flex items-center gap-2">
              <div className="w-8 h-8 bg-[#32BCAD] rounded flex items-center justify-center shrink-0">
                <span className="text-white font-bold text-sm">PIX</span>
              </div>
              <span>Pagamento PIX</span>
            </div>
          </CardTitle>
          <div className="flex justify-center sm:justify-between items-center mt-4 gap-4 flex-wrap">
            <Badge variant="outline" className="text-xs sm:text-sm">
              Pedido #{orderId}
            </Badge>
            <div className="flex items-center gap-4">
              {isPolling && (
                <div className="flex items-center gap-2 text-xs text-primary animate-pulse">
                  <Loader2 className="h-3 w-3 animate-spin" />
                  Verificando pagamento...
                </div>
              )}
              <div className="flex items-center gap-2 text-sm bg-muted/50 px-3 py-1 rounded-full">
                <Clock className="h-4 w-4" />
                <span className={timeLeft < 300 ? 'text-red-500 font-medium' : 'text-muted-foreground font-medium'}>
                  {formatTime(timeLeft)}
                </span>
              </div>
            </div>
          </div>
        </CardHeader>
      </Card>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6 md:gap-8 items-start">
        {/* Left Column: Visual Focus (QR Code) */}
        <Card className="h-full flex flex-col justify-center border-none shadow-sm">
          <CardContent className="p-6 md:p-8">
            <div className="text-center space-y-6">
              <div className="text-3xl sm:text-4xl font-bold text-[#16a34a]">
                R$ {amount.toFixed(2)}
              </div>

              {qrCodeUrl && (
                <div className="flex justify-center">
                  <div className="p-2 sm:p-4 bg-white rounded-xl border-2 border-gray-100 shadow-sm">
                    <img
                      src={qrCodeUrl}
                      alt="QR Code PIX"
                      className="w-56 h-56 sm:w-64 sm:h-64"
                    />
                  </div>
                </div>
              )}

              <div className="space-y-2">
                <p className="text-sm text-muted-foreground px-4">
                  Use o app do seu banco para escanear o código
                </p>
                {!qrCodePayload && pixKey && (
                  <div className="flex flex-col items-center justify-center gap-2 text-xs text-muted-foreground">
                    <span className="font-medium">Chave PIX:</span>
                    <span className="text-center break-all bg-muted px-2 py-1 rounded select-all">{pixKey}</span>
                  </div>
                )}
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Right Column: Actions & Instructions */}
        <div className="space-y-6">
          {/* Código para Copia e Cola */}
          <Card className="border-none shadow-sm">
            <CardContent className="p-6">
              <div className="space-y-4">
                <h4 className="font-medium text-base">Copia e Cola</h4>
                <div className="space-y-3">
                  <div className="p-4 bg-muted/50 rounded-lg text-xs font-mono break-all max-h-32 overflow-y-auto border border-border">
                    {pixCode}
                  </div>
                  <Button
                    size="lg"
                    onClick={handleCopyCode}
                    className={`w-full ${copied ? 'bg-green-600 hover:bg-green-700' : ''}`}
                  >
                    {copied ? (
                      <>
                        <CheckCircle className="h-5 w-5 mr-2" />
                        Código Copiado!
                      </>
                    ) : (
                      <>
                        <Copy className="h-5 w-5 mr-2" />
                        Copiar Código PIX
                      </>
                    )}
                  </Button>
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Instruções */}
          <Card className="bg-blue-50/50 border-blue-100 shadow-none">
            <CardContent className="p-6">
              <h4 className="font-medium text-sm mb-3 text-blue-900 flex items-center gap-2">
                <div className="w-1.5 h-1.5 rounded-full bg-blue-600" />
                Como pagar
              </h4>
              <ol className="text-sm space-y-3 text-blue-800">
                <li className="flex gap-3">
                  <span className="font-bold text-blue-300">1</span>
                  Abra o app do seu banco
                </li>
                <li className="flex gap-3">
                  <span className="font-bold text-blue-300">2</span>
                  Escaneie o QR Code ou cole o código PIX
                </li>
                <li className="flex gap-3">
                  <span className="font-bold text-blue-300">3</span>
                  Confirme o pagamento e aguarde a confirmação
                </li>
              </ol>
            </CardContent>
          </Card>

          {/* Botões */}
          <div className="flex flex-col sm:flex-row gap-4 pt-2">
            <Button
              variant="outline"
              onClick={onCancel}
              className="flex-1 order-2 sm:order-1 h-12 text-base"
            >
              Voltar
            </Button>
            <Button
              onClick={onPaymentConfirmed}
              className="flex-1 bg-[#16a34a] hover:bg-[#15803d] order-1 sm:order-2 h-12 text-base font-medium shadow-md shadow-green-900/10"
            >
              Já realizei o pagamento
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default PixPayment;
