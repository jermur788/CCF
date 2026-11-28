using SitkaP1.Core.Senescence.Tests;

VersionGateTests.Register();
HazardTests.Register();
RollsAndSemanticsTests.Register();
LateLifeGrowthTests.Register();
DeterminismTests.Register();
HygieneTests.Register();
int exitCode = TestRunner.RunAll();
Console.WriteLine("DIGEST canonical-sha256 " + DeterminismTests.CanonicalDigest());
return exitCode;
