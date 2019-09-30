using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace SampleInjector.Extensions
{
	public static class INamedTypeSymbolExtensions
	{
		public static string FullClassName(this INamedTypeSymbol namedTypeSymbol)
		{
			string identifierName = namedTypeSymbol.Name;
			if (namedTypeSymbol.ContainingNamespace != null)
			{
				return $"{namedTypeSymbol.ContainingNamespace.Name.ToString()}.{identifierName}";
			}

			return identifierName;
		}
	}
}
