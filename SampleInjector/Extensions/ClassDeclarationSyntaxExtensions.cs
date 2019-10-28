using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SampleInjector.Extensions
{
	public static class ClassDeclarationSyntaxExtensions
	{
		public static string FullClassName(this ClassDeclarationSyntax classDeclaration)
		{
			string identifierName = classDeclaration.Identifier.ToString();
			if (classDeclaration.Parent is NamespaceDeclarationSyntax namespaceDeclaration)
			{
				return $"{namespaceDeclaration.Name.ToString()}.{identifierName}";
			}

			return identifierName;
		}
	}
}