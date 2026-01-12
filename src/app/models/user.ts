export interface User {
    id: string;
    email: string;
    isAdmin: boolean;
    passwordHash: string;
    passwordSalt?: string;
}
