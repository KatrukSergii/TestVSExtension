﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;

namespace SampleInjector
{
    public class SampleInjector : ISampleInjector
    {
        private List<string> packages = new List<string>
        {
            "System.Configuration.ConfigurationManager",
            "DocuSign.eSign.dll"
        };

        public async System.Threading.Tasks.Task InjectSample(string sampleId, string solutionFilePath, IAsyncServiceProvider serviceProvider)
        {

            string gitPath = @"https://github.com/docusign/eg-01-csharp-jwt-core/archive/master.zip";
            string zipPath = Path.Combine(Path.GetTempPath(), "master.zip");
            string projTempPath = Path.Combine(Path.GetTempPath(), "master");
            try
            {
                this.TryDeleteTempFiles(zipPath, projTempPath);

                using (var client = new HttpClient())
                {
                    var contents = await client.GetByteArrayAsync(gitPath);
                    File.WriteAllBytes(zipPath, contents);
                }

                ZipFile.ExtractToDirectory(zipPath, projTempPath);
                var getTemplateSolutionFile = Directory.GetFiles(projTempPath, "*.sln", SearchOption.AllDirectories).FirstOrDefault();

                var templateMsWorkspace = MSBuildWorkspace.Create();
                var templateSolution = await templateMsWorkspace.OpenSolutionAsync(getTemplateSolutionFile);
                var templateProject = templateSolution.Projects.FirstOrDefault();


                var vsHelper = new VSHelper(serviceProvider);
                var selectedVsProject = vsHelper.GetSelectedProject();
                foreach (var package in this.packages)
                    vsHelper.SetupNugetPakadge(selectedVsProject, package);

                selectedVsProject.Save();

                var msWorkspace = MSBuildWorkspace.Create();
                List<string> ignorDocumentNames = new List<string>
                {
                    "Program.cs",
                    "Assembly"
                };
                var selectedWsProject = await this.RecreateActiveProject(msWorkspace, solutionFilePath, selectedVsProject.FullName);
                var tdoc1 = selectedWsProject.Documents.FirstOrDefault(doc2 => doc2.Name == "Program.cs");
                var troot = await tdoc1.GetSyntaxRootAsync();
                var nameSpace = troot.DescendantNodes().OfType<NamespaceDeclarationSyntax>().FirstOrDefault().Name.ToFullString();

                foreach (var doc in templateProject.Documents.Where(doc => ignorDocumentNames.All(name => !doc.Name.Contains(name))))
                {
                    SyntaxTree stree = CSharpSyntaxTree.ParseText(await doc.GetTextAsync());
                    SyntaxNode r = stree.GetCompilationUnitRoot();
                    var usings = (r as CompilationUnitSyntax).Usings.ToArray();
                    NamespaceDeclarationSyntax namespaceNode = r.DescendantNodes().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
                    r = namespaceNode.WithName(SyntaxFactory.ParseName(nameSpace));
                    r = (r as NamespaceDeclarationSyntax).AddUsings(usings);
                    var newDocument = selectedWsProject.AddDocument(doc.Name, r);
                    msWorkspace.TryApplyChanges(newDocument.Project.Solution);
                    selectedWsProject = await this.RecreateActiveProject(msWorkspace, solutionFilePath, selectedVsProject.FullName);
                }
                var actualWsProject = await this.RecreateActiveProject(msWorkspace, solutionFilePath, selectedVsProject.FullName);

                var templateCompilation = await templateProject.GetCompilationAsync();
                var projectCompilation = await actualWsProject.GetCompilationAsync();

                ClassDeclarationSyntax templateProgramClass = templateCompilation.SyntaxTrees.SelectMany(tre => tre.GetRoot().DescendantNodes().
                    OfType<ClassDeclarationSyntax>().Where(d => d.Identifier.ValueText == "Program")).FirstOrDefault();
                MethodDeclarationSyntax templateMainMethod = templateProgramClass.Members.OfType<MethodDeclarationSyntax>().FirstOrDefault(m => m.Identifier.ValueText == "Main");
                MethodDeclarationSyntax testClientMethod = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("void"), " TestApi")
                    .WithBody(templateMainMethod.Body);
                var doc1 = actualWsProject.Documents.FirstOrDefault(doc2 => doc2.Name == "Program.cs");
                var root = await doc1.GetSyntaxRootAsync();

                ClassDeclarationSyntax programClass = root.DescendantNodes().
                    OfType<ClassDeclarationSyntax>().FirstOrDefault(d => d.Identifier.ValueText == "Program");
                
                SyntaxNode node = Formatter.Format(testClientMethod, Formatter.Annotation, msWorkspace);
                ClassDeclarationSyntax newProgramClass = programClass.AddMembers(node as MethodDeclarationSyntax);

                SyntaxNode newRoot = root.ReplaceNode(programClass, newProgramClass);
                newRoot = Formatter.Format(newRoot, Formatter.Annotation, msWorkspace);
                var compRoot = newRoot.SyntaxTree.GetCompilationUnitRoot();
                compRoot = compRoot.RemoveNodes(compRoot.Usings, SyntaxRemoveOptions.KeepNoTrivia);
                SyntaxList<UsingDirectiveSyntax> newUsings = compRoot.Usings.AddRange(templateMainMethod.SyntaxTree.GetCompilationUnitRoot().Usings);
                compRoot = compRoot.AddUsings(newUsings.ToArray());
                msWorkspace.TryApplyChanges(doc1.WithSyntaxRoot(compRoot).Project.Solution);
            }
            finally
            {
                this.TryDeleteTempFiles(zipPath, projTempPath);
            }
        }

        private async Task<Project> RecreateActiveProject(MSBuildWorkspace msWorkspace, string solutionFilePath, string selectedProjectFullName)
        {
            var solution = await msWorkspace.OpenSolutionAsync(solutionFilePath);
            return solution.Projects.FirstOrDefault(proj => proj.FilePath == selectedProjectFullName);

        }

        private void TryDeleteTempFiles(string zipPath, string projTempPath)
        {
            if (File.Exists(zipPath))
                File.Delete(zipPath);

            if (Directory.Exists(projTempPath))
                Directory.Delete(projTempPath, true);
        }
    }
}
