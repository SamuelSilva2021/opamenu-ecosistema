import { API_CONFIG } from '@/config/api';

/**
 * Converte uma URL de imagem relativa em URL absoluta apontando para o servidor da API
 * @param imageUrl URL relativa da imagem (ex: "products/2025/08/filename.jpg")
 * @returns URL absoluta da imagem ou null se não houver imageUrl
 */
export const getFullImageUrl = (imageUrl?: string): string | null => {
  if (!imageUrl) return null;
  
  // Se já é uma URL absoluta (começa com http/https), retornar como está
  if (imageUrl.startsWith('http://') || imageUrl.startsWith('https://')) {
    return imageUrl;
  }
  
  // Construir URL absoluta combinando base URL da API com a URL relativa
  const apiBaseUrl = API_CONFIG.BASE_URL === '/api' 
    ? 'http://localhost:5072' // URL padrão do servidor API em desenvolvimento
    : API_CONFIG.BASE_URL.replace('/api', ''); // Remover /api do final se existir
  
  // Garantir que não há barras duplas
  const cleanImageUrl = imageUrl.startsWith('/') ? imageUrl.substring(1) : imageUrl;
  const cleanBaseUrl = apiBaseUrl.endsWith('/') ? apiBaseUrl.slice(0, -1) : apiBaseUrl;
  
  // Adicionar prefixo /uploads para imagens armazenadas no servidor
  const fullUrl = `${cleanBaseUrl}/uploads/${cleanImageUrl}`;
  
  return fullUrl;
};

/**
 * Hook React para converter URLs de imagem
 */
export const useImageUrl = (imageUrl?: string) => {
  return getFullImageUrl(imageUrl);
};
