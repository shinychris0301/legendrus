# CHAPTER-SMS-01 — phone-number verification via SMS

Status: user explicitly requested and clarified on 2026-09-16; scheduled after UI design work is complete. This is real phone-number verification, not physical-device visual QA. No SMS provider, backend or verification flow has been implemented by this scheduling task.

Before implementation, define where Chapter Complete starts this flow, its purpose and whether it is optional; select the existing account/backend integration and SMS provider. Do not block chapter rewards or progression on an unspecified requirement.

Implementation stage: phone number entry with country code and consent copy; server-issued expiring OTP; code entry/resend cooldown; server-side verification, attempt/rate limits, retries/expiry and delivery failures; securely attach verified number to the intended account. Never treat a client timer, fabricated code or local boolean as verification. Configure secrets outside Unity/client assets. Define account recovery/change-number behavior with the chosen identity service.

Acceptance: provider sandbox and real-device delivery, invalid/expired/replayed code rejection, resend/attempt limits, intermittent-network handling, localization/accessibility, consent/privacy and unchanged rewards/navigation outside the explicit verification flow. Exact provider and UX decisions remain open for that stage.
