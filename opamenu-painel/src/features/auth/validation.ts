import { z } from "zod";

export const loginSchema = z.object({
  usernameOrEmail: z.string().min(1, "E-mail ou usuário é obrigatório"),
  password: z.string().min(1, "Senha é obrigatória"),
});

export type LoginFormData = z.infer<typeof loginSchema>;

export const registerSchema = z
  .object({
    companyName: z.string().min(3, "Nome da empresa deve ter no mínimo 3 caracteres"),
    document: z.string().min(11, "CNPJ/CPF inválido"),
    firstName: z.string().min(2, "Nome deve ter no mínimo 2 caracteres"),
    lastName: z.string().min(2, "Sobrenome deve ter no mínimo 2 caracteres"),
    email: z.string().email("E-mail inválido"),
    password: z.string().min(6, "A senha deve ter no mínimo 6 caracteres"),
    confirmPassword: z.string(),
  })
  .refine((data) => data.password === data.confirmPassword, {
    message: "As senhas não conferem",
    path: ["confirmPassword"],
  });

export type RegisterFormData = z.infer<typeof registerSchema>;
