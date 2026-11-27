using SitkaP1.Core.ScotsPine.Tests;

SiteMoistureTests.Register();
ScotsPineBiologyTests.Register();
ScotsPineTimberTests.Register();
DeterminismTests.Register();
int exitCode = TestRunner.RunAll();
Console.WriteLine("DIGEST canonical-sha256 " + DeterminismTests.CanonicalDigest());
return exitCode;
