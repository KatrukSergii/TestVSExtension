using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using SampleInjector.Exceptions;
using SampleInjector.Extensions;
using SampleInjector.Interface;

namespace SampleInjector
{
    public class SampleInjector : ISampleInjector
    {
       
        public async Task InjectSampleAsync(string projFilePath)
        {
            //using (GitSampleProject sample = new GitSampleProject(@"https://github.com/docusign/eg-01-csharp-jwt-core/archive/master.zip"))
            {
                try 
                {
                    var templateMsWorkspace = MSBuildWorkspace.Create();
                    Project roslynProject = await templateMsWorkspace.OpenProjectAsync(projFilePath);
                    string sourceCode = "namespace Foo{public class Foo{ }}";
                    Compilation compilation = roslynProject.GetCompilationAsync().Result;
                    SyntaxTree stree = CSharpSyntaxTree.ParseText(sourceCode);
                    SyntaxNode root = stree.GetCompilationUnitRoot();
                    UsingDirectiveSyntax[] usings = (root as CompilationUnitSyntax).Usings.ToArray();
                    NamespaceDeclarationSyntax namespaceNode = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
                    root = namespaceNode.WithName(SyntaxFactory.ParseName(compilation.GetEntryPoint(CancellationToken.None).ContainingNamespace.ToString()));
                    root = (root as NamespaceDeclarationSyntax).AddUsings(usings);
                    string fileName = GetFileName(root);
                    var newDoc = roslynProject.AddDocument(fileName, root);
                    templateMsWorkspace.TryApplyChanges(newDoc.Project.Solution);
                }
                catch(SampleInitException sampleException)
                {
                    Debug.Fail(sampleException.Message, sampleException.InnerException.ToString());
                }
                catch(Exception exception)
                {
                    Debug.Fail(exception.Message, exception.ToString());
                }
            }
            
        }

        private static string GetFileName(SyntaxNode root) =>
            root.DescendantNodes().OfType<TypeDeclarationSyntax>().FirstOrDefault()?.Identifier.ValueText;

        private async Task<Project> RecreateActiveProject(MSBuildWorkspace msWorkspace, string solutionFilePath, string selectedProjectFullName)
        {
            var solution = await msWorkspace.OpenSolutionAsync(solutionFilePath);
            return solution.Projects.FirstOrDefault(proj => proj.FilePath == selectedProjectFullName);

		}

        //var msWorkspace = MSBuildWorkspace.Create();
        //List<string> ignorDocumentNames = new List<string>
        //        {
        //            "Program.cs",
        //            "Assembly"
        //        };
        //var selectedWsProject = await this.RecreateActiveProject(msWorkspace, solutionFilePath, selectedVsProject.FullName);
        //var tdoc1 = selectedWsProject.Documents.FirstOrDefault(doc2 => doc2.Name == "Program.cs");
        //var troot = await tdoc1.GetSyntaxRootAsync();
        //var nameSpace = troot.DescendantNodes().OfType<NamespaceDeclarationSyntax>().FirstOrDefault().Name.ToFullString();

        //            foreach (var doc in templateProject.Documents.Where(doc => ignorDocumentNames.All(name => !doc.Name.Contains(name))))
        //            {
        //                SyntaxTree stree = CSharpSyntaxTree.ParseText(await doc.GetTextAsync());
        //SyntaxNode r = stree.GetCompilationUnitRoot();
        //var usings = (r as CompilationUnitSyntax).Usings.ToArray();
        //NamespaceDeclarationSyntax namespaceNode = r.DescendantNodes().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
        //r = namespaceNode.WithName(SyntaxFactory.ParseName(nameSpace));
        //                r = (r as NamespaceDeclarationSyntax).AddUsings(usings);
        //var newDocument = selectedWsProject.AddDocument(doc.Name, r);
        //msWorkspace.TryApplyChanges(newDocument.Project.Solution);
        //                selectedWsProject = await this.RecreateActiveProject(msWorkspace, solutionFilePath, selectedVsProject.FullName);
        //            }
        //var actualWsProject = await this.RecreateActiveProject(msWorkspace, solutionFilePath, selectedVsProject.FullName);

        //var templateCompilation = await templateProject.GetCompilationAsync();
        //var projectCompilation = await actualWsProject.GetCompilationAsync();

        //ClassDeclarationSyntax templateProgramClass = templateCompilation.SyntaxTrees.SelectMany(tre => tre.GetRoot().DescendantNodes().
        //    OfType<ClassDeclarationSyntax>().Where(d => d.Identifier.ValueText == "Program")).FirstOrDefault();
        //MethodDeclarationSyntax templateMainMethod = templateProgramClass.Members.OfType<MethodDeclarationSyntax>().FirstOrDefault(m => m.Identifier.ValueText == "Main");
        //MethodDeclarationSyntax testClientMethod = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("void"), " TestApi")
        //    .WithBody(templateMainMethod.Body);
        //var doc1 = actualWsProject.Documents.FirstOrDefault(doc2 => doc2.Name == "Program.cs");
        //var root = await doc1.GetSyntaxRootAsync();

        //ClassDeclarationSyntax programClass = root.DescendantNodes().
        //    OfType<ClassDeclarationSyntax>().FirstOrDefault(d => d.Identifier.ValueText == "Program");

        //SyntaxNode node = Formatter.Format(testClientMethod, Formatter.Annotation, msWorkspace);
        //ClassDeclarationSyntax newProgramClass = programClass.AddMembers(node as MethodDeclarationSyntax);

        //SyntaxNode newRoot = root.ReplaceNode(programClass, newProgramClass);
        //newRoot = Formatter.Format(newRoot, Formatter.Annotation, msWorkspace);
        //            var compRoot = newRoot.SyntaxTree.GetCompilationUnitRoot();
        //compRoot = compRoot.RemoveNodes(compRoot.Usings, SyntaxRemoveOptions.KeepNoTrivia);
        //            SyntaxList<UsingDirectiveSyntax> newUsings = compRoot.Usings.AddRange(templateMainMethod.SyntaxTree.GetCompilationUnitRoot().Usings);
        //compRoot = compRoot.AddUsings(newUsings.ToArray());
        //            msWorkspace.TryApplyChanges(doc1.WithSyntaxRoot(compRoot).Project.Solution);

    }
}
