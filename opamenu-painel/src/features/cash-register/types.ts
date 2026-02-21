export const CashShiftStatus = {
    Open: 1,
    Closed: 2
} as const;

export type CashShiftStatus = typeof CashShiftStatus[keyof typeof CashShiftStatus];

export const CashMovementType = {
    Opening: 1,
    OrderPayment: 2,
    Inbound: 3,
    Outbound: 4,
    Reversed: 5,
    Closing: 6
} as const;

export type CashMovementType = typeof CashMovementType[keyof typeof CashMovementType];

export interface CashMovement {
    id: string;
    type: CashMovementType;
    amount: number;
    description: string;
    paymentMethod?: string;
    orderId?: string;
    orderNumber?: number;
    createdAt: string;
}

export interface CashShift {
    id: string;
    userId: string;
    userName?: string;
    openedAt: string;
    closedAt?: string;
    openingBalance: number;
    closingBalance?: number;
    expectedBalance: number;
    status: CashShiftStatus;
    movements: CashMovement[];
}

export interface OpenCashShiftRequest {
    openingBalance: number;
}

export interface CloseCashShiftRequest {
    closingBalance: number;
}

export interface AddCashMovementRequest {
    type: CashMovementType;
    amount: number;
    description: string;
    paymentMethod?: string;
}
