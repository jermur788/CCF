#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
// executeMethod is reached only after Unity imports and successfully compiles the candidate.
public static class S1CCompileGate
{
    public static void Begin() { Debug.Log("S1C_T05_COMPILE_PASS Unity=" + Application.unityVersion); EditorApplication.Exit(0); }
}
#endif
