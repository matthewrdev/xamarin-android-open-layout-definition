using MonoDevelop.MonoDroid;
using MonoDevelop.Ide;

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
