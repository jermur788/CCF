#!/usr/bin/env python3
"""Execute the harness's actual F10 classifier without Unity. Build files stay in --out."""
import argparse
import os
from pathlib import Path
import subprocess


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--out", required=True, type=Path)
    args = parser.parse_args()
    source = Path(__file__).with_name("UxQualification.cs").read_text()
    classifier = source.split("public sealed class F10DeliveryEvidence\n", 1)[1]
    classifier = "public sealed class F10DeliveryEvidence\n" + classifier.split(
        "public sealed class UxQualificationRunner", 1)[0]
    # Both flow locations must use the same persistent classifier before direct fallbacks.
    for method, names in [
        ("ResetFlow", ["F10_OPENS_THE_SESSION_MENU", "MENU_CLOSES_WITH_F10"]),
        ("SessionMenuInput", ["INPUT_SESSIONMENU_OPENS", "INPUT_SESSIONMENU_CLOSES"]),
    ]:
        body = source.split("private IEnumerator " + method + "()", 1)[1].split(
            "private IEnumerator ", 1)[0]
        assert body.count("RecordF10Toggle(") == 2, method
        assert "Unverified(" not in body, method
        for name in names:
            assert 'RecordF10Toggle("' + name + '"' in body, name
    record = source.split("private void RecordF10Toggle(", 1)[1].split(
        "private ForestSaveData Clone", 1)[0]
    assert "f10Delivery.Observe(beforeOpen, afterOpen, expectedOpen)" in record
    assert 'else Hard(name, result == "PASS", detail);' in record
    assert source.count("new F10DeliveryEvidence()") == 1

    program = r'''
public static class F10SelfTest
{
    private static int checks;
    private static void Check(string expected, string actual, string name)
    {
        if (actual != expected) throw new System.Exception(name + ": " + actual);
        checks++;
        System.Console.WriteLine("F10_SELF_TEST_PASS " + name + "=" + actual);
    }
    public static void Main()
    {
        var never = new F10DeliveryEvidence();
        Check("UNVERIFIED", never.Observe(false, false, true), "never-delivered-open");
        Check("UNVERIFIED", never.Observe(true, true, false), "direct-open-is-not-delivery");
        Check("UNVERIFIED", never.Observe(false, false, false), "already-closed-is-not-delivery");
        var both = new F10DeliveryEvidence();
        Check("PASS", both.Observe(false, true, true), "open-delivered");
        Check("PASS", both.Observe(true, false, false), "close-delivered");
        var partial = new F10DeliveryEvidence();
        Check("PASS", partial.Observe(false, true, true), "partial-open-delivered");
        Check("FAIL", partial.Observe(true, true, false), "later-close-delivery-fails");
        Check("FAIL", partial.Observe(false, false, true), "delivery-history-persists-across-flows");
        Check("PASS", partial.Observe(false, true, true), "success-after-failure");
        var late = new F10DeliveryEvidence();
        Check("UNVERIFIED", late.Observe(false, false, true), "late-delivery-initial-miss");
        Check("PASS", late.Observe(true, false, false), "close-first-demonstrates-delivery");
        Check("FAIL", late.Observe(false, false, true), "later-open-fails");
        var wrong = new F10DeliveryEvidence();
        Check("FAIL", wrong.Observe(false, true, false), "wrong-transition-proves-delivery");
        System.Console.WriteLine("F10_PARTIAL_DELIVERY_SELF_TEST_PASS checks=" + checks);
    }
}
'''
    out = args.out.resolve()
    out.mkdir(parents=True, exist_ok=True)
    (out / "F10SelfTest.cs").write_text(classifier + program)
    (out / "f10.csproj").write_text('''<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType><TargetFramework>net8.0</TargetFramework>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
  </PropertyGroup>
  <ItemGroup><Compile Include="F10SelfTest.cs" /></ItemGroup>
</Project>
''')
    env = dict(os.environ, DOTNET_CLI_TELEMETRY_OPTOUT="1", DOTNET_NOLOGO="1")
    subprocess.run(["dotnet", "run", "--project", str(out / "f10.csproj"), "-c", "Release"],
                   env=env, check=True)
    print("F10_FLOW_WIRING_PASS RESET_FLOW SessionMenuInput", flush=True)


if __name__ == "__main__":
    main()
