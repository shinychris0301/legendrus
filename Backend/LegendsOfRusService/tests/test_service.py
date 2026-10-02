import concurrent.futures
import copy
import io
import json
from pathlib import Path
import sqlite3
import tempfile
import unittest
import uuid
from service import ApiError, Application, Product, Service
from rustore import RuStoreVerifier, VerificationError


class BackendTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.database = Path(self.temp.name) / "test.db"
        self.clock = [1000]
        self.response = None
        self.calls = []
        def transport(url, token):
            self.calls.append((url, token))
            return copy.deepcopy(self.response)
        self.verifier = RuStoreVerifier("123", "sandbox", lambda: "synthetic-test-token", transport)
        self.service = Service(self.database, self.verifier, [Product("fixture.gems", 9900, 50)], lambda: self.clock[0])
        self.app = Application(self.service)
        self.session = self.service.guest()
        self.player = self.session["player_id"]
        self.order = self.service.order(self.player, "fixture.gems", str(uuid.uuid4()))
        self.response = self.proof()

    def tearDown(self):
        self.temp.cleanup()

    def proof(self, order=None, invoice="987"):
        order = order or self.order
        return {"code": "OK", "body": {"invoiceId": int(invoice), "appId": 123,
            "invoiceStatus": "CONFIRMED", "appUserId": order["player_id"], "acknowledgementState": "PENDING",
            "paymentInfo": {"order": {"orderId": order["id"], "itemCode": order["product_id"], "currency": "RUB", "amountCreate": order["price_minor"]}}}}

    def call(self, method, path, data=None, token=True, raw=None):
        body = raw if raw is not None else json.dumps(data or {}).encode()
        env = {"REQUEST_METHOD": method, "PATH_INFO": path, "CONTENT_LENGTH": str(len(body)), "wsgi.input": io.BytesIO(body)}
        if token:
            env["HTTP_AUTHORIZATION"] = "Bearer " + (self.session["access_token"] if token is True else token)
        status = []
        result = b"".join(self.app(env, lambda value, headers: status.append(int(value.split()[0]))))
        return status[0], json.loads(result)

    def fulfill(self):
        return self.call("POST", "/v1/rustore/fulfill", {"order_id": self.order["id"], "invoice_id": "987"})

    def test_guest_session_persists_and_only_hash_is_stored(self):
        reopened = Service(self.database, self.verifier, clock=lambda:self.clock[0])
        self.assertEqual(self.player, reopened.authenticate(self.session["access_token"]))
        with reopened.connect() as db:
            self.assertNotEqual(self.session["access_token"], db.execute("SELECT token_hash FROM sessions").fetchone()[0])
        self.assertEqual(0, reopened.player(self.player)["gems"])

    def test_authentication_expiry_and_revocation(self):
        self.assertEqual(401, self.call("GET", "/v1/player", token=False)[0])
        self.assertEqual(401, self.call("GET", "/v1/player", token="x" * 43)[0])
        self.clock[0] += 86401
        self.assertEqual(401, self.call("GET", "/v1/player")[0])
        self.clock[0] = 1000
        self.assertEqual(200, self.call("DELETE", "/v1/session")[0])
        self.assertEqual(401, self.call("GET", "/v1/player")[0])

    def test_order_retries_are_idempotent_and_client_cannot_set_price(self):
        repeated = self.service.order(self.player, "fixture.gems", self.order["request_id"])
        self.assertEqual(repeated, self.order)
        self.assertEqual(400, self.call("POST", "/v1/rustore/orders", {"product_id": "fixture.gems", "request_id": str(uuid.uuid4()), "gems": "999"})[0])
        with self.assertRaises(ApiError):
            self.service.order(self.player, "different", self.order["request_id"])

    def test_empty_catalog_and_continuation_remain_unavailable(self):
        self.service.products = {}
        self.assertEqual(503, self.call("POST", "/v1/rustore/orders", {"product_id": "x", "request_id": str(uuid.uuid4())})[0])
        self.assertEqual((503, {"error": "round_authority_unavailable"}), self.call("POST", "/v1/continuations"))

    def test_confirmed_invoice_credits_once_and_uses_official_sandbox_endpoint(self):
        self.assertEqual(200, self.fulfill()[0])
        self.assertEqual(50, self.service.player(self.player)["gems"])
        self.assertTrue(self.fulfill()[1]["duplicate"])
        self.assertEqual(50, self.service.player(self.player)["gems"])
        self.assertEqual("https://public-api.rustore.ru/public/sandbox/v2/invoices/987", self.calls[0][0])

    def test_unconfirmed_refunded_and_other_terminal_statuses_do_not_grant(self):
        for status in ["CREATED", "PAID", "CANCELLED", "REFUNDED", "REFUNDING", "REVERSED", "REJECTED", "EXPIRED"]:
            with self.subTest(status=status):
                self.response = self.proof();self.response["body"]["invoiceStatus"] = status
                self.assertEqual(503, self.fulfill()[0]);self.assertEqual(0, self.service.player(self.player)["gems"])

    def test_invoice_identity_and_order_fields_must_match(self):
        cases = [("invoiceId", 1), ("appId", 999), ("appUserId", "someone-else"), ("acknowledgementState", "UNKNOWN")]
        for key, value in cases:
            with self.subTest(key=key):
                self.response=self.proof();self.response["body"][key]=value;self.assertEqual(503,self.fulfill()[0])
        for key, value in [("orderId", "wrong"), ("itemCode", "other"), ("currency", "USD"), ("amountCreate", 1), ("amountCreate", True)]:
            with self.subTest(key=key,value=value):
                self.response=self.proof();self.response["body"]["paymentInfo"]["order"][key]=value;self.assertEqual(503,self.fulfill()[0])
        self.assertEqual(0,self.service.player(self.player)["gems"])

    def test_ambiguous_or_missing_order_and_provider_error_are_rejected(self):
        for response in [None, {}, {"code":"ERROR","message":"DO_NOT_ECHO_PROVIDER_DATA"}, {"code":"OK","body":[]}]:
            self.response=response;status,body=self.fulfill();self.assertEqual(503,status);self.assertNotIn("DO_NOT_ECHO",str(body))
        self.response=self.proof();self.response["body"]["order"]={"orderId":"different"};self.assertEqual(503,self.fulfill()[0])

    def test_acknowledged_without_ledger_is_rejected_but_existing_grant_can_be_retried(self):
        self.response["body"]["acknowledgementState"]="ACKNOWLEDGED";self.assertEqual(409,self.fulfill()[0])
        self.response=self.proof();self.assertEqual(200,self.fulfill()[0]);self.response["body"]["acknowledgementState"]="ACKNOWLEDGED"
        self.assertEqual({"granted":True,"duplicate":True,"acknowledgement_required":False},self.fulfill()[1])

    def test_another_player_cannot_submit_this_order(self):
        other=self.service.guest()
        result=self.call("POST","/v1/rustore/fulfill",{"order_id":self.order["id"],"invoice_id":"987"},token=other["access_token"])
        self.assertEqual(404,result[0]);self.assertEqual(0,self.service.player(other["player_id"])["gems"])

    def test_second_invoice_cannot_fulfill_same_order(self):
        self.assertEqual(200,self.fulfill()[0]);self.response=self.proof(invoice="988")
        result=self.call("POST","/v1/rustore/fulfill",{"order_id":self.order["id"],"invoice_id":"988"})
        self.assertEqual(409,result[0]);self.assertEqual(50,self.service.player(self.player)["gems"])

    def test_parallel_duplicate_receipts_credit_only_once(self):
        with concurrent.futures.ThreadPoolExecutor(max_workers=8) as pool:
            results=list(pool.map(lambda _:self.service.fulfill(self.player,self.order["id"],"987"),range(16)))
        self.assertEqual(1,sum(not r["duplicate"] for r in results));self.assertEqual(50,self.service.player(self.player)["gems"])

    def test_storage_failure_rolls_back_grant_and_wallet_then_retry_succeeds(self):
        with self.service.connect() as db:
            db.execute("CREATE TRIGGER inject_failure BEFORE UPDATE ON players BEGIN SELECT RAISE(ABORT,'test failure'); END;")
        self.assertEqual(503,self.fulfill()[0]);self.assertEqual(0,self.service.player(self.player)["gems"])
        with self.service.connect() as db:
            self.assertEqual(0,db.execute("SELECT count(*) FROM grants").fetchone()[0]);db.execute("DROP TRIGGER inject_failure")
        self.assertEqual(200,self.fulfill()[0]);self.assertEqual(50,self.service.player(self.player)["gems"])

    def test_restart_preserves_receipt_idempotency(self):
        self.assertEqual(200,self.fulfill()[0]);self.service=Service(self.database,self.verifier,clock=lambda:self.clock[0]);self.app=Application(self.service)
        self.assertEqual(self.player,self.service.authenticate(self.session["access_token"]));self.assertTrue(self.fulfill()[1]["duplicate"])
        self.assertEqual(50,self.service.player(self.player)["gems"])

    def test_application_is_bound_to_order_before_provider_lookup(self):
        self.verifier.app_id = "456"
        self.assertEqual((409, {"error": "app_mismatch"}), self.fulfill())
        self.assertEqual([], self.calls)
        self.assertEqual(0, self.service.player(self.player)["gems"])

    def test_environment_is_bound_to_order_and_not_client(self):
        self.verifier.environment="production"
        self.assertEqual(409,self.fulfill()[0])
        with self.assertRaises(ValueError):RuStoreVerifier("123","anything",lambda:"")

    def test_malformed_requests_and_invoice_paths_are_rejected(self):
        self.assertEqual(400,self.call("POST","/v1/auth/guest",raw=b'not json',token=False)[0])
        self.assertEqual(413,self.call("POST","/v1/auth/guest",raw=b'x'*16385,token=False)[0])
        self.assertEqual(400,self.call("POST","/v1/rustore/fulfill",{"order_id":self.order["id"],"invoice_id":"../auth"})[0])
        self.assertEqual(400,self.call("POST","/v1/rustore/orders",{"product_id":"fixture.gems","request_id":"invalid"})[0])

    def test_missing_server_credentials_do_not_use_fake_verification(self):
        self.verifier.token_supplier=lambda:""
        self.assertEqual(503,self.fulfill()[0]);self.assertEqual([],self.calls)

    def test_existing_order_terms_are_not_repriced_by_catalog_change(self):
        self.service.products={"fixture.gems":Product("fixture.gems",19900,100)}
        self.assertEqual(200,self.fulfill()[0]);self.assertEqual(50,self.service.player(self.player)["gems"])

    def test_wallet_overflow_does_not_record_entitlement(self):
        with self.service.connect() as db:db.execute("UPDATE players SET gems=2147483647 WHERE id=?",(self.player,))
        self.assertEqual(409,self.fulfill()[0])
        with self.service.connect() as db:self.assertEqual(0,db.execute("SELECT count(*) FROM grants").fetchone()[0])

    def test_unknown_database_schema_is_not_downgraded(self):
        with self.service.connect() as db:db.execute("PRAGMA user_version=99")
        with self.assertRaises(ValueError):Service(self.database,self.verifier)
        with self.service.connect() as db:self.assertEqual(99,db.execute("PRAGMA user_version").fetchone()[0])

    def test_loopback_http_guest_and_authenticated_wallet(self):
        from wsgiref.simple_server import make_server
        from run_local import QuietHandler
        from threading import Thread
        from urllib.request import Request, urlopen
        server=make_server("127.0.0.1",0,self.app,handler_class=QuietHandler)
        thread=Thread(target=server.serve_forever,daemon=True);thread.start()
        base="http://127.0.0.1:"+str(server.server_port)
        try:
            with urlopen(base+"/health",timeout=2) as response:self.assertEqual("rustore",json.load(response)["store"])
            with urlopen(Request(base+"/v1/auth/guest",data=b"{}",headers={"Content-Type":"application/json"}),timeout=2) as response:session=json.load(response)
            with urlopen(Request(base+"/v1/player",headers={"Authorization":"Bearer "+session["access_token"]}),timeout=2) as response:wallet=json.load(response)
            self.assertEqual(session["player_id"],wallet["id"]);self.assertEqual(0,wallet["gems"])
        finally:
            server.shutdown();thread.join(timeout=3);server.server_close()

if __name__ == "__main__":unittest.main()
