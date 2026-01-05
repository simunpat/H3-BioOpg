export type UserRole = 'Admin' | 'Customer';

export interface User {
    id: string;
    email: string;
    role: UserRole;
    passwordHash: string;
    passwordSalt?: string;
}
