# Admin redesign — Phase 2 backend scope

Companion to `docs/admin-redesign-brief.md` and the Phase 1 implementation in the
frontend repo (`SenceCon/src/screens/admin/*`). Phase 1 shipped a full Users/Roles/
Audit/Integrations UI, but four pieces are frontend-only stubs today because the
endpoints below don't exist yet. This doc scopes each one against this repo's
actual patterns (verified by reading the code, not assumed) so implementation can
start without re-deriving conventions.

Frontend already calls these paths for real (see `src/lib/api.js`, each marked
`TODO(Phase 2 — Sencecon.API)`) — they currently 404, and the UI shows that
honestly rather than faking success. Once each endpoint ships, the corresponding
frontend TODO/comment should be removed and any client-only workaround (see
"Frontend follow-up" under each section) cleaned up.

## Conventions to reuse (already in the codebase)

- **Command pattern**: `Command : IRequest<TDto>` + `CommandHandler` in one file
  (e.g. `Sencecon.Application/Users/Commands/UpdateUserRole/UpdateUserRoleCommand.cs`).
  Load entity via `IApplicationDbContext`, throw `NotFoundException`/`ConflictException`
  from `Sencecon.Domain.Exceptions` (handled centrally by `ExceptionHandlingMiddleware`
  — no per-controller error handling needed), mutate, set `LastModified`, `SaveChangesAsync`.
- **Self-action guard**: `UpdateUserRoleCommandHandler` blocks an admin from targeting
  their own account (`RequestingUserId` passed in from the controller's `CurrentUserId`,
  compared to the target `UserId`, throws `ConflictException` if equal). Mirror this
  wherever an admin action targets another user.
- **Authorization**: `[Authorize(Roles = Roles.Admin)]` on the controller action —
  `UsersController` already does this for `GetAll`/`UpdateRole`, not the `ModuleAccess.*`
  constants (those are for the module-level Read/Write matrix, not admin-only actions).
- **Audit logging is automatic**: `AuditLoggingBehaviour` (MediatR pipeline behaviour)
  logs every handled command by humanizing its type name — no manual audit-log call
  needed in a new handler, just implement it as a proper `IRequest`.
- **New entity checklist**: (1) POCO in `Sencecon.Domain/Entities` extending
  `BaseAuditableEntity`, (2) `IEntityTypeConfiguration<T>` in
  `Sencecon.Infrastructure/Persistence/Configurations` (auto-discovered), (3) `DbSet<T>`
  in `ApplicationDbContext.cs`, (4) `dotnet ef migrations add <Name>`. Migrations run
  automatically on startup (`db.Database.MigrateAsync()` in `Program.cs`) — no manual
  deploy step.

## 1. Disable / enable user — small–medium

**Endpoint**: `PUT /api/users/{id}/status`, `[Authorize(Roles = Roles.Admin)]`,
body `{ enabled: bool }`. Frontend already calls this exact shape
(`setUserStatus` in `api.js`).

**Data model**: add `bool IsActive { get; set; } = true;` to `User.cs`. No existing
status/soft-delete convention to reuse — every other "status" in this codebase
(`NonConformityStatus`, `BomStatus`, etc.) is a domain-specific enum on its own
entity, not a shared pattern, so a plain bool is the right level of complexity here.

**Handler**: mirror `UpdateUserRoleCommandHandler` exactly — block
`UserId == RequestingUserId` (an admin can't disable themselves), `NotFoundException`
if the target doesn't exist, set `IsActive`, `LastModified`, save.

**⚠️ Open problem — this won't actually revoke access immediately.** JWTs here are
stateless: `JwtTokenGenerator` signs a token with a `role` claim and
`Program.cs`'s `AddJwtBearer` only validates signature/issuer/audience/expiry —
there's no DB lookup, no revocation list, nothing checking `IsActive` per request.
A disabled user's existing token stays valid until it naturally expires
(`JwtSettings.ExpiryMinutes` = 60 in production, 120 in dev). The frontend copy
already says "will lose access immediately," which won't be true without one more
piece:

- **Recommended**: add an `OnTokenValidated` handler in the `AddJwtBearer` config
  in `Program.cs` that loads the user by the token's `sub` claim and rejects
  (`context.Fail(...)`) if `IsActive == false`. Adds one DB read per authenticated
  request — acceptable at this app's scale, and it's the only way to make
  "disable" actually immediate without building token revocation infrastructure.
- **Alternative (cheaper, weaker)**: ship the status field/endpoint now, accept
  that disabling has up to a 60–120 min tail before it takes effect, and fix the
  frontend copy to say so. Only reasonable as a stopgap.

**Frontend follow-up**: `AdminUsers.jsx` currently hardcodes every user's status
to `'Active'` client-side (there's no field to read yet). Once `role`/`status`
comes back on `UserDto`, update `toUserView`/`AdminUsers.jsx` to read the real
field instead of the hardcoded default.

## 2. Admin-set password — small

**Endpoint**: `PUT /api/users/{id}/password`, `[Authorize(Roles = Roles.Admin)]`,
body `{ newPassword: string }`. Frontend already calls this exact shape
(`adminSetPassword` in `api.js`).

**Handler**: near-identical to `ChangePasswordCommand`
(`Sencecon.Application/Users/Commands/ChangePassword/ChangePasswordCommand.cs`)
but skip the current-password verification step (the caller is an admin acting on
someone else's account, not the account owner) — hash via the existing
`IPasswordHasher`/`PasswordHasher` (BCrypt, already wired), set `LastModified`, save.
Same self-action guard as above: block `UserId == RequestingUserId` and point the
admin at their own Profile → change-password flow instead, for consistency with
how role changes handle "acting on yourself."

**Validation**: reuse the password rule from `RegisterCommandValidator`
(`MinimumLength(8).MaximumLength(128)`).

**Same JWT caveat as above** — the frontend copy says "the user will be signed out
of all active sessions," which isn't true today for the same stateless-JWT reason.
If #1's `OnTokenValidated` check ships, it incidentally fixes this too (an
`IsActive` check doesn't help by itself, but a similar "was the password changed
after this token was issued" check could piggyback on the same middleware — worth
building both together rather than twice).

## 3. Send password reset link — large, needs a provider decision before starting

This is the biggest piece. Nothing to reuse — **no email-sending code exists
anywhere in this codebase** (verified: zero hits for `IEmailService`/`Smtp`/
similar). Needs to be built from scratch.

**New entity**: `PasswordResetToken` — `Id`, `UserId` (FK), `TokenHash` (store a
hash of the token, same reasoning as password hashing — a DB leak shouldn't hand
out usable reset links), `ExpiresAt`, `Used` (bool), `Created`.

**Endpoints**:
- `POST /api/users/{id}/password-reset` — `[Authorize(Roles = Roles.Admin)]`,
  matches what the frontend already calls (`sendPasswordReset` in `api.js`).
  Generates a token, stores its hash, emails the raw token as a link to the
  target user.
- `POST /api/auth/reset-password` — `AllowAnonymous` (new, mirrors
  `AuthController`'s existing `Register`/`Login` pattern), body
  `{ token, newPassword }`. Looks up by token hash, checks `ExpiresAt`/`Used`,
  sets new password, marks token used.

**Needs a decision before any code**: which email provider? No SMTP/API key
exists in this project today. Options: a transactional email API (SendGrid,
Resend, Mailgun, Amazon SES — pick one you already have or want an account
with) or raw SMTP relay. Whichever it is, wrap it behind an `IEmailService`
interface (same pattern as `IPasswordHasher`/`IJwtTokenGenerator` — interface in
`Sencecon.Application`, implementation in `Sencecon.Infrastructure`), with the
provider's API key as a Railway env var.

**Also touches the frontend**: the reset link needs somewhere to land. There's no
router in the SPA today (`App.jsx`'s `SCREENS` dict keyed by string, gated purely
on `isAuthenticated()`) — a `?resetToken=...` query param would need to be read
before the auth gate and route to a new "Set new password" screen (sibling to
`Login.jsx`). Scope this as part of the same piece of work, not a separate one —
the backend endpoint is useless without it.

**Recommendation**: don't start this until the email provider is picked. Everything
else in this doc can proceed independently.

## 4. Integrations settings persistence — medium, blocked on a product decision

**There is no `Tenant` entity in this backend at all** (verified: zero matches
for "Tenant" anywhere under `src/`). The tenant switcher in the frontend topbar
and the "Tenants" card on the Admin Overview tab are 100% mock data — nothing in
the backend enforces or even models tenant isolation today, on any table.

That means "integrations settings, scoped per tenant" isn't actually a small
addition — it's blocked on a real product decision:

- **Option A (pragmatic default)**: ship a single global `IntegrationSetting`
  entity (`Key`, provider fields, `Status`, `UpdatedBy`, `LastModified`) with no
  tenant scoping, matching the current reality that the tenant switcher doesn't
  change any real data anyway. CRUD endpoints under `[Authorize(Roles = Roles.Admin)]`.
  Ships integrations persistence now; if real multi-tenancy becomes a goal later,
  this table gets a `TenantId` column added same as everything else would.
- **Option B**: build a real `Tenant` entity and `TenantId` foreign keys across
  the domain first, then scope integrations to it properly. This is a much larger
  architectural change (touches every existing table, every query, every
  `[Authorize]` check) that goes well beyond "integrations settings" — it's its
  own project, not a Phase 2 line item.

**Recommendation**: Option A now; treat "real multi-tenancy" as a separate,
explicitly-scoped decision if/when it's actually needed.

**Secrets handling**: integration credentials (API keys, tokens) must not be
stored as plaintext columns. Use ASP.NET Core's Data Protection API to
encrypt-at-rest, matching the sensitivity of what's being stored (third-party
service credentials, not just app data).

**Frontend follow-up**: `AdminIntegrations.jsx` currently shows a "not yet
persisted" banner and local-only field state per integration. Once this ships,
wire it to real GET/PUT calls and drop the banner + the local-only state.

## Not in this scope: dynamic RBAC (Option B from the Phase 1 brief)

Replacing the fixed 5-role enum with DB-backed `Roles`/`RolePermissions` tables
(to support "create a new role" and per-role checkbox editing) is a separate,
larger project — it touches every `[Authorize(Roles=...)]` in this repo and every
`canAccess`/`MODULE_ACCESS` check in the frontend. Still out of scope here;
flagging so it doesn't get silently forgotten, not proposing to build it now.

## Suggested order

1. **Disable/enable** (#1) and **admin-set-password** (#2) together — same
   pattern, same self-guard, same `OnTokenValidated` piece benefits both.
2. **Integrations** (#4) — independent of the others, just needs the Option A/B
   call made.
3. **Password reset email** (#3) — largest, and blocked on picking a provider.
