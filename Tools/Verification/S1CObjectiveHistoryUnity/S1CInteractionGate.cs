#if UNITY_EDITOR
// Run the unchanged existing interaction gate, including its explicitly configured canonical Sitka lifecycle assertion.
public static class S1CInteractionGate
{
    public static void Begin() => ScenarioOneInteractionVerification.Begin();
}
#endif
