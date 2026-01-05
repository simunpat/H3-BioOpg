// Minimal typing shim for crypto-js/sha256 to satisfy TS in this project.
declare module 'crypto-js/sha256' {
    const SHA256: (message: unknown) => { toString(): string };
    export default SHA256;
}

