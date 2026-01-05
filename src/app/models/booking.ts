export interface Booking {
    id: string;
    screeningId: string;
    userId: string;
    seats: { row: number; number: number }[];
    items?: { ticketTypeId: string; name?: string; qty: number }[];
    totalPrice: number;
}
