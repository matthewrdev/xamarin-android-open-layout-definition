using System;
using MonoDevelop.Refactoring;
using MonoDevelop.MonoDroid;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.CSharp;
using System.Collections.Generic;
using MonoDevelop.Ide;
using MonoDevelop.Refactoring.Rename;
using MonoDevelop.Ide.ProgressMonitoring;
using MonoDevelop.Ide.FindInFiles;
using MonoDevelop.Core;
using System.Linq;
using System.IO;

namespace XamarinOpenLayoutDefinition
{
	public class AndroidProjectHelpers
	{
		public static MonoDroidProject ResolveAndroidProject()
		{
			var doc = IdeApp.Workbench.ActiveDocument;
			if (doc == null || doc.ParsedDocument == null)
				return null;
			
			var project = doc.Project;
			MonoDroidProject monoDroidProject = project as MonoDroidProject;
			return monoDroidProject;
		}
	}
	
}
