using System;
using System.IO;
using System.Linq;
using Buildalyzer;
using Buildalyzer.Workspaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Extension.Core
{
    public class Class1
    {
        public void Exec(string projectFilePath)
        {
            //string projectFilePath = @"C:\Users\USER\source\repos\ClassLibrary1\ClassLibrary1\ClassLibrary1.csproj";
            string projectDirectory = Path.GetDirectoryName(projectFilePath);
            var manager = new AnalyzerManager();
            ProjectAnalyzer analyzer = manager.GetProject(projectFilePath);
            using (var workspace = new AdhocWorkspace())
            {
                Project roslynProject = analyzer.AddToWorkspace(workspace);
                if (roslynProject.Language != LanguageNames.CSharp)
                    throw new NotSupportedException($"The language: {roslynProject.Language} is not supported.");

                var sourceCode = "namespace Foo{public class Foo{ }}";
                Compilation compilation = roslynProject.GetCompilationAsync().Result;
                SyntaxTree stree = CSharpSyntaxTree.ParseText(sourceCode);
                SyntaxNode root = stree.GetCompilationUnitRoot();
                UsingDirectiveSyntax[] usings = (root as CompilationUnitSyntax).Usings.ToArray();
                NamespaceDeclarationSyntax namespaceNode = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
                root = namespaceNode.WithName(SyntaxFactory.ParseName(compilation.GetEntryPoint(System.Threading.CancellationToken.None).ContainingNamespace.ToString()));
                root = (root as NamespaceDeclarationSyntax).AddUsings(usings);
                string fileName = GetFileName(root);
                fileName = Path.ChangeExtension(fileName, "cs");
                string filePath = Path.Combine(projectDirectory, fileName);
                File.WriteAllText(filePath, root.ToFullString());

                var documentId = DocumentId.CreateNewId(roslynProject.Id);
                var docInfo = DocumentInfo.Create(documentId, fileName, filePath: filePath);
                //await _addDocumentToProjectService.AddDocumentToProject(docInfo, projectFilePath).ConfigureAwait(false);
            }
        }

        private static string GetFileName(SyntaxNode root) =>
            root.DescendantNodes().OfType<TypeDeclarationSyntax>().FirstOrDefault()?.Identifier.ValueText;
    }
}
