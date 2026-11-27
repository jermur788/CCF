using SitkaP1.Core.BirchAlder.Tests;

SiteAndZoneTests.Register();
BirchTests.Register();
AlderTests.Register();
TimberTests.Register();
DeterminismTests.Register();
int exitCode = TestRunner.RunAll();
Console.WriteLine("DIGEST canonical-sha256 " + DeterminismTests.CanonicalDigest());
return exitCode;
