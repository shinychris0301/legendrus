# AUTH-ACCOUNT-01 — Account services after UI

Status: integration follow-up identified during approved Visual Page30 UI work; not implemented or activated. No provider/backend choice or production credentials requested for this design pass.

Before implementation, define supported identity providers and email/phone authentication, approved backend, account-link/merge rules, recovery, sign-out/deletion, legal destinations and consent policy. Reconcile the separate CHAPTER-SMS-01 trigger with the chosen identity flow; do not assume Chapter Complete verification creates or links an account automatically.

Implement real authentication and cancellable provider flows, secure platform session storage, local-to-cloud progress reconciliation with explicit conflict handling, recovery and online/offline states. Preserve guest progress and device language preference. Keep existing compliance activation guards until their dependencies are satisfied.

Acceptance requires configured sandbox/provider tests, restart/revocation/recovery behavior, no credential logging, safe cancellation and merge regression coverage, followed by actual Android keyboard/provider/deep-link verification. Login R1 currently sends/stores no credentials, grants no authenticated identity, and records no legal acceptance.
