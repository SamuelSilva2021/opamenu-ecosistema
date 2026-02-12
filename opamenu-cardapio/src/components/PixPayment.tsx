import React, { useState, useEffect } from 'react';
import QRCode from 'qrcode';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { Copy, CheckCircle, Clock, RefreshCw, Loader2, Smartphone, ArrowLeft, Check } from 'lucide-react';
import { toast } from 'sonner';
import { orderService } from '@/services/order-service';
import { OrderStatus } from '@/types/api';

interface PixPaymentProps {
  orderId: string;
  orderNumber?: string;
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
  orderNumber,
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
      <Card className="w-full max-w-md mx-auto rounded-[2.5rem] border-0 shadow-none md:border md:shadow-sm">
        <CardContent className="p-8 text-center space-y-6">
          <div className="bg-destructive/10 w-24 h-24 rounded-full flex items-center justify-center mx-auto">
            <Clock className="h-10 w-10 text-destructive" />
          </div>
          <div className="space-y-2">
            <h3 className="text-xl font-black uppercase italic tracking-tight">Código Expirado</h3>
            <p className="text-muted-foreground">
              O tempo para pagamento expirou. Gere um novo código para continuar.
            </p>
          </div>
          <div className="flex flex-col gap-3 pt-4">
            <Button onClick={handleRefresh} className="h-14 rounded-2xl font-black uppercase italic tracking-widest bg-primary hover:bg-primary/90 text-white shadow-xl shadow-primary/20">
              <RefreshCw className="h-5 w-5 mr-2 stroke-[3]" />
              Gerar Novo Código
            </Button>
            <Button variant="ghost" onClick={onCancel} className="h-14 rounded-2xl font-black uppercase italic tracking-widest text-muted-foreground">
              Cancelar
            </Button>
          </div>
        </CardContent>
      </Card>
    );
  }

  return (
    <div className="w-full max-w-4xl mx-auto p-0 md:p-1">
      {/* Header */}
      <Card className="border-0 shadow-none md:border md:shadow-sm rounded-[2.5rem] bg-background/50 backdrop-blur-sm mb-6">
        <CardContent className="p-6 md:p-8">
          <div className="flex flex-col md:flex-row items-center justify-between gap-6">
            <div className="flex items-center gap-4">
              <div className="bg-primary/10 p-3 rounded-2xl">
                <Smartphone className="h-8 w-8 text-primary" />
              </div>
              <div>
                <h2 className="font-black text-2xl uppercase italic tracking-tighter text-foreground">Pagamento PIX</h2>
                <div className="flex items-center gap-2 mt-1">
                  <Badge variant="secondary" className="font-bold">
                    Pedido #{orderNumber || orderId}
                  </Badge>
                  {isPolling && (
                    <span className="flex items-center gap-1 text-xs text-primary font-bold animate-pulse uppercase tracking-wider">
                      <Loader2 className="h-3 w-3 animate-spin" />
                      Aguardando...
                    </span>
                  )}
                </div>
              </div>
            </div>

            <div className="flex items-center gap-3 bg-muted/50 px-4 py-2 rounded-2xl">
              <Clock className="h-5 w-5 text-muted-foreground" />
              <div className="flex flex-col items-end">
                <span className="text-[10px] uppercase font-bold text-muted-foreground tracking-wider">Expira em</span>
                <span className={`font-mono text-lg font-bold leading-none ${timeLeft < 300 ? 'text-destructive' : 'text-foreground'}`}>
                  {formatTime(timeLeft)}
                </span>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6 items-start">
        {/* Left Column: QR Code */}
        <Card className="border-0 shadow-none md:border md:shadow-sm rounded-[2.5rem] overflow-hidden">
          <CardContent className="p-8 flex flex-col items-center text-center space-y-8">
            <div className="space-y-2">
              <span className="text-sm font-bold uppercase tracking-widest text-muted-foreground">Valor a Pagar</span>
              <div className="text-5xl font-black text-primary tracking-tight">
                {new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(amount)}
              </div>
            </div>

            {qrCodeUrl && (
              <div className="relative group">
                <div className="absolute -inset-1 bg-gradient-to-r from-primary to-primary/50 rounded-[2rem] blur opacity-25 group-hover:opacity-50 transition duration-1000"></div>
                <div className="relative p-6 bg-white rounded-[1.8rem] border-2 border-border shadow-sm">
                  <img
                    src={qrCodeUrl}
                    alt="QR Code PIX"
                    className="w-56 h-56 sm:w-64 sm:h-64 mix-blend-multiply"
                  />
                </div>
              </div>
            )}

            <div className="space-y-2 max-w-xs mx-auto">
              <p className="text-sm font-medium text-muted-foreground">
                Abra o app do seu banco e escaneie o código acima para pagar
              </p>
              {!qrCodePayload && pixKey && (
                <div className="pt-4">
                   <Badge variant="outline" className="py-1 px-3 font-mono text-xs">
                    Chave: {pixKey}
                   </Badge>
                </div>
              )}
            </div>
          </CardContent>
        </Card>

        {/* Right Column: Actions */}
        <div className="space-y-6">
          {/* Copy Paste */}
          <Card className="border-0 shadow-none md:border md:shadow-sm rounded-[2.5rem]">
            <CardContent className="p-8 space-y-6">
              <h3 className="font-black text-lg uppercase italic tracking-tighter text-foreground flex items-center gap-2">
                <Copy className="h-5 w-5 text-primary" />
                Pix Copia e Cola
              </h3>
              
              <div className="space-y-4">
                <div className="p-4 bg-muted/30 rounded-2xl border-2 border-border/50 text-xs font-mono break-all max-h-32 overflow-y-auto">
                  {pixCode}
                </div>
                
                <Button
                  onClick={handleCopyCode}
                  className={`w-full h-14 rounded-2xl font-black uppercase italic tracking-widest shadow-lg transition-all
                    ${copied 
                      ? 'bg-green-600 hover:bg-green-700 shadow-green-900/20 text-white' 
                      : 'bg-white border-2 border-primary text-primary hover:bg-primary/5 shadow-primary/10'}
                  `}
                >
                  {copied ? (
                    <>
                      <CheckCircle className="h-5 w-5 mr-2 stroke-[3]" />
                      Copiado!
                    </>
                  ) : (
                    <>
                      <Copy className="h-5 w-5 mr-2 stroke-[3]" />
                      Copiar Código
                    </>
                  )}
                </Button>
              </div>
            </CardContent>
          </Card>

          {/* Instructions */}
          <div className="bg-blue-50/50 p-6 rounded-[2rem] border border-blue-100/50 space-y-4">
            <h4 className="font-black text-sm uppercase tracking-wider text-blue-900 flex items-center gap-2">
              <div className="w-2 h-2 rounded-full bg-blue-600" />
              Como pagar?
            </h4>
            <ol className="space-y-4">
              <li className="flex gap-4 items-start">
                <span className="flex-shrink-0 w-6 h-6 rounded-full bg-blue-100 text-blue-700 font-bold text-xs flex items-center justify-center">1</span>
                <span className="text-sm font-medium text-blue-800/80">Abra o aplicativo do seu banco no celular</span>
              </li>
              <li className="flex gap-4 items-start">
                <span className="flex-shrink-0 w-6 h-6 rounded-full bg-blue-100 text-blue-700 font-bold text-xs flex items-center justify-center">2</span>
                <span className="text-sm font-medium text-blue-800/80">Escolha a opção <strong>Pagar com Pix</strong> e escaneie o QR Code ou use o Copia e Cola</span>
              </li>
              <li className="flex gap-4 items-start">
                <span className="flex-shrink-0 w-6 h-6 rounded-full bg-blue-100 text-blue-700 font-bold text-xs flex items-center justify-center">3</span>
                <span className="text-sm font-medium text-blue-800/80">Confirme os dados e finalize o pagamento</span>
              </li>
            </ol>
          </div>

          {/* Action Buttons */}
          <div className="flex flex-col gap-3 pt-2">
            <Button
              onClick={onPaymentConfirmed}
              className="w-full h-16 rounded-2xl font-black uppercase italic tracking-widest bg-primary hover:bg-primary-hover text-white shadow-xl shadow-primary/20 flex items-center justify-center"
            >
              <Check className="h-5 w-5 mr-2 stroke-[3]" />
              Já paguei
            </Button>
            
            <Button
              variant="ghost"
              onClick={onCancel}
              className="w-full h-14 rounded-2xl font-black uppercase italic tracking-widest text-muted-foreground hover:bg-muted/50"
            >
              <ArrowLeft className="h-5 w-5 mr-2 stroke-[3]" />
              Voltar
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default PixPayment;
