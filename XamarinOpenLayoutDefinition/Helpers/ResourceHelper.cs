using System;
using System.IO;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Semantics;
using MonoDevelop.MonoDroid;
using System.Linq;

namespace XamarinOpenLayoutDefinition
{
	public static class ResourceFolders
	{
		public const string ANIMATOR = "animator";
		public const string ANIM = "anim";
		public const string COLOR = "color";
		public const string DRAWABLE = "drawable";
		public const string MIPMAP = "mipmap";
		public const string LAYOUT = "layout";
		public const string MENU = "menu";
		public const string RAW = "raw";
		public const string VALUES = "values";
		public const string XML = "xml";
	}

	public static class ResourceClassifier
	{
		public const string Layout = "layout";
		public const string Asset = "asset";
		public const string String = "string";
		public const string Dimen = "dimen";
		public const string Drawable = "drawable";
		public const string Anim = "anim";
		public const string Style = "style";
		public const string Menu = "menu";
		public const string Attribute = "attribute";
		public const string Id = "id";
		public const string Bool = "bool";
		public const string Color = "color";
		public const string Integer = "integer";
		public const string Array = "array";

		public static readonly Dictionary<string, ResourceCategory> Conversions = new Dictionary<string, ResourceCategory>()
		{
			{Layout, ResourceCategory.Layout},
			{Asset, ResourceCategory.Asset},
			{String, ResourceCategory.String},
			{Dimen, ResourceCategory.Dimen},
			{Drawable, ResourceCategory.Drawable},
			{Anim, ResourceCategory.Anim},
			{Style, ResourceCategory.Style},
			{Menu, ResourceCategory.Menu},
			{Attribute, ResourceCategory.Attribute},
			{Id, ResourceCategory.Id},
			{Bool, ResourceCategory.Boolean},
			{Color, ResourceCategory.Color},
			{Integer, ResourceCategory.Integer},
			{Array, ResourceCategory.Array},

			// TODO: Review the list of resource types in the docs and update this.
		};
	}

	public enum ResourceCategory
	{
		Layout,
		Asset,
		String,
		Dimen,
		Drawable,
		Anim,
		Style,
		Menu,
		Attribute,
		Id,
		Boolean,
		Color,
		Integer,
		Array,

		Unknown,
	}

	public class ResourceHelper
	{
		public static ResourceCategory MapToResourceCategory(string resourceCategory)
		{
			if (ResourceClassifier.Conversions.ContainsKey(resourceCategory.ToLower())) {
				return ResourceClassifier.Conversions [resourceCategory.ToLower ()];
			}

			return ResourceCategory.Unknown;
		}

		public static List<string> ResolveToFilenames(MemberResolveResult member, MonoDroidProject androidProject, string resolveCategory)
		{
			FileInfo fi = new FileInfo (androidProject.AndroidResgenFile.FullPath);

			var directories = Directory.GetDirectories (fi.Directory.FullName);

			List<DirectoryInfo> layoutFolders = new List<DirectoryInfo> ();
			foreach (var d in directories) {
				if (d.ToLower().Contains(resolveCategory)) {
					layoutFolders.Add (new DirectoryInfo(d));
				}
			}

			List<string> resolvedLayouts = new List<string> ();

			foreach (var folder in layoutFolders)
			{
				// Get all the resources directories;
				var r = androidProject.GetAndroidResources (folder.Name); // Also get the land, portrait etc...

				resolvedLayouts.AddRange(from l in r
					where l.Key == member.Member.Name
					select l.Value.FilePath.FullPath.ToString ());
			}

			return resolvedLayouts;
		}

		public static List<string> ResolveToResourceDirectories(string directoryName, MonoDroidProject androidProject)
		{
			FileInfo fi = new FileInfo (androidProject.AndroidResgenFile.FullPath);

			var directories = Directory.GetDirectories (fi.Directory.FullName);

			List<string> layoutFolders = new List<string> ();
			foreach (var d in directories) {
				if (d.ToLower().Contains(directoryName)) {
					layoutFolders.Add ((new DirectoryInfo(d)).FullName);
				}
			}

			return layoutFolders;
		}

		public static string ResourcesFolderPath(MonoDroidProject androidProject)
		{
			var resgenFile = new FileInfo(androidProject.AndroidResgenFile);
			return resgenFile.Directory.FullName;
		}

		public static bool IsResourcesMemberField(MemberResolveResult memberResult)
		{
			MonoDroidProject monoDroidProject = AndroidProjectHelpers.ResolveAndroidProject ();

			return (memberResult.TargetResult.Type.DeclaringType.FullName == monoDroidProject.DefaultNamespace + ".Resource");
		}
	}
}

