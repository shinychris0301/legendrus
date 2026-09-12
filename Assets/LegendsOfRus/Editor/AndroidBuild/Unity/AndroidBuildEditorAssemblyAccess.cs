using System.Runtime.CompilerServices;

#if UNITY_EDITOR && !UNITY_6000_0_OR_NEWER
#error v1.17 Step 1 requires Unity 6000.5.9f1; the Android build boundary is fail-closed.
#endif

[assembly: InternalsVisibleTo("LegendsOfRus.AndroidBuild.Editor.Tests")]
[assembly: InternalsVisibleTo("Assembly-CSharp-Editor")]
