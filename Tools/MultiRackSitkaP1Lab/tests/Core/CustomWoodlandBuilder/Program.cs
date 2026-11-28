using SitkaP1.Core.CustomWoodlandBuilder.Tests;

BoundaryTests.Register();
LaneTests.Register();
MoistureTests.Register();
DeerTests.Register();
MixAndStandTests.Register();
DeterminismTests.Register();

int exitCode = TestRunner.RunAll();
Console.WriteLine("DIGEST canonical-sha256 " + DeterminismTests.CanonicalDigest());
return exitCode;
