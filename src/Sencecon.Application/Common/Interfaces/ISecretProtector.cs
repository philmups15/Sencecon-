namespace Sencecon.Application.Common.Interfaces;

// Encrypt-at-rest for stored third-party credentials (e.g. IntegrationSetting
// API keys) — not for passwords, which use IPasswordHasher's one-way hashing.
public interface ISecretProtector
{
    string Protect(string plaintext);
    string Unprotect(string ciphertext);
}
