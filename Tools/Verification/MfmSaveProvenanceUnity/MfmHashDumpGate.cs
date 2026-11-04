#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

// Prints every protected world-identity hash of the shared fixture worlds through Unity's own JsonUtility. Run on the pre-v23
// base (ac876a6) and on the candidate; the two dumps must be byte-identical (compared by the caller). HashFixtures is staged
// from Tools/Verification/MfmSaveProvenance and compiles unchanged against both schemas.
public static class MfmHashDumpGate
{
    public static void Begin()
    {
        try
        {
            string dump = HashFixtures.Dump();
            int lines = 0;
            foreach (string line in dump.Split('\n'))
                if (line.Length > 0) { Debug.Log("MFM06_HASH " + line); lines++; }
            Debug.Log("MFM06_HASHDUMP_PASS lines=" + lines + " saveVersion=" + ForestSaveData.CurrentVersion);
            EditorApplication.Exit(0);
        }
        catch (Exception error) { Debug.LogError("MFM06_HASHDUMP_FAIL " + error); EditorApplication.Exit(1); }
    }
}
#endif
