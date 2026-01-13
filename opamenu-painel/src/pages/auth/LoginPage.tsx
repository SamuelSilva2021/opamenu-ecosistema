import { LoginForm } from "@/features/auth/components/LoginForm";
import { Link } from "react-router-dom";
import { cn } from "@/lib/utils";
import { buttonVariants } from "@/components/ui/button";

export default function LoginPage() {
  return (
    <div className="container relative min-h-screen flex-col items-center justify-center grid lg:max-w-none lg:grid-cols-2 lg:px-0">
      <Link
        to="/register"
        className={cn(
          buttonVariants({ variant: "ghost" }),
          "absolute right-4 top-4 md:right-8 md:top-8"
        )}
      >
        Cadastre-se
      </Link>
      <div className="relative hidden h-full flex-col bg-muted p-10 text-white dark:border-r lg:flex">
        <div className="absolute inset-0 bg-zinc-900" />
        <div className="relative z-20 flex items-center text-lg font-medium">
          Opamenu Painel
        </div>
        <div className="relative z-20 flex flex-1 items-center justify-center">
          <img 
            src="/logo-normal.png" 
            alt="OpaMenu Logo Grande" 
            className="h-[300px] w-[300px] rounded-full object-cover shadow-2xl" 
          />
        </div>
        <div className="relative z-20 mt-auto">
          <blockquote className="space-y-2">
            <p className="text-lg">
              &ldquo;Gerencie seu cardápio digital com agilidade e eficiência. 
              O controle total do seu restaurante em um só lugar.&rdquo;
            </p>
            <footer className="text-sm">OpaMenu Team</footer>
          </blockquote>
        </div>
      </div>
      <div className="lg:p-8">
        <div className="mx-auto flex w-full flex-col justify-center space-y-6 sm:w-[350px]">
          <div className="flex flex-col space-y-2 text-center">
            <h1 className="text-2xl font-semibold tracking-tight">
              Acesse sua conta
            </h1>
            <p className="text-sm text-muted-foreground">
              Entre com seu e-mail e senha para gerenciar seu estabelecimento
            </p>
          </div>
          <LoginForm />
          <p className="px-8 text-center text-sm text-muted-foreground">
            Não tem uma conta?{" "}
            <Link
              to="/register"
              className="underline underline-offset-4 hover:text-primary"
            >
              Cadastre-se agora
            </Link>
          </p>
          <p className="px-8 text-center text-sm text-muted-foreground">
            Ao clicar em continuar, você concorda com nossos{" "}
            <a
              href="#"
              className="underline underline-offset-4 hover:text-primary"
            >
              Termos de Serviço
            </a>{" "}
            e{" "}
            <a
              href="#"
              className="underline underline-offset-4 hover:text-primary"
            >
              Política de Privacidade
            </a>
            .
          </p>
        </div>
      </div>
    </div>
  );
}
