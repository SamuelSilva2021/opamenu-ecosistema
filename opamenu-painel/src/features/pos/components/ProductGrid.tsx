import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { productsService } from "../../products/products.service";
import { categoriesService } from "../../categories/categories.service";
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Search, Plus } from "lucide-react";
import { Button } from "@/components/ui/button";
import { usePOSStore } from "../store/pos.store";
import type { Product } from "../../products/types";

export function ProductGrid() {
  const [selectedCategory, setSelectedCategory] = useState<string>("all");
  const [searchTerm, setSearchTerm] = useState("");
  const addItem = usePOSStore((state) => state.addItem);

  const { data: categories = [] } = useQuery({
    queryKey: ["categories", "active"],
    queryFn: categoriesService.getActiveCategories,
  });

  const { data: products = [] } = useQuery({
    queryKey: ["products", { searchTerm, categoryId: selectedCategory === "all" ? undefined : selectedCategory }],
    queryFn: () => productsService.getProducts({
      searchTerm,
      categoryId: selectedCategory === "all" ? undefined : selectedCategory
    }),
  });

  const handleAddToCart = (product: Product) => {
    // Aqui poderia abrir um modal para adicionais se tiver
    // Por enquanto adiciona direto
    addItem(product, 1);
  };

  return (
    <div className="flex flex-col h-full gap-4 p-4">
      <div className="flex gap-4 items-center">
        <div className="relative flex-1">
          <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input
            type="search"
            placeholder="Buscar produtos..."
            className="pl-8"
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
          />
        </div>
      </div>

      <div className="flex w-full justify-start overflow-x-auto h-auto p-1 bg-transparent gap-2 pb-2">
        <Button
          variant={selectedCategory === "all" ? "default" : "outline"}
          className="rounded-full"
          onClick={() => setSelectedCategory("all")}
        >
          Todos
        </Button>
        {categories.map((cat) => (
          <Button
            key={cat.id}
            variant={selectedCategory === cat.id ? "default" : "outline"}
            className="rounded-full whitespace-nowrap"
            onClick={() => setSelectedCategory(cat.id)}
          >
            {cat.name}
          </Button>
        ))}
      </div>

      <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4 overflow-y-auto pb-4 flex-1 content-start pr-2">
        {products.map((product) => (
          <Card
            key={product.id}
            className="cursor-pointer hover:border-primary transition-colors flex flex-col justify-between"
            onClick={() => handleAddToCart(product)}
          >
            <CardContent className="p-4">
              {product.imageUrl ? (
                <div className="w-full h-32 mb-2 rounded-md overflow-hidden bg-muted/10 flex items-center justify-center p-2">
                  <img
                    src={product.imageUrl}
                    alt={product.name}
                    className="w-full h-full object-contain"
                  />
                </div>
              ) : (
                <div className="w-full h-32 bg-muted rounded-md mb-2 flex items-center justify-center text-muted-foreground">
                  Sem imagem
                </div>
              )}
              <h3 className="font-medium line-clamp-2 text-sm">{product.name}</h3>
              <div className="flex justify-between items-center mt-2">
                <span className="font-bold text-lg">
                  {new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(product.price)}
                </span>
                <Button size="icon" variant="secondary" className="h-8 w-8 rounded-full">
                  <Plus className="h-4 w-4" />
                </Button>
              </div>
            </CardContent>
          </Card>
        ))}
        {products.length === 0 && (
          <div className="col-span-full text-center py-10 text-muted-foreground">
            Nenhum produto encontrado.
          </div>
        )}
      </div>
    </div>
  );
}
