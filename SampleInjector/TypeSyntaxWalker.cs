using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SampleInjector
{
	public class TypeSyntaxWalker : CSharpSyntaxWalker
	{
		private readonly string typeName;

		public TypeSyntaxWalker(string typeName)
		{
			this.typeName = typeName;
		}

		public bool ClassExists { get; private set; }

		public override void VisitClassDeclaration(ClassDeclarationSyntax node)
		{
			if(node.Identifier.ValueText == this.typeName)
				this.ClassExists = true;
			base.VisitClassDeclaration(node);
		}
	}
}
