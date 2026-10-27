// Entry point of the non-UI remediation offline checks (run_nonui_offline.py). Each check class prints its own PASS/FAIL token.
public static class NonUiOfflineMain
{
    public static int Main(string[] args)
    {
        string root = args.Length > 0 ? args[0] : ".";
        int result = 0;
        result |= CompetitorKeyboardOfflineCheck.Run();
        result |= RackBayOfflineCheck.Run(root);
        result |= RackSampleGeometryOfflineCheck.Run(root);
        result |= StandApproachOfflineCheck.Run(root);
        return result;
    }
}
