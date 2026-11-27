using System.Runtime.CompilerServices;

// The standalone Phase 2B-2b felling-dispositions test runner (tests/Core/EconomyDispositions) exercises the internal
// Economy engine directly, like SitkaP1.Core.Tests does. Declared here so SitkaP1.Core.csproj stays untouched.
[assembly: InternalsVisibleTo("SitkaP1.EconomyDispositions.Tests")]
