#if UNITY_EDITOR
using System;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace LegendsOfRus.Editor
{
    /// <summary>Wave 4 visual report. Settings demo guidance is informational; there are 0/7 direct approved references.</summary>
    public static class Step20ProductionAssetWave04Step102VisualAcceptance
    {
        const string DemoSettings="Assets/LegendsOfRus/Resources/DesignScreens/Settings.png";
        const string Result="ProjectDocs/V1_20_STEP10_2/RuntimeEvidence/WAVE04_RUNTIME_VISUAL_REPORT.json";
        [MenuItem("Legends of Rus/Tests/Analyze v1.20 Step 10.2 Wave 4 Runtime Visual Acceptance")]
        public static void Analyze(){string captureRoot=Step20ProductionAssetWave04Step102RuntimeCapture.AbsoluteOutputRoot();string executed=Path.Combine(captureRoot,"UNITY_RUNTIME_CAPTURE_EXECUTED.json");if(!File.Exists(executed)||File.ReadAllText(executed).IndexOf("\"captures\": 28",StringComparison.Ordinal)<0)throw new InvalidOperationException("Run the 28/28 Step 10.2 Unity runtime capture first.");Metrics m=ComparePage(ScreenId.Settings,DemoSettings,captureRoot);string json="{\n  \"schemaVersion\": 1,\n  \"milestone\": \"v1.20-step10.2-wave04-runtime-visual-report\",\n  \"directApprovedReferencePages\": 0,\n  \"demoDesignGuidancePages\": 1,\n  \"settingsDemoCorrelation\": "+m.correlation.ToString("F4",CultureInfo.InvariantCulture)+",\n  \"settingsDemoMae\": "+m.mae.ToString("F4",CultureInfo.InvariantCulture)+",\n  \"demoMetricsAreGating\": false,\n  \"decision\": \"NO_GO_WAVE04_CUTOVER_REFERENCE_AUTHORITY_MISSING\",\n  \"activeWave04CutoverPerformed\": false\n}\n";string abs=Absolute(Result);Directory.CreateDirectory(Path.GetDirectoryName(abs));File.WriteAllText(abs,json);AssetDatabase.Refresh();Debug.Log("v1.20 Step 10.2 Wave 4 visual report: Settings demo-guidance correlation="+m.correlation.ToString("F4")+", MAE="+m.mae.ToString("F4")+". Metrics are informational; direct approved authority remains 0/7.");}
        static Metrics ComparePage(ScreenId id,string referenceProjectPath,string captureRoot){string capturePath=Path.Combine(captureRoot,id+"_941x1672.png");string referencePath=Absolute(referenceProjectPath);if(!File.Exists(capturePath))throw new InvalidOperationException("Missing runtime capture: "+capturePath);if(!File.Exists(referencePath))throw new InvalidOperationException("Missing demo guidance: "+referenceProjectPath);Texture2D a=Load(capturePath),b=Load(referencePath);try{return Compare(a,b);}finally{UnityEngine.Object.DestroyImmediate(a);UnityEngine.Object.DestroyImmediate(b);}}
        static Texture2D Load(string p){Texture2D t=new Texture2D(2,2,TextureFormat.RGBA32,false);if(!t.LoadImage(File.ReadAllBytes(p),false))throw new InvalidOperationException("Could not decode PNG: "+p);return t;}
        struct Metrics{public float correlation,mae;}static Metrics Compare(Texture2D a,Texture2D b){if(a.width!=b.width||a.height!=b.height)throw new InvalidOperationException("Runtime/reference size mismatch.");Color32[] pa=a.GetPixels32(),pb=b.GetPixels32();int w=a.width,h=a.height;const int stride=4;double sa=0,sb=0,saa=0,sbb=0,sab=0,mae=0;long n=0;for(int ty=0;ty<h;ty+=stride){int py=h-1-ty,row=py*w;for(int x=0;x<w;x+=stride){Color32 ca=pa[row+x],cb=pb[row+x];double la=(.2126*ca.r+.7152*ca.g+.0722*ca.b)/255d,lb=(.2126*cb.r+.7152*cb.g+.0722*cb.b)/255d;sa+=la;sb+=lb;saa+=la*la;sbb+=lb*lb;sab+=la*lb;mae+=(Math.Abs(ca.r-cb.r)+Math.Abs(ca.g-cb.g)+Math.Abs(ca.b-cb.b))/(3d*255d);n++;}}double nn=n,cov=sab-(sa*sb/nn),va=saa-(sa*sa/nn),vb=sbb-(sb*sb/nn),den=Math.Sqrt(Math.Max(0,va)*Math.Max(0,vb));return new Metrics{correlation=(float)(den>1e-12?cov/den:0),mae=(float)(mae/nn)};}
        static string Absolute(string p){return Path.Combine(Directory.GetCurrentDirectory(),p.Replace('/',Path.DirectorySeparatorChar));}
    }
}
#endif
