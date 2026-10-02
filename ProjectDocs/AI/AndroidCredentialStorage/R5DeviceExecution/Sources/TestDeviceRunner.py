import json,unittest
from RunDeviceProbe import validate_report,EXPECTED,PHASE_CHECKS
class DeviceReportTests(unittest.TestCase):
    def report(self,phase='verify'):
        return {'version':1,'command':phase,'status':EXPECTED[phase],'checks':PHASE_CHECKS[phase],'authenticatedSessionTested':False}
    def test_complete_phases(self):
        for phase in EXPECTED:
            with self.subTest(phase=phase):self.assertEqual(validate_report(json.dumps(self.report(phase)),phase)['status'],EXPECTED[phase])
    def test_unknown_payload_field_is_rejected(self):
        r=self.report();r['payload']='synthetic forbidden export';self.assertRaises(ValueError,validate_report,json.dumps(r),'verify')
    def test_raw_exception_check_is_rejected(self):
        r=self.report();r['checks']=['native exception details'];self.assertRaises(ValueError,validate_report,json.dumps(r),'verify')
    def test_missing_success_checks_rejected(self):
        r=self.report();r['checks']=[];self.assertRaises(ValueError,validate_report,json.dumps(r),'verify')
    def test_duplicate_success_checks_rejected(self):
        r=self.report();r['checks']=r['checks']*2;self.assertRaises(ValueError,validate_report,json.dumps(r),'verify')
    def test_stale_phase_rejected(self):self.assertRaises(ValueError,validate_report,json.dumps(self.report('prepare')),'verify')
    def test_impossible_status_rejected(self):
        r=self.report();r['status']='awaiting_restart';self.assertRaises(ValueError,validate_report,json.dumps(r),'verify')
    def test_session_success_claim_rejected(self):
        r=self.report();r['authenticatedSessionTested']=True;self.assertRaises(ValueError,validate_report,json.dumps(r),'verify')
    def test_wrong_schema_rejected(self):
        r=self.report();r['version']=True;self.assertRaises(ValueError,validate_report,json.dumps(r),'verify')
    def test_oversize_report_rejected(self):self.assertRaises(ValueError,validate_report,' '*8193,'verify')
    def test_failure_keeps_valid_partial_checks(self):
        r=self.report();r['status']='failed_retained';r['checks']=['interrupted_write_recovery'];self.assertEqual(validate_report(json.dumps(r),'verify')['status'],'failed_retained')
    def test_refusal_is_reported_not_promoted(self):
        r=self.report();r['status']='refused';r['checks']=[];self.assertEqual(validate_report(json.dumps(r),'verify')['status'],'refused')
if __name__=='__main__':unittest.main(verbosity=2)
