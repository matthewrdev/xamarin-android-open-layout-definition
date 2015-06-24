using System;
using Mono.Addins;
using Mono.Addins.Description;

[assembly:Addin (
	"XamarinOpenLayoutDefinition", 
	Namespace = "XamarinOpenLayoutDefinition",
	Version = "0.0.1"
)]

[assembly:AddinName ("Xamarin.Android Open Layout Definition")]
[assembly:AddinCategory ("IDE extensions")]
[assembly:AddinDescription ("A small tool to jump from Resource.Layout.* reference in code to the layout file. Use [Command]+[Alt]+[R] to jump to the layout file.\n    \nThis is a sample application for a wider set of refactoring tools for Xamarin.Android. Please submit bug reports to:\nhttps://github.com/matthew-ch-robbins/xamarin-android-open-layout-definition\n    \nTo help shape the direction of this project, connect with me on LinkedIn and have a chat:\nhttps://au.linkedin.com/pub/matthew-robbins/17/8a7/139")]
[assembly:AddinUrl("https://github.com/matthew-ch-robbins/xamarin-android-open-layout-definition")]
[assembly:AddinAuthor ("Matthew Robbins")]
