import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";

interface Category {
  id: string;
  name: string;
  count: number;
}

interface CategoryFilterProps {
  categories: Category[];
  selectedCategory: string;
  onCategoryChange: (categoryId: string) => void;
}

const CategoryFilter = ({ categories, selectedCategory, onCategoryChange }: CategoryFilterProps) => {
  return (
    <div className="bg-card border-b border-border">
      <div className="container mx-auto px-4 py-4">
        <div className="flex gap-2 overflow-x-auto scrollbar-hide">
          <Button
            variant={selectedCategory === "all" ? "default" : "outline"}
            onClick={() => onCategoryChange("all")}
            className={`flex-shrink-0 transition-all duration-200 ${
              selectedCategory === "all"
                ? "bg-opamenu-green hover:bg-opamenu-green/90 text-white"
                : "hover:bg-opamenu-green/10 hover:border-opamenu-green"
            }`}
          >
            Todos
            <Badge variant="secondary" className="ml-2">
              {categories.reduce((total, cat) => total + cat.count, 0)}
            </Badge>
          </Button>
          
          {categories.map((category) => (
            <Button
              key={category.id}
              variant={selectedCategory === category.id ? "default" : "outline"}
              onClick={() => onCategoryChange(category.id)}
              className={`flex-shrink-0 transition-all duration-200 ${
                selectedCategory === category.id
                  ? "bg-opamenu-green hover:bg-opamenu-green/90 text-white"
                  : "hover:bg-opamenu-green/10 hover:border-opamenu-green"
              }`}
            >
              {category.name}
              <Badge variant="secondary" className="ml-2">
                {category.count}
              </Badge>
            </Button>
          ))}
        </div>
      </div>
    </div>
  );
};

export default CategoryFilter;