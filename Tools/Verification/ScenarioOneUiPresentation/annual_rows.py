#!/usr/bin/env python3
"""#378 annual-value composition regression; source/consumer evidence, not pixel fit."""
import argparse
from pathlib import Path
import subprocess
import sys
sys.dont_write_bytecode = True
from run_checks import member


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--src', type=Path, required=True)
    parser.add_argument('--out', type=Path, required=True)
    args = parser.parse_args()
    args.out.mkdir(parents=True, exist_ok=True)
    source = (args.src / 'Assets/ForestPrototype/UI/AnnualReviewView.cs').read_text()
    styles = (args.src / 'Assets/ForestPrototype/UI/Resources/ScenarioOneUi.uss').read_text()
    for selector, rules in {
        '.annual-value-entry': ['flex-direction: column', 'flex-shrink: 0'],
        '.annual-value-name': ['align-self: stretch', 'white-space: normal'],
        '.annual-value-amount': ['align-self: stretch', 'white-space: nowrap', '-unity-text-align: middle-right'],
    }.items():
        block = styles[styles.index(selector + ' {'):].split('}', 1)[0]
        assert all(rule in block for rule in rules), selector
    print('PASS annual-scoped column/name/amount style contracts (not render proof)')
    method = member(source, 'private static void TaggedLine(')
    code = '''using System;using System.Collections.Generic;
enum HistoryProvenance {Recorded,Derived}
class VisualElement {public string text="";public string[] classes;public List<VisualElement> children=new List<VisualElement>();public void Add(VisualElement e)=>children.Add(e);}
static class UiKit {public static VisualElement Box(params string[] c)=>new VisualElement{classes=c};public static void Add(VisualElement p,string t,params string[] c)=>p.Add(new VisualElement{text=t,classes=c});}
class Probe {
static void Tag(VisualElement parent,HistoryProvenance kind)=>UiKit.Add(parent,kind.ToString(),"prov");
/*METHOD*/
static void Need(bool ok,string label){if(!ok)throw new Exception("FAIL "+label);}
static void Main(){int cases=0;foreach(var kind in new[]{HistoryProvenance.Recorded,HistoryProvenance.Derived})foreach(var name in new[]{"Timber income","Contractor work","Recorded work materials","Income less contractor work","Long recorded annual timber assortment name with source uncertainty"})foreach(var value in new[]{"−€2496.39","+€123456789.01","€0.00"}){
var parent=UiKit.Box();TaggedLine(parent,kind,name,value,"money-negative");Need(parent.children.Count==1,"ONE_ENTRY");var entry=parent.children[0];Need(Array.IndexOf(entry.classes,"annual-value-entry")>=0&&entry.children.Count==3,"INDEPENDENT_SOURCE_NAME_AMOUNT");Need(entry.children[0].children[0].text==kind.ToString(),"PROVENANCE_PRESERVED");Need(entry.children[1].text==name&&Array.IndexOf(entry.children[1].classes,"annual-value-name")>=0,"FULL_LABEL_PRESERVED");Need(entry.children[2].text==value&&Array.IndexOf(entry.children[2].classes,"annual-value-amount")>=0&&Array.IndexOf(entry.children[2].classes,"money-negative")>=0,"FULL_SIGNED_VALUE_PRESERVED");cases++;}Console.WriteLine("PASS exact TaggedLine consumer "+cases+" cases");}}
'''.replace('/*METHOD*/', method)

    def run(name, content):
        out = args.out / name
        out.mkdir(exist_ok=True)
        (out / 'Probe.cs').write_text(content)
        (out / 'probe.csproj').write_text('<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net8.0</TargetFramework></PropertyGroup></Project>')
        result = subprocess.run(['dotnet', 'run', '--project', str(out / 'probe.csproj'), '-v', 'q'], capture_output=True, text=True)
        (out / 'result.log').write_text(result.stdout + result.stderr)
        return result

    result = run('positive', code)
    assert result.returncode == 0, result.stdout + result.stderr
    print(result.stdout.strip())
    # Reintroduce the actual old shape: provenance beside a nested name/value row.
    bad = method[:method.index('{') + 1] + '''
        var entry = UiKit.Box("row"); Tag(entry, kind);
        var line = UiKit.Box("table-row"); UiKit.Add(line, name, "body");
        UiKit.Add(line, value, valueClass); entry.Add(line); parent.Add(entry);
    }'''
    result = run('old-width-competition', code.replace(method, bad))
    assert result.returncode != 0 and 'FAIL INDEPENDENT_SOURCE_NAME_AMOUNT' in result.stderr and 'error CS' not in result.stdout
    print('KILLED compiled old-width-competition by INDEPENDENT_SOURCE_NAME_AMOUNT')


if __name__ == '__main__':
    main()
