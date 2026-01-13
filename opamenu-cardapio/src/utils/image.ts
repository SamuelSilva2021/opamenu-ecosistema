import { API_BASE_URL } from '@/config/api';

/**
 * Constrói a URL completa para uma imagem do produto
 * @param imageUrl - Caminho relativo da imagem retornado pela API
 * @returns URL completa da imagem ou null se não houver imagem
 */
export function buildImageUrl(imageUrl: string | null | undefined): string | null {
  if (!imageUrl) {
    return null;
  }
  
  // Se já é uma URL completa, retorna como está
  if (imageUrl.startsWith('http://') || imageUrl.startsWith('https://')) {
    return imageUrl;
  }
  
  // Constrói a URL completa baseada na configuração da API
  return `${API_BASE_URL}/uploads/${imageUrl}`;
}

/**
 * Verifica se uma URL de imagem é válida
 * @param imageUrl - URL da imagem para verificar
 * @returns Promise que resolve em true se a imagem existe
 */
export async function isImageValid(imageUrl: string): Promise<boolean> {
  try {
    const response = await fetch(imageUrl, { method: 'HEAD' });
    return response.ok;
  } catch {
    return false;
  }
}
