// Objective CTA Roslyn symbol reporter (#328). Source/offline approximation only: NOT Unity compilation.
//
// This tool does not decide anything. It compiles the player-like C# source set (every Assets/**/*.cs outside
// Editor folders) at C# 9 against an explicit, hash-verified reference list, then REPORTS what Roslyn bound:
//   * the compile errors (a source set that does not compile is never accepted by the Python rules),
//   * every type named ScenarioOneUiRoot with its declaring files and resolved base type, and
//   * every member access in the root's declaring files whose receiver is spelled `Time` or `DisplayStyle`,
//     with the semantic symbol of the receiver and of the member, and any identifier use outside such a receiver.
// objective_cta_roslyn_binding.py owns the pass/fail rules and the build/run plumbing.
//
// Usage: dotnet ObjectiveCtaRoslynBinding.dll <sourceRoot> <referenceListFile> <definesSemicolonList> <outJson> [overlayRoot]
// An overlay root holds one sub-folder per case; the files inside replace or add C# files at the same path
// relative to <sourceRoot>. Each case is reported next to the unmodified BASELINE.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

static class ObjectiveCtaRoslynBinding
{
    const string RootTypeName = "ScenarioOneUiRoot";
    static readonly HashSet<string> ReviewedReceivers = new HashSet<string> { "Time", "DisplayStyle" };

    static int Main(string[] args)
    {
        if (args.Length < 4)
        {
            Console.Error.WriteLine("usage: <sourceRoot> <referenceListFile> <defines> <outJson> [overlayRoot]");
            return 3;
        }
        try
        {
            string sourceRoot = args[0], referenceList = args[1], defines = args[2], outFile = args[3];
            string overlayRoot = args.Length > 4 ? args[4] : null;
            var parse = new CSharpParseOptions(LanguageVersion.CSharp9, DocumentationMode.None, SourceCodeKind.Regular,
                defines.Split(';', StringSplitOptions.RemoveEmptyEntries));
            var references = File.ReadAllLines(referenceList).Where(l => l.Length > 0)
                .Select(l => (MetadataReference)MetadataReference.CreateFromFile(l)).ToList();

            // path (relative to sourceRoot, '/' separators) -> text
            var baseline = new SortedDictionary<string, string>(StringComparer.Ordinal);
            foreach (string path in EnumerateSources(sourceRoot))
                baseline[Rel(sourceRoot, path)] = File.ReadAllText(path);

            var results = new List<object> { Report("BASELINE", baseline, references, parse) };
            if (overlayRoot != null)
            {
                foreach (string dir in Directory.GetDirectories(overlayRoot).OrderBy(d => d, StringComparer.Ordinal))
                {
                    var files = new SortedDictionary<string, string>(baseline, StringComparer.Ordinal);
                    foreach (string file in Directory.EnumerateFiles(dir, "*.cs", SearchOption.AllDirectories))
                        files[Rel(dir, file)] = File.ReadAllText(file);
                    results.Add(Report(Path.GetFileName(dir), files, references, parse));
                }
            }

            var document = new
            {
                tool = "ObjectiveCtaRoslynBinding",
                roslynAssembly = typeof(CSharpCompilation).Assembly.GetName().ToString(),
                runtime = Environment.Version.ToString(),
                language = "CSharp9",
                referenceCount = references.Count,
                results
            };
            File.WriteAllText(outFile, JsonSerializer.Serialize(document, new JsonSerializerOptions { WriteIndented = true }));
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine("TOOL-ERROR " + error.GetType().Name + ": " + error.Message);
            return 3;
        }
    }

    static string Rel(string root, string path) => Path.GetRelativePath(root, path).Replace('\\', '/');

    // Unity compiles Assets/**/*.cs but ignores Editor folders for the player assembly and
    // any folder or file whose name starts with '.' or ends with '~'.
    static IEnumerable<string> EnumerateSources(string sourceRoot)
    {
        string assets = Path.Combine(sourceRoot, "Assets");
        if (!Directory.Exists(assets)) throw new DirectoryNotFoundException("no Assets folder under " + sourceRoot);
        return Directory.EnumerateFiles(assets, "*.cs", SearchOption.AllDirectories)
            .Where(p =>
            {
                string[] parts = Rel(sourceRoot, p).Split('/');
                return !parts.Take(parts.Length - 1).Any(s => s == "Editor")
                    && !parts.Any(s => s.StartsWith(".") || s.EndsWith("~"));
            })
            .OrderBy(p => p, StringComparer.Ordinal);
    }

    static object Report(string label, IDictionary<string, string> files, List<MetadataReference> references, CSharpParseOptions parse)
    {
        var trees = new Dictionary<string, SyntaxTree>(StringComparer.Ordinal);
        var identity = new StringBuilder();
        foreach (var item in files)
        {
            trees[item.Key] = CSharpSyntaxTree.ParseText(item.Value, parse, path: item.Key, encoding: Encoding.UTF8);
            identity.Append(item.Key).Append('\t')
                .Append(Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(item.Value))).ToLowerInvariant()).Append('\n');
        }
        string sourceSetSha256 = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(identity.ToString()))).ToLowerInvariant();

        var compilation = CSharpCompilation.Create("Assembly-CSharp", trees.Values, references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true));
        var errors = compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToList();

        var rootTypes = compilation.GetSymbolsWithName(n => n == RootTypeName, SymbolFilter.Type)
            .OfType<INamedTypeSymbol>().OrderBy(t => t.ToDisplayString(), StringComparer.Ordinal).ToList();
        var siteFiles = rootTypes.SelectMany(t => t.DeclaringSyntaxReferences).Select(r => r.SyntaxTree.FilePath)
            .Distinct(StringComparer.Ordinal).OrderBy(p => p, StringComparer.Ordinal).ToList();

        var sites = new List<object>();
        var others = new List<object>();
        foreach (string file in siteFiles)
        {
            SyntaxTree tree = trees[file];
            SemanticModel model = compilation.GetSemanticModel(tree);
            SyntaxNode treeRoot = tree.GetRoot();
            foreach (var access in treeRoot.DescendantNodes().OfType<MemberAccessExpressionSyntax>()
                         .Where(m => m.IsKind(SyntaxKind.SimpleMemberAccessExpression)))
            {
                string receiverName = RightmostName(access.Expression);
                if (receiverName == null || !ReviewedReceivers.Contains(receiverName)) continue;
                sites.Add(new
                {
                    file,
                    line = access.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                    text = access.ToString(),
                    receiverName,
                    receiver = Describe(model.GetSymbolInfo(access.Expression)),
                    receiverAlias = model.GetAliasInfo(RightmostIdentifier(access.Expression))?.Name,
                    member = access.Name.Identifier.ValueText,
                    memberSymbol = Describe(model.GetSymbolInfo(access))
                });
            }
            foreach (var id in treeRoot.DescendantNodes().OfType<IdentifierNameSyntax>()
                         .Where(n => ReviewedReceivers.Contains(n.Identifier.ValueText)))
            {
                if (IsCountedReceiver(id)) continue;
                // A using directive names a type or alias; what it binds is judged where the name is used.
                if (id.Ancestors().OfType<UsingDirectiveSyntax>().Any()) continue;
                others.Add(new
                {
                    file,
                    line = id.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                    name = id.Identifier.ValueText,
                    parentKind = id.Parent?.Kind().ToString(),
                    text = id.Parent?.ToString()
                });
            }
        }

        return new
        {
            label,
            fileCount = files.Count,
            sourceSetSha256,
            errorCount = errors.Count,
            errors = errors.Take(5).Select(e => new
            {
                id = e.Id,
                file = e.Location.SourceTree?.FilePath,
                line = e.Location.IsInSource ? e.Location.GetLineSpan().StartLinePosition.Line + 1 : 0,
                message = e.GetMessage()
            }).ToList(),
            rootTypes = rootTypes.Select(t => new
            {
                display = t.ToDisplayString(),
                kind = t.TypeKind.ToString(),
                declaringFiles = t.DeclaringSyntaxReferences.Select(r => r.SyntaxTree.FilePath)
                    .Distinct(StringComparer.Ordinal).OrderBy(p => p, StringComparer.Ordinal).ToList(),
                baseType = t.BaseType == null ? null : Symbol(t.BaseType),
                baseChain = BaseChain(t)
            }).ToList(),
            sites,
            otherOccurrences = others
        };
    }

    static string RightmostName(ExpressionSyntax expression) => RightmostIdentifier(expression)?.Identifier.ValueText;

    static IdentifierNameSyntax RightmostIdentifier(ExpressionSyntax expression)
    {
        switch (expression)
        {
            case IdentifierNameSyntax id: return id;
            case MemberAccessExpressionSyntax m: return m.Name as IdentifierNameSyntax;
            case AliasQualifiedNameSyntax a: return a.Name as IdentifierNameSyntax;
            default: return null;
        }
    }

    // True when this identifier is the receiver part of a member access that Report already listed as a site.
    static bool IsCountedReceiver(IdentifierNameSyntax id)
    {
        if (id.Parent is MemberAccessExpressionSyntax direct && direct.Expression == id) return true;
        return id.Parent is MemberAccessExpressionSyntax named && named.Name == id
            && named.Parent is MemberAccessExpressionSyntax outer && outer.Expression == named;
    }

    static List<string> BaseChain(INamedTypeSymbol type)
    {
        var chain = new List<string>();
        for (var t = type.BaseType; t != null; t = t.BaseType) chain.Add(t.ToDisplayString());
        return chain;
    }

    static object Describe(SymbolInfo info) => new
    {
        symbol = info.Symbol == null ? null : Symbol(info.Symbol),
        candidateReason = info.CandidateReason.ToString(),
        candidates = info.CandidateSymbols.Select(Symbol).ToList()
    };

    static object Symbol(ISymbol symbol) => new
    {
        kind = symbol.Kind.ToString(),
        display = symbol.ToDisplayString(),
        containingType = symbol.ContainingType?.ToDisplayString(),
        containingAssembly = symbol.ContainingAssembly?.Name,
        isMetadata = symbol.DeclaringSyntaxReferences.Length == 0 && symbol.Locations.All(l => l.IsInMetadata),
        isTupleElement = symbol is IFieldSymbol f && f.CorrespondingTupleField != null
    };
}
