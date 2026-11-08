// Objective CTA Roslyn symbol reporter (#328). Source/offline approximation only: NOT Unity compilation.
//
// This tool does not decide anything. It compiles the player-like C# source set (every Assets/**/*.cs outside
// Editor folders) at C# 9 against an explicit, hash-verified reference list, then REPORTS what Roslyn bound:
//   * the compile errors (a source set that does not compile is never accepted by the Python rules),
//   * every type named ScenarioOneUiRoot with its declaring files and resolved base type, and
//   * every member access in the root's declaring files whose receiver is spelled `Time` or `DisplayStyle`,
//     with the semantic symbol of the receiver and of the member, and any identifier use outside such a receiver,
//   * every candidate Objectives visibility assignment with its enclosing callable and method (#330/A329-01:
//     the Python rules require the real `Update()` method site, not a same-string statement elsewhere), and
//   * the root's members named `Update`, so their shape can be required fail-closed.
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
    const string UpdateMethodName = "Update";
    // The reviewed Objectives panel visibility assignment (#330/A329-01). The text is normalised before
    // comparison so layout and a harmless `this.`/`global::` qualification do not change the shape.
    const string UpdateSiteLeft = "learning.Root.style.display";
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

        // #330 (A329-01): report every candidate Objectives visibility assignment with the callable that
        // encloses it and the enclosing method, so the Python rules can bind the statement to the actual
        // Update() member instead of accepting a same-string statement in an uncalled method.
        var updateSites = new List<object>();
        foreach (string file in siteFiles)
        {
            SyntaxTree tree = trees[file];
            SemanticModel model = compilation.GetSemanticModel(tree);
            foreach (var assignment in tree.GetRoot().DescendantNodes().OfType<AssignmentExpressionSyntax>()
                         .Where(a => a.IsKind(SyntaxKind.SimpleAssignmentExpression) && IsObjectivesVisibilityUpdate(a)))
            {
                var conditional = assignment.Right as ConditionalExpressionSyntax;
                updateSites.Add(new
                {
                    file,
                    line = assignment.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                    text = assignment.ToString(),
                    leftText = NormalizedLeft(assignment.Left),
                    leftReceiver = Describe(model.GetSymbolInfo(LeftmostIdentifier(assignment.Left))),
                    whenTrue = conditional == null ? null : ArmFact(model, conditional.WhenTrue),
                    whenFalse = conditional == null ? null : ArmFact(model, conditional.WhenFalse),
                    objectives = conditional == null
                        ? new List<object>()
                        : conditional.Condition.DescendantNodesAndSelf().OfType<MemberAccessExpressionSyntax>()
                            .Where(m => IsSpelled(m, "UiScreen", "Objectives")).Select(m => ArmFact(model, m)).ToList(),
                    callable = EnclosingCallable(model, assignment),
                    method = EnclosingMethod(model, assignment)
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
                baseChain = BaseChain(t),
                updateMembers = t.GetMembers(UpdateMethodName).Select(MemberFact).ToList()
            }).ToList(),
            sites,
            updateSites,
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

    // ---- #330 (A329-01): Objectives visibility update and its enclosing method --------------------------

    // Whitespace/`@`-insensitive text of an assignment target, with a leading `this.` or `global::`
    // qualifier stripped: layout and harmless qualification must not change the reviewed shape.
    static string NormalizedLeft(ExpressionSyntax expression)
    {
        string text = System.Text.RegularExpressions.Regex.Replace(expression.ToString(), @"[\s@]", "");
        if (text.StartsWith("this.")) text = text.Substring("this.".Length);
        if (text.StartsWith("global::")) text = text.Substring("global::".Length);
        return text;
    }

    // The receiver identifier of a member-access chain (`learning` in learning.Root.style.display).
    static IdentifierNameSyntax LeftmostIdentifier(ExpressionSyntax expression)
    {
        ExpressionSyntax current = expression;
        while (true)
        {
            if (current is MemberAccessExpressionSyntax access)
            {
                if (access.Expression is ThisExpressionSyntax) return access.Name as IdentifierNameSyntax;
                current = access.Expression;
            }
            else if (current is AliasQualifiedNameSyntax qualified) return qualified.Name as IdentifierNameSyntax;
            else return current as IdentifierNameSyntax;
        }
    }

    static bool IsSpelled(MemberAccessExpressionSyntax access, string receiver, string member) =>
        access.IsKind(SyntaxKind.SimpleMemberAccessExpression)
        && RightmostName(access.Expression) == receiver && access.Name.Identifier.ValueText == member;

    static bool IsArm(ExpressionSyntax expression, string member) =>
        expression is MemberAccessExpressionSyntax access && IsSpelled(access, "DisplayStyle", member);

    // A candidate for the Objectives visibility update: the reviewed assignment target, or the reviewed
    // Objectives conditional (`? DisplayStyle.Flex : DisplayStyle.None` with an `UiScreen.Objectives` test).
    static bool IsObjectivesVisibilityUpdate(AssignmentExpressionSyntax assignment)
    {
        if (NormalizedLeft(assignment.Left) == UpdateSiteLeft) return true;
        if (!(assignment.Right is ConditionalExpressionSyntax conditional)) return false;
        return IsArm(conditional.WhenTrue, "Flex") && IsArm(conditional.WhenFalse, "None")
            && conditional.Condition.DescendantNodesAndSelf().OfType<MemberAccessExpressionSyntax>()
                .Any(m => IsSpelled(m, "UiScreen", "Objectives"));
    }

    // One `Receiver.Member` occurrence (an arm of the conditional, or the UiScreen.Objectives test).
    // A shape that is not such an access reports null and is rejected fail-closed by the Python rules.
    static object ArmFact(SemanticModel model, ExpressionSyntax expression)
    {
        if (!(expression is MemberAccessExpressionSyntax access)) return null;
        return new
        {
            text = access.ToString(),
            member = access.Name.Identifier.ValueText,
            receiver = Describe(model.GetSymbolInfo(access.Expression)),
            memberSymbol = Describe(model.GetSymbolInfo(access))
        };
    }

    // Shape of a callable/member: kind, name and the symbol facts the Python rules require fail-closed.
    static object CallableFact(string kind, string name, IMethodSymbol symbol) => new
    {
        kind,
        name,
        display = symbol == null ? null : symbol.ToDisplayString(),
        containingType = symbol == null ? null : symbol.ContainingType?.ToDisplayString(),
        parameterCount = symbol == null ? 0 : symbol.Parameters.Length,
        returnType = symbol == null ? null : (symbol.ReturnsVoid ? "void" : symbol.ReturnType.ToDisplayString()),
        isGeneric = symbol != null && symbol.IsGenericMethod
    };

    // The nearest enclosing callable: a local function or lambda boundary is NOT the Update() method,
    // even when its text lies inside Update's body.
    static object EnclosingCallable(SemanticModel model, SyntaxNode node)
    {
        foreach (SyntaxNode ancestor in node.Ancestors())
        {
            if (ancestor is MethodDeclarationSyntax method)
                return CallableFact("MethodDeclaration", method.Identifier.ValueText, model.GetDeclaredSymbol(method) as IMethodSymbol);
            if (ancestor is LocalFunctionStatementSyntax local)
                return CallableFact("LocalFunction", local.Identifier.ValueText, model.GetDeclaredSymbol(local) as IMethodSymbol);
            if (ancestor is AnonymousFunctionExpressionSyntax)
                return CallableFact("AnonymousFunction", null, null);
            if (ancestor is ConstructorDeclarationSyntax constructor)
                return CallableFact("Constructor", constructor.Identifier.ValueText, model.GetDeclaredSymbol(constructor) as IMethodSymbol);
            if (ancestor is BaseMethodDeclarationSyntax || ancestor is BasePropertyDeclarationSyntax)
                return CallableFact("OtherMember", null, null);
        }
        return null;
    }

    static object EnclosingMethod(SemanticModel model, SyntaxNode node)
    {
        var method = node.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
        return method == null
            ? null
            : CallableFact("MethodDeclaration", method.Identifier.ValueText, model.GetDeclaredSymbol(method) as IMethodSymbol);
    }

    // A root member named `Update`: any kind, so a property/field/generic shape fails closed in the rules.
    static object MemberFact(ISymbol symbol)
    {
        var method = symbol as IMethodSymbol;
        return new
        {
            kind = symbol.Kind.ToString(),
            name = symbol.Name,
            display = symbol.ToDisplayString(),
            containingType = symbol.ContainingType?.ToDisplayString(),
            parameterCount = method == null ? 0 : method.Parameters.Length,
            returnType = method == null ? null : (method.ReturnsVoid ? "void" : method.ReturnType.ToDisplayString()),
            isGeneric = method != null && method.IsGenericMethod
        };
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
