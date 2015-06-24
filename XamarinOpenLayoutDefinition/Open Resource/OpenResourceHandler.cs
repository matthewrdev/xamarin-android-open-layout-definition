using System;
using Mono.TextEditor;
using MonoDevelop.Ide.TypeSystem;
using ICSharpCode.NRefactory.Semantics;
using MonoDevelop.Ide;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using System.Threading;
using MonoDevelop.Core;
using MonoDevelop.Ide.CodeCompletion;
using MonoDevelop.Components;
using MonoDevelop.CodeGeneration;
using Mono.Addins;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Components.Commands;
using MonoDevelop.MonoDroid;



namespace XamarinOpenLayoutDefinition
{
	public class OpenResourceHandler : CommandHandler
	{
		protected override void Run ()
		{
			Document doc = IdeApp.Workbench.ActiveDocument;  
			var editor = doc.GetContent<ITextEditorDataProvider> ();
			if (doc != null 
				&& editor != null 
				&& doc.Project is MonoDroidProject)
			{
				var data = editor.GetTextEditorData ();
				var offset = data.Caret.Offset;


				var loc = new DocumentLocation (data.Caret.Line, data.Caret.Column);

				ResolveResult result;
				AstNode node;
				if (!doc.TryResolveAt (loc, out result, out node)) {
					return;
				}

				var memberResolve = result as MemberResolveResult;
				if (memberResolve == null) {
					return;
				}

				if (ResourceHelper.IsResourcesMemberField(memberResolve)) {
					
					MonoDevelop.Projects.Project project;
					if (memberResolve.Type.TryGetSourceProject (out project))
					{
						var androidProject = project as MonoDroidProject;
						var fileNames = ResourceHelper.ResolveToFilenames (memberResolve, androidProject, ResourceFolders.LAYOUT);

						if (fileNames.Count == 1) {
							IdeApp.Workbench.OpenDocument(new FileOpenInformation(fileNames[0], project));
						} else if (fileNames.Count > 1) {
							OpenAutoOpenWindow (doc, data, project, fileNames);
						} else {
							Console.WriteLine ("Failed to resolve the layout");
						}
					}


				}

				//info.Enabled = ResourceHelper.IsAssetsMemberField(memberResolve) || ResourceHelper.IsResourcesMemberField(memberResolve);
			}
		}

		void OpenAutoOpenWindow (Document doc, TextEditorData textEditor, MonoDevelop.Projects.Project project, System.Collections.Generic.List<string> fileNames)
		{
			var completionWidget = doc.GetContent<ICompletionWidget> ();
			if (completionWidget == null)
				return;
			
			CodeCompletionContext completionContext = completionWidget.CreateCodeCompletionContext (textEditor.Caret.Offset);
			GUI.OpenLayoutWindow.ShowCompletionWindow (completionContext, fileNames, (r) => {
				IdeApp.Workbench.OpenDocument(new FileOpenInformation(r, project));
			});
		}

		protected override void Update (CommandInfo info)
		{
			Document doc = IdeApp.Workbench.ActiveDocument;  
			var editor = doc.GetContent<ITextEditorDataProvider> ();
			info.Enabled = false;
			if (doc != null 
				&& editor != null 
				&& doc.Project is MonoDroidProject)
			{
				var data = editor.GetTextEditorData ();

				var loc = new DocumentLocation (data.Caret.Line, data.Caret.Column);

				ResolveResult result;
				AstNode node;
				if (!doc.TryResolveAt (loc, out result, out node)) {
					return;
				}

				var memberResolve = result as MemberResolveResult;
				if (memberResolve == null) {
					return;
				}

				info.Enabled = ResourceHelper.IsResourcesMemberField(memberResolve);
			}
		}   
	}
}


