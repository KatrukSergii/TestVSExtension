using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;

namespace SampleInjector
{
	public class SampleInjector : ISampleInjector
	{	

		public async Task InjectSample(string sampleId, string solutionFilePath)
		{
			SourceText source = SourceText.From(
    $"public class {sampleId}"+
    @"{
        public void MyMethod()
        {
        }
        public void MyMethod(int n)
        {
        }
    }");
			var msWorkspace = MSBuildWorkspace.Create();
			var solution = await msWorkspace.OpenSolutionAsync(solutionFilePath);
			
			foreach (Project project in solution.Projects)
			{
				var compilation = await project.GetCompilationAsync();
				
				foreach(var stree in compilation.SyntaxTrees)
				{
					if((await stree.GetRootAsync()).ChildNodes().OfType<ClassDeclarationSyntax>().Any(node => node.Identifier.ValueText == sampleId))
						return;
				}

				var doc = project.AddDocument(sampleId, source);
				msWorkspace.TryApplyChanges(doc.Project.Solution);
				
			}
		}
	}
}
