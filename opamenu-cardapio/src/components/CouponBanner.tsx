import { Coupon } from "@/types/api";
import {
  Carousel,
  CarouselContent,
  CarouselItem,
  CarouselNext,
  CarouselPrevious,
} from "@/components/ui/carousel";
import { CouponCard } from "@/components/CouponCard";

interface CouponBannerProps {
  coupons: Coupon[];
}

export const CouponBanner = ({ coupons }: CouponBannerProps) => {
  const activeCoupons = coupons?.filter(coupon => coupon.isActive) || [];
  if (activeCoupons.length === 0) return null;

  return (
    <div className="w-full max-w-6xl mx-auto px-4 my-6">
      <div className="flex items-center gap-2 mb-4">
        <span className="text-2xl">🎫</span>
        <h2 className="text-xl font-bold text-gray-800">Cupons Disponíveis</h2>
      </div>
      
      <Carousel
        opts={{
          align: "start",
          loop: true,
        }}
        className="w-full"
      >
        <CarouselContent className="-ml-2 md:-ml-4">
          {activeCoupons.map((coupon) => (
            <CarouselItem key={coupon.id} className="pl-2 md:pl-4 md:basis-1/2 lg:basis-1/3">
              <div className="p-1 h-full">
                <CouponCard 
                  coupon={coupon} 
                  className="w-full h-full min-w-0 snap-align-none shadow-sm hover:shadow-md transition-shadow" 
                />
              </div>
            </CarouselItem>
          ))}
        </CarouselContent>
        <div className="hidden md:block">
            <CarouselPrevious />
            <CarouselNext />
        </div>
      </Carousel>
    </div>
  );
};
