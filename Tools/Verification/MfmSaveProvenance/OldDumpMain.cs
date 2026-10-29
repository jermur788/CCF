// Entry point of the BASELINE build only (ac876a6 sources, save v22): prints the fixture hashes the v23 build must reproduce.
public static class OldDumpMain
{
    public static int Main(string[] args)
    {
        System.IO.File.WriteAllText(args[0], HashFixtures.Dump());
        System.Console.WriteLine("BASELINE_DUMP_WRITTEN version=" + ForestSaveData.CurrentVersion);
        return 0;
    }
}
