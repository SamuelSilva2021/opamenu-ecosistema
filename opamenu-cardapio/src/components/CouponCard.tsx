import { Coupon } from '@/types/api';
import { Button } from '@/components/ui/button';
import { toast } from 'sonner';
import { cn } from '@/lib/utils';

interface CouponCardProps {
  coupon: Coupon;
  className?: string;
}

export const CouponCard = ({ coupon, className }: CouponCardProps) => {
  const copyToClipboard = () => {
    navigator.clipboard.writeText(coupon.code);
    toast.success('Cupom copiado!');
  };

  const isPercentage = coupon.discountType === 1;

  return (
    <div className={cn("bg-primary/5 rounded-lg p-4 flex justify-between items-center border border-primary/10 min-w-[300px] flex-shrink-0 snap-center", className)}>
      <div>
        <h3 className="text-primary font-bold text-xl">
          {isPercentage ? `${coupon.discountValue}% off` : `R$ ${coupon.discountValue} off`}
        </h3>
        <p className="text-gray-600 text-sm mb-2">
          {coupon.description || (coupon.firstOrderOnly ? 'no seu primeiro pedido' : 'em todo o site')}
        </p>
        <Button 
          variant="default" 
          className="bg-primary hover:bg-primary/90 text-white font-bold h-8 text-xs px-3 uppercase"
          onClick={copyToClipboard}
        >
          CUPOM: {coupon.code}
        </Button>
      </div>
      <div className="bg-primary/10 w-16 h-16 rounded-lg ml-4 flex items-center justify-center">
         <span className="text-2xl">🎟️</span>
      </div>
    </div>
  );
};
