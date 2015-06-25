using System;
using System.Linq;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using MonoDevelop.Ide;

namespace XamarinOpenLayoutDefinition
{
	
	public class AstHelper
	{
		public AstHelper ()
		{
		}

		public static AstNode ResolveNodeEncapsulatingClass(AstNode node)
		{
			var parent = node.Parent;
			while (parent != null) {
				if (parent is TypeDeclaration) {
					break;
				}
				parent = parent.Parent;
			}

			return parent as TypeDeclaration;
		}

		public static bool IsClassDerivedFrom(TypeDeclaration typeToCheck, string[] supportedBaseTypes)
		{
			var doc = IdeApp.Workbench.ActiveDocument;
			if (doc == null || doc.ParsedDocument == null)
				return false;

			var ast = doc.ParsedDocument.GetAst<SyntaxTree> ();
			if (ast == null)
				return false;

			var parsedFile = doc.ParsedDocument.ParsedFile as CSharpUnresolvedFile;
			if (parsedFile == null)
				return false;
			
			bool isDerivedFrom = false;


			foreach (var baseType in typeToCheck.BaseTypes) {
				var t = baseType as SimpleType;


				AstNode astNode;
				var resolve = ResolveAtLocation.Resolve(new Lazy<ICompilation>(() => doc.Compilation), parsedFile, ast, t.StartLocation, out astNode);

				if (t != null && supportedBaseTypes.Contains(t.Identifier)) {
					isDerivedFrom = true;
					break;
				}



				// Lets resolve the parent classes.
				var baseTypes = resolve.Type.GetAllBaseTypes();
				foreach (var bt in baseTypes)
				{
					DefaultResolvedTypeDefinition resolvedType = bt as DefaultResolvedTypeDefinition;
					if (resolvedType != null && supportedBaseTypes.Contains(resolvedType.FullName)) {
						isDerivedFrom = true;
						break;
					}
				}

			}

			return isDerivedFrom;
		}
	}
}

