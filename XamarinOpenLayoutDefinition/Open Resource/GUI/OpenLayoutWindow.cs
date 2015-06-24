using System;
using MonoDevelop.Components;
using Gtk;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Refactoring;
using System.Collections.Generic;
using MonoDevelop.Ide;
using Mono.TextEditor.PopupWindow;

namespace XamarinOpenLayoutDefinition.GUI
{
	partial class OpenLayoutWindow : Gtk.Window
	{
		TreeStore optionsStore = new TreeStore (typeof(Xwt.Drawing.Image), typeof (string), typeof(string));

		class CustomTreeView : TreeView
		{
			protected override bool OnKeyPressEvent (Gdk.EventKey evnt)
			{
				if (evnt.Key == Gdk.Key.Return || evnt.Key == Gdk.Key.KP_Enter) {
					OnSubmit (EventArgs.Empty);
					return true;
				}
				if (evnt.Key == Gdk.Key.Escape) {
					OnCancel (EventArgs.Empty);
					return true;
				}
				return base.OnKeyPressEvent (evnt);
			}

			protected virtual void OnSubmit (EventArgs e)
			{
				if (Submit != null)
					Submit (this, e);
			}
			public event EventHandler Submit;

			protected virtual void OnCancel (EventArgs e)
			{
				if (Cancel != null)
					Cancel (this, e);
			}
			public event EventHandler Cancel;
		}

		CustomTreeView treeviewGenerateActions = new CustomTreeView ();


		Action<string> _onSubmitted;

		string _selectedItem;


		OpenLayoutWindow (MonoDevelop.Ide.CodeCompletion.CodeCompletionContext completionContext, Action<string> onSubmitted) : base(Gtk.WindowType.Toplevel)
		{
			_onSubmitted = onSubmitted;

			this.Build ();

			scrolledwindow1.Child = treeviewGenerateActions;
			scrolledwindow1.ShowAll ();

			treeviewGenerateActions.Cancel += delegate {
				Destroy ();
			};
			treeviewGenerateActions.Submit += delegate {
				if (String.IsNullOrEmpty(_selectedItem) == false) {
					_onSubmitted(_selectedItem);
				}
				Destroy ();
			};

			WindowTransparencyDecorator.Attach (this);

			treeviewGenerateActions.HeadersVisible = false;
			treeviewGenerateActions.Model = optionsStore;
			TreeViewColumn column = new TreeViewColumn ();
			var pixbufRenderer = new CellRendererImage ();
			column.PackStart (pixbufRenderer, false);
			column.AddAttribute (pixbufRenderer, "image", 0);

			CellRendererText textRenderer = new CellRendererText ();
			column.PackStart (textRenderer, true);
			column.AddAttribute (textRenderer, "text", 1);
			column.Expand = true;
			treeviewGenerateActions.AppendColumn (column);

			treeviewGenerateActions.Selection.Changed += TreeviewGenerateActionsSelectionChanged;
			this.Remove (this.vbox1);
			BorderBox messageArea = new BorderBox ();
			messageArea.Add (vbox1);
			this.Add (messageArea);
			this.ShowAll ();

			int x = completionContext.TriggerXCoord;
			int y = completionContext.TriggerYCoord;

			int w, h;
			GetSize (out w, out h);

			int myMonitor = Screen.GetMonitorAtPoint (x, y);
			Gdk.Rectangle geometry = DesktopService.GetUsableMonitorGeometry (Screen, myMonitor);

			if (x + w > geometry.Right)
				x = geometry.Right - w;

			if (y + h > geometry.Bottom)
				y = y - completionContext.TriggerTextHeight - h;

			Move (x, y);
		}

		void Populate (List<string> inputOptions)
		{
			foreach (var op in inputOptions)
				optionsStore.AppendValues (null, FormatToShortPath(op), op);

			TreeIter iter;
			if (optionsStore.GetIterFirst (out iter))
				treeviewGenerateActions.Selection.SelectIter (iter);
			
			treeviewGenerateActions.GrabFocus ();
		}

		void TreeviewGenerateActionsSelectionChanged (object sender, EventArgs e)
		{
			TreeIter iter;
			if (treeviewGenerateActions.Selection.GetSelected (out iter)) {
				_selectedItem = (string)optionsStore.GetValue (iter, 2);
				labelDescription.Text = (string)optionsStore.GetValue (iter, 1);
			} else {
				labelDescription.Text = "";
				_selectedItem = "";
			}
		}

		string FormatToShortPath (string item)
		{
			System.IO.FileInfo fi = new System.IO.FileInfo (item);
			return System.IO.Path.Combine(fi.Directory.Name, fi.Name);
		}

		public static void ShowCompletionWindow (MonoDevelop.Ide.CodeCompletion.CodeCompletionContext completionContext, List<string> options, Action<string> onSubmitted)
		{
			var window = new OpenLayoutWindow (completionContext, onSubmitted);
			window.Populate (options);
		}

		protected override bool OnFocusOutEvent (Gdk.EventFocus evnt)
		{
			Destroy ();
			return base.OnFocusOutEvent (evnt);
		}

		class BorderBox : HBox
		{
			public BorderBox () : base (false, 8)
			{
				BorderWidth = 3;
			}

			protected override bool OnExposeEvent (Gdk.EventExpose evnt)
			{
				Style.PaintFlatBox (Style,
					evnt.Window,
					StateType.Normal,
					ShadowType.Out,
					evnt.Area,
					this,
					"tooltip",
					Allocation.X + 1,
					Allocation.Y + 1,
					Allocation.Width - 2,
					Allocation.Height - 2);

				return base.OnExposeEvent (evnt);
			}
		}

	}
}