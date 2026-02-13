import { useParams, useNavigate } from 'react-router-dom';
import { useLoyalty, useCustomer, useStorefront } from '@/hooks';
import { ChevronLeft, Gift, Award, Star, Wallet, Info, Loader2 } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Progress } from '@/components/ui/progress';
import { ELoyaltyProgramType, ELoyaltyRewardType } from '@/types/api';
import { CustomerProvider } from '@/hooks/use-customer';
import { CartProvider } from '@/hooks/use-cart';

const LoyaltyPageContent = () => {
    const { slug } = useParams<{ slug: string }>();
    const navigate = useNavigate();
    const { customer } = useCustomer();
    const { tenantBusiness } = useStorefront(slug);
    const { programs, balance, loading, error } = useLoyalty(slug, customer?.phone);

    const getProgramTypeDescription = (type: ELoyaltyProgramType) => {
        switch (type) {
            case ELoyaltyProgramType.PointsPerValue:
                return 'Ganhe pontos a cada real gasto em seus pedidos.';
            case ELoyaltyProgramType.OrderCount:
                return 'Acumule pedidos e ganhe recompensas exclusivas.';
            case ELoyaltyProgramType.ItemCount:
                return 'Compre itens específicos para completar seu cartão fidelidade.';
            default:
                return '';
        }
    };

    const getRewardDescription = (type?: ELoyaltyRewardType, value?: number) => {
        if (value === undefined) return '';
        switch (type) {
            case ELoyaltyRewardType.DiscountPercentage:
                return `${value}% de desconto no seu próximo pedido`;
            case ELoyaltyRewardType.DiscountValue:
                return `R$ ${value.toFixed(2)} de desconto no seu próximo pedido`;
            case ELoyaltyRewardType.FreeProduct:
                return 'Gahne um produto grátis na sua próxima compra';
            default:
                return 'Recompensa especial';
        }
    };

    if (loading) {
        return (
            <div className="min-h-screen flex items-center justify-center bg-gray-50 text-primary">
                <Loader2 className="w-8 h-8 animate-spin" />
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-gray-50 pb-20">
            <header className="bg-white border-b border-gray-100 sticky top-0 z-50">
                <div className="container mx-auto px-4 h-16 flex items-center gap-4">
                    <Button
                        variant="ghost"
                        size="icon"
                        onClick={() => navigate(`/${slug}`)}
                        className="rounded-full"
                    >
                        <ChevronLeft className="w-6 h-6" />
                    </Button>
                    <h1 className="text-xl font-bold text-gray-800">Fidelidade</h1>
                </div>
            </header>

            <main className="container mx-auto px-4 py-8 max-w-2xl space-y-8">
                {/* User Balance Section */}
                {customer && (
                    <Card className="border-none shadow-md bg-white overflow-hidden">
                        <div className="bg-primary/5 p-6 flex items-center justify-between">
                            <div className="flex items-center gap-4">
                                <div className="bg-primary p-3 rounded-2xl text-white shadow-lg shadow-primary/20">
                                    <Wallet className="w-6 h-6" />
                                </div>
                                <div>
                                    <p className="text-sm font-medium text-gray-500">Seu saldo atual</p>
                                    <p className="text-3xl font-black text-primary">
                                        {balance?.balance || 0} <span className="text-sm font-bold uppercase tracking-widest text-primary/60 ml-1">pontos</span>
                                    </p>
                                </div>
                            </div>
                            <div className="text-right">
                                <p className="text-xs font-bold text-gray-400 uppercase tracking-tighter">Total acumulado</p>
                                <p className="font-bold text-gray-700">{balance?.totalEarned || 0} pts</p>
                            </div>
                        </div>
                    </Card>
                )}

                {/* Info Card if not logged in */}
                {!customer && (
                    <Card className="border-none shadow-sm bg-blue-50">
                        <CardContent className="p-6 flex gap-4">
                            <div className="bg-blue-100 p-2 rounded-full h-fit text-blue-600">
                                <Info className="w-5 h-5" />
                            </div>
                            <div>
                                <h3 className="font-bold text-blue-900 mb-1">Identifique-se para ver seus pontos</h3>
                                <p className="text-sm text-blue-800/70">
                                    Faça login ou cadastre-se no menu principal para começar a acumular pontos e resgatar recompensas.
                                </p>
                            </div>
                        </CardContent>
                    </Card>
                )}

                {/* Active Programs Section */}
                <section className="space-y-4">
                    <h2 className="text-lg font-bold text-gray-800 flex items-center gap-2">
                        <Star className="w-5 h-5 text-yellow-500 fill-yellow-500" />
                        Programas Ativos
                    </h2>

                    {programs.length === 0 ? (
                        <div className="text-center py-12 bg-white rounded-3xl border border-dashed border-gray-200">
                            <Gift className="w-12 h-12 text-gray-300 mx-auto mb-4" />
                            <p className="text-gray-500">Nenhum programa de fidelidade ativo no momento.</p>
                        </div>
                    ) : (
                        <div className="grid gap-4">
                            {programs.map((program) => {
                                const isUserInThisProgram = balance?.program?.id === program.id;
                                const progress = isUserInThisProgram && program.targetCount
                                    ? (balance.balance / program.targetCount) * 100
                                    : 0;

                                return (
                                    <Card key={program.id} className="border-none shadow-sm hover:shadow-md transition-shadow">
                                        <CardHeader className="pb-2">
                                            <div className="flex justify-between items-start">
                                                <CardTitle className="text-lg font-bold text-gray-800">{program.name}</CardTitle>
                                                <div className={`
                          px-2 py-1 rounded-full text-[10px] font-black uppercase tracking-widest
                          ${isUserInThisProgram ? 'bg-green-100 text-green-700' : 'bg-gray-100 text-gray-500'}
                        `}>
                                                    {isUserInThisProgram ? 'Sua meta atual' : 'Participe'}
                                                </div>
                                            </div>
                                            <p className="text-sm text-gray-500 leading-tight">
                                                {getProgramTypeDescription(program.type)}
                                            </p>
                                        </CardHeader>
                                        <CardContent className="space-y-4">
                                            {program.description && (
                                                <div className="bg-gray-50 p-3 rounded-xl">
                                                    <p className="text-xs text-gray-600 italic">"{program.description}"</p>
                                                </div>
                                            )}

                                            <div className="flex items-center gap-3 p-4 bg-primary/5 rounded-2xl border border-primary/10">
                                                <div className="bg-primary p-2 rounded-full text-white">
                                                    <Award className="w-5 h-5" />
                                                </div>
                                                <div>
                                                    <p className="text-[10px] font-bold text-primary uppercase tracking-widest">Recompensa</p>
                                                    <p className="font-bold text-gray-800">
                                                        {getRewardDescription(program.rewardType, program.rewardValue)}
                                                    </p>
                                                </div>
                                            </div>

                                            {program.targetCount && (
                                                <div className="space-y-2">
                                                    <div className="flex justify-between text-xs font-bold text-gray-500">
                                                        <span>Progresso</span>
                                                        <span>{isUserInThisProgram ? balance.balance : 0} / {program.targetCount}</span>
                                                    </div>
                                                    <Progress value={progress} className="h-2 rounded-full" />
                                                </div>
                                            )}
                                        </CardContent>
                                    </Card>
                                );
                            })}
                        </div>
                    )}
                </section>

                {/* FAQ Section */}
                <section className="space-y-4">
                    <h2 className="text-lg font-bold text-gray-800">Como funciona?</h2>
                    <div className="grid gap-4">
                        <div className="flex gap-4">
                            <div className="bg-gray-100 w-8 h-8 rounded-full flex items-center justify-center shrink-0 font-bold text-gray-600">1</div>
                            <div>
                                <h4 className="font-bold text-gray-800">Identifique-se</h4>
                                <p className="text-sm text-gray-600">Sempre use seu número de telefone ao fazer um pedido para acumular pontos automaticamente.</p>
                            </div>
                        </div>
                        <div className="flex gap-4">
                            <div className="bg-gray-100 w-8 h-8 rounded-full flex items-center justify-center shrink-0 font-bold text-gray-600">2</div>
                            <div>
                                <h4 className="font-bold text-gray-800">Acumule pontos</h4>
                                <p className="text-sm text-gray-600">Cada restaurante possui sua própria regra de pontuação. Verifique os programas ativos acima.</p>
                            </div>
                        </div>
                        <div className="flex gap-4">
                            <div className="bg-gray-100 w-8 h-8 rounded-full flex items-center justify-center shrink-0 font-bold text-gray-600">3</div>
                            <div>
                                <h4 className="font-bold text-gray-800">Resgate prêmios</h4>
                                <p className="text-sm text-gray-600">Ao atingir a pontuação necessária, você poderá selecionar sua recompensa diretamente no checkout.</p>
                            </div>
                        </div>
                    </div>
                </section>
            </main>
        </div>
    );
};

const LoyaltyPageWrapper = () => {
    const { slug } = useParams<{ slug: string }>();
    return (
        <CustomerProvider slug={slug}>
            <CartProvider slug={slug}>
                <LoyaltyPageContent />
            </CartProvider>
        </CustomerProvider>
    );
};

export default LoyaltyPageWrapper;
