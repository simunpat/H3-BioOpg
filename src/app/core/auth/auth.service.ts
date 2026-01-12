import { Injectable, computed, signal } from '@angular/core';
import { Router } from '@angular/router';

export interface AuthToken {
    sub: string;
    isAdmin: boolean;
    exp: number; // epoch seconds
}

const TOKEN_KEY = 'auth_token';

// Base64url helpers (sufficient for ASCII JSON)
function b64urlEncode(input: string): string {
    return btoa(input).replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/g, '');
}

function b64urlDecode(input: string): string {
    const pad = '='.repeat((4 - (input.length % 4)) % 4);
    const s = input.replace(/-/g, '+').replace(/_/g, '/') + pad;

    return atob(s);
}

function toMockJwt(payload: AuthToken): string {
    const header = b64urlEncode(JSON.stringify({ alg: 'none', typ: 'JWT' }));
    const body = b64urlEncode(JSON.stringify(payload));

    // Empty signature for mock; later the backend will return a real signature.
    return `${header}.${body}.`;
}

function parseJwt(jwt: string | null): AuthToken | null {
    if (!jwt) return null;

    const parts = jwt.split('.');

    if (parts.length < 2) return null;

    try {
        const json = b64urlDecode(parts[1]);

        const raw = JSON.parse(json) as any;

        // Coerce isAdmin to a strict boolean even if backend serialized as string
        const isAdmin: boolean =
            typeof raw.isAdmin === 'boolean'
                ? raw.isAdmin
                : String(raw.isAdmin).toLowerCase() === 'true';

        return { ...raw, isAdmin } as AuthToken;
    } catch {
        return null;
    }
}

@Injectable({ providedIn: 'root' })
export class AuthService {
    // Store the raw JWT string
    private readonly jwtSignal = signal<string | null>(this.loadJwt());

    // Decode without side-effects (no logout) for lightweight reads
    private readonly decoded = computed(() => parseJwt(this.jwtSignal()));

    readonly isAuthenticated = computed(() => !!this.decoded());
    readonly isAdmin = computed(() => this.decoded()?.isAdmin ?? false);
    readonly userId = computed(() => this.decoded()?.sub ?? null);

    constructor(private readonly router: Router) {}

    loginWithJwt(jwt: string): void {
        localStorage.setItem(TOKEN_KEY, jwt);
        this.jwtSignal.set(jwt);
    }

    logout(): void {
        localStorage.removeItem(TOKEN_KEY);

        this.jwtSignal.set(null);

        void this.router.navigate(['/login']);
    }

    // Returns decoded payload and enforces expiry (will logout if expired)
    getToken(): AuthToken | null {
        const p = parseJwt(this.jwtSignal());

        if (!p) return null;

        if (p.exp * 1000 < Date.now()) {
            this.logout();

            return null;
        }

        return p;
    }

    // Gives the raw JWT string (used by the interceptor)
    getJwt(): string | null {
        return this.jwtSignal();
    }

    private loadJwt(): string | null {
        return localStorage.getItem(TOKEN_KEY);
    }
}
