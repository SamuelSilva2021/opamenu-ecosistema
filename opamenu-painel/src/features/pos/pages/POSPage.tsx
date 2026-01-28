import { ProductGrid } from "../components/ProductGrid";
import { CartSidebar } from "../components/CartSidebar";

export function POSPage() {
  return (
    <div className="flex h-[calc(100vh-8rem)] overflow-hidden rounded-lg border bg-background shadow-sm">
      <div className="flex-1 overflow-hidden bg-gray-50/50 dark:bg-zinc-900/50">
        <ProductGrid />
      </div>
      <div className="w-[400px] border-l h-full bg-background">
        <CartSidebar />
      </div>
    </div>
  );
}
