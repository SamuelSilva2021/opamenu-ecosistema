import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription } from "@/components/ui/dialog";
import { TenantBusinessInfo } from "@/types/api";
import { MapPin, Clock, CreditCard, Phone, Mail, Instagram, Facebook } from "lucide-react";

interface TenantInfoModalProps {
  isOpen: boolean;
  onClose: () => void;
  tenant: TenantBusinessInfo;
}

const TenantInfoModal = ({ isOpen, onClose, tenant }: TenantInfoModalProps) => {
  const openingHours = tenant.openingHours;
  const paymentMethods = tenant.paymentMethods;

  const fullAddress = [
      tenant.addressStreet,
      tenant.addressNumber,
      tenant.addressNeighborhood,
      tenant.addressCity,
      tenant.addressState
  ].filter(Boolean).join(', ');

  const translateDay = (day: string) => {
      const map: Record<string, string> = {
          monday: "Segunda-feira", tuesday: "Terça-feira", wednesday: "Quarta-feira",
          thursday: "Quinta-feira", friday: "Sexta-feira", saturday: "Sábado", sunday: "Domingo"
      };
      return map[day] || day;
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle className="text-2xl font-bold flex items-center gap-2">
            {tenant.logoUrl ? (
              <img src={tenant.logoUrl} alt={tenant.name} className="w-10 h-10 rounded-full object-cover" />
            ) : (
               <div className="w-10 h-10 rounded-full bg-gray-200 flex items-center justify-center text-gray-500 font-bold">
                 {tenant.name.substring(0, 1)}
               </div>
            )}
            {tenant.name}
          </DialogTitle>
          {tenant.description && (
            <DialogDescription className="text-base mt-2">
              {tenant.description}
            </DialogDescription>
          )}
        </DialogHeader>

        <div className="space-y-6 py-4">
          {/* Address & Contact */}
          <div className="grid gap-4 md:grid-cols-2">
            <div className="space-y-3">
              <h3 className="font-semibold text-lg flex items-center gap-2">
                <MapPin className="w-5 h-5 text-primary" /> Endereço
              </h3>
              <p className="text-muted-foreground">{fullAddress || "Endereço não informado"}</p>
              {tenant.addressZipcode && (
                 <p className="text-sm text-muted-foreground">CEP: {tenant.addressZipcode}</p>
              )}
              {/* Map Placeholder */}
              {/*TODO: Implementar mapa com endereço*/}
              {/* <div className="w-full h-32 bg-muted rounded-md flex items-center justify-center text-sm text-muted-foreground">
                Mapa Indisponível
              </div> */}
            </div>

            <div className="space-y-3">
              <h3 className="font-semibold text-lg flex items-center gap-2">
                <Phone className="w-5 h-5 text-primary" /> Contato
              </h3>
              <div className="space-y-2">
                {tenant.phone && (
                  <div className="flex items-center gap-2 text-muted-foreground">
                    <Phone className="w-4 h-4" /> {tenant.phone}
                  </div>
                )}
                {tenant.whatsappNumber && (
                  <div className="flex items-center gap-2 text-muted-foreground">
                    <span className="font-bold text-green-600">WhatsApp:</span> {tenant.whatsappNumber}
                  </div>
                )}
                {tenant.email && (
                  <div className="flex items-center gap-2 text-muted-foreground">
                    <Mail className="w-4 h-4" /> {tenant.email}
                  </div>
                )}
                <div className="flex gap-4 mt-2">
                  {tenant.instagramUrl && (
                    <a href={tenant.instagramUrl} target="_blank" rel="noopener noreferrer" className="text-pink-600 hover:opacity-80">
                      <Instagram className="w-6 h-6" />
                    </a>
                  )}
                  {tenant.facebookUrl && (
                    <a href={tenant.facebookUrl} target="_blank" rel="noopener noreferrer" className="text-blue-600 hover:opacity-80">
                      <Facebook className="w-6 h-6" />
                    </a>
                  )}
                </div>
              </div>
            </div>
          </div>

          {/* Opening Hours */}
          <div className="space-y-3">
            <h3 className="font-semibold text-lg flex items-center gap-2">
              <Clock className="w-5 h-5 text-primary" /> Horário de Funcionamento
            </h3>
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-2 text-sm text-muted-foreground">
              {openingHours ? (
                Object.entries(openingHours).map(([day, config]) => (
                  <div key={day} className="flex justify-between border-b pb-1 last:border-0">
                    <span className="capitalize">{translateDay(day)}</span>
                    <span className="font-medium">
                        {config.isOpen ? `${config.open} - ${config.close}` : "Fechado"}
                    </span>
                  </div>
                ))
              ) : (
                <p>Horários não informados</p>
              )}
            </div>
          </div>

          {/* Payment Methods */}
          <div className="space-y-3">
             <h3 className="font-semibold text-lg flex items-center gap-2">
                <CreditCard className="w-5 h-5 text-primary" /> Formas de Pagamento
             </h3>
             <div className="flex flex-wrap gap-2">
                {paymentMethods && paymentMethods.length > 0 ? (
                    paymentMethods.map((method, index) => (
                        <div key={index} className="px-3 py-1 bg-secondary text-secondary-foreground rounded-full text-sm">
                            {method}
                        </div>
                    ))
                ) : (
                    <p className="text-muted-foreground">Não informado</p>
                )}
             </div>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
};

export default TenantInfoModal;
