import { api } from "@/lib/axios";

export interface FileUploadResult {
  isSuccess: boolean;
  errorMessage?: string;
  fileName?: string;
  originalFileName?: string;
  filePath?: string;
  fileUrl?: string;
  fileSize: number;
  mimeType?: string;
  uploadDate: string;
  imageVariants: string[];
  variants: string[];
  imageMetadata?: {
    width: number;
    height: number;
    format: string;
    fileSize: number;
    aspectRatio: number;
    colorDepth: number;
    hasTransparency: boolean;
  };
}

export const filesService = {
  uploadFile: async (file: File, folder: string = "products"): Promise<FileUploadResult> => {
    const formData = new FormData();
    formData.append("file", file);
    formData.append("folder", folder);

    const response = await api.post<FileUploadResult>("/files/upload", formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
    return response.data;
  },

  uploadMultipleFiles: async (files: File[], folder: string = "products"): Promise<FileUploadResult[]> => {
    const formData = new FormData();
    files.forEach((file) => {
      formData.append("files", file);
    });
    formData.append("folder", folder);

    const response = await api.post<FileUploadResult[]>("/files/upload-multiple", formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
    return response.data;
  },
};
