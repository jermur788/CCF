#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
// executeMethod is reached only after Unity imports and successfully compiles the candidate.
public static class MfmCompileGate
{
    public static void Begin() { Debug.Log("MFM06_COMPILE_PASS Unity=" + Application.unityVersion + " saveVersion=" + ForestSaveData.CurrentVersion); EditorApplication.Exit(0); }
}
#endif
