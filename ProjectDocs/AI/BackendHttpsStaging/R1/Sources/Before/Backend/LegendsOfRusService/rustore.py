"""RuStore Pay invoice lookup. No client callback is accepted as payment proof."""
import json
import re
import urllib.error
import urllib.request


class VerificationError(Exception):
    pass


def invoice_id(value):
    value = str(value)
    if not re.fullmatch(r"[1-9][0-9]{0,19}", value):
        raise VerificationError("invalid_invoice")
    return value


class NoRedirect(urllib.request.HTTPRedirectHandler):
    def redirect_request(self, req, fp, code, msg, headers, newurl):
        raise VerificationError("provider_redirect_rejected")


def fetch_json(url, token):
    request = urllib.request.Request(url, headers={"Public-Token": token, "Accept": "application/json"})
    try:
        with urllib.request.build_opener(NoRedirect()).open(request, timeout=5) as response:
            raw = response.read(262145)
        if len(raw) > 262144:
            raise VerificationError("provider_response_too_large")
        return json.loads(raw)
    except (OSError, ValueError, urllib.error.HTTPError) as error:
        # Never echo upstream bodies, payment data or the token into responses/logs.
        raise VerificationError("provider_unavailable") from None


class RuStoreVerifier:
    def __init__(self, app_id, environment, token_supplier, transport=fetch_json):
        if environment not in ("sandbox", "production"):
            raise ValueError("Explicit RuStore environment required")
        self.app_id = str(app_id)
        self.environment = environment
        self.token_supplier = token_supplier
        self.transport = transport

    def verify(self, invoice, order):
        invoice = invoice_id(invoice)
        token = self.token_supplier()
        if not token or not re.fullmatch(r"[1-9][0-9]*", self.app_id):
            raise VerificationError("rustore_not_configured")
        path = "/public/sandbox/v2/invoices/" if self.environment == "sandbox" else "/public/v2/invoices/"
        result = self.transport("https://public-api.rustore.ru" + path + invoice, token)
        if not isinstance(result, dict) or result.get("code") != "OK":
            raise VerificationError("invoice_unverified")
        body = result.get("body")
        if not isinstance(body, dict):
            raise VerificationError("invalid_provider_response")
        # The official v2 example nests order inside paymentInfo. Accept the documented
        # top-level representation too, but reject contradictory duplicate objects.
        payment = body.get("paymentInfo") or {}
        if not isinstance(payment, dict):
            raise VerificationError("invalid_provider_response")
        details = body.get("order") or payment.get("order")
        if body.get("order") and payment.get("order") and body["order"] != payment["order"]:
            raise VerificationError("ambiguous_order")
        if not isinstance(details, dict):
            raise VerificationError("invalid_order")
        valid = (str(body.get("invoiceId")) == invoice and str(body.get("appId")) == self.app_id
                 and str(body.get("invoiceStatus", "")).upper() == "CONFIRMED"
                 and body.get("appUserId") == order["player_id"]
                 and details.get("orderId") == order["id"]
                 and details.get("itemCode") == order["product_id"]
                 and details.get("currency") == "RUB"
                 and type(details.get("amountCreate")) is int
                 and details["amountCreate"] == order["price_minor"])
        if not valid:
            raise VerificationError("invoice_mismatch_or_unconfirmed")
        acknowledged = body.get("acknowledgementState")
        if acknowledged not in ("PENDING", "ACKNOWLEDGED"):
            raise VerificationError("unsupported_acknowledgement")
        return {"invoice_id": invoice, "environment": self.environment, "acknowledged": acknowledged == "ACKNOWLEDGED"}
