#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step20ProductionAssetStep11RegressionGate
    {
        [MenuItem("Legends of Rus/Tests/v1.20 Step 11/Validate Full Functional + Android Regression Evidence")]
        public static void Validate()
        {
            Step20ProductionAssetStep11Evidence.RcStatus status=Step20ProductionAssetStep11Evidence.EvaluateAndWrite();
            var failures=new List<string>();
            if(status.globalFailures!=0)failures.Add("Global authority/integration failures are present.");
            if(!status.functionalPass)failures.Add("Full functional regression PASS is missing/stale for the current integration fingerprint.");
            if(!status.androidPass)failures.Add("Physical Android regression PASS is missing/stale for the current integration fingerprint.");
            if(!status.finalAndroidPass)failures.Add("Final Step 10.3 Android acceptance is missing/stale for the current integration fingerprint.");
            if(failures.Count>0)throw new InvalidOperationException(string.Join(Environment.NewLine,failures.ToArray()));
            Debug.Log("v1.20 Step 11 regression evidence VALID for the current integration fingerprint. Development RC freeze may be created.");
            if(!Application.isBatchMode)EditorUtility.DisplayDialog("Legends of Rus","Full functional + physical Android regression evidence is valid for the current integration fingerprint. Development RC freeze is eligible.","OK");
        }
    }
}
#endif
